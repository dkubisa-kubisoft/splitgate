namespace Splitgate.Api.Functions
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Splitgate.Api.Request;
    using Azure.Data.Tables;
    using Splitgate.Api.Entities;
    using Splitgate.Api.Response;

    public class ToggleChallengeCompletion
    {
        private TableServiceClient tableServicesClient { get; set; }
        public ToggleChallengeCompletion(TableServiceClient tableServiceClient) 
        {
            this.tableServicesClient = tableServiceClient ?? throw new ArgumentNullException(nameof(tableServiceClient));
        }

        [FunctionName("ToggleChallengeCompletion")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            try 
            {
                log.LogInformation("ToggleChallengeCompletion called!");

                if (req == null) {
                    throw new ArgumentNullException(nameof(req));
                }
                if (log == null) {
                    throw new ArgumentNullException(nameof(log));
                }
                
                ToggleChallengeCompletionRequest request = new ToggleChallengeCompletionRequest();

                try 
                {
                    request = JsonConvert.DeserializeObject<ToggleChallengeCompletionRequest>(await new StreamReader(req.Body).ReadToEndAsync());

                    if (string.IsNullOrEmpty(request.ChallengeId)) 
                    {
                        return new BadRequestObjectResult(new { Message = "Invalid request. Please provide a challenge id." });
                    }
                }
                catch
                {
                    return new BadRequestObjectResult(new { Message = "Unable to deserialize request." });
                }

                var ipAddress = req.HttpContext.Connection.RemoteIpAddress.ToString();

                await this.tableServicesClient.CreateTableIfNotExistsAsync(TableNames.CompletedChallenges).ConfigureAwait(false);
                
                var tableClient = tableServicesClient.GetTableClient(TableNames.CompletedChallenges);
                
                var record = new Splitgate.Api.Entities.TableEntity 
                {
                    PartitionKey = ipAddress,
                    RowKey = request.ChallengeId
                };

                try 
                {
                    await tableClient.AddEntityAsync<Splitgate.Api.Entities.TableEntity>(record).ConfigureAwait(false);
                }
                catch 
                {
                    try 
                    {
                        await tableClient.DeleteEntityAsync(record.PartitionKey, record.RowKey).ConfigureAwait(false);
                    }
                    catch {}
                }
            
                return new OkObjectResult(new BasicResponse { Success = true });
            }
            catch(Exception ex) 
            {
                log.LogError(ex, $"Unhandled exception in PostChallenges: {ex.Message}");
                return new ObjectResult(new { Message = "Internal server error." }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}

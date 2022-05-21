namespace Splitgate.Api
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
    using System.Collections.Generic;
    using Splitgate.Api.Models;
    using Azure.Data.Tables;
    using Splitgate.Api.Entities;

    public class PostChallenges
    {
        private TableServiceClient tableServicesClient { get; set; }
        public PostChallenges(TableServiceClient tableServiceClient) 
        {
            this.tableServicesClient = tableServiceClient ?? throw new ArgumentNullException(nameof(tableServiceClient));
        }

        [FunctionName("PostChallenges")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            try 
            {
                log.LogInformation("PostChallenges called!");

                if (req == null) {
                    throw new ArgumentNullException(nameof(req));
                }
                if (log == null) {
                    throw new ArgumentNullException(nameof(log));
                }
                
                PostChallengesRequest request = new PostChallengesRequest();

                try 
                {
                    request = JsonConvert.DeserializeObject<PostChallengesRequest>(await new StreamReader(req.Body).ReadToEndAsync());

                    if (!request.Challenges.Any()) 
                    {
                        return new BadRequestObjectResult(new { Message = "Invalid request. Please provide a list of challenges." });
                    }
                }
                catch
                {
                    return new BadRequestObjectResult(new { Message = "Unable to deserialize request." });
                }

                await this.tableServicesClient.CreateTableIfNotExistsAsync(TableNames.Challenges).ConfigureAwait(false);

                var tableClient = tableServicesClient.GetTableClient(TableNames.Challenges);

                foreach( var challenge in request.Challenges) 
                {
                    var entity = new ChallengeEntity(challenge);
                    await tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey).ConfigureAwait(false);
                    await tableClient.AddEntityAsync<ChallengeEntity>(entity).ConfigureAwait(false);
                }
            
                return new OkResult();
            }
            catch(Exception ex) 
            {
                log.LogError(ex, $"Unhandled exception in PostChallenges: {ex.Message}");
                return new ObjectResult(new { Message = "Internal server error." }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}

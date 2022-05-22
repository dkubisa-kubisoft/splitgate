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
                await this.tableServicesClient.CreateTableIfNotExistsAsync(TableNames.ChallengeArchive).ConfigureAwait(false);

                var challengesTableClient = tableServicesClient.GetTableClient(TableNames.Challenges);
                var challengeArchiveTableClient = tableServicesClient.GetTableClient(TableNames.ChallengeArchive);

                foreach( var challenge in request.Challenges) 
                {
                    var entity = new ChallengeEntity(challenge);
                    await challengesTableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey).ConfigureAwait(false);
                    await challengesTableClient.AddEntityAsync<ChallengeEntity>(entity).ConfigureAwait(false);

                    try 
                    {
                        var archiveEntity = new ChallengeArchiveEntity(challenge);
                        await challengeArchiveTableClient.AddEntityAsync<ChallengeArchiveEntity>(archiveEntity).ConfigureAwait(false);
                    }
                    catch 
                    {
                        // record for today already exists in the archive, ignore
                    }
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

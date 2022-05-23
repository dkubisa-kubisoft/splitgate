namespace Splitgate.Api.Functions
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Azure;
    using Azure.Data.Tables;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Splitgate.Api.Entities;
    using Splitgate.Api.Request;
    
    public class PostChallenges
    {
        private const string FunctionName = "PostChallenges";

        private TableServiceClient tableServicesClient { get; set; }
        public PostChallenges(TableServiceClient tableServiceClient) 
        {
            this.tableServicesClient = tableServiceClient ?? throw new ArgumentNullException(nameof(tableServiceClient));
        }

        [FunctionName(FunctionName)]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            try 
            {
                log.LogInformation($"{FunctionName} called!");

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
                var challengesTableClient = tableServicesClient.GetTableClient(TableNames.Challenges);
                
                foreach( var challenge in request.Challenges) 
                {
                    var newChallenge = new ChallengeEntity(challenge);

                    ChallengeEntity existingChallenge = null;

                    try 
                    {
                        existingChallenge = (await challengesTableClient.GetEntityAsync<ChallengeEntity>("1", newChallenge.RowKey).ConfigureAwait(false))?.Value;
                    }
                    catch (RequestFailedException) {}

                    if (existingChallenge != null && existingChallenge.Description != newChallenge.Description) 
                    {
                        // The new challenge replaces an existing challenge. Purge the completions for the existing one since this one is different.
                        await PurgeCompletions(existingChallenge).ConfigureAwait(false);
                    }

                    // Archive the old challenge
                    await ArchiveChallenge(existingChallenge, challengesTableClient).ConfigureAwait(false);

                    // Add the new challenge
                    await challengesTableClient.AddEntityAsync<ChallengeEntity>(newChallenge).ConfigureAwait(false);
                }
            
                return new OkResult();
            }
            catch(Exception ex) 
            {
                log.LogError(ex, $"Unhandled exception in PostChallenges: {ex.Message}");
                return new ObjectResult(new { Message = "Internal server error." }) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        /// <summary>
        /// Archives a challenge
        /// </summary>
        /// <param name="challenge">The challenge to archive.</param>
        /// <param name="challengesTableClient">Client to the Challenges table.</param>
        private async Task ArchiveChallenge(ChallengeEntity challenge, TableClient challengesTableClient) 
        {
            try 
            {
                await this.tableServicesClient.CreateTableIfNotExistsAsync(TableNames.ChallengeArchive).ConfigureAwait(false);
                var challengeArchiveTableClient = tableServicesClient.GetTableClient(TableNames.ChallengeArchive);

                await challengeArchiveTableClient.AddEntityAsync<ChallengeArchiveEntity>(challenge.GetArchiveEntity()).ConfigureAwait(false);
            }
            catch (RequestFailedException)
            {
                // Record for today already exists in the archive, ignore
            }
            finally
            {
                // Delete the archived challenge
                await challengesTableClient.DeleteEntityAsync(challenge.PartitionKey, challenge.RowKey).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Purges the completions of an existing challenge.
        /// </summary>
        /// <param name="challenge">The challenge.</param>
        private async Task PurgeCompletions(ChallengeEntity challenge) 
        {
            var completedChallengesTableClient = tableServicesClient.GetTableClient(TableNames.CompletedChallenges);
            var challengesTableClient = tableServicesClient.GetTableClient(TableNames.Challenges);

            // The new challenge replaces an existing one, wipe the completions for the old one and then delete it before adding the replacement
            var completions = completedChallengesTableClient.Query<ChallengeEntity>(c => c.RowKey == challenge.RowKey);

            foreach( var completion in completions) 
            {
                // Delete the completions
                await completedChallengesTableClient.DeleteEntityAsync(completion.PartitionKey, completion.RowKey).ConfigureAwait(false);
            }
        }
    }
}

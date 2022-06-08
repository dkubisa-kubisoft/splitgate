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
    using Splitgate.Api.Models;
    using Splitgate.Api.Request;
    
    /// <summary>
    /// Azure function for posting a challenge.
    /// </summary>
    public class PostChallenges
    {
        /// <summary>
        /// The name of the Azure function.
        /// </summary>
        private const string FunctionName = "PostChallenges";

        /// <summary>
        /// The client to use to communicate with Azure Table Storage.
        /// </summary>
        private TableServiceClient tableServicesClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostChallenges" /> class.
        /// </summary>
        /// <param name="tableServiceClient">The table service client.</param>
        public PostChallenges(TableServiceClient tableServiceClient) 
        {
            this.tableServicesClient = tableServiceClient ?? throw new ArgumentNullException(nameof(tableServiceClient));
        }

        /// <summary>
        /// Azure function to create or update a challenge.
        /// </summary>
        /// <param name="req">The request.</param>
        /// <param name="log">The logger.</param>
        /// <returns>The <see cref="Task">.</returns>
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
                        return new BadRequestObjectResult(new { Message = "Invalid request: Please provide a list of challenges." });
                    }

                    var invalidChallengeTypes = request.Challenges.Where(c =>
                        c.ChallengeType != ChallengeTypes.Daily 
                        && c.ChallengeType != ChallengeTypes.Weekly 
                        && c.ChallengeType != ChallengeTypes.Seasonal)
                        .Select(c => c.ChallengeType);

                    if (invalidChallengeTypes.Any()) 
                    {
                        var invalidRequestTypes = request.Challenges.Select(c => c.ChallengeType);
                        return new BadRequestObjectResult(new { Message = $"Invalid request: Unknown challenge type(s): {string.Join(',', invalidChallengeTypes)}"});
                    }
                }
                catch
                {
                    return new BadRequestObjectResult(new { Message = "Unable to deserialize request." });
                }

                await this.tableServicesClient.CreateTableIfNotExistsAsync(TableNames.Challenges).ConfigureAwait(false);
                var challengesTableClient = tableServicesClient.GetTableClient(TableNames.Challenges);
                
                foreach( var challengeModel in request.Challenges) 
                {
                    var challengeToUpdate = new ChallengeEntity(challengeModel);

                    ChallengeEntity existingEntity = null;

                    try 
                    {
                        existingEntity = (await challengesTableClient.GetEntityAsync<ChallengeEntity>(challengeToUpdate.PartitionKey, challengeToUpdate.RowKey).ConfigureAwait(false))?.Value;
                    }
                    catch (RequestFailedException) {}

                    if (existingEntity != null)
                    {
                        // The posted challenge replaces an existing challenge
                        if (request.SuppressCompletionPurge == false)
                        {
                            await this.PurgeCompletions(existingEntity.ChallengeType, existingEntity.Index).ConfigureAwait(false);
                        }

                        // Load the existing entity with new values from the request
                        existingEntity.Load(challengeModel);

                        // Update the challenge
                        await challengesTableClient.UpdateEntityAsync<ChallengeEntity>(existingEntity, Azure.ETag.All, TableUpdateMode.Replace);
                    }
                    else 
                    {
                        // No existing challenge found, add the new challenge
                        await challengesTableClient.AddEntityAsync<ChallengeEntity>(challengeToUpdate).ConfigureAwait(false);
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

        /// <summary>
        /// Purges the completions of an existing challenge.
        /// </summary>
        /// <param name="challengeType">The challenge challenge type of the challenge.</param>
        /// <param name="index">The index of the challenge.</param>
        /// <returns>The <see cref="Task" />.</returns>
        private async Task PurgeCompletions(string challengeType, int index) 
        {
            var completedChallengesTableClient = tableServicesClient.GetTableClient(TableNames.CompletedChallenges);
            var challengesTableClient = tableServicesClient.GetTableClient(TableNames.Challenges);

            // The new challenge replaces an existing one, wipe the completions for the old one and then delete it before adding the replacement
            var completions = completedChallengesTableClient.Query<ChallengeEntity>(c => c.RowKey == $"{challengeType},{index}");

            foreach( var completion in completions)
            {
                // Delete the completions
                await completedChallengesTableClient.DeleteEntityAsync(completion.PartitionKey, completion.RowKey).ConfigureAwait(false);
            }
        }
    }
}

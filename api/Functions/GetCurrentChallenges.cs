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
    using Azure.Data.Tables;
    using Splitgate.Api.Response;
    using Splitgate.Api.Entities;

    public class GetCurrentChallenges
    {
        private TableServiceClient tableServiceClient;

        public GetCurrentChallenges(TableServiceClient tableServiceClient) 
        {
            this.tableServiceClient = tableServiceClient ?? throw new ArgumentNullException(nameof(tableServiceClient));
        }

        [FunctionName("GetCurrentChallenges")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("GetCurrentChallenges called.");

            var challengesTable = this.tableServiceClient.GetTableClient(TableNames.Challenges);
            var completedChallengesTable = this.tableServiceClient.GetTableClient(TableNames.CompletedChallenges);
            var ip = req.HttpContext.Connection.RemoteIpAddress.ToString();

            var challengeEntities = challengesTable.Query<ChallengeEntity>($"PartitionKey eq '1' and EndDateUtc gt '{DateTime.UtcNow.ToString(ChallengeEntity.DateTimeFormat)}'");

            // Get the completed challenges for the caller's ip
            var completedChallenges = completedChallengesTable.Query<Splitgate.Api.Entities.TableEntity>($"PartitionKey eq '{ip}'");
            
            var response = new GetCurrentChallengesResponse();

            response.Challenges.AddRange(challengeEntities.Select(entity => entity.GetModelObject(completedChallenges.Any(completed => completed.RowKey == entity.RowKey))));

            return new OkObjectResult(response);
        }
    }
}

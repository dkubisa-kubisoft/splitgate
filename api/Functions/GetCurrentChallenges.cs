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
            
            var response = new GetCurrentChallengesResponse();

            var challengesTable = this.tableServiceClient.GetTableClient(TableNames.Challenges);
            var completedChallengesTable = this.tableServiceClient.GetTableClient(TableNames.CompletedChallenges);
            var ip = req.HttpContext.Connection.RemoteIpAddress.ToString();

            // Retrieve all the challenges
            var dailys = challengesTable.Query<ChallengeEntity>("PartitionKey eq 'daily'");
            var weeklys = challengesTable.Query<ChallengeEntity>("PartitionKey eq 'weekly'");
            var seasonals = challengesTable.Query<ChallengeEntity>("PartitionKey eq 'seasonal'");

            // Get the completed challenges for the caller's ip
            var completedChallenges = completedChallengesTable.Query<Splitgate.Api.Entities.TableEntity>($"PartitionKey eq '{ip}'");
            
            // Add the unexpired daily, weekly and seasonal challenges to the response along with the caller's completion
            // state for each challenge.
            response.DailyChallenges.AddRange(
                dailys.Where(challenge => DateTime.Parse(challenge.EndDateUtc) > DateTime.UtcNow)
                .Select(challenge => challenge.GetModelObject(completedChallenges.Any(completed => completed.RowKey == $"{challenge.ChallengeType},{challenge.Index}"))));
            response.DailyChallengeRefreshTimestamp = dailys.Select(challenge => challenge.Timestamp).Min() ?? DateTime.MinValue;

            response.WeeklyChallenges.AddRange(
                weeklys.Where(challenge => DateTime.Parse(challenge.EndDateUtc) > DateTime.UtcNow)
                .Select(challenge => challenge.GetModelObject(completedChallenges.Any(completed => completed.RowKey == $"{challenge.ChallengeType},{challenge.Index}"))));
            response.WeeklyChallengeRefreshTimestamp = weeklys.Select(challenge => challenge.Timestamp).Min() ?? DateTimeOffset.MinValue;

            response.SeasonalChallenges.AddRange(
                seasonals.Where(challenge => DateTime.Parse(challenge.EndDateUtc) > DateTime.UtcNow)
                .Select(challenge => challenge.GetModelObject(completedChallenges.Any(completed => completed.RowKey == $"{challenge.ChallengeType},{challenge.Index}"))));
            response.SeasonalChallengeRefreshTimestamp = seasonals.Select(challenge => challenge.Timestamp).Min() ?? DateTimeOffset.MinValue;

            return new OkObjectResult(response);
        }
    }
}

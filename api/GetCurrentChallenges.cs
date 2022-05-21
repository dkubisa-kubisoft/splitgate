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
    using Azure.Data.Tables;
    using Splitgate.Api.Models;
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

            var tableClient = this.tableServiceClient.GetTableClient(TableNames.Challenges);

            var challengeEntities = tableClient.Query<ChallengeEntity>($"PartitionKey eq '{ DateTime.UtcNow.ToString(ChallengeEntity.PartitionKeyDateFormatString) }'");

            var response = new GetCurrentChallengesResponse();

            response.Challenges.AddRange(challengeEntities.Select(c => c.ToChallenge()));

            return new OkObjectResult(response);
        }
    }
}

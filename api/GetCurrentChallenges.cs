namespace Splitgate.Api
{
    using System;
    using System.IO;
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

    public class GetCurrentChallenges
    {
        private TableServiceClient tableServiceClient;

        public GetCurrentChallenges(TableServiceClient tableServiceClient) 
        {
            this.tableServiceClient = tableServiceClient ?? throw new ArgumentNullException(nameof(tableServiceClient));
        }

        [FunctionName("GetCurrentChallenges")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("GetCurrentChallenges called.");

            var tableClient = this.tableServiceClient.GetTableClient(TableNames.Challenges);

            var challenges = tableClient.Query<Challenge>(c => c.StartDateUtc <= DateTime.UtcNow && c.EndDateUtc > DateTime.UtcNow);

            var response = new GetCurrentChallengesResponse();

            response.Challenges.AddRange(challenges);

            return new OkObjectResult(response);
        }
    }
}

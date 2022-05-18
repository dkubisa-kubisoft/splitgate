namespace Splitgate.Api.Response 
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Splitgate.Api.Models;

    public class GetCurrentChallengesResponse
    {
        [JsonProperty("challenges")]
        public List<Challenge> Challenges { get; }

        public GetCurrentChallengesResponse() 
        {
            this.Challenges = new List<Challenge>();
        }
    }
}
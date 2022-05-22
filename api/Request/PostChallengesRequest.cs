namespace Splitgate.Api.Request 
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Splitgate.Api.Models;

    public class PostChallengesRequest
    {
        [JsonProperty("challenges")]
        public List<Challenge> Challenges { get; }

        public PostChallengesRequest() 
        {
            Challenges = new List<Challenge>();
        }
    }
}
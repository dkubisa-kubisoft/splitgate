namespace Splitgate.Api.Models 
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

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
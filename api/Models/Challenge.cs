namespace Splitgate.Api.Models
{
    using System;
    using Newtonsoft.Json;

    public class Challenge
    {
        [JsonProperty("challengeType")]
        public string ChallengeType { get; set; }

        [JsonProperty("index")]
        public string Index { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("startDateUtc")]
        public DateTime StartDateUtc { get; set; }

        [JsonProperty("endDateUtc")]
        public DateTime EndDateUtc { get; set; }
    }
}
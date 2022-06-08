namespace Splitgate.Api.Models
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Model object for a challenge.
    /// </summary>
    public class Challenge
    {
        /// <summary>
        /// Gets or sets the challenge type.
        /// </summary>
        [JsonProperty("challengeType")]
        public string ChallengeType { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        [JsonProperty("index")]
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the challenge description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the challenge start date.
        /// </summary>
        [JsonProperty("startDateUtc")]
        public DateTime StartDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the challenge end date.
        /// </summary>
        [JsonProperty("endDateUtc")]
        public DateTime EndDateUtc { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether the challenge has been completed.
        /// </summary>
        [JsonProperty("completed")]
        public bool Completed { get; set; }

        /// <summary>
        /// Gets or sets the challenge start date.
        /// </summary>
        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the challenge stage.
        /// </summary>
        /// <value>The stage.</value>
        [JsonProperty("stage")]
        public int? Stage { get; set; }
    }
}
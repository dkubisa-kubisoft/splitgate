namespace Splitgate.Api.Response 
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Splitgate.Api.Models;

    /// <summary>
    /// Response object for the <see cref="Splitgate.Api.Functions.GetCurrentChallenges" /> Azure function.
    /// </summary>
    public class GetCurrentChallengesResponse
    {
        /// <summary>
        /// Gets the daily challenges.
        /// </summary>
        /// <value>The daily challenges.</value>
        [JsonProperty("dailyChallenges")]
        public List<Challenge> DailyChallenges { get; }

        /// <summary>
        /// Gets or sets the time that the daily challenges were last refreshed.
        /// </summary>
        /// <value>The time that the daily challenges were last refreshed.</value>
        [JsonProperty("dailyChallengeRefreshTimestamp")]
        public DateTimeOffset DailyChallengeRefreshTimestamp { get; set; }

        /// <summary>
        /// Gets the weekly challenges.
        /// </summary>
        /// <value>The weekly challenges.</value>
        [JsonProperty("weeklyChallenges")]
        public List<Challenge> WeeklyChallenges { get; }

        /// <summary>
        /// Gets or sets the time that the weekly challenges were last refreshed.
        /// </summary>
        /// <value>The time that the weekly challenges were last refreshed.</value>
        [JsonProperty("weeklyChallengeRefreshTimestamp")]
        public DateTimeOffset WeeklyChallengeRefreshTimestamp { get; set; }

        /// <summary>
        /// Gets the season challenges.
        /// </summary>
        /// <value>The season challenges.</value>
        [JsonProperty("seasonalChallenges")]
        public List<Challenge> SeasonalChallenges { get; }

        /// <summary>
        /// Gets or sets the time that the season challenges were last refreshed.
        /// </summary>
        /// <value>The time that the season challenges were last refreshed.</value>
        [JsonProperty("seasonalChallengeRefreshTimestamp")]
        public DateTimeOffset SeasonalChallengeRefreshTimestamp { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Challenge" /> class.
        /// </summary>
        public GetCurrentChallengesResponse() 
        {
            this.DailyChallenges = new List<Challenge>();
            this.DailyChallengeRefreshTimestamp = DateTime.MinValue;
            this.WeeklyChallenges = new List<Challenge>();
            this.WeeklyChallengeRefreshTimestamp = DateTime.MinValue;
            this.SeasonalChallenges = new List<Challenge>();
            this.SeasonalChallengeRefreshTimestamp = DateTime.MinValue;
        }
    }
}
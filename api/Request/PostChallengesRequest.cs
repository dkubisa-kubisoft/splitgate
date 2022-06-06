namespace Splitgate.Api.Request 
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Splitgate.Api.Models;

    /// <summary>
    /// Request object for the <see cref="Splitgate.Api.Functions.PostChallenges" /> endpoint.
    /// </summary>
    public class PostChallengesRequest
    {
        /// <summary>
        /// Gets or sets the challenges to be created/updated.
        /// </summary>
        /// <value></value>
        [JsonProperty("challenges")]
        public List<Challenge> Challenges { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether to suppress the purging of completions for the 
        /// challenges specified in this request when they are updated.
        /// </summary>
        /// <value></value>
        [JsonProperty("suppressCompletionPurge")]
        public bool SuppressCompletionPurge { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PostChallengesRequest" /> class.
        /// </summary>
        public PostChallengesRequest() 
        {
            Challenges = new List<Challenge>();
        }
    }
}
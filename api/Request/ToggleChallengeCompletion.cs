namespace Splitgate.Api.Request 
{
    using Newtonsoft.Json;

    public class ToggleChallengeCompletionRequest
    {
        [JsonProperty("challengeId")]
        public string ChallengeId { get;set; }
    }
}
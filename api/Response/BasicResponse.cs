namespace Splitgate.Api.Response 
{
    using Newtonsoft.Json;
 
    public class BasicResponse
    {
        [JsonProperty("success")]
        public bool Success { get;set; }
    }
}
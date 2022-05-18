namespace Splitgate.Api.Models
{
    using System;
    using Azure;
    using Azure.Data.Tables;
    using Newtonsoft.Json;

    public class Challenge : ITableEntity
    {
        [JsonProperty("challengeType")]
        public string ChallengeType { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("startDateUtc")]
        public string StartDateUtc {get; set;}

        [JsonProperty("endDateUtc")]
        public string EndDateUtc {get; set;}

        public string PartitionKey 
        { 
            get 
            {
                return ChallengeType; 
            }
            set {
                ChallengeType = value;
            }
        }

        public string RowKey 
        { 
            get 
            {
                return $"{this.ChallengeType},{this.Description},{this.StartDateUtc},{this.EndDateUtc}";
            }
            
            set
            {
            } 
        }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
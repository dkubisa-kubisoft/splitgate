namespace Splitgate.Api.Entities 
{
    using System;
    using Splitgate.Api.Models;

    public class ChallengeEntity : TableEntity
    {
        public const string PartitionKeyDateFormatString = "yyyy-MM-dd";
        public string ChallengeType { get; set; }
        public string Index { get; set; }
        public string Description { get; set; }
        public string StartDateUtc { get;set; }
        public string EndDateUtc { get; set; }

        public ChallengeEntity() 
        {
        }

        public ChallengeEntity(Challenge challenge)
        {
            // Validate input
            if (challenge.ChallengeType != ChallengeTypes.Daily
                && challenge.ChallengeType != ChallengeTypes.Weekly
                && challenge.ChallengeType != ChallengeTypes.Seasonal) 
            {
                throw new ArgumentException($"Invalid challenge type: [{challenge.ChallengeType}].");
            }

            if (string.IsNullOrWhiteSpace(challenge.Index))
            {
                throw new ArgumentException("Missing challenge index.");
            }

            if (string.IsNullOrWhiteSpace(challenge.Description))
            {
                throw new ArgumentException("Missing challenge description.");
            }

            this.StartDateUtc = challenge.StartDateUtc.ToString("s");
            this.EndDateUtc = challenge.EndDateUtc.ToString("s");
            this.ChallengeType = challenge.ChallengeType;
            this.Index = challenge.Index;
            this.Description = challenge.Description;
            
            this.PartitionKey = DateTime.UtcNow.ToString(PartitionKeyDateFormatString);
            this.RowKey = $"{challenge.ChallengeType},{challenge.Index}";
        }

        public Challenge ToChallenge() 
        {
            return new Challenge
            {
                ChallengeType = this.ChallengeType,
                Index = this.Index,
                Description = this.Description,
                StartDateUtc = DateTime.Parse(this.StartDateUtc),
                EndDateUtc = DateTime.Parse(this.EndDateUtc)
            };
        }
    }
}
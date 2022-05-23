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

        public ChallengeEntity(Challenge modelObject)
        {
            // Validate input
            if (modelObject.ChallengeType != ChallengeTypes.Daily
                && modelObject.ChallengeType != ChallengeTypes.Weekly
                && modelObject.ChallengeType != ChallengeTypes.Seasonal) 
            {
                throw new ArgumentException($"Invalid challenge type: [{modelObject.ChallengeType}].");
            }

            if (string.IsNullOrWhiteSpace(modelObject.Index))
            {
                throw new ArgumentException("Missing challenge index.");
            }

            if (string.IsNullOrWhiteSpace(modelObject.Description))
            {
                throw new ArgumentException("Missing challenge description.");
            }

            this.StartDateUtc = modelObject.StartDateUtc.ToString("s");
            this.EndDateUtc = modelObject.EndDateUtc.ToString("s");
            this.ChallengeType = modelObject.ChallengeType;
            this.Index = modelObject.Index;
            this.Description = modelObject.Description;
            
            this.PartitionKey = "1";
            this.RowKey = $"{modelObject.ChallengeType},{modelObject.Index}";
        }

        public Challenge GetModelObject(bool completed) 
        {
            return new Challenge
            {
                ChallengeType = this.ChallengeType,
                Index = this.Index,
                Description = this.Description,
                StartDateUtc = DateTime.Parse(this.StartDateUtc),
                EndDateUtc = DateTime.Parse(this.EndDateUtc),
                Completed = completed
            };
        }

        public ChallengeArchiveEntity GetArchiveEntity() 
        {
            return new ChallengeArchiveEntity(this.GetModelObject(false));
        }
    }
}
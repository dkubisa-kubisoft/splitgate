namespace Splitgate.Api.Entities 
{
    using System;
    using Splitgate.Api.Models;

    /// <summary>
    /// Represents an entity in the ChallengeArchive table.
    /// </summary>
    public class ChallengeArchiveEntity : TableEntity
    {
        /// <summary>
        /// The format string for the partition key.
        /// </summary>
        private const string PartitionKeyDateFormatString = "yyyy-MM-dd";

        /// <summary>
        /// Gets or sets the challenge type.
        /// </summary>
        public string ChallengeType { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public string StartDateUtc { get;set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public string EndDateUtc { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeArchiveEntity" /> class.
        /// </summary>
        public ChallengeArchiveEntity() 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeArchiveEntity" /> class and initializes its
        /// values to reflect the specified <see cref="Challenge" />.
        /// <param name="modelObject">The <see cref="Challenge" />.</param>
        /// </summary>
        public ChallengeArchiveEntity(Challenge modelObject)
        {
            // Validate input
            if (modelObject.ChallengeType != ChallengeTypes.Daily
                && modelObject.ChallengeType != ChallengeTypes.Weekly
                && modelObject.ChallengeType != ChallengeTypes.Seasonal) 
            {
                throw new ArgumentException($"Invalid challenge type: [{modelObject.ChallengeType}].");
            }

            if (modelObject.Index <= 0)
            {
                throw new ArgumentException("Invalid index.");
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
            
            this.PartitionKey = DateTime.UtcNow.ToString(PartitionKeyDateFormatString);
            this.RowKey = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Creates and returns a <see cref="Challenge" /> object that reflects this entity.
        /// </summary>
        /// <returns>The <see cref="Challenge" />.</returns>
        public Challenge GetModelObject() 
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
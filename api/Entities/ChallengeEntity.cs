namespace Splitgate.Api.Entities 
{
    using System;
    using Splitgate.Api.Models;

    /// <summary>
    /// Represents an entity in the Challenge table
    /// </summary>
    public class ChallengeEntity : TableEntity
    {

        /// <summary>
        /// The format string used for datetime translation between this entity and its model.
        /// </summary>
        public const string DateTimeFormat = "o";

        /// <summary>
        /// Gets or sets the challenge type.
        /// </summary>
        public string ChallengeType { get; set; }
        
        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the challenge description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the challenge start date.
        /// </summary>
        public string StartDateUtc { get;set; }

        /// <summary>
        /// Gets or sets the challenge end date.
        /// </summary>
        public string EndDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the challenge stage.
        /// </summary>
        /// <value>The challenge stage.</value>
        public int? Stage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeEntity" class.
        /// </summary>
        public ChallengeEntity() 
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeEntity" class populated with values 
        /// from the specified <see cref="Challenge" />.
        /// </summary>
        /// <param name="modelObject">The model object to populate the entity from.</param>
        public ChallengeEntity(Challenge modelObject)
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

            this.PartitionKey = modelObject.ChallengeType;
            this.RowKey = modelObject.Index.ToString();
            this.StartDateUtc = modelObject.StartDateUtc.ToString(DateTimeFormat);
            this.EndDateUtc = modelObject.EndDateUtc.ToString(DateTimeFormat);
            this.ChallengeType = modelObject.ChallengeType;
            this.Index = modelObject.Index;
            this.Description = modelObject.Description;
            this.Stage = modelObject.Stage;
        }

        /// <summary>
        /// Creates and returns a new <see cref="Challenge" /> object that reflects the properties of this <see cref="ChallengeEntity" />.
        /// </summary>
        /// <param name="completed">Flag indicating whether to mark the returned challeng</param>
        /// <returns>The <see cref="Challenge" />.</returns>
        public Challenge GetModelObject(bool completed) 
        {
            return new Challenge
            {
                ChallengeType = this.ChallengeType,
                Index = this.Index,
                Description = this.Description,
                StartDateUtc = DateTime.Parse(this.StartDateUtc),
                EndDateUtc = DateTime.Parse(this.EndDateUtc),
                Completed = completed,
                Timestamp = this.Timestamp.HasValue ? this.Timestamp.Value : DateTimeOffset.MinValue,
                Stage = this.Stage
            };
        }

        /// <summary>
        /// Loads this entity with values from the specified <see cref="Challenge" />.
        /// <param name="modelObject">The model object.</param>
        /// </summary>
        public void Load(Challenge modelObject) 
        {
            this.ChallengeType = modelObject.ChallengeType;
            this.Description = modelObject.Description;
            this.EndDateUtc = modelObject.EndDateUtc.ToString(ChallengeEntity.DateTimeFormat);
            this.Index = modelObject.Index;
            this.StartDateUtc = modelObject.StartDateUtc.ToString(ChallengeEntity.DateTimeFormat);
            this.Stage = modelObject.Stage;
        }
    }
}
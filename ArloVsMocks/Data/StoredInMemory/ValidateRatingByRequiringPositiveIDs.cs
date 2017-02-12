using System;

namespace ArloVsMocks.Data.StoredInMemory
{
	internal class ValidateRatingByRequiringPositiveIDs : Validator<Rating>
	{
		public ValidateRatingByRequiringPositiveIDs(bool hasErrors)
		{
			HasErrors = hasErrors;
		}

		public bool HasErrors { get; set; }

		public void ReportErrors()
		{
			if (HasErrors)
			{
				HasErrors = false;
				try
				{
					throw new Exception("Foreign key violation.");
				}
				catch (Exception innerException)
				{
					throw new Exception("An error occurred while updating the entries. See the inner exception for details.",
						innerException);
				}
			}
		}

		public void Validate(Rating rating)
		{
			HasErrors = HasErrors || (rating.CriticId < 1) || (rating.MovieId < 1);
		}
	}
}
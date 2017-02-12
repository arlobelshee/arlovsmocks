using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ArloVsMocks.Data;

namespace ArloVsMocks
{
	public static class RatingsExtensions
	{
		public static DataTablePort<T> ToDataTablePort<T>(this DbSet<T> table, MovieReviewEntities db) where T : class
		{
			return new DataTablePort<T>(table, rating => table.Add(rating), () => db.SaveChanges());
		}

		public static DataTablePort<Rating> AsDataTablePort(this HashSet<Rating> data)
		{
			return MakeDataPort(data, new ValidateRatingByRequiringPositiveIDs(false));
		}

		public static DataTablePort<T> AsDataTablePort<T>(this HashSet<T> data) where T : class
		{
			return MakeDataPort(data, new ValidateByAllowingAnything<T>());
		}

		private static DataTablePort<T> MakeDataPort<T>(HashSet<T> data, Validator<T> validator)
			where T : class
		{
			var nextState = new HashSet<T>(data);
			return new DataTablePort<T>(data.AsQueryable(), d =>
			{
				validator.Validate(d);
				nextState.Add(d);
			}, () =>
			{
				validator.ReportErrors();
				data.Clear();
				data.UnionWith(nextState);
			});
		}

		public static Rating ToRating(this Critique critique)
		{
			var createdRating = new Rating
			{
				CriticId = critique.CriticId,
				MovieId = critique.MovieId,
				Stars = critique.Stars
			};
			return createdRating;
		}

		private class ValidateByAllowingAnything<T> : Validator<T>
		{
			public void Validate(T data)
			{
			}

			public void ReportErrors()
			{
			}
		}

		private class ValidateRatingByRequiringPositiveIDs : Validator<Rating>
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

	internal interface Validator<T>
	{
		void Validate(T data);
		void ReportErrors();
	}
}
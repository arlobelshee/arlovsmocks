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
			var hasErrors = false;
			return MakeDataPort(data, rating => { hasErrors = hasErrors || (rating.CriticId < 1) || (rating.MovieId < 1); },
				() =>
				{
					if (hasErrors)
					{
						hasErrors = false;
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
				});
		}

		public static DataTablePort<T> AsDataTablePort<T>(this HashSet<T> data) where T : class
		{
			return MakeDataPort(data, d => { }, () => { });
		}

		private static DataTablePort<T> MakeDataPort<T>(HashSet<T> data, Action<T> validate, Action reportErrors)
			where T : class
		{
			var nextState = new HashSet<T>(data);
			return new DataTablePort<T>(data.AsQueryable(), d =>
			{
				validate(d);
				nextState.Add(d);
			}, () =>
			{
				reportErrors();
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
	}
}
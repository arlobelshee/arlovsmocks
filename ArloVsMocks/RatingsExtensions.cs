using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ArloVsMocks.Data;

namespace ArloVsMocks
{
	public static class RatingsExtensions
	{
		public static DataTablePort ToDataTablePort(this DbSet<Rating> table, MovieReviewEntities db)
		{
			return new DataTablePort(table, rating => table.Add(rating), ()=> db.SaveChanges());
		}

		public static DataTablePort AsDataTablePort(this HashSet<Rating> data)
		{
			return new DataTablePort(data.AsQueryable(), d => data.Add(d), () => { });
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
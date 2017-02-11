using System.Data.Entity;
using ArloVsMocks.Data;

namespace ArloVsMocks
{
	internal static class RatingsExtensions
	{
		public static Ratings ToRatings(this DbSet<Rating> table)
		{
			return new Ratings(table, rating => table.Add(rating));
		}
	}
}
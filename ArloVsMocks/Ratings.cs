using System.Data.Entity;
using ArloVsMocks.Data;

namespace ArloVsMocks
{
	internal class Ratings
	{
		private DbSet<Rating> _ratings;

		public Ratings(DbSet<Rating> ratings)
		{
			_ratings = ratings;
		}

		public DbSet<Rating> Ratings1
		{
			get { return _ratings; }
		}

		public void Save(Rating existingRating)
		{
			Ratings1.Add(existingRating);
		}
	}
}
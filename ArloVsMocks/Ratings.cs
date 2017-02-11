using System.Data.Entity;
using System.Linq;
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

		public IQueryable<Rating> Ratings1
		{
			get { return _ratings; }
		}

		public void Save(Rating existingRating)
		{
			_ratings.Add(existingRating);
		}
	}
}
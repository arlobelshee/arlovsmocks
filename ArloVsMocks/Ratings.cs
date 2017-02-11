using System;
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

		public IQueryable<Rating> ExistingData
		{
			get { return _ratings; }
		}

		public void Save(Rating existingRating)
		{
			Action<Rating> save = rating => _ratings.Add(rating);
			save(existingRating);
		}
	}
}
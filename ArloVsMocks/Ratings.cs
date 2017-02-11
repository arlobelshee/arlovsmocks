using System;
using System.Data.Entity;
using System.Linq;
using ArloVsMocks.Data;

namespace ArloVsMocks
{
	internal class Ratings
	{
		private DbSet<Rating> _ratings;
		private readonly Action<Rating> _saveImpl;

		public Ratings(DbSet<Rating> ratings, Action<Rating> saveImpl)
		{
			_ratings = ratings;
			_saveImpl = saveImpl;
		}

		public IQueryable<Rating> ExistingData
		{
			get { return _ratings; }
		}

		public void Save(Rating existingRating)
		{
			_saveImpl(existingRating);
		}
	}
}
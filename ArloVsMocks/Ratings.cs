using System;
using System.Linq;
using ArloVsMocks.Data;

namespace ArloVsMocks
{
	internal class Ratings
	{
		private readonly Action<Rating> _saveImpl;

		public Ratings(IQueryable<Rating> ratings, Action<Rating> saveImpl)
		{
			ExistingData = ratings;
			_saveImpl = saveImpl;
		}

		public IQueryable<Rating> ExistingData { get; }

		public void Save(Rating existingRating)
		{
			_saveImpl(existingRating);
		}
	}
}
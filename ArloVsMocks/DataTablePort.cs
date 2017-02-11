using System;
using System.Linq;
using ArloVsMocks.Data;

namespace ArloVsMocks
{
	internal class DataTablePort
	{
		private readonly Action<Rating> _saveImpl;

		public DataTablePort(IQueryable<Rating> existingData, Action<Rating> saveImpl)
		{
			ExistingData = existingData;
			_saveImpl = saveImpl;
		}

		public IQueryable<Rating> ExistingData { get; }

		public void Save(Rating existingRating)
		{
			_saveImpl(existingRating);
		}
	}
}
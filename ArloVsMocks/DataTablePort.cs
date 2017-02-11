using System;
using System.Linq;
using ArloVsMocks.Data;

namespace ArloVsMocks
{
	public class DataTablePort
	{
		private readonly Action<Rating> _saveImpl;
		private readonly Action _saveChanges;

		public DataTablePort(IQueryable<Rating> existingData, Action<Rating> saveImpl, Action saveChanges)
		{
			ExistingData = existingData;
			_saveImpl = saveImpl;
			_saveChanges = saveChanges;
		}

		public IQueryable<Rating> ExistingData { get; }

		public void Save(Rating existingRating)
		{
			_saveImpl(existingRating);
		}

		public void PersistAll()
		{
			_saveChanges();
		}
	}
}
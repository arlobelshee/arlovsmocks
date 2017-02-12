using System;
using System.Linq;

namespace ArloVsMocks
{
	public class DataTablePort<T> where T : class
	{
		private readonly Action _saveChanges;
		private readonly Action<T> _saveImpl;

		public DataTablePort(IQueryable<T> existingData, Action<T> saveImpl, Action saveChanges)
		{
			ExistingData = existingData;
			_saveImpl = saveImpl;
			_saveChanges = saveChanges;
		}

		public IQueryable<T> ExistingData { get; }

		public void Save(T existingT)
		{
			_saveImpl(existingT);
		}

		public void PersistAll()
		{
			_saveChanges();
		}
	}
}
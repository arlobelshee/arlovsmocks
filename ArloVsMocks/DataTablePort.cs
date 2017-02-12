using System;
using System.Linq;
using ArloVsMocks.Data;

namespace ArloVsMocks
{
	public class DataTablePort<T> where T : class
	{
		private readonly Action _saveChanges;
		private readonly DataTablePortAdapter<T> _adapter;
		private readonly Action<T> _saveImpl;

		public DataTablePort(IQueryable<T> existingData, Action<T> saveImpl, Action saveChanges, DataTablePortAdapter<T> adapter)
		{
			ExistingData = existingData;
			_saveImpl = saveImpl;
			_saveChanges = saveChanges;
			_adapter = adapter;
		}

		public IQueryable<T> ExistingData { get; }

		public void Save(T item)
		{
			_adapter.SaveItem(item);
		}

		public void PersistAll()
		{
			_adapter.PersistAll();
		}
	}
}
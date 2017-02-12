using System.Linq;
using ArloVsMocks.Data;

namespace ArloVsMocks
{
	public class DataTablePort<T> where T : class
	{
		private readonly DataTablePortAdapter<T> _adapter;

		public DataTablePort(IQueryable<T> existingData, DataTablePortAdapter<T> adapter)
		{
			ExistingData = existingData;
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
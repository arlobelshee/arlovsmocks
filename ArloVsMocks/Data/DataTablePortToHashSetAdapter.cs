using System.Collections.Generic;

namespace ArloVsMocks.Data
{
	public class DataTablePortToHashSetAdapter<T> : DataTablePortAdapter<T> where T : class
	{
		private readonly HashSet<T> _data;
		private readonly HashSet<T> _nextState;
		private readonly Validator<T> _validator;

		public DataTablePortToHashSetAdapter(HashSet<T> data, Validator<T> validator)
		{
			_data = data;
			_validator = validator;
			_nextState = new HashSet<T>();
		}

		public void PersistAll()
		{
			_validator.ReportErrors();
			_data.Clear();
			_data.UnionWith(_nextState);
		}

		public void SaveItem(T item)
		{
			_validator.Validate(item);
			_nextState.Add(item);
		}
	}
}
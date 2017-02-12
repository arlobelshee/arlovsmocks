using System.Collections.Generic;

namespace ArloVsMocks.Data
{
	public class DataTablePortToHashSetAdapter<T> where T : class
	{
		public DataTablePortToHashSetAdapter(HashSet<T> data, Validator<T> validator)
		{
			Data = data;
			Validator = validator;
			NextState = new HashSet<T>();
		}

		public HashSet<T> Data { get; }

		public Validator<T> Validator { get; }

		public HashSet<T> NextState { get; }

		public void PersistAll()
		{
			Validator.ReportErrors();
			Data.Clear();
			Data.UnionWith(NextState);
		}

		public void SaveItem(T d)
		{
			Validator.Validate(d);
			NextState.Add(d);
		}
	}
}
namespace ArloVsMocks.Data
{
	public interface DataTablePortAdapter<T> where T : class
	{
		void PersistAll();
		void SaveItem(T item);
	}
}
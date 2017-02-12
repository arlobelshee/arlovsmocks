using System.Data.Entity;

namespace ArloVsMocks.Data.StoredInEntityFrameworkRepository
{
	public class DataTablePortToEntityFrameworkAdapter<T> : DataTablePortAdapter<T> where T : class
	{
		private readonly MovieReviewEntities _db;
		private readonly DbSet<T> _table;

		public DataTablePortToEntityFrameworkAdapter(MovieReviewEntities db, DbSet<T> table)
		{
			_db = db;
			_table = table;
		}

		public void PersistAll()
		{
			_db.SaveChanges();
		}

		public void SaveItem(T item)
		{
			_table.Add(item);
		}
	}
}
using System.Data.Entity;

namespace ArloVsMocks.Data
{
	public class DataTablePortToEntityFrameworkAdapter<T> where T : class
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

		public void SaveItem(T rating)
		{
			_table.Add(rating);
		}
	}
}
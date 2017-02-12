using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ArloVsMocks.Data.StoredInEntityFrameworkRepository;
using ArloVsMocks.Data.StoredInMemory;

namespace ArloVsMocks.Data
{
	public static class DataExtensions
	{
		public static DataTablePort<T> ToDataTablePort<T>(this DbSet<T> table, MovieReviewEntities db) where T : class
		{
			var adapter = new DataTablePortToEntityFrameworkAdapter<T>(db, table);
			return new DataTablePort<T>(table, adapter);
		}

		public static DataTablePort<Rating> AsDataTablePort(this HashSet<Rating> data)
		{
			return MakeDataPort(data, new ValidateRatingByRequiringPositiveIDs(false));
		}

		public static DataTablePort<T> AsDataTablePort<T>(this HashSet<T> data) where T : class
		{
			return MakeDataPort(data, new ValidateByAllowingAnything<T>());
		}

		private static DataTablePort<T> MakeDataPort<T>(HashSet<T> data, Validator<T> validator) where T : class
		{
			var adapter = new DataTablePortToHashSetAdapter<T>(data, validator);
			return new DataTablePort<T>(data.AsQueryable(), adapter);
		}
	}
}
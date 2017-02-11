using System.Data.Entity;
using ArloVsMocks.Data;

namespace ArloVsMocks
{
	internal static class RatingsExtensions
	{
		public static DataTablePort ToDataTablePort(this DbSet<Rating> table)
		{
			return new DataTablePort(table, rating => table.Add(rating));
		}
	}
}
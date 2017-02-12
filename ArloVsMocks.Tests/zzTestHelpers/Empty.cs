using System.Collections.Generic;
using ArloVsMocks.Data;

namespace ArloVsMocks.Tests.zzTestHelpers
{
	internal static class Empty
	{
		public static DataTablePort<T> Table<T>() where T : class
		{
			return new HashSet<T>().AsDataTablePort();
		}
		public static DataTablePort<Rating> TableThatMonitorsForeignKeys()
		{
			return new HashSet<Rating>().AsDataTablePort();
		}
	}
}
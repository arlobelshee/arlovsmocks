using System.Collections.Generic;
using ArloVsMocks.Data;
using NUnit.Framework;

namespace ArloVsMocks.Tests
{
	[TestFixture]
	public class HashSetAdapterMeetsDataTablePortSpec : DataTablePortSpec
	{
		protected override DataTablePort<Critic> CreateTestSubject()
		{
			return new HashSet<Critic>().AsDataTablePort();
		}
	}
}
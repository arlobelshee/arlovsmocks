using System.Collections.Generic;
using ArloVsMocks.Data;
using NUnit.Framework;

namespace ArloVsMocks.Tests
{
	[TestFixture]
	public class HashSetAdapterMeetsDataTablePortSpec : DataTablePortSpec
	{
		protected override DataTablePort CreateTestSubject()
		{
			return new HashSet<Rating>().AsDataTablePort();
		}
	}
}
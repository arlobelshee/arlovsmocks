using System.Collections.Generic;
using System.Linq;
using ArloVsMocks.Data;
using NUnit.Framework;

namespace ArloVsMocks.Tests
{
	[TestFixture]
	public class TrackRating
	{
		[Test]
		public void NewRatingShouldBeCreated()
		{
			var data = new HashSet<Rating>();
			var port = new DataTablePort(data.AsQueryable(), d=> data.Add(d));
		}

	}
}
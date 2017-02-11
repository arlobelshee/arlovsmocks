using System.Collections.Generic;
using System.Linq;
using ArloVsMocks.Data;
using FluentAssertions;
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
			var port = new DataTablePort(data.AsQueryable(), d => data.Add(d));
			var critique = new Critique(1, 2, 3);
			var createdRating = new Rating
			{
				CriticId = critique.CriticId,
				MovieId = critique.MovieId,
				Stars = critique.Stars
			};

			Program.UpsertRating(port, critique);
			data.Should()
				.BeEquivalentTo(createdRating);
		}
	}
}
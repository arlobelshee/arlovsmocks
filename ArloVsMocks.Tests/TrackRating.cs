using System;
using System.Collections.Generic;
using ArloVsMocks.Data;
using FluentAssertions;
using NUnit.Framework;

namespace ArloVsMocks.Tests
{
	[TestFixture]
	public class TrackRating
	{
		private static DataTablePort<T> EmptyTable<T>() where T : class
		{
			var data = new HashSet<T>();
			return data.AsDataTablePort();
		}

		[Test]
		public void ExistingMatchingRatingShouldBeUpdated()
		{
			var data = new HashSet<Rating>();
			var port = data.AsDataTablePort();
			var critique = new Critique(1, 2, 3);
			port.Save(new Rating
			{
				CriticId = critique.CriticId,
				MovieId = critique.MovieId,
				Stars = 1
			});
			port.PersistAll();

			Program.UpsertRating(port, critique);
			port.PersistAll();
			data.Should().BeEquivalentTo(critique.ToRating());
		}

		[Test]
		public void NewRatingShouldBeCreated()
		{
			var data = new HashSet<Rating>();
			var port = data.AsDataTablePort();
			var critique = new Critique(1, 2, 3);

			Program.UpsertRating(port, critique);
			port.PersistAll();
			data.Should().BeEquivalentTo(critique.ToRating());
		}

		[Test]
		public void NonmatchingRatingShouldBeCreatedNextToExistingOne()
		{
			var port = EmptyTable<Rating>();
			var critique = new Critique(1, 2, 3);
			var existingRating = new Rating
			{
				CriticId = critique.CriticId,
				MovieId = critique.MovieId + 5,
				Stars = 1
			};
			port.Save(existingRating);
			port.PersistAll();

			Program.UpsertRating(port, critique);
			port.PersistAll();
			port.ExistingData.Should().BeEquivalentTo(critique.ToRating(), existingRating);
		}

		[Test]
		public void RatingThatDoesntMatchKnownMovieOrCriticShouldSetUpBombWithWillEventuallyExplodeAtUserWithPoorUx()
		{
			var data = new HashSet<Rating>();
			var port = data.AsDataTablePort();
			var critique = new Critique(-1, -2, 3);

			Program.UpsertRating(port, critique);
			Action persist = port.PersistAll;
			persist.ShouldThrow<Exception>();
		}
	}
}
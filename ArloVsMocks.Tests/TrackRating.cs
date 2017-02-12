using System;
using System.Collections.Generic;
using ArloVsMocks.Data;
using ArloVsMocks.Tests.zzTestHelpers;
using FluentAssertions;
using NUnit.Framework;

namespace ArloVsMocks.Tests
{
	[TestFixture]
	public class TrackRating
	{
		[Test]
		public void ExistingMatchingRatingShouldBeUpdated()
		{
			var port = Empty.Table<Rating>();
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
			port.ExistingData.Should().BeEquivalentTo(critique.ToRating());
		}

		[Test]
		public void NewRatingShouldBeCreated()
		{
			var port = Empty.Table<Rating>();
			var critique = new Critique(1, 2, 3);

			Program.UpsertRating(port, critique);
			port.PersistAll();
			port.ExistingData.Should().BeEquivalentTo(critique.ToRating());
		}

		[Test]
		public void NonmatchingRatingShouldBeCreatedNextToExistingOne()
		{
			var port = Empty.Table<Rating>();
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
			var port = Empty.TableThatMonitorsForeignKeys();
			var critique = new Critique(-1, -2, 3);

			Program.UpsertRating(port, critique);
			Action persist = port.PersistAll;
			persist.ShouldThrow<Exception>();
		}
	}
}
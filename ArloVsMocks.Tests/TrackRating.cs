using System;
using ArloVsMocks.Data;
using ArloVsMocks.Tests.zzTestHelpers;
using FluentAssertions;
using NUnit.Framework;

namespace ArloVsMocks.Tests
{
	[TestFixture]
	public class TrackRating
	{
		private static readonly Critique NewCritique = new Critique(1, 2, 3);

		[Test]
		public void ExistingMatchingRatingShouldBeUpdated()
		{
			var port = Empty.Table<Rating>();
			port.Save(new Rating
			{
				CriticId = NewCritique.CriticId,
				MovieId = NewCritique.MovieId,
				Stars = 1
			});
			port.PersistAll();

			Program.UpsertRating(port, NewCritique);
			port.PersistAll();
			port.ExistingData.Should().BeEquivalentTo(NewCritique.ToRating());
		}

		[Test]
		public void NewRatingShouldBeCreated()
		{
			var port = Empty.Table<Rating>();

			Program.UpsertRating(port, NewCritique);
			port.PersistAll();
			port.ExistingData.Should().BeEquivalentTo(NewCritique.ToRating());
		}

		[Test]
		public void NonmatchingRatingShouldBeCreatedNextToExistingOne()
		{
			var port = Empty.Table<Rating>();
			var existingRating = new Rating
			{
				CriticId = NewCritique.CriticId,
				MovieId = NewCritique.MovieId + 5,
				Stars = 1
			};
			port.Save(existingRating);
			port.PersistAll();

			Program.UpsertRating(port, NewCritique);
			port.PersistAll();
			port.ExistingData.Should().BeEquivalentTo(NewCritique.ToRating(), existingRating);
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
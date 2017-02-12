using System;
using ArloVsMocks.CritiqueMovies;
using ArloVsMocks.Data;
using ArloVsMocks.Tests.zzTestHelpers;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;

namespace ArloVsMocks.Tests.MaintainRatings
{
	[TestFixture]
	public class TrackRating
	{
		[Test]
		public void ExistingMatchingRatingShouldBeUpdated()
		{
			var port = TableWithOneRating(PriorCritiqueForSameMovieAndCritic.ToRating());

			NewCritique.UpsertRating(port);
			port.PersistAll();
			port.ExistingData.Should().BeEquivalentTo(NewCritique.ToRating());
		}

		[Test]
		public void NewRatingShouldBeCreated()
		{
			var port = Empty.Table<Rating>();

			NewCritique.UpsertRating(port);
			port.PersistAll();
			port.ExistingData.Should().BeEquivalentTo(NewCritique.ToRating());
		}

		[Test]
		public void NonmatchingRatingShouldBeCreatedNextToExistingOne()
		{
			var existingRating = CritiqueForDifferentMovie.ToRating();
			var port = TableWithOneRating(existingRating);

			NewCritique.UpsertRating(port);
			port.PersistAll();
			port.ExistingData.Should().BeEquivalentTo(NewCritique.ToRating(), existingRating);
		}

		[Test]
		[Category("probably a bug")]
		public void RatingThatDoesntMatchKnownMovieOrCriticShouldSetUpBombThatWillEventuallyExplodeAtUserWithPoorUx()
		{
			var port = Empty.TableThatMonitorsForeignKeys();
			var critique = new Critique(-1, -2, 3);

			critique.UpsertRating(port);
			Action persist = port.PersistAll;
			persist.ShouldThrow<Exception>();
		}

		private static DataTablePort<Rating> TableWithOneRating(Rating existingRating)
		{
			var port = Empty.Table<Rating>();
			port.Save(existingRating);
			port.PersistAll();
			return port;
		}

		[NotNull]
		private static readonly Critique NewCritique = new Critique(1, 2, 3);
		[NotNull]
		private static readonly Critique CritiqueForDifferentMovie = new Critique(6, 2, 3);
		[NotNull]
		private static readonly Critique PriorCritiqueForSameMovieAndCritic = new Critique(1, 2, 5);
	}
}
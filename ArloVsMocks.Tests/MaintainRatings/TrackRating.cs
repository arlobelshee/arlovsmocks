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
			var port = TableWithOneRating(RatingMatchingNewCritiqueButWithDifferentStars());

			MovieRatings.UpsertRating(port, NewCritique);
			port.PersistAll();
			port.ExistingData.Should().BeEquivalentTo(NewCritique.ToRating());
		}

		[Test]
		public void NewRatingShouldBeCreated()
		{
			var port = Empty.Table<Rating>();

			MovieRatings.UpsertRating(port, NewCritique);
			port.PersistAll();
			port.ExistingData.Should().BeEquivalentTo(NewCritique.ToRating());
		}

		[Test]
		public void NonmatchingRatingShouldBeCreatedNextToExistingOne()
		{
			var existingRating = RatingForDifferentMovieThanNewCritique();
			var port = TableWithOneRating(existingRating);

			MovieRatings.UpsertRating(port, NewCritique);
			port.PersistAll();
			port.ExistingData.Should().BeEquivalentTo(NewCritique.ToRating(), existingRating);
		}

		[Test]
		[Category("probably a bug")]
		public void RatingThatDoesntMatchKnownMovieOrCriticShouldSetUpBombThatWillEventuallyExplodeAtUserWithPoorUx()
		{
			var port = Empty.TableThatMonitorsForeignKeys();
			var critique = new Critique(-1, -2, 3);

			MovieRatings.UpsertRating(port, critique);
			Action persist = port.PersistAll;
			persist.ShouldThrow<Exception>();
		}

		private static Rating RatingMatchingNewCritiqueButWithDifferentStars()
		{
			return _MakeRating(1, NewCritique.MovieId);
		}

		private static Rating RatingForDifferentMovieThanNewCritique()
		{
			return _MakeRating(NewCritique.Stars, NewCritique.MovieId + 5);
		}

		private static Rating _MakeRating(int stars, int movieId)
		{
			return new Rating
			{
				CriticId = NewCritique.CriticId,
				MovieId = movieId,
				Stars = stars
			};
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
	}
}
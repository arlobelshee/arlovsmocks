using ArloVsMocks.CritiqueMovies;
using ArloVsMocks.Data;
using ArloVsMocks.Tests.zzTestHelpers;
using FluentAssertions;
using NUnit.Framework;

namespace ArloVsMocks.Tests.JudgeCriticReliability
{
	[TestFixture]
	public class TrackCriticCredibility
	{
		[Test]
		public void CriticGenerallyCloseButNotOnShouldBeTypical()
		{
			CriticShouldBeTrustedToCorrectDegree(CriticTrustworthiness.Typical,
				History(Opinion(TwoStarMovie, 3), Opinion(ThreeStarMovie, 4), Opinion(FourStarMovie, 2)));
		}

		[Test]
		[Category("probably a bug")]
		public void CriticWhoOnlyReviewedUnknownMoviesShouldBeFullyTrusted()
		{
			CriticShouldBeTrustedToCorrectDegree(CriticTrustworthiness.Trustworthy, History(Opinion(UnknownStarMovie, 5)));
		}

		[Test]
		public void CriticWithAbnormalReviewsShouldBeMostlyIgnored()
		{
			CriticShouldBeTrustedToCorrectDegree(CriticTrustworthiness.Untrustworthy,
				History(Opinion(ThreeStarMovie, 1), Opinion(TwoStarMovie, 5)));
		}

		[Test]
		public void CriticWithNoRatingsShouldBeTotallyIgnored()
		{
			Critic target;
			var critics = DbWithOneCritic(History(), out target);

			CriticTrustworthiness.DecideHowMuchToTrustEachCritic(critics);
			target.RatingWeight.Should().BeApproximately(UninitializedTrustLevel, 0.0001);
		}

		[Test]
		public void CriticWithOneCrazyReviewAndSeveralSpotOnReviewsShouldBeTrusted()
		{
			CriticShouldBeTrustedToCorrectDegree(CriticTrustworthiness.Trustworthy,
				History(Opinion(TwoStarMovie, 5), Opinion(ThreeStarMovie, 3), Opinion(FourStarMovie, 4)));
		}

		[Test]
		public void CriticWithOneCrazyReviewShouldBeUntrusted()
		{
			CriticShouldBeTrustedToCorrectDegree(CriticTrustworthiness.Untrustworthy, History(Opinion(TwoStarMovie, 5)));
		}

		private static DataTablePort<Critic> DbWithOneCritic(Opinion[] ratingHistory, out Critic target)
		{
			var critics = Empty.Table<Critic>();
			target = Critic.Create(5);
			target.RatingWeight = UninitializedTrustLevel;
			critics.Save(target);
			critics.PersistAll();
			target.RateAllMovies(ratingHistory);
			return critics;
		}

		private static void CriticShouldBeTrustedToCorrectDegree(double criticTrustworthiness, params Opinion[] ratingHistory)
		{
			var testSubject = Critic.Create(5);
			testSubject.RateAllMovies(ratingHistory);

			testSubject.SetTrustworthiness();
			testSubject.RatingWeight.Should().BeApproximately(criticTrustworthiness, 0.0001);
		}

		private static Opinion[] History(params Opinion[] opinions)
		{
			return opinions;
		}

		private static Opinion Opinion(Movie movie, int stars)
		{
			return new Opinion(movie, stars);
		}

		private const double UninitializedTrustLevel = 3.14159;

		private static readonly Movie ThreeStarMovie = new Movie
		{
			Id = 9,
			AverageRating = 3
		};

		private static readonly Movie FourStarMovie = new Movie
		{
			Id = 11,
			AverageRating = 4
		};

		private static readonly Movie UnknownStarMovie = new Movie
		{
			Id = 12,
			AverageRating = null
		};

		private static readonly Movie TwoStarMovie = new Movie
		{
			Id = 10,
			AverageRating = 2
		};
	}
}
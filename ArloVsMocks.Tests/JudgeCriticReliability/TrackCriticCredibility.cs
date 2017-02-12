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
			CriticShouldBeTrustedToCorrectDegree(CriticTrustworthiness.TypicalCriticWeight,
				History(Opinion(TwoStarMovie, 3), Opinion(ThreeStarMovie, 4), Opinion(FourStarMovie, 2)));
		}

		[Test]
		public void CriticWhoOnlyReviewedUnknownMoviesShouldBeFullyTrusted()
		{
			CriticShouldBeTrustedToCorrectDegree(CriticTrustworthiness.TrustworthyCriticWeight, History(Opinion(UnknownStarMovie, 5)));
		}

		[Test]
		public void CriticWithAbnormalReviewsShouldBeMostlyIgnored()
		{
			CriticShouldBeTrustedToCorrectDegree(CriticTrustworthiness.UntrustworthyCriticWeight,
				History(Opinion(ThreeStarMovie, 1), Opinion(TwoStarMovie, 5)));
		}

		[Test]
		public void CriticWithNoRatingsShouldBeTotallyIgnored()
		{
			CriticShouldBeTrustedToCorrectDegree(0.0, History());
		}

		[Test]
		public void CriticWithOneCrazyReviewAndSeveralSpotOnReviewsShouldBeTrusted()
		{
			CriticShouldBeTrustedToCorrectDegree(CriticTrustworthiness.TrustworthyCriticWeight,
				History(Opinion(TwoStarMovie, 5), Opinion(ThreeStarMovie, 3), Opinion(FourStarMovie, 4)));
		}

		[Test]
		public void CriticWithOneCrazyReviewShouldBeUntrusted()
		{
			CriticShouldBeTrustedToCorrectDegree(CriticTrustworthiness.UntrustworthyCriticWeight, History(Opinion(TwoStarMovie, 5)));
		}

		private static void CriticShouldBeTrustedToCorrectDegree(double criticTrustworthiness, params Opinion[] ratingHistory)
		{
			Critic target;
			var critics = DbWithOneCritic(out target);
			target.RateAllMovies(ratingHistory);

			CriticTrustworthiness.UpdateCriticRatingWeightAccordingToHowSimilarTheyAreToAverage(critics);
			target.RatingWeight.Should().BeApproximately(criticTrustworthiness, 0.0001);
		}

		private static Opinion[] History(params Opinion[] opinions)
		{
			return opinions;
		}

		private static Opinion Opinion(Movie movie, int stars)
		{
			return new Opinion(movie, stars);
		}

		private static DataTablePort<Critic> DbWithOneCritic(out Critic target)
		{
			var critics = Empty.Table<Critic>();
			target = Critic.Create(5);
			critics.Save(target);
			critics.PersistAll();
			return critics;
		}

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
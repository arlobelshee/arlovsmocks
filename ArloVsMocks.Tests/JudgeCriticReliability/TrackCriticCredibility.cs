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
			var ratingHistory = History(Opinion(TwoStarMovie, 3), Opinion(ThreeStarMovie, 4), Opinion(FourStarMovie, 2));
			var criticTrustworthiness = Program.TypicalCriticWeight;
			CriticShouldBeTrustedToCorrectDegree(ratingHistory, criticTrustworthiness);
		}

		[Test]
		public void CriticWithAbnormalReviewsShouldBeMostlyIgnored()
		{
			var ratingHistory = History(Opinion(ThreeStarMovie, 1), Opinion(TwoStarMovie, 5));
			var criticTrustworthiness = Program.UntrustworthyCriticWeight;
			CriticShouldBeTrustedToCorrectDegree(ratingHistory, criticTrustworthiness);
		}

		[Test]
		public void CriticWithNoRatingsShouldBeTotallyIgnored()
		{
			var ratingHistory = History();
			var criticTrustworthiness = 0.0;
			CriticShouldBeTrustedToCorrectDegree(ratingHistory, criticTrustworthiness);
		}

		[Test]
		public void CriticWithOneCrazyReviewAndSeveralSpotOnReviewsShouldBeTrusted()
		{
			var ratingHistory = History(Opinion(TwoStarMovie, 5), Opinion(ThreeStarMovie, 3), Opinion(FourStarMovie, 4));
			var criticTrustworthiness = Program.TrustworthyCriticWeight;
			CriticShouldBeTrustedToCorrectDegree(ratingHistory, criticTrustworthiness);
		}

		[Test]
		public void CriticWithOneCrazyReviewShouldBeUntrusted()
		{
			var ratingHistory = History(Opinion(TwoStarMovie, 5));
			var criticTrustworthiness = Program.UntrustworthyCriticWeight;
			CriticShouldBeTrustedToCorrectDegree(ratingHistory, criticTrustworthiness);
		}

		[Test]
		public void CriticWhoOnlyReviewedUnknownMoviesShouldBeFullyTrusted()
		{
			var ratingHistory = History(Opinion(UnknownStarMovie, 5));
			var criticTrustworthiness = Program.TrustworthyCriticWeight;
			CriticShouldBeTrustedToCorrectDegree(ratingHistory, criticTrustworthiness);
		}

		private static void CriticShouldBeTrustedToCorrectDegree(Opinion[] ratingHistory, double criticTrustworthiness)
		{
			Critic target;
			var critics = DbWithOneCritic(out target);
			target.RateAllMovies(ratingHistory);

			Program.UpdateCriticRatingWeightAccordingToHowSimilarTheyAreToAverage(critics);
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
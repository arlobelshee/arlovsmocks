using ArloVsMocks.Data;
using ArloVsMocks.Tests.zzTestHelpers;
using FluentAssertions;
using NUnit.Framework;

namespace ArloVsMocks.Tests
{
	[TestFixture]
	public class TrackCriticCredibility
	{
		[Test]
		public void CriticWithAbnormalReviewsShouldBeMostlyIgnored()
		{
			Critic target;
			var critics = DbWithOneCritic(out target);
			target.RateMovie(ThreeStarMovie, 1);
			target.RateMovie(TwoStarMovie, 5);

			Program.UpdateCriticRatingWeightAccordingToHowSimilarTheyAreToAverage(critics);
			target.RatingWeight.Should().BeApproximately(Program.UntrustworthyCriticWeight, 0.0001);
		}

		[Test]
		public void CriticWithNoRatingsShouldBeTotallyIgnored()
		{
			Critic target;
			var critics = DbWithOneCritic(out target);

			Program.UpdateCriticRatingWeightAccordingToHowSimilarTheyAreToAverage(critics);
			target.RatingWeight.Should().BeApproximately(0.0, 0.0001);
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

		private static readonly Movie TwoStarMovie = new Movie
		{
			Id = 10,
			AverageRating = 2
		};
	}
}
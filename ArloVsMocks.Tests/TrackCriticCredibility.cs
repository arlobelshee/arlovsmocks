using ArloVsMocks.Data;
using ArloVsMocks.Tests.zzTestHelpers;
using FluentAssertions;
using NUnit.Framework;

namespace ArloVsMocks.Tests
{
	[TestFixture]
	public class TrackCriticCredibility
	{
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

		[Test]
		public void CriticWithAbnormalReviewsShouldBeMostlyIgnored()
		{
			var critics = Empty.Table<Critic>();
			var target = Critic.Create(5);
			critics.Save(target);
			critics.PersistAll();
			TestExtensions.RateMovie(target, ThreeStarMovie, 1);
			target.Ratings.Add(new Rating
			{
				Critic = target,
				Movie = TwoStarMovie,
				Stars = 5
			});

			Program.UpdateCriticRatingWeightAccordingToHowSimilarTheyAreToAverage(critics);
			target.RatingWeight.Should().BeApproximately(Program.UntrustworthyCriticWeight, 0.0001);
		}

		[Test]
		public void CriticWithNoRatingsShouldBeTotallyIgnored()
		{
			var critics = Empty.Table<Critic>();
			var target = Critic.Create(5);
			critics.Save(target);
			critics.PersistAll();

			Program.UpdateCriticRatingWeightAccordingToHowSimilarTheyAreToAverage(critics);
			target.RatingWeight.Should().BeApproximately(0.0, 0.0001);
		}
	}
}
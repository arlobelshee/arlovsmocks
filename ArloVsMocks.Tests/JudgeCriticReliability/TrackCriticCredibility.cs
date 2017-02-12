﻿using ArloVsMocks.Data;
using ArloVsMocks.Tests.zzTestHelpers;
using FluentAssertions;
using NUnit.Framework;

namespace ArloVsMocks.Tests.JudgeCriticReliability
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
		public void CriticWithOneCrazyReviewShouldBeUntrusted()
		{
			Critic target;
			var critics = DbWithOneCritic(out target);
			target.RateMovie(TwoStarMovie, 5);

			Program.UpdateCriticRatingWeightAccordingToHowSimilarTheyAreToAverage(critics);
			target.RatingWeight.Should().BeApproximately(Program.UntrustworthyCriticWeight, 0.0001);
		}

		[Test]
		public void CriticWithOneCrazyReviewAndSeveralSpotOnReviewsShouldBeTrusted()
		{
			Critic target;
			var critics = DbWithOneCritic(out target);
			target.RateMovie(TwoStarMovie, 5);
			target.RateMovie(ThreeStarMovie, 3);
			target.RateMovie(FourStarMovie, 4);

			Program.UpdateCriticRatingWeightAccordingToHowSimilarTheyAreToAverage(critics);
			target.RatingWeight.Should().BeApproximately(Program.TrustworthyCriticWeight, 0.0001);
		}

		[Test]
		public void CriticGenerallyCLoseButNotOnShouldBeTypical()
		{
			Critic target;
			var critics = DbWithOneCritic(out target);
			target.RateMovie(TwoStarMovie, 3);
			target.RateMovie(ThreeStarMovie, 4);
			target.RateMovie(FourStarMovie, 2);

			Program.UpdateCriticRatingWeightAccordingToHowSimilarTheyAreToAverage(critics);
			target.RatingWeight.Should().BeApproximately(Program.TypicalCriticWeight, 0.0001);
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
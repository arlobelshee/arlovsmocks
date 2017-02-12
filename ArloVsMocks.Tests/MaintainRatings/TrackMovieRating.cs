using ArloVsMocks.Data;
using FluentAssertions;
using NUnit.Framework;

namespace ArloVsMocks.Tests.MaintainRatings
{
	[TestFixture]
	public class TrackMovieRating
	{
		[Test]
		public void MovieWithNoRatingsShouldHaveRatingSetToNaNWhichIsProbablyABug()
		{
			var movie = Movie.Create(4);
			Program.UpdateAverageRatingForMovie(movie);
			movie.AverageRating.Should().Be(double.NaN);
		}

		[Test]
		public void MovieWithOneRatingShouldHaveThatAsItsAverage()
		{
			var movie = Movie.Create(4);
			AddRating(movie, 0.33, 3);
			Program.UpdateAverageRatingForMovie(movie);
			movie.AverageRating.Should().BeApproximately(3.0, 0.0000001);
		}

		[Test]
		public void MovieWithMultipleRatingsShouldHaveAWeightedMean()
		{
			var movie = Movie.Create(4);
			AddRating(movie, 0.33, 3);
			AddRating(movie, 1.0, 5);
			AddRating(movie, 0.15, 2);
			AddRating(movie, 0.15, 2);
			Program.UpdateAverageRatingForMovie(movie);
			movie.AverageRating.Should().BeApproximately(4.0429447852, 0.0000001);
		}

		private static void AddRating(Movie movie, double criticTrustworthiness, int stars)
		{
			movie.Ratings.Add(new Rating
			{
				Critic = new Critic
				{
					RatingWeight = criticTrustworthiness
				},
				Stars = stars
			});
		}
	}
}
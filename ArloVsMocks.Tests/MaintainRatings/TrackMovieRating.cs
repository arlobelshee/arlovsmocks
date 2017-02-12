using System.Linq;
using ArloVsMocks.Data;
using ArloVsMocks.Tests.zzTestHelpers;
using FluentAssertions;
using NUnit.Framework;

namespace ArloVsMocks.Tests.MaintainRatings
{
	[TestFixture]
	public class TrackMovieRating
	{
		[Test]
		public void MovieWithMultipleRatingsShouldHaveAWeightedMean()
		{
			var movie = Movie.Create(4);
			AddRating(movie, 0.33, 3);
			AddRating(movie, 1.0, 5);
			AddRating(movie, 0.15, 2);
			AddRating(movie, 0.15, 2);
			MovieRatings.UpdateAverageRatingForMovie(movie);
			movie.AverageRating.Should().BeApproximately(4.0429447852, 0.0000001);
		}

		[Test]
		public void MovieWithNoRatingsShouldHaveRatingSetToNaNWhichIsProbablyABug()
		{
			var movie = Movie.Create(4);
			MovieRatings.UpdateAverageRatingForMovie(movie);
			movie.AverageRating.Should().Be(double.NaN);
		}

		[Test]
		public void MovieWithOneRatingShouldHaveThatAsItsAverage()
		{
			var movie = Movie.Create(4);
			AddRating(movie, 0.33, 3);
			MovieRatings.UpdateAverageRatingForMovie(movie);
			movie.AverageRating.Should().BeApproximately(3.0, 0.0000001);
		}

		[Test]
		public void ShouldUpdateAllMovieRatingsInTheDatabase()
		{
			var movies = Empty.Table<Movie>();
			AddMovieWithOneRating(movies, 0.33, 3);
			AddMovieWithOneRating(movies, 1.0, 3);
			movies.PersistAll();
			MovieRatings.RecalcWeightedAveragesOfAllMovieRatings(movies);
			movies.ExistingData.All(m => m.AverageRating == 3.0);
			movies.ExistingData.Should().HaveCount(2);
		}

		private static void AddMovieWithOneRating(DataTablePort<Movie> movies, double criticTrustworthiness, int stars)
		{
			var movie = Movie.Create(4);
			AddRating(movie, criticTrustworthiness, stars);
			movies.Save(movie);
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
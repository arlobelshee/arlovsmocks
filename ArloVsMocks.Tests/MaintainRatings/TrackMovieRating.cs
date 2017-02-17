using System.Linq;
using ArloVsMocks.CritiqueMovies;
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
			var testSubject = Movie.Create(4);
			AddRating(testSubject, 0.33, 3);
			AddRating(testSubject, 1.0, 5);
			AddRating(testSubject, 0.15, 2);
			AddRating(testSubject, 0.15, 2);
			testSubject.UpdateAverageRating();
			testSubject.AverageRating.Should().BeApproximately(4.0429447852, 0.0000001);
		}

		[Test]
		[Category("probably a bug")]
		public void MovieWithNoRatingsShouldHaveRatingSetToNaNInsteadOfNullEvenThoughOtherCodeExpectsNullToMeanUnrated()
		{
			var testSubject = Movie.Create(4);
			testSubject.UpdateAverageRating();
			testSubject.AverageRating.Should().Be(double.NaN);
		}

		[Test]
		public void MovieWithOneRatingShouldHaveThatAsItsAverage()
		{
			var testSubject = Movie.Create(4);
			AddRating(testSubject, 0.33, 3);
			testSubject.UpdateAverageRating();
			testSubject.AverageRating.Should().BeApproximately(3.0, 0.0000001);
		}

		[Test]
		public void ShouldUpdateAllMovieRatingsInTheDatabaseEvenThoseWithoutRatings()
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
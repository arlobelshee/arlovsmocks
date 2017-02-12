using System.Linq;
using ArloVsMocks.Data;

namespace ArloVsMocks.CritiqueMovies
{
	public class MovieRatings
	{
		public static void RecalcWeightedAveragesOfAllMovieRatings(DataTablePort<Movie> movies)
		{
			foreach (var movie in movies.ExistingData)
				UpdateAverageRatingForMovie(movie);
		}

		public static void UpdateAverageRatingForMovie(Movie movie)
		{
			var weightTotal = movie.Ratings.Select(r => r.Critic.RatingWeight).Sum();
			var ratingTotal = movie.Ratings.Select(r => r.Stars*r.Critic.RatingWeight).Sum();

			movie.AverageRating = ratingTotal/weightTotal;
		}

		public static void UpsertRating(DataTablePort<Rating> dataTablePort, int movie, int critic, int stars)
		{
			var existingRating =
				dataTablePort.ExistingData.SingleOrDefault(r => (r.MovieId == movie) && (r.CriticId == critic));
			if (existingRating == null)
			{
				existingRating = new Rating
				{
					MovieId = movie,
					CriticId = critic
				};
				dataTablePort.Save(existingRating);
			}
			existingRating.Stars = stars;
		}
	}
}
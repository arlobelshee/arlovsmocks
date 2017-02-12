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

		public static void UpsertRating(DataTablePort<Rating> dataTablePort, Critique critique)
		{
			DoUpsert(dataTablePort, critique);
		}

		private static void DoUpsert(DataTablePort<Rating> dataTablePort, Critique critique)
		{
			var existingRating =
				dataTablePort.ExistingData.SingleOrDefault(r => (r.MovieId == critique.MovieId) && (r.CriticId == critique.CriticId));
			if (existingRating == null)
			{
				existingRating = new Rating
				{
					MovieId = critique.MovieId,
					CriticId = critique.CriticId
				};
				dataTablePort.Save(existingRating);
			}
			existingRating.Stars = critique.Stars;
		}
	}
}
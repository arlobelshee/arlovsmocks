using System.Linq;
using ArloVsMocks.Data;

namespace ArloVsMocks.CritiqueMovies
{
	public class MovieRatings
	{
		public static void RecalcWeightedAveragesOfAllMovieRatings(DataTablePort<Movie> movies)
		{
			foreach (var movie in movies.ExistingData)
				movie.UpdateAverageRating();
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
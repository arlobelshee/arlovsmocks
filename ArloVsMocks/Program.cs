using System;
using System.Linq;
using ArloVsMocks.Data;

namespace ArloVsMocks
{
	public class Program
	{
		public const double UntrustworthyCriticWeight = 0.15;
		public const double TrustworthyCriticWeight = 1.0;
		public const double TypicalCriticWeight = 0.33;

		private static void Main(string[] args)
		{
			var critique = Critique.FromArgs(args);
			if (!critique.IsValid)
			{
				Console.WriteLine(critique.ErrorMessage);
				return;
			}

			try
			{
				using (var db = new MovieReviewEntities())
				{
					var ratings = db.Ratings.ToDataTablePort(db);
					var movies = db.Movies.ToDataTablePort(db);
					var critics = db.Critics.ToDataTablePort(db);

					UpsertRating(ratings, critique);
					UpdateCriticRatingWeightAccordingToHowSimilarTheyAreToAverage(critics);
					RecalcWeightedAveragesOfAllMovieRatings(movies);

					ratings.PersistAll();

					OutputSummary(critics, critique, movies);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			Console.ReadKey();
		}

		private static void OutputSummary(DataTablePort<Critic> critics, Critique critique, DataTablePort<Movie> movies)
		{
			var newCriticRatingWeight = critics.ExistingData.Single(c => c.Id == critique.CriticId).RatingWeight;
			var newMovieRating = movies.ExistingData.Single(m => m.Id == critique.MovieId).AverageRating.Value;
			Console.WriteLine("New critic rating weight: {0:N1}", newCriticRatingWeight);
			Console.WriteLine("New movie rating: {0:N1}", newMovieRating);
		}

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

		public static void UpdateCriticRatingWeightAccordingToHowSimilarTheyAreToAverage(DataTablePort<Critic> critics)
		{
			var criticsHavingRated = critics.ExistingData.Where(c => c.Ratings.Count > 0);
			foreach (var critic in criticsHavingRated)
			{
				var ratingsWithAverages = critic.Ratings.Where(r => r.Movie.AverageRating.HasValue).ToList();
				var totalDisparity = ratingsWithAverages.Sum(r => Math.Abs(r.Stars - r.Movie.AverageRating.Value));
				var relativeDisparity = totalDisparity/ratingsWithAverages.Count;

				critic.RatingWeight = relativeDisparity > 2
					? UntrustworthyCriticWeight
					: relativeDisparity > 1 ? TypicalCriticWeight : TrustworthyCriticWeight;
			}
		}

		public static void UpsertRating(DataTablePort<Rating> dataTablePort, Critique critique)
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
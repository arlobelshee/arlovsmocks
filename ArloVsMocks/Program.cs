using System;
using System.Linq;
using ArloVsMocks.Data;

namespace ArloVsMocks
{
	public class Program
	{
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
				var summary = ProcessNewCritiqueAndGenerateSummary(critique);
				summary.Output(Console.WriteLine);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			Console.ReadKey();
		}

		private static InfoForUser ProcessNewCritiqueAndGenerateSummary(Critique critique)
		{
			using (var db = new MovieReviewEntities())
			{
				var ratings = db.Ratings.ToDataTablePort(db);
				var movies = db.Movies.ToDataTablePort(db);
				var critics = db.Critics.ToDataTablePort(db);

				UpsertRating(ratings, critique);
				CriticTrustworthiness.UpdateCriticRatingWeightAccordingToHowSimilarTheyAreToAverage(critics);
				RecalcWeightedAveragesOfAllMovieRatings(movies);

				ratings.PersistAll();

				Movie reviewedMovie;
				var reviewingCritic = GetEntitiesRelatedToThisReview(critique, critics, movies, out reviewedMovie);
				return Summarize(reviewingCritic, reviewedMovie);
			}
		}

		private static Critic GetEntitiesRelatedToThisReview(Critique critique, DataTablePort<Critic> critics,
			DataTablePort<Movie> movies, out Movie reviewedMovie)
		{
			var reviewingCritic = critics.ExistingData.Single(c => c.Id == critique.CriticId);
			reviewedMovie = movies.ExistingData.Single(m => m.Id == critique.MovieId);
			return reviewingCritic;
		}

		private static InfoForUser Summarize(Critic reviewingCritic, Movie reviewedMovie)
		{
			var newCriticRatingWeight = reviewingCritic.RatingWeight;
			var newMovieRating = reviewedMovie.AverageRating.Value;
			return
				new InfoForUser(new[]
					{$"New critic rating weight: {newCriticRatingWeight:N1}", $"New movie rating: {newMovieRating:N1}"});
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
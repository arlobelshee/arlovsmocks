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
				ProcessNewCritique(critique);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			Console.ReadKey();
		}

		private static void ProcessNewCritique(Critique critique)
		{
			var summary = ProcessNewCritiqueAndGenerateSummary(critique);
			summary.Output();
		}

		private static Summary ProcessNewCritiqueAndGenerateSummary(Critique critique)
		{
			string[] messages;
			using (var db = new MovieReviewEntities())
			{
				var ratings = db.Ratings.ToDataTablePort(db);
				var movies = db.Movies.ToDataTablePort(db);
				var critics = db.Critics.ToDataTablePort(db);

				UpsertRating(ratings, critique);
				UpdateCriticRatingWeightAccordingToHowSimilarTheyAreToAverage(critics);
				RecalcWeightedAveragesOfAllMovieRatings(movies);

				ratings.PersistAll();

				Movie reviewedMovie;
				var reviewingCritic = GetEntitiesRelatedToThisReview(critique, critics, movies, out reviewedMovie);
				messages = Summarize(reviewingCritic, reviewedMovie);
			}
			return new Summary(messages);
		}

		private static Critic GetEntitiesRelatedToThisReview(Critique critique, DataTablePort<Critic> critics, DataTablePort<Movie> movies,
			out Movie reviewedMovie)
		{
			var reviewingCritic = critics.ExistingData.Single(c => c.Id == critique.CriticId);
			reviewedMovie = movies.ExistingData.Single(m => m.Id == critique.MovieId);
			return reviewingCritic;
		}

		private static string[] Summarize(Critic reviewingCritic, Movie reviewedMovie)
		{
			var newCriticRatingWeight = reviewingCritic.RatingWeight;
			var newMovieRating = reviewedMovie.AverageRating.Value;
			return new[] {$"New critic rating weight: {newCriticRatingWeight:N1}", $"New movie rating: {newMovieRating:N1}"};
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

	internal class Summary
	{
		private readonly string[] _messages;

		public Summary(string[] messages)
		{
			_messages = messages;
		}

		public void Output()
		{
			foreach (var message in _messages)
			{
				Action<string> writeLine = Console.WriteLine;
				writeLine(message);
			}
		}
	}
}
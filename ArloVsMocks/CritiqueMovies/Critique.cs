using System;
using System.Linq;
using ArloVsMocks.Data;
using ArloVsMocks.Ui;

namespace ArloVsMocks.CritiqueMovies
{
	public class Critique
	{
		public Critique(int movieId, int criticId, int stars)
		{
			MovieId = movieId;
			CriticId = criticId;
			Stars = stars;
			IsValid = true;
			ErrorMessage = String.Empty;
		}

		public Critique(string errorMessage)
		{
			MovieId = 0;
			CriticId = 0;
			Stars = 0;
			IsValid = false;
			ErrorMessage = errorMessage;
		}

		public int MovieId { get; }
		public int CriticId { get; }
		public int Stars { get; }
		public bool IsValid { get; }
		public string ErrorMessage { get; }

		public static Critique FromArgs(string[] args)
		{
			try
			{
				var movieId = Int32.Parse(args[0]);
				var criticId = Int32.Parse(args[1]);
				var stars = Int32.Parse(args[2]);
				return new Critique(movieId, criticId, stars);
			}
			catch (Exception ex)
			{
				return new Critique(ex.Message);
			}
		}

		public Rating ToRating()
		{
			var createdRating = new Rating
			{
				CriticId = CriticId,
				MovieId = MovieId,
				Stars = Stars
			};
			return createdRating;
		}

		public InfoForUser ProcessNewCritiqueAndGenerateSummary(DataTablePort<Rating> ratings, DataTablePort<Critic> critics, DataTablePort<Movie> movies)
		{
			UpsertRating(ratings);
			CriticTrustworthiness.DecideHowmuchToTrustEachCritic(critics);
			MovieRatings.RecalcWeightedAveragesOfAllMovieRatings(movies);

			ratings.PersistAll();

			Movie reviewedMovie;
			var reviewingCritic = GetEntitiesRelatedToThisReview(critics, movies, out reviewedMovie);
			return Summarize(reviewingCritic, reviewedMovie);
		}

		private Critic GetEntitiesRelatedToThisReview(DataTablePort<Critic> critics,
			DataTablePort<Movie> movies, out Movie reviewedMovie)
		{
			var reviewingCritic = critics.ExistingData.Single(c => c.Id == CriticId);
			reviewedMovie = movies.ExistingData.Single(m => m.Id == MovieId);
			return reviewingCritic;
		}

		public static InfoForUser Summarize(Critic reviewingCritic, Movie reviewedMovie)
		{
			var newCriticRatingWeight = reviewingCritic.RatingWeight;
			var newMovieRating = reviewedMovie.AverageRating.Value;
			return
				new InfoForUser(new[]
					{$"New critic rating weight: {newCriticRatingWeight:N1}", $"New movie rating: {newMovieRating:N1}"});
		}

		public void UpsertRating(DataTablePort<Rating> dataTablePort)
		{
			MovieRatings.UpsertRating(dataTablePort, MovieId, CriticId, Stars);
		}
	}
}
using System;
using System.Linq;
using ArloVsMocks.Data;

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
			ErrorMessage = string.Empty;
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
				var movieId = int.Parse(args[0]);
				var criticId = int.Parse(args[1]);
				var stars = int.Parse(args[2]);
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

		public ReviewImpact Process(DataTablePort<Rating> ratings, DataTablePort<Critic> critics, DataTablePort<Movie> movies)
		{
			UpsertRating(ratings);
			CriticTrustworthiness.DecideHowmuchToTrustEachCritic(critics);
			MovieRatings.RecalcWeightedAveragesOfAllMovieRatings(movies);

			ratings.PersistAll();

			return CalculateImpactOfThisReview(critics, movies);
		}

		private ReviewImpact CalculateImpactOfThisReview(DataTablePort<Critic> critics, DataTablePort<Movie> movies)
		{
			var reviewingCritic = critics.ExistingData.Single(c => c.Id == CriticId);
			var reviewedMovie = movies.ExistingData.Single(m => m.Id == MovieId);
			return new ReviewImpact(reviewingCritic, reviewedMovie);
		}

		public void UpsertRating(DataTablePort<Rating> dataTablePort)
		{
			MovieRatings.UpsertRating(dataTablePort, MovieId, CriticId, Stars);
		}
	}
}
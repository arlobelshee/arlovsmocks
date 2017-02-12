using System;
using System.Linq;
using ArloVsMocks.Data;
using ArloVsMocks.Ui;

namespace ArloVsMocks.CritiqueMovies
{
	public class Critique
	{
		private readonly int _criticId;
		private readonly int _movieId;
		private readonly int _stars;

		public Critique(int movieId, int criticId, int stars)
		{
			_movieId = movieId;
			_criticId = criticId;
			_stars = stars;
			IsValid = true;
			ErrorMessage = new InfoForUser();
		}

		public Critique(InfoForUser errorMessage)
		{
			_movieId = 0;
			_criticId = 0;
			_stars = 0;
			IsValid = false;
			ErrorMessage = errorMessage;
		}

		public bool IsValid { get; }
		public InfoForUser ErrorMessage { get; }

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
				return new Critique(new InfoForUser(ex.Message));
			}
		}

		public Rating ToRating()
		{
			var createdRating = new Rating
			{
				CriticId = _criticId,
				MovieId = _movieId,
				Stars = _stars
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
			var reviewingCritic = critics.ExistingData.Single(c => c.Id == _criticId);
			var reviewedMovie = movies.ExistingData.Single(m => m.Id == _movieId);
			return new ReviewImpact(reviewingCritic, reviewedMovie);
		}

		public void UpsertRating(DataTablePort<Rating> dataTablePort)
		{
			MovieRatings.UpsertRating(dataTablePort, _movieId, _criticId, _stars);
		}
	}
}
using System.Linq;
using ArloVsMocks.Data;
using ArloVsMocks.Ui;

namespace ArloVsMocks
{
	public class CritiqueProcessor
	{
		public static InfoForUser ProcessNewCritiqueAndGenerateSummary(Critique critique)
		{
			using (var db = new MovieReviewEntities())
			{
				var ratings = db.Ratings.ToDataTablePort(db);
				var movies = db.Movies.ToDataTablePort(db);
				var critics = db.Critics.ToDataTablePort(db);

				MovieRatings.UpsertRating(ratings, critique);
				CriticTrustworthiness.DecideHowmuchToTrustEachCritic(critics);
				MovieRatings.RecalcWeightedAveragesOfAllMovieRatings(movies);

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

		public static InfoForUser Summarize(Critic reviewingCritic, Movie reviewedMovie)
		{
			var newCriticRatingWeight = reviewingCritic.RatingWeight;
			var newMovieRating = reviewedMovie.AverageRating.Value;
			return
				new InfoForUser(new[]
					{$"New critic rating weight: {newCriticRatingWeight:N1}", $"New movie rating: {newMovieRating:N1}"});
		}
	}
}
using ArloVsMocks.Data;
using ArloVsMocks.Ui;

namespace ArloVsMocks.CritiqueMovies
{
	public class ReviewImpact
	{
		private readonly Critic _reviewingCritic;
		private readonly Movie _reviewedMovie;

		public ReviewImpact(Critic reviewingCritic, Movie reviewedMovie)
		{
			_reviewingCritic = reviewingCritic;
			_reviewedMovie = reviewedMovie;
		}

		public InfoForUser Summarize()
		{
			var newCriticRatingWeight = _reviewingCritic.RatingWeight;
			var newMovieRating = _reviewedMovie.AverageRating.Value;
			return
				new InfoForUser(new[]
					{$"New critic rating weight: {newCriticRatingWeight:N1}", $"New movie rating: {newMovieRating:N1}"});
		}
	}
}
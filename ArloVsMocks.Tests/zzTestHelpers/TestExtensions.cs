using ArloVsMocks.Data;

namespace ArloVsMocks.Tests.zzTestHelpers
{
	internal class Opinion
	{
		private Movie _movie;
		private int _stars;

		public Opinion(Movie movie, int stars)
		{
			_movie = movie;
			_stars = stars;
		}

		public Movie Movie
		{
			get { return _movie; }
		}

		public int Stars
		{
			get { return _stars; }
		}
	}

	internal static class TestExtensions
	{
		public static void RateMovie(this Critic target, Opinion opinion)
		{
			target.Ratings.Add(new Rating
			{
				Critic = target,
				Movie = opinion.Movie,
				Stars = opinion.Stars
			});
		}

		public static void RateAllMovies(this Critic target, Opinion[] ratingHistory)
		{
			foreach (var opinion in ratingHistory)
			{
				target.RateMovie(opinion);
			}
		}
	}
}
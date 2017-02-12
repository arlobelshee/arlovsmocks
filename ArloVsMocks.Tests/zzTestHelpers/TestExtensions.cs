using ArloVsMocks.Data;

namespace ArloVsMocks.Tests.zzTestHelpers
{
	internal static class TestExtensions
	{
		public static void RateMovie(Critic target, Movie movie, int stars)
		{
			target.Ratings.Add(new Rating
			{
				Critic = target,
				Movie = movie,
				Stars = stars
			});
		}
	}
}
using System;
using System.Linq;
using ArloVsMocks.Data;

namespace ArloVsMocks
{
	public class CriticTrustworthiness
	{
		public const double UntrustworthyCriticWeight = 0.15;
		public const double TrustworthyCriticWeight = 1.0;
		public const double TypicalCriticWeight = 0.33;

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
	}
}
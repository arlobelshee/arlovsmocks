using System;
using System.Linq;
using ArloVsMocks.Data;

namespace ArloVsMocks.CritiqueMovies
{
	public class CriticTrustworthiness
	{
		public const double Untrustworthy = 0.15;
		public const double Trustworthy = 1.0;
		public const double Typical = 0.33;

		public static void DecideHowmuchToTrustEachCritic(DataTablePort<Critic> critics)
		{
			var criticsHavingRated = critics.ExistingData.Where(c => c.Ratings.Count > 0);
			foreach (var critic in criticsHavingRated)
			{
				critic.SetTrustworthiness();
			}
		}
	}
}
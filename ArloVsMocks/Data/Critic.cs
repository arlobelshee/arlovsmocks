using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ArloVsMocks.CritiqueMovies;

namespace ArloVsMocks.Data
{
	[Table("Critic")]
	public class Critic
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public double RatingWeight { get; set; }

		public virtual ICollection<Rating> Ratings { get; set; }

		public static Critic Create(int id)
		{
			return new Critic
			{
				Id = id,
				Ratings = new List<Rating>()
			};
		}

		public void SetTrustworthiness()
		{
			var ratingsWithAverages = Ratings.Where(r => r.Movie.AverageRating.HasValue).ToList();
			var totalDisparity = ratingsWithAverages.Sum(r => Math.Abs(r.Stars - r.Movie.AverageRating.Value));
			var relativeDisparity = totalDisparity/ratingsWithAverages.Count;

			RatingWeight = relativeDisparity > 2 ? CriticTrustworthiness.Untrustworthy : relativeDisparity > 1 ? CriticTrustworthiness.Typical : CriticTrustworthiness.Trustworthy;
		}
	}
}
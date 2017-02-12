using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArloVsMocks.Data
{
	[Table("Movie")]
	public class Movie
	{
		[Key]
		public int Id { get; set; }

		public double? AverageRating { get; set; }

		public virtual ICollection<Rating> Ratings { get; set; }

		public static Movie Create(int id)
		{
			return new Movie
			{
				Id = id,
				Ratings = new List<Rating>(),
				AverageRating = null
			};
		}
	}
}
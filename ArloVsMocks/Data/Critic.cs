using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
	}
}
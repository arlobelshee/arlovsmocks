using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArloVsMocks.Data
{
	[Table("Rating")]
	public class Rating : IEquatable<Rating>
	{
		[Key]
		[Column(Order = 0)]
		[ForeignKey("Movie")]
		public int MovieId { get; set; }

		[Key]
		[Column(Order = 1)]
		[ForeignKey("Critic")]
		public int CriticId { get; set; }

		public int Stars { get; set; }

		public virtual Movie Movie { get; set; }
		public virtual Critic Critic { get; set; }

		public bool Equals(Rating other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (MovieId == other.MovieId) && (CriticId == other.CriticId) && (Stars == other.Stars);
		}

		public override string ToString()
		{
			return $"Rating{{{nameof(MovieId)}: {MovieId}, {nameof(CriticId)}: {CriticId}, {nameof(Stars)}: {Stars}}}";
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Rating);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = MovieId;
				hashCode = (hashCode*397) ^ CriticId;
				hashCode = (hashCode*397) ^ Stars;
				return hashCode;
			}
		}

		public static bool operator ==(Rating left, Rating right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Rating left, Rating right)
		{
			return !Equals(left, right);
		}
	}
}
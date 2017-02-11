using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

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
	}

	[Table("Movie")]
	public class Movie
	{
		[Key]
		public int Id { get; set; }

		public double? AverageRating { get; set; }

		public virtual ICollection<Rating> Ratings { get; set; }
	}

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

	public class MovieReviewEntities : DbContext
	{
		public MovieReviewEntities()
			: this("Data/MovieReviews")
		{
		}

		public MovieReviewEntities(string fileName)
			: base(new SqlCeConnectionFactory("System.Data.SqlServerCe.4.0").CreateConnection(fileName), true)
		{
		}

		public DbSet<Movie> Movies { get; set; }
		public DbSet<Critic> Critics { get; set; }
		public DbSet<Rating> Ratings { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Movie>()
				.HasMany(m => m.Ratings)
				.WithRequired(r => r.Movie);

			modelBuilder.Entity<Critic>()
				.HasMany(c => c.Ratings)
				.WithRequired(r => r.Critic);


			base.OnModelCreating(modelBuilder);
		}
	}
}
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

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
    public class Rating
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Movie")]
        public int MovieId { get; set; }
        [Key, Column(Order = 1)]
        [ForeignKey("Critic")]
        public int CriticId { get; set; }
        public int Stars { get; set; }

        public virtual Movie Movie { get; set; }
        public virtual Critic Critic { get; set; }
    }

    public class MovieReviewEntities : DbContext
    {
        public MovieReviewEntities()
            : base(new SqlCeConnectionFactory("System.Data.SqlServerCe.3.5").CreateConnection("Data/MovieReviews"), true)
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
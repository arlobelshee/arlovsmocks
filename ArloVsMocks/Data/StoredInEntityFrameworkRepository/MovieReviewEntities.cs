using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace ArloVsMocks.Data.StoredInEntityFrameworkRepository
{
	public class MovieReviewEntities : DbContext
	{
		public MovieReviewEntities() : this("Data/StoredInEntityFrameworkRepository/MovieReviews")
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
			modelBuilder.Entity<Movie>().HasMany(m => m.Ratings).WithRequired(r => r.Movie);
			modelBuilder.Entity<Critic>().HasMany(c => c.Ratings).WithRequired(r => r.Critic);
			base.OnModelCreating(modelBuilder);
		}
	}
}
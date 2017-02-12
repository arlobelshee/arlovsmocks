using ArloVsMocks.CritiqueMovies;
using ArloVsMocks.Data.StoredInEntityFrameworkRepository;
using ArloVsMocks.Ui;

namespace ArloVsMocks.Data
{
	public class CritiqueRepository
	{
		public static InfoForUser ProcessNewCritiqueAndGenerateSummary(Critique critique)
		{
			using (var db = new MovieReviewEntities())
			{
				var ratings = db.Ratings.ToDataTablePort(db);
				var movies = db.Movies.ToDataTablePort(db);
				var critics = db.Critics.ToDataTablePort(db);

				return critique.ProcessNewCritiqueAndGenerateSummary(ratings, critics, movies);
			}
		}
	}
}
using System;
using System.Linq;
using ArloVsMocks.Data;

namespace ArloVsMocks
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			//parse input
			int movieId;
			int criticId;
			int stars;
			try
			{
				movieId = int.Parse(args[0]);
				criticId = int.Parse(args[1]);
				stars = int.Parse(args[2]);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return;
			}
			var critique = new Critique(movieId, criticId, stars, false);
			stars = critique.Stars;
			movieId = critique.MovieId;
			criticId = critique.CriticId;

			//process rating
			MovieReviewEntities db = null;
			try
			{
				db = new MovieReviewEntities();

				//insert or update new rating
				var existingRating = db.Ratings.SingleOrDefault(r => (r.MovieId == movieId) && (r.CriticId == criticId));
				if (existingRating == null)
				{
					existingRating = new Rating {MovieId = movieId, CriticId = criticId};
					db.Ratings.Add(existingRating);
				}
				existingRating.Stars = stars;

				//update critic rating weight according to how closely their ratings match the average rating
				var criticsHavingRated = db.Critics.Where(c => c.Ratings.Count > 0);
				foreach (var critic in criticsHavingRated)
				{
					var ratingsWithAverages = critic.Ratings.Where(r => r.Movie.AverageRating.HasValue).ToList();
					var totalDisparity = ratingsWithAverages.Sum(r => Math.Abs(r.Stars - r.Movie.AverageRating.Value));
					var relativeDisparity = totalDisparity/ratingsWithAverages.Count;

					critic.RatingWeight = relativeDisparity > 2 ? 0.15 : relativeDisparity > 1 ? 0.33 : 1.0;
				}

				//re-calculate weighted average of all movie ratings
				foreach (var movie in db.Movies)
				{
					var weightTotal = movie.Ratings.Select(r => r.Critic.RatingWeight).Sum();
					var ratingTotal = movie.Ratings.Select(r => r.Stars*r.Critic.RatingWeight).Sum();

					movie.AverageRating = ratingTotal/weightTotal;
				}

				db.SaveChanges();

				//output summary
				var newCriticRatingWeight = db.Critics.Single(c => c.Id == criticId).RatingWeight;
				var newMovieRating = db.Movies.Single(m => m.Id == movieId).AverageRating.Value;
				Console.WriteLine("New critic rating weight: {0:N1}", newCriticRatingWeight);
				Console.WriteLine("New movie rating: {0:N1}", newMovieRating);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				if (db != null) db.Dispose();
			}

			Console.ReadKey();
		}
	}
}
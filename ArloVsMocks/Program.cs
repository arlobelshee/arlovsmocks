﻿using System;
using System.Data.Entity;
using System.Linq;
using ArloVsMocks.Data;

namespace ArloVsMocks
{
	internal class Ratings
	{
		private DbSet<Rating> _ratings;

		public Ratings(DbSet<Rating> ratings)
		{
			_ratings = ratings;
		}

		public DbSet<Rating> Ratings1
		{
			get { return _ratings; }
		}
	}

	internal class Program
	{
		private static void Main(string[] args)
		{
			var critique = Critique.FromArgs(args);
			if (!critique.IsValid)
			{
				Console.WriteLine(critique.ErrorMessage);
				return;
			}

			//process rating
			MovieReviewEntities db = null;
			try
			{
				db = new MovieReviewEntities();

				UpsertRating(new Ratings(db.Ratings), critique);

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
				var newCriticRatingWeight = db.Critics.Single(c => c.Id == critique.CriticId).RatingWeight;
				var newMovieRating = db.Movies.Single(m => m.Id == critique.MovieId).AverageRating.Value;
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

		private static void UpsertRating(Ratings ratings, Critique critique)
		{
			var existingRating =
				ratings.Ratings1.SingleOrDefault(r => (r.MovieId == critique.MovieId) && (r.CriticId == critique.CriticId));
			if (existingRating == null)
			{
				existingRating = new Rating {MovieId = critique.MovieId, CriticId = critique.CriticId};
				ratings.Ratings1.Add(existingRating);
			}
			existingRating.Stars = critique.Stars;
		}
	}
}
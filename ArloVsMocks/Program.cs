﻿using System;
using System.Linq;
using ArloVsMocks.Data;

namespace ArloVsMocks
{
	public class Program
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
				var ratings = db.Ratings.ToDataTablePort(db);

				UpsertRating(ratings, critique);
				UpdateCriticRatingWeightAccordingToHowSimilarTheyAreToAverage(db);

				//re-calculate weighted average of all movie ratings
				foreach (var movie in db.Movies)
				{
					var weightTotal = movie.Ratings.Select(r => r.Critic.RatingWeight).Sum();
					var ratingTotal = movie.Ratings.Select(r => r.Stars*r.Critic.RatingWeight).Sum();

					movie.AverageRating = ratingTotal/weightTotal;
				}

				ratings.PersistAll();

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
				db?.Dispose();
			}

			Console.ReadKey();
		}

		private static void UpdateCriticRatingWeightAccordingToHowSimilarTheyAreToAverage(MovieReviewEntities db)
		{
			var critics = db.Critics.ToDataTablePort(db);
			var criticsHavingRated = critics.ExistingData.Where(c => c.Ratings.Count > 0);
			foreach (var critic in criticsHavingRated)
			{
				var ratingsWithAverages = critic.Ratings.Where(r => r.Movie.AverageRating.HasValue).ToList();
				var totalDisparity = ratingsWithAverages.Sum(r => Math.Abs(r.Stars - r.Movie.AverageRating.Value));
				var relativeDisparity = totalDisparity/ratingsWithAverages.Count;

				critic.RatingWeight = relativeDisparity > 2 ? 0.15 : relativeDisparity > 1 ? 0.33 : 1.0;
			}
		}

		public static void UpsertRating(DataTablePort<Rating> dataTablePort, Critique critique)
		{
			var existingRating =
				dataTablePort.ExistingData.SingleOrDefault(r => (r.MovieId == critique.MovieId) && (r.CriticId == critique.CriticId));
			if (existingRating == null)
			{
				existingRating = new Rating
				{
					MovieId = critique.MovieId,
					CriticId = critique.CriticId
				};
				dataTablePort.Save(existingRating);
			}
			existingRating.Stars = critique.Stars;
		}
	}
}
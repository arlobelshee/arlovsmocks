using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ArloVsMocks.Data
{
	public class DataTablePortToEntityFrameworkAdapter<T> where T : class
	{
		public DataTablePortToEntityFrameworkAdapter(MovieReviewEntities db, DbSet<T> table)
		{
			Db = db;
			Table = table;
		}

		public MovieReviewEntities Db { get; }

		public DbSet<T> Table { get; }

		public Action PersistAll()
		{
			return () => Db.SaveChanges();
		}

		public Action<T> SaveItem()
		{
			return rating => Table.Add(rating);
		}
	}

	public static class RatingsExtensions
	{
		public static DataTablePort<T> ToDataTablePort<T>(this DbSet<T> table, MovieReviewEntities db) where T : class
		{
			var adapter = new DataTablePortToEntityFrameworkAdapter<T>(db, table);
			return new DataTablePort<T>(table, adapter.SaveItem(), adapter.PersistAll());
		}

		public static DataTablePort<Rating> AsDataTablePort(this HashSet<Rating> data)
		{
			return MakeDataPort(data, new ValidateRatingByRequiringPositiveIDs(false));
		}

		public static DataTablePort<T> AsDataTablePort<T>(this HashSet<T> data) where T : class
		{
			return MakeDataPort(data, new ValidateByAllowingAnything<T>());
		}

		private static DataTablePort<T> MakeDataPort<T>(HashSet<T> data, Validator<T> validator) where T : class
		{
			var nextState = new HashSet<T>(data);
			return new DataTablePort<T>(data.AsQueryable(), d =>
			{
				validator.Validate(d);
				nextState.Add(d);
			}, () =>
			{
				validator.ReportErrors();
				data.Clear();
				data.UnionWith(nextState);
			});
		}

		public static Rating ToRating(this Critique critique)
		{
			var createdRating = new Rating
			{
				CriticId = critique.CriticId,
				MovieId = critique.MovieId,
				Stars = critique.Stars
			};
			return createdRating;
		}
	}
}
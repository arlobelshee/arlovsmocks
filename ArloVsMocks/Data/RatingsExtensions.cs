using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ArloVsMocks.Data
{
	public class DataTablePortToHashSetAdapter<T> where T : class
	{
		public DataTablePortToHashSetAdapter(HashSet<T> data, Validator<T> validator)
		{
			Data = data;
			Validator = validator;
			NextState = new HashSet<T>();
		}

		public HashSet<T> Data { get; }

		public Validator<T> Validator { get; }

		public HashSet<T> NextState { get; }

		public static Action PersistAll(DataTablePortToHashSetAdapter<T> dataTablePortToHashSetAdapter)
		{
			return () =>
			{
				dataTablePortToHashSetAdapter.Validator.ReportErrors();
				dataTablePortToHashSetAdapter.Data.Clear();
				dataTablePortToHashSetAdapter.Data.UnionWith(dataTablePortToHashSetAdapter.NextState);
			};
		}

		public static Action<T> SaveItem(DataTablePortToHashSetAdapter<T> adapter)
		{
			return d =>
			{
				adapter.Validator.Validate(d);
				adapter.NextState.Add(d);
			};
		}
	}

	public static class RatingsExtensions
	{
		public static DataTablePort<T> ToDataTablePort<T>(this DbSet<T> table, MovieReviewEntities db) where T : class
		{
			var adapter = new DataTablePortToEntityFrameworkAdapter<T>(db, table);
			return new DataTablePort<T>(table, adapter.SaveItem, adapter.PersistAll);
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
			var adapter = new DataTablePortToHashSetAdapter<T>(data, validator);
			return new DataTablePort<T>(data.AsQueryable(), DataTablePortToHashSetAdapter<T>.SaveItem(adapter),
				DataTablePortToHashSetAdapter<T>.PersistAll(adapter));
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
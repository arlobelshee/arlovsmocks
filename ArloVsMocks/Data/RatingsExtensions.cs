using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ArloVsMocks.Data
{
	public class DataTablePortToHashSetAdapter<TT> where TT : class
	{
		public DataTablePortToHashSetAdapter(HashSet<TT> data, Validator<TT> validator)
		{
			Data = data;
			Validator = validator;
			NextState = new HashSet<TT>();
		}

		public HashSet<TT> Data { get; }

		public Validator<TT> Validator { get; }

		public HashSet<TT> NextState { get; }
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
			return new DataTablePort<T>(data.AsQueryable(), SaveItem(adapter), PersistAll(adapter));
		}

		private static Action PersistAll<T>(DataTablePortToHashSetAdapter<T> dataTablePortToHashSetAdapter) where T : class
		{
			return () =>
			{
				dataTablePortToHashSetAdapter.Validator.ReportErrors();
				dataTablePortToHashSetAdapter.Data.Clear();
				dataTablePortToHashSetAdapter.Data.UnionWith(dataTablePortToHashSetAdapter.NextState);
			};
		}

		private static Action<T> SaveItem<T>(DataTablePortToHashSetAdapter<T> adapter) where T : class
		{
			return d =>
			{
				adapter.Validator.Validate(d);
				adapter.NextState.Add(d);
			};
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
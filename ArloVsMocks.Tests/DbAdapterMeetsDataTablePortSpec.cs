using System;
using System.Data.SqlServerCe;
using System.IO;
using ArloVsMocks.Data;
using NUnit.Framework;

namespace ArloVsMocks.Tests
{
	[TestFixture, Category("PlatformMonitoring"), Explicit]
	public class DbAdapterMeetsDataTablePortSpec : DataTablePortSpec
	{
		[SetUp]
		public void OpenDatabase()
		{
			_fileName = $"Test_{Guid.NewGuid()}.sdf";
			using (var sqlCeEngine = new SqlCeEngine("DataSource = " + _fileName))
			{
				sqlCeEngine.CreateDatabase();
			}
			_db = new MovieReviewEntities("DataSource = " + _fileName);
		}

		[TearDown]
		public void DropDatabase()
		{
			_db?.Dispose();
			File.Delete(_fileName);
		}

		private string _fileName;

		private MovieReviewEntities _db;

		protected override DataTablePort<Critic> CreateTestSubject()
		{
			return _db.Critics.ToDataTablePort(_db);
		}

		protected override DataTablePort<Rating> CreateTestSubjectWithFk()
		{
			return _db.Ratings.ToDataTablePort(_db);
		}
	}
}
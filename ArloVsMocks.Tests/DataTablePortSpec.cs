using System.Linq;
using ArloVsMocks.Data;
using FluentAssertions;
using NUnit.Framework;

namespace ArloVsMocks.Tests
{
	public abstract class DataTablePortSpec
	{
		protected abstract DataTablePort<Critic> CreateTestSubject();

		[Test]
		public void SavingItemShouldAddItToExistingDataAfterPersistAll()
		{
			var testSubject = CreateTestSubject();
			testSubject.ExistingData.Should().BeEmpty();
			var newItem = new Critic
			{
				Id = 4,
				RatingWeight = 2.717
			};
			testSubject.Save(newItem);
			testSubject.PersistAll();
			testSubject.ExistingData.Should().BeEquivalentTo(newItem);
		}

		[Test]
		public void SavingItemShouldHaveNoEffectBeforePersistAll()
		{
			var testSubject = CreateTestSubject();
			testSubject.ExistingData.Should().BeEmpty();
			var newItem = new Critic
			{
				Id = 4,
				RatingWeight = 2.717
			};
			testSubject.Save(newItem);
			testSubject.ExistingData.Should().BeEmpty();
		}

		[Test]
		public void LoadingAnItemAndModifyingItShouldUpdateStoredItem()
		{
			var testSubject = CreateTestSubject();
			var newItem = new Critic
			{
				Id = 4,
				RatingWeight = 2.171
			};
			testSubject.Save(newItem);
			testSubject.PersistAll();
			var selectedRating = testSubject.ExistingData.First();
			selectedRating.RatingWeight = 3.14159;
			testSubject.ExistingData.Should().BeEquivalentTo(selectedRating);
		}
	}
}
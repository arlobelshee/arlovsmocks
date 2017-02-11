using System.Linq;
using ArloVsMocks.Data;
using FluentAssertions;
using NUnit.Framework;

namespace ArloVsMocks.Tests
{
	public abstract class DataTablePortSpec
	{
		protected abstract DataTablePort CreateTestSubject();

		[Test]
		public void SavingItemShouldAddItToExistingDataAfterPersistAll()
		{
			var testSubject = CreateTestSubject();
			testSubject.ExistingData.Should().BeEmpty();
			var newItem = new Rating
			{
				CriticId = 1,
				MovieId = 2,
				Stars = 3
			};
			testSubject.Save(newItem);
			testSubject.PersistAll();
			testSubject.ExistingData.Should().BeEquivalentTo(newItem);
		}

		[Test]
		public void LoadingAnItemAndModifyingItShouldUpdateStoredItem()
		{
			var testSubject = CreateTestSubject();
			var newItem = new Rating
			{
				CriticId = 1,
				MovieId = 2,
				Stars = 3
			};
			testSubject.Save(newItem);
			testSubject.PersistAll();
			var selectedRating = testSubject.ExistingData.First();
			selectedRating.Stars = 5;
			testSubject.ExistingData.Should().BeEquivalentTo(selectedRating);
		}
	}
}
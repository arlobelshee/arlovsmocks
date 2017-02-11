using System.Collections.Generic;
using ArloVsMocks.Data;
using FluentAssertions;
using NUnit.Framework;

namespace ArloVsMocks.Tests
{
	[TestFixture]
	public class HashSetAdapterMeetsDataTablePortSpec
	{
		protected virtual DataTablePort CreateTestSubject()
		{
			return new HashSet<Rating>().AsDataTablePort();
		}

		[Test]
		public void SavingItemShouldAddItToExistingDataImmediately()
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
			testSubject.ExistingData.Should().BeEquivalentTo(newItem);
		}
	}
}
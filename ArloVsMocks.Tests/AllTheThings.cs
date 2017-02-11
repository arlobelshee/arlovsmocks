using FluentAssertions;
using NUnit.Framework;

namespace ArloVsMocks.Tests
{
	[TestFixture]
	public class AllTheThings
	{
		[Test]
		public void ValidInputsShouldMakeAValidCritique()
		{
			Critique.FromArgs(new[] {"2", "3", "4"}).ShouldBeEquivalentTo(new Critique(2, 3, 4));
		}

		[Test]
		public void InvalidInputsShouldMakeAnInvalidCritique()
		{
			Critique.FromArgs(new[] {"2", "3", "awesome"}).ShouldBeEquivalentTo(new Critique("Input string was not in a correct format."));
		}

		[Test]
		public void InsufficientInputsShouldMakeAnInvalidCritique()
		{
			Critique.FromArgs(new[] {"2", "3"}).ShouldBeEquivalentTo(new Critique("Index was outside the bounds of the array."));
		}
	}
}
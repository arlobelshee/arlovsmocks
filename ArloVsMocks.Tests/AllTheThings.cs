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
	}
}
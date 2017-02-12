using System.Text;
using ArloVsMocks.CritiqueMovies;
using ArloVsMocks.Data;
using FluentAssertions;
using NUnit.Framework;

namespace ArloVsMocks.Tests.MaintainRatings
{
	[TestFixture]
	public class InteractWithCritic
	{
		[Test]
		[Category("probably a bug")]
		public void InsufficientInputsShouldMakeAnInvalidCritiqueWithAnUninformativeUserMessage()
		{
			Critique.FromArgs(new[] {"2", "3"}).ShouldBeEquivalentTo(new Critique("Index was outside the bounds of the array."));
		}

		[Test]
		[Category("probably a bug")]
		public void InvalidInputsShouldMakeAnInvalidCritiqueWithAnUninformativeUserMessage()
		{
			Critique.FromArgs(new[] {"2", "3", "awesome"})
				.ShouldBeEquivalentTo(new Critique("Input string was not in a correct format."));
		}

		[Test]
		public void SummaryShouldIndicateChangesThatHappenedBasedOnReview()
		{
			var infoForUser = Critique.Summarize(new Critic
			{
				RatingWeight = 3.14159
			}, new Movie
			{
				AverageRating = 2.71829
			});
			var console = new StringBuilder();
			infoForUser.Output(s => console.AppendLine(s));
			console.ToString().Should().Be(@"New critic rating weight: 3.1
New movie rating: 2.7
");
		}

		[Test]
		public void ValidInputsShouldMakeAValidCritique()
		{
			Critique.FromArgs(new[] {"2", "3", "4"}).ShouldBeEquivalentTo(new Critique(2, 3, 4));
		}
	}
}
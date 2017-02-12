using ArloVsMocks.Data;
using ArloVsMocks.Tests.zzTestHelpers;
using FluentAssertions;
using NUnit.Framework;

namespace ArloVsMocks.Tests
{
	[TestFixture]
	public class TrackCriticCredibility
	{
		[Test]
		public void CriticWithNoRatingsShouldBeTotallyIgnored()
		{
			var critics = Empty.Table<Critic>();
			var target = Critic.Create(5);
			critics.Save(target);
			critics.PersistAll();

			Program.UpdateCriticRatingWeightAccordingToHowSimilarTheyAreToAverage(critics);
			target.RatingWeight.Should().BeApproximately(0.0, 0.0001);
		}
	}
}
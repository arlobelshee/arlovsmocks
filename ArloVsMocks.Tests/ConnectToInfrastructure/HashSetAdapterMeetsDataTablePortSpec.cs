using ArloVsMocks.Data;
using ArloVsMocks.Tests.zzTestHelpers;
using NUnit.Framework;

namespace ArloVsMocks.Tests.ConnectToInfrastructure
{
	[TestFixture]
	public class HashSetAdapterMeetsDataTablePortSpec : DataTablePortSpec
	{
		protected override DataTablePort<Critic> CreateTestSubject()
		{
			return Empty.Table<Critic>();
		}

		protected override DataTablePort<Rating> CreateTestSubjectWithFk()
		{
			return Empty.TableThatMonitorsForeignKeys();
		}
	}
}
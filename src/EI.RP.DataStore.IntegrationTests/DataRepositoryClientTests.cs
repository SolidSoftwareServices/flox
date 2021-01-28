using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using AutoFixture;
using EI.RP.CoreServices.Resiliency;
using EI.RP.DataModels.ResidentialPortal;
using EI.RP.DataServices;
using EI.RP.DataStore.IntegrationTests.Infrastructure;
using EI.RP.Stubs.IoC;
using NUnit.Framework;

namespace EI.RP.DataStore.IntegrationTests
{
	[TestFixture]
	public class DataRepositoryClientTests
	{
		
		private readonly ConcurrentBag<IContainer> _createdContainers=new ConcurrentBag<IContainer>();

		[OneTimeTearDown]
		public async Task OneTimeTearDown()
		{
			var tasks=new List<Task>();
			foreach (var container in _createdContainers)
			{
				tasks.Add(container.DisposeAsync().AsTask());
			}

			await Task.WhenAll(tasks);
		}



		public static IEnumerable<TestCaseData> EnvironmentCases(string prefixName)
		{
			var environmentNames = new[] {"test"};
			foreach (var environmentName in environmentNames)
			{
				var container = IoCContainerBuilder.From(new TestsModule(environmentName));
				yield return new TestCaseData(container).SetName($"{prefixName}.{environmentName}");
			}
		}

		[TestCaseSource(nameof(EnvironmentCases),new object[]{nameof(CanInsertThenGetCompetitionEntry)})]
		public async Task CanInsertThenGetCompetitionEntry(IContainer container)
		{
			var expected = new Fixture().Build<CompetitionEntryDto>().With(x=>x.Id,0)
				.With(x=>x.IpAddress,"127.0.0.1").Create();
			var target = container.Resolve<IResidentialPortalDataRepository>();
			expected=await ResilientOperations.Default.RetryIfNeeded(async()=>await target.AddCompetitionEntry(expected),maxAttempts:3);
			Assert.AreNotEqual(0,expected.Id);
			

			var actual=await ResilientOperations.Default.RetryIfNeeded(async ()=>await target.GetCompetitionEntry(expected.UserName),maxAttempts:3);
			Assert.AreEqual(expected, actual);
		}

		[TestCaseSource(nameof(EnvironmentCases),new object[]{nameof(CanInsertUpdateAndGetMoveHouse)})]
		public async Task CanInsertUpdateAndGetMoveHouse(IContainer container)
		{
			var fixture = new Fixture();
			var uniqueId = DateTime.UtcNow.Ticks;
			var expected = new MovingHouseProcessStatusDataModel()
			{
				UNIQUE_ID = uniqueId,
				USERNAME = "RMC7000100308@esb.ie",
				ELEC_PAYMENTMETHOD = "manual",
				ELECCONTRACT_ID = 2010985766,
				ELEC_CONTRACT_ACCOUNT = 904441660
			};

			Assert.AreEqual(0, expected.ID);

			
			var target = container.Resolve<IResidentialPortalDataRepository>();
			var added = await ResilientOperations.Default.RetryIfNeeded(async()=>await target.SetMovingHouseProcessStatus(expected),waitBetweenAttempts:TimeSpan.FromSeconds(2));
			Assert.AreNotEqual(0, added.ID);
			expected.ID = added.ID;
			Assert.AreEqual(expected,added);

			var toUpdate = await target.GetMovingHouseProcessStatus(uniqueId);
			Assert.AreEqual(added,toUpdate);
			

			toUpdate.TOWN = fixture.Create<string>();
			var updated = await ResilientOperations.Default.RetryIfNeeded(async()=>await target.SetMovingHouseProcessStatus(toUpdate),waitBetweenAttempts:TimeSpan.FromSeconds(2));

			Assert.AreEqual(toUpdate,updated);
			var actual = await target.GetMovingHouseProcessStatus(uniqueId);

			Assert.AreEqual(updated, actual);

		}

		[TestCase("SINGLE")]
		[TestCase("DUAL")]
		public async Task CanGetSmartActivationPlans(string groupName)
		{
			using (var container=IoCContainerBuilder.From(new TestsModule("preprod")))
			{
				var sut = container.Resolve<IResidentialPortalDataRepository>();
				var actual = await sut.GetSmartActivationPlans(groupName);

				Assert.IsNotEmpty(actual);
				Assert.IsTrue(actual.All(x=>x.GroupName==groupName));
			}
		}


	}
}
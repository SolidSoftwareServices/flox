using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using AutoFixture;
using EI.RP.DataModels.ResidentialPortal;
using EI.RP.DataServices;
using EI.RP.Stubs.IoC;
using EI.RP.SwitchApi.Clients.IntegrationTests.Infrastructure;
using NUnit.Framework;

namespace EI.RP.SwitchApi.Clients.IntegrationTests
{
	[TestFixture]
	public class SwitchDataRepositoryClientTests
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
			var environmentNames = new[]
			{
				//"development",  
				"test"
				//,"preprod"
			};
			foreach (var environmentName in environmentNames)
			{
				var container = IoCContainerBuilder.From(new TestsModule(environmentName));
				yield return new TestCaseData(container).SetName($"{prefixName}.{environmentName}");
			}
		}

		[TestCaseSource(nameof(EnvironmentCases),new object[]{nameof(CanGetDiscounts)})]
		public async Task CanGetDiscounts(IContainer container)
		{
			var target = container.Resolve<ISwitchDataRepository>();
			var discounts=await target.GetDiscountsAsync();
			Assert.IsNotEmpty(discounts);
		}

	}
}
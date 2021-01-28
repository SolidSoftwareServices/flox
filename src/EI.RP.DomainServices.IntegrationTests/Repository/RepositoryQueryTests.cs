using System;
using System.Threading.Tasks;
using Autofac;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using EI.RP.DataServices;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Repository
{
	[Explicit("TODO:TEST")]
	[TestFixture]
	public class RepositoryQueryTests : DomainTests
	{
		[Test]
		public async Task GetAllValuesOf()
		{
			await LoginUser("DFDD8@esb.ie", "Test3333");
			var repo = AssemblySetUp.Container.Value.Resolve<ISapRepositoryOfErpUmc>();
			var values = await repo.NewQuery<RegisterTypeDto>().GetMany();


			Console.WriteLine(string.Join(Environment.NewLine, values));
		}
	}
}
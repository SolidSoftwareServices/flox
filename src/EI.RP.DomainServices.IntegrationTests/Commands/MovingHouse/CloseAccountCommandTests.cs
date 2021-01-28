using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Metering;
using EI.RP.DomainModels.SpecimenBuilders;
using EI.RP.DomainServices.Commands.Contracts.CloseAccounts;
using EI.RP.DomainServices.Commands.Contracts.CloseAccounts.Model;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Commands.MovingHouse
{
	[Explicit]
	[TestFixture]
	public class CloseAccountCommandTests: DomainTests
	{

		[Test]
		public async Task CanExecute()
		{

			await LoginUser("elecequal8@esb.ie", "Test3333");

			await DomainCommandDispatcher.ExecuteAsync(new CloseAccountsCommand(ClientAccountType.Electricity, 
				new AddressInfo
				{
					AddressLine1="1addsdasdf",
					AddressLine2 = "2afffsdasdf",
					AddressLine4 = "4asdagggsdf",
					AddressLine5 = "1asdas33df",
					AddressType=AddressType.RepublicOfIreland,
					City="Dublin",Region = "Dublin"
				}
				,DateTime.Today,
				new ElectricityMeterReading("901699040", 2536),null,null)
				// new GasMeterReading("950493762",2525))
				,true);
		}
	}
}

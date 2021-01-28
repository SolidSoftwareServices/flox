using AutoFixture;
using Ei.Rp.DomainModels.Contracts;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using System.Collections.Generic;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.BusinessPartnersSearch.Infrastructure.StepsDataBuilder
{
	class BusinessPartnersDataBuilder
    {
	    readonly IFixture fixture = new Fixture().CustomizeDomainTypeBuilders();

		public IEnumerable<BusinessPartner> Create(int count = 30)
		{
			for (int i= 0; i<count; i++)
			{
				yield return new BusinessPartner
				{
					CommunicationsLevel = 0,
					Description = fixture.Create<string>(),
					MeterConfiguration = 0,
					NumPartner = fixture.Create<long>().ToString()
				};
			}
		}
    }
}

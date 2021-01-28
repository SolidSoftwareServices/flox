using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Banking;
using EI.RP.DomainServices.Queries.Banking.PaymentCards;
using EI.RP.TestServices;
using Moq;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Queries.Banking.PaymentCards
{
	[TestFixture]
	partial class PaymentCardsQueryHandlerTests:UnitTestFixture<PaymentCardsQueryHandlerTests.TestContext, PaymentCardsQueryHandler>
	{

		[Test]
		public async Task CanExecute()
		{
			//result
			var expected =Context.Fixture.CreateMany<PaymentCardDto>();
			//input
			var domainQuery = Context.Fixture.Create<PaymentCardsQuery>();

			//query
			var accountQueryMock = Context.AutoMocker.GetMock<IFluentODataModelQuery<AccountDto>>();
			accountQueryMock.Setup(_ => _.Key(domainQuery.Partner)).Returns(accountQueryMock.Object);
			var cardQueryMock = Context.AutoMocker.GetMock<IFluentODataModelQuery<PaymentCardDto>>();
			accountQueryMock.Setup(_ => _.NavigateTo<PaymentCardDto>()).Returns(cardQueryMock.Object);
			cardQueryMock.Setup(_ => _.GetMany(It.IsAny<bool>())).ReturnsAsync(expected);

			var repoMock = Context.AutoMocker.GetMock<ISapRepositoryOfCrmUmc>();
			repoMock.Setup(_ => _.NewQuery<AccountDto>()).Returns(accountQueryMock.Object);

			//act
			var actual=await Context.Sut.ExecuteQueryAsync<PaymentCardInfo>(domainQuery);

			CollectionAssert.AreEquivalent(expected.Select(dto =>
			{
				var map = new PaymentCardInfo
				{
					CardHolder = dto.Cardholder,
					Description = dto.Description,
					IsStandard = dto.StandardFlag,
					Issuer = dto.Issuer
				};
				return map;
			}),actual);

		}
		
	}

}
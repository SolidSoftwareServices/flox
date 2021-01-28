using System.Threading.Tasks;
using System.Collections.Generic;
using EI.RP.DomainServices.UnitTests.Infrastructure.CommonTestSutTypes;
using EI.RP.DomainServices.Commands.BusinessPartner.ActivateAsEBiller;
using EI.RP.DataServices;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.CoreServices.Ports.OData;
using Ei.Rp.DomainModels.MappingValues;
using NUnit.Framework;
using AutoFixture;
using Moq;

namespace EI.RP.DomainServices.UnitTests.Commands.BusinessPartner.ActivateAsEBiller
{
	internal class ActivateBusinessAgreementAsEBillerCommandHandlerTest : CommandHandlerTest<ActivateBusinessAgreementAsEBillerCommandHandler, ActivateBusinessAgreementAsEBillerCommand>
	{
		internal class CaseModel
		{
			public string InitialEBillerFlag { get; set; }
			public string CaseName { get; set; }
		}

		private static IEnumerable<TestCaseData> CanResolveCases()
		{
			var cases = new[]
			{
				new CaseModel { InitialEBillerFlag = SapBooleanFlag.No, CaseName = "BusinessContactWithBillerSapFlagNo" },
				new CaseModel { InitialEBillerFlag = SapBooleanFlag.Yes, CaseName = "BusinessContactWithBillerSapFlagYes" },
			};

			foreach (var caseItem in cases)
			{
				yield return new TestCaseData(caseItem)
					.SetName(caseItem.CaseName);
			}
		}

		[TestCaseSource(nameof(CanResolveCases))]
		public async Task ItActivateBusinessAgreementAsEBillerCommandExecutesCorrectly(CaseModel caseModel)
		{
			var businessAgreementID = Context.Fixture.Create<long>().ToString();
			var businessAgreementDto = GetBusinessAgreement(businessAgreementID, caseModel.InitialEBillerFlag);

			var businessAgreementQueryMock = Context.AutoMocker.GetMock<IFluentODataModelQuery<BusinessAgreementDto>>();
			businessAgreementQueryMock.Setup(x => x.Key(businessAgreementID)).Returns(businessAgreementQueryMock.Object);
			businessAgreementQueryMock.Setup(x => x.GetOne()).ReturnsAsync(businessAgreementDto);

			var repoMock = Context.AutoMocker.GetMock<ISapRepositoryOfCrmUmc>();
			repoMock.Setup(x => x.NewQuery<BusinessAgreementDto>()).Returns(businessAgreementQueryMock.Object);

			var cmd = ArrangeAndGetCommand(businessAgreementID);
			await Context.Sut.ExecuteAsync(cmd);

			Assert(businessAgreementDto, caseModel.InitialEBillerFlag);
		}

		BusinessAgreementDto GetBusinessAgreement(string businessAgreementID, string eBiller)
		{
			var businessAgreement = Context.Fixture
				.Build<BusinessAgreementDto>()
				.With(x => x.BusinessAgreementID, businessAgreementID)
				.With(x => x.EBiller, eBiller)
				.Without(x => x.ContractItems)
				.Without(x => x.IncomingAlternativePayerPaymentCard)
				.Without(x => x.IncomingPaymentMethod)
				.Without(x => x.OutgoingPaymentMethod5)
				.Without(x => x.AlternativePayer)
				.Without(x => x.OutgoingAlternativePayeePaymentCard)
				.Without(x => x.IncomingAlternativePayerBankAccount)
				.Without(x => x.OutgoingBankAccount)
				.Without(x => x.OutgoingPaymentMethod3)
				.Without(x => x.PaymentCard)
				.Without(x => x.Country)
				.Without(x => x.IncomingBankAccount)
				.Without(x => x.OutgoingAlternativePayeeBankAccount)
				.Without(x => x.OutgoingPaymentMethod4)
				.Without(x => x.OutgoingPaymentMethod2)
				.Without(x => x.IncomingPaymentCard)
				.Without(x => x.OutgoingPaymentCard)
				.Without(x => x.AlternativePayee)
				.Without(x => x.BillToAccountAddress)
				.Without(x => x.OutgoingPaymentMethod1)
				.Without(x => x.AccountAddress)
				.Without(x => x.CollectiveParent)
				.Without(x => x.AlternativePayerBuAg)
				.Create();

			return businessAgreement;
		}

		void Assert(BusinessAgreementDto businessAgreement, string eBillerFlag)
		{
			var cmd = Context.AutoMocker.GetMock<ISapRepositoryOfCrmUmc>();

			if (eBillerFlag != SapBooleanFlag.Yes)
			{
				businessAgreement.EBiller = SapBooleanFlag.Yes;
				cmd.Verify(x => x.Update(businessAgreement, It.IsAny<bool>()), Times.Once);
			}
			else
			{
				cmd.Verify(x => x.Update(businessAgreement, It.IsAny<bool>()), Times.Never);
			}
		}

		ActivateBusinessAgreementAsEBillerCommand ArrangeAndGetCommand(string businessAgreementId)
		{
			return new ActivateBusinessAgreementAsEBillerCommand(businessAgreementId);
		}
	}
}
using Ei.Rp.DomainErrors;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.DataServices.SAP.Clients.Infrastructure.Mappers;
using EI.RP.TestServices;
using NUnit.Framework;
using System.Collections.Generic;

namespace EI.RP.DataServices.SAP.Clients.UnitTests.Infrastructure.Mappers
{
    [TestFixture]
    internal class SapErrorMapperUnitTests : UnitTestFixture<
        UnitTestContext<SapErrorMapper>,
        SapErrorMapper>
    {
        public class CaseModel
        {
            public string ErrorCode { get; set; }
            public string ErrorMessage { get; set; }

            public DomainError Result { get; set; }
        }

        public static IEnumerable<TestCaseData> CanResolveCases()
        {
            var cases = new []
            {
                new CaseModel { ErrorCode = "ZMCFU/003", ErrorMessage = string.Empty, Result = ResidentialDomainError.CreateAccountTermsAndConditionsNotSelected},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_URM/015", ErrorMessage = string.Empty, Result = ResidentialDomainError.UserAlreadyExists},
                new CaseModel { ErrorCode = "ZMCFU/002", ErrorMessage = string.Empty, Result = ResidentialDomainError.Invalid_MPRN},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/023", ErrorMessage = string.Empty, Result = ResidentialDomainError.Invalid_AccountNumber},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/084", ErrorMessage = string.Empty, Result = ResidentialDomainError.AccountAlreadyExists},
                new CaseModel { ErrorCode = "ZMCFU/014", ErrorMessage = string.Empty, Result = ResidentialDomainError.BusinessAccount},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_URM/006", ErrorMessage = string.Empty, Result = ResidentialDomainError.ActionCancelled},
                new CaseModel { ErrorCode = "SY/530", ErrorMessage = string.Empty, Result = ResidentialDomainError.ResetPassword_InvalidEmail},

                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_RT/022", ErrorMessage = string.Empty, Result =ResidentialDomainError.CouldNotChangePasswordError},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_RT/022", ErrorMessage = "Choose a password that is different from your last 5 passwords.", Result = ResidentialDomainError.NewPasswordMustBeDifferentThanTheLast5Passwords},

                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_RT/022", ErrorMessage = "Name or password is incorrect (repeat logon)", Result = ResidentialDomainError.IncorrectPasswordError},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_RT/022", ErrorMessage = "You can change your password only once a day.", Result = ResidentialDomainError.ChangePasswordOncePerDayError},

                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/081", ErrorMessage = string.Empty, Result = ResidentialDomainError.MandatoryFieldMissing},
                new CaseModel { ErrorCode = "MDMXFW/033", ErrorMessage = string.Empty, Result = ResidentialDomainError.NoDataFound},
                new CaseModel { ErrorCode = "EL/424", ErrorMessage = string.Empty, Result = ResidentialDomainError.MeterReadingAlreadyReceived},
                new CaseModel { ErrorCode = "EL/355", ErrorMessage = string.Empty, Result = ResidentialDomainError.UnableToProcessRequest},
                new CaseModel { ErrorCode = "/IWFND/CM_BEC/026", ErrorMessage = string.Empty, Result = ResidentialDomainError.DataAlreadyReleased},
                new CaseModel { ErrorCode = "/IWFND/CM_BEC/026", ErrorMessage = "An exception occurred that was not caught.", Result = ResidentialDomainError.ThereHasBeenAnErrorPlsTryAgain},
                new CaseModel { ErrorCode = "ZMCFU/005", ErrorMessage = string.Empty, Result = ResidentialDomainError.InvalidBusinessAgreement},
                new CaseModel { ErrorCode = "ZMCFU/022", ErrorMessage = string.Empty, Result = ResidentialDomainError.ContractAlreadyRegistered},
                new CaseModel { ErrorCode = "EL/209", ErrorMessage = string.Empty, Result = ResidentialDomainError.UnableToProcessRequest},
                new CaseModel { ErrorCode = "ZM006", ErrorMessage = string.Empty, Result = ResidentialDomainError.UnableToProcessRequest},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_URM/022", ErrorMessage = string.Empty, Result = ResidentialDomainError.EnterAPasswordError},
                new CaseModel { ErrorCode = "CRM_BUPA_MKTPERM/202", ErrorMessage = string.Empty, Result = ResidentialDomainError.ContactDetailsUpToDate},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_URM/044", ErrorMessage = string.Empty, Result = ResidentialDomainError.ShouldNotBeSeenByEndUser},
                new CaseModel { ErrorCode = "ZS06", ErrorMessage = string.Empty, Result = ResidentialDomainError.UnableToProcessRequest},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/071", ErrorMessage = string.Empty, Result = ResidentialDomainError.ErrorWillNotOccur},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/070", ErrorMessage = string.Empty, Result = ResidentialDomainError.ErrorWillNotOccur},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_URM/003", ErrorMessage = string.Empty, Result = ResidentialDomainError.InvalidActivationLinkError},
                new CaseModel { ErrorCode = "ZMCFU/004", ErrorMessage = string.Empty, Result = ResidentialDomainError.AccountOwnerError},
                new CaseModel { ErrorCode = "ZMCFU/006", ErrorMessage = string.Empty, Result = ResidentialDomainError.ForAgentsDuringDeregistration},
                new CaseModel { ErrorCode = "ZMCFU/007", ErrorMessage = string.Empty, Result = ResidentialDomainError.ForAgentsDuringDeregistration},
                new CaseModel { ErrorCode = "ZMCFU/008", ErrorMessage = string.Empty, Result = ResidentialDomainError.ForAgentsDuringDeregistration},
                new CaseModel { ErrorCode = "ZMCFU/010", ErrorMessage = string.Empty, Result = ResidentialDomainError.DateOfBirthError},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_URM/043", ErrorMessage = string.Empty, Result = ResidentialDomainError.ShouldNotBeSeenByEndUser},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/091", ErrorMessage = string.Empty, Result = ResidentialDomainError.AddressUpdatesNotPermitted},
                new CaseModel { ErrorCode = "EL/459", ErrorMessage = string.Empty, Result = ResidentialDomainError.MeterReadingNextBillDue},
                new CaseModel { ErrorCode = "EL/422", ErrorMessage = string.Empty, Result = ResidentialDomainError.MeterReadingNextBillDue},
                new CaseModel { ErrorCode = "EL/100", ErrorMessage = string.Empty, Result = ResidentialDomainError.MeterReadingNextBillDue},
                new CaseModel { ErrorCode = "E9/100", ErrorMessage = string.Empty, Result = ResidentialDomainError.MeterReadingNextBillDue},
                new CaseModel { ErrorCode = "EL/573", ErrorMessage = string.Empty, Result = ResidentialDomainError.MeterReadingNextBillDue},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/111", ErrorMessage = string.Empty, Result = ResidentialDomainError.IBANInvalidError},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/022", ErrorMessage = string.Empty, Result = ResidentialDomainError.AccAlreadyRegisteredError},
                new CaseModel { ErrorCode = "ZEXIT_FEE", ErrorMessage = string.Empty, Result = ResidentialDomainError.FeeNotifier},
                new CaseModel { ErrorCode = "ZCRM_CHECK/064", ErrorMessage = string.Empty, Result = ResidentialDomainError.FeeNotifier},
                new CaseModel { ErrorCode = "ZGMO_FUT", ErrorMessage = string.Empty, Result = ResidentialDomainError.GasNoFutureDated},
                new CaseModel { ErrorCode = "ZGOSCHK", ErrorMessage = string.Empty, Result = ResidentialDomainError.SupplyGauranteeRemovedAlert},
                new CaseModel { ErrorCode = "ZINSTPLAN_W", ErrorMessage = string.Empty, Result = ResidentialDomainError.OpenInstallment},
                new CaseModel { ErrorCode = "ZBILLREV_W", ErrorMessage = string.Empty, Result = ResidentialDomainError.BillDateReversedAlert},
                new CaseModel { ErrorCode = "ZCOLLBACHECK_E147", ErrorMessage = string.Empty, Result = ResidentialDomainError.ContractCollectiveBill},
                new CaseModel { ErrorCode = "ZCOLLBACHECK_E148", ErrorMessage = string.Empty, Result = ResidentialDomainError.AccountPartOfCollectiveBill},
                new CaseModel { ErrorCode = "ZSITE_SWITCH_E149", ErrorMessage = string.Empty, Result = ResidentialDomainError.SiteSwitch},
                new CaseModel { ErrorCode = "ZDISCSTART_E152", ErrorMessage = string.Empty, Result = ResidentialDomainError.SiteBeingDisconnected},
                new CaseModel { ErrorCode = "ZENDDATE_E037", ErrorMessage = string.Empty, Result = ResidentialDomainError.ContractEndTooEarly},
                new CaseModel { ErrorCode = "ZGMOPAST014", ErrorMessage = string.Empty, Result = ResidentialDomainError.MoveOutThirtyDays},
                new CaseModel { ErrorCode = "ZMAXDEMAND021", ErrorMessage = string.Empty, Result = ResidentialDomainError.TokenMeterExists},
                new CaseModel { ErrorCode = "ZPODLOCK_E142", ErrorMessage = string.Empty, Result = ResidentialDomainError.PoDLocked},
                new CaseModel { ErrorCode = "ZPAYGO_E158", ErrorMessage = string.Empty, Result = ResidentialDomainError.PayAsYouGoNotSupported},
                new CaseModel { ErrorCode = "ZHASMANBIL_E130", ErrorMessage = string.Empty, Result = ResidentialDomainError.AnnualBillsAlert},
                new CaseModel { ErrorCode = "HASADJREVB_E140", ErrorMessage = string.Empty, Result = ResidentialDomainError.AdjustmentReversalBillsAlert},
                new CaseModel { ErrorCode = "ZCOLL_BILLP119", ErrorMessage = string.Empty, Result = ResidentialDomainError.ContractCollectiveBillQueryAlert},
                new CaseModel { ErrorCode = "ZOUTSIDEBILL_E105", ErrorMessage = string.Empty, Result = ResidentialDomainError.ExistingBillOutsideContract},
                new CaseModel { ErrorCode = "ZGAS001", ErrorMessage = string.Empty, Result = ResidentialDomainError.GasBillReversalNotPossible},
                new CaseModel { ErrorCode = "ZMOREBILLS072", ErrorMessage = string.Empty, Result = ResidentialDomainError.MoreThanFourBillsReversed},
                new CaseModel { ErrorCode = "ZREVPROT_E154", ErrorMessage = string.Empty, Result = ResidentialDomainError.RevenueProtection},
                new CaseModel { ErrorCode = "ZTOKENMTR_E021", ErrorMessage = string.Empty, Result = ResidentialDomainError.TokenMeterExistsContactESB},
                new CaseModel { ErrorCode = "ZINSTPLAN_W/098", ErrorMessage = string.Empty, Result = ResidentialDomainError.InstallmentPlanWillBeCancelled},
                new CaseModel { ErrorCode = "ZPODLOCK_E/142", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnErrorPlsTryAgain},
                new CaseModel { ErrorCode = "COM_PME_PARSER/101", ErrorMessage = string.Empty, Result = ResidentialDomainError.CantProcessMoveInMoveOut},
                new CaseModel { ErrorCode = "ZTOKENMTR_E/021", ErrorMessage = string.Empty, Result = ResidentialDomainError.CantProcessMoveInMoveOut},
                new CaseModel { ErrorCode = "ZSTAFFDISC_W/134", ErrorMessage = string.Empty, Result = ResidentialDomainError.CantProcessMoveInMoveOut},
                new CaseModel { ErrorCode = "ZSTAFFDISC_E/133", ErrorMessage = string.Empty, Result = ResidentialDomainError.CantProcessMoveInMoveOut},
                new CaseModel { ErrorCode = "ZSTAFFDIS2_E/135", ErrorMessage = string.Empty, Result = ResidentialDomainError.CantProcessMoveInMoveOut},
                new CaseModel { ErrorCode = "ZSITE_SWITCH_E/149", ErrorMessage = string.Empty, Result = ResidentialDomainError.CantProcessMoveInMoveOut},
                new CaseModel { ErrorCode = "ZREVPROT_E/154", ErrorMessage = string.Empty, Result = ResidentialDomainError.CantProcessMoveInMoveOut},
                new CaseModel { ErrorCode = "ZREVMVEIN/002", ErrorMessage = string.Empty, Result = ResidentialDomainError.CantProcessMoveInMoveOut},
                new CaseModel { ErrorCode = "ZRCDP/112", ErrorMessage = string.Empty, Result = ResidentialDomainError.CantProcessMoveInMoveOut},
                new CaseModel { ErrorCode = "ZPOD_MOV_EXST/008", ErrorMessage = string.Empty, Result = ResidentialDomainError.CantProcessMoveInMoveOut},
                new CaseModel { ErrorCode = "ZPOD_MOV_END/009", ErrorMessage = string.Empty, Result = ResidentialDomainError.CantProcessMoveInMoveOut},
                new CaseModel { ErrorCode = "ZPAYGO_E/158", ErrorMessage = string.Empty, Result = ResidentialDomainError.CantProcessMoveInMoveOut},
                new CaseModel { ErrorCode = "ZOUTSIDEBILL_E/105", ErrorMessage = string.Empty, Result = ResidentialDomainError.CantProcessMoveInMoveOut},
                new CaseModel { ErrorCode = "ZMOREBILLS/072", ErrorMessage = string.Empty, Result = ResidentialDomainError.CantProcessMoveInMoveOut},
                new CaseModel { ErrorCode = "ZMAXDEMAND/021", ErrorMessage = string.Empty, Result = ResidentialDomainError.CantProcessMoveInMoveOut},
                new CaseModel { ErrorCode = "ZHASMANBIL_E/130", ErrorMessage = string.Empty, Result = ResidentialDomainError.CantProcessMoveInMoveOut},
                new CaseModel { ErrorCode = "ZHASADJREVB_E/140", ErrorMessage = string.Empty, Result = ResidentialDomainError.CantProcessMoveInMoveOut},
                new CaseModel { ErrorCode = "ZGOSCHK/001", ErrorMessage = string.Empty, Result = ResidentialDomainError.CantProcessMoveInMoveOut},
                new CaseModel { ErrorCode = "ZGMOPAST/014", ErrorMessage = string.Empty, Result = ResidentialDomainError.CantProcessMoveInMoveOut},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_URM/001", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/001", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "ZMCFU/001", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_URM/002", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/002", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/003", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_URM/004", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/004", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_URM/005", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/005", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/006", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_URM/007", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/007", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_URM/008", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/008", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_URM/009", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/009", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "ZMCFU/009", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_URM/010", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/010", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_URM/011", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/011", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "ZMCFU/011", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/043", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_URM/047", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/047", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_URM/021", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/021", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_URM/019", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/044", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_URM/026", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/026", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/065", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/020", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "CRM_IU_UMC_ODATA/031", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_URM/023", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "/IWBEP/CM_MGW_URM/020", ErrorMessage = string.Empty, Result = ResidentialDomainError.ThereHasBeenAnError},
                new CaseModel { ErrorCode = "ECRM_ISU/081", ErrorMessage = string.Empty, Result = ResidentialDomainError.InvalidActiveContract},
				new CaseModel { ErrorCode = "ZCRM_BICO/000", ErrorMessage = string.Empty, Result = ResidentialDomainError.MeterSubmitOutOfTolerance},
				new CaseModel { ErrorCode = "ZCRM_CHECK/000", ErrorMessage = string.Empty, Result = ResidentialDomainError.ContractErrorPreventTheContactFromBeingSubmitted},
                new CaseModel { ErrorCode = string.Empty, ErrorMessage = string.Empty, Result = DomainError.Undefined},
                new CaseModel { ErrorCode = "/Invalid_Error", ErrorMessage = string.Empty, Result = DomainError.Undefined},
            };

            foreach (var caseItem in cases)
            {
                yield return new TestCaseData(caseItem).SetName($"{caseItem.ErrorCode} - {caseItem.ErrorMessage}").Returns(caseItem.Result);
            }
        }

        [TestCaseSource(nameof(CanResolveCases))]
        public DomainError CanResolve(CaseModel caseModel)
        {
            return Context.Sut.Map(caseModel.ErrorCode, caseModel.ErrorMessage);
        }
    }
}

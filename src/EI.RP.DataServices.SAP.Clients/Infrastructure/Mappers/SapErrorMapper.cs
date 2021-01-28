using Ei.Rp.DomainErrors;
using EI.RP.CoreServices.ErrorHandling;

namespace EI.RP.DataServices.SAP.Clients.Infrastructure.Mappers
{
    internal class SapErrorMapper : ISapErrorMapper
    {
        public DomainError Map(string errorCode, string errorMessage)
        {
            if (errorCode == null)
            {
                errorCode = "Unknown";
            }
            switch (errorCode.ToUpper())
            {
                case "ZMCFU/003":
                    return ResidentialDomainError.CreateAccountTermsAndConditionsNotSelected;
                case "/IWBEP/CM_MGW_URM/015":
                    return ResidentialDomainError.UserAlreadyExists;
                case "ZMCFU/002":
                    return ResidentialDomainError.Invalid_MPRN;
                case "CRM_IU_UMC_ODATA/023":
                    return ResidentialDomainError.Invalid_AccountNumber;
                case "CRM_IU_UMC_ODATA/084":
                    return ResidentialDomainError.AccountAlreadyExists;
                case "ZMCFU/014":
                    return ResidentialDomainError.BusinessAccount;
                case "/IWBEP/CM_MGW_URM/006":
                    return ResidentialDomainError.ActionCancelled;
                case "SY/530":
                    return ResidentialDomainError.ResetPassword_InvalidEmail;

                case "/IWBEP/CM_MGW_RT/022":
                    {
                        if (errorMessage.Contains(
                            "Choose a password that is different from your last 5 passwords."))
                        {
                            return ResidentialDomainError.NewPasswordMustBeDifferentThanTheLast5Passwords;
                        }

                        if (errorMessage.Contains("Name or password is incorrect (repeat logon)"))
                        {
                            return ResidentialDomainError.IncorrectPasswordError;
                        }

                        if (errorMessage.Contains("You can change your password only once a day."))
                        {
                            return ResidentialDomainError.ChangePasswordOncePerDayError;
                        }

                        return ResidentialDomainError.CouldNotChangePasswordError;
                    }
                case "CRM_IU_UMC_ODATA/081":
                    return ResidentialDomainError.MandatoryFieldMissing;
                case "MDMXFW/033":
                    return ResidentialDomainError.NoDataFound;
                case "EL/424":
                    return ResidentialDomainError.MeterReadingAlreadyReceived;
                case "/IWFND/CM_BEC/026":
                {
                    if (errorMessage.Contains("An exception occurred that was not caught."))
                    {
                        return ResidentialDomainError.ThereHasBeenAnErrorPlsTryAgain;
                    }

                    return ResidentialDomainError.DataAlreadyReleased;
                }
                case "ZMCFU/005":
                    return ResidentialDomainError.InvalidBusinessAgreement;
                case "ZMCFU/022":
                    return ResidentialDomainError.ContractAlreadyRegistered;
                case "EL/209":
                case "ZM006":
                case "EL/355":
                    return ResidentialDomainError.UnableToProcessRequest;
                case "/IWBEP/CM_MGW_URM/022":
                    return ResidentialDomainError.EnterAPasswordError;
                case "CRM_BUPA_MKTPERM/202":
                    return ResidentialDomainError.ContactDetailsUpToDate;
                case "/IWBEP/CM_MGW_URM/044":
                    return ResidentialDomainError.ShouldNotBeSeenByEndUser;
                case "ZS06":
                    return ResidentialDomainError.UnableToProcessRequest;
                case "CRM_IU_UMC_ODATA/071":
                case "CRM_IU_UMC_ODATA/070":
                    return ResidentialDomainError.ErrorWillNotOccur;
                case "/IWBEP/CM_MGW_URM/003":
                    return ResidentialDomainError.InvalidActivationLinkError;
                case "ZMCFU/004":
                    return ResidentialDomainError.AccountOwnerError;
                case "ZMCFU/006":
                case "ZMCFU/007":
                case "ZMCFU/008":
                    return ResidentialDomainError.ForAgentsDuringDeregistration;
                case "ZMCFU/010":
                    return ResidentialDomainError.DateOfBirthError;
                case "/IWBEP/CM_MGW_URM/043":
                    return ResidentialDomainError.ShouldNotBeSeenByEndUser;
                case "CRM_IU_UMC_ODATA/091":
                    return ResidentialDomainError.AddressUpdatesNotPermitted;

                case "EL/459":
                case "EL/422":
                case "EL/100":
                case "E9/100":
                case "EL/573":
                    return ResidentialDomainError.MeterReadingNextBillDue;
                case "CRM_IU_UMC_ODATA/111":
                    return ResidentialDomainError.IBANInvalidError;
                case "CRM_IU_UMC_ODATA/022":
                    return ResidentialDomainError.AccAlreadyRegisteredError;
                case "ZEXIT_FEE":
                case "ZCRM_CHECK/064":
                    return ResidentialDomainError.FeeNotifier;
                case "ZGMO_FUT":
                    return ResidentialDomainError.GasNoFutureDated;
                case "ZGOSCHK":
                    return ResidentialDomainError.SupplyGauranteeRemovedAlert;
                case "ZINSTPLAN_W":
                    return ResidentialDomainError.OpenInstallment;
                case "ZBILLREV_W":
                    return ResidentialDomainError.BillDateReversedAlert;
                case "ZCOLLBACHECK_E147":
                    return ResidentialDomainError.ContractCollectiveBill;
                case "ZCOLLBACHECK_E148":
                    return ResidentialDomainError.AccountPartOfCollectiveBill;
                case "ZSITE_SWITCH_E149":
                    return ResidentialDomainError.SiteSwitch;
                case "ZDISCSTART_E152":
                    return ResidentialDomainError.SiteBeingDisconnected;
                case "ZENDDATE_E037":
                    return ResidentialDomainError.ContractEndTooEarly;
                case "ZGMOPAST014":
                    return ResidentialDomainError.MoveOutThirtyDays;
                case "ZMAXDEMAND021":
                    return ResidentialDomainError.TokenMeterExists;
                case "ZPODLOCK_E142":
                    return ResidentialDomainError.PoDLocked;
                case "ZPAYGO_E158":
                    return ResidentialDomainError.PayAsYouGoNotSupported;
                case "ZHASMANBIL_E130":
                    return ResidentialDomainError.AnnualBillsAlert;
                case "HASADJREVB_E140":
                    return ResidentialDomainError.AdjustmentReversalBillsAlert;
                case "ZCOLL_BILLP119":
                    return ResidentialDomainError.ContractCollectiveBillQueryAlert;
                case "ZOUTSIDEBILL_E105":
                    return ResidentialDomainError.ExistingBillOutsideContract;
                case "ZGAS001":
                    return ResidentialDomainError.GasBillReversalNotPossible;
                case "ZMOREBILLS072":
                    return ResidentialDomainError.MoreThanFourBillsReversed;
                case "ZREVPROT_E154":
                    return ResidentialDomainError.RevenueProtection;
                case "ZTOKENMTR_E021":
                    return ResidentialDomainError.TokenMeterExistsContactESB;
                case "ZINSTPLAN_W/098":
                    return ResidentialDomainError.InstallmentPlanWillBeCancelled;
                case "ZPODLOCK_E/142":
                    return ResidentialDomainError.ThereHasBeenAnErrorPlsTryAgain;
                case "COM_PME_PARSER/101":
                case "ZTOKENMTR_E/021":
                case "ZSTAFFDISC_W/134":
                case "ZSTAFFDISC_E/133":
                case "ZSTAFFDIS2_E/135":
                case "ZSITE_SWITCH_E/149":
                case "ZREVPROT_E/154":
                case "ZREVMVEIN/002":
                case "ZRCDP/112":
                case "ZPOD_MOV_EXST/008":
                case "ZPOD_MOV_END/009":
                case "ZPAYGO_E/158":
                case "ZOUTSIDEBILL_E/105":
                case "ZMOREBILLS/072":
                case "ZMAXDEMAND/021":
                case "ZHASMANBIL_E/130":
                case "ZHASADJREVB_E/140":
                case "ZGOSCHK/001":
                case "ZGMOPAST/014":
                    return ResidentialDomainError.CantProcessMoveInMoveOut;
                case "/IWBEP/CM_MGW_URM/001":
                case "CRM_IU_UMC_ODATA/001":
                case "ZMCFU/001":
                case "/IWBEP/CM_MGW_URM/002":
                case "CRM_IU_UMC_ODATA/002":
                case "CRM_IU_UMC_ODATA/003":
                case "/IWBEP/CM_MGW_URM/004":
                case "CRM_IU_UMC_ODATA/004":
                case "/IWBEP/CM_MGW_URM/005":
                case "CRM_IU_UMC_ODATA/005":
                case "CRM_IU_UMC_ODATA/006":
                case "/IWBEP/CM_MGW_URM/007":
                case "CRM_IU_UMC_ODATA/007":
                case "/IWBEP/CM_MGW_URM/008":
                case "CRM_IU_UMC_ODATA/008":
                case "/IWBEP/CM_MGW_URM/009":
                case "CRM_IU_UMC_ODATA/009":
                case "ZMCFU/009":
                case "/IWBEP/CM_MGW_URM/010":
                case "CRM_IU_UMC_ODATA/010":
                case "/IWBEP/CM_MGW_URM/011":
                case "CRM_IU_UMC_ODATA/011":
                case "ZMCFU/011":
                case "CRM_IU_UMC_ODATA/043":
                case "/IWBEP/CM_MGW_URM/047":
                case "CRM_IU_UMC_ODATA/047":
                case "/IWBEP/CM_MGW_URM/021":
                case "CRM_IU_UMC_ODATA/021":
                case "/IWBEP/CM_MGW_URM/019":
                case "CRM_IU_UMC_ODATA/044":
                case "/IWBEP/CM_MGW_URM/026":
                case "CRM_IU_UMC_ODATA/026":
                case "CRM_IU_UMC_ODATA/065":
                case "CRM_IU_UMC_ODATA/020":
                case "CRM_IU_UMC_ODATA/031":
                case "/IWBEP/CM_MGW_URM/023":
                case "/IWBEP/CM_MGW_URM/020":
                    return ResidentialDomainError.ThereHasBeenAnError;
                case "ECRM_ISU/081":
                    return ResidentialDomainError.InvalidActiveContract;
                case "ZCRM_CHECK/000":
                    return ResidentialDomainError.ContractErrorPreventTheContactFromBeingSubmitted;
				case "ZCRM_BICO/000":
					return ResidentialDomainError.MeterSubmitOutOfTolerance;

				case "R1/218":
					return ResidentialDomainError.IbanInvalidBankForCountry;

				default:

                    return DomainError.Undefined;
            }
        }
    }
}

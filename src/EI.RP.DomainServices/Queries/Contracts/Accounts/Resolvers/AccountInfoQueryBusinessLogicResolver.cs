using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.ErrorHandling;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.CoreServices.System;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataModels.Sap.CrmUmc.Dtos.Extensions;
using EI.RP.DataServices;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.Banking;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Infrastructure.Mappers;
using EI.RP.DomainServices.ModelExtensions;
using EI.RP.DomainServices.Queries.Metering.Devices;
using EI.RP.DomainServices.Queries.Metering.Premises;
using NLog;

namespace EI.RP.DomainServices.Queries.Contracts.Accounts.Resolvers
{
	//TODO: REFACTOR
		internal class AccountInfoQueryBusinessLogicResolver
		{
			private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
			private readonly IDomainQueryResolver _queryResolver;
			private readonly ISapRepositoryOfCrmUmc _crmUmc;
			private readonly IDomainMapper _domainMapper;
			private readonly IResidentialPortalDataRepository _repository;
			private readonly IUserSessionProvider _userSession;

			public AccountInfoQueryBusinessLogicResolver(IDomainQueryResolver queryResolver,
				ISapRepositoryOfCrmUmc crmUmc, IDomainMapper domainMapper, IResidentialPortalDataRepository repository, IUserSessionProvider userSession)
			{
				_queryResolver = queryResolver;
				_crmUmc = crmUmc;
				_domainMapper = domainMapper;
				_repository = repository;
				_userSession = userSession;
			}

			public class ResolveSmartActivationStatusRequest
			{
				public string AccountNumber { get; }
				public ClientAccountType AccountType { get; }
				public bool AccountIsOpen { get; }
				public ContractItemDto Contract { get; }

				public bool IsPAYGCustomer { get; }

				public bool HasQuotationsInProgress { get; }

				public bool IsLossInProgress { get; }

				public PointReferenceNumber PointReferenceNumber { get; }

				public PaymentMethodType PaymentMethodType
				{
					get;
				}

				public DateTime? ContractStartDate { get; }
				public string BundleReference { get; }

				//TODO: remove at the end of the refactor
				public Func<ContractItemDto,bool, Task<IEnumerable<ContractItemDto>>> ResolveDuelFuelSisterAccountsFunc { get; }

				public ResolveSmartActivationStatusRequest(string accountNumber,
					ClientAccountType accountType, bool accountIsOpen, ContractItemDto contract, bool isPaygCustomer,
					bool hasQuotationsInProgress, bool isLossInProgress, PointReferenceNumber pointReferenceNumber,
					PaymentMethodType paymentMethodType, DateTime? contractStartDate, string bundleReference,Func<ContractItemDto, bool, Task<IEnumerable<ContractItemDto>>> resolveDuelFuelSisterAccountsFunc)
				{
					AccountNumber = accountNumber ?? throw new ArgumentNullException(nameof(accountNumber));
					AccountType = accountType;
					AccountIsOpen = accountIsOpen;
					Contract = contract ?? throw new ArgumentNullException(nameof(contract));
					IsPAYGCustomer = isPaygCustomer;
					HasQuotationsInProgress = hasQuotationsInProgress;
					IsLossInProgress = isLossInProgress;
					PointReferenceNumber = pointReferenceNumber ??
										   throw new ArgumentNullException(nameof(pointReferenceNumber));
					PaymentMethodType = paymentMethodType;
					ContractStartDate = contractStartDate;
					BundleReference = bundleReference;
					ResolveDuelFuelSisterAccountsFunc = resolveDuelFuelSisterAccountsFunc;
				}
			}

			public async Task<SmartActivationStatus> ResolveSmartActivationStatus(
				ResolveSmartActivationStatusRequest request)
			{
				if (!request.AccountIsOpen || !request.AccountType.IsElectricity())
				{
					return SmartActivationStatus.SmartNotAvailable;
				}
				return await ProcessRules();

				async Task<SmartActivationStatus> ProcessRules()
				{
					var smartActivationStatus = await ProcessDeviceBasedRules();
					if (smartActivationStatus == SmartActivationStatus.SmartAndEligible)
					{
						var tasks = new List<Task<SmartActivationStatus>>();

						tasks.Add(ProcessContractTermRules(request.Contract));
						tasks.Add(ProcessDuelFuelRules());
						tasks.Add(ProcessStaffDiscountRule());
						tasks.Add(ProcessPremiseIsNotConnected());
						tasks.Add(ProcessChangeOfSupplierLossInProgress());
						tasks.Add(ProcessBusinessPartnerHasQuotationsInProgress());
						tasks.Add(ProcessElectricityAccountIsPaygInProgress());
						tasks.Add(ProcessAllowAtLeast2DaysBeforeChangeElectricity());
						tasks.Add(ProcessDenyEqualiser());
						while (tasks.Any())
						{
							await Task.WhenAny(tasks);
							var completed = tasks.Where(x => x.IsCompleted).ToArray();
							foreach (var task in completed)
							{
								tasks.Remove(task);
							}

							var completedResults = await Task.WhenAll(completed);
							if (completedResults.Any(x => x != SmartActivationStatus.SmartAndEligible))
							{
								smartActivationStatus = SmartActivationStatus.SmartButNotEligible;
								tasks.Clear();
							}
						}
					}

					return smartActivationStatus;
				}

				async Task<SmartActivationStatus> ProcessContractTermRules(ContractItemDto contract)
				{
					string reason = null;
					var isFiniteContractTerm = contract.IsFiniteContractTerm();
					SmartActivationStatus result;
					if (!isFiniteContractTerm)
					{
						result = SmartActivationStatus.SmartAndEligible;
					}
					else
					{
						if (contract.ProductID.IsOneOf(
							ProductType.RE_ELECTRICITYPLAN_WIN,
							ProductType.RE_VR_MOVEIN,
							ProductType.RG_GASPLAN_WIN,
							ProductType.RG_GAS_MOVEIN))
						{
							reason = $"Product {contract.ProductID} is from exception";
							result = SmartActivationStatus.SmartAndEligible;
						}
						else
						{
							result = SmartActivationStatus.SmartButNotEligible;
							reason = "CONTTERM not eligible";
						}
					}

					LogRuleOutput($"{nameof(ProcessContractTermRules)}-ContractID:{contract.ContractID}", result, reason);
					return result;
				}

				async Task<SmartActivationStatus> ProcessDeviceBasedRules()
				{
					var devices =
						await _queryResolver.GetDevicesByAccountAndContract(request.AccountNumber, request.Contract.ContractID, true);
					var result = devices.Max(x => x.SmartActivationStatus);

					LogRuleOutput(nameof(ProcessDeviceBasedRules), result);
					return result;
				}
				async Task<SmartActivationStatus> ProcessDenyEqualiser()
				{
					var result = request.PaymentMethodType.IsEqualiser()
						? SmartActivationStatus.SmartButNotEligible
						: SmartActivationStatus.SmartAndEligible;

					LogRuleOutput(nameof(ProcessDenyEqualiser), result);
					return result;
				}

				async Task<SmartActivationStatus> ProcessAllowAtLeast2DaysBeforeChangeElectricity()
				{
					var result = request.ContractStartDate.HasValue && request.ContractStartDate.Value < DateTime.Today.AddDays(-2)
						? SmartActivationStatus.SmartAndEligible
						: SmartActivationStatus.SmartButNotEligible;
					LogRuleOutput(nameof(ProcessAllowAtLeast2DaysBeforeChangeElectricity), result);

					return result;
				}

				async Task<SmartActivationStatus> ProcessElectricityAccountIsPaygInProgress()
				{
					var result = request.IsPAYGCustomer
						? SmartActivationStatus.SmartButNotEligible
						: SmartActivationStatus.SmartAndEligible;
					LogRuleOutput(nameof(ProcessElectricityAccountIsPaygInProgress), result);
					return result;
				}


				async Task<SmartActivationStatus> ProcessBusinessPartnerHasQuotationsInProgress()
				{
					var result = request.HasQuotationsInProgress
						? SmartActivationStatus.SmartButNotEligible
						: SmartActivationStatus.SmartAndEligible;
					LogRuleOutput(nameof(ProcessBusinessPartnerHasQuotationsInProgress), result);
					return result;
				}

				async Task<SmartActivationStatus> ProcessChangeOfSupplierLossInProgress()
				{
					var result = SmartActivationStatus.SmartAndEligible;

					if (request.IsLossInProgress)
					{
						result = SmartActivationStatus.SmartButNotEligible;
					}
					LogRuleOutput(nameof(ProcessChangeOfSupplierLossInProgress), result);
					return result;
				}

				async Task<SmartActivationStatus> ProcessStaffDiscountRule()
				{
					var result = SmartActivationStatus.SmartAndEligible;
					var premise = await _queryResolver.GetPremiseByPrn(request.PointReferenceNumber, true);
					var staffDiscountApplied = premise?.Installations?.Any(i => i.HasFirstStaffDiscount) == true;
					if (staffDiscountApplied)
					{
						result = SmartActivationStatus.SmartButNotEligible;
					}
					LogRuleOutput(nameof(ProcessStaffDiscountRule), result);
					return result;
				}

				async Task<SmartActivationStatus> ProcessPremiseIsNotConnected()
				{
					var result = SmartActivationStatus.SmartAndEligible;
					var premise = await _queryResolver.GetPremiseByPrn(request.PointReferenceNumber, true);
					var disconnected =
						premise?.Installations?.Any(i => i.DiscStatus != InstallationDiscStatusType.New) == true;
					if (disconnected)
					{
						result = SmartActivationStatus.SmartButNotEligible;
					}
					LogRuleOutput(nameof(ProcessPremiseIsNotConnected), result);
					return result;
				}

				async Task<SmartActivationStatus> ProcessDuelFuelRules()
				{
					var result = SmartActivationStatus.SmartAndEligible;

					//TODO: this IS TEMPORAL WHILE REFACTORING IS ONGOING
					var duelFuelAccounts = await request.ResolveDuelFuelSisterAccountsFunc(request.Contract, true);
					var gasContract = duelFuelAccounts.SingleOrDefault(x => x.DivisionID == DivisionType.Gas);
					string reason = null;
					if (gasContract != null)
					{
						var processContractTermRulesTask = ProcessContractTermRules(gasContract);

						result = gasContract.ContractStartDate.HasValue &&
								 gasContract.ContractStartDate.Value < DateTime.Today.AddDays(-2)
							? SmartActivationStatus.SmartAndEligible
							: SmartActivationStatus.SmartButNotEligible;

						if (result == SmartActivationStatus.SmartAndEligible
							&& ((ProductType)gasContract.ProductID).IsPAYGPRoduct()
						)
						{
							reason = "duel fuel gas account is PAYG";
							result = SmartActivationStatus.SmartButNotEligible;
						}

						if (result == SmartActivationStatus.SmartAndEligible)
						{
							result = await processContractTermRulesTask;
						}
					}

					LogRuleOutput(nameof(ProcessDuelFuelRules), result, reason);
					return result;
				}

				void LogRuleOutput(string ruleName, SmartActivationStatus result, string reason = null)
				{

					Logger.Debug(() =>
					{
						if (reason != null) reason = $"Reason: {reason}";
						return
								$"{nameof(ResolveSmartActivationStatus)} - Rule({ruleName}) User({_userSession.UserName}):{result} {reason}";
					});
				}
			}


			public async Task<bool> HasStaffDiscountApplied(PointReferenceNumber accountPointReferenceNumber,
				ClientAccountType accountType)
			{
				var result = false;
				if (accountType.IsElectricity())
				{
					var premise = await _queryResolver.GetPremiseByPrn(accountPointReferenceNumber, true);
					result = premise?.Installations?.Any(i => i.HasFirstStaffDiscount) == true;
				}

				return result;
			}

			public async Task<bool> HasQuotationsInProgress(string accountInfoPartner)
			{
				var quotations = await _crmUmc
					.NewQuery<AccountDto>()
					.Key(accountInfoPartner)
					.NavigateTo<QuotationItemDto>()
					.GetMany();

				return quotations.Any(x => (QuotationStatusType)x.DocumentStatusID != QuotationStatusType.Finished);
			}

			

			public async Task<BankAccountInfo[]> ResolveBankAccounts(ContractItemDto contractItemDto)
			{
				var bankAccountInfos = await Task.WhenAll(contractItemDto.BusinessAgreement.Account.BankAccounts
					.Select(x => _domainMapper.Map<BankAccountDto, BankAccountInfo>(x)));
				return bankAccountInfos.Select(x =>
				{
					x.AccountNumber = contractItemDto.BusinessAgreementID;
					return x;
				}).ToArray();
			}

			public DateTimeRange[] ResolveSmartPeriods(ContractItemDto contractItemDto)
			{
				var consents = contractItemDto.ContractItemTimeSlices
					.SelectMany(x => x.SmartConsents)
					.Where(x => x.Cancelled == SapBooleanFlag.No)
					.Select(x => new DateTimeRange(x.PermissionStartDate, x.PermissionEndDate == x.PermissionStartDate ? x.PermissionEndDate.AddDays(1).AddMilliseconds(-1) : x.PermissionEndDate))
					.MergeConsecutivePeriods().ToArray();
				return consents;
			}

			public DateTimeRange[] ResolveNonSmartPeriods(ContractItemDto contractItemDto)
			{
				var result = contractItemDto.ContractItemTimeSlices
					.Where(x => !x.SmartConsents.Any())
					.Select(x => new DateTimeRange(x.StartDate, (x.EndDate == x.StartDate ? x.EndDate?.AddDays(1).AddMilliseconds(-1) : x.EndDate) ?? SapDateTimes.SapDateTimeMax))
					.MergeConsecutivePeriods().ToArray();

				return result;
			}

			public async Task<bool> SwitchToSmartPlanWasDismissed(string accountNumber)
			{
				var entry = _repository.GetSmartActivationNotificationInfo(_userSession.UserName, accountNumber);
				return entry?.IsNotificationDismissed ?? false;
			}

			public async Task<bool> CanSubmitMeterReading(string accountNumber, ContractItemDto contractItemDto)
			{
				var devices = await _queryResolver.GetDevicesByAccountAndContract(accountNumber, contractId: contractItemDto.ContractID, byPassPipeline: true);

				return !devices.Any(x => x.SmartActivationStatus == SmartActivationStatus.SmartActive ||
										 x.SmartActivationStatus == SmartActivationStatus.SmartAndEligible ||
										 (x.CTF != null &&
										  x.CTF.IsOneOf(CommsTechnicallyFeasibleValue.CTF2,
											  CommsTechnicallyFeasibleValue.CTF3,
											  CommsTechnicallyFeasibleValue.CTF4) &&
										  x.MCCConfiguration != null &&
										  x.MCCConfiguration.IsOneOf(RegisterConfigType.MCC01,
											  RegisterConfigType.MCC16))
										 );
			}
		}
	}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Cqrs.Commands;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Profiling;
using EI.RP.CoreServices.System;
using EI.RP.DataServices.SAP.Clients.Config;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Users.Session.CreateUserSession;
using EI.RP.DomainServices.Queries.Billing.Activity;
using EI.RP.DomainServices.Queries.Billing.EqualiserPaymentSetupInfo;
using EI.RP.DomainServices.Queries.Billing.Info;
using EI.RP.DomainServices.Queries.Billing.InvoiceFiles;
using EI.RP.DomainServices.Queries.Billing.LatestBill;
using EI.RP.DomainServices.Queries.Competitions;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using EI.RP.DomainServices.Queries.Contracts.BusinessPartners;
using EI.RP.DomainServices.Queries.Metering.Consumption;
using EI.RP.DomainServices.Queries.Metering.Devices;
using EI.RP.DomainServices.Queries.Metering.MeterReadings;
using EI.RP.DomainServices.Queries.Metering.Premises;
using EI.RP.DomainServices.Queries.MovingHouse.CheckMoveOutRequestResults;
using NLog;

namespace EI.RP.WebApp.Infrastructure.Caching.PreLoad.Processors
{
	internal class CacheAccountsPreloaderTask : ICachePreloadTask
	{
		private static readonly ILogger Logger = LogManager.GetLogger("CacheLogger");
		private readonly IDomainCommandDispatcher _commandDispatcher;
		private readonly IProfiler _profiler;
		private readonly IDomainQueryResolver _queryResolver;
		private readonly IUserSessionProvider _sessionProvider;
		private readonly ICachePreloaderSettings _settings;
		private readonly ISapSettings _sapSettings;

		public CacheAccountsPreloaderTask(IDomainCommandDispatcher commandDispatcher, IProfiler profiler,
			IDomainQueryResolver queryResolver, IUserSessionProvider sessionProvider,ICachePreloaderSettings settings,ISapSettings sapSettings)
		{
			_commandDispatcher = commandDispatcher;
			_profiler = profiler;
			_queryResolver = queryResolver;
			_sessionProvider = sessionProvider;
			_settings = settings;
			_sapSettings = sapSettings;
			_sessionProvider.DisableContext();
		}

		public int PriorityOrder { get; } = 1;

		public async Task ProcessRequest(CacheAccountPreLoaderQueue.CacheAccountPreLoadRequest message)
		{
			Logger.Debug(() => $"PreloadingCache for {message.ForUserName}");
			using (_profiler.RecordStep("Cache Preload"))
			{
				try
				{
					var accounts = (await GetAccountNumbers()).ToArray();

					await GetUserAccountsItems(message, accounts);
					Logger.Debug(() => $"Done PreloadingCache for {message.ForUserName}");
				}
				catch (Exception ex)
				{
					Logger.Warn(() => ex.ToString());
				}
			}

			async Task<IEnumerable<string>> GetAccountNumbers()
			{
				var isAnonymous = _sessionProvider.IsAnonymous();
				if (isAnonymous)
				{
					var cacheServicePasswordAsync = _settings.CacheServicePasswordAsync();
					var cacheServiceUserNameAsync = _settings.CacheServiceUserNameAsync();
					await _commandDispatcher.ExecuteAsync(new CreateUserSessionCommand(await cacheServiceUserNameAsync, await cacheServicePasswordAsync,
						isServiceAccount: true));
				}


				var accountsByUserName = await _queryResolver.GetAccountsByUserName<AccountInfo>(message.ForUserName, true);
				_sessionProvider.ActingAsUserName = message.ForUserName;
				return accountsByUserName.Select(x => x.AccountNumber);
			}
		}

		private async Task GetUserAccountsItems(CacheAccountPreLoaderQueue.CacheAccountPreLoadRequest message,
			IEnumerable<string> accountNumbers)
		{
			var tasks = new ConcurrentBag<Task>();
			tasks.Add(_queryResolver.GetCompetitionEntriesByUserName(message.ForUserName, true));
			tasks.Add(_queryResolver.GetAccounts(byPassPipeline: true));
			await Task.Delay((int) _sapSettings.BatchEnlistTimeoutMilliseconds);
			foreach (var accountNumber in accountNumbers)
			{
				//thread per account
				tasks.Add(GetSingleAccountItems(accountNumber));
			}

			await Task.WhenAll(tasks);

			async Task GetSingleAccountItems(string accountNumber)
			{


				var part1 = Task.Run(async () => await GetAccountDependents());
				await Task.Delay((int) _sapSettings.BatchEnlistTimeoutMilliseconds);
				var part2_1 = Task.Run(async () =>
				{
					await Task.WhenAll(
						_queryResolver.GetAccountBillingInfoByAccountNumber(accountNumber, true),
						_queryResolver.GetAccountBillingsByAccountNumber(accountNumber, true)
						);
				});
				await Task.Delay((int) _sapSettings.BatchEnlistTimeoutMilliseconds);
				var part2_2 = Task.Run(async () =>
				{
					await Task.WhenAll(_queryResolver.GetDuelFuelAccountsByAccountNumber(accountNumber, true),
						_queryResolver.GetMeterReadings(accountNumber, true));
				});
				await Task.Delay((int) _sapSettings.BatchEnlistTimeoutMilliseconds);
				var part3 = Task.Run(async () =>
					await Task.WhenAll(_queryResolver.GetLatestBillByAccountNumber(accountNumber, true),
						_queryResolver.GetInvoicesByAccountNumber(accountNumber, byPassPipeline: true)));
				await Task.Delay((int) _sapSettings.BatchEnlistTimeoutMilliseconds);
				var part4 = Task.Run(async () =>
					await Task.WhenAll(
						_queryResolver.GetDevicesByAccount(accountNumber, byPassPipeline: true),
						_queryResolver.GetEqualiserSetUpInfo(accountNumber, byPassPipeline: true))
				);
				await Task.WhenAll(part1, part2_1,part2_2, part3, part4);

				async Task GetAccountDependents()
				{
					var accountInfo = await _queryResolver.GetAccountInfoByAccountNumber(accountNumber, true);
					Task consumptionTask;
					if (!accountInfo.IsSmart())
					{
						consumptionTask = _queryResolver.GetAccountConsumption(accountNumber,
							TimePeriodAggregationType.BiMonthly,
							DateTimeRange.CurrentYear, ConsumptionDataRetrievalType.NonSmart, true, true);
					}
					else
					{
						consumptionTask = _queryResolver.GetAccountConsumption(accountNumber,
							TimePeriodAggregationType.Daily,
							DateTimeRange.CurrentMonth, ConsumptionDataRetrievalType.Smart, true, true);
					}

					await Task.WhenAll(
						consumptionTask,
						_queryResolver.GetPremiseByPrn(accountInfo.PointReferenceNumber, byPassPipeline: true),
						_queryResolver.CheckMoveOut(accountInfo.ContractId, true)
					);
				}
			}
		}
	}
}
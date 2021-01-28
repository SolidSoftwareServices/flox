using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;
using EI.RP.DataServices.SAP.Clients.IntegrationTests.Infrastructure;
using NUnit.Framework;

namespace EI.RP.DataServices.SAP.Clients.IntegrationTests.Batching
{
	
	
	[TestFixture]
	public class BatchTests : SapTest
	{

		private async Task DoLogin()
		{
			//make sure it has only one account
			await LoginUser("MR862_Elec-1Meter@preprod.esb.ie", "Test3333");
		}
		[TestCase(1)]
		[TestCase(2)]
		public async Task ImplicitBatches(int times)
		{
			await DoLogin();

			for (int i = 0; i < times; i++)
			{
				await DoPayload();
			}

			async Task DoPayload()
			{
				var tasks = new List<Task<IEnumerable<AccountDto>>>();
				for (int i = 0; i < 50; i++)
				{
					tasks.Add(CrmUmc.NewQuery<AccountDto>().GetMany());
				}

				var actual = (await Task.WhenAll(tasks.ToArray())).SelectMany(x => x).ToArray();
				var accountId = actual[0].AccountID;
				Assert.IsFalse(string.IsNullOrWhiteSpace(accountId));
				Assert.IsTrue(actual.All(x => x != null && x.AccountID == accountId));
				Assert.IsTrue(actual.Length == 50);
				Console.WriteLine(DateTime.Now);
			}
		}
		[TestCase(2)]
		[TestCase(10)]
		[TestCase(50)]
		[Test]
		public async Task MultiThreadImplicitBatches(int batchSize)
		{
			await DoLogin();

			var tasks = new ConcurrentBag<Task<IEnumerable<AccountDto>>>();

			for (int i = 0; i < batchSize; i++)
			{

				tasks.Add(Task.Run(async () => await CrmUmc.NewQuery<AccountDto>().GetMany()));
			}

			var actual = (await Task.WhenAll(tasks.ToArray())).SelectMany(x => x).ToArray();
			var accountId = actual[0].AccountID;
			Assert.IsFalse(string.IsNullOrWhiteSpace(accountId));
			Assert.IsTrue(actual.All(x => x != null && x.AccountID == accountId));
			Assert.IsTrue(actual.Length == batchSize);
			Console.WriteLine(DateTime.Now);
		}

		[TestCase(1, 50)]
		[TestCase(2, 10)]
		[TestCase(10, 3)]
		public async Task ExplicitBatches(int asynchronousTaskCount, int batchSize)
		{
			await DoLogin();

			for (int i = 0; i < asynchronousTaskCount; i++)
			{
				await RunAsynchronousTask();
			}

			Console.WriteLine(DateTime.Now);

			async Task RunAsynchronousTask()
			{

				var tasks = new List<Task<IEnumerable<AccountDto>>>();

				using (var aggregator = CrmUmc.StartNewBatchAggregator())
				{
					for (int i = 0; i < batchSize; i++)
					{
						tasks.Add(CrmUmc.NewQuery<AccountDto>().GetMany());
					}

					await Task.Yield();

					await aggregator.CompleteBatch();
				}

				var actual = (await Task.WhenAll(tasks.ToArray())).SelectMany(x => x).ToArray();
				var accountId = actual[0].AccountID;
				Assert.IsFalse(string.IsNullOrWhiteSpace(accountId));
				Assert.IsTrue(actual.All(x => x != null && x.AccountID == accountId));
				Assert.IsTrue(actual.Length == batchSize);
			}
		}
		[TestCase(2, 20)]
		[TestCase(10, 3)]
		[TestCase(50, 2)]
		public async Task MultiThreadExplicitBatches(int asynchronousTaskCount, int batchSize)
		{
			await DoLogin();
			var threads = new List<Task>();
			for (int i = 0; i < asynchronousTaskCount; i++)
			{
				threads.Add(Task.Run(async () => await RunAsynchronousTask()));
			}

			await Task.WhenAll(threads.ToArray());

			Console.WriteLine(DateTime.Now);

			async Task RunAsynchronousTask()
			{

				var tasks = new List<Task<IEnumerable<AccountDto>>>();

				using (var aggregator = CrmUmc.StartNewBatchAggregator())
				{
					for (int i = 0; i < batchSize; i++)
					{
						tasks.Add(CrmUmc.NewQuery<AccountDto>().GetMany());
					}

					await Task.Yield();

					await aggregator.CompleteBatch();
				}

				var actual = (await Task.WhenAll(tasks.ToArray())).SelectMany(x => x).ToArray();
				var accountId = actual[0].AccountID;
				Assert.IsFalse(string.IsNullOrWhiteSpace(accountId));
				Assert.IsTrue(actual.All(x => x != null && x.AccountID == accountId));
				Assert.IsTrue(actual.Length == batchSize);
			}
		}
	}
}

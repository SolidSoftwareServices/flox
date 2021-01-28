using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Caching.InMemory;
using EI.RP.TestServices;
using NUnit.Framework;

namespace EI.RP.CoreServices.Caching.IntegrationTests.InMemory
{
	//[Parallelizable(ParallelScope.None)]
	[TestFixture]
	internal partial class
		InMemoryCacheProviderTests : UnitTestFixture<InMemoryCacheProviderTests.TestContext, InMemoryCacheProvider>
	{
		[Test]
		public async Task CanAddItems()
		{
			var sut = Context.Sut;

			for (var i = 0; i < 10000; i++)
			{
				var result = await sut.GetOrAddAsync(i, async () => await Task.FromResult(i.ToString()));
				Assert.AreEqual(i.ToString(), result);
			}
		}
		[Test]
		public async Task CanAddItemsConcurrently()
		{
			Context.Expiration=TimeSpan.FromSeconds(60);
			var tasks = new List<Task>();
			var mre = new ManualResetEvent(false);

			var sut = Context.Sut;
			
			var numItems = 100;
			for (var i = 0; i < numItems; i++)
			{
				var i1 = i;
				for (var tId = 0; tId < 4; tId++)
					tasks.Add(Task.Factory.StartNew(async () =>
						{
							mre.WaitOne();
							var result1 = sut.GetOrAddAsync(i1, async () => await Task.FromResult(i1.ToString()));
							var result2 = sut.GetOrAddAsync(i1, async () => await Task.FromResult(i1.ToString()));
							var result3 = sut.GetOrAddAsync(i1, async () => await Task.FromResult(i1.ToString()));
							Assert.IsNotNull(await result1);
							Assert.IsNotNull(await result2);
							Assert.IsNotNull(await result3);
						}
					));
			}
			var signal = Context.SignalWhenNumItemsSent(numItems);
			mre.Set();
			await Task.WhenAll(tasks);
			if (!signal.Wait(TimeSpan.FromSeconds(30)))
			{
				Assert.Fail("items not sent");
			}
			for (var i = 0; i < numItems; i++)
			{
				Assert.IsTrue(await sut.ContainsKeyAsync<int,string>(i),$"it does not contain {i}");
			}
		}

		[Test]
		public async Task CanAddItemsConcurrentlyWithHighExpirationIndefinitely()
		{
			for (var i = 0; i < 3; i++)
			{
				Console.WriteLine($"iteration #:{i+1}");
				await CanAddItemsConcurrently();
				await Context.Sut.ClearAsync();
			}
		}

		[Test]
		public async Task CanInvalidateItems()
		{
			var maxDurationFromNow = TimeSpan.FromSeconds(20);
			await Context.Sut.GetOrAddAsync(1, async () => await Task.FromResult(1.ToString()),maxDurationFromNow:maxDurationFromNow);
			await Context.Sut.InvalidateAsync(keys: 1);
			Assert.IsFalse(await Context.Sut.ContainsKeyAsync<int,string>(1));

		}

		[Test]
		public async Task CanExpireItems()
		{
			var maxDurationFromNow = TimeSpan.FromSeconds(2);
			await Context.Sut.GetOrAddAsync(1, async () => await Task.FromResult(1.ToString()),maxDurationFromNow:maxDurationFromNow);
			await Context.Sut.GetOrAddAsync(2, async () => await Task.FromResult(2.ToString()),maxDurationFromNow:maxDurationFromNow);
			await Task.Delay(maxDurationFromNow.Add(TimeSpan.FromSeconds(1)));
			Assert.IsFalse(await Context.Sut.ContainsKeyAsync<int,string>(1));
			Assert.IsFalse(await Context.Sut.ContainsKeyAsync<int,string>(2));
		}
	}
}
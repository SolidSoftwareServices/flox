using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using S3.CoreServices.Interop;
using S3.TestServices;
using NUnit.Framework;
using AutoFixture;
using S3.CoreServices.System.DesignPatterns.Observer;

namespace S3.CoreServices.UnitTests.Interop
{
	class TestMessage : IObservableMessage, IEquatable<TestMessage>
	{
		public string Value { get; set; }

		public Guid ReceptionBatchId { get; set; }

		public bool Equals(TestMessage other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Value == other.Value;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((TestMessage) obj);
		}

		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}

		public static bool operator ==(TestMessage left, TestMessage right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(TestMessage left, TestMessage right)
		{
			return !Equals(left, right);
		}
	}
	class AsyncPublisherSubscriberUnitTest : UnitTestFixture<AsyncPublisherSubscriberUnitTest.TestContext,
		AsyncPublisherSubscriber<TestMessage>>
	{

		[Test]
		public async Task CanSubscribeAndUnscribe()
		{
			var unscriber = Context.Sut.Subscribe(Context.Subscriber1);

			unscriber.Dispose();
		}

		[Test]
		public async Task WhenItemPushed_AndNoSubscriber_ItRemovesTheMessage()
		{

			Context.Sut.Publish(Context.Fixture.Create<TestMessage>());
			await Task.Delay(TimeSpan.FromSeconds(2));
			Assert.AreEqual(0, Context.Sut.PendingMessages);
		}

		[TestCase(1)]
		[TestCase(2)]
		[TestCase(1000)]
		[Test]
		public async Task WhenItemPushed_ItNotifiesSubscriber(int numMessages)
		{
			Context.Subscriber1.SignalWhenMessageCountIs(numMessages);
			using (Context.Sut.Subscribe(Context.Subscriber1))
			{
				var messages = Context.Fixture.CreateMany<TestMessage>(numMessages).ToArray();
				Context.Sut.Publish(messages);
				if (!Context.Subscriber1.Signal.WaitOne(TimeSpan.FromSeconds(10)))
					Assert.Fail("Subscriber did not signal");
				Context.Subscriber1.Signal.Reset();


				Assert.AreEqual(numMessages, Context.Subscriber1.ReceivedMessages.Count());
				CollectionAssert.AreEquivalent(messages, Context.Subscriber1.ReceivedMessages);

				Assert.AreEqual(0, Context.Sut.PendingMessages);
			}
		}

		[Test]
		public async Task SubscriberItDoesNotReceiveMessagesAfterUnsubscription()
		{
			Context.Subscriber1.SignalWhenMessageCountIs(1);
			using (Context.Sut.Subscribe(Context.Subscriber1))
			{ }

			var message = Context.Fixture.Create<TestMessage>();
			Context.Sut.Publish(message);
			if (Context.Subscriber1.Signal.WaitOne(TimeSpan.FromSeconds(3)))
				Assert.Fail("Subscriber did receiuve message ");
			Context.Subscriber1.Signal.Reset();

			Assert.AreEqual(0, Context.Sut.PendingMessages);
			Assert.AreEqual(0, Context.Subscriber1.ReceivedMessages.Count());

		}


		[TestCase(1)]
		[TestCase(2)]
		[TestCase(1000)]
		public async Task WhenItemPushed_ItNotifiesAllSubscribers_Of_All_PendingMessages(int numMessages)
		{
			Context.Subscriber1.SignalWhenMessageCountIs(numMessages);
			Context.Subscriber2.SignalWhenMessageCountIs(numMessages);
			using (Context.Sut.Subscribe(Context.Subscriber1))
			using (Context.Sut.Subscribe(Context.Subscriber2))
			{

				var messages = Context.Fixture.CreateMany<TestMessage>(numMessages);
				foreach (var message in messages)
				{
					Context.Sut.Publish(message);
				}

				AssertSubscriber(Context.Subscriber1, messages);
				AssertSubscriber(Context.Subscriber2, messages);
			}

			Assert.AreEqual(0, Context.Sut.PendingMessages);

			void AssertSubscriber(Subscriber subscriber, IEnumerable<TestMessage> messages)
			{
				if (!subscriber.Signal.WaitOne(TimeSpan.FromSeconds(3)))
					Assert.Fail("Subscriber did not signal");
				subscriber.Signal.Reset();


				Assert.AreEqual(numMessages, subscriber.ReceivedMessages.Count());
				CollectionAssert.AreEquivalent(messages, subscriber.ReceivedMessages);
			}
		}

		public class TestContext : UnitTestContext<AsyncPublisherSubscriber<TestMessage>>
		{
			public Subscriber Subscriber1 { get; } = new Subscriber();
			public Subscriber Subscriber2 { get; } = new Subscriber();

			protected override void Dispose(bool disposing)
			{
				Subscriber1.Dispose();
				Subscriber2.Dispose();
				base.Dispose(disposing);
			}
		}

		public class Subscriber : IAsyncObserver<TestMessage>, IDisposable
		{
			public readonly ManualResetEvent Signal = new ManualResetEvent(false);
			private readonly ConcurrentBag<TestMessage> _receivedMessages = new ConcurrentBag<TestMessage>();
			private int _signalWhenCount;


			public Task OnCompletedAsync()
			{
				throw new NotImplementedException();
			}

			public Task OnErrorAsync(Exception error)
			{
				throw new NotImplementedException();
			}

			public async Task OnNextAsync(TestMessage value)
			{
				_receivedMessages.Add(value);
				if (_signalWhenCount == _receivedMessages.Count)
				{
					Signal.Set();
				}
			}

			public IEnumerable<TestMessage> ReceivedMessages => _receivedMessages.ToArray();

			public void Dispose()
			{
				Signal?.Dispose();
			}

			public void SignalWhenMessageCountIs(int count)
			{
				_signalWhenCount = count;
			}
		}
	}
}
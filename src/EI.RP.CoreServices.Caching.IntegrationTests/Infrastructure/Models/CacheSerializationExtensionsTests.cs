using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using AutoFixture;
using AutoFixture.AutoMoq;
using EI.RP.CoreServices.Caching.Models;
using EI.RP.CoreServices.Encryption;
using EI.RP.CoreServices.Infrastructure;
using EI.RP.CoreServices.System;
using EI.RP.Stubs.IoC;
using Moq;
using NUnit.Framework;

namespace EI.RP.CoreServices.Caching.IntegrationTests.Infrastructure.Models
{
	[Parallelizable(ParallelScope.All)]
	class CacheSerializationExtensionsTests
	{
		private class TestModule : Module
		{
			protected override void Load(ContainerBuilder builder)
			{
				var settings = new Mock<IEncryptionSettings>();
				settings.Setup(x => x.EncryptionInitVectorAsync()).ReturnsAsync("initVector123456");
				settings.Setup(x => x.EncryptionPassPhraseAsync()).ReturnsAsync("#p@$$phr@$e#");
				settings.Setup(x => x.EncryptionSaltValueAsync()).ReturnsAsync("$$@1tv@1ue$");
				builder.RegisterInstance(settings.Object).As<IEncryptionSettings>();
				builder.RegisterModule(CoreModule.Configure().WithEncryptionFeature(true));
			}
		}
		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			_container = IoCContainerBuilder.From<TestModule>();
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			_container.Dispose();
		}
		private readonly IFixture Fixture= new Fixture().Customize(new AutoMoqCustomization());
		private IContainer _container;

		[Test,Theory]
		public async Task CanSerializeAndDeserialize(bool encrypt)
		{
			var model = Fixture.Create<DummyModel>();
			var expected=new CachedItem
			{
				GeneratedByInstanceId = Guid.NewGuid(),
				Item = model
			};
			var encryptor = encrypt ? _container.Resolve<IEncryptionService>() : null;
			var cacheSerialize = await expected.CacheSerializeAsync(encryptor);
			var actual=await cacheSerialize.CacheDeserializeAsync(encryptor);
			Assert.AreEqual(expected,actual);

		}
		[Test,Theory]
		public async Task CanSerializeAndDeserializeString(bool encrypt)
		{
			var model = Fixture.Create<string>();
			var expected=new CachedItem
			{
				GeneratedByInstanceId = Guid.NewGuid(),
				Item = model
			};

			var encryptor = encrypt ? _container.Resolve<IEncryptionService>() : null;
			var cacheSerialize = await expected.CacheSerializeAsync(encryptor);
			var actual=await cacheSerialize.CacheDeserializeAsync(encryptor);
			Assert.AreEqual(expected,actual);
		}
		[Test,Theory]
		public async Task CanSerializeAndDeserializeArray(bool encrypt)
		{
			var model = Fixture.CreateMany<DummyModel>().ToArray();
			var expected=new CachedItem
			{
				GeneratedByInstanceId = Guid.NewGuid(),
				Item = model
			};

			var encryptor = encrypt ? _container.Resolve<IEncryptionService>() : null;
			var cacheSerialize = await expected.CacheSerializeAsync(encryptor);
			var actual=await cacheSerialize.CacheDeserializeAsync(encryptor);
			Assert.AreEqual(expected.CreatedTimeUtc,actual.CreatedTimeUtc);
			Assert.AreEqual(expected.GeneratedByInstanceId,actual.GeneratedByInstanceId);
			CollectionAssert.AreEqual(model,actual.ItemAs<IEnumerable<DummyModel>>());
		}

		[Test,Theory]
		public async Task CanSerializeAndDeserializeEnumeration(bool encrypt)
		{
			var model = Fixture.CreateMany<DummyModel>().ToList();
			var expected=new CachedItem
			{
				GeneratedByInstanceId = Guid.NewGuid(),
				Item = model
			};

			var encryptor = encrypt ? _container.Resolve<IEncryptionService>() : null;
			var cacheSerialize = await expected.CacheSerializeAsync(encryptor);
			var actual=await cacheSerialize.CacheDeserializeAsync(encryptor);
			Assert.AreEqual(expected.CreatedTimeUtc,actual.CreatedTimeUtc);
			Assert.AreEqual(expected.GeneratedByInstanceId,actual.GeneratedByInstanceId);
			CollectionAssert.AreEqual(model,actual.ItemAs<IEnumerable<DummyModel>>());
		}
		[Test,Theory]
		public async Task CanSerializeAndDeserializeNullValue(bool encrypt)
		{
			var expected=new CachedItem
			{
				GeneratedByInstanceId = Guid.NewGuid(),
				Item = null
			};

			var encryptor = encrypt ? _container.Resolve<IEncryptionService>() : null;
			var cacheSerialize = await expected.CacheSerializeAsync(encryptor);
			var actual=await cacheSerialize.CacheDeserializeAsync(encryptor);
			Assert.AreEqual(expected,actual);

		}

		private class DummyModel : IEquatable<DummyModel>
		{
			public string Property { get; set; }

			public bool Equals(DummyModel other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(this, other)) return true;
				return Property == other.Property;
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != this.GetType()) return false;
				return Equals((DummyModel) obj);
			}

			public override int GetHashCode()
			{
				return (Property != null ? Property.GetHashCode() : 0);
			}

			public static bool operator ==(DummyModel left, DummyModel right)
			{
				return Equals(left, right);
			}

			public static bool operator !=(DummyModel left, DummyModel right)
			{
				return !Equals(left, right);
			}
		}
	}
}

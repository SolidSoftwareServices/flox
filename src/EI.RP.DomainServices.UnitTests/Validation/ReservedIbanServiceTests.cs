using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainServices.Validation;
using EI.RP.TestServices;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace EI.RP.DomainServices.UnitTests.Validation
{
	internal class ReservedIbanServiceTests : UnitTestFixture<ReservedIbanServiceTests.TestContext, ReservedIbanService>
	{
		public class TestContext : UnitTestContext<ReservedIbanService>
		{
			public TestContext() : base(new Fixture().CustomizeDomainTypeBuilders())
			{
			}

			private string _reservedIban = string.Empty;
			public TestContext WithReservedIban(string reservedIban)
			{
				_reservedIban += reservedIban + ",";
				return this;
			}

			protected override ReservedIbanService BuildSut(AutoMocker autoMocker)
			{
				var mockReservedIbanSettings = new Mock<IReservedIbanSettings>();
				mockReservedIbanSettings.Setup(x => x.ReservedIban).Returns(_reservedIban.Split(","));
				autoMocker.Use<IReservedIbanSettings>(mockReservedIbanSettings);

				return base.BuildSut(autoMocker);
			}
		}

		[Test]
		public void ItReturnsTrueWhenIbanIsReserved()
		{
			var reservedIban = "ReservedIban";
			Context.WithReservedIban(reservedIban);
			var actual = Context.Sut.IsReservedIban(reservedIban);

			Assert.IsTrue(actual);
		}

		[Test]
		public void ItReturnsFalseWhenIbanIsNotReserved()
		{
			var reservedIban = "ReservedIban";
			Context.WithReservedIban(reservedIban);
			var actual = Context.Sut.IsReservedIban("anyOtherIban");

			Assert.IsFalse(actual);
		}
	}
}
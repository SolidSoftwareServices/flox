using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using EI.RP.CoreServices.Platform;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Infrastructure.Mappers;
using EI.RP.DomainServices.Infrastructure.Settings;
using EI.RP.TestServices;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Infrastructure.Mappers
{
	[TestFixture]
	internal class DomainMapperTests : UnitTestFixture<DomainMapperTests.TestContext, DomainMapper>
	{
		internal class TestContext : UnitTestContext<DomainMapper>
		{
			private RegisterConfigType _mccConfig;
			private CommsTechnicallyFeasibleValue _ctf;
			private DivisionType _divisionType;
			private bool _isSmartActivationEnabled;

			public TestContext WithIsSmartActivationEnabled(bool isSmartActivationEnabled)
			{
				_isSmartActivationEnabled = isSmartActivationEnabled;
				return this;
			}

			public TestContext WithMccConfig(RegisterConfigType mccConfig)
			{
				_mccConfig = mccConfig;
				return this;
			}

			public TestContext WithCTF(CommsTechnicallyFeasibleValue ctf)
			{
				_ctf = ctf;
				return this;
			}

			public TestContext WithDivisionType(DivisionType divisionType)
			{
				_divisionType = divisionType;
				return this;
			}


			public DeviceDto BuildDeviceDto()
			{
				return new DeviceDto
				{
					ContractID = Fixture.Create<string>(),
					DeviceID = Fixture.Create<string>(),
					DeviceMaterial = Fixture.Create<string>(),
					DeviceLocation = Fixture.Create<string>(),
					SerialNumber = Fixture.Create<string>(),
					DeviceDescription = Fixture.Create<string>(),
					DivisionID = _divisionType,
					FunctionClass = "1006"
				};
			}

			public InstallationDto BuildInstallationDto()
			{
				var ctf = Fixture.Build<InstallationFactDto>()
				.With(i => i.Operand, OperandType.CTFValue)
					.With(i => i.ValidTo, SapDateTimes.SapDateTimeMax)
					.With(i => i.OperandValue, _ctf)
					.Create();
				var mcc = Fixture.Build<InstallationFactDto>()
					.With(i => i.Operand, OperandType.SmartMeterConfiguration)
					.With(i => i.ValidTo, SapDateTimes.SapDateTimeMax)
					.With(i => i.OperandValue, _mccConfig)
					.Create();
				return new InstallationDto
				{
					PremiseID = Fixture.Create<string>(),
					InstallationFacts = new List<InstallationFactDto> { ctf, mcc }
				};
			}

			protected override DomainMapper BuildSut(AutoMocker autoMocker)
			{
				var platformSettings = new Mock<IDomainSettings>();
				platformSettings.Setup(_ => _.IsSmartActivationEnabled).Returns(_isSmartActivationEnabled);
				autoMocker.Use(platformSettings);
				return base.BuildSut(autoMocker);
			}
		}

		public static IEnumerable<TestCaseData> ItCanMapDevicesStatusCases()
		{
			var names = new HashSet<string>();
			foreach (var isSmartActivationEnabled in new[] { true, false })
			{
				foreach (var ctf in CommsTechnicallyFeasibleValue.AllValues.Cast<CommsTechnicallyFeasibleValue>())
				{
					foreach (var mcc in RegisterConfigType.AllValues.Cast<RegisterConfigType>())
					{
						foreach (var divisionType in DivisionType.AllValues.Cast<DivisionType>())
						{
							var name = $"SmartActivationEnabled: {isSmartActivationEnabled}, CTF:{ctf ?? "(null)"}, MMC:{mcc}, divisionType:{divisionType.DebuggerDisplayValue}]";
							if (!names.Contains(name))
							{
								names.Add(name);
								yield return new TestCaseData(isSmartActivationEnabled, divisionType, mcc, ctf)
									.SetName(name)
									.Returns(ResolveExpectedResult());
							}
							SmartActivationStatus ResolveExpectedResult()
							{
								var result = SmartActivationStatus.SmartNotAvailable;
								if (isSmartActivationEnabled && divisionType == DivisionType.Electricity)
								{
									if (mcc == RegisterConfigType.MCC12)
									{
										result = SmartActivationStatus.SmartActive;
									}
									else if (new[]
									{
										CommsTechnicallyFeasibleValue.NotAvailableYet,
										CommsTechnicallyFeasibleValue.CTF1,
										CommsTechnicallyFeasibleValue.CTF2
									}.Any(x => x == ctf))
									{
										result = SmartActivationStatus.SmartButNotEligible;
									}
									else if (new[]
									{
										CommsTechnicallyFeasibleValue.CTF3,
										CommsTechnicallyFeasibleValue.CTF4
									}.Any(x => x == ctf) && new[] { RegisterConfigType.MCC01, RegisterConfigType.MCC16 }.Any(x =>
										x == mcc))
									{
										result = SmartActivationStatus.SmartAndEligible;
									}
								}

								return result;
							}
						}

					}
				}
			}
		}

		[TestCaseSource(nameof(ItCanMapDevicesStatusCases))]
		public async Task<SmartActivationStatus> ItCanMapDevices(bool isSmartActivationEnabled, DivisionType divisionType, RegisterConfigType mccConfiguration, CommsTechnicallyFeasibleValue ctf)
		{
			Context
				.WithIsSmartActivationEnabled(isSmartActivationEnabled)
				.WithDivisionType(divisionType)
				.WithMccConfig(mccConfiguration)
				.WithCTF(ctf);

			var deviceDto = Context.BuildDeviceDto();
			var installationDto = Context.BuildInstallationDto();
			var result = await Context
				.Sut
				.Map(deviceDto, installationDto);

			Assert.AreEqual(result.ContractId, deviceDto.ContractID);
			Assert.AreEqual(result.DeviceId, deviceDto.DeviceID);
			Assert.AreEqual(result.DivisionId, deviceDto.DivisionID);
			Assert.AreEqual(result.DeviceMaterial, deviceDto.DeviceMaterial);
			Assert.AreEqual(result.DeviceLocation, deviceDto.DeviceLocation);
			Assert.AreEqual(result.SerialNum, deviceDto.SerialNumber);
			Assert.AreEqual(result.DeviceDescription, deviceDto.DeviceDescription);
			Assert.AreEqual(result.IsSmart, deviceDto.FunctionClass == "1006");

			Assert.AreEqual(result.PremiseId, installationDto.PremiseID);

			return result.SmartActivationStatus;
		}
	}
}

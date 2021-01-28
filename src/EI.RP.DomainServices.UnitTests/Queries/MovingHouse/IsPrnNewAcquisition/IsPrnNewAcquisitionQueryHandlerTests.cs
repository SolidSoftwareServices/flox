using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.DomainServices.Queries.MovingHouse.IsPrnNewAcquisition;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.TestServices;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.UnitTests.Infrastructure.RepositoryMocks;
using EI.RP.DomainServices.Queries.Metering.Premises;
using Ei.Rp.DomainModels.Metering;
using EI.RP.CoreServices.System;
using AutoFixture;
using NUnit.Framework;
using Moq.AutoMock;

namespace EI.RP.DomainServices.UnitTests.Queries.MovingHouse.IsPrnNewAcquisition
{
	[TestFixture]
	internal class IsPrnNewAcquisitionQueryHandlerTests : UnitTestFixture<IsPrnNewAcquisitionQueryHandlerTests.TestContext, IsPrnNewAcquisitionQueryHandler>
	{
		internal class CaseModel
		{
			public bool GasPrn { get; set; }
			public bool IsPODIsNewAcquisition { get; set; }
			public bool HasInstallations { get; set; }
			public DeregStatusType InstallationsDeregStatus { get; set; }
			public bool IsNewAcquisitionResult { get; set; }

			public override string ToString()
			{
				return $"{nameof(GasPrn)}: {GasPrn}, {nameof(HasInstallations)}: {HasInstallations}, {nameof(InstallationsDeregStatus)}: {InstallationsDeregStatus}, {nameof(IsNewAcquisitionResult)}: {IsNewAcquisitionResult}";
			}
		}

		public static IEnumerable<TestCaseData> CanResolveCases()
		{
			var cases = new[]
			{
				new CaseModel { GasPrn = false, IsPODIsNewAcquisition = true, HasInstallations = true, InstallationsDeregStatus = DeregStatusType.Registered, IsNewAcquisitionResult = true },
				new CaseModel { GasPrn = false, IsPODIsNewAcquisition = false, HasInstallations = true, InstallationsDeregStatus = DeregStatusType.Registered, IsNewAcquisitionResult = false },
				new CaseModel { GasPrn = false, IsPODIsNewAcquisition = false, HasInstallations = true, InstallationsDeregStatus = DeregStatusType.Deregistered, IsNewAcquisitionResult = true },
				new CaseModel { GasPrn = false, IsPODIsNewAcquisition = false, HasInstallations = true, InstallationsDeregStatus = DeregStatusType.TariffExemption, IsNewAcquisitionResult = false },
				new CaseModel { GasPrn = false, IsPODIsNewAcquisition = false, HasInstallations = false, InstallationsDeregStatus = DeregStatusType.Registered, IsNewAcquisitionResult = true },
				new CaseModel { GasPrn = false, IsPODIsNewAcquisition = false, HasInstallations = false, InstallationsDeregStatus = DeregStatusType.Deregistered, IsNewAcquisitionResult = true },
				new CaseModel { GasPrn = false, IsPODIsNewAcquisition = false, HasInstallations = false, InstallationsDeregStatus = DeregStatusType.TariffExemption, IsNewAcquisitionResult = true },

				new CaseModel { GasPrn = true, IsPODIsNewAcquisition = true, HasInstallations = true, InstallationsDeregStatus = DeregStatusType.Registered, IsNewAcquisitionResult = true },
				new CaseModel { GasPrn = true, IsPODIsNewAcquisition = false, HasInstallations = true, InstallationsDeregStatus = DeregStatusType.Registered, IsNewAcquisitionResult = false },
				new CaseModel { GasPrn = true, IsPODIsNewAcquisition = false, HasInstallations = true, InstallationsDeregStatus = DeregStatusType.Deregistered, IsNewAcquisitionResult = true },
				new CaseModel { GasPrn = true, IsPODIsNewAcquisition = false, HasInstallations = true, InstallationsDeregStatus = DeregStatusType.TariffExemption, IsNewAcquisitionResult = false },
				new CaseModel { GasPrn = true, IsPODIsNewAcquisition = false, HasInstallations = false, InstallationsDeregStatus = DeregStatusType.Registered, IsNewAcquisitionResult = true },
				new CaseModel { GasPrn = true, IsPODIsNewAcquisition = false, HasInstallations = false, InstallationsDeregStatus = DeregStatusType.Deregistered, IsNewAcquisitionResult = true },
				new CaseModel { GasPrn = true, IsPODIsNewAcquisition = false, HasInstallations = false, InstallationsDeregStatus = DeregStatusType.TariffExemption, IsNewAcquisitionResult = true },
			};

			foreach (var caseItem in cases)
			{				
				yield return new TestCaseData(caseItem)
					.SetName(caseItem.ToString())
					.Returns(caseItem.IsNewAcquisitionResult);
			}
		}

		[TestCaseSource(nameof(CanResolveCases))]
		public async Task<bool> CanResolve(CaseModel caseModel)
		{
			var actual =
				await Context
				.WithGasPrn(caseModel.GasPrn)
				.WithIsPODIsNewAcquisition(caseModel.IsPODIsNewAcquisition)
				.WithHasInstallations(caseModel.HasInstallations)
				.WithInstallationsDeregStatus(caseModel.InstallationsDeregStatus)
				.WithIsNewAcquisitionResult(caseModel.IsNewAcquisitionResult)
				.Sut
				.ExecuteQueryAsync<IsPrnNewAcquisitionRequestResult>(Context.Query);

			return actual.Single().IsNewAcquisition;
		}

		public class TestContext : UnitTestContext<IsPrnNewAcquisitionQueryHandler>
		{
			private bool _gasPrn;
			private bool _isPODIsNewAcquisition;
			private bool _hasInstallations;
			private DeregStatusType _installationsDeregStatus;
			private bool _isNewAcquisitionResult;

			public IsPrnNewAcquisitionQuery Query { get; private set; }

			public TestContext WithGasPrn(bool gasPrn)
			{
				_gasPrn = gasPrn;
				return this;
			}

			public TestContext WithIsPODIsNewAcquisition(bool isPODIsNewAcquisition)
			{
				_isPODIsNewAcquisition = isPODIsNewAcquisition;
				return this;
			}

			public TestContext WithHasInstallations(bool hasInstallations)
			{
				_hasInstallations = hasInstallations;
				return this;
			}

			public TestContext WithInstallationsDeregStatus(DeregStatusType installationsDeregStatus)
			{
				_installationsDeregStatus = installationsDeregStatus;
				return this;
			}

			public TestContext WithIsNewAcquisitionResult(bool isNewAcquisitionResult)
			{
				_isNewAcquisitionResult = isNewAcquisitionResult;
				return this;
			}

			protected override IsPrnNewAcquisitionQueryHandler BuildSut(AutoMocker autoMocker)
			{
				var domainFacade = new DomainFacade();
				domainFacade.SetUpMocker(autoMocker);

				IFixture fixture = new Fixture().CustomizeDomainTypeBuilders();

				var gasPrn = fixture.Create<GasPointReferenceNumber>();
				var electricityPrn = fixture.Create<ElectricityPointReferenceNumber>();
				var prn = _gasPrn ? (PointReferenceNumber)gasPrn : (PointReferenceNumber)electricityPrn;

				var deviceInfos = fixture
					.Build<DeviceInfo>()
					.With(d => d.MeterType, _gasPrn ? MeterType.Gas : MeterType.Electricity24h)
					.Create();

				var installations = fixture.Build<InstallationInfo>()
					.With(x => x.Devices, deviceInfos.ToOneItemArray())
					.With(x => x.DeregStatus, _installationsDeregStatus)
					.Create()
					.ToOneItemArray()
					.AsEnumerable();

				var premise = _hasInstallations ?
						fixture.Build<Premise>()
						.With(x => x.Installations, installations)				
						.Create() :
						fixture.Build<Premise>()
						.Without(x => x.Installations)
						.Create();

				domainFacade.QueryResolver.ExpectQuery(new PremisesQuery
				{
					Prn = prn
				}, premise.ToOneItemArray().AsEnumerable());

				Query = Fixture.Build<IsPrnNewAcquisitionQuery>()
					.With(x => x.Prn, prn)
					.With(x => x.IsPODNewAcquisition, _isPODIsNewAcquisition)
					.Create();

				return base.BuildSut(autoMocker);
			}
		}
	}
}
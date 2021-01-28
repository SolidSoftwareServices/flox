using System.Security.Claims;
using AutoFixture;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.Membership;
using EI.RP.DomainModels.SpecimenBuilders.Mocks;
using EI.RP.DomainModels.SpecimenBuilders.PrimitiveBuilders;
using EI.RP.Stubs.CoreServices.Http.Session;
using Moq.AutoMock;

namespace EI.RP.DomainModels.SpecimenBuilders.RichBuilders
{
	/// <summary>
	/// A facade for all the domain
	/// </summary>
	public class DomainFacade
	{
		/// <summary>
		/// The facade of the domain
		/// </summary>
		public DomainCommandDispatcherMock CommandDispatcher { get; }
		/// <summary>
		/// The domain queries facade
		/// </summary>
		public DomainQueryResolverMock QueryResolver { get; }

		/// <summary>
		/// The domain models builder
		/// </summary>
		public IFixture ModelsBuilder { get; }

		/// <summary>
		/// A session provider for user-specific scenarios
		/// </summary>
		public FakeSessionProviderStrategy SessionProvider { get; private set; }

	
		public DomainFacade()
		{
			ModelsBuilder = new Fixture().CustomizeDomainTypeBuilders();
			QueryResolver = new DomainQueryResolverMock();

			CommandDispatcher = new DomainCommandDispatcherMock(QueryResolver);
			SessionProvider = new FakeSessionProviderStrategy();
		}


		public void SetValidSessionFor(string userName, ResidentialPortalUserRole userRole)
		{
			SessionProvider 
				.CreateSession(new Claim(ClaimTypes.Name, userName),
					new Claim(ClaimTypes.Email, userName),
					new Claim(ClaimTypes.NameIdentifier, userName),
					new Claim(ClaimTypes.Role, userRole as ResidentialPortalUserRole),
					new Claim(ResidentialPortalClaim.Csrf, ModelsBuilder.Create<string>()),
					new Claim(ResidentialPortalClaim.SapCookie, ModelsBuilder.Create<string>())).Wait();
		}

		public void Reset()
		{
			CommandDispatcher.Reset();
			QueryResolver.Reset();
			SessionProvider.Reset();
		}

	
	}

	public abstract class AppDomainConfigurator
	{
		public DomainFacade DomainFacade { get; }

		protected AppDomainConfigurator(DomainFacade domainFacade)
		{
			DomainFacade = domainFacade; 
		}

	}
}
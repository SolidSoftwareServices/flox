using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using EI.RP.CoreServices.OData.Client;
using EI.RP.DataModels.Sap;
using EI.RP.DomainServices.ModelExtensions;
using EI.RP.Stubs.CoreServices.Http.Session;
using NUnit.Framework;

namespace EI.RP.DataServices.SAP.Clients.IntegrationTests.Infrastructure
{
	[TestFixture]

	public abstract class SapTest
	{
		private ILifetimeScope _scope;

		protected ISapRepositoryOfCrmUmc CrmUmc =>
			_scope.Resolve<ISapRepositoryOfCrmUmc>();
		protected ISapRepositoryOfErpUmc ErpUmc =>
			_scope.Resolve<ISapRepositoryOfErpUmc>();
		protected ISapRepositoryOfCrmUmcUrm CrmUmcUrm =>
			_scope.Resolve<ISapRepositoryOfCrmUmcUrm>();

		protected ISapRepositoryOfUserManagement UserManagement =>
			_scope.Resolve<ISapRepositoryOfUserManagement>();

        protected async Task LoginUser(string userName, string password)
        {
	        SapSessionData session = await UserManagement.LoginUser(userName.AdaptToSapUserNameFormat(),
		        password.AdaptToSapPasswordFormat());
	        SessionDataHolder.Instance.SapCsrf = session.Csrf;
	        SessionDataHolder.Instance.SapJsonCookie = session.JsonCookie;
        }
		[SetUp]
        public virtual void OnSetUp()
        {
	        _scope = AssemblySetUp.Container.BeginLifetimeScope();
        }

        [TearDown]
        public virtual void OnTearDown()
        {
			_scope.Dispose();
			_scope = null;
        }
	}
}
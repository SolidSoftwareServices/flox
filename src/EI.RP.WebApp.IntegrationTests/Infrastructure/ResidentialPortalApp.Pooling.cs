using System;
using Microsoft.Extensions.ObjectPool;

namespace EI.RP.WebApp.IntegrationTests.Infrastructure
{
	internal partial class ResidentialPortalApp
	{
		private static readonly ObjectPool<ResidentialPortalApp> PublicPool =
			new DefaultObjectPool<ResidentialPortalApp>(new PublicPoolPolicy());

		private static readonly ObjectPool<ResidentialPortalApp> InternalPool =
			new DefaultObjectPool<ResidentialPortalApp>(new InternalPoolPolicy());

		public static ResidentialPortalApp StartNew(ResidentialPortalDeploymentType runAsDeploymentType)
		{
			Logger.Info("Starting App");
			switch (runAsDeploymentType)
			{
				case ResidentialPortalDeploymentType.Public:
					return PublicPool.Get();
				case ResidentialPortalDeploymentType.Internal:
					return InternalPool.Get();
				default:
					throw new ArgumentOutOfRangeException(nameof(runAsDeploymentType), runAsDeploymentType, null);
			}
		}

		/// <summary>
		///     frees the app for another test reseting it
		/// </summary>
		/// <returns></returns>
		public override void Release()
		{
			switch (RunningAsDeploymentType)
			{
				case ResidentialPortalDeploymentType.Public:
					PublicPool.Return(this);
					break;
				case ResidentialPortalDeploymentType.Internal:
					InternalPool.Return(this);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private abstract class PoolPolicy : PooledObjectPolicy<ResidentialPortalApp>
		{
			protected abstract ResidentialPortalDeploymentType Policy { get; }

			public override ResidentialPortalApp Create()
			{
				var app = new ResidentialPortalApp(Policy);
				return app;
			}

			public override bool Return(ResidentialPortalApp app)
			{
				app.Reset();

				return true;
			}
		}

		private class PublicPoolPolicy : PoolPolicy
		{
			protected override ResidentialPortalDeploymentType Policy { get; } = ResidentialPortalDeploymentType.Public;
		}

		private class InternalPoolPolicy : PoolPolicy
		{
			protected override ResidentialPortalDeploymentType Policy { get; } =
				ResidentialPortalDeploymentType.Internal;
		}
	}
}
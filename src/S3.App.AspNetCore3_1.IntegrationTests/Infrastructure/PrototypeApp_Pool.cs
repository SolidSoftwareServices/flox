using Microsoft.Extensions.ObjectPool;

namespace S3.App.AspNetCore3_1.IntegrationTests.Infrastructure
{
	internal partial class PrototypeApp
	{
		private static readonly ObjectPool<PrototypeApp> AppPool = new DefaultObjectPool<PrototypeApp>(new DefaultPoolPolicy());

		private class DefaultPoolPolicy : PooledObjectPolicy<PrototypeApp>
		{
			
			public override PrototypeApp Create()
			{
				return new PrototypeApp();
			}

			public override bool Return(PrototypeApp app)
			{
				app.Reset();
				return true;
			}
		}
		public static PrototypeApp StartNew()
		{
			return AppPool.Get();
		}

		public override void Release()
		{
			AppPool.Return(this);
		}
	}
}
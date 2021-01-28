using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.ObjectPool;

namespace EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers
{
	internal class DriversPool:IDisposable
	{
		public static readonly Lazy<DriversPool> Default = new Lazy<DriversPool>(() => new DriversPool());
		private readonly Dictionary<DriverType, ObjectPool<ResidentialPortalWebDriver>> _pools;

		private readonly ConcurrentDictionary<Guid,ResidentialPortalWebDriver> _createdInstances=new ConcurrentDictionary<Guid, ResidentialPortalWebDriver>();

		private DriversPool()
		{
			_pools = new Dictionary<DriverType, ObjectPool<ResidentialPortalWebDriver>>
			{
				{
					DriverType.Chrome,
					new DefaultObjectPool<ResidentialPortalWebDriver>(new PoolPolicy(DriverType.Chrome))
				},
				{
					DriverType.ZapChrome,
					new DefaultObjectPool<ResidentialPortalWebDriver>(new PoolPolicy(DriverType.ZapChrome))
				},
				{
					DriverType.RemoteChrome,
					new DefaultObjectPool<ResidentialPortalWebDriver>(new PoolPolicy(DriverType.RemoteChrome))
				}
			};
		}

		public ResidentialPortalWebDriver GetOneReleasedOrCreateNew()
		{
			ThrowIfDisposed();
			var driverType = ResolveDriverType();
			var driver = _pools[driverType].Get();
			_createdInstances.GetOrAdd(driver.DriverId, (k) => driver);
			return driver;
		}

		private static DriverType ResolveDriverType()
		{
			DriverType driverType;
			if (TestSettings.Default.SecurityTest)
				driverType = DriverType.ZapChrome;
			else
				driverType =
					TestSettings.Default.UseLocalWebdriver
						? DriverType.Chrome
						: DriverType.RemoteChrome;

			return driverType;
		}


		public void Release(ResidentialPortalWebDriver instance)
		{
			ThrowIfDisposed();
			_pools[ResolveDriverType()].Return(instance);
		}


		private class PoolPolicy : PooledObjectPolicy<ResidentialPortalWebDriver>
		{
			private readonly DriverType _driverType;

			public PoolPolicy(DriverType driverType)
			{
				_driverType = driverType;
			}


			public override bool Return(ResidentialPortalWebDriver driver)
			{
				//clean the state before returning it to the pool
				driver.Instance.Manage().Cookies.DeleteAllCookies();
				return true;
			}

			public override ResidentialPortalWebDriver Create()
			{
				return new ResidentialPortalWebDriver(_driverType);
			}
		}

		private void ThrowIfDisposed()
		{
			if(_disposed) throw new ObjectDisposedException(nameof(DriversPool));
		}

		private bool _disposed;
		public void Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				ThrowIfDisposed();
				foreach (var driver in _createdInstances.Values)
				{
					try
					{
						driver.Dispose();
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
					}
				}
			}
			
			_disposed=true;
		}

		~DriversPool()
		{
			Dispose(false);
		}
	}
}
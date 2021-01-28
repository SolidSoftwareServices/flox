using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EI.RP.CoreServices.System.DesignPatterns.Observer
{
	/// <summary>Provides a mechanism for receiving push-based notifications.</summary>
	/// <typeparam name="T">The object that provides notification information.</typeparam>
	public interface IAsyncObserver<in T>
	{
		/// <summary>Notifies the observer that the provider has finished sending push-based notifications.</summary>
		Task OnCompletedAsync();

		/// <summary>Notifies the observer that the provider has experienced an error condition.</summary>
		/// <param name="error">An object that provides additional information about the error.</param>
		Task OnErrorAsync(Exception error);

		/// <summary>Provides the observer with new data.</summary>
		/// <param name="value">The current notification information.</param>
		Task OnNextAsync(T value);
	}
}

using System;

namespace Ei.Rp.Mvc.Core.Cryptography.AntiTampering
{
	public class HiddenInputTamperedException : Exception
	{
		public HiddenInputTamperedException(string message) : base(message)
		{
		}

		public HiddenInputTamperedException(string message, Exception innerException) :base(message,innerException)
		{
		}
	}
}
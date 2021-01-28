using System;
using System.Collections.Generic;

namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Definitions.Accounts
{
	static partial class EnvironmentSet
	{
		private static Lazy<IEnvironmentInputs> _decorated = new Lazy<IEnvironmentInputs>(() =>
		{
			switch (TestSettings.Default.AccountsSet.ToLower())
			{
				case "test":
					return new TestInputs();
				case "preprod":
					return new PreProdInputs();
				default:
					throw new NotSupportedException();
			}

		});

		public static  IEnvironmentInputs Inputs => _decorated.Value;
	}


}
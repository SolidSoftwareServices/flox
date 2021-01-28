using System;
using System.Collections.Generic;
using System.Linq;
using Ei.Rp.DomainModels.MappingValues;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.Plan.Components
{
	static class PlanDetailsComponentCasesBuilder
	{
		public static IEnumerable<TestCaseData> CreateTestCasesForResult<TResult>(string prefix,
			Func<ClientAccountType, bool, bool, bool, TResult> resultResolver)
		{
			var bools = new[] { true, false };
			foreach (var isOpen in bools)
			foreach (var hasElectricity in bools)
			foreach (var hasGas in bools)
			{
				if (!hasElectricity && !hasGas) continue;
				foreach (var isAlreadySmart in bools)
				foreach (var canBeSmart in bools)
				{
					if (isAlreadySmart && canBeSmart) continue;
					foreach (var paymentType in PaymentMethodType.AllValues.Cast<PaymentMethodType>())
					foreach (var selectedAccount in new[] { ClientAccountType.Electricity, ClientAccountType.Gas })
					{
						if (selectedAccount.IsElectricity() && !hasElectricity
						    || selectedAccount.IsGas() && !hasGas) continue;
						yield return new TestCaseData(hasElectricity, hasGas, isAlreadySmart, canBeSmart,
								selectedAccount, isOpen, paymentType)
							.SetName(ResolveName())
							.Returns(resultResolver(selectedAccount, isOpen, isAlreadySmart, canBeSmart));



						string ResolveName()
						{
							var separator = hasGas && hasElectricity ? "," : string.Empty;
							var e = hasElectricity ? "Electricity" : string.Empty;
							var g = hasGas ? "Gas" : string.Empty;
							var s = isAlreadySmart ? string.Empty : "NOT";
							var c = canBeSmart ? ",BUT can be made smart" : string.Empty;
							var o = isOpen ? "" : "NOT";
							return
								$"{prefix} ->Accounts:[{e}{separator}{g}] selected {selectedAccount}  is {s} smart {c} and electricity is {o} opened with {paymentType.DebuggerDisplayValue} paymentType";
						}
					}
				}
			}

		}
	}
}
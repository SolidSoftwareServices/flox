using System;
using System.Collections.Generic;

namespace S3.UiFlows.Core.Registry
{
	

	public interface IFlowsRegistry
	{
		IEnumerable<FlowRegister> AllFlows { get; }
		FlowRegister GetByName(string name,bool failIfNotFound=false);
		FlowRegister GetByType(Type type, bool failIfNotFound = false);
	}
}
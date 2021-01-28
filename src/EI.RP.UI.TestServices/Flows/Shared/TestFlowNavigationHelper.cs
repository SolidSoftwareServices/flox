using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;


namespace EI.RP.UI.TestServices.Flows.Shared
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	abstract class TestFlowNavigationHelper : IScreenFlowConfigurator
	{
		private readonly ScreenName _currentStep;


		protected Dictionary<ScreenEvent,NavigationResolver> NavigationResolvers { get; }=new Dictionary<ScreenEvent, NavigationResolver>();

		public TestFlowNavigationHelper(ScreenName currentStep)
		{
			_currentStep = currentStep;
		}

		
		public IScreenFlowConfigurator OnEventNavigatesTo(ScreenEvent whenEvent, ScreenName navigatesTo)
		{
			return OnEventNavigatesTo(whenEvent,navigatesTo, ()=>true,null);
		}

		
		public IScreenFlowConfigurator OnEventsNavigatesTo(ScreenEvent[] whenEvents, ScreenName navigatesTo)
		{
			var instance = (IScreenFlowConfigurator)this;
			foreach (var stepEvent in whenEvents)
			{
				instance=instance.OnEventNavigatesTo(stepEvent, navigatesTo);
			}

			return instance;
		}

		public IScreenFlowConfigurator OnEventNavigatesTo(ScreenEvent whenEvent, ScreenName navigatesTo, Func<bool> whenConditionMatches,
			string conditionFriendlyDescription)
		{
			var resolver = GetResolver(whenEvent);

			resolver.Add(navigatesTo,whenConditionMatches,conditionFriendlyDescription);

			return this;
		}

		public IScreenFlowConfigurator OnEventNavigatesToAsync(ScreenEvent whenEvent, ScreenName navigatesTo, Func<Task<bool>> whenConditionMatches,
			string conditionFriendlyDescription)
		{
			return OnEventNavigatesTo(whenEvent, navigatesTo, () => whenConditionMatches().GetAwaiter().GetResult(),
				conditionFriendlyDescription);
		}

		public IScreenFlowConfigurator OnEventReentriesCurrent(ScreenEvent whenEvent)
		{
			return OnEventNavigatesTo(whenEvent, _currentStep);
		}

		public IScreenFlowConfigurator OnEventsReentriesCurrent(ScreenEvent[] whenEvents)
		{
			return OnEventsReentriesCurrent(whenEvents, null, null);
		}

		public IScreenFlowConfigurator OnEventReentriesCurrent(ScreenEvent whenEvent, Func<bool> whenConditionMatches,
			string conditionFriendlyDescription)
		{
			return OnEventNavigatesTo(whenEvent, _currentStep, whenConditionMatches, conditionFriendlyDescription);
		}

		public IScreenFlowConfigurator OnEventReentriesCurrentAsync(ScreenEvent whenEvent, Func<Task<bool>> whenConditionMatches,
			string conditionFriendlyDescription)
		{
			return OnEventReentriesCurrent(whenEvent, () => whenConditionMatches().GetAwaiter().GetResult(),
				conditionFriendlyDescription);
		}

		public IScreenFlowConfigurator OnEventsReentriesCurrent(ScreenEvent[] whenEvents, Func<bool> whenConditionMatches,
			string conditionFriendlyDescription)
		{
			var instance = (IScreenFlowConfigurator)this;
			foreach (var whenEvent in whenEvents)
			{
				instance = instance.OnEventReentriesCurrent(whenEvent,whenConditionMatches,conditionFriendlyDescription);
			}

			return instance;
		}

		public IScreenFlowConfigurator SubStepOf(ScreenName step)
		{
			throw new NotImplementedException();
		}

		public ScreenName Execute(ScreenEvent eventToTrigger)
		{
			return NavigationResolvers.ContainsKey(eventToTrigger)
				? OnExecute(NavigationResolvers[eventToTrigger])
				: _currentStep;
		}

		protected abstract ScreenName OnExecute(NavigationResolver navigationResolver);

		private NavigationResolver GetResolver(ScreenEvent whenEvent)
		{
			return NavigationResolvers.GetOrAdd(whenEvent, () => new NavigationResolver());
		}
		internal class NavigationResolver
		{
			private readonly HashSet<DestinationInfo> _destinations = new HashSet<DestinationInfo>();
			public void Add(ScreenName destination)
			{
				Add(destination,null,null);
			}

			public void Add(ScreenName destination, Func<bool> condition, string conditionFriendlyDescription)
			{
				if(condition==null&& _destinations.Any(x=>x.Condition==null)) throw new InvalidOperationException("Bad navigation configuration, there is already one without condition");
				_destinations.Add(new DestinationInfo(destination,condition,conditionFriendlyDescription));
			}
			

			public ScreenName Resolve()
			{
				var destinationInfo = _destinations.Single(x => x.Condition == null || x.Condition());
				return destinationInfo.DestinationStep;
			}
			

			private class DestinationInfo : IEquatable<DestinationInfo>
			{
				public DestinationInfo(ScreenName destinationStep, Func<bool> condition = null, string conditionFriendlyDescription = null)
				{
					DestinationStep = destinationStep;
					Condition = condition;
					ConditionFriendlyDescription = conditionFriendlyDescription;
				}
				public ScreenName DestinationStep { get; }

				public string ConditionFriendlyDescription { get; set; }

				public Func<bool> Condition { get; set; }

				public bool Equals(DestinationInfo other)
				{
					if (ReferenceEquals(null, other)) return false;
					if (ReferenceEquals(this, other)) return true;
					return Equals(DestinationStep, other.DestinationStep) && ConditionFriendlyDescription == other.ConditionFriendlyDescription && Equals(Condition, other.Condition);
				}

				public override bool Equals(object obj)
				{
					if (ReferenceEquals(null, obj)) return false;
					if (ReferenceEquals(this, obj)) return true;
					if (obj.GetType() != this.GetType()) return false;
					return Equals((DestinationInfo) obj);
				}

				public override int GetHashCode()
				{
					return HashCode.Combine(DestinationStep, ConditionFriendlyDescription, Condition);
				}

				public static bool operator ==(DestinationInfo left, DestinationInfo right)
				{
					return Equals(left, right);
				}

				public static bool operator !=(DestinationInfo left, DestinationInfo right)
				{
					return !Equals(left, right);
				}
			}

			
		}
	}
}
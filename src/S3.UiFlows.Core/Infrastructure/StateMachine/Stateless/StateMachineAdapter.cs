using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.Flows.Screens;
using Stateless;
using Stateless.Graph;

namespace S3.UiFlows.Core.Infrastructure.StateMachine.Stateless
{
	internal class StateMachineAdapter : IInternalStateMachine
	{
		private StateMachine<ScreenName, ScreenEvent> _machine;

		public IInternalStateMachine BuildNew(ScreenName initialStep)
		{
			_machine = new StateMachine<ScreenName, ScreenEvent>(initialStep);
			return this;
		}

		public bool WasCreated => _machine != null;

		public async Task ExecuteTrigger(ScreenEvent transitionTrigger)
		{
			await _machine.FireAsync(transitionTrigger);
		}

		public IInternalScreenFlowConfigurator Configure(ScreenName targetStep)
		{
			return new ScreenFlowStateMachineAdapter(_machine.Configure(targetStep));
		}

		public string ToDotGraph()
		{
			return UmlDotGraph.Format(_machine.GetInfo());
		}

		public void Reset()
		{
			_machine = null;
		}


		public ScreenName CurrentStep => _machine.State;

		private class ScreenFlowStateMachineAdapter : IInternalScreenFlowConfigurator
		{
			private readonly HashSet<ScreenEvent> _events = new HashSet<ScreenEvent>();

			private readonly HashSet<ScreenTransition> _transitions = new HashSet<ScreenTransition>();

			public ScreenFlowStateMachineAdapter(
				StateMachine<ScreenName, ScreenEvent>.StateConfiguration targetStep)
			{
				Target = targetStep;
			}

			public StateMachine<ScreenName, ScreenEvent>.StateConfiguration Target { get; }


			public void OnEntry(Func<Task> action, string entryActionDescription = null)
			{
				Target.OnEntryAsync(action, entryActionDescription);
			}

			public void AddErrorTransitionIfUndefined()
			{
				if (!_events.Contains(ScreenEvent.ErrorOccurred))
					//Add default reentry on error
					OnEventReentriesCurrent(ScreenEvent.ErrorOccurred);
			}

			public string ScreenName => Target.State;
			public IEnumerable<ScreenTransition> Transitions => _transitions.ToArray();


			public IScreenFlowConfigurator OnEventNavigatesTo(ScreenEvent whenEvent, ScreenName navigatesTo)
			{
				return OnEventNavigatesTo(whenEvent, navigatesTo, null, null);
			}

			public IScreenFlowConfigurator OnEventsNavigatesTo(ScreenEvent[] whenEvents, ScreenName navigatesTo)
			{
				foreach (var whenEvent in whenEvents) OnEventNavigatesTo(whenEvent, navigatesTo);

				return this;
			}

			public IScreenFlowConfigurator OnEventNavigatesToAsync(ScreenEvent whenEvent, ScreenName navigatesTo,
				Func<Task<bool>> whenConditionMatches, string conditionFriendlyDescription)
			{
				return OnEventNavigatesTo(whenEvent, navigatesTo, () => whenConditionMatches().GetAwaiter().GetResult(),
					conditionFriendlyDescription);
			}

			public IScreenFlowConfigurator OnEventNavigatesTo(ScreenEvent whenEvent, ScreenName navigatesTo,
				Func<bool> whenConditionMatches, string conditionFriendlyDescription)
			{
				if (whenConditionMatches == null)
				{
					Target.Permit(whenEvent, navigatesTo);
					_transitions.Add(new ScreenTransition(whenEvent, navigatesTo));
				}
				else
				{
					if (conditionFriendlyDescription == null)
						throw new ArgumentException(
							"Specify a friendly name of the condition that will appear on the dotgraph");
					Target.PermitIf(whenEvent, navigatesTo, whenConditionMatches, conditionFriendlyDescription);
					_transitions.Add(new ScreenTransition(whenEvent, navigatesTo, conditionFriendlyDescription));
				}

				AddUsedEventIfNotExists(whenEvent);
				return this;
			}

			public IScreenFlowConfigurator OnEventReentriesCurrent(ScreenEvent whenEvent)
			{
				return OnEventReentriesCurrent(whenEvent, null, null);
			}

			public IScreenFlowConfigurator OnEventsReentriesCurrent(ScreenEvent[] whenEvents)
			{
				foreach (var @event in whenEvents) OnEventReentriesCurrent(@event);

				return this;
			}

			public IScreenFlowConfigurator OnEventReentriesCurrentAsync(ScreenEvent whenEvent,
				Func<Task<bool>> whenConditionMatches,
				string conditionFriendlyDescription)
			{
				return this.OnEventReentriesCurrent(whenEvent, () => whenConditionMatches().GetAwaiter().GetResult(),conditionFriendlyDescription);
			}
			public IScreenFlowConfigurator OnEventReentriesCurrent(ScreenEvent whenEvent,
				Func<bool> whenConditionMatches,
				string conditionFriendlyDescription)
			{
				if (whenConditionMatches == null)
				{
					Target.PermitReentry(whenEvent);
					_transitions.Add(new ScreenTransition(whenEvent, ScreenName));
				}
				else
				{
					if (conditionFriendlyDescription == null)
						throw new ArgumentException(
							"Specify a friendly name of the condition that will appear on the dotgraph");
					Target.PermitReentryIf(whenEvent, whenConditionMatches, conditionFriendlyDescription);
					_transitions.Add(new ScreenTransition(whenEvent, ScreenName, conditionFriendlyDescription));
				}

				AddUsedEventIfNotExists(whenEvent);
				return this;
			}

			public IScreenFlowConfigurator OnEventsReentriesCurrent(ScreenEvent[] whenEvents,
				Func<bool> whenConditionMatches,
				string conditionFriendlyDescription)
			{
				foreach (var @event in whenEvents)
					OnEventReentriesCurrent(@event, whenConditionMatches, conditionFriendlyDescription);

				return this;
			}

			public IScreenFlowConfigurator SubStepOf(ScreenName step)
			{
				Target.SubstateOf(step);
				return this;
			}

			private void AddUsedEventIfNotExists(ScreenEvent whenEvent)
			{
				if (!_events.Contains(whenEvent)) _events.Add(whenEvent);
			}
		}
	}
}
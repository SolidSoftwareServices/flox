using System;
using System.Linq;
using EI.RP.CoreServices.System;
using Newtonsoft.Json;

namespace EI.RP.UiFlows.Core.Flows.Screens
{
	public sealed class ScreenEvent : TypedStringValue<ScreenEvent>
	{
		private const char QualificationSeparator = '.';

		//Default triggers
		public static ScreenEvent Start = new ScreenEvent(nameof(Start));
		public static ScreenEvent ErrorOccurred = new ScreenEvent(nameof(ErrorOccurred));

		[JsonConstructor]
		private ScreenEvent()
		{
		}

		public ScreenEvent(string screenName, string triggerName) : this(
			$"{screenName}{QualificationSeparator}{triggerName}")
		{
		}

		private ScreenEvent(string triggerName) : base(triggerName, disableVerifyValueExists: true)
		{
#if DEBUG || FrameworkDevelopment
			if (triggerName != nameof(Start) && triggerName != nameof(ErrorOccurred) &&
			    triggerName.Split(QualificationSeparator).Count(x => !string.IsNullOrWhiteSpace(x)) != 2)
				throw new ArgumentException(
					$"The trigger({triggerName}) is not fully qualified, it should contain the name of the screen that triggers it: '<StepName>_{triggerName}' instead of '{triggerName}'");
#endif
		}

		public static implicit operator ScreenEvent(string src)
		{
			return new ScreenEvent(src);
		}
	}
}
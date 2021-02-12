using System;

namespace S3.UiFlows.Core.Flows.Screens.ErrorHandling
{
	public class UiFlowUserInputError
	{
		private string _memberName = string.Empty;

		public string MemberName
		{
			get => _memberName;
			set => _memberName = value ?? throw new InvalidOperationException();
		}

		public string ErrorMessage { get; set; }
	}
}
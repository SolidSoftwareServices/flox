namespace EI.RP.DataServices.SAP.Clients.ErrorHandling
{
	public class NoODataException
	{
		public ErrorInfo Error { get; set; }
		public class ErrorInfo
		{
			public string Code { get; set; }

			public ErrorMessage Message { get; set; }

			public class ErrorMessage
			{
				public string Value { get; set; }
			}
		}
	}
}
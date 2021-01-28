namespace EI.RP.WebApp.Infrastructure.PresentationServices.EventsPublisher
{
	public class UiEventInfo
	{
		public long SubCategoryId { get; set; }
		public long CategoryId { get; set; }
		public string Description { get; set; }
		public string AccountNumber { get; set; }
		public string Partner { get; set; }
		public string MPRN { get; set; }

		public bool IsValid()
		{
			return !string.IsNullOrWhiteSpace(Description) && SubCategoryId != 0;
		}
	}
}
namespace EI.RP.WebApp.Models.Charts
{
    public class Price
    {
        public string Currency => "EUR";

        public decimal[] Values { get; set; } = new decimal[0];

        public decimal Total { get; set; }
    }
}

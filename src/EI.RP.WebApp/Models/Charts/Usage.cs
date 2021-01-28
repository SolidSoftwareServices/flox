namespace EI.RP.WebApp.Models.Charts
{
    public class Usage
    {
        public string Unit => "kWh";

        public decimal[] Values { get; set; } = new decimal[0];

        public decimal Total { get; set; }
    }
}

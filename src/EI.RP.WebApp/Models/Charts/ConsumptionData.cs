namespace EI.RP.WebApp.Models.Charts
{
    public class ConsumptionData
    {
        public ConsumptionData()
        {
            Period = new Period();
            Price = new Price();
            Usage = new Usage();
        }

        public Period Period { get; set; }

        public Price Price { get; set; }

        public Usage Usage { get; set;  }
    }
}

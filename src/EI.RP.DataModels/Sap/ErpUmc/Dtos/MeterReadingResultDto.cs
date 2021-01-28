using System.Globalization;

namespace EI.RP.DataModels.Sap.ErpUmc.Dtos
{
	public partial class MeterReadingResultDto
	{
		public void Clear()
		{
			MeterReadingResultID = DeviceID = RegisterID = MeterReadingReasonID = MeterReadingCategoryID = MeterReadingNoteID = SerialNumber = Email = Lcpe = FmoRequired = Vkont = PchaRequired = null;
		}

		public decimal ReadingResultAsDecimal()
		{
			return decimal.Parse(ReadingResult);
			
		}

		public void SetReadingResult(decimal value)
		{
			ReadingResult = value.ToString(CultureInfo.InvariantCulture);
		}

		public decimal? ConsumptionAsDecimal()
		{
			return Consumption!=null? (decimal?)decimal.Parse(Consumption):null;

		}

	}
}
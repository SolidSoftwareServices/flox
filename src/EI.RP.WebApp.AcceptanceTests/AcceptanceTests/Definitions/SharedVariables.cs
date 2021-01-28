namespace EI.RP.WebApp.AcceptanceTests.AcceptanceTests.Utils
{
    public class SharedVariables
    {      
        public bool MoveElecOnly  { get; set; }

        public bool MoveGasOnly { get; set; }

        public bool MoveElecAndGas { get; set; }

        public bool MoveElecAddGas { get; set; }

        public bool MoveGasAddElec { get; set; }

        public bool ElecMeterMoveIn { get; set; }

        public bool ElecMeterMoveOut { get; set; }

        public bool GasMeterMoveIn { get; set; }

        public bool GasMeterMoveOut { get; set; }

        public string NewMPRN { get; set; }

        public string NewGPRN { get; set; }

        public string
            step1TabID = "step1",
            step2TabID = "step2",
            step3TabID = "step3",
            step4TabID = "step4",
            step5TabID = "step5";


        public enum queryTypes
        {
            meterReadQuery,
            additionalAccount,
            billOrPaymentQuery,
            other,
        }
    }
}


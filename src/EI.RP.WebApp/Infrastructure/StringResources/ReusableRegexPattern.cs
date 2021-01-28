namespace EI.RP.WebApp.Infrastructure.StringResources
{
	public static class ReusableRegexPattern
	{
		public const string RegexAccountNumber = @"^([0-9]{9})$";
		public const string RegexShortMPRNGPRNNumber = @"^([0-9]{6})$";
		public const string RegexPassword = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,40}$";
        public const string ValidMeterReading = @"^([0-9]*)$|^$";
        public const string ValidCurrency = @"^\d+(\.\d{1,2})?$";
        public const string ValidStringWithoutSymbols = @"^([A-Za-z0-9@-_\[\].,\']*)$";
        public const string ValidAccountNumber = @"^([0-9]{9})$";
        public const string ValidAccountQuery = @"^[^<><|>]+$";
        public const string ValidSubject = @"^[^<><|>]+$";
        
        public const string ValidEmail = @"^((([a-zA-Z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-zA-Z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-zA-Z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-zA-Z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-zA-Z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-zA-Z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-zA-Z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-zA-Z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-zA-Z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-zA-Z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$";
        public const string ValidPhoneNumber = @"^([\+]{0,1}[0-9- .]{8,21})$";
        public const string ValidName = @"^([A-Za-z&.` \']*)$";
	}
}
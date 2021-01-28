using System;

namespace EI.RP.CoreServices.ErrorHandling
{
    /// <summary>
    /// represents a domain exception 
    /// </summary>
    public class DomainException : Exception
    {
        public DomainError DomainError { get; }

        public DomainException(DomainError domainError, Exception innerException=null,string extraInfo=null):this(domainError,domainError.ErrorMessage,innerException,extraInfo)
        {
        }
        public DomainException(DomainError domainError, string message, Exception innerException = null, string extraInfo = null) : base(message, innerException)
        {
	        DomainError = domainError ?? throw new ArgumentNullException(nameof(domainError));
	        ExtraInfo = extraInfo;
			
        }
		public string ExtraInfo { get; set; }

		public override string Message 
		#if DEBUG
			=> $"{DomainError.ErrorCode} - {DomainError.ErrorMessage} - {ExtraInfo}";
		#else
			=> $"{DomainError.ErrorMessage}";
#endif
    }
}
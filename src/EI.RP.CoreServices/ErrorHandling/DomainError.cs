namespace EI.RP.CoreServices.ErrorHandling
{
    /// <summary>
    /// This represents a domain error
    /// </summary>
    public class DomainError
    {
        protected DomainError(int errorCode, string defaultErrorMessage="")
        {
            ErrorCode = errorCode;
            ErrorMessage = defaultErrorMessage;
        }
        public string ErrorMessage { get; set; }
        public int ErrorCode { get; }

        protected bool Equals(DomainError other)
        {
            return string.Equals(ErrorMessage, other.ErrorMessage) && ErrorCode == other.ErrorCode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DomainError) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ErrorMessage != null ? ErrorMessage.GetHashCode() : 0) * 397) ^ ErrorCode;
            }
        }


        //default errors
        public static readonly DomainError Undefined = new DomainError(-10, "Unhandled error.");
        public static readonly DomainError GeneralValidation = new DomainError(-20, "Invalid data provided.");

	}
}

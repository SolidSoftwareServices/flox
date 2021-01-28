using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers
{
    /// <summary>
    /// It represents an MPRN/GPRN/...prns
    /// </summary>
    public abstract class PointReferenceNumber : IEquatable<PointReferenceNumber>
    {

        protected PointReferenceNumber(string input, PointReferenceNumberType type,
            params string[] validationExpressions)
        {
            if (validationExpressions.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(validationExpressions));
            if (!string.IsNullOrEmpty(input))
            {
                var compoundValidator = new Regex($"({string.Join(")|(", validationExpressions)})");
                if (!compoundValidator.IsMatch(input))
                {
                    throw new ArgumentException($"{input} is not a valid {type}");
                }
            }

            Input = input;
            Type = type;

        }



        [JsonProperty]
        private string _input;
        protected string Input
        {
            get => _input;
            set => _input = value;
        }

        public PointReferenceNumberType Type { get; }

        public bool HasValue => !string.IsNullOrEmpty(Input);

        public override string ToString()
        {
            return Input;
        }

        public static explicit operator string(PointReferenceNumber src)
        {
            return src?.Input;
        }


        public bool Equals(PointReferenceNumber other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Input == other.Input && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PointReferenceNumber)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Input != null ? Input.GetHashCode() : 0) * 397) ^ (int)Type;
            }
        }

        public static bool operator ==(PointReferenceNumber left, PointReferenceNumber right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PointReferenceNumber left, PointReferenceNumber right)
        {
            return !Equals(left, right);
        }
    }
}
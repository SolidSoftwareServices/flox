using System.Runtime.Serialization;

namespace EI.RP.DataModels.Switch
{
    public class AddressDetailDto
    {
        public string City { get; set; }
        public string Country { get; set; }
        [DataMember(Name = "Class")]
        public string DuosGroup { get; set; }
        public string HouseNo { get; set; }
        public string HouseNo2 { get; set; }
        public string PostalCode { get; set; }
        public string Street { get; set; }
        public string Street2 { get; set; }
        public string Street3 { get; set; }
        public string Street4 { get; set; }
        public string Street5 { get; set; }
        public string Region { get; set; }
        public string ShortForm { get; set; }
    }
}
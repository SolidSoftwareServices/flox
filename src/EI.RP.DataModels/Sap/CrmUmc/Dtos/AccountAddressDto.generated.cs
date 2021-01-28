using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Ports.OData;
using System.ComponentModel.DataAnnotations;

namespace EI.RP.DataModels.Sap.CrmUmc.Dtos
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    [CrudOperations(CanAdd = true, CanUpdate = true, CanDelete = true, CanQuery = true)]
    public partial class AccountAddressDto : ODataDtoModel
    {
        public override string CollectionName() => "AccountAddresses";
        public override object[] UniqueId()
        {
            return new object[]{AccountID, AddressID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        [Sortable]
        [Filterable]
        public virtual AddressInfoDto AddressInfo
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string AccountID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string AddressID
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual List<AccountAddressDependentFaxDto> AccountAddressDependentFaxes
        {
            get;
            set;
        }

        = new List<AccountAddressDependentFaxDto>();
        [Sortable]
        [Filterable]
        public virtual List<AccountAddressDependentMobilePhoneDto> AccountAddressDependentMobilePhones
        {
            get;
            set;
        }

        = new List<AccountAddressDependentMobilePhoneDto>();
        [Sortable]
        [Filterable]
        public virtual List<AccountAddressUsageDto> AccountAddressUsages
        {
            get;
            set;
        }

        = new List<AccountAddressUsageDto>();
        [Sortable]
        [Filterable]
        public virtual List<AccountAddressDependentPhoneDto> AccountAddressDependentPhones
        {
            get;
            set;
        }

        = new List<AccountAddressDependentPhoneDto>();
        [Sortable]
        [Filterable]
        public virtual List<AccountAddressDependentEmailDto> AccountAddressDependentEmails
        {
            get;
            set;
        }

        = new List<AccountAddressDependentEmailDto>();
    }
}
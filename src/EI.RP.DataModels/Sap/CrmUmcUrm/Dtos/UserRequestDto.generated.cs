using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Ports.OData;
using System.ComponentModel.DataAnnotations;

namespace EI.RP.DataModels.Sap.CrmUmcUrm.Dtos
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    [CrudOperations(CanAdd = true, CanUpdate = false, CanDelete = true, CanQuery = true)]
    public partial class UserRequestDto : ODataDtoModel
    {
        public override string CollectionName() => "UserRequestCollection";
        public override object[] UniqueId()
        {
            return new object[]{ID, };
        }

        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC_URM";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(32)]
        [Sortable]
        [Filterable]
        public virtual string ID
        {
            get;
            set;
        }

        = string.Empty;
        [StringLength(40)]
        [Sortable]
        [Filterable]
        public virtual string UserName
        {
            get;
            set;
        }

        [StringLength(60)]
        [Sortable]
        [Filterable]
        public virtual string FirstName
        {
            get;
            set;
        }

        [StringLength(60)]
        [Sortable]
        [Filterable]
        public virtual string LastName
        {
            get;
            set;
        }

        [StringLength(241)]
        [Sortable]
        [Filterable]
        public virtual string EmailAddress
        {
            get;
            set;
        }

        [StringLength(30)]
        [Sortable]
        [Filterable]
        public virtual string PhoneNumber
        {
            get;
            set;
        }

        [StringLength(3)]
        [Sortable]
        [Filterable]
        public virtual string UsrCategory
        {
            get;
            set;
        }

        [StringLength(40)]
        [Sortable]
        [Filterable]
        public virtual string Password
        {
            get;
            set;
        }

        [StringLength(2)]
        [Sortable]
        [Filterable]
        public virtual string StatusCode
        {
            get;
            set;
        }

        [StringLength(10)]
        [Sortable]
        [Filterable]
        public virtual string AccountID
        {
            get;
            set;
        }

        [StringLength(12)]
        [Sortable]
        [Filterable]
        public virtual string BusinessAgreementID
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual DateTime? Birthday
        {
            get => _Birthday;
            set => _Birthday = CompensateSapDateTimeBug(value);
        }

        private DateTime? _Birthday;
        [StringLength(3)]
        [Sortable]
        [Filterable]
        public virtual string CountryID
        {
            get;
            set;
        }

        [StringLength(3)]
        [Sortable]
        [Filterable]
        public virtual string RegionID
        {
            get;
            set;
        }

        [StringLength(40)]
        [Sortable]
        [Filterable]
        public virtual string City
        {
            get;
            set;
        }

        [StringLength(10)]
        [Sortable]
        [Filterable]
        public virtual string PostalCode
        {
            get;
            set;
        }

        [StringLength(60)]
        [Sortable]
        [Filterable]
        public virtual string Street
        {
            get;
            set;
        }

        [StringLength(10)]
        [Sortable]
        [Filterable]
        public virtual string HouseNo
        {
            get;
            set;
        }

        [StringLength(10)]
        [Sortable]
        [Filterable]
        public virtual string RoomNo
        {
            get;
            set;
        }

        [StringLength(50)]
        [Sortable]
        [Filterable]
        public virtual string PoD
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual bool? AccountOwnerFlag
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual bool? TermsConditionsFlag
        {
            get;
            set;
        }
    }
}
using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Ports.OData;
using System.ComponentModel.DataAnnotations;

namespace EI.RP.DataModels.Sap.ErpUmc.Dtos
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    [CrudOperations(CanAdd = false, CanUpdate = false, CanDelete = false, CanQuery = true)]
    public partial class CommunicationPreferenceDto : ODataDtoModel
    {
        public override string CollectionName() => "CommunicationPreferences";
        public override object[] UniqueId()
        {
            return new object[]{ContractAccountID, CommunicationCategoryID, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(12)]
        public virtual string ContractAccountID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string CommunicationCategoryID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string CommunicationMethodID
        {
            get;
            set;
        }

        = string.Empty;
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
        public virtual List<CommunicationPreferenceDto> DependentContractAccountCommPreferences
        {
            get;
            set;
        }

        = new List<CommunicationPreferenceDto>();
        [Sortable]
        [Filterable]
        public virtual AccountAddressDto AccountAddress
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<CommunicationPreferenceDto> DependentAccountCommPreferences
        {
            get;
            set;
        }

        = new List<CommunicationPreferenceDto>();
        [Sortable]
        [Filterable]
        public virtual CommunicationCategoryDto CommunicationCategory
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual CommunicationMethodDto CommunicationMethod
        {
            get;
            set;
        }
    }
}
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
    public partial class AccountRelationshipDto : ODataDtoModel
    {
        public override string CollectionName() => "AccountRelationships";
        public override object[] UniqueId()
        {
            return new object[]{AccountID, RelationshipID, RelatedAccountID, AccountRelationshipTypeID, DifferentiationValue, ValidFrom, ValidTo, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
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
        [StringLength(12)]
        public virtual string RelationshipID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(10)]
        public virtual string RelatedAccountID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(6)]
        public virtual string AccountRelationshipTypeID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(20)]
        public virtual string DifferentiationValue
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual DateTime ValidFrom
        {
            get => _ValidFrom;
            set => _ValidFrom = CompensateSapDateTimeBug(value);
        }

        private DateTime _ValidFrom;
        [Required(AllowEmptyStrings = true)]
        public virtual DateTime ValidTo
        {
            get => _ValidTo;
            set => _ValidTo = CompensateSapDateTimeBug(value);
        }

        private DateTime _ValidTo;
        [Required(AllowEmptyStrings = true)]
        [StringLength(80)]
        public virtual string RelatedAccountDescription
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual AccountRelationshipTypeDto AccountRelationshipType
        {
            get;
            set;
        }
    }
}
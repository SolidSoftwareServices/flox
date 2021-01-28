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
    public partial class ParkedDocumentDto : ODataDtoModel
    {
        public override string CollectionName() => "ParkedDocuments";
        public override object[] UniqueId()
        {
            return new object[]{ParkedDocumentID, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Sortable]
        [Filterable]
        public virtual MasterDataKeyDto MoveIn
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual MasterDataKeyDto MoveOut
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(12)]
        public virtual string ParkedDocumentID
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
        [Sortable]
        [Filterable]
        public virtual List<ParkedDocumentInstallationDto> MoveOutInstallations
        {
            get;
            set;
        }

        = new List<ParkedDocumentInstallationDto>();
        [Sortable]
        [Filterable]
        public virtual List<ParkedDocumentInstallationDto> MoveInInstallations
        {
            get;
            set;
        }

        = new List<ParkedDocumentInstallationDto>();
    }
}
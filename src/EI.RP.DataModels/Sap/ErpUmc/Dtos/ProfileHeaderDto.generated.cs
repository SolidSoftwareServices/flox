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
    public partial class ProfileHeaderDto : ODataDtoModel
    {
        public override string CollectionName() => "ProfileHeaders";
        public override object[] UniqueId()
        {
            return new object[]{ProfileHeaderID, };
        }

        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(18)]
        public virtual string ProfileHeaderID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(18)]
        public virtual string DeviceID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string DivisionID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        public virtual string RegisterID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(4)]
        public virtual string Interval
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(2)]
        public virtual string ProfileTypeID
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(3)]
        public virtual string UnitOfMeasure
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        [StringLength(5)]
        public virtual string Currency
        {
            get;
            set;
        }

        = string.Empty;
        [Required(AllowEmptyStrings = true)]
        public virtual DateTime Datefrom
        {
            get => _Datefrom;
            set => _Datefrom = CompensateSapDateTimeBug(value);
        }

        private DateTime _Datefrom;
        [Required(AllowEmptyStrings = true)]
        public virtual TimeSpan Timefrom
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        public virtual DateTime Dateto
        {
            get => _Dateto;
            set => _Dateto = CompensateSapDateTimeBug(value);
        }

        private DateTime _Dateto;
        [Required(AllowEmptyStrings = true)]
        public virtual TimeSpan Timeto
        {
            get;
            set;
        }

        [Required(AllowEmptyStrings = true)]
        [StringLength(6)]
        public virtual string TimeZone
        {
            get;
            set;
        }

        = string.Empty;
        [Sortable]
        [Filterable]
        public virtual ProfileTypeDto ProfileType
        {
            get;
            set;
        }

        [Sortable]
        [Filterable]
        public virtual List<ProfileValueDto> ProfileValues
        {
            get;
            set;
        }

        = new List<ProfileValueDto>();
    }
}
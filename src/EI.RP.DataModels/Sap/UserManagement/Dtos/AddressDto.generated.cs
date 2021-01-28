using System;
using System.Collections.Generic;
using EI.RP.CoreServices.Ports.OData;
using System.ComponentModel.DataAnnotations;

namespace EI.RP.DataModels.Sap.UserManagement.Dtos
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    [CrudOperations(CanAdd = false, CanUpdate = false, CanDelete = false, CanQuery = true)]
    public partial class AddressDto : ODataDtoModel
    {
        public override object[] UniqueId()
        {
            return new object[]{};
        }

        public override string GetEntityContainerName()
        {
            return "USERMANAGEMENT";
        }

        [StringLength(40)]
        [Sortable]
        [Filterable]
        public virtual string Department
        {
            get;
            set;
        }

        [StringLength(40)]
        [Sortable]
        [Filterable]
        public virtual string Function
        {
            get;
            set;
        }

        [StringLength(10)]
        [Sortable]
        [Filterable]
        public virtual string Building
        {
            get;
            set;
        }

        [StringLength(10)]
        [Sortable]
        [Filterable]
        public virtual string Floor
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

        [StringLength(10)]
        [Sortable]
        [Filterable]
        public virtual string ShortName
        {
            get;
            set;
        }

        [StringLength(40)]
        [Sortable]
        [Filterable]
        public virtual string CareOfName
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

        [StringLength(40)]
        [Sortable]
        [Filterable]
        public virtual string District
        {
            get;
            set;
        }

        [StringLength(12)]
        [Sortable]
        [Filterable]
        public virtual string CityCode
        {
            get;
            set;
        }

        [StringLength(8)]
        [Sortable]
        [Filterable]
        public virtual string DistrictCode
        {
            get;
            set;
        }

        [StringLength(10)]
        [Sortable]
        [Filterable]
        public virtual string POBoxPostalCode
        {
            get;
            set;
        }

        [StringLength(10)]
        [Sortable]
        [Filterable]
        public virtual string POBox
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

        [StringLength(12)]
        [Sortable]
        [Filterable]
        public virtual string StreetNo
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

        [StringLength(2)]
        [Sortable]
        [Filterable]
        public virtual string Country
        {
            get;
            set;
        }

        [StringLength(3)]
        [Sortable]
        [Filterable]
        public virtual string Region
        {
            get;
            set;
        }
    }
}
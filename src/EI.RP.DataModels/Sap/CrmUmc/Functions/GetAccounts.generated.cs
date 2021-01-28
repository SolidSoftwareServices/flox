using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel.DataAnnotations;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.DataModels.Sap.CrmUmc.Dtos;

namespace EI.RP.DataModels.Sap.CrmUmc.Functions
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    public partial class GetAccountsFunction : ODataFunction<GetAccountsFunction.QueryObject, AccountDto>
    {
        public GetAccountsFunction(params Expression<Func<AccountDto, object>>[] expands): base("GetAccounts", expands)
        {
        }

        public override bool ReturnsComplexType() => false;
        public override bool ReturnsCollection() => true;
        public override string GetEntityContainerName()
        {
            return "CRM_UTILITIES_UMC";
        }

        public class QueryObject
        {
            [StringLength(3)]
            public virtual string Country
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(40)]
            public virtual string HouseNo
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(40)]
            public virtual string UserName
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(40)]
            public virtual string City
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(40)]
            public virtual string FirstName
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(10)]
            public virtual string AccountID
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(30)]
            public virtual string Phone
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(12)]
            public virtual string BusinessAgreementID
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(10)]
            public virtual string PostalCode
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(40)]
            public virtual string LastName
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(3)]
            public virtual string Region
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(241)]
            public virtual string Email
            {
                get;
                set;
            }

            = string.Empty;
            [StringLength(60)]
            public virtual string Street
            {
                get;
                set;
            }

            = string.Empty;
        }
    }
}
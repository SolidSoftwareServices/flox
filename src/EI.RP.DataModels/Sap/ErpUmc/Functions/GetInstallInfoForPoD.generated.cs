using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel.DataAnnotations;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.DataModels.Sap.ErpUmc.Dtos;

namespace EI.RP.DataModels.Sap.ErpUmc.Functions
{
    /******************************
THIS CLASS WAS GENERATED, DO NOT EDIT MANUALLY. 

For customizations, create and edit a partial file instead
******************************/
    public partial class GetInstallInfoForPoDFunction : ODataFunction<GetInstallInfoForPoDFunction.QueryObject, IsuInstallationInfoDto>
    {
        public GetInstallInfoForPoDFunction(params Expression<Func<IsuInstallationInfoDto, object>>[] expands): base("GetInstallInfoForPoD", expands)
        {
        }

        public override bool ReturnsComplexType() => true;
        public override bool ReturnsCollection() => false;
        public override string GetEntityContainerName()
        {
            return "ERP_UTILITIES_UMC";
        }

        public class QueryObject
        {
            public virtual string POD_INT_UI
            {
                get;
                set;
            }

            = string.Empty;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.WF.Contracts.Json.Converters.DataObjects;
using MCS.Web.Library.Script;
using MCS.Web.Library.Script.Configuration;

namespace MCS.Library.WF.Contracts.Json.Converters
{
    public class WfClientJsonConverterHelper : IJsonConverterRegister
    {
        public static readonly WfClientJsonConverterHelper Instance = new WfClientJsonConverterHelper();

        /// <summary>
        /// 注册相关的序列化器
        /// </summary>
        public void RegisterConverters()
        {
            JSONSerializerExecute.RegisterConverter(typeof(ClientPropertyValueJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientOrganizationJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientGroupJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientUserJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientActivityDescriptorJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientTransitionDescriptorJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientBranchProcessTemplateDescriptorJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientRelativeLinkDescriptorJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientProcessDescriptorJsonConverter));

            JSONSerializerExecute.RegisterConverter(typeof(WfClientUserResourceDescriptorJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientDepartmentResourceDescriptorJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientGroupResourceDescriptorJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientRoleResourceDescriptorJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientDynamicResourceDescriptorJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientActivityAssigneesResourceDescriptorJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientActivityOperatorResourceDescriptorJsonConverter));

            JSONSerializerExecute.RegisterConverter(typeof(WfClientProcessStartupParamsJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientBranchProcessStartupParamsJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WClientBranchProcessTransferParamsJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientTransferParamsJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientProcessJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientProcessInfoJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientAuthorizationInfoJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientActivityJsonConverter));

            JSONSerializerExecute.RegisterConverter(typeof(WfClientRuntimeContextJsonConverter));

            JSONSerializerExecute.RegisterConverter(typeof(WfClientRolePropertyRowJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientRolePropertyDefinitionJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientRolePropertyValueJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientActivityMatrixResourceDescriptorJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientApprovalMatrixConverterJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfCreateClientDynamicProcessParamsJsonConverter));

            JSONSerializerExecute.RegisterConverter(typeof(WfClientNextStepJsonConverter));

            JSONSerializerExecute.RegisterConverter(typeof(WfClientOpinionJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientDelegationJsonConverter));

            JSONSerializerExecute.RegisterConverter(typeof(WfClientStreamJsonConverter));

            JSONSerializerExecute.RegisterConverter(typeof(WfClientMainStreamActivityDescriptorJsonConverter));

            JSONSerializerExecute.RegisterConverter(typeof(WfClientProcessCurrentInfoJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientProcessCurrentInfoPageQueryResultJsonConverter));

            JSONSerializerExecute.RegisterConverter(typeof(WfClientProcessDescriptorInfoJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientProcessDescriptorInfoPageQueryResultJsonConverter));

            JSONSerializerExecute.RegisterConverter(typeof(WfClientExportProcessDescriptorParamsJsonConverter));

            JSONSerializerExecute.RegisterConverter(typeof(WfClientUserOperationLogJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientUserOperationLogPageQueryResultJsonConverter));

            JSONSerializerExecute.RegisterConverter(typeof(WfClientApplicationJsonConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfClientProgramJsonConverter));

            JSONSerializerExecute.RegisterConverter(typeof(WfClientProcessQueryConditionJsonConverter));
        }
    }
}

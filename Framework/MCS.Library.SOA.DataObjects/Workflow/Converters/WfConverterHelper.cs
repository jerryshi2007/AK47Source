using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// JSON Converter的帮助类
    /// </summary>
    public static class WfConverterHelper
    {
        /// <summary>
        /// 注册相关的序列化器
        /// </summary>
        public static void RegisterConverters()
        {
            JSONSerializerExecute.RegisterConverter(typeof(WfParameterDescriptorConverter));
            JSONSerializerExecute.RegisterConverter(typeof(PropertyValueConverter));
            JSONSerializerExecute.RegisterConverter(typeof(OguRoleConverter));
            JSONSerializerExecute.RegisterConverter(typeof(OguObjectConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfActivityDescriptorConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfProcessDescriptorConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfForwardTransitionDescriptorConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfVariableDescriptorConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfUserResourceDescriptorConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfConditionDescriptorConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfDepartmentResourceDescriptorConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfGroupResourceDescriptorConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfDynamicResourceDescriptorConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfAURoleResourceDescriptorConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WrappedAUSchemaRoleConverter));

            JSONSerializerExecute.RegisterConverter(typeof(WfRoleResourceDescriptorConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfBranchProcessTemplateDescriptorConverter));

            JSONSerializerExecute.RegisterConverter(typeof(WfActivityAssigneesResourceDescriptorConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfActivityOperatorResourceDescriptorConverter));

            JSONSerializerExecute.RegisterConverter(typeof(WfRelativeLinkDescriptorConverter));
            JSONSerializerExecute.RegisterConverter(typeof(UserSettingsConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfExternalUserConverter));

            JSONSerializerExecute.RegisterConverter(typeof(WfControlNextStepConverter));
            JSONSerializerExecute.RegisterConverter(typeof(UserSettingsCategoryConverter));

            JSONSerializerExecute.RegisterConverter(typeof(WfAssigneeConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfActivityDescriptorCreateParamsConverter));

            JSONSerializerExecute.RegisterConverter(typeof(WfNetworkCredentialConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfServiceAddressDefinitionConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfServiceOperationDefinitionConverter));

            JSONSerializerExecute.RegisterConverter(typeof(WfBranchProcessStartupParamsConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfBranchProcessTransferParamsConverter));

            JSONSerializerExecute.RegisterConverter(typeof(SOARolePropertyValueConverter));
            JSONSerializerExecute.RegisterConverter(typeof(SOARolePropertyRowConverter));
            JSONSerializerExecute.RegisterConverter(typeof(SOARolePropertyDefinitionConverter));
            JSONSerializerExecute.RegisterConverter(typeof(WfActivityMatrixResourceDescriptorConverter));
        }

        /// <summary>
        /// 得到空流程定义的JSON串
        /// </summary>
        /// <returns></returns>
        public static string GetEmptyProcessDescriptorJsonString()
        {
            RegisterConverters();

            WfProcessDescriptor processDesp = new WfProcessDescriptor();

            return JSONSerializerExecute.Serialize(processDesp);
        }

        /// <summary>
        /// 生成空开始节点定义的JSON串
        /// </summary>
        /// <returns></returns>
        public static string GetEmptyInitialActivityDescriptorJsonString()
        {
            RegisterConverters();

            WfActivityDescriptor activityDesp = new WfActivityDescriptor(string.Empty, WfActivityType.InitialActivity);

            activityDesp.ActivityType = WfActivityType.InitialActivity;
            activityDesp.Name = "申请";

            return JSONSerializerExecute.Serialize(activityDesp);
        }

        /// <summary>
        /// 生成空办结节点定义的JSON串
        /// </summary>
        /// <returns></returns>
        public static string GetEmptyCompletedActivityDescriptorJsonString()
        {
            RegisterConverters();

            WfActivityDescriptor activityDesp = new WfActivityDescriptor(string.Empty, WfActivityType.CompletedActivity);
            activityDesp.Name = "办结";

            return JSONSerializerExecute.Serialize(activityDesp);
        }

        /// <summary>
        /// 生成空正常节点定义的JSON串
        /// </summary>
        /// <returns></returns>
        public static string GetEmptyNormalActivityDescriptorJsonString()
        {
            RegisterConverters();

            WfActivityDescriptor activityDesp = new WfActivityDescriptor(string.Empty);

            return JSONSerializerExecute.Serialize(activityDesp);
        }

        /// <summary>
        /// 生成空前向线定义的JSON串
        /// </summary>
        /// <returns></returns>
        public static string GetEmptyForwardTransitionDescriptorJsonString()
        {
            RegisterConverters();

            WfForwardTransitionDescriptor transition = new WfForwardTransitionDescriptor();

            return JSONSerializerExecute.Serialize(transition);
        }
    }
}

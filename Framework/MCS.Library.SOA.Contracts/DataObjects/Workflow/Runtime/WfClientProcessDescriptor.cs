using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MCS.Library.SOA.Contracts.DataObjects.Workflow.Runtime
{
    [DataContract]
    [Serializable]
    public class WfClientProcessDescriptor
    {
        [DataMember]
        string ApplicationName
        {
            get;
            set;
        }

        /// <summary>
        /// 应用的模块名称
        /// </summary>
        [DataMember]
        string ProgramName
        {
            get;
            set;
        }

        /// <summary>
        /// 流程启动或表单缺省的Url
        /// </summary>
        [DataMember]
        string Url
        {
            get;
            set;
        }

        /// <summary>
        /// 和流程图相关的描述信息，例如坐标等
        /// </summary>
        [DataMember]
        string GraphDescription
        {
            set;
            get;
        }

        /// <summary>
        /// 版本信息
        /// </summary>
        [DataMember]
        string Version
        {
            get;
            set;
        }

        /// <summary>
        /// 流程的类型
        /// </summary>
        [DataMember]
        WfClientProcessType ProcessType
        {
            set;
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        WfClientVariableDescriptorCollection Variables
        {
            set;
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        WfClientActivityDescriptorCollection Activities
        {
            set;
            get;
        }

        /// <summary>
        /// 相关链接
        /// </summary>
        [DataMember]
        WfClientRelativeLinkDescriptorCollection RelativeLinks
        {
            set;
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        WfClientActivityDescriptor InitialActivity
        {
            set;
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        WfClientActivityDescriptor CompletedActivity
        {
            set;
            get;
        }

        /// <summary>
        /// 内部相关人员
        /// </summary>
        [DataMember]
        WfClientResourceDescriptorCollection InternalRelativeUsers
        {
            set;
            get;
        }

        /// <summary>
        /// 外部人员
        /// </summary>
        [DataMember]
        WfClientExternalUserCollection ExternalUsers
        {
            set;
            get;
        }

        /// <summary>
        /// 流程启动时是否自动生成资源中的人员
        /// </summary>
        [DataMember]
        bool AutoGenerateResourceUsers
        {
            set;
            get;
        }

        /// <summary>
        /// 自动计算出一个没有用过的ActivityKey
        /// </summary>
        /// <returns></returns>
 
        //string FindNotUsedActivityKey();

        /// <summary>
        /// 自动计算出一个没有用过的TransitionKey
        /// </summary>
        /// <returns></returns>
  
        //string FindNotUsedTransitionKey();

        /// <summary>
        /// 根据Key查找Transition，如果没有找到，返回null;
        /// </summary>
        /// <param name="transitionKey"></param>
        /// <returns></returns>
        
        //WfClientTransitionDescriptor FindTransitionByKey(string transitionKey);

        /// <summary>
        /// 取消流程时需要通知的人
        /// </summary>
       
        //WfClientResourceDescriptorCollection CancelEventReceivers
        //{
        //    set;
        //    get;
        //}

        /// <summary>
        /// 得到主流活动点
        /// </summary>
        /// <returns></returns>
        //WfMainStreamActivityDescriptorCollection GetMainStreamActivities();
    }
}

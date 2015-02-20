using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.WF.Contracts.Ogu;
using MCS.Library.WF.Contracts.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Workflow.Descriptors
{

    /// <summary>
    /// 流程定义列表
    /// </summary>
    [Serializable]
    [DataContract]
    public class WfClientProcessDescriptorInfo
    {
        /// <summary>
        /// 大类（应用名称）
        /// </summary>
        [ConditionMapping("APPLICATION_NAME")]
        public string ApplicationName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ConditionMapping("CREATE_TIME")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [ConditionMapping("CREATOR_NAME")]
        public WfClientUser Creator { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        [NoMapping]
        public string Data { get; set; }

        /// <summary>
        /// 启用
        /// </summary>
        [ConditionMapping("ENABLED")]
        public bool Enabled { get; set; }

        /// <summary>
        /// 导入时间
        /// </summary>
        [ConditionMapping("IMPORT_TIME")]
        public DateTime ImportTime { get; set; }

        /// <summary>
        /// 导入人
        /// </summary>
        [ConditionMapping("IMPORT_USER_NAME")]
        public WfClientUser ImportUser { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        [ConditionMapping("MODIFIER_NAME")]
        public WfClientUser Modifier { get; set; }


        /// <summary>
        /// 修改日期
        /// </summary>
        [ConditionMapping("MODIFY_TIME")]
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 流程定义编码
        /// </summary>
        [ConditionMapping("PROCESS_KEY")]
        public string ProcessKey { get; set; }

        /// <summary>
        /// 流程定义名称
        /// </summary>
        [ConditionMapping("PROCESS_NAME")]
        public string ProcessName { get; set; }

        /// <summary>
        /// 小类（模块名称）
        /// </summary>
        [ConditionMapping("PROGRAM_NAME")]
        public string ProgramName { get; set; }
    }

    /// <summary>
    /// 流程定义集合
    /// </summary>
    [DataContract]
    [Serializable]
    public class WfClientProcessDescriptorInfoCollection : SerializableEditableKeyedDataObjectCollectionBase<string, WfClientProcessDescriptorInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        public WfClientProcessDescriptorInfoCollection()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected WfClientProcessDescriptorInfoCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override string GetKeyForItem(WfClientProcessDescriptorInfo item)
        {
            return item.ProcessKey;
        }
    }


    /// <summary>
    /// 流程定义分页集合
    /// </summary>
    [Serializable]
    [DataContract]
    public class WfClientProcessDescriptorInfoPageQueryResult : ClientPageQueryResultBase<WfClientProcessDescriptorInfo, WfClientProcessDescriptorInfoCollection>
    {
    }
}

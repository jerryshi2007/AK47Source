using CIIC.HSR.TSP.WF.Bizlet.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    /// <summary>
    /// 节点描述
    /// </summary>
    [Serializable]
    [DataContract]
    public class WfMatrixActivity : IWfMatrixActivity
    {
        private IWfMatrixConditionGroupCollection _WfMatrixConditionCollection =
            new WfMatrixConditionGroupCollection();

        private IWfMatrixCandidateCollection _IWfMatrixCandidateCollection =
            new WfMatrixCandidateCollection();

        private IDictionary<string, object> _Properties = new Dictionary<string, object>();

        /// <summary>
        /// 构造方法
        /// </summary>
        public WfMatrixActivity()
        {
            this.ActivityType = WfMaxtrixActivityType.NormalActivity;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="activityType"></param>
        public WfMatrixActivity(WfMaxtrixActivityType activityType)
        {
            this.ActivityType = activityType;
        }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 编码
        /// </summary>
        public string CodeName
        {
            get;
            set;
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 节点的类型
        /// </summary>
        public WfMaxtrixActivityType ActivityType
        {
            get;
            set;
        }

        /// <summary>
        /// 条件设置列表
        /// </summary>
        public IWfMatrixConditionGroupCollection Expression
        {
            get
            {
                return this._WfMatrixConditionCollection;
            }
            set
            {
                this._WfMatrixConditionCollection = value;
            }
        }

        /// <summary>
        /// 节点相关的资源
        /// </summary>
        public IWfMatrixCandidateCollection Candidates
        {
            get
            {
                return this._IWfMatrixCandidateCollection;
            }
            set
            {
                this._IWfMatrixCandidateCollection = value;
            }
        }

        /// <summary>
        /// 更多属性设置
        /// </summary>
        public IDictionary<string, object> Properties
        {
            get
            {
                return this._Properties;
            }
            set
            {
                this._Properties = value;
            }
        }

        /// <summary>
        /// 表单地址
        /// </summary>
        public string Url
        {
            set;
            get;
        }
    }
}

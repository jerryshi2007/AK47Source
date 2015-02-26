using CIIC.HSR.TSP.DataAccess.Query;
using CIIC.HSR.TSP.WF.Bizlet.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class WfMatrixProcess : IWfMatrixProcess
    {
        private IWfMatrixParameterDefinitionCollection _WfMatrixParameterDefinitionCollection
            = new WfMatrixParameterDefinitionCollection();
        private IWfMatrixActivityCollection _IWfMatrixActivityCollection = new WfMatrixActivityCollection();
        private IWfMatrixParameterDefinitionCollection _WfMatrixGlobalParameterDefinitionCollection
           = new WfMatrixParameterDefinitionCollection();

        public WfMatrixProcess()
        {
           
        }
             

        private IDictionary<string, object> _Properties = new Dictionary<string, object>();

        /// <summary>
        /// 租户编码
        /// </summary>
        public string TenantCode
        {
            get;
            set;
        }

        /// <summary>
        /// 流程名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 流程key
        /// </summary>
        public string Key
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
        /// 大分类
        /// </summary>
        public string ApplicationName
        {
            get;
            set;
        }
        /// <summary>
        /// 小分类
        /// </summary>
        public string ProgramName
        {
            get;
            set;
        }


        /// <summary>
        /// 表单地址
        /// </summary>
        public string Url
        {
            set;
            get;
        }

        /// <summary>
        /// 更多属性设置
        /// </summary>
        public IDictionary<string, object> Properties
        {
            get { return _Properties; }
            set { _Properties = value; }
        }

        /// <summary>
        /// 额外的参数定义
        /// </summary>
        public IWfMatrixParameterDefinitionCollection ParameterDefinitions
        {
            get
            {
                return _WfMatrixParameterDefinitionCollection;
            }
            set
            {
                _WfMatrixParameterDefinitionCollection = value;
            }
        }

        /// <summary>
        /// 节点列表
        /// </summary>
        public IWfMatrixActivityCollection Activities
        {
            get
            {
                return _IWfMatrixActivityCollection;
            }
            set
            {
                _IWfMatrixActivityCollection = value;
            }
        }

        public void InitGlobalParameter()
        {
            IWfMatrixParameterDefinitionCollection parameters = _WfMatrixGlobalParameterDefinitionCollection;
            parameters.Clear();
            foreach (WfMatrixGlobalParameterElement parameter in WfMatrixGlobalParameterSetting.GetInfo().Parameters)
            {
                if (string.IsNullOrEmpty(parameter.Key))
                {
                    continue;
                }
                WfMatrixParameterDefinition param = new WfMatrixParameterDefinition()
                {
                    Name = parameter.Key
                    ,
                    DisplayName = parameter.Name
                    ,
                    DefaultValue = parameter.DefaultValue
                    ,
                    Description = parameter.Description
                    ,
                    Enabled = parameter.Enable
                    ,
                    ParameterType = parameter.ValueType

                };
                parameters.Add(param);
            }
        }

        public IWfMatrixParameterDefinitionCollection GlobalParameterDefinitions
        {
            get
            { 
                return _WfMatrixGlobalParameterDefinitionCollection;
            }
        }

 
    }
}

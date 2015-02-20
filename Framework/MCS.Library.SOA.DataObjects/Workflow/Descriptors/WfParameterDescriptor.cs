using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 流程自动收集参数实体
    /// </summary>
    [Serializable]
    [XElementSerializable]
    public class WfParameterDescriptor
    {
        private string _ControlID = string.Empty;
     
		/// <summary>
        /// 控件ID
        /// </summary>
        public String ControlID
        {
            get
            {
                return this._ControlID;
            }
            set
            {
                this._ControlID = value;
            }
        }

        private string _ParameterName = string.Empty;
        
		/// <summary>
        /// 参数名称
        /// </summary>
        public string ParameterName
        {
            get
            {
                return this._ParameterName;
            }
            set
            {
                this._ParameterName = value;
            }
        }

        private PropertyDataType _ParameterType = PropertyDataType.String;
        
		/// 数据类型
        public PropertyDataType ParameterType
        {
            get
            {
                return this._ParameterType;
            }
            set
            {
                this._ParameterType = value;
            }
        }

        private string _ControlPropertyName = string.Empty;
        
		/// 绑定控件的属性值
        public String ControlPropertyName
        {
            get
            {
                return this._ControlPropertyName;
            }
            set
            {
                this._ControlPropertyName = value;
            }
        }

        private string _Key = string.Empty;

        /* 
         ///// <summary>
         ///// 编辑Key
         ///// </summary>
         public string Key
         {
             get
             {
                 return this._Key;
             }
             set
             {
                 this._Key = value;
             }
         }

         private ProcessParameterEvalMode _ProcessParameterEvalMode = ProcessParameterEvalMode.CurrentProcess;
         public ProcessParameterEvalMode ProcessParameterEvalMode
         {
             get
             {
                 return this._ProcessParameterEvalMode;
             }
             set
             {
                 this._ProcessParameterEvalMode = value;
             }
         } */

        private bool _AutoCollect = true;

		/// <summary>
        /// 是否自动收集此参数
        /// </summary>
        public bool AutoCollect
        {
            get
            {
                return this._AutoCollect;
            }
            set
            {
                this._AutoCollect = true;
            }
        }
    }
}

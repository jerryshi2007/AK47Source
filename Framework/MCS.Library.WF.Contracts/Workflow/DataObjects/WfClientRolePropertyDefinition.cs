using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MCS.Library.WF.Contracts.Workflow.DataObjects
{
    [DataContract]
    [Serializable]
    public class WfClientRolePropertyDefinition : ColumnDefinitionBase
    {
        [ScriptIgnore]
        public override Type RealDataType
        {
            get
            {
                return base.RealDataType;
            }
        }

        public override string Name
        {
            get;
            set;
        }

        public int SortOrder
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 数据类型
        /// </summary>
        public override ColumnDataType DataType
        {
            get
            {
                return base.DataType;
            }
            set
            {
                base.DataType = value;
            }
        }
    }

    /// <summary>
    /// 角色的扩展属性定义集合
    /// </summary>
    [Serializable]
    public class WfClientRolePropertyDefinitionCollection : ColumnDefinitionCollectionBase<WfClientRolePropertyDefinition>
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public WfClientRolePropertyDefinitionCollection()
        {
        }

        /// <summary>
        /// 从DataTable的Columns构造
        /// </summary>
        /// <param name="columns"></param>
        public WfClientRolePropertyDefinitionCollection(DataColumnCollection columns)
        {
            this.FromDataColumns(columns);
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected WfClientRolePropertyDefinitionCollection(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// 从DataTable的Columns构造
        /// </summary>
        /// <param name="columns"></param>
        public void FromDataColumns(DataColumnCollection columns)
        {
            columns.NullCheck("columns");

            this.Clear();

            int columnIndex = 0;

            foreach (DataColumn column in columns)
                this.Add(new WfClientRolePropertyDefinition() { Name = column.ColumnName, SortOrder = columnIndex++ });
        }

        /// <summary>
        /// 矩阵的类型
        /// </summary>
        public WfClientMatrixType MatrixType
        {
            get
            {
                WfClientMatrixType result = WfClientMatrixType.ApprovalMatrix;

                if (this.ContainsKey("OperatorType") || this.ContainsKey("Operator"))
                {
                    result = WfClientMatrixType.RoleMatrix;

                    if (this.ContainsKey("ActivitySN") || this.ContainsKey("ActivityName"))
                        result = WfClientMatrixType.ActivityMatrix;
                }

                return result;
            }
        }
    }
}

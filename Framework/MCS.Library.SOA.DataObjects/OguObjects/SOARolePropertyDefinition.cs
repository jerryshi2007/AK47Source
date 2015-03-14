using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Workflow;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Script.Serialization;

namespace MCS.Library.SOA.DataObjects
{
    /// <summary>
    /// 角色的属性定义项
    /// </summary>
    [Serializable]
    [ORTableMapping("WF.ROLE_PROPERTIES_DEFINITIONS")]
    [TenantRelativeObject]
    public class SOARolePropertyDefinition : ColumnDefinitionBase
    {
        public const string OperatorTypeColumn = "OperatorType";
        public const string OperatorColumn = "Operator";
        public const string ActivitySNColumn = "ActivitySN";
        public const string ActivityNameColumn = "ActivityName";
        public const string ActivityCodeColumn = "ActivityCode";
        public const string IsMergeableColumn = "IsMergeable";
        public const string AutoExtractColumn = "AutoExtract";
        public const string ConditionColumn = "Condition";
        public const string TransitionsColumn = "Transitions";

        private static readonly string[] ReservedPropertyName = new string[] {
            SOARolePropertyDefinition.OperatorTypeColumn, 
            SOARolePropertyDefinition.OperatorColumn,
            SOARolePropertyDefinition.ActivitySNColumn,
            SOARolePropertyDefinition.ActivityNameColumn,
            SOARolePropertyDefinition.ActivityCodeColumn,
            SOARolePropertyDefinition.AutoExtractColumn,
            SOARolePropertyDefinition.IsMergeableColumn,
            SOARolePropertyDefinition.ConditionColumn,
            SOARolePropertyDefinition.TransitionsColumn
        };

        public static readonly SOARolePropertyDefinitionCollection EmptyInstance = new SOARolePropertyDefinitionCollection();

        public SOARolePropertyDefinition()
        {
        }

        public SOARolePropertyDefinition(IRole role)
        {
            role.NullCheck("role");

            RoleID = role.ID;
        }

        /// <summary>
        /// 属性名是不是系统默认的保留字
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool IsReservedPropertyName(string propertyName)
        {
            propertyName.NullCheck("propertyName");

            bool result = false;

            foreach (string rpName in ReservedPropertyName)
            {
                result = string.Compare(rpName, propertyName, true) == 0;

                if (result)
                    break;
            }

            if (result == false)
                result = propertyName.IndexOf("Activity", StringComparison.OrdinalIgnoreCase) == 0;

            return result;
        }

        [ScriptIgnore]
        public override Type RealDataType
        {
            get
            {
                return base.RealDataType;
            }
        }

        [ORFieldMapping("ROLE_ID", PrimaryKey = true)]
        public string RoleID
        {
            get;
            set;
        }

        [ORFieldMapping("NAME", PrimaryKey = true)]
        public override string Name
        {
            get;
            set;
        }

        [ORFieldMapping("SORT_ORDER")]
        public int SortOrder
        {
            get;
            set;
        }

        [ORFieldMapping("DESCRIPTION")]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 数据类型
        /// </summary>
        [ORFieldMapping("DATA_TYPE")]
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

        /// <summary>
        /// 字段的默认值
        /// </summary>
        [ORFieldMapping("DEFAULT_VALUE")]
        public override string DefaultValue
        {
            get
            {
                return base.DefaultValue;
            }
            set
            {
                base.DefaultValue = value;
            }
        }
    }

    /// <summary>
    /// 角色的扩展属性定义集合
    /// </summary>
    [Serializable]
    public class SOARolePropertyDefinitionCollection : ColumnDefinitionCollectionBase<SOARolePropertyDefinition>
    {
        /// <summary>
        /// 
        /// </summary>
        public SOARolePropertyDefinitionCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// 从DataTable的Columns构造
        /// </summary>
        /// <param name="columns"></param>
        public SOARolePropertyDefinitionCollection(DataColumnCollection columns)
            : base(StringComparer.OrdinalIgnoreCase)
        {
            this.FromDataColumns(columns);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected SOARolePropertyDefinitionCollection(SerializationInfo info, StreamingContext context)
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
                this.Add(new SOARolePropertyDefinition() { Name = column.ColumnName, SortOrder = columnIndex++ });
        }

        /// <summary>
        /// 矩阵的类型
        /// </summary>
        public WfMatrixType MatrixType
        {
            get
            {
                WfMatrixType result = WfMatrixType.ApprovalMatrix;

                if (this.ContainsKey(SOARolePropertyDefinition.OperatorTypeColumn) || this.ContainsKey(SOARolePropertyDefinition.OperatorColumn))
                {
                    result = WfMatrixType.RoleMatrix;

                    if (this.ContainsKey(SOARolePropertyDefinition.ActivitySNColumn) || this.ContainsKey(SOARolePropertyDefinition.ActivityNameColumn))
                        result = WfMatrixType.ActivityMatrix;
                }

                return result;
            }
        }
    }
}

using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
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
    public class SOARolePropertyDefinition : ColumnDefinitionBase
    {
        private static readonly string[] ReservedPropertyName = new string[] { "OperatorType", "Operator", "ActivitySN", "ActivityName", "IsMergeable" };

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
        {
        }

        /// <summary>
        /// 从DataTable的Columns构造
        /// </summary>
        /// <param name="columns"></param>
        public SOARolePropertyDefinitionCollection(DataColumnCollection columns)
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
        /// 是否是活动矩阵
        /// </summary>
        public bool IsActivityMatrix
        {
            get
            {
                return this.ContainsKey("ActivitySN") ||
                        this.ContainsKey("ActivityName");
            }
        }
    }
}

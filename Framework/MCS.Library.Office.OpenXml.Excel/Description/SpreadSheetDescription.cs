using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using MCS.Library.Core;
using System.Data;

namespace MCS.Library.Office.OpenXml.Excel
{

    /// <summary>
    /// worksheet 映射描述类
    /// </summary>
    public class SpreadSheetDescription
    {
        private TableDescriptionCollection _TableDescriptions;
        public TableDescriptionCollection TableDescriptions
        {
            get
            {
                if (this._TableDescriptions == null)
                    this._TableDescriptions = new TableDescriptionCollection();

                return this._TableDescriptions;
            }
        }

        public SpreadSheetDescription(string sheetName)
        {
            this.SheetName = sheetName;
        }

        /// <summary>
        /// 工作表名称
        /// </summary>
        public string SheetName
        {
            get;
            set;
        }

        /// <summary>
        /// 工作表Code
        /// </summary>
        public string SheetCode
        {
            get;
            set;
        }

        private ExcelWorksheetHidden _SheetHide = ExcelWorksheetHidden.Visible;

        public ExcelWorksheetHidden SheetHide
        {
            get
            {
                return this._SheetHide;
            }
            set
            {
                this._SheetHide = value;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TableColumnDescription
    {
        private Type _DataType = typeof(string);

        public TableColumnDescription()
        {
        }

        public TableColumnDescription(PropertyInfo pi)
        {
            pi.NullCheck("pi");

            TableColumnDescriptionAttribute attr = AttributeHelper.GetCustomAttribute<TableColumnDescriptionAttribute>(pi);

            if (attr != null)
                InitDescription(attr, pi.Name);
            else
            {
                this.PropertyName = pi.Name;
                this.ColumnName = pi.Name;
            }

            this._DataType = pi.PropertyType;
        }

        public TableColumnDescription(DataColumn column)
        {
            column.NullCheck("column");

            this.ColumnName = column.Caption;

            if (this.ColumnName.IsNullOrEmpty())
                this.ColumnName = column.ColumnName;

            this.PropertyName = column.ColumnName;
            this._DataType = column.DataType;
        }

        public Type DataType
        {
            get
            {
                return this._DataType;
            }
            set
            {
                this._DataType = value;
            }
        }

        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 列公式
        /// </summary>
        public string ColumnFormula { get; set; }

        /// <summary>
        /// 这个表示对应属性名称
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 列的序序
        /// </summary>
        public int SortOrder { get; set; }

        public void InitDescription(TableColumnDescriptionAttribute tableAttribute, string propertyName)
        {
            this.SortOrder = tableAttribute.Index;
            this.ColumnName = tableAttribute.ColumnName;
            this.ColumnFormula = tableAttribute.ColumnFormula;
            this.PropertyName = propertyName;
        }
    }

    /// <summary>
    /// 填充ExcelTable数据方式
    /// </summary>
    public enum LoadFormTableMode
    {
        /// <summary>
        /// 追加
        /// </summary>
        AppendData,

        /// <summary>
        /// 清空原有数据添加
        /// </summary>
        FillData
    }

    public class TableDescription
    {
        internal TableDescription(DataTable table)
        {
            table.NullCheck("table");

            this.TableName = table.TableName;

            foreach (DataColumn col in table.Columns)
            {
                this.AllColumns.Add(new TableColumnDescription(col));
            }
        }

        public TableDescription(string tableName, string beginAddress)
            : this(tableName)
        {
            this.BeginAddress = beginAddress;
        }

        public TableDescription(Type type)
        {
            type.NullCheck("type");

            TableDescriptionAttribute attr = AttributeHelper.GetCustomAttribute<TableDescriptionAttribute>(type);

            if (attr != null)
                InitDescription(attr);
            else
                this.TableName = type.Name;
        }

        public TableDescription(string tableName)
        {
            this.TableName = tableName;
        }

        internal TableDescription()
        {
        }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName
        {
            get;
            set;
        }

        private ExcelTableStyles _TableStyle = ExcelTableStyles.Light12;
        public ExcelTableStyles TableStyle
        {
            get { return this._TableStyle; }
            set { this._TableStyle = value; }
        }

        private LoadFormTableMode _FillMode = LoadFormTableMode.FillData;
        /// <summary>
        /// 数据填充方式，默认将原有数据添空
        /// </summary>
        public LoadFormTableMode FillMode
        {
            get { return this._FillMode; }
            set { this._FillMode = value; }
        }

        private CellAddress _BeginAddress = "A1";

        public CellAddress BeginAddress
        {
            get
            {
                return this._BeginAddress;
            }
            set
            {
                this._BeginAddress = value;
            }
        }

        public void InitDescription(TableDescriptionAttribute tableAttribute)
        {
            this.TableName = tableAttribute.TableName;
            this.BeginAddress = tableAttribute.BeginAddress;
        }

        private TableColumnDescriptionCollection _AllColumns = null;

        public TableColumnDescriptionCollection AllColumns
        {
            get
            {
                if (this._AllColumns == null)
                    this._AllColumns = new TableColumnDescriptionCollection();

                return this._AllColumns;
            }
        }
    }

    public class TableDescriptionCollection : ExcelCollectionBase<string, TableDescription>
    {
        protected override string GetKeyForItem(TableDescription item)
        {
            return item.TableName;
        }
    }

    public class TableColumnDescriptionCollection : ExcelCollectionBase<string, TableColumnDescription>
    {
        public TableColumnDescriptionCollection()
        {
        }

        public TableColumnDescriptionCollection(IEnumerable<PropertyInfo> properties)
        {
            InitFromProperties(properties);
        }

        public void InitFromColumns(DataColumnCollection columns)
        {
            columns.NullCheck("columns");

            this.Clear();

            columns.ForEach<DataColumn>(column => this.Add(new TableColumnDescription(column)));
        }

        public void InitFromProperties(IEnumerable<PropertyInfo> properties)
        {
            properties.NullCheck("properties");

            this.Clear();

            properties.ForEach(pi => this.Add(new TableColumnDescription(pi)));
        }

        protected override string GetKeyForItem(TableColumnDescription item)
        {
            return string.IsNullOrEmpty(item.ColumnName) ? item.PropertyName : item.ColumnName;
        }
    }

    public class TableColumnDescriptionEqualityComparer : IEqualityComparer<TableColumnDescription>
    {

        public bool Equals(TableColumnDescription x, TableColumnDescription y)
        {
            return (x.SortOrder == y.SortOrder);
        }

        public int GetHashCode(TableColumnDescription obj)
        {
            return string.Format("{0}|{1}|{2}", obj.ColumnName, obj.PropertyName, obj.SortOrder).GetHashCode();
        }
    }

}

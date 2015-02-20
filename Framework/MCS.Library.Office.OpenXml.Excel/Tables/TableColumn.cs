using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Office.OpenXml.Excel
{
    public class TableColumn : ElementInfo
    {
        /*
         *public override string LocalName
        {
            get
            {
                return "tableColumn";
            }
        }
            "totalsRowFunction", 
            "queryTableFieldId", 
            "headerRowDxfId", 
            "totalsRowDxfId", 
            "headerRowCellStyle", 
            "totalsRowCellStyle"
          */

        private Table _Table;

        public TableColumn(Table table, string name)
        {
            this._Table = table;
            this.Name = name;
        }

        internal TableColumn(Table table, TableColumnDescription tcDescription)
        {
            this._Table = table;
            this.Name = tcDescription.ColumnName;
            this._ColumnFormula = tcDescription.ColumnFormula;

            if (tcDescription.DataType != null)
                this.Attributes["dataType"] = tcDescription.DataType.AssemblyQualifiedName;
        }

        internal TableColumn(Table table, string name, int position)
        {
            this._Table = table;
            this.Position = position;
            this.Name = name;
        }

        //private string _Name;
        public string Name
        {
            get { return base.GetAttribute("name"); }
            set
            {
                ExceptionHelper.TrueThrow(string.IsNullOrEmpty(value), "工作表列名不能为空");
                if (base.Attributes.ContainsKey("name") == true && base.GetAttribute("name").IsNotEmpty())
                {
                    if (this._Table.Columns._TableColumnNames.ContainsKey(base.GetAttribute("name")))
                    {
                        var Index = this._Table.Columns._TableColumnNames[base.GetAttribute("name")];
                        this._Table.Columns._TableColumnNames.Remove(base.GetAttribute("name"));
                        this._Table.Columns._TableColumnNames.Add(value, Index);
                    }
                }
                base.SetAttribute("name", value);
            }
        }

        public string UniqueName
        {
            get { return base.GetAttribute("uniqueName"); }
            set { base.SetAttribute("uniqueName", value); }
        }

        public string TotalsRowLabel
        {
            get { return base.GetAttribute("totalsRowLabel"); }
            set { base.SetAttribute("totalsRowLabel", value); }
        }

        private ExcelRowFunctions _ExcelRowFunctions = ExcelRowFunctions.None;
        /// <summary>
        /// 请不要设置 ExcelRowFunctions.Custom
        /// </summary>
        public ExcelRowFunctions TotalsRowFunction
        {
            get
            {
                return this._ExcelRowFunctions;
            }
            set
            {
                ExceptionHelper.TrueThrow<Exception>(value == ExcelRowFunctions.Custom, "使用TotalsRowFormula属性设置一个自定义的表公式");
                this._ExcelRowFunctions = value;
            }
        }

        private string _TotalsRowFormula = string.Empty;
        /// <summary>
        /// 设置自定义总计行公式。
        /// <example>
        /// tbl.Columns[9].TotalsRowFormula = string.Format("SUM([{0}])",tbl.Columns[9].Name);
        /// </example>
        /// </summary>
        public string TotalsRowFormula
        {
            get
            {
                return this._TotalsRowFormula;
            }
            set
            {
                if (value.StartsWith("="))
                {
                    value = value.Substring(1, value.Length - 1);
                }
                this._ExcelRowFunctions = ExcelRowFunctions.Custom;
            }
        }

        //private string _DataCellStyleName;
        public string DataCellStyleName
        {
            get
            {
                return base.GetAttribute("dataCellStyle");
            }
            set
            {
                base.SetAttribute("dataCellStyle", value);
            }
        }

        private const string COLUMNFORMULA_PATH = "d:calculatedColumnFormula";
        private string _ColumnFormula = string.Empty;
        public string ColumnFormula
        {
            get
            {
                return this._ColumnFormula;
            }
            set
            {
                this._ColumnFormula = value;
            }
        }

        internal string DataDxfId
        {
            get { return base.GetAttribute("dataDxfId"); }
            set { base.SetAttribute("dataDxfId", value); }
        }

        /// <summary>
        /// 序号
        /// </summary>
        public int Position
        {
            get
            {
                return base.GetIntAttribute("id");
            }
            internal set
            {
                base.SetIntAttribute("id", value);
            }
        }

        internal TableColumn Clone(Table table)
        {
            TableColumn tcClone = new TableColumn(table, this.Name)
            {
                _ColumnFormula = this._ColumnFormula,
                _TotalsRowFormula = this.TotalsRowFormula,
                _ExcelRowFunctions = this._ExcelRowFunctions,
            };

            foreach (KeyValuePair<string, string> item in this.Attributes)
            {
                tcClone.Attributes[item.Key] = item.Value;
            }

            return tcClone;
        }

        protected internal override string NodeName
        {
            get { return "tableColumn"; }
        }
    }
}

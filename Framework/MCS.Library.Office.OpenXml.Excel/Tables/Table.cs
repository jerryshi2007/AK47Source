using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MCS.Library.Core;
using System.IO.Packaging;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Data;
using MCS.Library.Caching;

namespace MCS.Library.Office.OpenXml.Excel
{
    public class Table : ElementInfo, IPersistable
    {
        /*
            "ref", 
            "tableType", 

            "insertRowShift", 
            "totalsRowCount", 
         * 
            "published", 
            "dataDxfId", 
            "totalsRowDxfId", 
            "headerRowBorderDxfId", 
            "tableBorderDxfId", 
            "totalsRowBorderDxfId", 
            "headerRowCellStyle", 

            "totalsRowCellStyle", 
            "connectionId"
         */

        /// <summary>
        /// 存储原来的存储
        /// </summary>
        internal Range OldRange { get; set; }

        public Table(WorkSheet workSheet, string name, CellAddress beginAddress)
            : this(workSheet, name, Range.Parse(workSheet, beginAddress.ColumnIndex, beginAddress.RowIndex, beginAddress.ColumnIndex, beginAddress.RowIndex))
        {
        }

        internal Table(WorkSheet workSheet, string name, Range rangeAddress)
        {
            this._WorkSheet = workSheet;
            this.Name = name;
            if (this.Address.StartColumn != -1 && this.Address.StartRow != -1)
                this.OldRange = this.Address;

            this.Address = rangeAddress;
        }

        internal Table(WorkSheet workSheet, string relationshipId)
        {
            this._WorkSheet = workSheet;
            this.RelationshipID = relationshipId;
        }

        internal Table(WorkSheet workSheet, TableDescription tbDes)
        {
            this._WorkSheet = workSheet;
            this.Name = tbDes.TableName;
            this.Address = Range.Parse(workSheet, tbDes.BeginAddress.ColumnIndex, tbDes.BeginAddress.RowIndex, tbDes.BeginAddress.ColumnIndex, tbDes.BeginAddress.RowIndex);
            this.Columns.InitColumns(tbDes.AllColumns);
        }

        /// <summary>
        /// 如果列集合为空，将自动初始化列集合
        /// </summary>
        internal void InitTableColumns()
        {
            int index = 0;
            for (int i = this.Address.StartColumn; i <= this.Address.EndColumn; i++)
            {
                string columnName = string.Format("列{0}", index);
                TableColumn currentColumn = new TableColumn(this, columnName)
                {
                    Position = index
                };
                this.Columns.Add(currentColumn);
                index++;
            }
            this._NextColumnID = index;
        }

        /// <summary>
        /// 初始化Table集合
        /// </summary>
        internal void InitTableAddress()
        {
            ExceptionHelper.TrueThrow(this.Address.StartColumn < 0 && this.Address.StartRow < 0, "Excel Table地址不符合标准");

            if (this.Columns.Count == 0 && this.Address.EndRow > this.Address.StartRow && this.Address.EndColumn >= this.Address.StartColumn)
            {
                this.InitTableColumns();
            }
            else
            {
                if (this.OldRange.Equals(this.Address) == false)
                {
                    int endRow = this.Address.StartRow + this.Rows.Count;
                    if (this.Rows.Count == 0)
                        endRow++;

                    this.Address = Range.Parse(this._WorkSheet, this.Address.StartColumn, this.Address.StartRow, this.Address.StartColumn + this.Columns.Count - 1, endRow);
                }
            }
        }

        internal WorkSheet _WorkSheet;

        private int _NextColumnID = 1;
        /// <summary>
        /// 下一个添加ColumnID
        /// </summary>
        internal int NextColumnID
        {
            get { return this._NextColumnID; }
            set { this._NextColumnID = value; }
        }

        public int ID
        {
            get { return base.GetIntAttribute("id"); }
            set { base.SetIntAttribute("id", value); }
        }

        private Uri _TableUri;
        internal Uri TableUri
        {
            get
            {
                if (this._TableUri == null)
                {
                    this._TableUri = new Uri(string.Format(@"/xl/tables/table{0}.xml", ID), UriKind.Relative);
                }
                return this._TableUri;
            }
        }

        internal string RelationshipID
        {
            get;
            set;
        }

        public string DisplayName
        {
            get
            {
                //if (this._DisplayName.IsNullOrEmpty())
                if (base.Attributes.ContainsKey("displayName") == false)
                    base.SetAttribute("displayName", CleanDisplayName(this.Name));

                return base.GetAttribute("displayName");
            }
            set
            {
                base.SetAttribute("displayName", value);
            }
        }

        internal static string CleanDisplayName(string name)
        {
            return Regex.Replace(name, @"[^\w\.-_]", "_");
        }

        public string Name
        {
            get
            {
                return base.GetAttribute("name");
            }
            set
            {
                base.SetAttribute("name", value);
                if (base.Attributes.ContainsKey("displayName") == false)
                    base.SetAttribute("displayName", CleanDisplayName(value));
            }
        }

        public string Comment
        {
            get { return base.GetAttribute("comment"); }
            set { base.SetAttribute("comment", value); }
        }

        public string DataCellStyle
        {
            get { return base.GetAttribute("dataCellStyle"); }
            set { base.SetAttribute("dataCellStyle", value); }
        }

        #region “tableStyleInfo”
        private ExcelTableStyles _TableStyle = ExcelTableStyles.Medium6;
        public ExcelTableStyles TableStyle
        {
            get
            {
                return this._TableStyle;
            }
            set
            {
                if (value == ExcelTableStyles.None)
                {
                    this._StyleName = string.Empty;
                }
                this._TableStyle = value;
            }
        }

        //const string STYLENAME_PATH = "d:tableStyleInfo/@name";
        private string _StyleName;
        public string StyleName
        {
            get
            {
                return this._StyleName;
            }
            set
            {
                if (value.StartsWith("TableStyle"))
                {
                    try
                    {
                        this._TableStyle = (ExcelTableStyles)Enum.Parse(typeof(ExcelTableStyles), value.Substring(10, value.Length - 10), true);
                    }
                    catch
                    {
                        this._TableStyle = ExcelTableStyles.Custom;
                    }
                }
                else if (value == "None")
                {
                    this._TableStyle = ExcelTableStyles.None;
                    value = string.Empty;
                }
                else
                {
                    this._TableStyle = ExcelTableStyles.Custom;
                }
            }
        }

        //const string SHOWFIRSTCOLUMN_PATH = "d:tableStyleInfo/@showFirstColumn";
        private bool _ShowFirstColumn = false;
        public bool ShowFirstColumn
        {
            get
            {
                return this._ShowFirstColumn;
            }
            set
            {
                this._ShowFirstColumn = value;
            }
        }

        //const string SHOWLASTCOLUMN_PATH = "d:tableStyleInfo/@showLastColumn";
        public bool ShowLastColumn
        {
            get;
            set;
        }

        //const string SHOWROWSTRIPES_PATH = "d:tableStyleInfo/@showRowStripes";
        private bool _ShowRowStripes = true;
        public bool ShowRowStripes
        {
            get { return this._ShowRowStripes; }
            set { this._ShowRowStripes = value; }
        }

        //const string SHOWCOLUMNSTRIPES_PATH = "d:tableStyleInfo/@showColumnStripes";
        public bool ShowColumnStripes
        {
            get;
            set;
        }
        #endregion

        public Range Address
        {
            get;
            set;
        }

        /// <summary>
        /// 0表示不显示表头，1表示首行首列，如果大于一行，则填写实际行数
        /// </summary>
        public int HeaderRowCount
        {
            get { return base.GetIntAttribute("headerRowCount"); }
            set { base.SetIntAttribute("headerRowCount", value); }
        }

        /// <summary>
        /// 从零开始的整数索引到差分格式记录在<dxfs>样式表说明适用于本表中的标题行的格式。
        /// </summary>
        public int HeaderRowDxfId
        {
            get { return base.GetIntAttribute("headerRowDxfId"); }
            set { base.SetIntAttribute("headerRowDxfId", value); }
        }

        /// <summary>
        /// 当用户点击UI中插入行
        /// True时插入行显示，否则为false
        /// </summary>
        public bool InsertRow
        {
            get { return base.GetBooleanAttribute("insertRow"); }
            set { base.SetBooleanAttribute("insertRow", value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool InsertRowShift
        {
            get { return base.GetBooleanAttribute("insertRowShift"); }
            set { base.SetBooleanAttribute("insertRowShift", value); }
        }

        /// <summary>
        /// 是否启用汇总行
        /// </summary>
        public bool IsTotalsRowShown
        {
            get { return base.GetBooleanAttribute("totalsRowShown"); }
            set { base.SetBooleanAttribute("totalsRowShown", value); }
        }


        private TableColumnCollection _Columns;
        /// <summary>
        /// 列定义集合
        /// </summary>
        public TableColumnCollection Columns
        {
            get
            {
                if (this._Columns == null)
                    this._Columns = new TableColumnCollection(this);

                return this._Columns;
            }
        }

        private TableRowCollection _Rows;
        public TableRowCollection Rows
        {
            get
            {
                if (this._Rows == null)
                    this._Rows = new TableRowCollection(this);

                return this._Rows;
            }
        }

        void IPersistable.Save(ExcelSaveContext context)
        {
            this.ID = context.NextTableID;
            context.NextTableID++;
            context.LinqWriter.WriteTable(this);
        }

        void IPersistable.Load(ExcelLoadContext context)
        {
            PackageRelationship tableRelation = context.Package.GetPart(this._WorkSheet.SheetUri).GetRelationship(this.RelationshipID);
            this._TableUri = PackUriHelper.ResolvePartUri(tableRelation.SourceUri, tableRelation.TargetUri);

            XElement tableElement = context.Package.GetXElementFromUri(PackUriHelper.ResolvePartUri(tableRelation.SourceUri, tableRelation.TargetUri));
            context.Reader.ReadTable(this, tableElement);
        }

        /// <summary>
        /// 匹配填充数据
        /// </summary>
        /// <param name="dv"></param>
        public void FillData(DataView dv)
        {
            this.FillData(dv, null);
        }

        /// <summary>
        /// 填充数据。可以在填充数据时定制单元格的内容
        /// </summary>
        /// <param name="dv"></param>
        /// <param name="creatingDataCellAction"></param>
        public void FillData(DataView dv, CreatingDataCellAction<DataRowView> creatingDataCellAction)
        {
            dv.NullCheck("数据源不能为空");
            int rowIndex = 0;

            this.Rows.Clear();

            foreach (DataRowView drv in dv)
            {
                TableRow newRow = this.Rows.NewTableRow();
                TableColumn excelColumn;

                foreach (DataColumn col in dv.Table.Columns)
                {
                    if (this.TryTableColumn(col.ColumnName, col.Caption, out excelColumn))
                    {
                        if (creatingDataCellAction == null)
                            newRow[excelColumn].Value = drv[col.ColumnName];
                        else
                            creatingDataCellAction(newRow[excelColumn], new CreatingDataCellParameters<DataRowView>(drv, drv[col.ColumnName], col.ColumnName, rowIndex));
                    }
                }

                rowIndex++;
            }

            this.SyncTablePropertiesAfterFillData(dv);
        }

        internal void FillData(DataView dv, TableDescription tableDesp, CreatingDataCellAction<DataRowView> creatingDataCellAction)
        {
            dv.NullCheck("数据源不能为空");
            int rowIndex = 0;

            this.Rows.Clear();

            foreach (DataRowView drv in dv)
            {
                TableRow newRow = this.Rows.NewTableRow();

                foreach (TableColumnDescription col in tableDesp.AllColumns)
                {
                    TableColumn excelColumn;

                    if (this.TryTableColumn(col.ColumnName, col.PropertyName, out excelColumn))
                    {
                        if (creatingDataCellAction == null)
                            newRow[excelColumn].Value = drv[col.PropertyName];
                        else
                            creatingDataCellAction(newRow[excelColumn], new CreatingDataCellParameters<DataRowView>(drv, drv[col.PropertyName], col.ColumnName, rowIndex));
                    }
                }

                rowIndex++;
            }

            this.SyncTablePropertiesAfterFillData(dv);
        }

        internal void FillData<T>(IEnumerable<T> collection, TableDescription tableDesp, CreatingDataCellAction<T> creatingDataCellAction, LoadFormTableMode fillMode = LoadFormTableMode.FillData)
        {
            int rowIndex = 0;

            if (creatingDataCellAction == null)
                creatingDataCellAction = WorkSheet.DefaultCreatingDataCellAction;

            if (fillMode == LoadFormTableMode.FillData)
                this.Rows.Clear();

            foreach (var objItem in collection)
            {
                TableRow newRow = this.Rows.NewTableRow();

                foreach (TableColumnDescription col in tableDesp.AllColumns)
                {
                    if (this.Columns.ContainsKey(col.ColumnName) == false)
                        continue;

                    TableCell tbCell = newRow[this.Columns[col.ColumnName]];

                    object propertyValue = TypePropertiesCacheQueue.Instance.GetObjectPropertyValue(objItem, col.PropertyName);

                    creatingDataCellAction(tbCell, new CreatingDataCellParameters<T>(objItem, propertyValue, col.ColumnName, rowIndex));
                }

                rowIndex++;
            }
        }

        internal bool TryTableColumn(string name, string caption, out TableColumn col)
        {
            bool result = false;
            col = null;
            if (this.Columns.ContainsKey(caption))
            {
                col = this.Columns[caption];
                result = true;
            }
            else if (this.Columns.ContainsKey(name))
            {
                col = this.Columns[name];
                result = true;
            }

            return result;
        }

        /// <summary>
        /// 将数据返回为DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable AsDataTable()
        {
            DataTable dt = new DataTable(this.Name);
            int beginCol = this.Address.StartColumn;

            foreach (TableColumn excelColumn in this.Columns)
            {
                Type columnDataType = typeof(string);

                string dataType = excelColumn.GetAttribute("dataType");

                if (dataType != null)
                    columnDataType = TypeCreator.GetTypeInfo(dataType);

                dt.Columns.Add(excelColumn.Name, columnDataType);
            }

            foreach (TableRow tr in this.Rows)
            {
                DataRow dr = dt.NewRow();
                foreach (TableColumn Col in this.Columns)
                {
                    dr[Col.Name] = tr[Col].Value;
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        internal Table Clone(WorkSheet worksheet)
        {
            Table cloneTable = new Table(worksheet, this.Name, this.Address)
            {
                _NextColumnID = this._NextColumnID,
                _TableStyle = this._TableStyle,
                _StyleName = this._StyleName,
                _ShowFirstColumn = this._ShowFirstColumn,
                ShowLastColumn = this.ShowLastColumn,
                _ShowRowStripes = this._ShowRowStripes,
                ShowColumnStripes = this.ShowColumnStripes
            };

            foreach (KeyValuePair<string, string> item in this.Attributes)
            {
                cloneTable.Attributes[item.Key] = item.Value;
            }

            cloneTable.DisplayName = string.Format("{0}_Clone{1}", this.Name, worksheet.WorkBook.GetTablesCount());

            if (this.Attributes.ContainsKey("id") == true)
                this.Attributes.Remove("id");

            if (this._Columns != null)
                cloneTable._Columns = this._Columns.Clone(cloneTable);

            if (this._Rows != null)
                cloneTable._Rows = this._Rows.Clone(cloneTable);

            return cloneTable;
        }

        protected internal override string NodeName
        {
            get { return "table"; }
        }

        /// <summary>
        /// 填充数据后同步表格的属性
        /// </summary>
        /// <param name="dv"></param>
        private void SyncTablePropertiesAfterFillData(DataView dv)
        {
            Range address = this.Address;

            this.Address = Range.Parse(address.StartColumn, address.StartRow, address.EndColumn, address.StartRow + dv.Count);

            this.SyncDataValidationRange();
        }

        /// <summary>
        /// 同步校验规则的范围
        /// </summary>
        private void SyncDataValidationRange()
        {
            foreach (IDataValidation validation in this._WorkSheet.Validations)
            {
                if (validation.Address.IsSubset(this.Address))
                {
                    if (validation.Address.EndRow < this.Address.EndRow)
                    {
                        validation.Address = Range.Parse(validation.Address.StartColumn, this.Address.StartRow + 1, validation.Address.EndColumn, this.Address.EndRow);
                    }
                }
            }
        }
    }
}

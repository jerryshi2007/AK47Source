using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Packaging;
using System.IO;
using System.Xml;
using MCS.Library.Core;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Security;
using System.Drawing;
using System.Xml.Linq;
using System.Data;
using MCS.Library.Office.OpenXml.Excel.DataValidation;
using System.Reflection;
using MCS.Library.Caching;

namespace MCS.Library.Office.OpenXml.Excel
{
    public sealed class WorkSheet : IPersistable
    {
        public WorkSheet(WorkBook workbook, string sheetName, ExcelWorksheetHidden sheetHide = ExcelWorksheetHidden.Visible)
        {
            this.Name = sheetName;
            this.WorkBook = workbook;
            this.Hidden = sheetHide;
        }

        internal WorkSheet(WorkBook workbook, string sheetName, int sheetID, string relationshipID, int positionID, ExcelWorksheetHidden sheetHide)
        {
            this.Name = sheetName;
            this._RelationshipID = relationshipID;
            this._PositionID = positionID;
            this.WorkBook = workbook;
            this.Hidden = sheetHide;
        }

        /// <summary>
        /// 工作簿所属工作表
        /// </summary>
        public WorkBook WorkBook { get; internal set; }

        /// <summary>
        /// 工作簿名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 隐藏工作表标识（通过代码可以拿到与设置)
        /// </summary>
        public string SheetCode { get; set; }

        private string _RelationshipID;
        internal string RelationshipID
        {
            get { return this._RelationshipID; }
            set { this._RelationshipID = value; }
        }

        private int _PositionID;
        internal int PositionID
        {
            get { return this._PositionID; }
            set { this._PositionID = value; }
        }


        #region “Sheet Properties”
        #region  "Attributes"
        //@syncHorizontal
        /// <summary>
        /// 
        /// </summary>
        public bool SyncHorizontal
        {
            get;
            set;
        }
        #endregion

        #region “Child Elements”

        #region “sheetFormatPr”
        //d://sheetFormatPr/@baseColWidth
        private double _BaseColumnWidth = double.MinValue;
        public double BaseColumnWidth
        {
            get
            {
                return this._BaseColumnWidth;
            }
            set
            {
                this._BaseColumnWidth = value;
            }
        }

        //d://sheetFormatPr/@customHeight
        internal bool CustomHeight { get; set; }
        #endregion

        #region "outlinePr"
        //applyStyles
        /// <summary>
        /// 是否适用于一个大纲样式
        /// </summary>
        public bool OutLineApplyStyle { get; set; }

        /// <summary>
        /// 汇总行是否会出现下面的详细提纲
        /// </summary>
        public bool OutLineSummaryBelow { get; set; }

        //showOutlineSymbols
        /// <summary>
        /// 是否表分级显示符号
        /// </summary>
        public bool ShowOutlineSymbols { get; set; }

        //summaryRight
        /// <summary>
        /// 标志汇总列是否出现在右侧的详细提纲
        /// </summary>
        public bool OutLineSummaryRight { get; set; }

        #endregion

        #region “tabColor”
        //d:sheetPr/d:tabColor/@rgb
        internal ColorXmlWrapper _TabColor = null;
        public ColorXmlWrapper TabColor
        {
            get
            {
                if (this._TabColor == null)
                {
                    this._TabColor = new ColorXmlWrapper();
                }
                return this._TabColor;
            }
            set
            {
                this._TabColor = value;
            }
        }
        #endregion

        #endregion

        #endregion

        #region “默认行高”
        private double _DefaultRowHeight = 15;
        /// <summary>
        ///  获取或设置默认行高
        /// </summary>
        public double DefaultRowHeight
        {
            get
            {
                return this._DefaultRowHeight;
            }
            set
            {
                if (value != ExcelCommon.WorkSheet_DefaultColumnWidth)
                {
                    this.CustomHeight = true;
                    this._DefaultRowHeight = value;
                }
                else
                {
                    if (this.CustomHeight)
                    {
                        this.CustomHeight = false;
                    }
                }
            }
        }
        #endregion

        #region “默认列宽”
        private double _DefaultColWidth = ExcelCommon.WorkSheet_DefaultColumnWidth;
        /// <summary>
        /// 获取或设置工作薄默认列宽
        /// </summary>
        public double DefaultColumnWidth
        {
            get
            {
                return this._DefaultColWidth;
            }
            set
            {
                if (value != ExcelCommon.WorkSheet_DefaultColumnWidth)
                {
                    this._DefaultColWidth = value;
                }
            }
        }
        #endregion

        internal Dictionary<string, Formulas> _SharedFormulas = new Dictionary<string, Formulas>();
        internal Dictionary<Cell, Formulas> _DataTableFormulas = new Dictionary<Cell, Formulas>();
        internal List<Range> _MergeCells = new List<Range>();

        private ExcelIndexCollection<Row> _Rows;
        /// <summary>
        /// 行集合
        /// </summary>
        public ExcelIndexCollection<Row> Rows
        {
            get
            {
                if (this._Rows == null)
                    this._Rows = new ExcelIndexCollection<Row>(this);

                return this._Rows;
            }
            set
            {
                this._Rows = value;
            }
        }

        private ExcelIndexCollection<Column> _Columns;
        /// <summary>
        /// 列集合
        /// </summary>
        public ExcelIndexCollection<Column> Columns
        {
            get
            {
                if (this._Columns == null)
                    this._Columns = new ExcelIndexCollection<Column>(this);

                return this._Columns;
            }
            set
            {
                this._Columns = value;
            }
        }

        internal TableCollection _Tables = null;
        /// <summary>
        /// Excel表集合
        /// </summary>
        public TableCollection Tables
        {
            get
            {
                if (this._Tables == null)
                    this._Tables = new TableCollection(this);

                return this._Tables;
            }
        }

        private CellCollection _Cells = null;
        /// <summary>
        /// 单元格集合
        /// </summary>
        public CellCollection Cells
        {
            get
            {
                if (this._Cells == null)
                    this._Cells = new CellCollection(this);

                return this._Cells;
            }
            set
            {
                this._Cells = value;
            }
        }

        private DefinedNameCollection _DefinedNames;
        /// <summary>
        /// 名称定义集合
        /// </summary>
        public DefinedNameCollection Names
        {
            get
            {
                if (this._DefinedNames == null)
                    this._DefinedNames = new DefinedNameCollection(this);

                return this._DefinedNames;
            }
        }


        internal DrawingCollection _Drawings;
        /// <summary>
        /// 画图集合
        /// </summary>
        public DrawingCollection Drawings
        {
            get
            {
                if (this._Drawings == null)
                    this._Drawings = new DrawingCollection(this);

                return this._Drawings;
            }
        }

        internal HeaderFooter _HeaderFooter;
        public HeaderFooter HeaderFooter
        {
            get
            {
                if (this._HeaderFooter == null)
                {
                    this._HeaderFooter = new HeaderFooter(this);
                }
                return this._HeaderFooter;
            }
        }

        internal DataValidationCollection _Validations = null;

        public DataValidationCollection Validations
        {
            get
            {
                if (this._Validations == null)
                    this._Validations = new DataValidationCollection(this);

                return this._Validations;
            }
        }

        #region "Formulas"
        /// <summary>
        /// 批量设置公式
        /// </summary>
        /// <param name="rangeAddress">Range地址</param>
        /// <param name="formulaValue">公式</param>
        public void SetFormulas(string rangeAddress, string formulaValue)
        {
            formulaValue.IsNullOrEmpty().TrueThrow("公式不能为空！{0}", formulaValue);

            Range setAddress = Range.Parse(this, rangeAddress);
            if (setAddress.StartColumn == setAddress.EndColumn && setAddress.StartRow == setAddress.EndRow)
            {
                this.Cells[setAddress.StartRow, setAddress.StartColumn].Formula = formulaValue;
            }
            else
            {
                this.SetFormulas(setAddress, formulaValue);
            }
        }

        /// <summary>
        /// 批量设置公式
        /// </summary>
        /// <param name="rangeAddress">Range地址</param>
        /// <param name="formulaValue">公式</param>
        public void SetFormulas(int startRow, int startColumn, int endRow, int endColumn, string formulaValue)
        {
            formulaValue.IsNullOrEmpty().TrueThrow("公式不能为空！{0}", formulaValue);
            Range setAddress = Range.Parse(this, startColumn, startRow, endColumn, endRow);
            if (setAddress.StartColumn == setAddress.EndColumn && setAddress.StartRow == setAddress.EndRow)
            {
                this.Cells[setAddress.StartRow, setAddress.StartColumn].Formula = formulaValue;
            }
            else
            {
                this.SetFormulas(setAddress, formulaValue);
            }
        }

        private void SetFormulas(Range setAddress, string formulaValue)
        {
            int shareIndex = this._SharedFormulas.Count + 1;
            this._SharedFormulas.Add(formulaValue, new Formulas() { Address = setAddress, Formula = formulaValue, Index = shareIndex, IsArray = false });
            this.Cells[setAddress.StartRow, setAddress.StartColumn].Formula = formulaValue;
            for (int i = setAddress.StartColumn; i <= setAddress.EndColumn; i++)
            {
                for (int j = setAddress.StartRow; j <= setAddress.EndRow; j++)
                {
                    Cell changCell = this.Cells[j, i];
                    changCell.FormulaSharedIndex = shareIndex;
                    changCell.Value = null;
                }
            }
        }

        private void SetFormulas(Range setAddress, Cell cell)
        {
            int shareIndex = this._SharedFormulas.Count;
            this._SharedFormulas.Add(cell.Formula, new Formulas() { Address = setAddress, Formula = cell.Formula, Index = shareIndex, IsArray = false });
            this.Cells[setAddress.StartRow, setAddress.StartColumn].Formula = SecurityElement.Escape(cell.Formula);
            for (int i = setAddress.StartColumn; i <= setAddress.EndColumn; i++)
            {
                for (int j = setAddress.StartRow; j <= setAddress.EndRow; j++)
                {
                    Cell changCell = this.Cells[j, i];
                    changCell.FormulaSharedIndex = shareIndex;
                    changCell.Style = cell.Style;
                    changCell.StyleID = cell.StyleID;
                    changCell.Value = null;
                }
            }
        }
        #endregion

        #region "LoadFromCollection"

        public void LoadFromCollection<T>(IEnumerable<T> collection)
        {
            this.LoadFromCollection<T>(collection, SpreadSheetAttributeHelper.GetTableDescription<T>(), null);
        }

        public void LoadFromCollection<T>(IEnumerable<T> collection, CreatingDataCellAction<T> creatingDataCellAction)
        {
            this.LoadFromCollection<T>(collection, SpreadSheetAttributeHelper.GetTableDescription<T>(), creatingDataCellAction);
        }

        /// <summary>
        /// 将数据源对象映射生成ExcelTable
        /// </summary>
        /// <typeparam name="T">数据模型实体</typeparam>
        /// <param name="collection">待填充集合</param>
        /// <param name="tableDesp">生成ExcelTable描述信息</param>
        /// <param name="creatingDataCellAction">设置Cell相关信息委托</param>
        public void LoadFromCollection<T>(IEnumerable<T> collection, TableDescription tableDesp, CreatingDataCellAction<T> creatingDataCellAction)
        {
            collection.NullCheck("collection");
            tableDesp.NullCheck("TableDescription");

            if (tableDesp.TableStyle == ExcelTableStyles.None && tableDesp.TableName.IsNullOrEmpty())
                this.LoadFromCollectionNotTable<T>(tableDesp, collection, creatingDataCellAction);
            else
            {
                Table table;
                if (this.Tables.TryGetTable(tableDesp.TableName, out table) == false)
                {
                    table = new Table(this, tableDesp);
                    this.Tables.Add(table);
                }

                table.FillData(collection, tableDesp, creatingDataCellAction, tableDesp.FillMode);
            }
        }

        private void LoadFromCollectionNotTable<T>(TableDescription tbDesp, IEnumerable<T> collection, CreatingDataCellAction<T> creatingDataCellAction)
        {
            int col = tbDesp.BeginAddress.ColumnIndex;
            int row = tbDesp.BeginAddress.RowIndex;

            if (creatingDataCellAction == null)
                creatingDataCellAction = DefaultCreatingDataCellAction;

            this.Names.AddRangeByDescription(tbDesp);

            foreach (var objItem in collection)
            {
                row++;
                col = tbDesp.BeginAddress.ColumnIndex;

                foreach (TableColumnDescription tcDesp in tbDesp.AllColumns)
                {
                    Cell cell = this.Cells[row, col];

                    if (tcDesp != null)
                        cell.DataType = tcDesp.DataType.ToCellDataType();

                    creatingDataCellAction(cell,
                        new CreatingDataCellParameters<T>(objItem, TypePropertiesCacheQueue.Instance.GetObjectPropertyValue(objItem, tcDesp.PropertyName), tcDesp.ColumnName, row));

                    col++;
                }
            }
        }

        internal static void DefaultCreatingDataCellAction<T>(CellBase cell, CreatingDataCellParameters<T> result)
        {
            cell.Value = result.PropertyValue;
        }
        #endregion

        #region “GetCollection”
        public string GetCollectionFromTable<T, TCollection>(TCollection collection, SpreadGetTableCollectionParams<T> param) where TCollection : ICollection<T>
        {
            return this.GetCollectionFromTable<T, TCollection>(collection, SpreadSheetAttributeHelper.GetTableDescription<T>(), param);
        }

        /// <summary>
        /// 将Excel数据直充到指定的Collection中
        /// </summary>
        /// <typeparam name="T">数据模型</typeparam>
        /// <typeparam name="TCollection">待待充Collection 必须实现ICollection<T> 接口</typeparam>
        /// <param name="collection">待待充Collection</param>
        /// <param name="tbDesp">Table相关描述信息</param>
        /// <param name="param">填充时所准备的相关参数，包括初始化每一个对象委托，根据相关数据设置相关属性值委托（默认反射）</param>
        /// <returns></returns>
        public string GetCollectionFromTable<T, TCollection>(TCollection collection, TableDescription tbDesp, SpreadGetTableCollectionParams<T> param) where TCollection : ICollection<T>
        {
            tbDesp.NullCheck("tbDesp");
            collection.NullCheck("数据集合不能为空");
            param.ExportRow.NullCheck("exportRow");
            this.CheckTableExists(tbDesp.TableName);

            StringBuilder customLog = new StringBuilder();
            Table tb = this.Tables[tbDesp.TableName];
            int rowIndex = 0;
            foreach (TableRow tr in tb.Rows)
            {
                ExcportRowContext context = new ExcportRowContext();
                context.RowIndex = rowIndex;
                foreach (TableColumnDescription tc in tbDesp.AllColumns)
                {
                    if (tb.Columns.ContainsKey(tc.ColumnName))
                        context.PropertyDescriptions.Add(new ExportCellDescription(tc.PropertyName) { TableColumnName = tc.ColumnName, Value = tr[tb.Columns[tc.ColumnName]].Value, Address = CellAddress.Parse(tb.Columns[tc.ColumnName].Position + tb.Address.StartColumn, tr.RowIndex + tb.Address.StartRow + 1).ToString() });
                }

                ExportRowResult<T> result = param.ExportRow(context);

                if (result.Validated == true && result.CurrentObject != null)
                    collection.Add(result.CurrentObject);

                customLog.Append(result.ErrorLog);
                rowIndex++;

                if (result.Validated == false && param.ValidationOperator == ValidationErrorStopMode.Stop)
                    break;
            }

            return customLog.ToString();
        }

        /*	public void GetCollectionFromTable<T, TCollection>(TCollection collection, SpreadGetTableCollectionParams<T> param) where TCollection : ICollection<T>
           {
               this.GetCollectionFromTable<T, TCollection>(SpreadSheetAttributeHelper.GetTableDescription<T>(), collection, param);
           } */

        public void GetCollectionFromTable<T, TCollection>(TableDescription tbDesp, TCollection collection, SpreadGetTableCollectionParams<T> param) where TCollection : ICollection<T>
        {
            tbDesp.NullCheck("tbDesp");
            collection.NullCheck("数据集合不能为空");
            param.IintObj.NullCheck("创建空实体不能为空");

            this.CheckTableExists(tbDesp.TableName);

            Table tb = this.Tables[tbDesp.TableName];
            foreach (TableRow tr in tb.Rows)
            {
                T data = param.IintObj();
                foreach (TableColumnDescription tc in tbDesp.AllColumns)
                {
                    if (tb.Columns.ContainsKey(tc.ColumnName))
                    {
                        CellBase cell = tr[tb.Columns[tc.ColumnName]];
                        if (param.SetObjProperty != null)
                            param.SetObjProperty(ref data, cell.Value, tc.ColumnName, tc.PropertyName);
                        else
                            SpreadSheetExcportHelper.SetPropertyValue<T>(data, tc.PropertyName, cell.Value);
                    }
                }
                collection.Add(data);
            }
        }

        /// <summary>
        ///  逐行获取Table数据
        /// </summary>
        /// <typeparam name="T">待转换对象</typeparam>
        /// <param name="exportRow">导出行方法</param>
        /// <param name="upCount">验证正确条数</param>
        /// <param name="stopMode">验证出错是处理方式</param>
        /// <returns>返回所有错误日制</returns>
        public string ForEachTableRows<T>(ExportTableRow<T> exportRow, out int upCount, ValidationErrorStopMode stopMode = ValidationErrorStopMode.Continue)
        {
            return this.ForEachTableRows(SpreadSheetAttributeHelper.GetTableDescription<T>(), new SpreadGetTableCollectionParams<T>(exportRow, stopMode), out upCount);
        }

        /// <summary>
        /// 逐行获取Table数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tbDesp"></param>
        /// <param name="getemptyObj"></param>
        /// <param name="validationOperator"></param>
        public string ForEachTableRows<T>(TableDescription tbDesp, SpreadGetTableCollectionParams<T> param, out int upCount)
        {
            tbDesp.NullCheck("tbDesp");
            param.ExportRow.NullCheck("exportRow");
            this.CheckTableExists(tbDesp.TableName);

            StringBuilder customLog = new StringBuilder();
            Table tb = this.Tables[tbDesp.TableName];
            int rowIndex = 0; upCount = 0;
            foreach (TableRow tr in tb.Rows)
            {
                ExcportRowContext context = new ExcportRowContext() { RowIndex = rowIndex };
                foreach (TableColumnDescription tc in tbDesp.AllColumns)
                {
                    if (tb.Columns.ContainsKey(tc.ColumnName))
                        context.PropertyDescriptions.Add(new ExportCellDescription(tc.PropertyName) { TableColumnName = tc.ColumnName, Value = tr[tb.Columns[tc.ColumnName]].Value, Address = CellAddress.Parse(tb.Columns[tc.ColumnName].Position + tb.Address.StartColumn, tr.RowIndex + tb.Address.StartRow + 1).ToString() });
                }
                ExportRowResult<T> result = param.ExportRow(context);
                customLog.Append(result.ErrorLog);
                rowIndex++;

                if (result.Validated)
                    upCount++;
                else
                    if (param.ValidationOperator == ValidationErrorStopMode.Stop)
                        break;
            }

            return customLog.ToString();
        }

        /*
        internal TCollection ExcelTableToObjectCollection<T, TCollection>()
            where T : class,new()
            where TCollection : ICollection<T>, new()
        {
            return this.ExcelTableToObjectCollection<T, TCollection>(SpreadSheetAttributeHelper.GetTableDescription<T>());
        }

        /// <summary>
        /// 拿到指定当前工作薄上指定表的数据。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="tableName"></param>
        /// <returns></returns>
        internal TCollection ExcelTableToObjectCollection<T, TCollection>(TableDescription tbDesp)
            where T : class, new()
            where TCollection : ICollection<T>, new()
        {
            //TODO: 建议采用FillCollectionFromTable，Collection由调用者来提供，提供两个委托定义，包括new每一个元素，以及每一次数据的SET
            tbDesp.NullCheck("tbDesp");
            this.CheckTableExists(tbDesp.TableName);

            TCollection result = new TCollection();

            Table tb = this.Tables[tbDesp.TableName];

            Dictionary<string, PropertyInfo> columnMapping = TypePropertiesCacheQueue.Instance.GetPropertyDictionary(typeof(T));

            foreach (TableRow tr in tb.Rows)
            {
                T data = new T();
                foreach (TableColumnDescription tc in tbDesp.AllColumns)
                {
                    if (tb.Columns.ContainsKey(tc.ColumnName))
                        this.SetPropertyValue<T>(data, columnMapping[tc.PropertyName], tr[tb.Columns[tc.ColumnName]]);
                }

                result.Add(data);
            }

            return result;
        }

        internal void ForeachTableRows<T>(Action<ExportTableRowContext<T>> rowValidator) where T : class,new()
        {
            this.ForeachTableRows<T>(SpreadSheetAttributeHelper.GetTableDescription<T>(), rowValidator);
        }

        /// <summary>
        /// 逐行验证处理指定Table数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="rowValidator"></param>
        internal void ForeachTableRows<T>(TableDescription tbDesp, Action<ExportTableRowContext<T>> rowValidator) where T : class,new()
        {
            tbDesp.NullCheck("tbDesp");
            this.CheckTableExists(tbDesp.TableName);
            Table tb = this.Tables[tbDesp.TableName];

            Type entityType = typeof(T);
            Dictionary<string, PropertyInfo> columnMapping = TypePropertiesCacheQueue.Instance.GetPropertyDictionary(entityType);

            this.ForeachTableRowsByDescriptions<T>(tb, tbDesp, columnMapping, rowValidator);
        }

        internal void ForeachTableRowsByDescriptions<T>(Table tb, TableDescription tdes, Dictionary<string, PropertyInfo> columnMapping, Action<ExportTableRowContext<T>> rowValidator) where T : class,new()
        {
            foreach (TableRow tr in tb.Rows)
            {
                T data = new T();
                ExportTableRowContext<T> result = new ExportTableRowContext<T>(data) { CurrentTableName = tb.Name };

                foreach (TableColumnDescription tcd in tdes.AllColumns)
                {
                    if (tb.Columns.ContainsKey(tcd.ColumnName) && columnMapping.ContainsKey(tcd.PropertyName))
                    {
                        TableColumn tc = tb.Columns[tcd.ColumnName];
                        this.SetPropertyValue<T>(data, columnMapping[tcd.PropertyName], tr[tc]);
                        result.PropertyCellAddressMapping.Add(columnMapping[tcd.PropertyName].Name, CellAddress.Parse(tc.Position + tb.Address.StartColumn, tr.RowIndex + tb.Address.StartRow + 1).ToString());
                    }
                }

                if (rowValidator != null)
                    rowValidator(result);
            }
        }

        /// <summary>
        /// 根据属性拿逐行读取Table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tb"></param>
        /// <param name="rowValidator"></param>
        internal void ForeachTableRowsByProperties<T>(Table tb, Dictionary<string, PropertyInfo> columnMapping, Action<ExportTableRowContext<T>> rowValidator) where T : class,new()
        {
            foreach (TableRow tr in tb.Rows)
            {
                T data = new T();
                ExportTableRowContext<T> result = new ExportTableRowContext<T>(data) { CurrentTableName = tb.Name };
                foreach (TableColumn tc in tb.Columns)
                {
                    if (columnMapping.ContainsKey(tc.Name))
                    {
                        this.SetPropertyValue<T>(data, columnMapping[tc.Name], tr[tc]);
                        result.PropertyCellAddressMapping.Add(columnMapping[tc.Name].Name, CellAddress.Parse(tc.Position + tb.Address.StartColumn, tr.RowIndex + tb.Address.StartRow + 1).ToString());
                    }
                }
                if (rowValidator != null)
                    rowValidator(result);
            }
        }

        internal string EachTableRows<T>(Func<ExportTableRowContext<T>, string> rowValidator) where T : class,new()
        {
            return this.EachTableRows<T>(SpreadSheetAttributeHelper.GetTableDescription<T>(), rowValidator);
        }

        /// <summary>
        /// 逐行验证处理指定Table数据，并返回所有日制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName"></param>
        /// <param name="rowValidator"></param>
        /// <returns></returns>
        internal string EachTableRows<T>(TableDescription tbDesp, Func<ExportTableRowContext<T>, string> rowValidator) where T : class,new()
        {
            tbDesp.NullCheck("tbDesp");
            this.CheckTableExists(tbDesp.TableName);

            Dictionary<string, PropertyInfo> columnMapping = TypePropertiesCacheQueue.Instance.GetPropertyDictionary(typeof(T));
            Table tb = this.Tables[tbDesp.TableName];

            return this.EachTableRowsByDescriptions<T>(tb, tbDesp, columnMapping, rowValidator);
        }

        private string EachTableRowsByDescriptions<T>(Table tb, TableDescription tbDes, Dictionary<string, PropertyInfo> columnMapping, Func<ExportTableRowContext<T>, string> rowValidator) where T : class,new()
        {
            StringBuilder customLog = new StringBuilder();

            foreach (TableRow tr in tb.Rows)
            {
                T data = new T();
                ExportTableRowContext<T> result = new ExportTableRowContext<T>(data);
                foreach (TableColumnDescription tcDes in tbDes.AllColumns)
                {
                    if (columnMapping.ContainsKey(tcDes.PropertyName) && tb.Columns.ContainsKey(tcDes.ColumnName))
                    {
                        TableColumn tc = tb.Columns[tcDes.ColumnName];
                        this.SetPropertyValue<T>(data, columnMapping[tcDes.PropertyName], tr[tc]);
                        result.PropertyCellAddressMapping.Add(columnMapping[tcDes.PropertyName].Name, CellAddress.Parse(tc.Position + tb.Address.StartColumn, tr.RowIndex + tb.Address.StartRow + 1).ToString());
                    }
                }
                if (rowValidator != null)
                    customLog.Append(rowValidator(result));
            }

            return customLog.ToString();
        }

        private string EachTableRowsByProperties<T>(Table tb, Dictionary<string, PropertyInfo> columnMapping, Func<ExportTableRowContext<T>, string> rowValidator) where T : class,new()
        {
            StringBuilder customLog = new StringBuilder();

            foreach (TableRow tr in tb.Rows)
            {
                T data = new T();
                ExportTableRowContext<T> result = new ExportTableRowContext<T>(data);
                foreach (TableColumn tc in tb.Columns)
                {
                    if (columnMapping.ContainsKey(tc.Name))
                    {
                        this.SetPropertyValue<T>(data, columnMapping[tc.Name], tr[tc]);
                        result.PropertyCellAddressMapping.Add(columnMapping[tc.Name].Name, CellAddress.Parse(tc.Position + tb.Address.StartColumn, tr.RowIndex + tb.Address.StartRow + 1).ToString());
                    }
                }
                if (rowValidator != null)
                    customLog.Append(rowValidator(result));
            }

            return customLog.ToString();
        }
        */
        internal void CheckTableExists(string tableName)
        {
            ExceptionHelper.TrueThrow(string.IsNullOrEmpty(tableName), "工作表名称不能为空", tableName);
            ExceptionHelper.TrueThrow(this.Tables.ContainsKey(tableName) == false, "名称为{0}的工作表不存在", tableName);
        }
        #endregion

        #region "LoadFromDataView"
        public void LoadFromDataView(DataView dv)
        {
            this.LoadFromDataView(CellAddress.Parse("A1"), ExcelTableStyles.Light12, dv, null);
        }

        public void LoadFromDataView(CellAddress beginAddress, ExcelTableStyles tableStyle, DataView dv, CreatingDataCellAction<DataRowView> creatingDataCellAction)
        {
            dv.NullCheck<ArgumentNullException>("数据源不能为空！");

            TableDescription tbDesp = SpreadSheetAttributeHelper.GetTableDescription(dv.Table);
            tbDesp.BeginAddress = beginAddress;

            if (tableStyle == ExcelTableStyles.None && tbDesp.TableName.IsNullOrEmpty())
                this.LoadFromDataView(tbDesp, dv, creatingDataCellAction);
            else
            {
                tbDesp.TableStyle = tableStyle;
                Table table;

                if (this.Tables.TryGetTable(tbDesp.TableName, out table) == false)
                {
                    table = new Table(this, tbDesp);
                    this.Tables.Add(table);
                }

                table.FillData(dv, tbDesp, creatingDataCellAction);
            }
        }

        private void LoadFromDataView(TableDescription tbDesp, DataView dv, CreatingDataCellAction<DataRowView> creatingDataCellAction)
        {
            dv.NullCheck("数据源不能为空");

            int colIndex = tbDesp.BeginAddress.ColumnIndex;
            int rowIndex = tbDesp.BeginAddress.RowIndex;

            this.Names.AddRangeByDescription(tbDesp);

            foreach (DataRowView drv in dv)
            {
                rowIndex++;
                foreach (TableColumnDescription col in tbDesp.AllColumns)
                {
                    if (creatingDataCellAction == null)
                    {
                        Cell cell = this.Cells[rowIndex, colIndex];
                        cell.Value = drv[col.PropertyName];
                        cell.DataType = dv.Table.Columns[col.PropertyName].DataType.ToCellDataType();
                    }
                    else
                        creatingDataCellAction(this.Cells[rowIndex, colIndex], new CreatingDataCellParameters<DataRowView>(drv, drv[col.PropertyName], col.ColumnName, rowIndex));

                    colIndex++;
                }

                colIndex = tbDesp.BeginAddress.ColumnIndex;
            }
        }
        #endregion

        #region 作废的方法
        /*
        /// <summary>
        /// 将DataTable数据输出到当前工作表
        /// </summary>
        /// <param name="beginAddress">开始单元格位置 "A1"</param>
        /// <param name="dv">DataTabel数据源</param>
        /// <param name="printHeaders">是否输出DataTable表头</param>
        [Obsolete("本方法已经作废了，请使用LoadFromDataView(DataView dv)或 LoadFromDataView(CellAddress beginAddress, ExcelTableStyles tableStyle, DataView dv, CreatingDataCellAction<DataRowView> creatingDataCellAction)方法来替换")]
        public void LoadFromDataTable(string beginAddress, DataView dv, bool printHeaders = true)
        {
            dv.NullCheck<ArgumentNullException>("数据源不能为空！");

            CellAddress cell = CellAddress.Parse(beginAddress);

            int col = cell.ColumnIndex, row = cell.RowIndex;

            if (printHeaders)
            {
                foreach (DataColumn dc in dv.Table.Columns)
                {
                    Cell currentCell = this.Cells[row, col];

                    currentCell.Value = dc.Caption.IsNullOrEmpty() ? dc.ColumnName : dc.ColumnName;

                    col++;
                }

                row++;
            }

            foreach (DataRowView dr in dv)
            {
                col = cell.ColumnIndex;

                foreach (DataColumn dc in dv.Table.Columns)
                {
                    this.Cells[row, col].Value = dr[dc.ColumnName];
                    col++;
                }
                row++;
            }
        }

        /// <summary>
        /// 将数据直接直充到Excel表格中
        /// </summary>
        /// <param name="beginAddress">起始位置</param>
        /// <param name="table">DataTable数据源</param>
        /// <param name="tableName">ExcelTable名称</param>
        /// <param name="tableStyle">工作表样式</param>
        /// <param name="printHeaders">是否显示表头</param>
        [Obsolete("本方法已经作废了，请使用LoadFromDataView(DataView dv)或 LoadFromDataView(CellAddress beginAddress, ExcelTableStyles tableStyle, DataView dv, CreatingDataCellAction<DataRowView> creatingDataCellAction)方法来替换")]
        public void LoadFromDataView(string beginAddress, DataView dv, string tableName, ExcelTableStyles tableStyle, bool printHeaders = true)
        {
            CellAddress cell = CellAddress.Parse(beginAddress);
            int rows = dv.Table.Rows.Count + (printHeaders ? 1 : 0) - 1, cols = dv.Table.Columns.Count;

            if (rows > 0 && cols > 0)
            {
                if (tableName.IsNullOrEmpty())
                    tableName = string.Format("Table{0}", this.Tables.Count + 1);

                Table tb = new Table(this, tableName, Range.Parse(this, cell.ColumnIndex, cell.RowIndex, cell.ColumnIndex + cols - 1, cell.RowIndex + rows));
                tb.TableStyle = tableStyle;

                int columnIndex = 0;

                foreach (DataColumn dc in dv.Table.Columns)
                {
                    string columnName = string.IsNullOrEmpty(dc.Caption) ? dc.ColumnName : dc.Caption;
                    TableColumn tc = new TableColumn(tb, columnName, columnIndex);
                    tb.Columns.Add(tc);

                    columnIndex++;
                }

                LoadFromDataTable_CreateRows(tb, dv);
            }
        }

        [Obsolete("本方法已经作废了，请使用LoadFromDataView(DataView dv)或 LoadFromDataView(CellAddress beginAddress, ExcelTableStyles tableStyle, DataView dv, CreatingDataCellAction<DataRowView> creatingDataCellAction)方法来替换")]
        public void LoadFromDataView(CellAddress beginAddress, DataView dv, string tableName, ExcelTableStyles tableStyle)
        {
            if (tableName.IsNullOrEmpty())
                tableName = string.Format("Table{0}", this.Tables.Count + 1);

            Table tb;
            if (this.Tables.TryGetTable(tableName, out tb) == false)
            {
                tb = new Table(this, tableName, beginAddress);
                int columnIndex = 0;

                foreach (DataColumn dc in dv.Table.Columns)
                {
                    string columnName = string.IsNullOrEmpty(dc.Caption) ? dc.ColumnName : dc.Caption;
                    TableColumn tc = new TableColumn(tb, columnName, columnIndex);
                    tb.Columns.Add(tc);
                    columnIndex++;
                }

                this.Tables.Add(tb);
            }

            tb.FillData(dv);
        }
        */
        #endregion 作废的方法

        private void LoadFromDataTable_CreateRows(Table tb, DataView dv)
        {
            int currentColIndex = 0;
            foreach (DataRowView drv in dv)
            {
                TableRow tr = new TableRow(tb.Columns, currentColIndex);

                foreach (TableColumn tc in tb.Columns)
                    tr[tc].Value = drv[tc.Position];

                tb.Rows.Add(tr);
                currentColIndex++;
            }
            this.Tables.Add(tb);
        }

        /// <summary>
        /// 设置合并单元格
        /// </summary>
        /// <param name="rangeAddress"></param>
        public void SetMerge(string rangeAddress)
        {
            Range setAddress = Range.Parse(this, rangeAddress);
            ExceptionHelper.TrueThrow(setAddress.StartColumn == setAddress.EndColumn && setAddress.StartRow == setAddress.EndRow, "已是最小单元格，不能设置合并单元格！");

            for (int i = setAddress.StartColumn; i <= setAddress.EndColumn; i++)
            {
                for (int j = setAddress.StartRow; j <= setAddress.EndRow; j++)
                {
                    this.Cells[j, i].IsMerge = true;
                }
            }

            this._MergeCells.Add(Range.Parse(this, rangeAddress));
        }

        public void InserRows(int rowIndex, int count, int copyStyleIndex)
        {
            this.Rows.Insert(rowIndex, count);
            if (copyStyleIndex != 0)
            {
                if (this.Rows.ContainsKey(copyStyleIndex))
                {
                    Row currentRow = null;
                    Cell currentCell = null;
                    Row copyRow = this.Rows[copyStyleIndex];
                    List<Cell> copyRowList = new List<Cell>();
                    List<int> formulaList = new List<int>();

                    for (int col = this.Dimension.StartColumn; col <= this.Dimension.EndColumn; col++)
                    {
                        copyRowList.Add(this.Cells[copyStyleIndex, col]);
                    }
                    int currentRowIndex;

                    for (int i = 1; i <= count; i++)
                    {
                        currentRowIndex = rowIndex + i;
                        currentRow = this.Rows[currentRowIndex];
                        copyRow.CopyTo(currentRow);

                        foreach (Cell cell in copyRowList)
                        {
                            //if (cell.IsMerge) //todo: 设置合并单元格
                            if (cell.Formula.IsNotEmpty() && !formulaList.Contains(cell.Column.Index))
                            {
                                formulaList.Add(cell.Column.Index);
                                if (this._SharedFormulas.ContainsKey(cell.Formula))
                                    this._SharedFormulas[cell.Formula].Address = Range.Parse(this, cell.Column.Index, currentRowIndex - 1, cell.Column.Index, currentRowIndex + count - 1);
                                else
                                    this.SetFormulas(Range.Parse(this, cell.Column.Index, currentRowIndex - 1, cell.Column.Index, currentRowIndex + count - 1), cell);
                            }
                            else if (cell.Formula.IsNullOrEmpty())
                            {
                                currentCell = new Cell() { Column = cell.Column, Row = currentRow, Style = cell.Style, _Comment = cell._Comment, DataType = cell.DataType };

                                this.Cells.Add(currentCell);
                            }
                        }
                    }
                }
            }
            this.ChangTables(rowIndex, count);
        }

        private void ChangTables(int beginRow, int rowCount)
        {
            foreach (Table table in this.Tables)
            {
                if (beginRow <= table.Address.StartRow)
                    table.Address = Range.Parse(this, table.Address.StartColumn, table.Address.StartRow + rowCount, table.Address.EndColumn, table.Address.EndRow + rowCount);
                else if (beginRow > table.Address.StartRow && beginRow <= table.Address.EndRow)
                    table.Address = Range.Parse(this, table.Address.StartColumn, table.Address.StartRow, table.Address.EndColumn, table.Address.EndRow + rowCount);
            }
        }

        internal void SetSheetDimension(int rowIndex, int colIndex)
        {
            Range current = new Range();
            if (this.Dimension.EndColumn < colIndex)
                current.EndColumn = colIndex;
            else
                current.EndColumn = this.Dimension.EndColumn;

            if (this.Dimension.StartColumn > 0)
            {
                if (this.Dimension.StartColumn > colIndex)
                    current.StartColumn = colIndex;
                else
                    current.StartColumn = this.Dimension.StartColumn;
            }
            else
                current.StartColumn = colIndex;

            if (this.Dimension.StartRow > rowIndex)
                current.StartRow = rowIndex;
            else
            {
                if (this.Dimension.StartRow > 0)
                    current.StartRow = this.Dimension.StartRow;
                else
                    current.StartRow = rowIndex;
            }

            if (this.Dimension.EndRow < rowIndex)
                current.EndRow = rowIndex;
            else
                current.EndRow = this.Dimension.EndRow;

            this.Dimension = current;
        }

        internal int GetAllTablesCount()
        {
            return this.WorkBook.GetTablesCount();
        }

        /// <summary>
        /// 工作表在工作薄是否隐藏 
        /// </summary>
        public ExcelWorksheetHidden Hidden
        {
            get;
            set;
        }

        public Range Dimension
        {
            get;
            set;
        }

        private SheetProtection _SheetProtection = null;
        /// <summary>
        /// 设置工作表的保护
        /// </summary>
        public SheetProtection SheetProtection
        {
            get
            {
                if (this._SheetProtection == null)
                {
                    this._SheetProtection = new SheetProtection();
                }
                return this._SheetProtection;
            }
        }

        internal PrinterSettings _PageSetup = null;
        public PrinterSettings PageSetup
        {
            get
            {
                if (this._PageSetup == null)
                {
                    this._PageSetup = new PrinterSettings(this);
                }
                return this._PageSetup;
            }
        }

        internal PhoneticProperties _PhoneticProperties;
        public PhoneticProperties PhoneticProperties
        {
            get
            {
                if (this._PhoneticProperties == null)
                    this._PhoneticProperties = new PhoneticProperties();

                return this._PhoneticProperties;
            }
        }

        private SheetView _SheetView;
        public SheetView SheetView
        {
            get
            {
                if (this._SheetView == null)
                    this._SheetView = new SheetView(this);

                return this._SheetView;
            }
        }

        //todo:
        // HeaderFooterWrapper
        //PrinterSettingsWrapper

        #region "Save"
        void IPersistable.Save(ExcelSaveContext context)
        {
            SaveChangeTableCell();
            context.LinqWriter.WriteWorkSheet(this);
        }

        private void SaveChangeTableCell()
        {
            int row = 0, col = 0;
            foreach (Table tb in this.Tables)
            {
                tb.InitTableAddress();
                row = tb.Address.StartRow;
                col = tb.Address.StartColumn;

                if (tb.Address.Equals(tb.OldRange) == false && tb.Address.ValidatorRange() && tb.OldRange.ValidatorRange())
                    this.ClearOldTableRangeValues(row, col, tb.OldRange);

                this.SaveChangeTableColumns(row, col, tb);
                this.SaveChangeTableRows(tb, row, col);
            }
        }

        /// <summary>
        /// 清理改运的数据
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="oldRane"></param>
        private void ClearOldTableRangeValues(int row, int col, Range oldRane)
        {
            for (int i = col; i < oldRane.EndColumn; i++)
            {
                for (int j = row; j < oldRane.EndRow; j++)
                {
                    Cell itemCell = this.Cells[j, i];
                    itemCell.Value = null;
                    itemCell.Formula = string.Empty;
                    itemCell._Style = null;
                }
            }
        }

        private void SaveChangeTableColumns(int row, int col, Table tb)
        {
            foreach (TableColumn tc in tb.Columns)
            {
                this.Cells[row, col].Value = tc.Name;
                if (string.IsNullOrEmpty(tc.ColumnFormula) == false)
                {
                    for (int i = row + 1; i <= tb.Address.EndRow; i++)
                    {
                        this.Cells[i, col].Formula = tc.ColumnFormula;
                    }
                }
                col++;
            }
        }

        private void SaveChangeTableRows(Table tb, int row, int col)
        {
            TableCell tcell = null;
            Cell cell = null;
            foreach (TableRow tr in tb.Rows)
            {
                row++;
                col = tb.Address.StartColumn;
                foreach (TableColumn tc in tb.Columns)
                {
                    tcell = tr[tc];
                    if (tcell.IsToSheetCell())
                    {
                        cell = this.Cells[row, col];
                        if (string.IsNullOrEmpty(tc.ColumnFormula))
                            cell.Value = tcell.Value;
                        else
                            cell.Formula = tc.ColumnFormula;

                        if (tcell._Style != null)
                            cell._Style = tcell.Style;
                    }

                    col++;
                }
            }
        }
        #endregion

        #region "Load"
        void IPersistable.Load(ExcelLoadContext context)
        {
            PackageRelationship sheetRelation = context.Package.GetPart(ExcelCommon.Uri_Workbook).GetRelationship(this.RelationshipID);
            this._SheetUri = PackUriHelper.ResolvePartUri(ExcelCommon.Uri_Workbook, sheetRelation.TargetUri);
            var sheet = context.Package.GetXElementFromUri(this._SheetUri);
            context.Reader.ReadWorkSheet(this, sheet);

            loadTableValues();
        }

        private void loadTableValues()
        {
            int row = 0, col = 0;
            foreach (Table tb in this.Tables)
            {
                row = tb.Address.StartRow;
                col = tb.Address.StartColumn;

                row++;

                loadTableRows(tb, row, col);
            }
        }

        private void loadTableRows(Table tb, int row, int col)
        {
            Cell cell = null;
            TableCell tcell = null;
            for (int rowindex = row; rowindex <= tb.Address.EndRow; rowindex++)
            {
                TableRow trow = tb.Rows.NewTableRow();
                foreach (TableColumn tc in tb.Columns)
                {
                    cell = this.Cells[rowindex, col + tc.Position];
                    tcell = trow[tc];
                    tcell.Value = cell.Value;
                    tcell._Style = cell._Style;
                }
            }
        }
        #endregion

        public WorkSheet Clone(string sheetName)
        {
            WorkSheet cloneSheet = new WorkSheet(this.WorkBook, sheetName, this.Hidden)
            {
                _PhoneticProperties = this._PhoneticProperties,
                _PageSetup = this._PageSetup,
                Dimension = this.Dimension,
                _Validations = this._Validations,
                _Drawings = this._Drawings,
                ShowOutlineSymbols = this.ShowOutlineSymbols,
                OutLineApplyStyle = this.OutLineApplyStyle,
                OutLineSummaryBelow = this.OutLineSummaryBelow,
                _HeaderFooter = this._HeaderFooter,
                _TabColor = this._TabColor
            };

            this.CloneData(cloneSheet);

            return cloneSheet;
        }

        private void CloneData(WorkSheet worksheet)
        {
            if (this._Rows != null)
            {
                worksheet._Rows = new ExcelIndexCollection<Row>(worksheet);

                foreach (Row row in this._Rows)
                    worksheet._Rows.Add(row.Clone(row.Index));
            }

            if (this._Columns != null)
            {
                worksheet._Columns = new ExcelIndexCollection<Column>(worksheet);

                foreach (Column column in this._Columns)
                    worksheet._Columns.Add(column.Clone(column.Index));
            }

            if (this._Cells != null)
                worksheet._Cells = this._Cells.Clone(worksheet);

            if (this._SheetView != null)
                worksheet._SheetView = this._SheetView.Clone(worksheet);

            if (this._Tables != null)
                worksheet._Tables = this._Tables.Clone(worksheet);
        }

        internal static ExcelWorksheetHidden TranslateHidden(string value)
        {
            switch (value)
            {
                case "hidden":
                    return ExcelWorksheetHidden.Hidden;
                case "veryHidden":
                    return ExcelWorksheetHidden.VeryHidden;
                default:
                    return ExcelWorksheetHidden.Visible;
            }
        }

        private Uri _SheetUri;
        internal Uri SheetUri
        {
            get
            {
                if (this._SheetUri == null)
                    this._SheetUri = new Uri("/xl/worksheets/sheet" + this.PositionID + 1 + ".xml", UriKind.Relative);

                return this._SheetUri;
            }
            set
            {
                this._SheetUri = value;
            }
        }

    }
}

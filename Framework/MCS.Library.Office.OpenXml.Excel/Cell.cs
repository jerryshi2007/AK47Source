using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Drawing;

namespace MCS.Library.Office.OpenXml.Excel
{
    public class Cell : CellBase
    {
        #region static method
        internal static Cell CreateNewCell(Row row, Column column)
        {
            return new Cell(row, column);
        }
        #endregion

        public Cell() { }

        public Cell(Row row, Column column)
        {
            this.Column = column;
            this.Row = row;
        }

        #region properties

        /// <summary>
        /// 公式
        /// </summary>
        private string _Formula = string.Empty;
        public string Formula
        {
            get { return this._Formula; }
            set
            {
                this._SharedIndex = int.MinValue;
                this._Formula = value;
                this._Value = null;
                this._DataType = "n";
            }
        }

        private bool _hasDataTable = false;
        public bool HasDataTable
        {
            get { return this._hasDataTable; }
            set { _hasDataTable = value; }
        }

        // public string FormulaR1C1 { get; set; }
        private int _SharedIndex = int.MinValue;
        internal int FormulaSharedIndex
        {
            get { return this._SharedIndex; }
            set { this._SharedIndex = value; }
        }

        private string _DataType;

        /// <summary>
        /// CellType
        /// b (Boolean)
        /// d (Date)
        /// e (Error)
        /// inlineStr (Inline String)
        /// n (Number)
        /// s (Shared String)
        /// str (String)
        /// </summary>
        internal string DataType
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

        private int _StyleID = 0;
        /// <summary>
        /// 样式ID
        /// </summary>
        internal int StyleID
        {
            get
            {
                if (this._StyleID > 0)
                {
                    return this._StyleID;
                }
                else if (this.Row.StyleID > 0)
                {
                    return this.Row.StyleID;
                }
                else
                {
                    if (this.Column.StyleID > 0)
                    {
                        return this.Column.StyleID;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            set
            {
                this._StyleID = value;
            }
        }

        public override CellStyleXmlWrapper Style
        {
            get
            {
                if (this._Style == null)
                {
                    if (this.Column._Style != null)
                        this._Style = this.Column._Style;
                    if (this.Row._Style != null)
                        this.Style = this.Row._Style;
                    if (this._Style == null)
                        this._Style = new CellStyleXmlWrapper();
                    //todo:创建默认的
                }
                return this._Style;
            }
            set
            {
                this._Style = value;
            }
        }

        public bool IsRichText { get; set; }

        internal bool IsMerge { get; set; }

        /// <summary>
        /// 该单元格所属列
        /// </summary>
        public Column Column { get; internal set; }

        /// <summary>
        /// 该单元格所属行
        /// </summary>
        public Row Row { get; internal set; }

        internal Comment _Comment;
        /// <summary>
        /// 评论
        /// </summary>
        public Comment Comment
        {
            get
            {
                if (this._Comment == null)
                {
                    this._Comment = new Comment(this);
                }
                return this._Comment;
            }
            set
            {
                this._Comment = value;
            }
        }

        internal Uri _Hyperlink;
        public Uri Hyperlink
        {
            get { return this._Hyperlink; }
            set
            {
                this._Hyperlink = value;

                this.Style.Font.Color.SetColor(Color.Blue);
                this.Style.Font.UnderLine = true;
                if (this.Value == null)
                {
                    if (this._Hyperlink is ExcelHyperLink)
                    {
                        if (string.IsNullOrEmpty((this._Hyperlink as ExcelHyperLink).Display))
                        {
                            this.Value = (this._Hyperlink as ExcelHyperLink).Display;
                        }
                    }
                    else
                    {
                        this.Value = this._Hyperlink.AbsoluteUri;
                    }
                }

            }
        }

        #endregion

        #region method
        public override string ToString()
        {
            return string.Format("{0}{1}", ExcelHelper.GetColumnLetterFromNumber(this.Column.Index), this.Row.Index.ToString(CultureInfo.InvariantCulture));
        }

        public string ToAddress()
        {
            return string.Format("${0}${1}", ExcelHelper.GetColumnLetterFromNumber(this.Column.Index), this.Row.Index.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 检查当前样式是否为空
        /// </summary>
        /// <returns></returns>
        public bool CheckStyleIsEmpty()
        {
            bool result = true;
            if (this._Style != null)
            {
                result = false;
            }

            if (this.Column._Style != null)
            {
                result = false;
            }

            if (this.Row._Style != null)
            {
                result = false;
            }

            return result;
        }
        #endregion

        internal Cell Clone(WorkSheet workSheet)
        {
            Cell cloneCell = new Cell(workSheet.Rows[this.Row.Index], workSheet.Columns[this.Column.Index]);

            cloneCell.IsMerge = this.IsMerge;

            cloneCell.IsRichText = this.IsRichText;
            cloneCell.Formula = this.Formula;
            cloneCell._SharedIndex = this._SharedIndex;
            cloneCell.FormulaSharedIndex = this.FormulaSharedIndex;
            cloneCell.DataType = this.DataType;

            if (this._StyleID != 0)
                cloneCell.StyleID = this.StyleID;

            if (this._Style != null)
                cloneCell._Style = this._Style;

            if (this._Hyperlink != null)
                cloneCell._Hyperlink = this._Hyperlink;

            if (this._Comment != null)
                cloneCell._Comment = this._Comment.Clone(cloneCell);

            cloneCell.Value = this.Value;

            return cloneCell;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Packaging;
using System.Xml;
using System.Xml.Linq;

namespace MCS.Library.Office.OpenXml.Excel
{
    internal class ExcelLoadContext
    {
        public ExcelLoadContext(Package spreadSheetPackage)
        {
            this.Package = spreadSheetPackage;
        }

        /// <summary>
        /// OpenXml SpreadSheet Package
        /// </summary>
        public Package Package { get; set; }

        public ExcelReader Reader { get; set; }

        private List<SharedStringItem> _SharedStrings;

        public List<SharedStringItem> SharedStrings
        {
            get
            {
                if (this._SharedStrings == null)
                {
                    this._SharedStrings = new List<SharedStringItem>();
                }

                return this._SharedStrings;
            }
            set
            {
                _SharedStrings = value;
            }
        }

        private List<DefinedName> _DefinedNames;

        public List<DefinedName> DefinedNames
        {
            get
            {
                if (this._DefinedNames == null)
                {
                    this._DefinedNames = new List<DefinedName>();
                }

                return this._DefinedNames;
            }
        }

        private WorkBookStylesWrapper _GlobalStyles;

        /// <summary>
        /// 工作簿样式
        /// </summary>
        internal WorkBookStylesWrapper GlobalStyles
        {
            get
            {
                if (this._GlobalStyles == null)
                {
                    this._GlobalStyles = new WorkBookStylesWrapper(this.Reader.WorkBook);
                    NumberFormatXmlWrapper.AddBuildIn(this._GlobalStyles.NumberFormats);
                }

                return this._GlobalStyles;
            }
        }

        internal Dictionary<string, CommentCollection> _Comments;

        /// <summary>
        /// 根据工作薄名称，与当前工作表对应评论集合
        /// </summary>
        internal Dictionary<string, CommentCollection> Comments
        {
            get
            {
                if (this._Comments == null)
                {
                    this._Comments = new Dictionary<string, CommentCollection>();
                }
                return this._Comments;
            }
            set
            {
                this._Comments = value;
            }
        }

        private Dictionary<string, List<Comment>> _DrawingShapes;
        public Dictionary<string, List<Comment>> DrawingShapes
        {
            get
            {
                if (this._DrawingShapes == null)
                {
                    this._DrawingShapes = new Dictionary<string, List<Comment>>();
                }
                return this._DrawingShapes;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO.Packaging;
using System.Xml.Linq;

namespace MCS.Library.Office.OpenXml.Excel
{
	internal class ExcelSaveContext
	{
		public Package Package { get; set; }

		public WorkBook WorkBook;

		public ExcelWriter LinqWriter { get; private set; }

		public ExcelSaveContext(WorkBook workbook)
		{
			this.WorkBook = workbook;
			this.LinqWriter = new ExcelWriter() { Context = this, Workbook = workbook };
		}

		private WorkBookStylesWrapper _GlobalStyles;
		/// <summary>
		/// 工作簿样式
		/// </summary>
		public WorkBookStylesWrapper GlobalStyles
		{
			get
			{
				if (this._GlobalStyles == null)
				{
					this._GlobalStyles = new WorkBookStylesWrapper(this.WorkBook);
					NumberFormatXmlWrapper.AddBuildIn(this._GlobalStyles.NumberFormats);

					#region "Fill"
					FillXmlWrapper None_fill = new FillXmlWrapper() { PatternType = ExcelFillStyle.None };
					this._GlobalStyles.Fills.Add(None_fill.Id, None_fill);
					FillXmlWrapper Gray125_fill = new FillXmlWrapper() { PatternType = ExcelFillStyle.Gray125 };
					this._GlobalStyles.Fills.Add(Gray125_fill.Id, Gray125_fill);
					#endregion

					BorderXmlWrapper defaultBorder = new BorderXmlWrapper();
					this._GlobalStyles.Borders.Add(defaultBorder.Id, defaultBorder);

					FontXmlWrapper defaultFont = new FontXmlWrapper() { Size = 11, Name = "宋体" };
					this._GlobalStyles.Fonts.Add(defaultFont.Id, defaultFont);

					CellStyleXmlWrapper defaultCellStyle = new CellStyleXmlWrapper()
					{
						Fill = None_fill,
						Border = defaultBorder,
						Font = defaultFont
					};
					this._GlobalStyles.CellXfs.Add(defaultCellStyle.Id, defaultCellStyle);
				}
				return this._GlobalStyles;
			}
		}

		private Dictionary<string, SharedStringItem> _SharedStrings = new Dictionary<string, SharedStringItem>();
		public Dictionary<string, SharedStringItem> SharedStrings
		{
			get
			{
				return this._SharedStrings;
			}
		}

		private List<DefinedName> _DefinedNames;
		/// <summary>
		/// “名称”定义
		/// </summary>
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

		internal Dictionary<string, List<Comment>> _Comments;
		/// <summary>
		/// 根据工作薄名称，与当前工作表对应评论集合
		/// </summary>
		internal Dictionary<string, List<Comment>> Comments
		{
			get
			{
				if (this._Comments == null)
				{
					this._Comments = new Dictionary<string, List<Comment>>();
				}
				return this._Comments;
			}
			set
			{
				this._Comments = value;
			}
		}

		private Dictionary<string, SortedDictionary<int, SortedDictionary<int, XElement>>> _CommentsDrawing;
		internal Dictionary<string, SortedDictionary<int, SortedDictionary<int, XElement>>> CommentsDrawing
		{
			get
			{
				if (this._CommentsDrawing == null)
				{
					this._CommentsDrawing = new Dictionary<string, SortedDictionary<int, SortedDictionary<int, XElement>>>();
				}
				return this._CommentsDrawing;
			}
		}

		internal Dictionary<string, string> _CommentsSheetRelationships;
		internal Dictionary<string, string> CommentsSheetRelationships
		{
			get
			{
				if (this._CommentsSheetRelationships == null)
				{
					this._CommentsSheetRelationships = new Dictionary<string, string>();
				}
				return this._CommentsSheetRelationships;
			}
		}

		private Dictionary<string, ExcelImageInfo> _DrawingImages;
		public Dictionary<string, ExcelImageInfo> DrawingImages
		{
			get
			{
				if (this._DrawingImages == null)
				{
					this._DrawingImages = new Dictionary<string, ExcelImageInfo>();
				}
				return this._DrawingImages;
			}

		}

		private Dictionary<string, string> _HashImageRelationships;
		public Dictionary<string, string> HashImageRelationships
		{
			get
			{
				if (this._HashImageRelationships == null)
				{
					this._HashImageRelationships = new Dictionary<string, string>();
				}

				return this._HashImageRelationships;
			}
		}

		public int NextTableID = 1;

		/// <summary>
		/// 重置DefinedNames属性为空
		/// </summary>
		public void ResetDefinedNames()
		{
			this._DefinedNames = null;
		}

		/// <summary>
		/// 重置SharedStrings属性为空
		/// </summary>
		public void ResetSharedStrings()
		{
			this._SharedStrings = null;
		}
	}
}

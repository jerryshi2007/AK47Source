using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Drawing;

namespace MCS.Library.Office.OpenXml.Excel
{
	/// <summary>
	/// 评论，批注
	/// </summary>
	public class Comment
	{
		#region "Comment"
		internal Cell _Cell = null;

		public Comment(Cell cell)
		{
			this._Cell = cell;
		}

		internal Comment()
		{ 
		}

		/// <summary>
		/// 作者
		/// </summary>
		public string Author
		{
			get;
			set;
		}

		//internal string _Text = string.Empty;
		/// <summary>
		/// 直接文本输出
		/// </summary>
		public string Text
		{
			get
			{
				return this.RichText.Text;
			}
			set
			{
				this.RichText.Text = value;
			}
		}

		public RichText Font
		{
			get
			{
				if (RichText.Count > 0)
				{
					return RichText[0];
				}
				return null;
			}
		}

		internal RichTextCollection _RichText;
		/// <summary>
		/// 可设置文本样式
		/// </summary>
		public RichTextCollection RichText
		{
			get
			{
				if (this._RichText == null)
				{
					this._RichText = new RichTextCollection(this._Cell);
				}
				return this._RichText;
			}
			set
			{
				this._RichText = value;
			}
		}

		#endregion

		#region "VmlDrawingComment"
		//x:ClientData/x:TextVAlign
		private ExcelTextAlignVerticalVml _VerticalAlignment = ExcelTextAlignVerticalVml.Top;
		/// <summary>
		/// 文本垂直对齐方式
		/// </summary>
		public ExcelTextAlignVerticalVml VerticalAlignment
		{
			get
			{
				return this._VerticalAlignment;
			}
			set
			{
				this._VerticalAlignment = value;
			}
		}


		private ExcelTextAlignHorizontalVml _HorizontalAlignment = ExcelTextAlignHorizontalVml.Left;
		// "x:ClientData/x:TextHAlign";
		/// <summary>
		/// 文本水平对齐方式
		/// </summary>
		public ExcelTextAlignHorizontalVml HorizontalAlignment
		{
			get
			{
				return this._HorizontalAlignment;
			}
			set
			{
				this._HorizontalAlignment = value;
			}
		}

		public string ID
		{
			get;
			set; 
		}

		// "x:ClientData/x:Visible";
		private bool _Visible = false;
		public bool Visible
		{
			get
			{
				return this._Visible;
			}
			set
			{
				if (value)
				{
					this._Style = "visible";
				}
				else
				{
					this._Style = "hidden";
				}
				this._Visible = value;
			}
		}

		private string _Style = string.Empty;
		internal string Style
		{
			get { return this._Style; }
			set { this._Style = value; }
		}

		

		private Color _BackgroundColor =Color.Empty ;
		// "@fillcolor";
		//"v:fill/@color2";
		/// <summary>
		/// Background color
		/// </summary>
		public Color BackgroundColor
		{
			get
			{
				return this._BackgroundColor;
			}
			set
			{
				this._BackgroundColor = value;
			}
		}

		// "v:stroke/@dashstyle";
		// "v:stroke/@endcap";
		private ExcelLineStyleVml _LineStyle = ExcelLineStyleVml.Solid;
		public ExcelLineStyleVml LineStyle
		{
			get
			{
				return this._LineStyle;
			}
			set
			{
				this._LineStyle = value;
			}
		}

		//"@strokecolor";
		private Color _LineColor = Color.Black;
		public Color LineColor
		{
			get
			{
				return this._LineColor;
			}
			set
			{
				this._LineColor = value;
			}
		}

		//"@strokeweight";
		private Single _LineWidth = (Single).75;
		public Single LineWidth
		{
			get
			{
				return this._LineWidth;
			}
			set
			{
				this._LineWidth = value;
			}
		}

		// "v:textbox/@style";
		/// <summary>
		/// 自动调整的绘图对象
		/// </summary>
		public bool AutoFit { get; set; }

		public bool AutoFill { get; set; }

		//"x:ClientData/x:Locked"
		public bool Locked { get; set; }

		//"x:ClientData/x:LockText"
		public bool LockText { get; set; }

		#region "x:ClientData/x:Anchor"

		private string _Anchor = string.Empty;
		internal string Anchor
		{
			get { return this._Anchor; }
			set { this._Anchor = value; }
		}

		internal VmlDrawingPosition _From = null;
		/// <summary>
		/// 当Visible=True时，开始位置
		/// </summary>
		public VmlDrawingPosition From
		{
			get
			{
				if (this._From == null)
				{
					this._From = new VmlDrawingPosition(this,0);
				}
				return this._From;
			}
		}

		internal VmlDrawingPosition _To = null;
		/// <summary>
		/// 当Visible=True时  位置
		/// </summary>
		public VmlDrawingPosition To
		{
			get
			{
				if (this._To == null)
				{
					this._To = new VmlDrawingPosition(this,4);
				}
				return this._To;
			}
		}
		#endregion
		#endregion

		internal Comment Clone(Cell cell)
		{
			Comment cloneComment = new Comment(cell);
			cloneComment._Anchor = this._Anchor;
			cloneComment._BackgroundColor = this._BackgroundColor;
			cloneComment._From = this._From;
			cloneComment._HorizontalAlignment = this._HorizontalAlignment;
			cloneComment._LineColor = this._LineColor;
			cloneComment._LineStyle = this._LineStyle;
			cloneComment._LineWidth = this._LineWidth;
			cloneComment._RichText = this._RichText;
			cloneComment._Style = this._Style;
			cloneComment._To = this._To;
			cloneComment._VerticalAlignment = this._VerticalAlignment;
			cloneComment._Visible = this._Visible;

			return cloneComment;
		}
	}
}

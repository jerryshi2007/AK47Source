using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class RichText
	{
		/*
		 * 
		 */
		public string Text { get; set; }

		/// 保留空白。默认为true
		private bool _PreserveSpace = true;
		/// <summary>
		/// 空格是否处理
		/// </summary>
		public bool PreserveSpace
		{
			get { return this._PreserveSpace; }
			set { this._PreserveSpace = value; }
		}

		/// <summary>
		/// 加粗
		/// </summary>
		public bool Bold
		{
			get;
			set;
		}

		/// <summary>
		/// 效果延伸或伸出文本
		/// </summary>
		public bool Extend
		{
			get;
			set; 
		}

		public bool Condense
		{
			get;
			set; 
		}

		//效果延伸或伸出文本
		public bool Italic
		{
			get;
			set;
		}

		public bool Outline
		{
			get;
			set; 
		}

		internal int _Charset = int.MinValue;
		public int Charset
		{
			get { return this._Charset; }
			set { this._Charset = value; }
		}

		/// <summary>
		/// 该元素通过文本的水平中间绘制删除线线。
		/// </summary>
		public bool Strike
		{
			get;
			set;
		}

		/// <summary>
		/// 带下划线的文本
		/// </summary>
		public bool UnderLine
		{
			get;
			set;
		}

		const string VERT_ALIGN_PATH = "d:rPr/d:vertAlign/@val";
		private ExcelVerticalAlignmentFont _VerticalAlign = ExcelVerticalAlignmentFont.None;
		/// <summary>
		/// 垂直对齐
		/// </summary>
		public ExcelVerticalAlignmentFont VerticalAlign
		{
			get { return this._VerticalAlign; }
			set { this._VerticalAlign = value; }
		}

		const string SIZE_PATH = "d:rPr/d:sz/@val";
		private float _Size = float.MinValue;
		/// <summary>
		/// 字体大小
		/// </summary>
		public float Size
		{
			get
			{
				return this._Size;
			}
			set
			{
				this._Size = value;
			}
		}

		/// <summary>
		/// 字体名称
		/// </summary>
		public string FontName
		{
			get;
			set;
		}

		internal string Scheme
		{
			get;
			set; 
		}

		public bool Shadow
		{
			get;
			set; 
		}

		internal ColorXmlWrapper _DataBarColor = null;
		///数据栏的颜色
		public ColorXmlWrapper DataBarColor
		{
			get
			{
				if (this._DataBarColor == null)
				{
					this._DataBarColor = new ColorXmlWrapper();
				}
				return this._DataBarColor;
			}
			set
			{
				this._DataBarColor = value;
			}
		}
	}
}

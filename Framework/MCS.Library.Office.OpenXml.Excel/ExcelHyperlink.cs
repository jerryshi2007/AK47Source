using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	public sealed class ExcelHyperLink : Uri
	{
		/// <summary>
		/// 一个新的超链接指定的URI
		/// </summary>
		/// <param name="uriString">指定的URI</param>
		public ExcelHyperLink(string uriString) :
			base(uriString)
		{

		}

		/// <summary>
		///  一个新的超链接指定的URI
		/// </summary>
		/// <param name="uriString"></param>
		/// <param name="uriKind"></param>
		public ExcelHyperLink(string uriString, UriKind uriKind) :
			base(uriString, uriKind)
		{

		}
		/// <summary>
		///  一个新的超链接指定的URI
		/// </summary>
		/// <param name="referenceAddress">引用地址</param>
		/// <param name="display">表的内部参考值</param>
		public ExcelHyperLink(string referenceAddress, string display) :
			base("xl://internal")
		{
			this._ReferenceAddress = referenceAddress;
			this._Display = display;
		}

		private string _ReferenceAddress = string.Empty;
		public string ReferenceAddress
		{
			get
			{
				return this._ReferenceAddress;
			}
			set
			{
				this._ReferenceAddress = value;
			}
		}

		private string _Display = string.Empty;
		/// <summary>
		/// 显示的文字
		/// </summary>
		public string Display
		{
			get
			{
				return this._Display;
			}
			set
			{
				this._Display = value;
			}
		}

		/// <summary>
		/// 提示
		/// </summary>
		public string ToolTip
		{
			get;
			set;
		}

		private int _ColSpann = 0;
		/// <summary>
		/// 如果超链接跨越多列
		/// </summary>
		public int ColSpann
		{
			get
			{
				return this._ColSpann;
			}
			set
			{
				this._ColSpann = value;
			}
		}

		private int _RowSpann = 0;
		/// <summary>
		/// 如果超链接跨越多行
		/// </summary>
		public int RowSpann
		{
			get
			{
				return this._RowSpann;
			}
			set
			{
				this._RowSpann = value;
			}
		}
	}
}

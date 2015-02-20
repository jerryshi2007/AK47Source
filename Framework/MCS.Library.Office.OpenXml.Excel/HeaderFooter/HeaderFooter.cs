using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	/// <summary>
	/// 表示在Excel工作表的页眉和页脚
	/// </summary>
	public sealed class HeaderFooter
	{
		private WorkSheet _WorkSheet;

		public HeaderFooter(WorkSheet wroksheet)
		{
			this._WorkSheet = wroksheet;
		}


		//@differentOddEven
		/// <summary>
		/// 设置文档的奇数页的页眉
		/// </summary>
		public bool DifferentOddEven
		{
			get;
			set;
		}

		//@differentFirst
		/// <summary>
		/// 获取或设置Excel第一页显示不同的页眉页脚
		/// </summary>
		public bool DifferentFirst
		{
			get;
			set;
		}

		//default="true"  @alignWithMargins
		private bool _AlignWithMargins = true;
		public bool AlignWithMargins
		{
			get { return this._AlignWithMargins; }
			set { this._AlignWithMargins = value; }
		}

		//default="true"/  @scaleWithDoc
		private bool _ScaleWithDoc = true;
		public bool ScaleWithDoc
		{
			get { return this._ScaleWithDoc; }
			set { this._ScaleWithDoc = value; }
		}


		internal HeaderFooterText _OddHeader;
		/// <summary>
		/// 提供访问文档的奇数页的页眉。
		/// 如果你想奇数和偶数页上都相同的标题，然后在这个HeaderFooterText类只设置值。
		/// </summary>
		public HeaderFooterText OddHeader
		{
			get
			{
				if (this._OddHeader == null)
					this._OddHeader = new HeaderFooterText(this, "H");

				return this._OddHeader;
			}
		}

		internal HeaderFooterText _OddFooter;
		/// <summary>
		/// 提供访问文档的奇数页的页脚
		/// </summary>
		public HeaderFooterText OddFooter
		{
			get
			{
				if (this._OddFooter == null)
					this._OddFooter = new HeaderFooterText(this, "F");

				return this._OddFooter;
			}
		}


		internal HeaderFooterText _EvenHeader;
		/// <summary>
		/// 提供该文件的偶数页眉
		/// </summary>
		public HeaderFooterText EvenHeader
		{
			get
			{
				if (this._EvenHeader == null)
				{
					this._EvenHeader = new HeaderFooterText(this, "CHEVEN");
					this.DifferentOddEven = true;
				}

				return this._EvenHeader;
			}
		}

		internal HeaderFooterText _EvenFooter;
		/// <summary>
		/// 访问文档的偶数页页脚。
		/// </summary>
		public HeaderFooterText EvenFooter
		{
			get
			{
				if (this._EvenFooter == null)
				{
					this._EvenFooter = new HeaderFooterText(this, "FEVEN");
					this.DifferentOddEven = true;
				}

				return this._EvenFooter;
			}
		}


		internal HeaderFooterText _FirstHeader;
		/// <summary>
		/// 第一页页眉
		/// </summary>
		public HeaderFooterText FirstHeader
		{
			get
			{
				if (this._FirstHeader == null)
				{
					this._FirstHeader = new HeaderFooterText(this, "HFIRST");
					this.DifferentFirst = true;
				}

				return _FirstHeader;
			}
		}

		internal HeaderFooterText _FirstFooter;
		/// <summary>
		///第一页页脚
		/// </summary>
		public HeaderFooterText FirstFooter
		{
			get
			{
				if (this._FirstFooter == null)
				{
					this._FirstFooter = new HeaderFooterText(this, "FFIRST");
					this.DifferentFirst = true;
				}

				return this._FirstFooter;
			}
		}

		internal VmlDrawingPictureCollection _Pictures = null;
		public VmlDrawingPictureCollection Pictures
		{
			get
			{
				if (this._Pictures == null)
					this._Pictures = new VmlDrawingPictureCollection(this._WorkSheet);
				
				return this._Pictures;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using System.Text.RegularExpressions;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class DefinedName
	{
		public static readonly Regex NameRangeReferenceRegex =
		   new Regex(@"^('?(?<Sheet>[^'!]+)'?!(?<Range>.+))|((?<Table>[^\[]+)\[(?<Column>[^\]]+)\])$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);

		internal WorkSheet _WorkSheet;

		internal DefinedName(string name, WorkSheet worksheet)
		{
			this.Name = name;
			this._WorkSheet = worksheet;
		}

		public string Name
		{
			get;
			set;
		}

		public Range Address { get; internal set; }

		private int _LocalSheetId = int.MinValue;
		/// <summary>
		/// “名称”所属工作簿
		/// </summary>
		public int LocalSheetId
		{
			get { return this._LocalSheetId; }
			set { this._LocalSheetId = value; }
		}

		/// <summary>
		/// 名称隐藏
		/// </summary>
		public bool IsNameHidden
		{
			get;
			set;
		}

		/// <summary>
		///评论名称
		/// </summary>
		public string NameComment
		{
			get;
			set;
		}

		/// <summary>
		/// 对应的值，目前只支持单个单元格
		/// </summary>
		public object NameValue
		{
			get
			{
				return this._WorkSheet.Cells[this.Address.StartRow, this.Address.StartColumn];
			}
			set
			{
				this._WorkSheet.Cells[this.Address.StartRow, this.Address.StartColumn].Value = value;
			}
		}

		internal string NameFormula { get; set; }
	}
}

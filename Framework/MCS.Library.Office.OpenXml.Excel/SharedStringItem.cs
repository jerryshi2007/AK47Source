using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	public struct SharedStringItem
	{
		internal int Pos { get; set; }
		internal string Text { get; set; }
		internal bool IsRichText { get; set; }
	}
}

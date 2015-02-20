using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	/// <summary>
	/// WorkBook 相关属性
	/// </summary>
	public class WorkBookProperties : ElementInfo
	{
		protected internal override string NodeName
		{
			get { return "workbookPr"; }
		}
	}
}

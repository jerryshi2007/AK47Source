using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class SheetTableStyles : ElementInfo
	{
		public int Count
		{
			get
			{
				string le = base.GetAttribute("count");
				if (string.IsNullOrEmpty(le) == false)
				{
					return int.Parse(le);
				}
				return 0;
			}
			set
			{
				base.SetAttribute("count", value.ToString());
			}
		}

		public string DefaultPivotStyle
		{
			get
			{
				return base.GetAttribute("defaultPivotStyle");
			}
			set
			{
				base.SetAttribute("defaultPivotStyle", value);
			}
		}

		public string DefaultTableStyle
		{
			get
			{
				return base.GetAttribute("defaultTableStyle");
			}
			set
			{
				base.SetAttribute("defaultTableStyle", value);
			}
		}

		protected internal override string NodeName
		{
			get { return "tableStyles"; }
		}


	}
}

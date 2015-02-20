using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	public class CalculationCell
	{
		public string Cell { get; internal set; }//todo:

		public string WorkSheet { get; internal set; }//todo:

		public string NewLevel { get; internal set; }//todo:BOOL

		public string InChildChain { get; internal set; }//todo:BOOL
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Packaging;

namespace MCS.Library.Office.OpenXml.Excel
{
	internal sealed class ExcelImageInfo
	{
		internal string Hash { get; set; }
		internal Uri Uri { get; set; }
		internal int RefCount { get; set; }
		//internal PackagePart Part { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Office.OpenXml.Excel
{
	internal interface IPersistable
	{
		void Save(ExcelSaveContext context);
		void Load(ExcelLoadContext context);
	}
}

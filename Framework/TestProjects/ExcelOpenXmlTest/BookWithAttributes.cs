using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Office.OpenXml.Excel;

namespace ExcelOpenXmlTest
{
	public class BookWithAttributes : BookBase
	{
		[TableColumnDescription("书名")]
		public override string Name
		{
			get;
			set;
		}

		[TableColumnDescription("价格")]
		public override Double Price
		{
			get;
			set;
		}

		[TableColumnDescription("发行日期")]
		public override DateTime IssueDate
		{
			get;
			set;
		}
	}
}

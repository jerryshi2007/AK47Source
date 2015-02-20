using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelOpenXmlTest
{
	public class BookBase
	{
		public virtual string Name
		{
			get;
			set;
		}

		public virtual Double Price
		{
			get;
			set;
		}

		public virtual DateTime IssueDate
		{
			get;
			set;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Web.Responsive.WebControls
{
	public class Define
	{
		public static string DefaultCulture = "SOAWebControls";
		//public const string DefaultCategory = "SOAWebControls";
	}

	/// <summary>
	/// 字段类型
	/// </summary>
	public enum DataType
	{
		Object = 1,
		Boolean = 3,
		Integer = 9,
		Decimal = 15,
		DateTime = 16,
		String = 18,
		Enum = 20
	}
}

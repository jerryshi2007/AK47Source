using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MCS.Library.SOA.Web.WebControls.Test.PropertyForm
{
	public class PropertyFormDemoSource
	{
		public DataTable GetPropertiesTable()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add(new DataColumn("ColName") { DataType = typeof(string) });
			dt.Columns.Add(new DataColumn("ColVale") { DataType = typeof(Int32) });

			for (int i = 0; i < 50; i++)
			{
				DataRow dr = dt.NewRow();
				dr["ColVale"] = i;
				dr["ColName"] = string.Format("ColName{0}", i);
				dt.Rows.Add(dr);
			}

			return dt;
		}

		public IList<DropdownLitsItem> GetPropertiesList()
		{
			List<DropdownLitsItem> result = new List<DropdownLitsItem>();

			for (int i = 0; i < 30; i++)
			{
				DropdownLitsItem item = new DropdownLitsItem() { ID = i, Name = string.Format("Name{0}", i), Caption = string.Format("Caption{0}", i) };
				result.Add(item);
			}

			return result;
		}
	}

	public class DropdownLitsItem
	{
		public int ID { get; set; }

		public string Name { get; set; }

		public string Caption { get; set; }
	}
}
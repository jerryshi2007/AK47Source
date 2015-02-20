using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter.Tables
{
	public class OguOrganizationTableBuilder : OguObjectTableBuilder
	{
		protected override DataTable CreateTable()
		{
			DataTable table = base.CreateTable();

			table.Columns.Add("CHILDREN_COUNTER", typeof(int));
			table.Columns.Add("CREATE_TIME", typeof(DateTime));
			table.Columns.Add("MODIFY_TIME", typeof(DateTime));
			table.Columns.Add("SYSDISTINCT1", typeof(string));
			table.Columns.Add("SYSDISTINCT2", typeof(string));
			table.Columns.Add("SYSCONTENT1", typeof(string));
			table.Columns.Add("SYSCONTENT2", typeof(string));
			table.Columns.Add("SYSCONTENT3", typeof(string));

			table.Columns.Add("SORT_ID", typeof(int));
			table.Columns.Add("NAME", typeof(string));
			table.Columns.Add("VISIBLE", typeof(int));

			table.Columns.Add("RANK_CLASS", typeof(int));

			return table;
		}
	}
}
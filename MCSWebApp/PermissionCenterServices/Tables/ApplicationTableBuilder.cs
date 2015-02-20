using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter.Tables
{
	public class ApplicationTableBuilder : AppObjectTableBuilderBase
	{
		protected override DataTable CreateTable()
		{
			DataTable table = base.CreateTable();

			table.Columns.Add("RESOURCE_LEVEL", typeof(string));
			table.Columns.Add("CHILDREN_COUNT", typeof(int));
			table.Columns.Add("ADD_SUBAPP", typeof(string));
			table.Columns.Add("USE_SCOPE", typeof(string));
			table.Columns.Add("INHERITED_STATE", typeof(int));

			return table;
		}

		protected override void FillPropertiesToTable(SCBase obj, int index, DataRow row)
		{
			base.FillPropertiesToTable(obj, index, row);
		}
	}
}
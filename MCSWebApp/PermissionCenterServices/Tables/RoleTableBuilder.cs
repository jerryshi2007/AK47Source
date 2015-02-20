using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter.Tables
{
	public class RoleTableBuilder : AppElementTableBuilderBase
	{
		protected override DataTable CreateTable()
		{
			DataTable table = base.CreateTable();

			table.Columns.Add("ALLOW_DELEGATE", typeof(string));

			return table;
		}

		protected override void FillPropertiesToTable(SCBase obj, int index, DataRow row)
		{
			base.FillPropertiesToTable(obj, index, row);
		}
	}
}
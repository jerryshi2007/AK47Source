using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter.Tables
{
	/// <summary>
	/// 创建应用所包含的对象表的基类，包括角色和权限的基类
	/// </summary>
	public abstract class AppElementTableBuilderBase : AppObjectTableBuilderBase
	{
		protected override DataTable CreateTable()
		{
			DataTable table = base.CreateTable();

			table.Columns.Add("APP_ID", typeof(string));
			table.Columns.Add("CLASSIFY", typeof(string));
			table.Columns.Add("INHERITED", typeof(string));

			return table;
		}

		protected override void FillPropertiesToTable(SCBase obj, int index, DataRow row)
		{
			base.FillPropertiesToTable(obj, index, row);
		}
	}
}
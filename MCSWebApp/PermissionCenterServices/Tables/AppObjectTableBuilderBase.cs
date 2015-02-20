using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter.Tables
{
	/// <summary>
	/// 创建应用授权对象表的基类
	/// </summary>
	public class AppObjectTableBuilderBase
	{
		/// <summary>
		/// 将SchemaObject转换为DataTable
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public DataTable Convert(SchemaObjectCollection objs)
		{
			DataTable table = CreateTable();

			for (int i = 0; i < objs.Count; i++)
			{
				SCBase obj = (SCBase)objs[i];

				DataRow row = table.NewRow();
				FillPropertiesToTable(obj, i, row);

				table.Rows.Add(row);
			}

			return table;
		}

		protected virtual DataTable CreateTable()
		{
			DataTable table = new DataTable();

			table.Columns.Add("ID", typeof(string));
			table.Columns.Add("NAME", typeof(string));
			table.Columns.Add("CODE_NAME", typeof(string));
			table.Columns.Add("DESCRIPTION", typeof(string));
			table.Columns.Add("SORT_ID", typeof(int));

			return table;
		}

		protected virtual void FillPropertiesToTable(SCBase obj, int index, DataRow row)
		{
			row["ID"] = obj.ID;
			row["NAME"] = obj.Name;
			row["CODE_NAME"] = obj.CodeName;
			row["DESCRIPTION"] = obj.Properties.GetValue("Description", string.Empty);
			row["SORT_ID"] = index.ToString();

            if (obj.Tag.IsNotEmpty())
                row["APP_ID"] = obj.Tag;
		}
	}
}
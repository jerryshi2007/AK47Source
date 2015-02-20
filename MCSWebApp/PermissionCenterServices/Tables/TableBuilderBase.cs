using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MCS.Library.SOA.DataObjects.Security;

namespace PermissionCenter.Tables
{
	/// <summary>
	/// DataTable创建器的虚基类
	/// </summary>
	public abstract class TableBuilderBase
	{
		/// <summary>
		/// 创建一个DataTable
		/// </summary>
		/// <returns></returns>
		protected abstract DataTable CreateTable();

		/// <summary>
		/// 将对象的属性填充到DataTable中
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="table"></param>
		protected abstract void FillPropertiesToTable(SCObjectAndRelation obj, DataRow row);

		/// <summary>
		/// 将SchemaObject转换为DataTable
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public DataTable Convert(SCObjectAndRelationCollection relations)
		{
			DataTable table = CreateTable();

			relations.FillDetails();

			foreach (SCObjectAndRelation obj in relations)
			{
				DataRow row = table.NewRow();
				FillPropertiesToTable(obj, row);

				table.Rows.Add(row);
			}

			return table;
		}
	}
}
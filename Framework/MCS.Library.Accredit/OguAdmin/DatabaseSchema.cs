using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Transactions;

using MCS.Library.Data;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Accredit.Properties;

namespace MCS.Library.Accredit.OguAdmin
{
#if DELUXEWORKSTEST
	/// <summary>
	/// 
	/// </summary>
	public sealed class DatabaseSchema
#else
	internal sealed class DatabaseSchema
#endif
	{
		private DatabaseSchema()
		{
			InitDatasetSchema();
		}

		public static readonly DatabaseSchema Instence = new DatabaseSchema();
#if DELUXEWORKSTEST
		/// <summary>
		/// 
		/// </summary>
		public DataSet dataSetSchema = null;
#else
		private DataSet dataSetSchema = null;
#endif
		public DataSet Schema
		{
			get
			{
				if (this.dataSetSchema == null)
					InitDatasetSchema();

				return this.dataSetSchema;
			}
		}

		private void InitDatasetSchema()
		{
			string[] allTables = new string[] { 
				CommonResource.Table_Org, 
				CommonResource.Table_Group, 
				CommonResource.Table_User, 
				CommonResource.Table_OuUser,  
				CommonResource.Table_GroupUser, 
				CommonResource.Table_RankDefine,
				CommonResource.Table_Secretary};

			StringBuilder builder = new StringBuilder(512);
			for (int i = 0; i < allTables.Length; i++)
			{
				//builder.Append(" SELECT SYSOBJECTS.NAME AS TNAME, SYSCOLUMNS.NAME AS CNAME ");
				//builder.Append(" FROM SYSOBJECTS, SYSCOLUMNS ");
				//builder.Append(" WHERE SYSOBJECTS.ID = SYSCOLUMNS.ID ");
				//builder.Append(" AND SYSOBJECTS.NAME IN (" + TSqlBuilder.Instance.CheckQuotationMark(allTables[i], true) + ")");
				//builder.Append(" ORDER BY TNAME, SYSCOLUMNS.COLID;" + Environment.NewLine);

				builder.Append(" SELECT TABLE_CATALOG,TABLE_SCHEMA,TABLE_NAME,COLUMN_NAME,ORDINAL_POSITION,COLUMN_DEFAULT,");
				builder.Append("IS_NULLABLE,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,CHARACTER_OCTET_LENGTH,NUMERIC_PRECISION,");
				builder.Append("NUMERIC_PRECISION_RADIX,NUMERIC_SCALE,DATETIME_PRECISION,CHARACTER_SET_CATALOG,CHARACTER_SET_SCHEMA,");
				builder.Append("CHARACTER_SET_NAME,COLLATION_CATALOG,COLLATION_SCHEMA,COLLATION_NAME,DOMAIN_CATALOG,DOMAIN_SCHEMA,DOMAIN_NAME ");
				builder.Append(" FROM INFORMATION_SCHEMA.COLUMNS ");
				builder.Append(string.Format(" WHERE TABLE_NAME ={0} ORDER BY ORDINAL_POSITION; ",
					TSqlBuilder.Instance.CheckQuotationMark(allTables[i], true)));
			}

			using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
			{
				Database database = DatabaseFactory.Create(context);
				this.dataSetSchema = database.ExecuteDataSet(CommandType.Text, builder.ToString(), allTables);
			}
		}

		/// <summary>
		/// 获取指定Table的Schema
		/// </summary>
		/// <param name="attrs"></param>
		/// <param name="tableNames"></param>
		/// <returns></returns>
		public string GetTableColumns(string attrs, params string[] tableNames)
		{
			StringBuilder builder = new StringBuilder(1024);
			ExceptionHelper.CheckStringIsNullOrEmpty(attrs, "attrs");
			ExceptionHelper.FalseThrow<ArgumentException>(tableNames.Length > 0 && !string.IsNullOrEmpty(tableNames[0]), "tables");

			string[] arrAttrs = attrs.Split(',', ';');
			for (int i = 0; i < arrAttrs.Length; i++)
			{
				if (builder.Length > 0)
					builder.Append(", ");

				if (arrAttrs[i].Trim() == "*")
					builder.Append(tableNames[i] + "." + arrAttrs[i]);
				else
				{
					bool HasComplicated = false;

					for (int j = 0; j < tableNames.Length; j++)
					{
						DataTable table = this.Schema.Tables[tableNames[j]];

						DataRow[] rows = table.Select("COLUMN_NAME=" + TSqlBuilder.Instance.CheckQuotationMark(arrAttrs[i].Trim(), true));

						if (rows.Length > 0)
						{
							builder.Append((string)rows[0]["TABLE_NAME"] + "." + arrAttrs[i]);
							HasComplicated = true;
							break;
						}
					}

					if (false == HasComplicated)
						builder.Append(" NULL AS " + arrAttrs[i]);
				}
			}
			return builder.ToString();
		}

		/// <summary>
		/// 判断数据表结构中是否存在有所有指定字段
		/// </summary>
		/// <param name="attrs"></param>
		/// <param name="tableNames"></param>
		/// <returns></returns>
		public bool CheckTableColumns(string attrs, params string[] tableNames)
		{ 
			string[] arrAttr = attrs.Split(',', ';');
			bool[] isExists = new bool[arrAttr.Length];

			for (int i = 0; i < arrAttr.Length; i++)
			{
				if (arrAttr[i].Trim() == "*")
				{
					isExists[i] = true;
				}
				else
				{
					for (int j = 0; j < tableNames.Length; j++)
					{
						DataTable table = this.Schema.Tables[tableNames[j]];
						if (table.Select("COLUMN_NAME=" + TSqlBuilder.Instance.CheckQuotationMark(arrAttr[i].Trim(), true)).Length > 0)
						{
							isExists[i] = true;
							break;
						}
					}
				}
			}
			foreach (bool bol in isExists)
			{
				if (bol == false)
					return false;
			}
			return true;
		}
	}
}

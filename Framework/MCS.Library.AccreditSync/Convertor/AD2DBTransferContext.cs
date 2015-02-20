using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;
using MCS.Library.Data;
using MCS.Library.Data.Builder;

namespace MCS.Library.Accredit
{
	internal class AD2DBTransferContext
	{
		public DataSet OriginalData = new DataSet();
		public DataSet ADData = new DataSet();

		public DataView ADOguAllPathView = null;
		public DataView ADOguGuidView = null;

		public DataView ADUsersGuidView = null;

		public DataView UsersGuidView = null;

		public DataView OriginalOguGuidView = null;
		public DataView OriginalOguParentGuidView = null;
		public DataView OriginalOuUserParentGuidView = null;
		public DataView OriginalOUUserParentUserGuidView = null;
		public DataView OriginalOUUserUserGuidView = null;
		public DataView OriginalUserExtendGuidView = null;

		public DataView ADOuUserParentGuidView = null;
		public DataView ADUserExtendGuidView = null;

		public string ParentGuid = string.Empty;
		public string ParentOriginalSort = string.Empty;
		public int ParentChildrenCount = 0;

		public int OrganizationsConverted = 0;
		public int UsersConverted = 0;

		private AD2DBInitialParams initialParams = null;

		public AD2DBTransferContext(AD2DBInitialParams initParams)
		{
			this.initialParams = initParams;
		}

		public void InitContext()
		{
			using (DbContext context = DbContext.GetContext(this.initialParams.AccreditAdminConnectionName))
			{
				FillData(this.OriginalData, "SELECT * FROM USERS", "USERS", context);
				FillData(this.OriginalData, "SELECT * FROM OU_USERS ORDER BY ALL_PATH_NAME", "OU_USERS", context);
				FillData(this.OriginalData, "SELECT * FROM ORGANIZATIONS ORDER BY ALL_PATH_NAME", "ORGANIZATIONS", context);
			}

			using (DbContext context = DbContext.GetContext(this.initialParams.UserInfoExtend))
			{
				FillData(this.OriginalData, "SELECT * FROM USERS_INFO_EXTEND", "USERS_INFO_EXTEND", context);
			}

			CloneDataSetSchema(OriginalData, ADData);

			InitRootOURow(
				OriginalData.Tables["ORGANIZATIONS"],
				ADData.Tables["ORGANIZATIONS"],
				ADToDBConfigSettings.GetConfig().RootOUName);

			DataRow newRow = InitRootOURow(
				OriginalData.Tables["ORGANIZATIONS"],
				ADData.Tables["ORGANIZATIONS"],
				AD2DBHelper.TranslateDNToFullPath(this.initialParams.Root.Properties["distinguishedName"].Value.ToString()));

			if (newRow != null)
				newRow["CHILDREN_COUNTER"] = 0;

			ADOguAllPathView = new DataView(ADData.Tables["ORGANIZATIONS"]);
			ADOguAllPathView.Sort = "ALL_PATH_NAME";

			ADOguGuidView = new DataView(ADData.Tables["ORGANIZATIONS"]);
			ADOguGuidView.Sort = "GUID";

			OriginalOguGuidView = new DataView(OriginalData.Tables["ORGANIZATIONS"]);
			OriginalOguGuidView.Sort = "GUID";

			OriginalOguParentGuidView = new DataView(OriginalData.Tables["ORGANIZATIONS"]);
			OriginalOguParentGuidView.Sort = "PARENT_GUID";

			OriginalOuUserParentGuidView = new DataView(OriginalData.Tables["OU_USERS"]);
			OriginalOuUserParentGuidView.Sort = "PARENT_GUID";

			ADUsersGuidView = new DataView(ADData.Tables["USERS"]);
			ADUsersGuidView.Sort = "GUID";

			UsersGuidView = new DataView(OriginalData.Tables["USERS"]);
			UsersGuidView.Sort = "GUID";

			ADOuUserParentGuidView = new DataView(ADData.Tables["OU_USERS"]);
			ADOuUserParentGuidView.Sort = "USER_GUID,PARENT_GUID";

			ADUserExtendGuidView = new DataView(ADData.Tables["USERS_INFO_EXTEND"]);
			ADUserExtendGuidView.Sort = "ID";

			OriginalUserExtendGuidView = new DataView(OriginalData.Tables["USERS_INFO_EXTEND"]);
			OriginalUserExtendGuidView.Sort = "ID";

			OriginalOUUserParentUserGuidView = new DataView(OriginalData.Tables["OU_USERS"]);
			OriginalOUUserParentUserGuidView.Sort = "USER_GUID,PARENT_GUID";

			OriginalOUUserUserGuidView = new DataView(OriginalData.Tables["OU_USERS"]);
			OriginalOUUserUserGuidView.Sort = "USER_GUID";
		}

		public AD2DBInitialParams InitialParams
		{
			get { return this.initialParams; }
		}

		private static DataRow InitRootOURow(DataTable srcTable, DataTable destTable, string fullPath)
		{
			DataRow[] rows = srcTable.Select("ALL_PATH_NAME = " + TSqlBuilder.Instance.CheckQuotationMark(fullPath, true));

			DataRow newRow = null;

			if (rows.Length > 0)
			{
				newRow = destTable.NewRow();

				foreach (DataColumn column in srcTable.Columns)
					newRow[column.ColumnName] = rows[0][column.ColumnName];

				destTable.Rows.Add(newRow);
			}

			return newRow;
		}

		private static void CloneDataSetSchema(DataSet srcDs, DataSet destDs)
		{
			destDs.Tables.Clear();

			foreach (DataTable srcTable in srcDs.Tables)
				destDs.Tables.Add(srcTable.Clone());
		}

		private static void FillData(DataSet ds, string sql, string tableName, DbContext context)
		{
			Database db = DatabaseFactory.Create(context);

			db.LoadDataSet(CommandType.Text, sql, ds, tableName);
		}
	}
}

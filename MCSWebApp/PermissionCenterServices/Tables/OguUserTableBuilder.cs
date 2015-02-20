using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;

namespace PermissionCenter.Tables
{
	public class OguUserTableBuilder : OguObjectTableBuilder
	{
		protected override DataTable CreateTable()
		{
			DataTable table = base.CreateTable();

			table.Columns.Add("START_TIME", typeof(DateTime));
			table.Columns.Add("END_TIME", typeof(DateTime));
			table.Columns.Add("MODIFY_TIME", typeof(DateTime));
			table.Columns.Add("CREATE_TIME", typeof(DateTime));

			table.Columns.Add("OUSYSDISTINCT1", typeof(string));
			table.Columns.Add("OUSYSDISTINCT2", typeof(string));
			table.Columns.Add("OUSYSCONTENT1", typeof(string));
			table.Columns.Add("OUSYSCONTENT2", typeof(string));
			table.Columns.Add("OUSYSCONTENT3", typeof(string));

			table.Columns.Add("FIRST_NAME", typeof(string));
			table.Columns.Add("LAST_NAME", typeof(string));

			table.Columns.Add("IC_CARD", typeof(string));
			table.Columns.Add("PWD_TYPE_GUID", typeof(string));
			table.Columns.Add("USER_PWD", typeof(string));

			table.Columns.Add("CREATE_TIME1", typeof(DateTime));
			table.Columns.Add("MODIFY_TIME1", typeof(DateTime));

			table.Columns.Add("AD_COUNT", typeof(int));

			table.Columns.Add("SYSDISTINCT1", typeof(string));
			table.Columns.Add("SYSDISTINCT2", typeof(string));
			table.Columns.Add("SYSCONTENT1", typeof(string));
			table.Columns.Add("SYSCONTENT2", typeof(string));
			table.Columns.Add("SYSCONTENT3", typeof(string));

			table.Columns.Add("PINYIN", typeof(string));
			table.Columns.Add("SORT_ID", typeof(int));
			table.Columns.Add("NAME", typeof(string));
			table.Columns.Add("VISIBLE", typeof(int));

			table.Columns.Add("RANK_CLASS", typeof(int));

			table.Columns.Add("AccountDisabled", typeof(Boolean));
			table.Columns.Add("PasswordNotRequired", typeof(Boolean));
			table.Columns.Add("DontExpirePassword", typeof(Boolean));
			table.Columns.Add("AccountInspires", typeof(DateTime));
			table.Columns.Add("AccountExpires", typeof(DateTime));
			table.Columns.Add("Address", typeof(string));

			table.Columns.Add("MP", typeof(string));
			table.Columns.Add("WP", typeof(string));

			table.Columns.Add("OtherMP", typeof(string));
			table.Columns.Add("CompanyName", typeof(string));
			table.Columns.Add("DepartmentName", typeof(string));
			table.Columns.Add("PhotoTimestamp", typeof(string));
			table.Columns.Add("Sip", typeof(string));
			
			return table;
		}

		protected override void FillPropertiesToTable(SCObjectAndRelation obj, DataRow row)
		{
			base.FillPropertiesToTable(obj, row);

			row["FIRST_NAME"] = obj.Detail.Properties.GetValue("FirstName", string.Empty);
			row["LAST_NAME"] = obj.Detail.Properties.GetValue("LastName", string.Empty);

			row["AccountDisabled"] = obj.Detail.Properties.GetValue("AccountDisabled", false);
			row["PasswordNotRequired"] = obj.Detail.Properties.GetValue("PasswordNotRequired", false);
			row["DontExpirePassword"] = obj.Detail.Properties.GetValue("DontExpirePassword", false);

			row["AccountInspires"] = obj.Detail.Properties.GetValue("AccountInspires", DateTime.MinValue);
			row["AccountExpires"] = obj.Detail.Properties.GetValue("AccountExpires", DateTime.MinValue);

			row["Address"] = obj.Detail.Properties.GetValue("Address", string.Empty);
			row["MP"] = obj.Detail.Properties.GetValue("MP", string.Empty);
			row["WP"] = obj.Detail.Properties.GetValue("WP", string.Empty);
			row["Sip"] = obj.Detail.Properties.GetValue("Sip", string.Empty);

			row["OtherMP"] = obj.Detail.Properties.GetValue("OtherMP", string.Empty);
			row["CompanyName"] = obj.Detail.Properties.GetValue("CompanyName", string.Empty);
			row["DepartmentName"] = obj.Detail.Properties.GetValue("DepartmentName", string.Empty);

			string photoKey = obj.Detail.Properties.GetValue("PhotoKey", string.Empty);
			string photoStamp = string.Empty;

			if (string.IsNullOrEmpty(photoKey) == false)
			{
				ImageProperty ip = (ImageProperty)MCS.Web.Library.Script.JSONSerializerExecute.Deserialize<ImageProperty>(photoKey);
				photoStamp = ip.UpdateTime.ToString("yyyyMMddHHmmss") + "|" + ip.ID;
			}

			row["PhotoTimestamp"] = photoStamp;
		}
	}
}
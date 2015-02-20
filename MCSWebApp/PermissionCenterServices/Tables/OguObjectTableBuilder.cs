using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects;

namespace PermissionCenter.Tables
{
	public class OguObjectTableBuilder : TableBuilderBase
	{
		private static Dictionary<UserRankType, string> UserRankMapping = new Dictionary<UserRankType, string>()
		{
			{ UserRankType.ZhengBuJi, "POS_MINISTRY_U" },
			{ UserRankType.FuBuJi, "SUB_MINISTRY_U" },
			{ UserRankType.ZhengJuJi, "POS_OFFICE_U" },
			{ UserRankType.FuJuJi, "SUB_OFFICE_U" },
			{ UserRankType.ZhengChuJi, "POS_ORGAN_U" },
			{ UserRankType.FuChuJi, "SUB_ORGAN_U" },
			{ UserRankType.ZhengKeJi, "POS_DEPART_U" },
			{ UserRankType.FuKeji, "SUB_DEPART_U" },
			{ UserRankType.YiBanRenYuan, "COMMON_U" },
			{ UserRankType.GongRen, "NAVVY_U" },
			{ UserRankType.MinGanJiBie, "SUSCEPTIVITY_U" }
		};

		/// <summary>
		/// 一般Ogu对象的Table结构
		/// </summary>
		/// <returns></returns>
		protected override DataTable CreateTable()
		{
			DataTable table = new DataTable();

			table.Columns.Add("OBJECTCLASS", typeof(string));
			table.Columns.Add("PERSON_ID", typeof(string));
			table.Columns.Add("POSTURAL", typeof(int));
			table.Columns.Add("RANK_NAME", typeof(string));
			table.Columns.Add("STATUS", typeof(int));

			table.Columns.Add("ALL_PATH_NAME", typeof(string));
			table.Columns.Add("GLOBAL_SORT", typeof(string));
			table.Columns.Add("ORIGINAL_SORT", typeof(string));

			table.Columns.Add("DISPLAY_NAME", typeof(string));
			table.Columns.Add("OBJ_NAME", typeof(string));
			table.Columns.Add("LOGON_NAME", typeof(string));

			table.Columns.Add("PARENT_GUID", typeof(string));
			table.Columns.Add("GUID", typeof(string));
			table.Columns.Add("INNER_SORT", typeof(string));

			table.Columns.Add("DESCRIPTION", typeof(string));
			table.Columns.Add("RANK_CODE", typeof(string));
			table.Columns.Add("ORG_CLASS", typeof(int));
			table.Columns.Add("CUSTOMS_CODE", typeof(string));

			table.Columns.Add("ORG_TYPE", typeof(int));
			table.Columns.Add("E_MAIL", typeof(string));
			table.Columns.Add("ATTRIBUTES", typeof(int));
			table.Columns.Add("SIDELINE", typeof(int));
			table.Columns.Add("CODE_NAME", typeof(string));

			table.Columns.Add("VERSION_START_TIME", typeof(DateTime));

			//table.Columns.Add("PhotoTimestamp", typeof(string));

			return table;
		}

		protected override void FillPropertiesToTable(SCObjectAndRelation obj, DataRow row)
		{
			row["OBJECTCLASS"] = obj.SchemaType.ToUpper();
			row["STATUS"] = (int)obj.Status;
			row["OBJ_NAME"] = obj.Name;
			row["DISPLAY_NAME"] = obj.DisplayName;
			row["LOGON_NAME"] = obj.CodeName;
			row["RANK_NAME"] = obj.Detail.Properties.GetValue("Occupation", string.Empty);
			row["E_MAIL"] = obj.Detail.Properties.GetValue("Mail", string.Empty);
			row["GLOBAL_SORT"] = obj.GlobalSort;
			row["ORIGINAL_SORT"] = obj.GlobalSort;
			row["ALL_PATH_NAME"] = obj.FullPath;

			row["PARENT_GUID"] = obj.ParentID;
			row["GUID"] = obj.ID;
			row["INNER_SORT"] = obj.InnerSort;
			row["SIDELINE"] = obj.Default ? 0 : 1;

			string userRank = obj.Detail.Properties.GetValue("UserRank", UserRankType.Unspecified.ToString());

			if (userRank.IsNullOrEmpty())
				userRank = UserRankType.Unspecified.ToString();

			row["RANK_CODE"] = Enum.Parse(typeof(UserRankType), userRank);
			row["CODE_NAME"] = obj.CodeName;
			row["VERSION_START_TIME"] = obj.Detail.VersionStartTime;

			//string photoKey = obj.Detail.Properties.GetValue("PhotoKey", string.Empty);
			//string photoStamp = string.Empty;

			//if (string.IsNullOrEmpty(photoKey) == false)
			//{
			//    ImageProperty ip = (ImageProperty)MCS.Web.Library.Script.JSONSerializerExecute.Deserialize<ImageProperty>(photoKey);
			//    photoStamp = ip.UpdateTime.ToBinary().ToString("X") + "|" + ip.ID;
			//}

			//row["PhotoTimestamp"] = photoStamp;
		}

		private static string ConvertUserRankCodeToString(UserRankType rankType)
		{
			string result = "COMMON_U";

			UserRankMapping.TryGetValue(rankType, out result);

			return result;
		}
	}
}
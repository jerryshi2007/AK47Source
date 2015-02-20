using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.Accredit
{
	public static class AD2DBHelper
	{
		public static readonly string[] ADObjProperties = new string[] { 
            "objectGUID", "distinguishedName", "objectClass", "name", "samAccountName", "displayName", "extensionName", "adminDisplayName", "internationalISDNNumber", "description", "title", "personalTitle", "accountExpires", "userAccountControl", "lastLogon", "lastLogoff", 
            "logonCount", "whenCreated", "whenChanged", "x121Address", "primaryTelexNumber", "co", "c", "l", "primaryInternationalISDNNumber", "physicalDeliveryOfficeName", "givenName", "member", "mail", "sn", "msRTCSIP-PrimaryUserAddress", "mobile", 
            "telephoneNumber"
         };
		public const string RootPath = "机构人员";

		public static bool CompareDataRowIsDifferent(DataRowView newdrv, DataRow olddr, params string[] columns)
		{
			foreach (string str in columns)
			{
				if (string.Compare(newdrv[str].ToString(), olddr[str].ToString(), true) != 0)
				{
					Debug.WriteLine("Diff column name: " + str);

					return true;
				}
			}
			return false;
		}

		public static DepartmentRank ConvertDepartmentRankCode(string strRCode)
		{
			DepartmentRank rank = DepartmentRank.COMMON_D;
			switch (strRCode.ToUpper())
			{
				case "90":
					return DepartmentRank.POS_MINISTRY_D;

				case "80":
					return DepartmentRank.SUB_MINISTRY_D;

				case "70":
					return DepartmentRank.POS_OFFICE_D;

				case "60":
					return DepartmentRank.SUB_OFFICE_D;

				case "50":
					return DepartmentRank.POS_ORGAN_D;

				case "40":
					return DepartmentRank.SUB_ORGAN_D;

				case "30":
					return DepartmentRank.POS_DEPART_D;

				case "20":
					return DepartmentRank.SUB_DEPART_D;

				case "10":
					return DepartmentRank.COMMON_D;
			}
			return rank;
		}

		public static UserRankType ConvertUserRankCode(string strRCode)
		{
			UserRankType type = UserRankType.COMMON_U;
			switch (strRCode.ToUpper())
			{
				case "90":
					return UserRankType.POS_MINISTRY_U;

				case "80":
					return UserRankType.SUB_MINISTRY_U;

				case "70":
					return UserRankType.POS_OFFICE_U;

				case "60":
					return UserRankType.SUB_OFFICE_U;

				case "50":
					return UserRankType.POS_ORGAN_U;

				case "40":
					return UserRankType.SUB_ORGAN_U;

				case "30":
					return UserRankType.POS_DEPART_U;

				case "20":
					return UserRankType.SUB_DEPART_U;

				case "10":
					return UserRankType.COMMON_U;
			}
			return type;
		}

		public static void CopyDataRow(DataRow srcRow, DataRow destRow, params string[] columns)
		{
			foreach (string str in columns)
			{
				destRow[str] = srcRow[str];
			}
		}

		private static string[] SplitDNPath(string dn)
		{
			List<string> list = new List<string>();
			int startIndex = 0;
			for (int i = 0; i < dn.Length; i++)
			{
				if (((dn[i] == ',') && (i < (dn.Length - 3))) && ((dn[i + 2] == '=') || (dn[i + 3] == '=')))
				{
					list.Add(dn.Substring(startIndex, i - startIndex));
					startIndex = i + 1;
				}
			}
			string str = dn.Substring(startIndex);
			if (!string.IsNullOrEmpty(str))
			{
				list.Add(str);
			}
			return list.ToArray();
		}

		public static DepartmentClassType TranslateDeptClass(string ct)
		{
			DepartmentClassType unspecified = DepartmentClassType.Unspecified;
			if (ct == "d")
			{
				unspecified |= DepartmentClassType.LiShuHaiGuan;
			}
			if (ct == "o")
			{
				unspecified |= DepartmentClassType.PaiZhuJiGou;
			}
			if (ct == "i")
			{
				unspecified |= DepartmentClassType.NeiSheJiGou;
			}
			return unspecified;
		}

		public static DepartmentTypeDefine TranslateDeptTypeDefine(SearchResult sr)
		{
			DepartmentTypeDefine unspecified = DepartmentTypeDefine.Unspecified;
			if (ADHelper.GetInstance().GetSearchResultPropertyStrValue("c", sr) == "v")
			{
				return (unspecified | DepartmentTypeDefine.XuNiJiGou);
			}
			if (ADHelper.GetInstance().GetSearchResultPropertyStrValue("x121Address", sr) == "1")
			{
				return (unspecified | DepartmentTypeDefine.BanGongShi);
			}
			if (ADHelper.GetInstance().GetSearchResultPropertyStrValue("x121Address", sr) == "2")
			{
				unspecified |= DepartmentTypeDefine.ZongHeChu;
			}
			return unspecified;
		}

		public static string TranslateDNToFullPath(string dn)
		{
			string[] strArray = SplitDNPath(dn);
			StringBuilder builder = new StringBuilder(0x100);
			for (int i = strArray.Length - 1; i >= 0; i--)
			{
				string[] strArray2 = strArray[i].Split(new char[] { '=' });
				if (strArray2[0].ToUpper() != "DC")
				{
					if (builder.Length > 0)
					{
						builder.Append(@"\");
					}
					builder.Append(strArray2[1]);
				}
			}
			string str = "机构人员";
			string str2 = builder.ToString();
			if (str != string.Empty)
			{
				str2 = str;
				if (builder.Length > 0)
				{
					str2 = str2 + @"\" + builder.ToString();
				}
			}
			return str2;
		}

		public static string TranslateObjectClass(string adClass)
		{
			string str = string.Empty;
			adClass = adClass.ToLower();
			switch (adClass)
			{
				case "person":
					return "USERS";

				case "domain":
				case "organizationalunit":
					return "ORGANIZATIONS";

				case "group":
					return "GROUPS";
			}
			ExceptionHelper.FalseThrow(false, "不能将AD中的对象类别{0}对应到办公系统的对象类别", new object[] { adClass });
			return str;
		}

		public static UserAttributesType TranslateUserAttributes(SearchResult sr)
		{
			UserAttributesType unspecified = UserAttributesType.Unspecified;
			if (ADHelper.GetInstance().GetSearchResultPropertyStrValue("x121Address", sr) == "y")
			{
				unspecified |= UserAttributesType.DangZuChengYuan;
			}
			if (ADHelper.GetInstance().GetSearchResultPropertyStrValue("primaryTelexNumber", sr) == "y")
			{
				unspecified |= UserAttributesType.ShuGuanGanBu;
			}
			if (ADHelper.GetInstance().GetSearchResultPropertyStrValue("co", sr) == "y")
			{
				unspecified |= UserAttributesType.JiaoLiuGanBu;
			}
			if (ADHelper.GetInstance().GetSearchResultPropertyStrValue("primaryTelexNumber", sr) == "y")
			{
				unspecified |= UserAttributesType.JieDiaoGanBu;
			}
			return unspecified;
		}

		public enum DepartmentRank
		{
			COMMON_D = 10,
			POS_DEPART_D = 30,
			POS_MINISTRY_D = 90,
			POS_OFFICE_D = 70,
			POS_ORGAN_D = 50,
			SUB_DEPART_D = 20,
			SUB_MINISTRY_D = 80,
			SUB_OFFICE_D = 60,
			SUB_ORGAN_D = 40,
			SUSCEPTIVITY_D = 1
		}

		public enum UserRankType
		{
			COMMON_U = 10,
			NAVVY_U = 8,
			POS_DEPART_U = 30,
			POS_MINISTRY_U = 90,
			POS_OFFICE_U = 70,
			POS_ORGAN_U = 50,
			SUB_DEPART_U = 20,
			SUB_MINISTRY_U = 80,
			SUB_OFFICE_U = 60,
			SUB_ORGAN_U = 40,
			SUSCEPTIVITY_U = 1,
			Unspecified = 0
		}
	}
}

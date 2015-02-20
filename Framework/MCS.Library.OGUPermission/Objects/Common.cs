#region
// -------------------------------------------------
// Assembly	£∫	DeluxeWorks.Library.OGUPermission
// FileName	£∫	Common.cs
// Remark	£∫	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    …Ú·ø	    20070430		¥¥Ω®
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using MCS.Library.OGUPermission.Properties;

namespace MCS.Library.OGUPermission
{
	internal static class Common
	{
		public const string DefaultAttrs = "INNER_SORT,DESCRIPTION,RANK_CODE,ORG_CLASS,CUSTOMS_CODE,ORG_TYPE,E_MAIL,ATTRIBUTES, SIDELINE";

		public static string GetDataRowTextValue(DataRow row, string fieldName)
		{
			return GetDataRowValue<string>(row, fieldName, string.Empty);
		}

		public static T GetDataRowValue<T>(DataRow row, string fieldName, T defaultValue)
		{
			T result = defaultValue;

			if (row.Table.Columns.Contains(fieldName))
			{
				object data = row[fieldName];

				if (data != DBNull.Value)
					result = (T)data;
			}

			return result;
		}

		public static List<T> BuildObjectsFromTable<T>(DataTable table) where T : IOguObject
		{
			return BuildObjectsFromTable<T>(table, null);
		}

		public static List<T> BuildObjectsFromTable<T>(DataTable table, IOrganization parent) where T : IOguObject
		{
			List<T> list = new List<T>();

			foreach (DataRow row in table.Rows)
			{
				SchemaType type;

				if (row.Table.Columns.Contains("OBJECTCLASS"))
				{
					type = (SchemaType)Enum.Parse(typeof(SchemaType), row["OBJECTCLASS"].ToString(), true);

					if (type == SchemaType.Organizations)
						if (row.Table.Columns.Contains("ACCESS_LEVEL") || (parent != null && parent is IOrganizationInRole))
							type = SchemaType.OrganizationsInRole;
				}
				else
					type = OguObjectHelper.GetSchemaTypeFromInterface<T>();

				IOguObject baseObject = OguPermissionSettings.GetConfig().OguObjectFactory.CreateObject(type);

				if (baseObject is OguBaseImpl)
				{
					OguBaseImpl oBase = (OguBaseImpl)baseObject;

					oBase.InitProperties(row);

					if (oBase is IOrganizationInRole && (parent != null && parent is IOrganizationInRole))
						((OguOrganizationInRoleImpl)oBase).AccessLevel = ((IOrganizationInRole)parent).AccessLevel;
				}

				list.Add((T)(baseObject as object));
			}

			return list;
		}

		//public static SchemaType GetObjectTypeFromInterface<T>() where T : IOguObject
		//{
		//    SchemaType type = SchemaType.Unspecified;

		//    if (typeof(T) == typeof(IOguObject))
		//        type = SchemaType.All & ~SchemaType.Sideline;
		//    else
		//        if (typeof(T) == typeof(IOrganization))
		//            type = SchemaType.Organizations;
		//        else
		//            if (typeof(T) == typeof(IOrganizationInRole))
		//                type = SchemaType.OrganizationsInRole;
		//            else
		//                if (typeof(T) == typeof(IUser))
		//                    type = SchemaType.Users;
		//                else
		//                    if (typeof(T) == typeof(IGroup))
		//                        type = SchemaType.Groups;

		//    return type;
		//}

		public static UserAttributesType ConvertUserAttribute(int nAttribute)
		{
			UserAttributesType uat = UserAttributesType.Unspecified;

			if ((nAttribute & 1) != 0)
				uat |= UserAttributesType.DangZuChengYuan;

			if ((nAttribute & 2) != 0)
				uat |= UserAttributesType.ShuGuanGanBu;

			if ((nAttribute & 4) != 0)
				uat |= UserAttributesType.JiaoLiuGanBu;

			if ((nAttribute & 8) != 0)
				uat |= UserAttributesType.JieDiaoGanBu;

			return uat;
		}

		public static UserRankType ConvertUserRankCode(string strRCode)
		{
			UserRankType rank = UserRankType.YiBanRenYuan;

			switch (strRCode.ToUpper())
			{
				case "POS_MINISTRY_U":
					rank = UserRankType.ZhengBuJi;
					break;
				case "SUB_MINISTRY_U":
					rank = UserRankType.FuBuJi;
					break;
				case "POS_OFFICE_U":
					rank = UserRankType.ZhengJuJi;
					break;
				case "SUB_OFFICE_U":
					rank = UserRankType.FuJuJi;
					break;
				case "POS_ORGAN_U":
					rank = UserRankType.ZhengChuJi;
					break;
				case "SUB_ORGAN_U":
					rank = UserRankType.FuChuJi;
					break;
				case "POS_DEPART_U":
					rank = UserRankType.ZhengKeJi;
					break;
				case "SUB_DEPART_U":
					rank = UserRankType.FuKeji;
					break;
				case "COMMON_U":
					rank = UserRankType.YiBanRenYuan;
					break;
				case "NAVVY_U":
					rank = UserRankType.GongRen;
					break;
				case "SUSCEPTIVITY_U":
					rank = UserRankType.MinGanJiBie;
					break;
			}

			return rank;
		}
	}
}

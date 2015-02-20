using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects
{
	public static class OguExtension
	{
		/// <summary>
		/// 用户的汇报人
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public static IUser ReportTo(this IUser user)
		{
			IUser result = null;

			if (user != null)
				result = UserReportInfoAdapter.Instance.GetUserReportTo(user);

			return result;
		}

		/// <summary>
		/// 得到部门的描述，默认是显示名称，如果有父部门，那么再显示一级父部门的名称。主要用于待办和AppCommonInfo的DraftDepartmentName
		/// </summary>
		/// <param name="org"></param>
		/// <returns></returns>
		public static string GetDepartmentDescription(this IOrganization org)
		{
			string result = string.Empty;

			if (org != null)
			{
				result = org.DisplayName;

				if (org.Parent != null)
					result = org.Parent.DisplayName + " " + result;
			}

			return result;
		}

		/// <summary>
		/// 生成对象的描述信息。如果不为空，返回DisplayName，否则是Name
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string ToDescription(this IOguObject obj)
		{
			string result = string.Empty;

			if (obj != null)
			{
				result = obj.DisplayName;

				if (result.IsNullOrEmpty())
					result = obj.Name;
			}

			return result;
		}

		/// <summary>
		/// 转换成简单XML节点
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="element"></param>
		/// <param name="childNodeName"></param>
		public static void ToSimpleXElement(this IOguObject obj, XElement element, string childNodeName)
		{
			element.NullCheck("element");

			if (obj != null)
				((ISimpleXmlSerializer)obj).ToXElement(element, childNodeName);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="element"></param>
		public static void ToSimpleXElement(this IOguObject obj, XElement element)
		{
			ToSimpleXElement(obj, element, string.Empty);
		}
	}
}

using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using System.Xml;
using MCS.Library.Accredit.OguAdmin;
using MCS.Library.Accredit.Properties;
using MCS.Library.Data.Builder;

namespace MCS.Library.Accredit
{
	internal partial class OguDBOperation
	{
		private void eventContainer_BeforeInitPassword(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			string strUserGuid = xmlDoc.DocumentElement.SelectSingleNode("GUID").InnerText;

			string strSql = "SELECT TOP 1 GUID FROM PWD_ARITHMETIC WHERE VISIBLE = 1 ORDER BY SORT_ID";
			object oPwdGuid = InnerCommon.ExecuteScalar(strSql);
			ExceptionHelper.TrueThrow(oPwdGuid == null, "对不起，系统中没有设置数据加密算法类型！");

			strSql = "UPDATE USERS SET MODIFY_TIME=GETDATE(), PWD_TYPE_GUID = {0}, USER_PWD = {1} WHERE GUID = {2}";

			string strPwd = SecurityCalculate.PwdCalculate(oPwdGuid.ToString(), CommonResource.OriginalSortDefault);

			strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(oPwdGuid.ToString(), true),
				TSqlBuilder.Instance.CheckQuotationMark(strPwd, true),
				TSqlBuilder.Instance.CheckQuotationMark(strUserGuid, true));

			context.Add("Sql", strSql);
		}

		private void eventContainer_InitPassword(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			InnerCommon.ExecuteNonQuery(context["Sql"].ToString());
		}

		private void eventContainer_BeforeResetPassword(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			XmlElement root = xmlDoc.DocumentElement;
			string strUserGuid = root.SelectSingleNode("GUID").InnerText;
			string strOrigOldPwd = root.SelectSingleNode("OldPwd").InnerText;
			string strOldPwdType = root.SelectSingleNode("OldPwdType").InnerText;
			string strOrigNewPwd = root.SelectSingleNode("NewPwd").InnerText;
			string strNewPwdType = root.SelectSingleNode("NewPwdType").InnerText;

			string strOldPwd = SecurityCalculate.PwdCalculate(strOldPwdType, strOrigOldPwd);
			string strNewPwd = SecurityCalculate.PwdCalculate(strNewPwdType, strOrigNewPwd);

			string strSql = "UPDATE USERS SET MODIFY_TIME=GETDATE(), PWD_TYPE_GUID={0}, USER_PWD={1} "
				+ "WHERE GUID={2} AND USER_PWD={3} AND PWD_TYPE_GUID={4}";
			strSql = string.Format(strSql, TSqlBuilder.Instance.CheckQuotationMark(strNewPwdType, true),
				TSqlBuilder.Instance.CheckQuotationMark(strNewPwd, true),
				TSqlBuilder.Instance.CheckQuotationMark(strUserGuid, true),
				TSqlBuilder.Instance.CheckQuotationMark(strOldPwd, true),
				TSqlBuilder.Instance.CheckQuotationMark(strOldPwdType, true));

			context.Add("Sql", strSql);
		}

		private void eventContainer_ResetPassword(XmlDocument xmlDoc, Dictionary<object, object> context)
		{
			ExceptionHelper.TrueThrow(InnerCommon.ExecuteNonQuery(context["Sql"].ToString()) == 0,
						"对不起，你输入的旧口令或者口令加密类型不正确！请重来一次！");
			//本操作不记录日志
		}
	}
}

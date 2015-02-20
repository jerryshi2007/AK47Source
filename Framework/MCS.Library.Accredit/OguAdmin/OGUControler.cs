using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

using MCS.Library.Data;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.Accredit.Properties;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// ������Ա���������
	/// </summary>
	/// <remarks>
	/// Ŀǰ�����ṩ�����޸Ĺ���
	/// </remarks>
	public class OGUControler
	{
		#region ���캯��
		/// <summary>
		/// ���캯��
		/// </summary>
		public OGUControler()
		{

		}

		#endregion

		#region public function

		/// <summary>
		/// �û��޸Ŀ���ӿ�
		/// </summary>
		/// <param name="strUserGuid">Ҫ���޸Ŀ�����û�</param>
		/// <param name="strOldPwd">�û��ľɿ���</param>
		/// <param name="strNewPwd">ʹ�õ��¿���</param>
		/// <param name="strConfirmPwd">�¿����ȷ��</param>
		/// <returns>�����޸��Ƿ�ɹ�</returns>
		public bool UpdateUserPwd(string strUserGuid, string strOldPwd, string strNewPwd, string strConfirmPwd)
		{
			return UpdateUserPwd(strUserGuid, SearchObjectColumn.SEARCH_GUID, strOldPwd, strNewPwd, strConfirmPwd);
		}

		#endregion

		#region public function (with DataAccess)
		/// <summary>
		/// �û��޸Ŀ���ӿ�
		/// </summary>
		/// <param name="strUserValue">Ҫ���޸Ŀ�����û�</param>
		/// <param name="socu">strUserValue��Ӧ����������</param>
		/// <param name="strOldPwd">�û��ľɿ���</param>
		/// <param name="strNewPwd">ʹ�õ��¿���</param>
		/// <param name="strConfirmPwd">�¿����ȷ��</param>
		/// <returns>�����޸��Ƿ�ɹ�</returns>
		public bool UpdateUserPwd(string strUserValue, SearchObjectColumn socu, string strOldPwd, string strNewPwd, string strConfirmPwd)
		{
			ExceptionHelper.TrueThrow(string.IsNullOrEmpty(strNewPwd.Trim()), "�Բ����û��ĵ�¼�����Ϊ�գ�");
			ExceptionHelper.FalseThrow(strNewPwd == strConfirmPwd, "�Բ����û��ġ��¿�������롰ȷ�Ͽ��һ�£�");

			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					Database database = DatabaseFactory.Create(context);

					string strUserColName = OGUCommonDefine.GetSearchObjectColumn(socu);
					string strSql = @"SELECT USERS.GUID 
				FROM USERS, OU_USERS
				WHERE USERS.GUID = OU_USERS.USER_GUID
					AND " + DatabaseSchema.Instence.GetTableColumns(strUserColName, "USERS")
						  + " = " + TSqlBuilder.Instance.CheckQuotationMark(strUserValue, true) + @";
				SELECT TOP 1 GUID FROM PWD_ARITHMETIC WHERE VISIBLE = 1 ORDER BY SORT_ID;";

					DataSet ds = database.ExecuteDataSet(CommandType.Text, strSql);

					ExceptionHelper.TrueThrow(ds.Tables[0].Rows.Count == 0, "�Բ���ϵͳ��û���ҵ���ָ�����û���");
					ExceptionHelper.TrueThrow(ds.Tables[0].Rows.Count > 1, "�Բ�����ָ�����û���ϵͳ�в�Ψһ��");
					ExceptionHelper.TrueThrow(ds.Tables[1].Rows.Count < 1, "�Բ���ϵͳ���ҵĲ������ݱ�PWD_ARITHMETIC�����ݣ�");
					string secNewPwd = SecurityCalculate.PwdCalculate(ds.Tables[1].Rows[0][0].ToString(), strNewPwd);
					string secOldPwd = SecurityCalculate.PwdCalculate(ds.Tables[1].Rows[0][0].ToString(), strOldPwd);

					strSql = "UPDATE USERS SET USER_PWD = "
						+ TSqlBuilder.Instance.CheckQuotationMark(secNewPwd, true) + @"
				WHERE USERS.GUID = " + TSqlBuilder.Instance.CheckQuotationMark((string)ds.Tables[0].Rows[0]["GUID"], true) + @"
					AND USERS.USER_PWD = "
						+ TSqlBuilder.Instance.CheckQuotationMark(secOldPwd, true);

					ExceptionHelper.FalseThrow(database.ExecuteNonQuery(CommandType.Text, strSql) == 1, "�Բ����û��ľɿ����ȷ��");
				}
				scope.Complete();
			}
			return true;
		}

		#endregion

		#region private function

		//private static DataSet _DataSet_Schema = null;
		///// <summary>
		///// �������ݱ������Լ�Ҫ���ȡ�����ݱ��е�������������ϳ�Ϊ�����ݱ���ص����ݲ�ѯ����
		///// </summary>
		///// <param name="strAttrs">���ݱ��е��������������</param>
		///// <param name="da">���ݿ��������</param>
		///// <param name="strTables">ָ�������ݱ�����</param>
		///// <returns>���ݲ�ѯ����SQL</returns>
		//private static string GetTableColumns(string strAttrs, params string[] strTables)
		//{
		//    StringBuilder strB = new StringBuilder(1024);
		//    ExceptionHelper.TrueThrow(strAttrs.Length == 0, "�Բ���,����û��ָ��Ҫ���ѯ�������ƣ�����֤��");
		//    ExceptionHelper.TrueThrow(strTables.Length == 0, "�Բ���û��ȷ�������ݱ����ƣ�");

		//    if (strAttrs.Trim() == "*")
		//        strB.Append(strTables[0] + "." + strAttrs.Trim());
		//    else
		//    {
		//        string[] strAttrArr = strAttrs.Split(',');
		//        InitHashTable(da);
		//        for (int i = 0; i < strAttrArr.Length; i++)
		//        {
		//            strAttrArr[i] = strAttrArr[i].Trim();

		//            if (strB.Length > 0)
		//                strB.Append(", ");

		//            bool bComplicated = false;

		//            for (int j = 0; j < strTables.Length; j++)
		//            {
		//                DataTable table = _DataSet_Schema.Tables[strTables[j]];

		//                DataRow[] drs = table.Select("CNAME=" + GetSqlStr.AddCheckQuotationMark(strAttrArr[i]));
		//                if (drs.Length > 0)
		//                {
		//                    strB.Append(XmlHelper.DBValueToString(drs[0]["TNAME"]) + "." + strAttrArr[i]);
		//                    bComplicated = true;
		//                    break;
		//                }
		//            }

		//            if (false==bComplicated)
		//                strB.Append(" NULL AS " + strAttrArr[i]);
		//        }
		//    }

		//    return strB.ToString();
		//}

//        /// <summary>
//        /// 
//        /// </summary>
//        private static void InitHashTable(DataAccess da)
//        {
//            if (_DataSet_Schema == null)
//            {
//                string[] strAllTables = { "ORGANIZATIONS", "GROUPS", "USERS", "OU_USERS", "RANK_DEFINE", "GROUP_USERS" };
//                StringBuilder strB = new StringBuilder(512);
//                for (int i = 0; i < strAllTables.Length; i++)
//                {
//                    strB.Append(@"
//						SELECT SYSOBJECTS.NAME AS TNAME, SYSCOLUMNS.NAME AS CNAME
//						FROM SYSOBJECTS, SYSCOLUMNS
//						WHERE SYSOBJECTS.ID = SYSCOLUMNS.ID
//							AND SYSOBJECTS.NAME IN (" + GetSqlStr.AddCheckQuotationMark(strAllTables[i]) + @")
//						ORDER BY TNAME, SYSCOLUMNS.COLID;
//						");
//                }
//                _DataSet_Schema = OGUCommonDefine.ExecuteDataset(strB.ToString(), da, strAllTables);
//            }
//        }

		#endregion
	}
}

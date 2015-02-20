using System;
using System.Xml;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Transactions;

using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MCS.Library.Configuration;
using MCS.Library.Accredit.Properties;
using MCS.Library.Accredit.Configuration;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// OGUCommonDefine ��ժҪ˵����
	/// </summary>
	public class OGUCommonDefine
	{
		#region ���캯��
		/// <summary>
		/// ���캯��
		/// </summary>
		public OGUCommonDefine()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
		#endregion

		#region Public Const Define ��Deleted��
		///// <summary>
		///// ��Ȩϵͳ�е����ݿ��������ö�Ӧ����
		///// </summary>
		//public const string STR_CONN = "AccreditAdmin";
		///// <summary>
		///// �������������ϵ�����6λһ�ڷ�ʽ��
		///// </summary>
		//public const string OGU_ORIGINAL_SORT = "000000";
		///// <summary>
		///// ������Ա����ϵͳ��ע������
		///// </summary>
		//public const string OGU_ADMIN_APP_NAME = "OGU_ADMIN";
		#endregion

		#region ϵͳȨ�޵Ķ��� ��Deleted��
		///// <summary>
		///// �����»���
		///// </summary>
		//public const string PM_CREATE_ORGANIZATIONS = "CREATE_ORGANIZATIONS";
		///// <summary>
		///// �������û�
		///// </summary>
		//public const string PM_CREATE_USERS = "CREATE_USERS";
		///// <summary>
		///// �������û���
		///// </summary>
		//public const string PM_CREATE_GROUPS = "CREATE_GROUPS";
		///// <summary>
		///// �����û���ְ
		///// </summary>
		//public const string PM_SET_SIDELINE = "SET_SIDELINE";
		///// <summary>
		///// �޸Ļ�������
		///// </summary>
		//public const string PM_UPDATE_ORGANIZATIONS = "UPDATE_ORGANIZATIONS";
		///// <summary>
		///// �޸��û�����
		///// </summary>
		//public const string PM_UPDATE_USERS = "UPDATE_USERS";
		///// <summary>
		///// �޸���Ա������
		///// </summary>
		//public const string PM_UPDATE_GROUPS = "UPDATE_GROUPS";
		///// <summary>
		///// �߼�ɾ������
		///// </summary>
		//public const string PM_LOGIC_DELETE_ORGANIZATIONS = "LOGIC_DELETE_ORGANIZATIONS";
		///// <summary>
		///// �߼�ɾ���û�
		///// </summary>
		//public const string PM_LOGIC_DELETE_USERS = "LOGIC_DELETE_USERS";
		///// <summary>
		///// �߼�ɾ���û���
		///// </summary>
		//public const string PM_LOGIC_DELETE_GROUPS = "LOGIC_DELETE_GROUPS";
		///// <summary>
		///// �ָ����߼�ɾ������
		///// </summary>
		//public const string PM_FURBISH_DELETE_ORGANIZATIONS = "FURBISH_DELETE_ORGANIZATIONS";
		///// <summary>
		///// �ָ����߼�ɾ���û�
		///// </summary>
		//public const string PM_FURBISH_DELETE_USERS = "FURBISH_DELETE_USERS";
		///// <summary>
		///// �ָ����߼�ɾ���û���
		///// </summary>
		//public const string PM_FURBISH_DELETE_GROUPS = "FURBISH_DELETE_GROUPS";
		///// <summary>
		///// �����ڶ�������
		///// </summary>
		//public const string PM_SORT_IN_ORGANIZATIONS = "SORT_IN_ORGANIZATIONS";
		///// <summary>
		///// ��Ա������Ա����
		///// </summary>
		//public const string PM_SORT_IN_GROUP = "SORT_IN_GROUP";
		///// <summary>
		///// ��ʼ���û�����
		///// </summary>
		//public const string PM_INIT_USERS_PWD = "INIT_USERS_PWD";
		///// <summary>
		///// ����ɾ������
		///// </summary>
		//public const string PM_REAL_DELETE_ORGANIZATIONS = "REAL_DELETE_ORGANIZATIONS";
		///// <summary>
		///// ����ɾ����Ա��
		///// </summary>
		//public const string PM_REAL_DELETE_GROUPS = "REAL_DELETE_GROUPS";
		///// <summary>
		///// ����ɾ���û�
		///// </summary>
		//public const string PM_REAL_DELETE_USERS = "REAL_DELETE_USERS";
		///// <summary>
		///// ��Ա�������ӳ�Ա
		///// </summary>
		//public const string PM_GROUP_ADD_USERS = "GROUP_ADD_USERS";
		///// <summary>
		///// ��Ա����ɾ����Ա
		///// </summary>
		//public const string PM_GROUP_DEL_USERS = "GROUP_DEL_USERS";
		///// <summary>
		///// ��������
		///// </summary>
		//public const string PM_SECRETARY_ADD = "SECRETARY_ADD";
		///// <summary>
		///// ɾ������
		///// </summary>
		//public const string PM_SECRETARY_DEL = "SECRETARY_DEL";

		#endregion

		#region DataBase Tables ��Deleted��
		///// <summary>
		///// �������ݱ�����
		///// </summary>
		//public const string DB_TABLE_ORGANIZATIONS = "ORGANIZATIONS";
		///// <summary>
		///// �û���Ϣ��
		///// </summary>
		//public const string DB_TABLE_USERS = "USERS";
		///// <summary>
		///// �û����
		///// </summary>
		//public const string DB_TABLE_GROUPS = "GROUPS";
		///// <summary>
		///// �û������֮��Ĺ�ϵ��
		///// </summary>
		//public const string DB_TABLE_OU_USERS = "OU_USERS";
		///// <summary>
		///// �û�����Ա��֮��Ĺ�ϵ��
		///// </summary>
		//public const string DB_TABLE_GROUP_USERS = "GROUP_USERS";
		///// <summary>
		///// �쵼������Ĺ�ϵ��
		///// </summary>
		//public const string DB_TABLE_SECRETARIES = "SECRETARIES";
		///// <summary>
		///// ϵͳ�������Ա����������Ϣ��
		///// </summary>
		//public const string DB_TABLE_RANK_DEFINE = "RANK_DEFINE";
		///// <summary>
		///// ϵͳ��ʹ�õ��������ͱ�
		///// </summary>
		//public const string DB_TABLE_PWD_ARITHMETIC = "PWD_ARITHMETIC";
		#endregion

		#region public function
		/// <summary>
		/// �Զ����SQL��ִ�й���
		/// </summary>
		/// <param name="strSql">Ҫ��ִ�е����ݲ�ѯSQL</param>
		/// <returns>���β�����Ӱ�����������</returns>
		internal static int ExecuteNonQuery(string strSql)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(strSql, "ExecuteNonQueryWithoutTransaction--strSql");
			using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
			{
				Database database = DatabaseFactory.Create(context);
#if DEBUG
				Debug.WriteLine(strSql);
#endif
				ExceptionHelper.TrueThrow(string.IsNullOrEmpty(strSql), "���ݴ������SQL����Ϊ�մ���");

				return database.ExecuteNonQuery(CommandType.Text, strSql);
			}
		}

		/// <summary>
		/// �Զ����SQL��ִ�й���
		/// </summary>
		/// <param name="strSql">Ҫ��ִ�е����ݲ�ѯSQL</param>
		/// <param name="strTables">Ҫ�����õ����ݱ�����</param>
		/// <returns>���β�ѯ�����ݽ����</returns>
		internal static DataSet ExecuteDataset(string strSql, params string[] strTables)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(strSql, "ExecuteDatasetWithoutTransaction--strSql");
#if DEBUG
			Debug.WriteLine(strSql);
#endif
			using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
			{
				Database database = DatabaseFactory.Create(context);
				return database.ExecuteDataSet(CommandType.Text, strSql, strTables);
			}
		}

		/// <summary>
		/// �Զ����SQL��ִ�й���
		/// </summary>
		/// <param name="strSql">Ҫ��ִ�е����ݲ�ѯSQL</param>
		/// <returns>���β�ѯ�Ľ������</returns>
		internal static object ExecuteScalar(string strSql)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty(strSql, "ExecuteScalar--strSql");
#if DEBUG
			Debug.WriteLine(strSql);
#endif
			using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
			{
				Database database = DatabaseFactory.Create(context);
				return database.ExecuteScalar(CommandType.Text, strSql);
			}
		}

		/// <summary>
		/// ��ϵͳ��Ҫ���ѯ���ݵ�Ĭ���������ر�Ҫ�����Ժϲ�
		/// </summary>
		/// <param name="strAttrs"></param>
		/// <returns></returns>
		public static string CombinateAttr(string strAttrs)
		{
			string[] strOriginalAttrs = {"GUID", "PARENT_GUID", "LOGON_NAME", "OBJ_NAME", "DISPLAY_NAME", "ORIGINAL_SORT",
										"GLOBAL_SORT", "ALL_PATH_NAME", "STATUS", "RANK_NAME", "POSTURAL", "SIDELINE", 
										"CUSTOMS_CODE", "PERSON_ID"};

			for (int i = 0; i < strOriginalAttrs.Length; i++)
			{
				if (strAttrs.IndexOf(strOriginalAttrs[i]) >= 0)
					continue;

				if (strAttrs.Length > 0)
					strAttrs = strOriginalAttrs[i] + ", " + strAttrs;
				else
					strAttrs = strOriginalAttrs[i];
			}

			return TSqlBuilder.Instance.CheckQuotationMark(strAttrs, false);
		}     



		/// <summary>
		/// ���ϵͳ�е��������ã����ɶ�ϵͳ��������ݲ�ѯ������
		/// </summary>
		/// <param name="strOriginalHideType">ԭʼ��ϵͳ����</param>
		/// <returns>���ϵͳ�е��������ã����ɶ�ϵͳ��������ݲ�ѯ������</returns>
		internal static string GetHideType(string strOriginalHideType)
		{
			string strResult = strOriginalHideType;

			//string strAutoHide = (new SysConfig()).GetDataFromConfig("AutohideType", string.Empty);
			string strAutoHide = AccreditSection.GetConfig().AccreditSettings.AutohideType;

			if (strOriginalHideType.Length == 0)
				strResult = strAutoHide;
			else
			{
				if (strAutoHide.Length > 0)
				{
					string[] strArrs = strOriginalHideType.Split(',', ' ', ';');
					string[] strAutoArrs = strAutoHide.Split(',', ' ', ';');

					for (int i = 0; i < strAutoArrs.Length; i++)
					{
						bool bAddAuto = true;

						for (int j = 0; j < strArrs.Length; j++)
						{
							if (strArrs[j] == strAutoArrs[i])
								bAddAuto = false;
						}

						if (bAddAuto)
							strResult += "," + strAutoArrs[i];
					}
				}
			}

			return strResult;
		}

		/// <summary>
		/// ��ȡ��ѯ�������ֶ�����
		/// </summary>
		/// <param name="soc">���ݲ�ѯ�������ͣ���Ӧ���ֶ����ƣ�</param>
		/// <returns>��Ӧ��ѯ�����ݽ������Ӧ�ֶ����ƣ�</returns>
		internal static string GetSearchObjectColumn(SearchObjectColumn soc)
		{
			string strResult = string.Empty;
			switch (soc)
			{
				case SearchObjectColumn.SEARCH_GUID: strResult = "GUID";
					break;
				case SearchObjectColumn.SEARCH_USER_GUID: strResult = "USER_GUID";
					break;
				case SearchObjectColumn.SEARCH_ORIGINAL_SORT: strResult = "ORIGINAL_SORT";
					break;
				case SearchObjectColumn.SEARCH_GLOBAL_SORT: strResult = "GLOBAL_SORT";
					break;
				case SearchObjectColumn.SEARCH_ALL_PATH_NAME: strResult = "ALL_PATH_NAME";
					break;
				case SearchObjectColumn.SEARCH_LOGON_NAME: strResult = "LOGON_NAME";
					break;
				case SearchObjectColumn.SEARCH_PERSON_ID: strResult = "PERSON_ID";
					break;
				case SearchObjectColumn.SEARCH_IC_CARD: strResult = "IC_CARD";
					break;
				case SearchObjectColumn.SEARCH_CUSTOMS_CODE: strResult = "CUSTOMS_CODE";
					break;
				case SearchObjectColumn.SEARCH_SYSDISTINCT1: strResult = "SYSDISTINCT1";
					break;
				case SearchObjectColumn.SEARCH_SYSDISTINCT2: strResult = "SYSDISTINCT2";
					break;
				case SearchObjectColumn.SEARCH_OUSYSDISTINCT1: strResult = "SYSOUDISTINCT1";
					break;
				case SearchObjectColumn.SEARCH_OUSYSDISTINCT2: strResult = "SYSOUDISTINCT2";
					break;
				//Ϊ����Ͼ�����ͳһƽ̨�л����������ֶ�ID[����Ψһ�ֶ�]
				case SearchObjectColumn.SEARCH_IDENTITY: strResult = "ID";
					break;
				case SearchObjectColumn.SEARCH_NULL:
				default: ExceptionHelper.TrueThrow<ApplicationException>(true, "�Բ���ϵͳ���ṩ��" + soc.ToString() + "��ѯ�������ͣ�");
					break;
			}
			return strResult;
		}

		/// <summary>
		/// ��ȡ��ѯ�������ֶ����ƴ���
		/// </summary>
		/// <param name="strValueType">��Ӧ��ѯ�����ݽ������Ӧ�ֶ����ƣ�</param>
		/// <returns>���ݲ�ѯ�������ͣ���Ӧ���ֶ����ƣ�</returns>
		public static SearchObjectColumn GetSearchObjectColumn(string strValueType)
		{
			SearchObjectColumn soc = SearchObjectColumn.SEARCH_NULL;
			switch (strValueType)
			{
				case "GUID": soc = SearchObjectColumn.SEARCH_GUID;
					break;
				case "USER_GUID": soc = SearchObjectColumn.SEARCH_USER_GUID;
					break;
				case "ORIGINAL_SORT": soc = SearchObjectColumn.SEARCH_ORIGINAL_SORT;
					break;
				case "GLOBAL_SORT": soc = SearchObjectColumn.SEARCH_GLOBAL_SORT;
					break;
				case "ALL_PATH_NAME": soc = SearchObjectColumn.SEARCH_ALL_PATH_NAME;
					break;
				case "LOGON_NAME": soc = SearchObjectColumn.SEARCH_LOGON_NAME;
					break;
				case "PERSON_ID": soc = SearchObjectColumn.SEARCH_PERSON_ID;
					break;
				case "IC_CARD": soc = SearchObjectColumn.SEARCH_IC_CARD;
					break;
				case "CUSTOMS_CODE": soc = SearchObjectColumn.SEARCH_CUSTOMS_CODE;
					break;
				case "SYSDISTINCT1": soc = SearchObjectColumn.SEARCH_SYSDISTINCT1;
					break;
				case "SYSDISTINCT2": soc = SearchObjectColumn.SEARCH_SYSDISTINCT2;
					break;
				case "SYSOUDISTINCT1": soc = SearchObjectColumn.SEARCH_OUSYSDISTINCT1;
					break;
				case "SYSOUDISTINCT2": soc = SearchObjectColumn.SEARCH_OUSYSDISTINCT2;
					break;
				//Ϊ����Ͼ�����ͳһƽ̨�л����������ֶ�ID[����Ψһ�ֶ�]
				case "ID":
				case "[ID]": soc = SearchObjectColumn.SEARCH_IDENTITY;
					break;
				default: ExceptionHelper.TrueThrow<ApplicationException>(true, "�Բ���ϵͳ���ṩ��" + strValueType + "��ѯ�������ͣ�");
					break;
			}
			return soc;
		}

		internal static string AddMulitStrWithQuotationMark(string source)
		{
			string[] arrSource = source.Split(',', ';');
			StringBuilder builder = new StringBuilder(128);

			foreach (string temp in arrSource)
			{
				if (false==string.IsNullOrEmpty(temp.Trim()))
				{
					if (builder.Length > 0)
						builder.Append(",");
					builder.Append(TSqlBuilder.Instance.CheckQuotationMark(temp.Trim(), true));
				}
			}

			return builder.Length == 0 ? "''" : builder.ToString();
		}

		/// <summary>
		/// �����ݿ��ֶ�ֵת��Ϊ�ַ���
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string DBValueToString(object source)
		{
			string result = string.Empty;

			if (false==(source is DBNull))
			{
				if (source is DateTime)
				{
					if ((DateTime)source != DateTime.MinValue && (DateTime)source != new DateTime(1900, 1, 1, 0, 0, 0, 0))
						result = string.Format("{0:yyyy-MM-dd HH:mm:ss}", source);
				}
				else
					result = source.ToString();
			}

			return result;
		}
		#endregion
	}
}

using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Web;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

using MCS.Library.Accredit.Properties;
using MCS.Library.Accredit.Configuration;
using MCS.Library.Accredit.OguAdmin.Interfaces;
using MCS.Library.Core;
using MCS.Library.Caching;
using MCS.Library.Data;
using MCS.Library.Data.Builder;
using MCS.Library.Accredit.OguAdmin.Caching;
using MCS.Library.Accredit.Common;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// ������Ա����ϵͳ�е����в�ѯ����ʵ��
	/// </summary>
	public sealed class OGUReader
	{
		#region Variable Define
		///// <summary>
		///// �������ݿ�ṹ�Ĳ����ԣ����Կ��Բ��þ�̬�����������ݿ�ṹ��Schema
		///// </summary>
		//private static DataSet _DataSet_Schema = null;

		private static XmlDocument MaskObjectDocument = null;

		/// <summary>
		/// 
		/// </summary>
		public const string SearchAllTerm = "@SearchAll@";
		#endregion

		#region ���캯��
		/// <summary>
		/// ���캯��
		/// </summary>
		public OGUReader()
		{
		}

		#endregion

		#region public function
		#region GetOrganizationChildren
		/// <summary>
		/// ��ȡָ�������µ������Ӷ���
		/// </summary>
		/// <param name="strOrgValues">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="iLot">Ҫ���ѯ�����ݶ������ͣ��������顢��Ա����ְ����</param>
		/// <param name="iLod">�Ƿ�������߼�ɾ���ĳ�Ա</param>
		/// <param name="iDepth">Ҫ���ѯ�Ĳ�Σ�����һ�㣩��0����ȫ���Ӷ���</param>
		/// <param name="strOrgRankCodeName">��ѯ��Ҫ��Ļ������󼶱�</param>
		/// <param name="strUserRankCodeName">��ѯ��Ҫ�����Ա���󼶱�</param>
		/// <param name="strHideType">��ѯ��Ҫ�����ε�����(��Ӧ�������ļ�HideTypes.xml�е�����)</param>
		/// <param name="strAttrs">��ѯ��Ҫ���ȡ���ݶ������������</param>
		/// <param name="iOrgClass">Ҫ��չ�ֻ���������</param>
		/// <param name="iOrgType">Ҫ��չ�ֻ���������</param>
		/// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
		public static DataSet GetOrganizationChildren(string strOrgValues,
			SearchObjectColumn soc,
			int iLot,
			int iLod,
			int iDepth,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strHideType,
			string strAttrs,
			int iOrgClass,
			int iOrgType)
		{
			strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);

			SearchOrgChildrenCondition scc = new SearchOrgChildrenCondition(strOrgValues, soc, strAttrs);
			scc.ListObjDelete = (ListObjectDelete)iLod;
			scc.ListObjType = (ListObjectType)iLot;
			scc.Depth = iDepth;
			scc.OrgRankCN = strOrgRankCodeName;
			scc.UserRankCN = strUserRankCodeName;
			scc.HideType = strHideType;
			scc.OrgClass = iOrgClass;
			scc.OrgType = iOrgType;

			return GetOrganizationChildren(scc);
		}

		/// <summary>
		/// ��ȡָ�������µ������Ӷ���
		/// </summary>
		/// <param name="strOrgValues">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="iLot">Ҫ���ѯ�����ݶ������ͣ��������顢��Ա����ְ����</param>
		/// <param name="iLod">�Ƿ�������߼�ɾ���ĳ�Ա</param>
		/// <param name="iDepth">Ҫ���ѯ�Ĳ�Σ�����һ�㣩��0����ȫ���Ӷ���</param>      
		/// <param name="strHideType">��ѯ��Ҫ�����ε�����(��Ӧ�������ļ�HideTypes.xml�е�����)</param>
		/// <param name="strAttrs">��ѯ��Ҫ���ȡ���ݶ������������</param>
		/// <param name="iOrgClass">Ҫ��չ�ֻ���������</param>
		/// <param name="iOrgType">Ҫ��չ�ֻ���������</param>
		/// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
		/// 
		//2009-05-07
		public static DataSet GetOrganizationChildren2(string strOrgValues,
			SearchObjectColumn soc,
			int iLot,
			int iLod,
			int iDepth,
			string strHideType,
			string strAttrs,
			int iOrgClass,
			int iOrgType)
		{
			strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);

			SearchOrgChildrenCondition scc = new SearchOrgChildrenCondition(strOrgValues, soc, strAttrs);
			scc.ListObjDelete = (ListObjectDelete)iLod;
			scc.ListObjType = (ListObjectType)iLot;
			scc.Depth = iDepth;
			scc.HideType = strHideType;
			scc.OrgClass = iOrgClass;
			scc.OrgType = iOrgType;

			return GetOrganizationChildren2(scc);
		}

		/// <summary>
		/// ��ȡָ�������µ������Ӷ���
		/// </summary>
		/// <param name="strOrgValues">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ��0���գ�1��GUID��2��USER_GUID��3��ORIGINAL_SORT��4��GLOBAL_SORT��5��ALL_PATH_NAME��6��LOGON_NAME����</param>
		/// <param name="iLot">Ҫ���ѯ�����ݶ������ͣ��������顢��Ա����ְ����</param>
		/// <param name="iLod">�Ƿ�������߼�ɾ���ĳ�Ա</param>
		/// <param name="iDepth">Ҫ���ѯ�Ĳ�Σ�����һ�㣩��0����ȫ���Ӷ���</param>
		/// <param name="strOrgRankCodeName">��ѯ��Ҫ��Ļ������󼶱�</param>
		/// <param name="strUserRankCodeName">��ѯ��Ҫ�����Ա���󼶱�</param>
		/// <param name="strHideType">��ѯ��Ҫ�����ε�����(��Ӧ�������ļ�HideTypes.xml�е�����)</param>
		/// <param name="strAttrs">��ѯ��Ҫ���ȡ���ݶ������������</param>
		/// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
		public static DataSet GetOrganizationChildren(string strOrgValues,
			SearchObjectColumn soc,
			int iLot,
			int iLod,
			int iDepth,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strHideType,
			string strAttrs)
		{
			return GetOrganizationChildren(strOrgValues, soc, iLot, iLod, iDepth, strOrgRankCodeName, strUserRankCodeName, strHideType, strAttrs, 0, 0);
		}


		/// <summary>
		/// ��ȡָ�������µ������Ӷ���
		/// </summary>
		/// <param name="strOrgValues">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="iLot">Ҫ���ѯ�����ݶ������ͣ��������顢��Ա����ְ����</param>
		/// <param name="iLod">�Ƿ�������߼�ɾ���ĳ�Ա</param>
		/// <param name="iDepth">Ҫ���ѯ�Ĳ�Σ�����һ�㣩��0����ȫ���Ӷ���</param>
		/// <param name="strOrgRankCodeName">��ѯ��Ҫ��Ļ������󼶱�</param>
		/// <param name="strUserRankCodeName">��ѯ��Ҫ�����Ա���󼶱�</param>
		/// <param name="strAttrs">��ѯ��Ҫ���ȡ���ݶ������������</param>
		/// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
		public static DataSet GetOrganizationChildren(string strOrgValues,
			SearchObjectColumn soc,
			int iLot,
			int iLod,
			int iDepth,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strAttrs)
		{
			return GetOrganizationChildren(strOrgValues, soc, iLot, iLod, iDepth, strOrgRankCodeName, strUserRankCodeName, string.Empty, strAttrs);
		}

		/// <summary>
		/// ��ȡָ�������µ������Ӷ���
		/// </summary>
		/// <param name="strOrgValues">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="iLod">�Ƿ�������߼�ɾ���ĳ�Ա</param>
		/// <param name="iDepth">Ҫ���ѯ�Ĳ�Σ�����һ�㣩��0����ȫ���Ӷ���</param>
		/// <param name="strOrgRankCodeName">��ѯ��Ҫ��Ļ������󼶱�</param>
		/// <param name="strUserRankCodeName">��ѯ��Ҫ�����Ա���󼶱�</param>
		/// <param name="strAttrs">��ѯ��Ҫ���ȡ���ݶ������������</param>
		/// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
		public static DataSet GetOrganizationChildren(string strOrgValues,
			SearchObjectColumn soc,
			int iLod,
			int iDepth,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strAttrs)
		{
			return GetOrganizationChildren(strOrgValues, soc, (int)(ListObjectType.ORGANIZATIONS | ListObjectType.USERS), iLod, iDepth, strOrgRankCodeName, strUserRankCodeName, strAttrs);
		}

		/// <summary>
		/// ��ȡָ�������µ������Ӷ���
		/// </summary>
		/// <param name="strOrgValues">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="iDepth">Ҫ���ѯ�Ĳ�Σ�����һ�㣩��0����ȫ���Ӷ���</param>
		/// <param name="strOrgRankCodeName">��ѯ��Ҫ��Ļ������󼶱�</param>
		/// <param name="strUserRankCodeName">��ѯ��Ҫ�����Ա���󼶱�</param>
		/// <param name="strAttrs">��ѯ��Ҫ���ȡ���ݶ������������</param>
		/// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
		public static DataSet GetOrganizationChildren(string strOrgValues,
			SearchObjectColumn soc,
			int iDepth,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strAttrs)
		{
			return GetOrganizationChildren(strOrgValues, soc, (int)ListObjectDelete.COMMON, iDepth, strOrgRankCodeName, strUserRankCodeName, strAttrs);
		}

		/// <summary>
		/// ��ȡָ�������µ������Ӷ���
		/// </summary>
		/// <param name="strOrgValues">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strOrgRankCodeName">��ѯ��Ҫ��Ļ������󼶱�</param>
		/// <param name="strUserRankCodeName">��ѯ��Ҫ�����Ա���󼶱�</param>
		/// <param name="strAttrs">��ѯ��Ҫ���ȡ���ݶ������������</param>
		/// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
		public static DataSet GetOrganizationChildren(string strOrgValues,
			SearchObjectColumn soc,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strAttrs)
		{
			return GetOrganizationChildren(strOrgValues, soc, 0, strOrgRankCodeName, strUserRankCodeName, strAttrs);
		}

		/// <summary>
		/// ��ȡָ�������µ������Ӷ���
		/// </summary>
		/// <param name="strOrgValues">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strUserRankCodeName">��ѯ��Ҫ�����Ա���󼶱�</param>
		/// <param name="strAttrs">��ѯ��Ҫ���ȡ���ݶ������������</param>
		/// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
		public static DataSet GetOrganizationChildren(string strOrgValues,
			SearchObjectColumn soc,
			string strUserRankCodeName,
			string strAttrs)
		{
			return GetOrganizationChildren(strOrgValues, soc, string.Empty, strUserRankCodeName, strAttrs);
		}

		/// <summary>
		/// ��ȡָ�������µ������Ӷ���
		/// </summary>
		/// <param name="strOrgValues">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strAttrs">��ѯ��Ҫ���ȡ���ݶ������������</param>
		/// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
		public static DataSet GetOrganizationChildren(string strOrgValues,
			SearchObjectColumn soc,
			string strAttrs)
		{
			return GetOrganizationChildren(strOrgValues, soc, string.Empty, strAttrs);
		}

		/// <summary>
		/// ��ȡָ�������µ������Ӷ���
		/// </summary>
		/// <param name="strOrgGuids">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
		/// <param name="strAttrs">��ѯ��Ҫ���ȡ���ݶ������������</param>
		/// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
		public static DataSet GetOrganizationChildren(string strOrgGuids, string strAttrs)
		{
			return GetOrganizationChildren(strOrgGuids, SearchObjectColumn.SEARCH_GUID, strAttrs);
		}

		/// <summary>
		/// ��ȡָ�������µ������Ӷ���
		/// </summary>
		/// <param name="strOrgGuids">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
		/// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
		public static DataSet GetOrganizationChildren(string strOrgGuids)
		{
			return GetOrganizationChildren(strOrgGuids, string.Empty);
		}

		/// <summary>
		/// ����һ���Ĳ�ѯ������ѯϵͳ�е����ݶ���
		/// </summary>
		/// <param name="scc">ϵͳ�Ĳ�ѯ��������</param>
		/// <returns>����һ���Ĳ�ѯ������ѯϵͳ�е����ݶ���</returns>
		/// 
		//2009-05-11
		public static DataSet GetOrganizationChildren(SearchOrgChildrenCondition scc)
		{
			string searchKey = scc.GetHashString();
#if DEBUG
			long cast = DateTime.Now.Ticks;
			Trace.WriteLine(searchKey);
#endif
			DataSet result;

			string strRootGuids = TransHashToSqlString(scc.RootGuids).Trim('\'');
			//�õ�All_Path_Name
			DataSet dsRootOrg = OGUReader.GetObjectsDetail("ORGANIZATIONS",
						strRootGuids,
						SearchObjectColumn.SEARCH_GUID,
						string.Empty,
						SearchObjectColumn.SEARCH_GUID);

			ExceptionHelper.FalseThrow(dsRootOrg.Tables[0].Rows.Count > 0, "�����ҵ�IDΪ{0}�Ļ���", strRootGuids);
			string rootPath = dsRootOrg.Tables[0].Rows[0]["ORIGINAL_SORT"].ToString();

			if (false == Caching.GetOrganizationChildrenQueue.Instance.TryGetValue(searchKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					Database database = DatabaseFactory.Create(context);
					StringBuilder strB = new StringBuilder(1024);
					if ((scc.ListObjType & ListObjectType.ORGANIZATIONS) != ListObjectType.None)
					{
						strB.Append(" ( " + GetOrganizationsSqlByScc(scc, rootPath) + " \n )");
					}

					if ((scc.ListObjType & ListObjectType.GROUPS) != ListObjectType.None)
					{
						if (strB.Length > 0)
							strB.Append(" \n UNION \n ");
						strB.Append(" ( " + GetGroupsSqlByScc(scc, rootPath) + " \n )");
					}

					if ((scc.ListObjType & ListObjectType.USERS) != ListObjectType.None)
					{
						if (strB.Length > 0)
							strB.Append(" \n UNION \n ");

						strB.Append(" ( " + GetUsersSqlByScc(scc, rootPath) + " \n )");
					}

					string strSql = "SELECT * FROM ( " + strB.ToString() + " ) RESULT ORDER BY GLOBAL_SORT";

					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				Caching.GetOrganizationChildrenQueue.Instance.Add(searchKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
#if DEBUG
			cast = DateTime.Now.Ticks - cast;
			Trace.WriteLine(cast.ToString(), "Cast Time");
#endif
			return result;
		}

		/// <summary>
		/// ����һ���Ĳ�ѯ������ѯϵͳ�е����ݶ���
		/// </summary>
		/// <param name="scc">ϵͳ�Ĳ�ѯ��������</param>
		/// <returns>����һ���Ĳ�ѯ������ѯϵͳ�е����ݶ���</returns>
		/// 
		//2009-05-07
		public static DataSet GetOrganizationChildren2(SearchOrgChildrenCondition scc)
		{
			string searchKey = scc.GetHashString();
#if DEBUG
			long cast = DateTime.Now.Ticks;
			Trace.WriteLine(searchKey);
#endif
			DataSet result;

			string strRootGuids = TransHashToSqlString(scc.RootGuids).Trim('\'');
			//�õ�All_Path_Name
			DataSet dsRootOrg = OGUReader.GetObjectsDetail("ORGANIZATIONS",
						strRootGuids,
						SearchObjectColumn.SEARCH_GUID,
						string.Empty,
						SearchObjectColumn.SEARCH_GUID);

			ExceptionHelper.FalseThrow(dsRootOrg.Tables[0].Rows.Count > 0, "�����ҵ�IDΪ{0}�Ļ���", strRootGuids);
			string rootPath = dsRootOrg.Tables[0].Rows[0]["ORIGINAL_SORT"].ToString();


			if (false == Caching.GetOrganizationChildrenQueue.Instance.TryGetValue(searchKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					Database database = DatabaseFactory.Create(context);
					StringBuilder strB = new StringBuilder(1024);
					if ((scc.ListObjType & ListObjectType.ORGANIZATIONS) != ListObjectType.None)
					{
						strB.Append(" ( " + GetOrganizationsSqlByScc2(scc, rootPath) + " \n )");
					}

					if ((scc.ListObjType & ListObjectType.GROUPS) != ListObjectType.None)
					{
						if (strB.Length > 0)
							strB.Append(" \n UNION \n ");
						strB.Append(" ( " + GetGroupsSqlByScc2(scc, rootPath) + " \n )");
					}

					if ((scc.ListObjType & ListObjectType.USERS) != ListObjectType.None)
					{
						if (strB.Length > 0)
							strB.Append(" \n UNION \n ");

						strB.Append(" ( " + GetUsersSqlByScc2(scc, rootPath) + " \n )");
					}

					string strSql = "SELECT * FROM ( " + strB.ToString() + " ) RESULT ORDER BY GLOBAL_SORT";

					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				Caching.GetOrganizationChildrenQueue.Instance.Add(searchKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
#if DEBUG
			cast = DateTime.Now.Ticks - cast;
			Trace.WriteLine(cast.ToString(), "Cast Time");
#endif
			return result;
		}

		#endregion

		#region IsUserInObjects
		///// <summary>
		///// ��ȡָ�������µ������Ӷ���
		///// </summary>
		///// <param name="strOrgValues">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
		///// <param name="soc">��ѯҪ��Ĳ�ѯ������
		///// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		///// <param name="iLot">Ҫ���ѯ�����ݶ������ͣ��������顢��Ա����ְ����</param>
		///// <param name="iLod">�Ƿ�������߼�ɾ���ĳ�Ա</param>
		///// <param name="iDepth">Ҫ���ѯ�Ĳ�Σ�����һ�㣩��0����ȫ���Ӷ���</param>
		///// <param name="strOrgRankCodeName">��ѯ��Ҫ��Ļ������󼶱�</param>
		///// <param name="strUserRankCodeName">��ѯ��Ҫ�����Ա���󼶱�</param>
		///// <param name="strHideType">��ѯ��Ҫ�����ε�����(��Ӧ�������ļ�HideTypes.xml�е�����)</param>
		///// <param name="strAttrs">��ѯ��Ҫ���ȡ���ݶ������������</param>
		///// <param name="iOrgClass">Ҫ��չ�ֻ���������</param>
		///// <param name="iOrgType">Ҫ��չ�ֻ���������</param>
		///// <param name="da">���ݿ��������</param>
		///// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
		//public static DataSet GetOrganizationChildren(string strOrgValues, SearchObjectColumn soc, int iLot, int iLod, int iDepth, string strOrgRankCodeName, string strUserRankCodeName, string strHideType, string strAttrs, int iOrgClass, int iOrgType, DataAccess da)
		//{
		//    strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);

		//    SearchOrgChildrenCondition scc = new SearchOrgChildrenCondition(strOrgValues, soc, strAttrs, da);
		//    scc.ListObjDelete = (ListObjectDelete)iLod;
		//    scc.ListObjType = (ListObjectType)iLot;
		//    scc.Depth = iDepth;
		//    scc.OrgRankCN = strOrgRankCodeName;
		//    scc.UserRankCN = strUserRankCodeName;
		//    scc.HideType = strHideType;
		//    scc.OrgClass = iOrgClass;
		//    scc.OrgType = iOrgType;
		//    return GetOrganizationChildren(scc, da);
		//}

		///// <summary>
		///// ��ȡָ�������µ������Ӷ���
		///// </summary>
		///// <param name="strOrgValues">Ҫ���ѯ�Ĳ��Ŷ���(�����ű�ʶ,���֮�����","�ָ�)</param>
		///// <param name="soc">��ѯҪ��Ĳ�ѯ������
		///// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		///// <param name="iLot">Ҫ���ѯ�����ݶ������ͣ��������顢��Ա����ְ����</param>
		///// <param name="iLod">�Ƿ�������߼�ɾ���ĳ�Ա</param>
		///// <param name="iDepth">Ҫ���ѯ�Ĳ�Σ�����һ�㣩��0����ȫ���Ӷ���</param>
		///// <param name="strOrgRankCodeName">��ѯ��Ҫ��Ļ������󼶱�</param>
		///// <param name="strUserRankCodeName">��ѯ��Ҫ�����Ա���󼶱�</param>
		///// <param name="strHideType">��ѯ��Ҫ�����ε�����(��Ӧ�������ļ�HideTypes.xml�е�����)</param>
		///// <param name="strAttrs">��ѯ��Ҫ���ȡ���ݶ������������</param>
		///// <param name="da">���ݿ��������</param>
		///// <returns>��ȡָ�������µ������Ӷ���Ĳ�ѯ���</returns>
		//public static DataSet GetOrganizationChildren(string strOrgValues, SearchObjectColumn soc, int iLot, int iLod, int iDepth, string strOrgRankCodeName, string strUserRankCodeName, string strHideType, string strAttrs, DataAccess da)
		//{
		//    return GetOrganizationChildren(strOrgValues, soc, iLot, iLod, iDepth, strOrgRankCodeName, strUserRankCodeName, strHideType, strAttrs, 0, 0, da);
		//}

		///// <summary>
		///// ����һ���Ĳ�ѯ������ѯϵͳ�е����ݶ���
		///// </summary>
		///// <param name="scc">ϵͳ�Ĳ�ѯ��������</param>
		///// <param name="da">���ݿ��������</param>
		///// <returns>����һ���Ĳ�ѯ������ѯϵͳ�е����ݶ���</returns>
		//public static DataSet GetOrganizationChildren(SearchOrgChildrenCondition scc, DataAccess da)
		//{
		//    StringBuilder strB = new StringBuilder(1024);
		//    if ((scc.ListObjType & ListObjectType.ORGANIZATIONS) != 0)
		//    {
		//        strB.Append(" ( " + GetOrganizationsSqlByScc(scc, da) + " \n )");
		//    }

		//    if ((scc.ListObjType & ListObjectType.GROUPS) != 0)
		//    {
		//        if (strB.Length > 0)
		//            strB.Append(" \n UNION \n ");
		//        strB.Append(" ( " + GetGroupsSqlByScc(scc, da) + " \n )");
		//    }

		//    if ((scc.ListObjType & ListObjectType.USERS) != 0)
		//    {
		//        if (strB.Length > 0)
		//            strB.Append(" \n UNION \n ");

		//        strB.Append(" ( " + GetUsersSqlByScc(scc, da) + " \n )");
		//    }

		//    string strSql = "SELECT * FROM ( " + strB.ToString() + " ) RESULT ORDER BY GLOBAL_SORT";

		//    return OGUCommonDefine.ExecuteDataset(strSql, da);
		//}

		/// <summary>
		/// �ж�һ���û��Ƿ������ָ���Ķ������֮��
		/// </summary>
		/// <param name="strUserValue">�û�����������ֵ</param>
		/// <param name="socu">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strUserParentGuid">ָ���û����ڵĸ����ű�ʶ���ɿգ�</param>
		/// <param name="objectXmlDoc">�ж϶������������ֵ</param>
		/// <param name="soc">��������������
		/// ���ǣգɣġ��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="lod">�Ƿ�������߼�ɾ���ĳ�Ա</param>
		/// <param name="strHideType">��ѯ��Ҫ�����ε�����(��Ӧ�������ļ�HideTypes.xml�е�����)</param>
		/// <param name="bDirect">�Ƿ�ֱ�Ӵ��������м䲿�ţ�</param>
		/// <param name="bFitAll">�Ƿ�Ҫ����ȫƥ�䣨������ָ����ÿһ�������У�</param>
		/// <returns>�ж�һ���û��Ƿ������ָ���Ķ������֮��</returns>
		/// <remarks>
		/// objectXmlDoc�Ľṹ���£�
		/// <code>
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// </remarks>
		public static bool IsUserInObjects(string strUserValue,
			SearchObjectColumn socu,
			string strUserParentGuid,
			XmlDocument objectXmlDoc,
			SearchObjectColumn soc,
			ListObjectDelete lod,
			string strHideType,
			bool bDirect,
			bool bFitAll)
		{
			string cacheKey = Common.InnerCacheHelper.BuildCacheKey(strUserValue,
				socu,
				strUserParentGuid,
				objectXmlDoc,
				soc,
				lod,
				strHideType,
				bDirect,
				bFitAll);

			bool result;
			//if (false == IsUserInObjectsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(IsUserInObjectsQueue))
			//    {
			if (false == IsUserInObjectsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string strUserColName = OGUCommonDefine.GetSearchObjectColumn(socu);
				string strObjColName = OGUCommonDefine.GetSearchObjectColumn(soc);

				strHideType = OGUCommonDefine.GetHideType(strHideType);
				string strHideList = GetHideTypeFromXmlForLike(strHideType, "OU_USERS");

				string strDirect = "%";
				if (bDirect)
					strDirect = "______";

				string strUserLimit = " AND " + DatabaseSchema.Instence.GetTableColumns(strUserColName, "OU_USERS", "USERS")
					+ " = " + TSqlBuilder.Instance.CheckQuotationMark(strUserValue, true);
				if (strUserParentGuid.Length > 0)
					strUserLimit += "	AND OU_USERS.PARENT_GUID = " + TSqlBuilder.Instance.CheckQuotationMark(strUserParentGuid, true);

				string strDelUser = GetSqlSearchStatus("OU_USERS", lod);
				StringBuilder strB = new StringBuilder(1024);
				#region �ڲ�ʵ��
				XmlElement root = objectXmlDoc.DocumentElement;
				foreach (XmlElement elem in root.ChildNodes)
				{
					string strRankLimit = string.Empty;
					if (elem.GetAttribute("rankCode") != null && elem.GetAttribute("rankCode").Length > 0)
						strRankLimit = " AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
							+ TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("rankCode"), true) + ") ";

					switch (elem.LocalName)
					{
						case "ORGANIZATIONS":
							strB.Append(@"
							SELECT DISTINCT ORGANIZATIONS.GUID, OU_USERS.USER_GUID 
							FROM OU_USERS, ORGANIZATIONS, USERS JOIN RANK_DEFINE 
								ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
								" + strRankLimit + @"
							WHERE OU_USERS.USER_GUID = USERS.GUID
								AND OU_USERS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + "
									  + TSqlBuilder.Instance.CheckQuotationMark(strDirect, true) + @" 
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "ORGANIZATIONS") + @" = "
										  + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true) + @"
								AND (" + strDelUser + @") 
								" + strUserLimit + strHideList + @";");
							break;
						case "GROUPS":
							strB.Append(@"
							SELECT DISTINCT GROUPS.GUID, OU_USERS.USER_GUID
							FROM GROUPS, GROUP_USERS, OU_USERS, USERS JOIN RANK_DEFINE 
								ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
								" + strRankLimit + @"
							WHERE GROUPS.GUID = GROUP_USERS.GROUP_GUID
								AND OU_USERS.USER_GUID = GROUP_USERS.USER_GUID
								AND OU_USERS.PARENT_GUID = GROUP_USERS.USER_PARENT_GUID
								AND OU_USERS.USER_GUID = USERS.GUID
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "GROUPS") + @" = "
										  + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true) + @"
								AND (" + strDelUser + @") 
								" + strUserLimit + strHideList + @";");
							break;
						case "USERS":
							strB.Append(@"
							SELECT DISTINCT USERS.GUID, OU_USERS.USER_GUID
							FROM OU_USERS, USERS
							WHERE OU_USERS.USER_GUID = USERS.GUID
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "OU_USERS", "USERS") + " =  "
										  + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true) + @"
								AND (" + strDelUser + @")" + strUserLimit + strHideList + @";");
							break;
						default: ExceptionHelper.TrueThrow(true, "�Բ���,ϵͳû�ж�Ӧ����" + elem.LocalName + "������Ӧ����");
							break;
					}
				}
				#endregion
				result = CheckIsUserInOrganizations(strB, bFitAll);

				IsUserInObjectsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}

			return result;
		}
		///// <summary>
		///// �ж�һ���û��Ƿ������ָ���Ķ������֮��
		///// </summary>
		///// <param name="strUserValue">�û�����������ֵ</param>
		///// 
		///// <param name="socu">�û�����������
		///// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		///// </param>
		///// <param name="strUserParentGuid">ָ���û����ڵĸ����ű�ʶ���ɿգ�</param>
		///// <param name="objectXmlDoc">�ж϶������������ֵ</param>
		///// <param name="soco">��������������
		///// ���ǣգɣġ��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		///// </param>
		///// <param name="lod">�Ƿ�������߼�ɾ���ĳ�Ա</param>
		///// <param name="strHideType">��ѯ��Ҫ�����ε�����(��Ӧ�������ļ�HideTypes.xml�е�����)</param>
		///// <param name="bDirect">�Ƿ�ֱ�Ӵ��������м䲿�ţ�</param>
		///// <param name="bFitAll">�Ƿ�Ҫ����ȫƥ�䣨������ָ����ÿһ�������У�</param>
		///// <returns>�ж�һ���û��Ƿ������ָ���Ķ������֮��</returns>
		//public static bool IsUserInObjects(string strUserValue, SearchObjectColumn socu, string strUserParentGuid, XmlDocument objectXmlDoc, SearchObjectColumn soco, ListObjectDelete lod, string strHideType, bool bDirect, bool bFitAll)
		//{
		//    using (DbContext context = DbContext.GetContext(AccreditResource.ConnAlias))
		//    {
		//        return IsUserInObjects(strUserValue, socu, strUserParentGuid, objectXmlDoc, soco, lod, strHideType, bDirect, bFitAll, da);
		//    }
		//}

		/// <summary>
		/// �ж�һ���û��Ƿ������ָ���Ķ������֮��
		/// </summary>
		/// <param name="strUserValue">�û�����������ֵ</param>
		/// 
		/// <param name="socu">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strUserParentGuid">ָ���û����ڵĸ����ű�ʶ���ɿգ�</param>
		/// <param name="objectXmlDoc">�ж϶������������ֵ</param>
		/// <param name="soco">��������������
		/// ���ǣգɣġ��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="lod">�Ƿ�������߼�ɾ���ĳ�Ա</param>
		/// <param name="bDirect">�Ƿ�ֱ�Ӵ��������м䲿�ţ�</param>
		/// <param name="bFitAll">�Ƿ�Ҫ����ȫƥ�䣨������ָ����ÿһ�������У�</param>
		/// <returns>�ж�һ���û��Ƿ������ָ���Ķ������֮��</returns>
		public static bool IsUserInObjects(string strUserValue,
			SearchObjectColumn socu,
			string strUserParentGuid,
			XmlDocument objectXmlDoc,
			SearchObjectColumn soco,
			ListObjectDelete lod,
			bool bDirect,
			bool bFitAll)
		{
			return IsUserInObjects(strUserValue, socu, strUserParentGuid, objectXmlDoc, soco, lod, string.Empty, bDirect, bFitAll);
		}

		/// <summary>
		/// �ж�һ���û��Ƿ������ָ���Ķ������֮��
		/// </summary>
		/// <param name="strUserValue">�û�����������ֵ</param>
		/// 
		/// <param name="socu">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strUserParentGuid">ָ���û����ڵĸ����ű�ʶ���ɿգ�</param>
		/// <param name="objectXmlDoc">�ж϶������������ֵ</param>
		/// <param name="soco">��������������
		/// ���ǣգɣġ��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="bDirect">�Ƿ�ֱ�Ӵ��������м䲿�ţ�</param>
		/// <param name="bFitAll">�Ƿ�Ҫ����ȫƥ�䣨������ָ����ÿһ�������У�</param>
		/// <returns>�ж�һ���û��Ƿ������ָ���Ķ������֮��</returns>
		public static bool IsUserInObjects(string strUserValue,
			SearchObjectColumn socu,
			string strUserParentGuid,
			XmlDocument objectXmlDoc,
			SearchObjectColumn soco,
			bool bDirect,
			bool bFitAll)
		{
			return IsUserInObjects(strUserValue, socu, strUserParentGuid, objectXmlDoc, soco, ListObjectDelete.COMMON, bDirect, bFitAll);
		}

		/// <summary>
		/// �ж�һ���û��Ƿ������ָ���Ķ������֮��
		/// </summary>
		/// <param name="strUserValue">�û�����������ֵ</param>
		/// 
		/// <param name="socu">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strUserParentGuid">ָ���û����ڵĸ����ű�ʶ���ɿգ�</param>
		/// <param name="objectXmlDoc">�ж϶������������ֵ</param>
		/// <param name="soco">��������������
		/// ���ǣգɣġ��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="bFitAll">�Ƿ�Ҫ����ȫƥ�䣨������ָ����ÿһ�������У�</param>
		/// <returns>�ж�һ���û��Ƿ������ָ���Ķ������֮��</returns>
		public static bool IsUserInObjects(string strUserValue,
			SearchObjectColumn socu,
			string strUserParentGuid,
			XmlDocument objectXmlDoc,
			SearchObjectColumn soco,
			bool bFitAll)
		{
			return IsUserInObjects(strUserValue, socu, strUserParentGuid, objectXmlDoc, soco, false, bFitAll);
		}

		/// <summary>
		/// �ж�һ���û��Ƿ������ָ���Ķ������֮��
		/// </summary>
		/// <param name="strUserValue">�û�����������ֵ</param>
		/// <param name="socu">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strUserParentGuid">ָ���û����ڵĸ����ű�ʶ���ɿգ�</param>
		/// <param name="objectXmlDoc">�ж϶������������ֵ</param>
		/// <param name="soco">��������������
		/// ���ǣգɣġ��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <returns>�ж�һ���û��Ƿ������ָ���Ķ������֮��</returns>
		public static bool IsUserInObjects(string strUserValue,
			SearchObjectColumn socu,
			string strUserParentGuid,
			XmlDocument objectXmlDoc,
			SearchObjectColumn soco)
		{
			return IsUserInObjects(strUserValue, socu, strUserParentGuid, objectXmlDoc, soco, false);
		}

		/// <summary>
		/// �ж�һ���û��Ƿ������ָ���Ķ������֮��
		/// </summary>
		/// <param name="strUserGuid">�û�����������ֵ</param>
		/// <param name="strUserParentGuid">ָ���û����ڵĸ����ű�ʶ���ɿգ�</param>
		/// <param name="objectXmlDoc">�ж϶������������ֵ</param>
		/// <param name="soco">��������������
		/// ���ǣգɣġ��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <returns>�ж�һ���û��Ƿ������ָ���Ķ������֮��</returns>
		public static bool IsUserInObjects(string strUserGuid,
			string strUserParentGuid,
			XmlDocument objectXmlDoc,
			SearchObjectColumn soco)
		{
			return IsUserInObjects(strUserGuid, SearchObjectColumn.SEARCH_GUID, strUserParentGuid, objectXmlDoc, soco);
		}

		/// <summary>
		/// �ж�һ���û��Ƿ������ָ���Ķ������֮��
		/// </summary>
		/// <param name="strUserGuid">�û�����������ֵ</param>
		/// <param name="strUserParentGuid">ָ���û����ڵĸ����ű�ʶ���ɿգ�</param>
		/// <param name="objectXmlDoc">�ж϶������������ֵ</param>
		/// <returns>�ж�һ���û��Ƿ������ָ���Ķ������֮��</returns>
		public static bool IsUserInObjects(string strUserGuid,
			string strUserParentGuid,
			XmlDocument objectXmlDoc)
		{
			return IsUserInObjects(strUserGuid, strUserParentGuid, objectXmlDoc, SearchObjectColumn.SEARCH_GUID);
		}

		/// <summary>
		/// �ж�һ���û��Ƿ������ָ���Ķ������֮��
		/// </summary>
		/// <param name="strUserGuid">�û�����������ֵ</param>
		/// <param name="objectXmlDoc">�ж϶������������ֵ</param>
		/// <returns>�ж�һ���û��Ƿ������ָ���Ķ������֮��</returns>
		public static bool IsUserInObjects(string strUserGuid,
			XmlDocument objectXmlDoc)
		{
			return IsUserInObjects(strUserGuid, string.Empty, objectXmlDoc);
		}

		#endregion

		#region CheckUserInObjects
		/// <summary>
		/// �ж��û�Ⱥ�Ƿ������ָ���Ķ������֮��
		/// </summary>
		/// <param name="xmlUserDoc">�û�Ⱥ��ʶ�����֮�����","�ָ���</param>
		/// <param name="socu">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="xmlObjDoc">����Ⱥ������XML��ʽ��</param>
		/// <param name="soc">��������������
		/// ���ǣգɣġ��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="lod">�Ƿ�������߼�ɾ���ĳ�Ա</param>
		/// <param name="strHideType">��ѯ��Ҫ�����ε�����(��Ӧ�������ļ�HideTypes.xml�е�����)</param>
		/// <param name="bDirect">�Ƿ�ֱ�Ӵ��������м䲿�ţ�</param>
		/// <remarks>
		/// <code>
		/// xmlUserDoc�Ľṹ���£�˵��oValue�����socu���ʹ�ã�parentGuid�ɲ����
		///		<USERS>
		///			<USERS oValue="" parentGuid="" />
		///			<USERS oValue="" parentGuid="" />
		///		</USERS>
		/// xmlObjDoc�Ľṹ���£�˵��oValue�����soc���ʹ�ã�parentGuid�ɲ��rankCode�ɲ����
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// xmlObjDoc�ķ��ؽ�����ֽڵ㷽ʽǶ�뷵�أ���
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</ORGANIZATIONS>
		///			<GROUPS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</GROUPS>
		///			<USERS oValue="" parentGuid="" >
		///				<USERS oValue="" parentGuid=""/>
		///			</USERS>
		///		</OBJECTS>
		/// </remarks>
		public static void CheckUserInObjects(XmlDocument xmlUserDoc,
			SearchObjectColumn socu,
			XmlDocument xmlObjDoc,
			SearchObjectColumn soc,
			ListObjectDelete lod,
			string strHideType,
			bool bDirect)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(xmlUserDoc.DocumentElement.OuterXml,
				socu,
				xmlObjDoc.DocumentElement.OuterXml,
				soc,
				lod,
				strHideType,
				bDirect);
			XmlDocument result;
#if DEBUG
			Debug.WriteLine(xmlObjDoc.OuterXml, "begin");
#endif
			//if (false == CheckUserInObjectsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(CheckUserInObjectsQueue))
			//    {
			if (false == CheckUserInObjectsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string strUserColName = OGUCommonDefine.GetSearchObjectColumn(socu);
				string strObjColName = OGUCommonDefine.GetSearchObjectColumn(soc);

				strHideType = OGUCommonDefine.GetHideType(strHideType);
				string strHideList = GetHideTypeFromXmlForLike(strHideType, "OU_USERS");

				string strDelUser = GetSqlSearchStatus("OU_USERS", lod);

				string strDirect = "%";
				if (bDirect)
					strDirect = "______";
				#region Db control
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					string strUserLimit = GetUserLimitInCheckUserInOrganizations(xmlUserDoc, strUserColName);
					/*****************************************************************************************************************/
					string strExtraWhere = @"
								AND (" + strDelUser + @") 
								AND " + strUserLimit + strHideList;
					StringBuilder strB = GetSqlSearchForCheckUserInObjects(xmlObjDoc, strUserColName, strObjColName, strDirect, strExtraWhere);

					if (strB.Length > 0)
					{
						Database database = DatabaseFactory.Create(context);
						DataSet ds = database.ExecuteDataSet(CommandType.Text, strB.ToString());
						foreach (DataTable oTable in ds.Tables)
						{
							foreach (DataRow row in oTable.Rows)
							{
								XmlNode uNode = xmlUserDoc.DocumentElement.SelectSingleNode("USERS[@oValue=\"" + OGUCommonDefine.DBValueToString(row["USER_VALUE"]) + "\"]");
								if (uNode != null)
								{
									XmlElement oRoot = xmlObjDoc.DocumentElement;
									string oClass = OGUCommonDefine.DBValueToString(row["OBJECTCLASS"]);
									string oValue = OGUCommonDefine.DBValueToString(row["OBJ_VALUE"]);

									foreach (XmlElement oElem in oRoot.SelectNodes(oClass + "[@oValue=\"" + oValue + "\"]"))
									{
										if (row["RANK_CODE"] is DBNull)
										{
											if (oElem.GetAttribute("rankCode") == string.Empty)
											{
												XmlElement uElem = (XmlElement)XmlHelper.AppendNode(oElem, uNode.LocalName);
												foreach (XmlAttribute xAttr in uNode.Attributes)
													uElem.SetAttribute(xAttr.LocalName, xAttr.InnerText);
											}
										}
										else
										{
											if (oElem.GetAttribute("rankCode") == OGUCommonDefine.DBValueToString(row["RANK_CODE"]))
											{
												XmlElement uElem = (XmlElement)XmlHelper.AppendNode(oElem, uNode.LocalName);
												foreach (XmlAttribute xAttr in uNode.Attributes)
													uElem.SetAttribute(xAttr.LocalName, xAttr.InnerText);
											}
										}
									}
								}
							}
						}
					}
				}
				#endregion

				#region Deleted
				/*****************************************************************************************************************/
				/*******����һ�γ���û�д��󣬵����������в��졣���³������ÿ��OBJ�������һ��SQL��ѯ��䣬********************/
				/*******�ڶ���������µ���SQL��ѯ�����Ӵ��޸Ĳ���GetSqlSearchForCheckUserInObjects**************************/
				/*****************************************************************************************************************/
				//			foreach (XmlElement elem in xmlObjDoc.DocumentElement.ChildNodes)
				//			{
				//				string strRankLimit = elem.GetAttribute("rankCode");
				//				if (strRankLimit != null && strRankLimit.Length > 0)
				//					strRankLimit = " AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = " + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("rankCode")) + ") ";
				//				else
				//					strRankLimit = string.Empty;
				//
				//				string strSql = string.Empty;
				//				switch (elem.LocalName)
				//				{
				//					case "ORGANIZATIONS":
				//						strSql = @"
				//							SELECT DISTINCT " + GetTableColumns(strUserColName, da, "USERS", "OU_USERS") + " AS USER_VALUE, " + GetTableColumns(strObjColName, da, "ORGANIZATIONS") + @" AS OBJ_VALUE
				//							FROM ORGANIZATIONS, OU_USERS, USERS JOIN RANK_DEFINE ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
				//							WHERE OU_USERS.USER_GUID = USERS.GUID
				//								AND OU_USERS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + " + TSqlBuilder.Instance.CheckQuotationMark(strDirect) + @"
				//								AND (" + strDelUser + @")
				//								AND " + GetTableColumns(strObjColName, da, "ORGANIZATIONS") + " = " + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue")) + @"
				//								AND " + strUserLimit + @"
				//								" + strRankLimit + strHideList + @"
				//							ORDER BY OBJ_VALUE";
				//						break;
				//					case "GROUPS":
				//						strSql = @"
				//							SELECT DISTINCT " + GetTableColumns(strUserColName, da, "USERS", "OU_USERS") + " AS USER_VALUE, " + GetTableColumns(strObjColName, da, "GROUPS") + @" AS OBJ_VALUE
				//							FROM GROUPS, GROUP_USERS, OU_USERS, USERS JOIN RANK_DEFINE ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
				//							WHERE GROUPS.GUID = GROUP_USERS.GROUP_GUID
				//								AND GROUP_USERS.USER_GUID = OU_USERS.USER_GUID
				//								AND GROUP_USERS.USER_PARENT_GUID = OU_USERS.PARENT_GUID
				//								AND OU_USERS.USER_GUID = USERS.GUID
				//								AND GROUPS." + strObjColName + " = " + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue")) + @"
				//								AND ( " + strDelUser + @" ) 
				//								AND " + strUserLimit + strHideList + @"
				//								" + strRankLimit + @"
				//							ORDER BY OBJ_VALUE";
				//						break;
				//					case "USERS":
				//						strSql = @"
				//							SELECT DISTINCT " + GetTableColumns(strUserColName, da, "USERS", "OU_USERS") + " AS USER_VALUE, " + GetTableColumns(strObjColName, da, "USERS", "OU_USERS") + @" AS OBJ_VALUE
				//							FROM OU_USERS, USERS
				//							WHERE OU_USERS.USER_GUID = USERS.GUID
				//								AND " + GetTableColumns(strObjColName, da, "USERS", "OU_USERS") + " = " + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue")) + @"
				//								AND ( " + strDelUser + @" ) 
				//								AND " + strUserLimit + strHideList + @"
				//							ORDER BY OBJ_VALUE ";
				//						break;
				//					default :	ExceptionHelper.TrueThrow(true, "�Բ���,ϵͳû�ж�Ӧ����" + elem.LocalName + "������Ӧ����");
				//						break;
				//				}
				//
				//				strB.Append(strSql + ";\n");
				//			}
				//			if (strB.Length > 0)
				//			{
				//				DataSet ds = OGUCommonDefine.ExecuteDataset(strB.ToString(), da);
				//				for (int iTable = 0; iTable < ds.Tables.Count; iTable++)
				//				{
				//					foreach (DataRow row in ds.Tables[iTable].Rows)
				//					{
				//						XmlNode uNode = xmlUserDoc.DocumentElement.SelectSingleNode("USERS[@oValue=\"" + XmlHelper.DBValueToString(row["USER_VALUE"]) + "\"]");
				//						if (uNode != null)
				//						{						
				//							XmlElement oElem = (XmlElement)XmlHelper.AppendNode(xmlObjDoc, xmlObjDoc.DocumentElement.ChildNodes[iTable], uNode.LocalName);
				//							foreach (XmlAttribute xAttr in uNode.Attributes)
				//								oElem.SetAttribute(xAttr.LocalName, xAttr.InnerText);
				//						}
				//					}
				//				}
				//			}
				/*****************************************************************************************************************/
				#endregion

				result = xmlObjDoc;
				CheckUserInObjectsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			xmlObjDoc = result;
#if DEBUG
			Debug.WriteLine(xmlObjDoc.OuterXml, "result");
#endif
		}
		///// <summary>
		///// �ж��û�Ⱥ�Ƿ������ָ���Ķ������֮��
		///// </summary>
		///// <param name="xmlUserDoc">�û�Ⱥ��ʶ������XML��ʽ��</param>
		///// <param name="socu">�û�����������
		///// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		///// <param name="xmlObjDoc">����Ⱥ������XML��ʽ��</param>
		///// <param name="soc">��������������
		///// ���ǣգɣġ��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		///// <param name="lod">�Ƿ�������߼�ɾ���ĳ�Ա</param>
		///// <param name="strHideType">��ѯ��Ҫ�����ε�����(��Ӧ�������ļ�HideTypes.xml�е�����)</param>
		///// <param name="bDirect">�Ƿ�ֱ�Ӵ��������м䲿�ţ�</param>
		///// <remarks>
		///// <code>
		///// xmlUserDoc�Ľṹ���£�˵��oValue�����socu���ʹ�ã�parentGuid�ɲ����
		/////		<USERS>
		/////			<USERS oValue="" parentGuid="" />
		/////			<USERS oValue="" parentGuid="" />
		/////		</USERS>
		///// xmlObjDoc�Ľṹ���£�˵��oValue�����soc���ʹ�ã�parentGuid�ɲ��rankCode�ɲ����
		/////		<OBJECTS>
		/////			<ORGANIZATIONS oValue="" rankCode="" />
		/////			<GROUPS oValue="" rankCode="" />
		/////			<USERS oValue="" parentGuid="" />
		/////		</OBJECTS>
		///// </code>
		///// xmlObjDoc�ķ��ؽ�����ֽڵ㷽ʽǶ�뷵�أ���
		///// <OBJECTS>
		/////			<ORGANIZATIONS oValue="" rankCode="" >
		/////				<USERS oValue="" parentGuid=""/>
		/////				<USERS oValue="" parentGuid=""/>
		/////			</ORGANIZATIONS>
		/////			<GROUPS oValue="" rankCode="" >
		/////				<USERS oValue="" parentGuid=""/>
		/////				<USERS oValue="" parentGuid=""/>
		/////			</GROUPS>
		/////			<USERS oValue="" parentGuid="" >
		/////				<USERS oValue="" parentGuid=""/>
		/////			</USERS>
		/////		</OBJECTS>
		///// </remarks>
		//public static void CheckUserInObjects(XmlDocument xmlUserDoc, SearchObjectColumn socu, XmlDocument xmlObjDoc, SearchObjectColumn soc, ListObjectDelete lod, string strHideType, bool bDirect)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        CheckUserInObjects(xmlUserDoc, socu, xmlObjDoc, soc, lod, strHideType, bDirect, da);
		//    }
		//}

		/// <summary>
		/// �ж��û�Ⱥ�Ƿ������ָ���Ķ������֮��
		/// </summary>
		/// <param name="xmlUserDoc">�û�Ⱥ��ʶ������XML��ʽ��</param>
		/// <param name="socu">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="xmlObjDoc">����Ⱥ������XML��ʽ��</param>
		/// <param name="soc">��������������
		/// ���ǣգɣġ��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="lod">�Ƿ�������߼�ɾ���ĳ�Ա</param>
		/// <param name="bDirect">�Ƿ�ֱ�Ӵ��������м䲿�ţ�</param>
		/// <remarks>
		/// <code>
		/// xmlUserDoc�Ľṹ���£�˵��oValue�����socu���ʹ�ã�parentGuid�ɲ����
		///		<USERS>
		///			<USERS oValue="" parentGuid="" />
		///			<USERS oValue="" parentGuid="" />
		///		</USERS>
		/// xmlObjDoc�Ľṹ���£�˵��oValue�����soc���ʹ�ã�parentGuid�ɲ��rankCode�ɲ����
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// xmlObjDoc�ķ��ؽ�����ֽڵ㷽ʽǶ�뷵�أ���
		/// <OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</ORGANIZATIONS>
		///			<GROUPS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</GROUPS>
		///			<USERS oValue="" parentGuid="" >
		///				<USERS oValue="" parentGuid=""/>
		///			</USERS>
		///		</OBJECTS>
		/// </remarks>
		public static void CheckUserInObjects(XmlDocument xmlUserDoc,
			SearchObjectColumn socu,
			XmlDocument xmlObjDoc,
			SearchObjectColumn soc,
			ListObjectDelete lod,
			bool bDirect)
		{
			CheckUserInObjects(xmlUserDoc, socu, xmlObjDoc, soc, lod, string.Empty, bDirect);
		}

		/// <summary>
		/// �ж��û�Ⱥ�Ƿ������ָ���Ķ������֮��
		/// </summary>
		/// <param name="xmlUserDoc">�û�Ⱥ��ʶ������XML��ʽ��</param>
		/// <param name="socu">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="xmlObjDoc">����Ⱥ������XML��ʽ��</param>
		/// <param name="soc">��������������
		/// ���ǣգɣġ��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="bDirect">�Ƿ�ֱ�Ӵ��������м䲿�ţ�</param>
		/// <remarks>
		/// <code>
		/// xmlUserDoc�Ľṹ���£�˵��oValue�����socu���ʹ�ã�parentGuid�ɲ����
		///		<USERS>
		///			<USERS oValue="" parentGuid="" />
		///			<USERS oValue="" parentGuid="" />
		///		</USERS>
		/// xmlObjDoc�Ľṹ���£�˵��oValue�����soc���ʹ�ã�parentGuid�ɲ��rankCode�ɲ����
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// xmlObjDoc�ķ��ؽ�����ֽڵ㷽ʽǶ�뷵�أ���
		/// <OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</ORGANIZATIONS>
		///			<GROUPS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</GROUPS>
		///			<USERS oValue="" parentGuid="" >
		///				<USERS oValue="" parentGuid=""/>
		///			</USERS>
		///		</OBJECTS>
		/// </remarks>
		public static void CheckUserInObjects(XmlDocument xmlUserDoc,
			SearchObjectColumn socu,
			XmlDocument xmlObjDoc,
			SearchObjectColumn soc,
			bool bDirect)
		{
			CheckUserInObjects(xmlUserDoc, socu, xmlObjDoc, soc, ListObjectDelete.COMMON, bDirect);
		}

		/// <summary>
		/// �ж��û�Ⱥ�Ƿ������ָ���Ķ������֮��
		/// </summary>
		/// <param name="xmlUserDoc">�û�Ⱥ��ʶ������XML��ʽ��</param>
		/// <param name="socu">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="xmlObjDoc">����Ⱥ������XML��ʽ��</param>
		/// <param name="soc">��������������
		/// ���ǣգɣġ��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <remarks>
		/// <code>
		/// xmlUserDoc�Ľṹ���£�˵��oValue�����socu���ʹ�ã�parentGuid�ɲ����
		///		<USERS>
		///			<USERS oValue="" parentGuid="" />
		///			<USERS oValue="" parentGuid="" />
		///		</USERS>
		/// xmlObjDoc�Ľṹ���£�˵��oValue�����soc���ʹ�ã�parentGuid�ɲ��rankCode�ɲ����
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// xmlObjDoc�ķ��ؽ�����ֽڵ㷽ʽǶ�뷵�أ���
		/// <OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</ORGANIZATIONS>
		///			<GROUPS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</GROUPS>
		///			<USERS oValue="" parentGuid="" >
		///				<USERS oValue="" parentGuid=""/>
		///			</USERS>
		///		</OBJECTS>
		/// </remarks>
		public static void CheckUserInObjects(XmlDocument xmlUserDoc,
			SearchObjectColumn socu,
			XmlDocument xmlObjDoc,
			SearchObjectColumn soc)
		{
			CheckUserInObjects(xmlUserDoc, socu, xmlObjDoc, soc, false);
		}

		/// <summary>
		/// �ж��û�Ⱥ�Ƿ������ָ���Ķ������֮��
		/// </summary>
		/// <param name="xmlUserDoc">�û�Ⱥ��ʶ������XML��ʽ��</param>
		/// <param name="socu">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="xmlObjDoc">����Ⱥ������XML��ʽ��(Ĭ�ϼ�¼GUID)</param>
		/// <remarks>
		/// <code>
		/// xmlUserDoc�Ľṹ���£�˵��oValue�����socu���ʹ�ã�parentGuid�ɲ����
		///		<USERS>
		///			<USERS oValue="" parentGuid="" />
		///			<USERS oValue="" parentGuid="" />
		///		</USERS>
		/// xmlObjDoc�Ľṹ���£�˵��oValue�����soc���ʹ�ã�parentGuid�ɲ��rankCode�ɲ����
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// xmlObjDoc�ķ��ؽ�����ֽڵ㷽ʽǶ�뷵�أ���
		/// <OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</ORGANIZATIONS>
		///			<GROUPS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</GROUPS>
		///			<USERS oValue="" parentGuid="" >
		///				<USERS oValue="" parentGuid=""/>
		///			</USERS>
		///		</OBJECTS>
		/// </remarks>
		public static void CheckUserInObjects(XmlDocument xmlUserDoc, SearchObjectColumn socu, XmlDocument xmlObjDoc)
		{
			CheckUserInObjects(xmlUserDoc, socu, xmlObjDoc, SearchObjectColumn.SEARCH_GUID);
		}

		/// <summary>
		/// �ж��û�Ⱥ�Ƿ������ָ���Ķ������֮��
		/// </summary>
		/// <param name="xmlUserDoc">�û�Ⱥ��ʶ������XML��ʽ��(Ĭ�ϼ�¼GUID)</param>
		/// <param name="xmlObjDoc">����Ⱥ������XML��ʽ��(Ĭ�ϼ�¼GUID)</param>
		/// <remarks>
		/// <code>
		/// xmlUserDoc�Ľṹ���£�˵��oValue�����socu���ʹ�ã�parentGuid�ɲ����
		///		<USERS>
		///			<USERS oValue="" parentGuid="" />
		///			<USERS oValue="" parentGuid="" />
		///		</USERS>
		/// xmlObjDoc�Ľṹ���£�˵��oValue�����soc���ʹ�ã�parentGuid�ɲ��rankCode�ɲ����
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// xmlObjDoc�ķ��ؽ�����ֽڵ㷽ʽǶ�뷵�أ���
		/// <OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</ORGANIZATIONS>
		///			<GROUPS oValue="" rankCode="" >
		///				<USERS oValue="" parentGuid=""/>
		///				<USERS oValue="" parentGuid=""/>
		///			</GROUPS>
		///			<USERS oValue="" parentGuid="" >
		///				<USERS oValue="" parentGuid=""/>
		///			</USERS>
		///		</OBJECTS>
		/// </remarks>
		public static void CheckUserInObjects(XmlDocument xmlUserDoc, XmlDocument xmlObjDoc)
		{
			CheckUserInObjects(xmlUserDoc, SearchObjectColumn.SEARCH_GUID, xmlObjDoc);
		}
		#endregion

		#region GetAllUsersInAllObjects
		/// <summary>
		/// ��ȡָ�������е������û�����
		/// </summary>
		/// <param name="xmlObjDoc">Ҫ�󱻲�ѯ�����ݶ���</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strOrgLimitValues">Ҫ��������ڻ����ķ�Χ�ڣ����û�н�����ϵͳ�������ݣ��ɿգ�</param>
		/// <param name="soco">Ҫ�����ڻ����ķ�Χ�Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="lot">Ҫ�󱻲�ѯ�����ݶ������ͣ���Ҫ�����ڱ���Ƿ�Ҫ���ѯ��ְ��Ա��</param>
		/// <param name="lod">ϵͳ�б��߼�ɾ�������Ƿ��ѯȡ��</param>
		/// <param name="strHideType">Ҫ�����ص���������</param>
		/// <param name="strAttrs">Ҫ���õ���������</param>
		/// <returns>��ȡָ�������е������û�����</returns>
		/// <remarks>
		/// <code>
		/// xmlObjDoc�Ľṹ��
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// </remarks>
		public static DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc,
			SearchObjectColumn soc,
			string strOrgLimitValues,
			SearchObjectColumn soco,
			ListObjectType lot,
			ListObjectDelete lod,
			string strHideType,
			string strAttrs)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(xmlObjDoc.DocumentElement.OuterXml,
				soc,
				strOrgLimitValues,
				soco,
				lot,
				lod,
				strHideType,
				strAttrs);
			DataSet result;
			//if (false == GetAllUsersInAllObjectsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetAllUsersInAllObjectsQueue))
			//    {
			if (false == GetAllUsersInAllObjectsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				#region Prepare Db
				ExceptionHelper.TrueThrow(xmlObjDoc.DocumentElement.ChildNodes.Count <= 0, "�Բ���ϵͳû�и���Ҫ���ѯ�����ݶ���");
				strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);
				string strObjColName = OGUCommonDefine.GetSearchObjectColumn(soc);

				string strDelUser = GetSqlSearchStatus("OU_USERS", lod);

				strHideType = OGUCommonDefine.GetHideType(strHideType);
				string strHideList = GetHideTypeFromXmlForLike(strHideType, "OU_USERS");

				string strListObjectType = string.Empty;
				if ((lot & ListObjectType.SIDELINE) == 0)
					strListObjectType = " AND OU_USERS.SIDELINE = 0 ";

				if (strOrgLimitValues.Length == 0)
				{
					strOrgLimitValues = AccreditSection.GetConfig().AccreditSettings.OguRootName;// (new SysConfig()).GetDataFromConfig("OGURootName", string.Empty);
					ExceptionHelper.TrueThrow<ApplicationException>(string.IsNullOrEmpty(strOrgLimitValues),
						"�Բ�����û�����ú�ϵͳĬ��ָ���ĳ�ʼ����������web.config�е�configuration\\appSettings\\<add key=\"OGURootName\" value=\"\" />");
					soco = SearchObjectColumn.SEARCH_ALL_PATH_NAME;
				}
				#endregion
				StringBuilder strB = new StringBuilder(1024);

				foreach (XmlElement elem in xmlObjDoc.DocumentElement.ChildNodes)
				{
					string strRankLimit = elem.GetAttribute("rankCode");
					if (strRankLimit != null && strRankLimit.Length > 0)
						strRankLimit = " AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
							+ TSqlBuilder.Instance.CheckQuotationMark(strRankLimit, true) + " ) ";
					else
						strRankLimit = string.Empty;

					string strSql = string.Empty;
					switch (elem.LocalName)
					{
						case "ORGANIZATIONS":
							#region ORGANIZATIONS
							strSql = @"
							SELECT 'USERS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "USERS", "OU_USERS", "RANK_DEFINE") + @"
							FROM ORGANIZATIONS, OU_USERS, USERS JOIN RANK_DEFINE 
								ON RANK_DEFINE.CODE_NAME = USERS.RANK_CODE
								" + strRankLimit + @",
								(
									SELECT ORIGINAL_SORT
									FROM ORGANIZATIONS
									WHERE " + OGUCommonDefine.GetSearchObjectColumn(soco) + " IN ("
												+ OGUCommonDefine.AddMulitStrWithQuotationMark(strOrgLimitValues) + @" )
								) ORG_LIMIT
							WHERE USERS.GUID = OU_USERS.USER_GUID 
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "ORGANIZATIONS") + " = "
										  + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true) + @"
								AND OU_USERS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + '%'
								AND OU_USERS.ORIGINAL_SORT LIKE ORG_LIMIT.ORIGINAL_SORT + '%'
								AND (" + strDelUser + @") 
								" + strHideList + strListObjectType;
							break;
							#endregion
						case "GROUPS":
							#region GROUPS
							strSql = @"
							SELECT 'USERS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "USERS", "OU_USERS", "RANK_DEFINE") + @"
							FROM GROUPS, GROUP_USERS, OU_USERS, USERS JOIN RANK_DEFINE 
								ON RANK_DEFINE.CODE_NAME = USERS.RANK_CODE
								" + strRankLimit + @",
								(
									SELECT ORIGINAL_SORT
									FROM ORGANIZATIONS
									WHERE " + OGUCommonDefine.GetSearchObjectColumn(soco) + " IN ("
												+ OGUCommonDefine.AddMulitStrWithQuotationMark(strOrgLimitValues) + @" )
								) ORG_LIMIT
							WHERE GROUPS.GUID = GROUP_USERS.GROUP_GUID
								AND GROUP_USERS.USER_PARENT_GUID = OU_USERS.PARENT_GUID
								AND GROUP_USERS.USER_GUID = OU_USERS.USER_GUID
								AND USERS.GUID = OU_USERS.USER_GUID 
								AND OU_USERS.ORIGINAL_SORT LIKE ORG_LIMIT.ORIGINAL_SORT + '%'
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "GROUPS") + " = "
										  + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true) + @"
								AND (" + strDelUser + @") 
								" + strHideList + strListObjectType;
							break;
							#endregion
						case "USERS":
							#region USERS
							strSql = @"
							SELECT 'USERS' AS OBJECTCLASS, "
								+ DatabaseSchema.Instence.GetTableColumns(strAttrs, "USERS", "OU_USERS", "RANK_DEFINE") + @"
							FROM OU_USERS, USERS JOIN RANK_DEFINE 
								ON RANK_DEFINE.CODE_NAME = USERS.RANK_CODE
								" + strRankLimit + @",
								(
									SELECT ORIGINAL_SORT
									FROM ORGANIZATIONS
									WHERE " + OGUCommonDefine.GetSearchObjectColumn(soco) + " IN ("
												+ OGUCommonDefine.AddMulitStrWithQuotationMark(strOrgLimitValues) + @" )
								) ORG_LIMIT
							WHERE OU_USERS.USER_GUID = USERS.GUID
								AND OU_USERS.ORIGINAL_SORT LIKE ORG_LIMIT.ORIGINAL_SORT + '%'
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "OU_USERS", "USERS") + " = "
										  + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true) + @"
								AND (" + strDelUser + @") 
								" + strHideList + strListObjectType;
							break;
							#endregion
						default: ExceptionHelper.TrueThrow(true, "�Բ���,ϵͳû�ж�Ӧ����" + elem.LocalName + "������Ӧ����");
							break;
					}

					if (strB.Length > 0)
						strB.Append(Environment.NewLine + " UNION " + Environment.NewLine);

					strB.Append("(" + strSql + ")");
				}

				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					Database database = DatabaseFactory.Create(context);
					string sql = "SELECT * FROM (" + strB.ToString() + ") A ORDER BY GLOBAL_SORT";
					result = database.ExecuteDataSet(CommandType.Text, sql);
				}

				GetAllUsersInAllObjectsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		///// <summary>
		///// ��ȡָ�������е������û�����
		///// </summary>
		///// <param name="xmlObjDoc">Ҫ�󱻲�ѯ�����ݶ���</param>
		///// <param name="soc">��ѯҪ��Ĳ�ѯ������
		///// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		///// <param name="strOrgLimitValues">Ҫ��������ڻ����ķ�Χ�ڣ����û�н�����ϵͳ�������ݣ��ɿգ�</param>
		///// <param name="soco">Ҫ�����ڻ����ķ�Χ�Ĳ�ѯ������
		///// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		///// <param name="lot">Ҫ�󱻲�ѯ�����ݶ������ͣ���Ҫ�����ڱ���Ƿ�Ҫ���ѯ��ְ��Ա��</param>
		///// <param name="lod">ϵͳ�б��߼�ɾ�������Ƿ��ѯȡ��</param>
		///// <param name="strHideType">Ҫ�����ص���������</param>
		///// <param name="strAttrs">Ҫ���õ���������</param>
		///// <returns>��ȡָ�������е������û�����</returns>
		///// <remarks>
		///// <code>
		///// xmlObjDoc�Ľṹ��
		/////		<OBJECTS>
		/////			<ORGANIZATIONS oValue="" rankCode="" />
		/////			<GROUPS oValue="" rankCode="" />
		/////			<USERS oValue="" parentGuid="" />
		/////		</OBJECTS>
		///// </code>
		///// </remarks>
		//public static DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, SearchObjectColumn soc, string strOrgLimitValues, SearchObjectColumn soco, ListObjectType lot, ListObjectDelete lod, string strHideType, string strAttrs)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetAllUsersInAllObjects(xmlObjDoc, soc, strOrgLimitValues, soco, lot, lod, strHideType, strAttrs, da);
		//    }
		//}

		/// <summary>
		/// ��ȡָ�������е������û�����
		/// </summary>
		/// <param name="xmlObjDoc">Ҫ�󱻲�ѯ�����ݶ���</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strOrgLimitValues">Ҫ��������ڻ����ķ�Χ�ڣ����û�н�����ϵͳ�������ݣ��ɿգ�</param>
		/// <param name="soco">Ҫ�����ڻ����ķ�Χ�Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="lot">Ҫ�󱻲�ѯ�����ݶ������ͣ���Ҫ�����ڱ���Ƿ�Ҫ���ѯ��ְ��Ա��</param>
		/// <param name="lod">ϵͳ�б��߼�ɾ�������Ƿ��ѯȡ��</param>
		/// <param name="strAttrs">Ҫ���õ���������</param>
		/// <returns>��ȡָ�������е������û�����</returns>
		/// <remarks>
		/// <code>
		/// xmlObjDoc�Ľṹ��
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// </remarks>
		public static DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc,
			SearchObjectColumn soc,
			string strOrgLimitValues,
			SearchObjectColumn soco,
			ListObjectType lot,
			ListObjectDelete lod,
			string strAttrs)
		{
			return GetAllUsersInAllObjects(xmlObjDoc, soc, strOrgLimitValues, soco, lot, lod, string.Empty, strAttrs);
		}

		/// <summary>
		/// ��ȡָ�������е������û�����
		/// </summary>
		/// <param name="xmlObjDoc">Ҫ�󱻲�ѯ�����ݶ���</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strOrgLimitValues">Ҫ��������ڻ����ķ�Χ�ڣ����û�н�����ϵͳ�������ݣ��ɿգ�</param>
		/// <param name="soco">Ҫ�����ڻ����ķ�Χ�Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="lod">ϵͳ�б��߼�ɾ�������Ƿ��ѯȡ��</param>
		/// <param name="strAttrs">Ҫ���õ���������</param>
		/// <returns>��ȡָ�������е������û�����</returns>
		/// <remarks>
		/// <code>
		/// xmlObjDoc�Ľṹ��
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// </remarks>
		public static DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc,
			SearchObjectColumn soc,
			string strOrgLimitValues,
			SearchObjectColumn soco,
			ListObjectDelete lod,
			string strAttrs)
		{
			return GetAllUsersInAllObjects(xmlObjDoc, soc, strOrgLimitValues, soco, ListObjectType.ALL_TYPE, lod, strAttrs);
		}

		/// <summary>
		/// ��ȡָ�������е������û�����
		/// </summary>
		/// <param name="xmlObjDoc">Ҫ�󱻲�ѯ�����ݶ���</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strOrgLimitValues">Ҫ��������ڻ����ķ�Χ�ڣ����û�н�����ϵͳ�������ݣ��ɿգ�</param>
		/// <param name="soco">Ҫ�����ڻ����ķ�Χ�Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strAttrs">Ҫ���õ���������</param>
		/// <returns>��ȡָ�������е������û�����</returns>
		/// <remarks>
		/// <code>
		/// xmlObjDoc�Ľṹ��
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// </remarks>
		public static DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc,
			SearchObjectColumn soc,
			string strOrgLimitValues,
			SearchObjectColumn soco,
			string strAttrs)
		{
			return GetAllUsersInAllObjects(xmlObjDoc, soc, strOrgLimitValues, soco, ListObjectDelete.COMMON, strAttrs);
		}

		/// <summary>
		/// ��ȡָ�������е������û�����
		/// </summary>
		/// <param name="xmlObjDoc">Ҫ�󱻲�ѯ�����ݶ���</param>
		/// <param name="strOrgLimitValues">Ҫ��������ڻ����ķ�Χ�ڣ����û�н�����ϵͳ�������ݣ��ɿգ�</param>
		/// <param name="soco">Ҫ�����ڻ����ķ�Χ�Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strAttrs">Ҫ���õ���������</param>
		/// <returns>��ȡָ�������е������û�����</returns>
		/// <remarks>
		/// <code>
		/// xmlObjDoc�Ľṹ��
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// </remarks>
		public static DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, string strOrgLimitValues, SearchObjectColumn soco, string strAttrs)
		{
			return GetAllUsersInAllObjects(xmlObjDoc, SearchObjectColumn.SEARCH_GUID, strOrgLimitValues, soco, strAttrs);
		}

		/// <summary>
		/// ��ȡָ�������е������û�����
		/// </summary>
		/// <param name="xmlObjDoc">Ҫ�󱻲�ѯ�����ݶ���</param>
		/// <param name="strOrgLimitGuids">Ҫ��������ڻ����ķ�Χ�ڣ����û�н�����ϵͳ�������ݣ��ɿգ�</param>
		/// <param name="strAttrs">Ҫ���õ���������</param>
		/// <returns>��ȡָ�������е������û�����</returns>
		/// <remarks>
		/// <code>
		/// xmlObjDoc�Ľṹ��
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// </remarks>
		public static DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, string strOrgLimitGuids, string strAttrs)
		{
			return GetAllUsersInAllObjects(xmlObjDoc, strOrgLimitGuids, SearchObjectColumn.SEARCH_GUID, strAttrs);
		}

		/// <summary>
		/// ��ȡָ�������е������û�����
		/// </summary>
		/// <param name="xmlObjDoc">Ҫ�󱻲�ѯ�����ݶ���</param>
		/// <param name="strAttrs">Ҫ���õ���������</param>
		/// <returns>��ȡָ�������е������û�����</returns>
		/// <remarks>
		/// <code>
		/// xmlObjDoc�Ľṹ��
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// </remarks>
		public static DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc, string strAttrs)
		{
			return GetAllUsersInAllObjects(xmlObjDoc, string.Empty, strAttrs);
		}

		/// <summary>
		/// ��ȡָ�������е������û�����
		/// </summary>
		/// <param name="xmlObjDoc">Ҫ�󱻲�ѯ�����ݶ���</param>
		/// <returns>��ȡָ�������е������û�����</returns>
		/// <remarks>
		/// <code>
		/// xmlObjDoc�Ľṹ��
		///		<OBJECTS>
		///			<ORGANIZATIONS oValue="" rankCode="" />
		///			<GROUPS oValue="" rankCode="" />
		///			<USERS oValue="" parentGuid="" />
		///		</OBJECTS>
		/// </code>
		/// </remarks>
		public static DataSet GetAllUsersInAllObjects(XmlDocument xmlObjDoc)
		{
			return GetAllUsersInAllObjects(xmlObjDoc, string.Empty);
		}

		#endregion

		#region GetObjectsDetail
		///// <summary>
		///// ��ȡָ���������ϸ��������
		///// </summary>
		///// <param name="strObjType">Ҫ���ѯ���������(����Ϊ�գ�������û�ϲ�ѯ)</param>
		///// <param name="strObjValues">Ҫ���ѯ�������ݵı�ʶ(���֮����","�ָ���)</param>
		///// <param name="socu">��ѯҪ��Ĳ�ѯ������
		///// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		///// <param name="strParentValues">�������ڵĻ�����ʶ���ڶ���Ϊ��Ա�����ʱ����Ч��һ�㶼Ϊ�գ�</param>
		///// <param name="soco">��ѯҪ��Ĳ�ѯ������
		///// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		///// <param name="da">���ݿ��������</param>
		///// <returns>����ԵĲ�ѯ�������</returns>
		//public static DataSet GetObjectsDetail(string strObjType, 
		//    string strObjValues, 
		//    SearchObjectColumn socu, 
		//    string strParentValues, 
		//    SearchObjectColumn soco)
		//{
		//    return GetObjectsDetail(strObjType, strObjValues, socu, strParentValues, soco, string.Empty, da);
		//}
		/// <summary>
		/// ��ȡָ���������ϸ��������
		/// </summary>
		/// <param name="strObjType">Ҫ���ѯ���������(����Ϊ�գ�������û�ϲ�ѯ)</param>
		/// <param name="strObjValues">Ҫ���ѯ�������ݵı�ʶ(���֮����","�ָ���)</param>
		/// <param name="socu">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strParentValues">�������ڵĻ�����ʶ���ڶ���Ϊ��Ա�����ʱ����Ч��һ�㶼Ϊ�գ�</param>
		/// <param name="soco">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strExtAttrs">����Ҫ����չ���ԣ���������strObjTypeΪ��ʱ��</param>
		/// <returns>����ԵĲ�ѯ�������</returns>
		public static DataSet GetObjectsDetail(string strObjType,
			string strObjValues,
			SearchObjectColumn socu,
			string strParentValues,
			SearchObjectColumn soco,
			string strExtAttrs)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strObjType, strObjValues, socu, strParentValues, soco, strExtAttrs);
			DataSet result;
			//if (false == GetObjectsDetailQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetObjectsDetailQueue))
			//    {
			if (false == GetObjectsDetailQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string strColName = OGUCommonDefine.GetSearchObjectColumn(socu);
				string strSql = string.Empty;

				#region Data Prepare
				string columns = string.Empty;
				string objWhereClause = string.Empty;

				string objValues = OGUCommonDefine.AddMulitStrWithQuotationMark(strObjValues);
				string parentWhereClause = string.Empty;
				if (strParentValues.Length > 0 && soco != SearchObjectColumn.SEARCH_NULL)
				{
					parentWhereClause = " AND PARENT_GUID IN (SELECT GUID FROM ORGANIZATIONS WHERE "
						+ DatabaseSchema.Instence.GetTableColumns(OGUCommonDefine.GetSearchObjectColumn(soco), "ORGANIZATIONS")
						+ " IN (" + OGUCommonDefine.AddMulitStrWithQuotationMark(strParentValues) + "))";
				}
				string strAttrs = OGUCommonDefine.CombinateAttr(strExtAttrs);
				#endregion

				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					switch (strObjType)
					{
						case "ORGANIZATIONS":
							#region ORGANIZATIONS
							columns = DatabaseSchema.Instence.GetTableColumns(strAttrs, "ORGANIZATIONS", "RANK_DEFINE");
							objWhereClause = DatabaseSchema.Instence.GetTableColumns(strColName, "ORGANIZATIONS") + " IN (" + objValues + ")";
							strSql = @"
SELECT 'ORGANIZATIONS' AS OBJECTCLASS, *
FROM ORGANIZATIONS JOIN RANK_DEFINE 
	ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME 
WHERE {1}
	{2}
ORDER BY RANK_DEFINE.SORT_ID, ORGANIZATIONS.GLOBAL_SORT;";
							strSql = string.Format(strSql, columns, objWhereClause, parentWhereClause);
							break;
							#endregion
						case "GROUPS":
							#region GROUPS
							columns = DatabaseSchema.Instence.GetTableColumns(strAttrs, "GROUPS");
							objWhereClause = DatabaseSchema.Instence.GetTableColumns(strColName, "GROUPS") + " IN (" + objValues + ")";
							strSql = @"
SELECT 'GROUPS' AS OBJECTCLASS, *
FROM GROUPS
WHERE {1}
	{2}
ORDER BY GROUPS.GLOBAL_SORT;";
							strSql = string.Format(strSql, columns, objWhereClause, parentWhereClause);
							break;
							#endregion
						case "USERS":
							#region USERS
							columns = DatabaseSchema.Instence.GetTableColumns(strAttrs, "OU_USERS", "USERS", "RANK_DEFINE");
							objWhereClause = DatabaseSchema.Instence.GetTableColumns(strColName, "USERS", "OU_USERS") + " IN (" + objValues + ")";
							strSql = @"
SELECT 'USERS' AS OBJECTCLASS, OU_USERS.*, USERS.*, RANK_DEFINE.*
FROM OU_USERS, USERS JOIN RANK_DEFINE 
	ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
WHERE USERS.GUID = OU_USERS.USER_GUID
	AND {1}
	{2}
ORDER BY USERS.GUID, OU_USERS.SIDELINE, RANK_DEFINE.SORT_ID, OU_USERS.GLOBAL_SORT ; ";
							strSql = string.Format(strSql, columns, objWhereClause, parentWhereClause);
							break;
							#endregion
						case "":
							#region UNKNOW
							StringBuilder strTempSql = new StringBuilder(512);
							#region ORGANIZATIONS
							if (DatabaseSchema.Instence.CheckTableColumns(strColName, "ORGANIZATIONS"))
							{
								columns = DatabaseSchema.Instence.GetTableColumns(strAttrs, "ORGANIZATIONS", "RANK_DEFINE");
								objWhereClause = DatabaseSchema.Instence.GetTableColumns(strColName, "ORGANIZATIONS") + " IN (" + objValues + ") ";
								strTempSql.Append(string.Format(@" 
SELECT 'ORGANIZATIONS' AS OBJECTCLASS, {0}
FROM ORGANIZATIONS JOIN RANK_DEFINE 
	ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME 
WHERE {1}
	{2}", columns, objWhereClause, parentWhereClause));
							}
							#endregion
							#region GROUPS
							if (DatabaseSchema.Instence.CheckTableColumns(strColName, "GROUPS"))
							{
								if (strTempSql.Length > 0)
									strTempSql.Append("\nUNION\n");

								columns = DatabaseSchema.Instence.GetTableColumns(strAttrs, "GROUPS");
								objWhereClause = DatabaseSchema.Instence.GetTableColumns(strColName, "GROUPS") + " IN (" + objValues + ") ";

								strTempSql.Append(string.Format(@" 
SELECT 'GROUPS' AS OBJECTCLASS, {0}
FROM GROUPS
WHERE {1}
	{2}", columns, objWhereClause, parentWhereClause));
							}
							#endregion
							#region USERS
							if (DatabaseSchema.Instence.GetTableColumns(strColName, "OU_USERS", "USERS").IndexOf("NULL AS") < 0)
							{
								if (strTempSql.Length > 0)
									strTempSql.Append("\nUNION\n");

								columns = DatabaseSchema.Instence.GetTableColumns(strAttrs, "OU_USERS", "USERS", "RANK_DEFINE");
								objWhereClause = DatabaseSchema.Instence.GetTableColumns(strColName, "OU_USERS", "USERS") + " IN (" + objValues + ") ";

								strTempSql.Append(string.Format(@" 
SELECT 'USERS' AS OBJECTCLASS, {0}
FROM OU_USERS, USERS JOIN RANK_DEFINE 
	ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
WHERE USERS.GUID = OU_USERS.USER_GUID
	AND {1}
	{2}", columns, objWhereClause, parentWhereClause));
							}
							#endregion
							strSql = @"
						SELECT * 
						FROM	(" + strTempSql.ToString() + @") RESULT
						ORDER BY GLOBAL_SORT ";
							break;
							#endregion
						default: ExceptionHelper.TrueThrow(true, "�Բ���ϵͳ��û�д��������(" + strObjType + ")�Ĵ������");
							break;
					}

					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetObjectsDetailQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		///// <summary>
		///// ��ȡָ���������ϸ��������
		///// </summary>
		///// <param name="strObjType">Ҫ���ѯ���������(����Ϊ�գ��漰�����ѯ)</param>
		///// <param name="strObjValues">Ҫ���ѯ�������ݵı�ʶ(���֮����","�ָ���)</param>
		///// <param name="soc">��ѯҪ��Ĳ�ѯ������
		///// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		///// </param>
		///// <param name="strParentValues">�������ڵĻ�����ʶ���ڶ���Ϊ��Ա�����ʱ����Ч��һ�㶼Ϊ�գ�</param>
		///// <param name="soco">��ѯҪ��Ĳ�ѯ������
		///// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		///// <param name="strExtAttrs">����Ҫ����չ���ԣ���������strObjTypeΪ��ʱ��</param>
		///// <returns>����ԵĲ�ѯ�������</returns>
		//public static DataSet GetObjectsDetail(string strObjType, string strObjValues, SearchObjectColumn soc, string strParentValues, SearchObjectColumn soco, string strExtAttrs)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetObjectsDetail(strObjType, strObjValues, soc, strParentValues, soco, strExtAttrs, da);
		//    }
		//}

		/// <summary>
		/// ��ȡָ���������ϸ��������
		/// </summary>
		/// <param name="strObjType">Ҫ���ѯ���������(����Ϊ�գ��漰�����ѯ)</param>
		/// <param name="strObjValues">Ҫ���ѯ�������ݵı�ʶ(���֮����","�ָ���)</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strParentValues">�������ڵĻ�����ʶ���ڶ���Ϊ��Ա�����ʱ����Ч��һ�㶼Ϊ�գ�</param>
		/// <param name="soco">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <returns>����ԵĲ�ѯ�������</returns>
		public static DataSet GetObjectsDetail(string strObjType,
			string strObjValues,
			SearchObjectColumn soc,
			string strParentValues,
			SearchObjectColumn soco)
		{
			return GetObjectsDetail(strObjType, strObjValues, soc, strParentValues, soco, string.Empty);
		}

		/// <summary>
		/// ��ȡָ���������ϸ��������
		/// </summary>
		/// <param name="strObjValues">Ҫ���ѯ�������ݵı�ʶ(���֮����","�ָ���)</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strParentValues">�������ڵĻ�����ʶ���ڶ���Ϊ��Ա�����ʱ����Ч��һ�㶼Ϊ�գ�</param>
		/// <param name="soco">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <returns>����ԵĲ�ѯ�������</returns>
		public static DataSet GetObjectsDetail(string strObjValues,
			SearchObjectColumn soc,
			string strParentValues,
			SearchObjectColumn soco)
		{
			return GetObjectsDetail(string.Empty, strObjValues, soc, strParentValues, soco);
		}

		/// <summary>
		/// ��ȡָ���������ϸ��������
		/// </summary>
		/// <param name="strObjType">Ҫ���ѯ���������(����Ϊ�գ��漰�����ѯ)</param>
		/// <param name="strObjValues">Ҫ���ѯ�������ݵı�ʶ(���֮����","�ָ���)</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <returns>����ԵĲ�ѯ�������</returns>
		public static DataSet GetObjectsDetail(string strObjType, string strObjValues, SearchObjectColumn soc)
		{
			return GetObjectsDetail(strObjType, strObjValues, soc, string.Empty, SearchObjectColumn.SEARCH_NULL);
		}

		/// <summary>
		/// ��ȡָ���������ϸ��������
		/// </summary>
		/// <param name="strObjValues">Ҫ���ѯ�������ݵı�ʶ(���֮����","�ָ���)</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <returns>����ԵĲ�ѯ�������</returns>
		public static DataSet GetObjectsDetail(string strObjValues, SearchObjectColumn soc)
		{
			return GetObjectsDetail(string.Empty, strObjValues, soc);
		}

		/// <summary>
		/// ��ȡָ���������ϸ��������
		/// </summary>
		/// <param name="strObjType">Ҫ���ѯ���������(����Ϊ�գ��漰�����ѯ)</param>
		/// <param name="strObjGuids">Ҫ���ѯ�������ݵı�ʶGUID(���GUID֮����","�ָ���)</param>
		/// <returns>����ԵĲ�ѯ�������</returns>
		public static DataSet GetObjectsDetail(string strObjType, string strObjGuids)
		{
			return GetObjectsDetail(strObjType, strObjGuids, SearchObjectColumn.SEARCH_GUID);
		}

		/// <summary>
		/// ��ȡָ���������ϸ��������
		/// </summary>
		/// <param name="strObjGuids">Ҫ���ѯ�������ݵı�ʶGUID(���GUID֮����","�ָ���)</param>
		/// <returns>����ԵĲ�ѯ�������</returns>
		public static DataSet GetObjectsDetail(string strObjGuids)
		{
			return GetObjectsDetail(string.Empty, strObjGuids);
		}

		#endregion

		#region GetRankDefine
		/// <summary>
		/// ��ȡ�������������������
		/// </summary>
		/// <param name="iObjType">��ѯ����������Ϣ�ϵ����(1����������2����Ա����)</param>
		/// <param name="iShowHidden">�Ƿ�չ��ϵͳ�е����ظ��˼�����Ϣ����Щ������Ϣ�ǲ�����չ�ֵģ�Ĭ�������Ϊ0��</param>
		/// <returns>��ȡ�������������������</returns>
		public static DataSet GetRankDefine(int iObjType, int iShowHidden)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(iObjType, iShowHidden);
			DataSet result;
			//if (false == GetRankDefineQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetRankDefineQueue))
			//    {
			if (false == GetRankDefineQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string strShowHidden = " AND VISIBLE > 0 ";
				if (iShowHidden > 0)
					strShowHidden = string.Empty;
				string strSql = "SELECT CODE_NAME, NAME, SORT_ID FROM RANK_DEFINE WHERE RANK_CLASS = {0} {1} ORDER BY SORT_ID";
				strSql = string.Format(strSql, iObjType.ToString(), strShowHidden);

				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetRankDefineQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		///// <summary>
		///// ��ȡ�������������������
		///// </summary>
		///// <param name="iObjType">��ѯ����������Ϣ�ϵ����(1����������2����Ա����)</param>
		///// <param name="iShowHidden">�Ƿ�չ��ϵͳ�е����ظ��˼�����Ϣ����Щ������Ϣ�ǲ�����չ�ֵģ�Ĭ�������Ϊ0��</param>
		///// <returns>��ȡ�������������������</returns>
		//public static DataSet GetRankDefine(int iObjType, int iShowHidden)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetRankDefine(iObjType, iShowHidden, da);
		//    }
		//}

		/// <summary>
		/// ��ȡ�������������������
		/// </summary>
		/// <param name="iObjType">��ѯ����������Ϣ�ϵ����(1����������2����Ա����)</param>
		/// <returns>��ȡ�������������������</returns>
		public static DataSet GetRankDefine(int iObjType)
		{
			return GetRankDefine(iObjType, 0);
		}

		#endregion

		#region QueryOGUByCondition
		/// <summary>
		/// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
		/// </summary>
		/// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
		/// <param name="bLike">�Ƿ����ģ��ƥ��</param>
		/// <param name="bFirstPerson">Ҫ��һ�������</param>
		/// <param name="strOrgRankCodeName">Ҫ���ѯ�Ļ����ϵ�������������</param>
		/// <param name="strUserRankCodeName">Ҫ���ѯ�ϵ��û���������������</param>
		/// <param name="strAttr">Ҫ���ȡ���ֶ�</param>
		/// <param name="iListObjType">Ҫ���ѯ�Ķ�������</param>
		/// <param name="iDep">��ѯ���</param>
		/// <param name="strHideType">Ҫ�����ε���������</param>
		/// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
		/// 
		//2009-05-11
		public static DataSet QueryOGUByCondition(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			bool bLike,
			bool bFirstPerson,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strAttr,
			int iListObjType,
			int iDep,
			string strHideType)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strOrgValues,
				soc,
				strLikeName,
				bLike,
				bFirstPerson,
				strOrgRankCodeName,
				strUserRankCodeName,
				strAttr,
				iListObjType,
				iDep,
				strHideType);
			DataSet result;

			//�õ�All_Path_Name
			DataSet dsRootOrg = OGUReader.GetObjectsDetail("ORGANIZATIONS",
						strOrgValues,
						SearchObjectColumn.SEARCH_GUID,
						string.Empty,
						SearchObjectColumn.SEARCH_GUID);

			ExceptionHelper.FalseThrow(dsRootOrg.Tables[0].Rows.Count > 0, "�����ҵ�IDΪ{0}�Ļ���", strOrgValues);
			string rootPath = dsRootOrg.Tables[0].Rows[0]["ORIGINAL_SORT"].ToString();

			if (false == QueryOGUByConditionQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					if (strOrgValues.Length == 0)
					{
						strOrgValues = OGUCommonDefine.DBValueToString(OGUReader.GetRootDSE().Tables[0].Rows[0]["GUID"]);
						soc = SearchObjectColumn.SEARCH_GUID;
					}

					strAttr = OGUCommonDefine.CombinateAttr(strAttr);

					strHideType = OGUCommonDefine.GetHideType(strHideType);

					strLikeName = strLikeName.Replace("*", "%");

					StringBuilder strB = new StringBuilder(1024);
					if ((iListObjType & (int)ListObjectType.ORGANIZATIONS) != 0)
						strB.Append(" ( " + QueryOrganizationsByCondition(strOrgValues, soc, strLikeName, bLike, strOrgRankCodeName, strAttr, iDep, strHideType, rootPath) + " \n )");

					if ((iListObjType & (int)ListObjectType.GROUPS) != 0)
					{
						if (strB.Length > 0)
							strB.Append(" \n UNION \n ");
						strB.Append(" ( " + QueryGroupsByCondition(strOrgValues, soc, strLikeName, bLike, strAttr, iDep, strHideType, rootPath) + " \n )");
					}

					if ((iListObjType & (int)ListObjectType.USERS) != 0)
					{
						if (strB.Length > 0)
							strB.Append(" \n UNION \n ");

						strB.Append(" ( " + QueryUsersByCondition(strOrgValues, soc, strLikeName, bLike, bFirstPerson, strUserRankCodeName, strAttr, iListObjType, iDep, strHideType, rootPath) + " \n )");
					}

					string strSql = "SELECT * FROM ( " + strB.ToString() + " ) RESULT ORDER BY GLOBAL_SORT";

					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				QueryOGUByConditionQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}


		/// <summary>
		/// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
		/// </summary>
		/// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
		/// <param name="bLike">�Ƿ����ģ��ƥ��</param>      
		/// <param name="strAttr">Ҫ���ȡ���ֶ�</param>
		/// <param name="iListObjType">Ҫ���ѯ�Ķ�������</param>
		/// <param name="iLod">��ѯɾ���Ķ������</param>
		/// <param name="iDep">��ѯ���</param>
		/// <param name="strHideType">Ҫ�����ε���������</param>
		/// <param name="rtnRowLimit">������������</param>
		/// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
		/// 
		//2009-05-06 ɾ�� RANK_DEFINE Լ��������ORIGINAL_SORTԼ�������ӷ�����������:-1(ȫ��)
		public static DataSet QueryOGUByCondition2(string strOrgValues,
								SearchObjectColumn soc,
								string strLikeName,
								bool bLike,
								string strAttr,
								int iListObjType,
								ListObjectDelete iLod,
								int iDep,
								string strHideType,
								int rtnRowLimit)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strOrgValues,
				soc,
				strLikeName,
				bLike,
				strAttr,
				iListObjType,
				iDep,
				strHideType,
				rtnRowLimit);

			DataSet result;

			//�õ�All_Path_Name
			DataSet dsRootOrg = OGUReader.GetObjectsDetail("ORGANIZATIONS",
						strOrgValues,
						SearchObjectColumn.SEARCH_GUID,
						string.Empty,
						SearchObjectColumn.SEARCH_GUID);

			ExceptionHelper.FalseThrow(dsRootOrg.Tables[0].Rows.Count > 0, "�����ҵ�IDΪ{0}�Ļ���", strOrgValues);
			string rootPath = dsRootOrg.Tables[0].Rows[0]["ORIGINAL_SORT"].ToString();

			if (false == QueryOGUByConditionQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					if (strOrgValues.Length == 0)
					{
						strOrgValues = OGUCommonDefine.DBValueToString(OGUReader.GetRootDSE().Tables[0].Rows[0]["GUID"]);
						soc = SearchObjectColumn.SEARCH_GUID;
					}

					strAttr = OGUCommonDefine.CombinateAttr(strAttr);

					strHideType = OGUCommonDefine.GetHideType(strHideType);

					strLikeName = strLikeName.Replace("*", "%");

					StringBuilder strB = new StringBuilder(1024);
					if ((iListObjType & (int)ListObjectType.ORGANIZATIONS) != 0)
						strB.Append(" ( " + QueryOrganizationsByCondition2(strOrgValues, soc, strLikeName, bLike, strAttr, iLod, iDep, strHideType, rootPath) + " \n )");

					if ((iListObjType & (int)ListObjectType.GROUPS) != 0)
					{
						if (strB.Length > 0)
							strB.Append(" \n UNION \n ");
						strB.Append(" ( " + QueryGroupsByCondition2(strOrgValues, soc, strLikeName, bLike, strAttr, iLod, iDep, strHideType, rootPath) + " \n )");
					}

					if ((iListObjType & (int)ListObjectType.USERS) != 0)
					{
						if (strB.Length > 0)
							strB.Append(" \n UNION \n ");

						strB.Append(" ( " + QueryUsersByCondition2(strOrgValues, soc, strLikeName, bLike, strAttr, iListObjType, iLod, iDep, strHideType, rootPath) + " \n )");
					}

					string strSql = string.Empty;
					if (rtnRowLimit >= 0)
					{
						strSql = "SELECT top " + rtnRowLimit + " * FROM ( " + strB.ToString() + " ) RESULT ORDER BY GLOBAL_SORT";
					}
					else
					{
						strSql = "SELECT * FROM ( " + strB.ToString() + " ) RESULT ORDER BY GLOBAL_SORT";
					}

					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				QueryOGUByConditionQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}

			return result;
		}

		///// <summary>
		///// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
		///// </summary>
		///// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
		///// <param name="soc">��ѯҪ��Ĳ�ѯ������
		///// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		///// </param>
		///// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
		///// <param name="bLike">�Ƿ����ģ��ƥ��</param>
		///// <param name="bFirstPerson">Ҫ��һ�������</param>
		///// <param name="strOrgRankCodeName">Ҫ���ѯ�Ļ����ϵ�������������</param>
		///// <param name="strUserRankCodeName">Ҫ���ѯ�ϵ��û���������������</param>
		///// <param name="strAttr">Ҫ���ȡ���ֶ�</param>
		///// <param name="iListObjType">Ҫ���ѯ�Ķ�������</param>
		///// <param name="iDep">��ѯ���</param>
		///// <param name="strHideType">Ҫ�����ε���������</param>
		///// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
		//public static DataSet QueryOGUByCondition(string strOrgValues, SearchObjectColumn soc, string strLikeName, bool bLike, bool bFirstPerson, string strOrgRankCodeName, string strUserRankCodeName, string strAttr, int iListObjType, int iDep, string strHideType)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return QueryOGUByCondition(strOrgValues, soc, strLikeName, bLike, bFirstPerson, strOrgRankCodeName, strUserRankCodeName, strAttr, iListObjType, iDep, strHideType, da);
		//    }
		//}

		/// <summary>
		/// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
		/// </summary>
		/// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
		/// <param name="bFirstPerson">Ҫ��һ�������</param>
		/// <param name="strOrgRankCodeName">Ҫ���ѯ�Ļ����ϵ�������������</param>
		/// <param name="strUserRankCodeName">Ҫ���ѯ�ϵ��û���������������</param>
		/// <param name="strAttr">Ҫ���ȡ���ֶ�</param>
		/// <param name="iListObjType">Ҫ���ѯ�Ķ�������</param>
		/// <param name="iDep">��ѯ���</param>
		/// <param name="strHideType">Ҫ�����ε���������</param>
		/// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
		public static DataSet QueryOGUByCondition(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			bool bFirstPerson,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strAttr,
			int iListObjType,
			int iDep,
			string strHideType)
		{
			return QueryOGUByCondition(strOrgValues, soc, strLikeName, true, bFirstPerson, strOrgRankCodeName, strUserRankCodeName, strAttr, iListObjType, iDep, strHideType);
		}
		/// <summary>
		/// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
		/// </summary>
		/// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
		/// <param name="bFirstPerson">Ҫ��һ�������</param>
		/// <param name="strOrgRankCodeName">Ҫ���ѯ�Ļ����ϵ�������������</param>
		/// <param name="strUserRankCodeName">Ҫ���ѯ�ϵ��û���������������</param>
		/// <param name="strAttr">Ҫ���ȡ���ֶ�</param>
		/// <param name="iListObjType">Ҫ���ѯ�Ķ�������</param>
		/// <param name="iDep">��ѯ���</param>
		/// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
		public static DataSet QueryOGUByCondition(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			bool bFirstPerson,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strAttr,
			int iListObjType, int iDep)
		{
			return QueryOGUByCondition(strOrgValues, soc, strLikeName, bFirstPerson, strOrgRankCodeName, strUserRankCodeName, strAttr, iListObjType, iDep, string.Empty);
		}

		/// <summary>
		/// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
		/// </summary>
		/// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
		/// <param name="bFirstPerson">Ҫ��һ�������</param>
		/// <param name="strOrgRankCodeName">Ҫ���ѯ�Ļ����ϵ�������������</param>
		/// <param name="strUserRankCodeName">Ҫ���ѯ�ϵ��û���������������</param>
		/// <param name="strAttr">Ҫ���ȡ���ֶ�</param>
		/// <param name="iListObjType">Ҫ���ѯ�Ķ�������</param>
		/// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
		public static DataSet QueryOGUByCondition(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			bool bFirstPerson,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strAttr,
			int iListObjType)
		{
			return QueryOGUByCondition(strOrgValues, soc, strLikeName, bFirstPerson, strOrgRankCodeName, strUserRankCodeName, strAttr, iListObjType, 0);
		}

		/// <summary>
		/// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
		/// </summary>
		/// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
		/// <param name="strOrgRankCodeName">Ҫ���ѯ�Ļ����ϵ�������������</param>
		/// <param name="strUserRankCodeName">Ҫ���ѯ�ϵ��û���������������</param>
		/// <param name="strAttr">Ҫ���ȡ���ֶ�</param>
		/// <param name="iListObjType">Ҫ���ѯ�Ķ�������</param>
		/// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
		public static DataSet QueryOGUByCondition(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strAttr,
			int iListObjType)
		{
			return QueryOGUByCondition(strOrgValues, soc, strLikeName, false, strOrgRankCodeName, strUserRankCodeName, strAttr, iListObjType);
		}

		/// <summary>
		/// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
		/// </summary>
		/// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
		/// <param name="strOrgRankCodeName">Ҫ���ѯ�Ļ����ϵ�������������</param>
		/// <param name="strUserRankCodeName">Ҫ���ѯ�ϵ��û���������������</param>
		/// <param name="strAttr">Ҫ���ȡ���ֶ�</param>
		/// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
		public static DataSet QueryOGUByCondition(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			string strOrgRankCodeName,
			string strUserRankCodeName,
			string strAttr)
		{
			return QueryOGUByCondition(strOrgValues, soc, strLikeName, strOrgRankCodeName, strUserRankCodeName, strAttr, (int)(ListObjectType.ORGANIZATIONS | ListObjectType.USERS));
		}

		/// <summary>
		/// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
		/// </summary>
		/// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
		/// <param name="strUserRankCodeName">Ҫ���ѯ�ϵ��û���������������</param>
		/// <param name="strAttr">Ҫ���ȡ���ֶ�</param>
		/// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
		public static DataSet QueryOGUByCondition(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			string strUserRankCodeName,
			string strAttr)
		{
			return QueryOGUByCondition(strOrgValues, soc, strLikeName, string.Empty, strUserRankCodeName, strAttr);
		}

		/// <summary>
		/// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
		/// </summary>
		/// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
		/// <param name="strAttr">Ҫ���ȡ���ֶ�</param>
		/// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
		public static DataSet QueryOGUByCondition(string strOrgValues, SearchObjectColumn soc, string strLikeName, string strAttr)
		{
			return QueryOGUByCondition(strOrgValues, soc, strLikeName, string.Empty, strAttr);
		}

		/// <summary>
		/// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
		/// </summary>
		/// <param name="strOrgValues">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
		/// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
		public static DataSet QueryOGUByCondition(string strOrgValues, SearchObjectColumn soc, string strLikeName)
		{
			return QueryOGUByCondition(strOrgValues, soc, strLikeName, string.Empty);
		}

		/// <summary>
		/// ���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������
		/// </summary>
		/// <param name="strOrgGuids">ָ�������������֮�����","�ָ�,�վͲ���Ĭ�ϣ�</param>
		/// <param name="strLikeName">�����еģ�ģ��ƥ�����</param>
		/// <returns>���ղ�ͬ��Ҫ���ѯϵͳ�е����з�������������</returns>
		public static DataSet QueryOGUByCondition(string strOrgGuids, string strLikeName)
		{
			return QueryOGUByCondition(strOrgGuids, SearchObjectColumn.SEARCH_GUID, strLikeName);
		}

		#endregion

		#region GetUsersInGroups
		/// <summary>
		/// ��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���
		/// </summary>
		/// <param name="strGroupValues">Ҫ���ѯ����Ա������ʶ�����֮�����","�ָ���</param>
		/// <param name="socg">����ѯ������Ҫ���Ӧ���������ͣ����ݱ��ֶ����ƣ�</param>
		/// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
		/// <param name="strOrgValues">Ҫ���ѯ���ݵĻ�����Χ(���֮�����","�ָ��� �յ�ʱ���ʾ�޻���Ҫ��)</param>
		/// <param name="soco">����ѯ������Ҫ���Ӧ���������ͣ����ݱ��ֶ����ƣ�</param>
		/// <param name="strUserRankCodeName">����ԱҪ��������������</param>
		/// <param name="iLod">���β�ѯ��Ҫ���ѯ�����״̬��Ϣ���ݣ��Ƿ�����߼�ɾ������</param>
		/// <returns>��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���</returns>
		public static DataSet GetUsersInGroups(string strGroupValues,
			SearchObjectColumn socg,
			string strAttrs,
			string strOrgValues,
			SearchObjectColumn soco,
			string strUserRankCodeName,
			int iLod)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strGroupValues, socg, strAttrs, strOrgValues, soco, strUserRankCodeName, iLod);
			DataSet result;
			//if (false == GetUsersInGroupsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetUsersInGroupsQueue))
			//    {
			if (false == GetUsersInGroupsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);
					#region SQL Prepare
					string strSql = @"
				SELECT 'USERS' AS OBJECTCLASS, GROUP_USERS.GROUP_GUID, "
						+ DatabaseSchema.Instence.GetTableColumns(strAttrs, "USERS", "OU_USERS", "RANK_DEFINE") + @"
				FROM ORGANIZATIONS, GROUPS, GROUP_USERS, OU_USERS, USERS JOIN RANK_DEFINE 
					ON RANK_DEFINE.CODE_NAME = USERS.RANK_CODE
					{2}
				WHERE " + DatabaseSchema.Instence.GetTableColumns(OGUCommonDefine.GetSearchObjectColumn(socg), "GROUPS") + @" IN ({0})
					AND GROUP_USERS.GROUP_GUID = GROUPS.GUID
					AND GROUP_USERS.USER_GUID = OU_USERS.USER_GUID
					AND GROUP_USERS.USER_PARENT_GUID = OU_USERS.PARENT_GUID
					AND ({1})
					AND USERS.GUID = GROUP_USERS.USER_GUID
					AND OU_USERS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + '%'
					{3}
				ORDER BY GROUPS.GLOBAL_SORT, GROUP_USERS.INNER_SORT";

					string strOrgLimit = string.Empty;
					if (strOrgValues.Length > 0)
						strOrgLimit = " AND " + DatabaseSchema.Instence.GetTableColumns(OGUCommonDefine.GetSearchObjectColumn(soco), "ORGANIZATIONS")
							+ " IN (" + OGUCommonDefine.AddMulitStrWithQuotationMark(strOrgValues) + ") ";
					else
						strOrgLimit = " AND ORGANIZATIONS.GUID = "
							+ TSqlBuilder.Instance.CheckQuotationMark(OGUCommonDefine.DBValueToString(OGUReader.GetRootDSE().Tables[0].Rows[0]["GUID"]), true);

					string strRankLimit = string.Empty;
					if (strUserRankCodeName.Length > 0)
						strRankLimit = " AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
							+ TSqlBuilder.Instance.CheckQuotationMark(strUserRankCodeName, true) + ") ";


					strSql = string.Format(strSql,
						OGUCommonDefine.AddMulitStrWithQuotationMark(strGroupValues),
						GetSqlSearchStatus("OU_USERS", (ListObjectDelete)iLod),
						strRankLimit,
						strOrgLimit);
					#endregion
					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetUsersInGroupsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		///// <summary>
		///// ��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���
		///// </summary>
		///// <param name="strGroupValues">Ҫ���ѯ����Ա������ʶ�����֮�����","�ָ���</param>
		///// <param name="socg">����ѯ������Ҫ���Ӧ���������ͣ����ݱ��ֶ����ƣ�</param>
		///// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
		///// <param name="strOrgValues">Ҫ���ѯ���ݵĻ�����Χ(���֮�����","�ָ��� �յ�ʱ���ʾ�޻���Ҫ��)</param>
		///// <param name="soco">����ѯ������Ҫ���Ӧ���������ͣ����ݱ��ֶ����ƣ�</param>
		///// <param name="strUserRankCodeName">����ԱҪ��������������</param>
		///// <param name="iLod">���β�ѯ��Ҫ���ѯ�����״̬��Ϣ���ݣ��Ƿ�����߼�ɾ������</param>
		///// <returns>��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���</returns>
		//public static DataSet GetUsersInGroups(string strGroupValues, SearchObjectColumn socg, string strAttrs, string strOrgValues, SearchObjectColumn soco, string strUserRankCodeName, int iLod)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetUsersInGroups(strGroupValues, socg, strAttrs, strOrgValues, soco, strUserRankCodeName, iLod, da);
		//    }
		//}

		/// <summary>
		/// ��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���
		/// </summary>
		/// <param name="strGroupValues">Ҫ���ѯ����Ա������ʶ�����֮�����","�ָ���</param>
		/// <param name="socg">����ѯ������Ҫ���Ӧ���������ͣ����ݱ��ֶ����ƣ�</param>
		/// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
		/// <param name="strOrgValues">Ҫ���ѯ���ݵĻ�����Χ(���֮�����","�ָ��� �յ�ʱ���ʾ�޻���Ҫ��)</param>
		/// <param name="soco">����ѯ������Ҫ���Ӧ���������ͣ����ݱ��ֶ����ƣ�</param>
		/// <param name="strUserRankCodeName">����ԱҪ��������������</param>
		/// <returns>��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���</returns>
		public static DataSet GetUsersInGroups(string strGroupValues,
			SearchObjectColumn socg,
			string strAttrs,
			string strOrgValues,
			SearchObjectColumn soco,
			string strUserRankCodeName)
		{
			return GetUsersInGroups(strGroupValues, socg, strAttrs, strOrgValues, soco, strUserRankCodeName, (int)ListObjectDelete.COMMON);
		}
		/// <summary>
		/// ��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���
		/// </summary>
		/// <param name="strGroupValues">Ҫ���ѯ����Ա������ʶ�����֮�����","�ָ���</param>
		/// <param name="socg">����ѯ������Ҫ���Ӧ���������ͣ����ݱ��ֶ����ƣ�</param>
		/// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
		/// <param name="strOrgValues">Ҫ���ѯ���ݵĻ�����Χ(���֮�����","�ָ��� �յ�ʱ���ʾ�޻���Ҫ��)</param>
		/// <param name="soco">����ѯ������Ҫ���Ӧ���������ͣ����ݱ��ֶ����ƣ�</param>
		/// <returns>��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���</returns>
		public static DataSet GetUsersInGroups(string strGroupValues,
			SearchObjectColumn socg,
			string strAttrs,
			string strOrgValues,
			SearchObjectColumn soco)
		{
			return GetUsersInGroups(strGroupValues, socg, strAttrs, strOrgValues, soco, string.Empty);
		}
		/// <summary>
		/// ��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���
		/// </summary>
		/// <param name="strGroupValues">Ҫ���ѯ����Ա������ʶ�����֮�����","�ָ���</param>
		/// <param name="socg">����ѯ������Ҫ���Ӧ���������ͣ����ݱ��ֶ����ƣ�</param>
		/// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
		/// <returns>��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���</returns>
		public static DataSet GetUsersInGroups(string strGroupValues, SearchObjectColumn socg, string strAttrs)
		{
			return GetUsersInGroups(strGroupValues, socg, strAttrs, string.Empty, SearchObjectColumn.SEARCH_NULL);
		}
		/// <summary>
		/// ��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���
		/// </summary>
		/// <param name="strGroupValues">Ҫ���ѯ����Ա������ʶ�����֮�����","�ָ���</param>
		/// <param name="socg">����ѯ������Ҫ���Ӧ���������ͣ����ݱ��ֶ����ƣ�</param>
		/// <returns>��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���</returns>
		public static DataSet GetUsersInGroups(string strGroupValues, SearchObjectColumn socg)
		{
			return GetUsersInGroups(strGroupValues, socg, string.Empty);
		}
		/// <summary>
		/// ��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���
		/// </summary>
		/// <param name="strGroupGuids">Ҫ���ѯ����Ա������ʶGUID�����GUID֮�����","�ָ���</param>
		/// <returns>��ȡָ����Ա���е����г�Ա��ע�������߼�ɾ�������ݶ���</returns>
		public static DataSet GetUsersInGroups(string strGroupGuids)
		{
			return GetUsersInGroups(strGroupGuids, SearchObjectColumn.SEARCH_GUID);
		}

		#endregion

		#region GetUsersInGroups [Add By Yuanyong @ 2005-04-20]
		/// <summary>
		/// 
		/// </summary>
		/// <param name="strGroupValues"></param>
		/// <param name="socg"></param>
		/// <param name="strNameLike"></param>
		/// <param name="strSortColumn"></param>
		/// <param name="strAttrs"></param>
		/// <param name="strOrgValues"></param>
		/// <param name="soco"></param>
		/// <param name="strUserRankCodeName"></param>
		/// <param name="lod"></param>
		/// <param name="iPageNo"></param>
		/// <param name="iPageSize"></param>
		/// <returns></returns>
		public static DataSet GetUsersInGroups(string strGroupValues,
			SearchObjectColumn socg,
			string strNameLike,
			string strSortColumn,
			string strAttrs,
			string strOrgValues,
			SearchObjectColumn soco,
			string strUserRankCodeName,
			ListObjectDelete lod,
			int iPageNo,
			int iPageSize)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strGroupValues,
				socg,
				strNameLike,
				strSortColumn,
				strAttrs,
				strOrgValues,
				soco,
				strUserRankCodeName,
				lod,
				iPageNo,
				iPageSize);
			DataSet result;
			//if (false == GetUsersInGroupsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetUsersInGroupsQueue))
			//    {
			if (false == GetUsersInGroupsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					Database database = DatabaseFactory.Create(context);
					strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);
					#region SQL Prepare
					string strSql = @"
				SELECT 'USERS' AS OBJECTCLASS, GROUP_USERS.GROUP_GUID, "
						+ DatabaseSchema.Instence.GetTableColumns(strAttrs, "USERS", "OU_USERS", "RANK_DEFINE") + @"
				FROM ORGANIZATIONS, GROUPS, GROUP_USERS, OU_USERS, USERS LEFT JOIN RANK_DEFINE 
					ON RANK_DEFINE.CODE_NAME = USERS.RANK_CODE
					{2}
				WHERE " + DatabaseSchema.Instence.GetTableColumns(OGUCommonDefine.GetSearchObjectColumn(socg), "GROUPS") + @" IN ({0})
					AND GROUP_USERS.GROUP_GUID = GROUPS.GUID
					AND GROUP_USERS.USER_GUID = OU_USERS.USER_GUID
					AND GROUP_USERS.USER_PARENT_GUID = OU_USERS.PARENT_GUID
					AND ({1})
					AND USERS.GUID = GROUP_USERS.USER_GUID
					AND OU_USERS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + '%'
					{3}
					{4}
				ORDER BY GROUPS.GLOBAL_SORT, "
							+ TSqlBuilder.Instance.CheckQuotationMark(strSortColumn == string.Empty ? "GROUP_USERS.INNER_SORT" : strSortColumn, false);

					string strOrgLimit = string.Empty;
					if (strOrgValues.Length > 0)
						strOrgLimit = " AND " + DatabaseSchema.Instence.GetTableColumns(OGUCommonDefine.GetSearchObjectColumn(soco), "ORGANIZATIONS")
							+ " IN (" + OGUCommonDefine.AddMulitStrWithQuotationMark(strOrgValues) + ") ";
					else
						strOrgLimit = " AND ORGANIZATIONS.GUID = "
							+ TSqlBuilder.Instance.CheckQuotationMark(OGUCommonDefine.DBValueToString(OGUReader.GetRootDSE().Tables[0].Rows[0]["GUID"]), true);

					string strRankLimit = string.Empty;
					if (strUserRankCodeName.Length > 0)
						strRankLimit = " AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
							+ TSqlBuilder.Instance.CheckQuotationMark(strUserRankCodeName, true) + ") ";

					string strNameLikeWhere = string.Empty;
					if (strNameLike != string.Empty)
					{
						strNameLike = strNameLike.Replace("*", "");
						strNameLikeWhere = " AND (USERS.LOGON_NAME LIKE '%' + " + TSqlBuilder.Instance.CheckQuotationMark(strNameLike, true) + @" + '%' 
						OR OU_USERS.OBJ_NAME LIKE '%' + " + TSqlBuilder.Instance.CheckQuotationMark(strNameLike, true) + @" + '%' 
						OR OU_USERS.DISPLAY_NAME LIKE '%' + " + TSqlBuilder.Instance.CheckQuotationMark(strNameLike, true) + @" + '%') ";
					}
					#endregion
					strSql = string.Format(strSql, OGUCommonDefine.AddMulitStrWithQuotationMark(strGroupValues), GetSqlSearchStatus("OU_USERS", lod), strRankLimit, strOrgLimit, strNameLikeWhere);
					//if (iPageNo >= 0 && iPageSize > 0)
					//    result = database.ExecuteDataSet(Com, strSql, "GROUPS", iPageNo, iPageSize);
					//else
					result = database.ExecuteDataSet(CommandType.Text, strSql, iPageNo, iPageSize, "USERS");
				}
				GetUsersInGroupsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}

		///// <summary>
		///// 
		///// </summary>
		///// <param name="strGroupValues"></param>
		///// <param name="socg"></param>
		///// <param name="strNameLike"></param>
		///// <param name="strSortColumn"></param>
		///// <param name="strAttrs"></param>
		///// <param name="strOrgValues"></param>
		///// <param name="soco"></param>
		///// <param name="strUserRankCodeName"></param>
		///// <param name="lod"></param>
		///// <param name="iPageNo"></param>
		///// <param name="iPageSize"></param>
		///// <returns></returns>
		//public static DataSet GetUsersInGroups(string strGroupValues, SearchObjectColumn socg, string strNameLike, string strSortColumn, string strAttrs, string strOrgValues, SearchObjectColumn soco, string strUserRankCodeName, ListObjectDelete lod, int iPageNo, int iPageSize)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetUsersInGroups(strGroupValues, socg, strNameLike, strSortColumn, strAttrs, strOrgValues, soco, strUserRankCodeName, lod, iPageNo, iPageSize, da);
		//    }
		//}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strGroupValues"></param>
		/// <param name="socg"></param>
		/// <param name="strNameLike"></param>
		/// <param name="strSortColumn"></param>
		/// <param name="strAttrs"></param>
		/// <param name="strOrgValues"></param>
		/// <param name="soco"></param>
		/// <param name="strUserRankCodeName"></param>
		/// <param name="iPageNo"></param>
		/// <param name="iPageSize"></param>
		/// <returns></returns>
		public static DataSet GetUsersInGroups(string strGroupValues,
			SearchObjectColumn socg,
			string strNameLike,
			string strSortColumn,
			string strAttrs,
			string strOrgValues,
			SearchObjectColumn soco,
			string strUserRankCodeName,
			int iPageNo,
			int iPageSize)
		{
			return GetUsersInGroups(strGroupValues, socg, strNameLike, strSortColumn, strAttrs, strOrgValues, soco, strUserRankCodeName, ListObjectDelete.COMMON, iPageNo, iPageSize);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strGroupValues"></param>
		/// <param name="socg"></param>
		/// <param name="strNameLike"></param>
		/// <param name="strSortColumn"></param>
		/// <param name="strAttrs"></param>
		/// <param name="strOrgValues"></param>
		/// <param name="soco"></param>
		/// <param name="iPageNo"></param>
		/// <param name="iPageSize"></param>
		/// <returns></returns>
		public static DataSet GetUsersInGroups(string strGroupValues,
			SearchObjectColumn socg,
			string strNameLike,
			string strSortColumn,
			string strAttrs,
			string strOrgValues,
			SearchObjectColumn soco,
			int iPageNo,
			int iPageSize)
		{
			return GetUsersInGroups(strGroupValues, socg, strNameLike, strSortColumn, strAttrs, strOrgValues, soco, string.Empty, iPageNo, iPageSize);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strGroupValues"></param>
		/// <param name="socg"></param>
		/// <param name="strNameLike"></param>
		/// <param name="strSortColumn"></param>
		/// <param name="strAttrs"></param>
		/// <param name="iPageNo"></param>
		/// <param name="iPageSize"></param>
		/// <returns></returns>
		public static DataSet GetUsersInGroups(string strGroupValues,
			SearchObjectColumn socg,
			string strNameLike,
			string strSortColumn,
			string strAttrs,
			int iPageNo,
			int iPageSize)
		{
			return GetUsersInGroups(strGroupValues, socg, strNameLike, strSortColumn, strAttrs, string.Empty, SearchObjectColumn.SEARCH_NULL, iPageNo, iPageSize);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strGroupValues"></param>
		/// <param name="socg"></param>
		/// <param name="strNameLike"></param>
		/// <param name="strSortColumn"></param>
		/// <param name="iPageNo"></param>
		/// <param name="iPageSize"></param>
		/// <returns></returns>
		public static DataSet GetUsersInGroups(string strGroupValues,
			SearchObjectColumn socg,
			string strNameLike,
			string strSortColumn,
			int iPageNo,
			int iPageSize)
		{
			return GetUsersInGroups(strGroupValues, socg, strNameLike, strSortColumn, string.Empty, iPageNo, iPageSize);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strGroupValues"></param>
		/// <param name="socg"></param>
		/// <param name="strSortColumn"></param>
		/// <param name="iPageNo"></param>
		/// <param name="iPageSize"></param>
		/// <returns></returns>
		public static DataSet GetUsersInGroups(string strGroupValues,
			SearchObjectColumn socg,
			string strSortColumn,
			int iPageNo,
			int iPageSize)
		{
			return GetUsersInGroups(strGroupValues, socg, string.Empty, strSortColumn, string.Empty, iPageNo, iPageSize);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strGroupGuids"></param>
		/// <param name="iPageNo"></param>
		/// <param name="iPageSize"></param>
		/// <returns></returns>
		public static DataSet GetUsersInGroups(string strGroupGuids, int iPageNo, int iPageSize)
		{
			return GetUsersInGroups(strGroupGuids, SearchObjectColumn.SEARCH_GUID, string.Empty, iPageNo, iPageSize);
		}

		#endregion

		#region GetGroupsOfUsers
		/// <summary>
		/// ��ȡָ���û���������"��Ա��"����
		/// </summary>
		/// <param name="strUserValues">ָ�����û���ʶ�����֮����á�,���ָ���</param>
		/// <param name="socu">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strParentValue">ָ�����û����ڲ��ţ����������ְ���⣩</param>
		/// <param name="soco">��������������
		/// ���ǣգɣġ��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strAttrs">��Ҫ���ȡ��������Ϣ</param>
		/// <param name="iLod">���β�ѯ��Ҫ���ѯ�����״̬��Ϣ���ݣ��Ƿ�����߼�ɾ������</param>
		/// <returns>��ȡָ����Ա������"��Ա��"���ϣ�ע�������߼�ɾ�������ݶ���</returns>
		public static DataSet GetGroupsOfUsers(string strUserValues,
			SearchObjectColumn socu,
			string strParentValue,
			SearchObjectColumn soco,
			string strAttrs,
			int iLod)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strUserValues, socu, strParentValue, soco, strAttrs, iLod);
			DataSet result;
			//if (false == GetGroupsOfUsersQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetGroupsOfUsersQueue))
			//    {
			if (false == GetGroupsOfUsersQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string strColName = OGUCommonDefine.GetSearchObjectColumn(socu);
				string strParentColName = OGUCommonDefine.GetSearchObjectColumn(soco);

				strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);

				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					string strSql = @"
				SELECT 'GROUPS' AS OBJECTCLASS, GROUPS.GUID, " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "GROUPS") + @"
				FROM GROUPS, GROUP_USERS, OU_USERS, USERS, ORGANIZATIONS
				WHERE GROUPS.GUID = GROUP_USERS.GROUP_GUID
					AND ORGANIZATIONS.GUID = OU_USERS.PARENT_GUID
					AND OU_USERS.USER_GUID = USERS.GUID
					AND GROUP_USERS.USER_GUID = OU_USERS.USER_GUID
					AND GROUP_USERS.USER_PARENT_GUID = OU_USERS.PARENT_GUID
					{0}
					AND {1} IN ({2})
					AND ({3})
				ORDER BY OU_USERS.GLOBAL_SORT, GROUPS.GLOBAL_SORT
				";

					string strParent = string.Empty;
					if (strParentValue.Length != 0)
						strParent = " AND " + DatabaseSchema.Instence.GetTableColumns(strParentColName, "ORGANIZATIONS")
							+ " IN (" + OGUCommonDefine.AddMulitStrWithQuotationMark(strParentValue) + ") ";

					strSql = string.Format(strSql, strParent,
						DatabaseSchema.Instence.GetTableColumns(strColName, "OU_USERS", "USERS"),
						OGUCommonDefine.AddMulitStrWithQuotationMark(strUserValues),
						GetSqlSearchStatus("GROUPS", (ListObjectDelete)iLod));

					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetGroupsOfUsersQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		///// <summary>
		///// ��ȡָ���û���������"��Ա��"����
		///// </summary>
		///// <param name="strUserValues">ָ�����û���ʶ�����֮����á�,���ָ���</param>
		///// <param name="socu">�û�����������
		///// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		///// </param>
		///// <param name="strParentValue">ָ�����û����ڲ��ţ����������ְ���⣩</param>
		///// <param name="soco">��������������
		///// ���ǣգɣġ��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		///// </param>
		///// <param name="strAttrs">��Ҫ���ȡ��������Ϣ</param>
		///// <param name="iLod">���β�ѯ��Ҫ���ѯ�����״̬��Ϣ���ݣ��Ƿ�����߼�ɾ������</param>
		///// <returns>��ȡָ����Ա������"��Ա��"���ϣ�ע�������߼�ɾ�������ݶ���</returns>
		//public static DataSet GetGroupsOfUsers(string strUserValues, SearchObjectColumn socu, string strParentValue, SearchObjectColumn soco, string strAttrs, int iLod)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetGroupsOfUsers(strUserValues, socu, strParentValue, soco, strAttrs, iLod, da);
		//    }
		//}

		/// <summary>
		/// ��ȡָ���û���������"��Ա��"����
		/// </summary>
		/// <param name="strUserValues">ָ�����û���ʶ�����֮����á�,���ָ���</param>
		/// <param name="socu">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strParentValue">ָ�����û����ڲ��ţ����������ְ���⣩</param>
		/// <param name="soco">��������������
		/// ���ǣգɣġ��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strAttrs">��Ҫ���ȡ��������Ϣ</param>
		/// <returns>��ȡָ����Ա������"��Ա��"���ϣ�ע�������߼�ɾ�������ݶ���</returns>
		public static DataSet GetGroupsOfUsers(string strUserValues,
			SearchObjectColumn socu,
			string strParentValue,
			SearchObjectColumn soco,
			string strAttrs)
		{
			return GetGroupsOfUsers(strUserValues, socu, strParentValue, soco, strAttrs, (int)ListObjectDelete.COMMON);
		}

		/// <summary>
		/// ��ȡָ���û���������"��Ա��"����
		/// </summary>
		/// <param name="strUserValues">ָ�����û���ʶ�����֮����á�,���ָ���</param>
		/// <param name="socu">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strAttrs">��Ҫ���ȡ��������Ϣ</param>
		/// <returns>��ȡָ����Ա������"��Ա��"���ϣ�ע�������߼�ɾ�������ݶ���</returns>
		public static DataSet GetGroupsOfUsers(string strUserValues, SearchObjectColumn socu, string strAttrs)
		{
			return GetGroupsOfUsers(strUserValues, socu, string.Empty, SearchObjectColumn.SEARCH_NULL, strAttrs);
		}

		/// <summary>
		/// ��ȡָ���û���������"��Ա��"����
		/// </summary>
		/// <param name="strUserGuids">ָ�����û���ʶGUID�����GUID֮����á�,���ָ���</param>
		/// <param name="strAttrs">��Ҫ���ȡ��������Ϣ</param>
		/// <returns>��ȡָ����Ա������"��Ա��"���ϣ�ע�������߼�ɾ�������ݶ���</returns>
		public static DataSet GetGroupsOfUsers(string strUserGuids, string strAttrs)
		{
			return GetGroupsOfUsers(strUserGuids, SearchObjectColumn.SEARCH_GUID, strAttrs);
		}

		/// <summary>
		/// ��ȡָ���û���������"��Ա��"����
		/// </summary>
		/// <param name="strUserGuids">ָ�����û���ʶGUID�����GUID֮����á�,���ָ���</param>
		/// <returns>��ȡָ����Ա������"��Ա��"���ϣ�ע�������߼�ɾ�������ݶ���</returns>
		public static DataSet GetGroupsOfUsers(string strUserGuids)
		{
			return GetGroupsOfUsers(strUserGuids, string.Empty);
		}

		#endregion

		#region GetSecretariesOfLeaders
		/// <summary>
		/// ��ȡָ���쵼�����������˳�Ա
		/// </summary>
		/// <param name="strLeaderValues">ָ���쵼�ı�ʶ�����֮�����","�ָ���</param>
		/// <param name="soc">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
		/// <param name="iLod">���β�ѯ��Ҫ���ѯ�����״̬��Ϣ���ݣ��Ƿ�����߼�ɾ������</param>
		/// <returns>��ȡָ���쵼�����������˳�Ա</returns>
		public static DataSet GetSecretariesOfLeaders(string strLeaderValues,
			SearchObjectColumn soc,
			string strAttrs,
			int iLod)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strLeaderValues, soc, strAttrs, iLod);
			DataSet result;
			//if (false == GetSecretariesOfLeadersQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetSecretariesOfLeadersQueue))
			//    {
			if (false == GetSecretariesOfLeadersQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);

				strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);

				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					string strSql = @"
				SELECT 'USERS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "USERS", "OU_USERS", "RANK_DEFINE", "SECRETARIES") + @"
				FROM OU_USERS, SECRETARIES, USERS JOIN RANK_DEFINE 
					ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
				WHERE USERS.GUID = OU_USERS.USER_GUID
					AND OU_USERS.USER_GUID = SECRETARIES.SECRETARY_GUID
					AND OU_USERS.SIDELINE = 0
					AND SECRETARIES.LEADER_GUID IN	(
														SELECT USERS.GUID 
														FROM USERS, OU_USERS 
														WHERE USERS.GUID = OU_USERS.USER_GUID 
															AND {0} IN ({1})
													)
					AND ({2})
				ORDER BY SECRETARIES.LEADER_GUID, RANK_DEFINE.SORT_ID;
				";

					strSql = string.Format(strSql,
						DatabaseSchema.Instence.GetTableColumns(strColName, "OU_USERS", "USERS"),
						OGUCommonDefine.AddMulitStrWithQuotationMark(strLeaderValues),
						GetSqlSearchStatus("OU_USERS", (ListObjectDelete)iLod));

					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetSecretariesOfLeadersQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		///// <summary>
		///// ��ȡָ���쵼�����������˳�Ա
		///// </summary>
		///// <param name="strLeaderValues">ָ���쵼�ı�ʶ�����֮�����","�ָ���</param>
		///// <param name="soc">�û�����������
		///// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		///// </param>
		///// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
		///// <param name="iLod">���β�ѯ��Ҫ���ѯ�����״̬��Ϣ���ݣ��Ƿ�����߼�ɾ������</param>
		///// <returns>��ȡָ���쵼�����������˳�Ա</returns>
		//public static DataSet GetSecretariesOfLeaders(string strLeaderValues, SearchObjectColumn soc, string strAttrs, int iLod)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetSecretariesOfLeaders(strLeaderValues, soc, strAttrs, iLod, da);
		//    }
		//}

		/// <summary>
		/// ��ȡָ���쵼�����������˳�Ա
		/// </summary>
		/// <param name="strLeaderValues">ָ���쵼�ı�ʶ�����֮�����","�ָ���</param>
		/// <param name="soc">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
		/// <returns>��ȡָ���쵼�����������˳�Ա</returns>
		public static DataSet GetSecretariesOfLeaders(string strLeaderValues, SearchObjectColumn soc, string strAttrs)
		{
			return GetSecretariesOfLeaders(strLeaderValues, soc, strAttrs, (int)ListObjectDelete.COMMON);
		}

		/// <summary>
		/// ��ȡָ���쵼�����������˳�Ա
		/// </summary>
		/// <param name="strLeaderGuids">ָ���쵼�ı�ʶGUID�����GUID֮�����","�ָ���</param>
		/// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
		/// <returns>��ȡָ���쵼�����������˳�Ա</returns>
		public static DataSet GetSecretariesOfLeaders(string strLeaderGuids, string strAttrs)
		{
			return GetSecretariesOfLeaders(strLeaderGuids, SearchObjectColumn.SEARCH_GUID, strAttrs);
		}

		/// <summary>
		/// ��ȡָ���쵼�����������˳�Ա
		/// </summary>
		/// <param name="strLeaderGuids">ָ���쵼�ı�ʶGUID�����GUID֮�����","�ָ���</param>
		/// <returns>��ȡָ���쵼�����������˳�Ա</returns>
		public static DataSet GetSecretariesOfLeaders(string strLeaderGuids)
		{
			return GetSecretariesOfLeaders(strLeaderGuids, string.Empty);
		}

		#endregion

		#region GetLeadersOfSecretaries
		/// <summary>
		/// ��ȡָ������������쵼�˳�Ա
		/// </summary>
		/// <param name="strSecValues">ָ������ı�ʶ�����֮�����","�ָ���</param>
		/// <param name="soc">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
		/// <param name="iLod">���β�ѯ��Ҫ���ѯ�����״̬��Ϣ���ݣ��Ƿ�����߼�ɾ������</param>
		/// <returns>��ȡָ������������쵼�˳�Ա</returns>
		public static DataSet GetLeadersOfSecretaries(string strSecValues,
			SearchObjectColumn soc,
			string strAttrs,
			int iLod)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strSecValues, soc, strAttrs, iLod);
			DataSet result;
			//if (false == GetLeadersOfSecretariesQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetLeadersOfSecretariesQueue))
			//    {
			if (false == GetLeadersOfSecretariesQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);
				strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);

				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					string strSql = @"
				SELECT 'USERS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "USERS", "OU_USERS", "RANK_DEFINE", "SECRETARIES") + @"
				FROM OU_USERS, SECRETARIES, USERS JOIN RANK_DEFINE 
					ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
				WHERE USERS.GUID = OU_USERS.USER_GUID
					AND OU_USERS.USER_GUID = SECRETARIES.LEADER_GUID
					AND OU_USERS.SIDELINE = 0
					AND SECRETARIES.SECRETARY_GUID IN	(
															SELECT USERS.GUID 
															FROM USERS, OU_USERS 
															WHERE USERS.GUID = OU_USERS.USER_GUID 
																AND {0} IN ({1})
														)
					AND ({2})
				ORDER BY SECRETARIES.SECRETARY_GUID, RANK_DEFINE.SORT_ID
				";

					strSql = string.Format(strSql,
						DatabaseSchema.Instence.GetTableColumns(strColName, "OU_USERS", "USERS"),
						OGUCommonDefine.AddMulitStrWithQuotationMark(strSecValues),
						GetSqlSearchStatus("OU_USERS", (ListObjectDelete)iLod));

					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetLeadersOfSecretariesQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		///// <summary>
		///// ��ȡָ������������쵼�˳�Ա
		///// </summary>
		///// <param name="strSecValues">ָ������ı�ʶ�����֮�����","�ָ���</param>
		///// <param name="soc">�û�����������
		///// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		///// </param>
		///// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
		///// <param name="iLod">���β�ѯ��Ҫ���ѯ�����״̬��Ϣ���ݣ��Ƿ�����߼�ɾ������</param>
		///// <returns>��ȡָ������������쵼�˳�Ա</returns>
		//public static DataSet GetLeadersOfSecretaries(string strSecValues, SearchObjectColumn soc, string strAttrs, int iLod)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetLeadersOfSecretaries(strSecValues, soc, strAttrs, iLod, da);
		//    }
		//}

		/// <summary>
		/// ��ȡָ������������쵼�˳�Ա
		/// </summary>
		/// <param name="strSecValues">ָ������ı�ʶ�����֮�����","�ָ���</param>
		/// <param name="soc">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
		/// <returns>��ȡָ������������쵼�˳�Ա</returns>
		public static DataSet GetLeadersOfSecretaries(string strSecValues, SearchObjectColumn soc, string strAttrs)
		{
			return GetLeadersOfSecretaries(strSecValues, soc, strAttrs, (int)ListObjectDelete.COMMON);
		}

		/// <summary>
		/// ��ȡָ������������쵼�˳�Ա
		/// </summary>
		/// <param name="strSecGuids">ָ������ı�ʶGUID�����GUID֮�����","�ָ���</param>
		/// <param name="strAttrs">Ҫ���ڱ��β�ѯ�л�ȡ������ֶ�����</param>
		/// <returns>��ȡָ������������쵼�˳�Ա</returns>
		public static DataSet GetLeadersOfSecretaries(string strSecGuids, string strAttrs)
		{
			return GetLeadersOfSecretaries(strSecGuids, SearchObjectColumn.SEARCH_GUID, strAttrs);
		}

		/// <summary>
		/// ��ȡָ������������쵼�˳�Ա
		/// </summary>
		/// <param name="strSecGuids">ָ������ı�ʶGUID�����GUID֮�����","�ָ���</param>
		/// <returns>��ȡָ������������쵼�˳�Ա</returns>
		public static DataSet GetLeadersOfSecretaries(string strSecGuids)
		{
			return GetLeadersOfSecretaries(strSecGuids, string.Empty);
		}

		#endregion

		#region GetObjectParentOrgs
		/// <summary>
		/// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ����ţ�
		/// </summary>
		/// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
		/// <param name="strObjValues">���������е�����ֵ</param>
		/// <param name="soc">���������е��������ͣ�GUID��OBJ_NAME��ORIGINAL_SORT��GLOBAL_SORT�ȣ�</param>
		/// <param name="bOnlyDirectly">�Ƿ������ȡ��ӽ��Ļ�������</param>
		/// <param name="bWithVisiual">�Ƿ�Ҫ��������ⲿ��</param>
		/// <param name="strOrgRankCodeName">Ҫ����͵Ļ�����������</param>
		/// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
		/// <returns>��ȡָ������ĸ����Ŷ���</returns>
		public static DataSet GetObjectParentOrgs(string strObjType,
			string strObjValues,
			SearchObjectColumn soc,
			bool bOnlyDirectly,
			bool bWithVisiual,
			string strOrgRankCodeName,
			string strAttrs)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strObjType, strObjValues, soc, bOnlyDirectly, bWithVisiual, strOrgRankCodeName, strAttrs);
			DataSet result;
			//if (false == GetObjectParentOrgsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetObjectParentOrgsQueue))
			//    {
			if (false == GetObjectParentOrgsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);

				strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);

				string strSql = string.Empty;

				string strWhere = string.Empty;
				string strWhereRankDefine = string.Empty;

				if (false == bWithVisiual)//�����ⲿ��
					strWhere = " AND ORGANIZATIONS.ORG_TYPE <> 1 ";
				if (strOrgRankCodeName.Length > 0)
					strWhereRankDefine = " AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
						+ TSqlBuilder.Instance.CheckQuotationMark(strOrgRankCodeName.Trim(), true) + ") ";

				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					switch (strObjType)
					{
						case "ORGANIZATIONS":
						case "GROUPS":
							#region ORGANIZATIONS And GROUPS
							strSql = @"
						SELECT {0} 'ORGANIZATIONS' AS OBJECTCLASS, "
								+ DatabaseSchema.Instence.GetTableColumns(strAttrs, "ORGANIZATIONS", "RANK_DEFINE") + @"
						FROM ORGANIZATIONS JOIN RANK_DEFINE 
							ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME " + strWhereRankDefine + @"
						WHERE (SELECT ORIGINAL_SORT FROM {1} WHERE {2}) LIKE ORGANIZATIONS.ORIGINAL_SORT + '%'
							" + strWhere + @" 
						ORDER BY ORGANIZATIONS.ORIGINAL_SORT DESC";

							string strOnlyDirectly = string.Empty;
							if (bOnlyDirectly)
								strOnlyDirectly = " TOP 2 "; //strOnlyDirectly = " TOP 1 ";//Modify By Yuanyong 20080723���Ϊ1�Ļ���ֻ�ܷ����������������

							string strOrgSelf = DatabaseSchema.Instence.GetTableColumns(strColName, strObjType)
								+ " IN (" + OGUCommonDefine.AddMulitStrWithQuotationMark(strObjValues) + ") ";

							strSql = string.Format(strSql, strOnlyDirectly, TSqlBuilder.Instance.CheckQuotationMark(strObjType, false), strOrgSelf);
							break;
							#endregion
						case "USERS":
							#region USERS
							strWhere += " AND " + DatabaseSchema.Instence.GetTableColumns(strColName, strObjType, "OU_USERS") + " IN ("
								+ OGUCommonDefine.AddMulitStrWithQuotationMark(strObjValues) + ") ";

							if (bOnlyDirectly)
							{
								strSql = @"
							SELECT 'ORGANIZATIONS' AS OBJECTCLASS, S.*
							FROM (
									SELECT OU_USERS.GLOBAL_SORT AS USERS_GLOBAL_SORT, MAX(LEN(ORGANIZATIONS.ORIGINAL_SORT)) AS MAX_LEN
									FROM USERS, OU_USERS, ORGANIZATIONS JOIN RANK_DEFINE 
										ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME
										{2}
									WHERE 	USERS.GUID = OU_USERS.USER_GUID
										AND OU_USERS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + '%'
										{0}
									GROUP BY OU_USERS.GLOBAL_SORT
								) G, 
								(	
									SELECT OU_USERS.GLOBAL_SORT AS USERS_GLOBAL_SORT, {1}
									FROM USERS, OU_USERS, ORGANIZATIONS JOIN RANK_DEFINE 
										ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME
										{2}
									WHERE 	USERS.GUID = OU_USERS.USER_GUID
										AND OU_USERS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + '%'
										{0}
								) S
							WHERE G.USERS_GLOBAL_SORT = S.USERS_GLOBAL_SORT
								AND G.MAX_LEN = LEN(S.ORIGINAL_SORT)
							ORDER BY S.USERS_GLOBAL_SORT DESC
							";
							}
							else
							{
								strSql = @"
							SELECT 'ORGANIZATIONS' AS OBJECTCLASS, {1}
							FROM USERS, OU_USERS, ORGANIZATIONS JOIN RANK_DEFINE 
								ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME
								{2}
							WHERE USERS.GUID = OU_USERS.USER_GUID
								AND OU_USERS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + '%'
								{0}
							ORDER BY OU_USERS.ORIGINAL_SORT, ORGANIZATIONS.ORIGINAL_SORT DESC
							";
							}

							strSql = string.Format(strSql, strWhere,
								DatabaseSchema.Instence.GetTableColumns(strAttrs, "ORGANIZATIONS", "RANK_DEFINE", "OU_USERS", "USERS"),
								strWhereRankDefine);
							break;
							#endregion
					}

					Database database = DatabaseFactory.Create(CommonResource.AccreditConnAlias);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetObjectParentOrgsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		///// <summary>
		///// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ����ţ�
		///// </summary>
		///// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
		///// <param name="strObjValues">���������е�����ֵ(���֮�����","�ָ�)</param>
		///// <param name="soc">���������е��������ͣ�GUID��OBJ_NAME��ORIGINAL_SORT��GLOBAL_SORT�ȣ�</param>
		///// <param name="bOnlyDirectly">�Ƿ������ȡ��ӽ��Ļ�������</param>
		///// <param name="bWithVisiual">�Ƿ�Ҫ��������ⲿ��</param>
		///// <param name="strOrgRankCodeName">Ҫ����͵Ļ�����������</param>
		///// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
		///// <returns>��ȡָ������ĸ����Ŷ���</returns>
		//public static DataSet GetObjectParentOrgs(string strObjType, string strObjValues, SearchObjectColumn soc, bool bOnlyDirectly, bool bWithVisiual, string strOrgRankCodeName, string strAttrs)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetObjectParentOrgs(strObjType, strObjValues, soc, bOnlyDirectly, bWithVisiual, strOrgRankCodeName, strAttrs, da);
		//    }
		//}

		/// <summary>
		/// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ����ţ�
		/// </summary>
		/// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
		/// <param name="strObjValues">���������е�����ֵ(���֮�����","�ָ�)</param>
		/// <param name="soc">���������е��������ͣ�GUID��OBJ_NAME��ORIGINAL_SORT��GLOBAL_SORT�ȣ�</param>
		/// <param name="bWithVisiual">�Ƿ�Ҫ��������ⲿ��</param>
		/// <param name="strOrgRankCodeName">Ҫ����͵Ļ�����������</param>
		/// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
		/// <returns>��ȡָ������ĸ����Ŷ���</returns>
		public static DataSet GetObjectParentOrgs(string strObjType,
			string strObjValues,
			SearchObjectColumn soc,
			bool bWithVisiual,
			string strOrgRankCodeName,
			string strAttrs)
		{
			return GetObjectParentOrgs(strObjType, strObjValues, soc, false, bWithVisiual, strOrgRankCodeName, strAttrs);
		}

		/// <summary>
		/// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ����ţ�
		/// </summary>
		/// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
		/// <param name="strObjValues">���������е�����ֵ(���֮�����","�ָ�)</param>
		/// <param name="soc">���������е��������ͣ�GUID��OBJ_NAME��ORIGINAL_SORT��GLOBAL_SORT�ȣ�</param>
		/// <param name="strOrgRankCodeName">Ҫ����͵Ļ�����������</param>
		/// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
		/// <returns>��ȡָ������ĸ����Ŷ���</returns>
		public static DataSet GetObjectParentOrgs(string strObjType,
			string strObjValues,
			SearchObjectColumn soc,
			string strOrgRankCodeName,
			string strAttrs)
		{
			return GetObjectParentOrgs(strObjType, strObjValues, soc, false, strOrgRankCodeName, strAttrs);
		}

		/// <summary>
		/// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ����ţ�
		/// </summary>
		/// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
		/// <param name="strObjValues">���������е�����ֵ(���֮�����","�ָ�)</param>
		/// <param name="soc">���������е��������ͣ�GUID��OBJ_NAME��ORIGINAL_SORT��GLOBAL_SORT�ȣ�</param>
		/// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
		/// <returns>��ȡָ������ĸ����Ŷ���</returns>
		public static DataSet GetObjectParentOrgs(string strObjType, string strObjValues, SearchObjectColumn soc, string strAttrs)
		{
			return GetObjectParentOrgs(strObjType, strObjValues, soc, string.Empty, strAttrs);
		}

		/// <summary>
		/// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ����ţ�
		/// </summary>
		/// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
		/// <param name="strObjGuids">���������е�����ֵGUID(���GUID֮�����","�ָ�)</param>
		/// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
		/// <returns>��ȡָ������ĸ����Ŷ���</returns>
		public static DataSet GetObjectParentOrgs(string strObjType, string strObjGuids, string strAttrs)
		{
			return GetObjectParentOrgs(strObjType, strObjGuids, SearchObjectColumn.SEARCH_GUID, strAttrs);
		}

		/// <summary>
		/// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ����ţ�
		/// </summary>
		/// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
		/// <param name="strObjGuids">���������е�����ֵGUID(���GUID֮�����","�ָ�)</param>
		/// <returns>��ȡָ������ĸ����Ŷ���</returns>
		public static DataSet GetObjectParentOrgs(string strObjType, string strObjGuids)
		{
			return GetObjectParentOrgs(strObjType, strObjGuids, string.Empty);
		}

		#endregion

		#region GetObjectDepOrgs
		/// <summary>
		/// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ�����(ָ�����)��
		/// </summary>
		/// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
		/// <param name="strObjValues">���������е�����ֵ</param>
		/// <param name="soc">���������е��������ͣ�GUID��OBJ_NAME��ORIGINAL_SORT��GLOBAL_SORT�ȣ�</param>
		/// <param name="iDep">Ҫ���ȡ�����</param>
		/// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
		/// <returns>��ȡָ������ĸ����Ŷ���</returns>
		public static DataSet GetObjectDepOrgs(string strObjType,
			string strObjValues,
			SearchObjectColumn soc,
			int iDep,
			string strAttrs)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strObjType, strObjValues, soc, iDep, strAttrs);
			DataSet result;
			//if (false == GetObjectDepOrgsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetObjectDepOrgsQueue))
			//    {
			if (false == GetObjectDepOrgsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				ExceptionHelper.TrueThrow(iDep <= 0, "������iDep����ֵ�������0��");

				strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);
				string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);

				int iLength = iDep * CommonResource.OriginalSortDefault.Length;// OGUCommonDefine.OGU_ORIGINAL_SORT.Length;

				string strSql = string.Empty;
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					switch (strObjType)
					{
						case "ORGANIZATIONS":
						case "GROUPS":
							#region ORGANIZATIONS And GROUPS
							strSql = @"
						SELECT 'ORGANIZATIONS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "ORGANIZATIONS", "RANK_DEFINE") + @" 
						FROM	(	
									SELECT ORIGINAL_SORT 
									FROM " + strObjType + @" 
									WHERE " + strColName + " IN (" + OGUCommonDefine.AddMulitStrWithQuotationMark(strObjValues) + @") 
								) ROS,
							ORGANIZATIONS JOIN RANK_DEFINE 
								ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME 
						WHERE LEN(ORGANIZATIONS.ORIGINAL_SORT) = " + iLength.ToString() + @" 
							AND ROS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + '%' 
						ORDER BY RANK_DEFINE.SORT_ID, ORGANIZATIONS.GLOBAL_SORT ";
							break;
							#endregion
						case "USERS":
							#region USERS
							strSql = @"
						SELECT 'ORGANIZATIONS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "ORGANIZATIONS", "RANK_DEFINE") + @" 
						FROM	(	
									SELECT OU_USERS.ORIGINAL_SORT 
									FROM USERS, OU_USERS 
									WHERE USERS.GUID = OU_USERS.USER_GUID 
										AND " + DatabaseSchema.Instence.GetTableColumns(strColName, "USERS", "OU_USERS")
													  + " IN (" + OGUCommonDefine.AddMulitStrWithQuotationMark(strObjValues) + @")
								)  ROS,
							ORGANIZATIONS JOIN RANK_DEFINE 
								ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME 
						WHERE LEN(ORGANIZATIONS.ORIGINAL_SORT) = " + iLength.ToString() + @" 
							AND ROS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + '%'
						ORDER BY RANK_DEFINE.SORT_ID, ORGANIZATIONS.GLOBAL_SORT";
							break;
							#endregion
						default:
							ExceptionHelper.TrueThrow(true, "��" + strObjType + "��δ֪�Ķ����������ͣ�");
							break;
					}
					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetObjectDepOrgsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		///// <summary>
		///// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ�����(ָ�����)��
		///// </summary>
		///// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
		///// <param name="strObjValues">���������е�����ֵ(���֮�����","�ָ�)</param>
		///// <param name="soc">���������е��������ͣ�GUID��OBJ_NAME��ORIGINAL_SORT��GLOBAL_SORT�ȣ�</param>
		///// <param name="iDep">Ҫ���ȡ�����</param>
		///// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
		///// <returns>��ȡָ������ĸ����Ŷ���</returns>
		//public static DataSet GetObjectDepOrgs(string strObjType, string strObjValues, SearchObjectColumn soc, int iDep, string strAttrs)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetObjectDepOrgs(strObjType, strObjValues, soc, iDep, strAttrs, da);
		//    }
		//}

		/// <summary>
		/// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ�����(ָ�����)��
		/// </summary>
		/// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
		/// <param name="strObjValues">���������е�����ֵ(���֮�����","�ָ�)</param>
		/// <param name="soc">���������е��������ͣ�GUID��OBJ_NAME��ORIGINAL_SORT��GLOBAL_SORT�ȣ�</param>
		/// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
		/// <returns>��ȡָ������ĸ����Ŷ���</returns>
		public static DataSet GetObjectDepOrgs(string strObjType, string strObjValues, SearchObjectColumn soc, string strAttrs)
		{
			return GetObjectDepOrgs(strObjType, strObjValues, soc, 1, strAttrs);
		}

		/// <summary>
		/// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ�����(ָ�����)��
		/// </summary>
		/// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
		/// <param name="strObjGuids">���������е�����ֵGUID(���GUID֮�����","�ָ�)</param>
		/// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
		/// <returns>��ȡָ������ĸ����Ŷ���</returns>
		public static DataSet GetObjectDepOrgs(string strObjType, string strObjGuids, string strAttrs)
		{
			return GetObjectDepOrgs(strObjType, strObjGuids, SearchObjectColumn.SEARCH_GUID, strAttrs);
		}

		/// <summary>
		/// ��ȡָ������ĸ���������GROUPS��USERS������ORGANIZATIONS�ĸ�����(ָ�����)��
		/// </summary>
		/// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
		/// <param name="strObjGuids">���������е�����ֵGUID(���GUID֮�����","�ָ�)</param>
		/// <returns>��ȡָ������ĸ����Ŷ���</returns>
		public static DataSet GetObjectDepOrgs(string strObjType, string strObjGuids)
		{
			return GetObjectDepOrgs(strObjType, strObjGuids, string.Empty);
		}

		#endregion

		#region GetObjectsSort
		/// <summary>
		/// ����ָ��������ϵͳ�����������Ժ󷵻�
		/// </summary>
		/// <param name="xmlDoc">��������Ҫ���������</param>
		/// <param name="soc">xmlDoc�������ڱ�ʶ������Աϵͳ������������</param>
		/// <param name="bSortByRank">�Ƿ�Ҫ����ü�������</param>
		/// <param name="strAttrs">������������</param>
		/// <returns>����������</returns>
		public static DataSet GetObjectsSort(XmlDocument xmlDoc,
			SearchObjectColumn soc,
			bool bSortByRank,
			string strAttrs)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(xmlDoc.DocumentElement.OuterXml, soc, bSortByRank, strAttrs);
			DataSet result;
			//if (false == GetObjectsSortQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetObjectsSortQueue))
			//    {
			if (false == GetObjectsSortQueue.Instance.TryGetValue(cacheKey, out result))
			{
				if (strAttrs.Length == 0)
					strAttrs = "RANK_CODE";
				else
				{
					if (strAttrs.IndexOf("RANK_CODE") < 0)
						strAttrs += ",RANK_CODE";
				}
				strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);
				string strColumnName = OGUCommonDefine.GetSearchObjectColumn(soc);

				StringBuilder strB = new StringBuilder(1024);
				XmlElement root = xmlDoc.DocumentElement;
				foreach (XmlElement elem in root.ChildNodes)
				{
					if (strB.Length > 0)
						strB.Append(",");

					strB.Append(TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute(strColumnName), true));
				}

				string strSortByRank = string.Empty;
				string strRankSortID = string.Empty;
				if (bSortByRank)
				{
					strSortByRank = " RANK_DEFINE.SORT_ID, ";
					strRankSortID = ", ISNULL (RANK_DEFINE.SORT_ID, 99) AS SORT_ID ";
				}

				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					#region Sql Prepare
					string strSql = @"
				SELECT RESULT.* " + strRankSortID + @"
				FROM	(
							SELECT 'ORGANIZATIONS' AS OBJECTCLASS," + DatabaseSchema.Instence.GetTableColumns(strAttrs, "ORGANIZATIONS", "RANK_DEFINE") + @"
							FROM ORGANIZATIONS JOIN RANK_DEFINE 
								ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME
							WHERE " + DatabaseSchema.Instence.GetTableColumns(strColumnName, "ORGANIZATIONS") + @" IN ({0})
						UNION
							SELECT 'GROUPS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "GROUPS") + @"
							FROM GROUPS
							WHERE " + DatabaseSchema.Instence.GetTableColumns(strColumnName, "GROUPS") + @" IN ({0})
						UNION
							SELECT 'USERS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "OU_USERS", "USERS", "RANK_DEFINE") + @"
							FROM ORGANIZATIONS, OU_USERS, USERS JOIN RANK_DEFINE 
								ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
							WHERE ORGANIZATIONS.GUID = OU_USERS.PARENT_GUID
								AND USERS.GUID = OU_USERS.USER_GUID
								AND " + DatabaseSchema.Instence.GetTableColumns(strColumnName, "OU_USERS", "USERS") + @" IN ({0})
						)RESULT JOIN RANK_DEFINE 
							ON RANK_DEFINE.CODE_NAME = RESULT.RANK_CODE
				ORDER BY " + strSortByRank + " RESULT.GLOBAL_SORT";

					strSql = string.Format(strSql, strB.ToString());
					#endregion
					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetObjectsSortQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		///// <summary>
		///// ����ָ��������ϵͳ�����������Ժ󷵻�
		///// </summary>
		///// <param name="xmlDoc">��������Ҫ���������</param>
		///// <param name="soc">xmlDoc�������ڱ�ʶ������Աϵͳ������������</param>
		///// <param name="bSortByRank">�Ƿ�Ҫ����ü�������</param>
		///// <param name="strAttrs">��������</param>
		///// <returns>����������</returns>
		//public static DataSet GetObjectsSort(XmlDocument xmlDoc, SearchObjectColumn soc, bool bSortByRank, string strAttrs)
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetObjectsSort(xmlDoc, soc, bSortByRank, strAttrs, da);
		//    }
		//}

		/// <summary>
		/// ����ָ��������ϵͳ�����������Ժ󷵻�
		/// </summary>
		/// <param name="xmlDoc">��������Ҫ���������</param>
		/// <param name="soc">xmlDoc�������ڱ�ʶ������Աϵͳ������������</param>
		/// <param name="strAttrs">��������</param>
		/// <returns>����������</returns>
		public static DataSet GetObjectsSort(XmlDocument xmlDoc, SearchObjectColumn soc, string strAttrs)
		{
			return GetObjectsSort(xmlDoc, soc, true, strAttrs);
		}

		/// <summary>
		/// ����ָ��������ϵͳ�����������Ժ󷵻�
		/// </summary>
		/// <param name="xmlDoc">��������Ҫ���������</param>
		/// <param name="soc">xmlDoc�������ڱ�ʶ������Աϵͳ������������</param>
		/// <returns>����������</returns>
		public static DataSet GetObjectsSort(XmlDocument xmlDoc, SearchObjectColumn soc)
		{
			return GetObjectsSort(xmlDoc, soc, string.Empty);
		}
		#endregion

		#region GetRootDSE
		///// <summary>
		///// ��ȡϵͳָ���ĸ�����
		///// </summary>
		///// <returns>��ȡϵͳָ���ĸ�����</returns>
		//public static DataSet GetRootDSE()
		//{
		//    DataAccess da = new DataAccess(OGUCommonDefine.STR_CONN);

		//    using (da.dBContextInfo)
		//    {
		//        da.dBContextInfo.OpenConnection();

		//        return GetRootDSE(da);
		//    }
		//}
		/// <summary>
		/// ��ȡϵͳָ���ĸ�����
		/// </summary>
		/// <returns>��ȡϵͳָ���ĸ�����</returns>
		public static DataSet GetRootDSE()
		{
			string strRootAllPathName = AccreditSection.GetConfig().AccreditSettings.OguRootName;
			DataSet result;
			//if (false == GetRootDSEQueue.Instance.TryGetValue(strRootAllPathName, out result))
			//{
			//    lock (typeof(GetRootDSEQueue))
			//    {
			if (false == GetRootDSEQueue.Instance.TryGetValue(strRootAllPathName, out result))
			{
				ExceptionHelper.TrueThrow(strRootAllPathName.Length == 0, "�Բ��������ú�ϵͳ�е�Ĭ�ϸ����ţ�");
				string strSql = "SELECT * FROM ORGANIZATIONS WHERE ALL_PATH_NAME = " + TSqlBuilder.Instance.CheckQuotationMark(strRootAllPathName, true);

				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					Database database = DatabaseFactory.Create(context);
					result = database.ExecuteDataSet(CommandType.Text, strSql);

					ExceptionHelper.TrueThrow(result.Tables[0].Rows.Count == 0, "�Բ���,ϵͳ���õ�Ĭ�ϸ����Ų�����,���֤��");
				}
				GetRootDSEQueue.Instance.Add(strRootAllPathName, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}
		#endregion

		#region SignInCheck
		/// <summary>
		/// ��֤��Ա��¼���������Ƿ���ȷ
		/// </summary>
		/// <param name="strLogonName">�û���¼��</param>
		/// <param name="strUserPwd">�û�����</param>
		/// <returns>��¼���������Ƿ�ƥ��</returns>
		public static bool SignInCheck(string strLogonName, string strUserPwd)
		{
			try
			{
				ExceptionHelper.TrueThrow<ApplicationException>(string.IsNullOrEmpty(strLogonName), "�Բ��𣬵�¼���Ʋ���Ϊ��");

				bool result = false;
				string key = strLogonName + strUserPwd;
				if (false == SignInCheckQueue.Instance.TryGetValue(key, out result))
				{
					ILogOnUserInfo logonUserInfo = new LogOnUserInfo(strLogonName, strUserPwd);

					result = (logonUserInfo != null);

					SignInCheckQueue.Instance.Add(key, result, InnerCacheHelper.PrepareDependency());
				}

				return result;
			}
			catch (ApplicationException)
			{
				return false;
			}
		}
		#endregion

		#region GetIndependOrganizationOfUser

		/// <summary>
		/// ��ȡ��ǰ�ƶ���������ڶ������ţ��������ػ�������פ������ֱ�����ء����𡢷������ɰ죩
		/// </summary>
		/// <param name="strUserGuid">���������е�����ֵ(�û�)</param>
		/// <returns>��ȡ��ǰ�ƶ���������ڶ�������</returns>
		public static DataSet GetIndependOrganizationOfUser(string strUserGuid)
		{
			return GetIndependOrganizationOfUser("USERS", strUserGuid);
		}

		/// <summary>
		/// ��ȡ��ǰ�ƶ���������ڶ������ţ��������ػ�������פ������ֱ�����ء����𡢷������ɰ죩
		/// </summary>
		/// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
		/// <param name="strObjGuid">���������е�����ֵ</param>
		/// <returns>��ȡ��ǰ�ƶ���������ڶ�������</returns>
		public static DataSet GetIndependOrganizationOfUser(string strObjType, string strObjGuid)
		{
			return GetIndependOrganizationOfUser(strObjType, strObjGuid, string.Empty);
		}

		/// <summary>
		/// ��ȡ��ǰ�ƶ���������ڶ������ţ��������ػ�������פ������ֱ�����ء����𡢷������ɰ죩
		/// </summary>
		/// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
		/// <param name="strObjGuid">���������е�����ֵ</param>
		/// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
		/// <returns>��ȡ��ǰ�ƶ���������ڶ�������</returns>
		public static DataSet GetIndependOrganizationOfUser(string strObjType, string strObjGuid, string strAttrs)
		{
			return GetIndependOrganizationOfUser(strObjType, strObjGuid, SearchObjectColumn.SEARCH_GUID, strAttrs);
		}

		/// <summary>
		/// ��ȡ��ǰ�ƶ���������ڶ������ţ��������ػ�������פ������ֱ�����ء����𡢷������ɰ죩
		/// </summary>
		/// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
		/// <param name="strObjValue">���������е�����ֵ</param>
		/// <param name="soc">���������е��������ͣ�GUID��OBJ_NAME��ORIGINAL_SORT��GLOBAL_SORT�ȣ�</param>
		/// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
		/// <returns>��ȡ��ǰ�ƶ���������ڶ�������</returns>
		public static DataSet GetIndependOrganizationOfUser(string strObjType, string strObjValue, SearchObjectColumn soc, string strAttrs)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strObjType, strObjValue, soc, strAttrs);
			DataSet result;
			//if (false == GetIndependOrganizationOfUserQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetIndependOrganizationOfUserQueue))
			//    {
			if (false == GetIndependOrganizationOfUserQueue.Instance.TryGetValue(cacheKey, out result))
			{
				if (strAttrs.IndexOf("ORG_CLASS") < 0)
				{
					if (strAttrs.Length > 0)
						strAttrs += ",ORG_CLASS";
					else
						strAttrs = "ORG_CLASS";
				}

				DataSet resultDS = GetObjectDepOrgs(strObjType, strObjValue, soc, 3, strAttrs);
				DataTable oTable = resultDS.Tables[0];
				if (oTable.Rows.Count > 0)
				{
					if (false == (oTable.Rows[0]["ORG_CLASS"] is DBNull))
						if (((int)oTable.Rows[0]["ORG_CLASS"] & (32 + 64)) != 0)
							return resultDS;
				}

				result = GetObjectDepOrgs(strObjType, strObjValue, soc, 2, strAttrs);
				GetIndependOrganizationOfUserQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}

		#endregion

		#region GetDirectCustoms
		/// <summary>
		/// ��ȡϵͳ�е�����ֱ�����أ������������õġ��������롱��
		/// </summary>
		/// <returns></returns>
		public static DataSet GetDirectCustoms()
		{
			return GetDirectCustoms(string.Empty);
		}

		/// <summary>
		/// ��ȡϵͳ�е�����ֱ�����أ������������õġ��������롱��
		/// </summary>
		/// <param name="strAttrs">����Ҫ�ĸ����ֶ�����</param>
		/// <returns></returns>
		public static DataSet GetDirectCustoms(string strAttrs)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strAttrs);
			DataSet result;
			//if (false == GetDirectCustomsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetDirectCustomsQueue))
			//    {
			if (false == GetDirectCustomsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					Database database = DatabaseFactory.Create(context);
					string strSql = @"
					SELECT " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "ORGANIZATIONS") + @" 
					FROM ORGANIZATIONS 
					WHERE LEN(CUSTOMS_CODE) = 4
						AND LEN(ORIGINAL_SORT) = 12 
					ORDER BY GLOBAL_SORT";

					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetDirectCustomsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}

		#endregion

		#region GetSubjectionCustoms
		/// <summary>
		/// ����ֱ���������Ի�ȡ��ֱ������Ͻ����������
		/// </summary>
		/// <param name="strParentOrgValue"></param>
		/// <param name="soc"></param>
		/// <param name="strAttrs"></param>
		/// <returns></returns>
		public static DataSet GetSubjectionCustoms(string strParentOrgValue, SearchObjectColumn soc, string strAttrs)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strParentOrgValue, soc, strAttrs);
			DataSet result;
			//if (false == GetSubjectionCustomsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetSubjectionCustomsQueue))
			//    {
			if (false == GetSubjectionCustomsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					if (soc == SearchObjectColumn.SEARCH_GUID)
					{
						//�������ڲ����л��洦������ֱ�ӷ��ؼ���
						return GetSubjectionCustoms(strParentOrgValue, strAttrs);
						//result = GetSubjectionCustoms(strParentOrgValue, strAttrs);
					}
					else
					{
						strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);
						string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);

						Database database = DatabaseFactory.Create(context);

						string strSql = @"
						SELECT " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "ORGANIZATIONS") + @" 
						FROM ORGANIZATIONS
						WHERE LEN(CUSTOMS_CODE) = 4
							AND PARENT_GUID IN (
								SELECT GUID 
								FROM ORGANIZATIONS 
								WHERE " + TSqlBuilder.Instance.CheckQuotationMark(strColName, false)
											+ " = " + TSqlBuilder.Instance.CheckQuotationMark(strParentOrgValue, true) + @")
						ORDER BY GLOBAL_SORT";

						result = database.ExecuteDataSet(CommandType.Text, strSql);
					}
				}
				GetSubjectionCustomsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}

		/// <summary>
		/// ����ֱ���������Ի�ȡ��ֱ������Ͻ����������
		/// </summary>
		/// <param name="strParentOrgValue"></param>
		/// <param name="soc"></param>
		/// <returns></returns>
		public static DataSet GetSubjectionCustoms(string strParentOrgValue, SearchObjectColumn soc)
		{
			return GetSubjectionCustoms(strParentOrgValue, soc, string.Empty);
		}

		/// <summary>
		/// ����ֱ���������Ի�ȡ��ֱ������Ͻ����������
		/// </summary>
		/// <param name="strParentOrgGuid"></param>
		/// <param name="strAttrs"></param>
		/// <returns></returns>
		public static DataSet GetSubjectionCustoms(string strParentOrgGuid, string strAttrs)
		{
			string cacheKey = InnerCacheHelper.BuildCacheKey(strParentOrgGuid, SearchObjectColumn.SEARCH_GUID, strAttrs);
			DataSet result;
			//if (false == GetSubjectionCustomsQueue.Instance.TryGetValue(cacheKey, out result))
			//{
			//    lock (typeof(GetSubjectionCustomsQueue))
			//    {
			if (false == GetSubjectionCustomsQueue.Instance.TryGetValue(cacheKey, out result))
			{
				strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					Database database = DatabaseFactory.Create(context);
					string strSql = @"
					SELECT " + DatabaseSchema.Instence.GetTableColumns(strAttrs, "ORGANIZATIONS") + @" 
					FROM ORGANIZATIONS
					WHERE LEN(CUSTOMS_CODE) = 4
						AND PARENT_GUID = " + TSqlBuilder.Instance.CheckQuotationMark(strParentOrgGuid, true);

					result = database.ExecuteDataSet(CommandType.Text, strSql);
				}
				GetSubjectionCustomsQueue.Instance.Add(cacheKey, result, InnerCacheHelper.PrepareDependency());
			}
			//    }
			//}
			return result;
		}

		/// <summary>
		/// ����ֱ���������Ի�ȡ��ֱ������Ͻ����������
		/// </summary>
		/// <param name="strParentOrgGuid"></param>
		/// <returns></returns>
		public static DataSet GetSubjectionCustoms(string strParentOrgGuid)
		{
			return GetSubjectionCustoms(strParentOrgGuid, string.Empty);
		}

		#endregion

		#region GetLevelSortXmlDocAttr
		/// <summary>
		/// ���ո��������е�strSortCol��ָ���ֶ���������tabls�еĶ���ʵ�ְ��������XML��
		/// </summary>
		/// <param name="table">�����������ݶ�������ݱ�</param>
		/// <param name="strSortCol">���ʵ�ֵ������ֶΣ�һ����ORIGINAL_SORT��GLOBAL_SORT��</param>
		/// <param name="strNameCol">��������Ͷ�Ӧ�ֶ����ɣ�һ����OBJECTCLASS��</param>
		/// <param name="iSortLength">��λ��ֵĳ������ݣ�Ĭ��Ϊ6��</param>
		/// <returns>���ո��������е�strSortCol��ָ���ֶ���������tabls�еĶ���ʵ�ְ��������XML��</returns>
		public static XmlDocument GetLevelSortXmlDocAttr(DataTable table, string strSortCol, string strNameCol, int iSortLength)
		{
			if (iSortLength == 0)
				iSortLength = 6;

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml("<DataTable />");
			XmlNode root = xmlDoc.DocumentElement;
			string strParentSort = string.Empty;

			foreach (DataRow row in table.Rows)
			{
				XmlElement rowElem = (XmlElement)xmlDoc.CreateNode(XmlNodeType.Element, OGUCommonDefine.DBValueToString(row[strNameCol]), string.Empty);
				string strSortValue = OGUCommonDefine.DBValueToString(row[strSortCol]);

				while (strParentSort != strSortValue.Substring(0, strSortValue.Length - iSortLength))
				{
					if (root == xmlDoc.DocumentElement)
						break;
					else
					{
						if (strParentSort.Length > 0
							&& strSortValue.Length > strParentSort.Length
							&& strParentSort == strSortValue.Substring(0, strParentSort.Length))
							break;
						//{
						//	root = root.ParentNode;
						//	strParentSort = ((XmlElement)root).GetAttribute(strSortCol);
						//}
						else
						{
							root = root.ParentNode;
							strParentSort = ((XmlElement)root).GetAttribute(strSortCol);
						}
					}
				}

				root = root.AppendChild(rowElem);
				strParentSort = strSortValue;
				foreach (DataColumn col in table.Columns)
				{
					rowElem.SetAttribute(col.ColumnName, OGUCommonDefine.DBValueToString(row[col.ColumnName]));
				}
			}

			return xmlDoc;
		}
		#endregion

		#region RemoveAllDataCache
		/// <summary>
		/// �������ݻ���
		/// </summary>
		public static void RemoveAllCache()
		{
			InnerCacheHelper.RemoveAllCache();
		}
		#endregion

		#endregion

		#region public functions ��Deleted��
		///// <summary>
		///// ��ȡ��ǰ�ƶ���������ڶ������ţ��������ػ�������פ������ֱ�����ء����𡢷������ɰ죩
		///// </summary>
		///// <param name="strUserGuid">���������е�����ֵ(�û�)</param>
		///// <param name="da">���ݿ��������</param>
		///// <returns>��ȡ��ǰ�ƶ���������ڶ�������</returns>
		//public static DataSet GetIndependOrganizationOfUser(string strUserGuid)
		//{
		//    return GetIndependOrganizationOfUser("USERS", strUserGuid, da);
		//}

		///// <summary>
		///// ��ȡ��ǰ�ƶ���������ڶ������ţ��������ػ�������פ������ֱ�����ء����𡢷������ɰ죩
		///// </summary>
		///// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
		///// <param name="strObjGuid">���������е�����ֵ</param>
		///// <param name="da">���ݿ��������</param>
		///// <returns>��ȡ��ǰ�ƶ���������ڶ�������</returns>
		//public static DataSet GetIndependOrganizationOfUser(string strObjType, string strObjGuid)
		//{
		//    return GetIndependOrganizationOfUser(strObjType, strObjGuid, string.Empty, da);
		//}

		///// <summary>
		///// ��ȡ��ǰ�ƶ���������ڶ������ţ��������ػ�������פ������ֱ�����ء����𡢷������ɰ죩
		///// </summary>
		///// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
		///// <param name="strObjGuid">���������е�����ֵ</param>
		///// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
		///// <param name="da">���ݿ��������</param>
		///// <returns>��ȡ��ǰ�ƶ���������ڶ�������</returns>
		//public static DataSet GetIndependOrganizationOfUser(string strObjType, string strObjGuid, string strAttrs)
		//{
		//    return GetIndependOrganizationOfUser(strObjType, strObjGuid, SearchObjectColumn.SEARCH_GUID, strAttrs, da);
		//}

		///// <summary>
		///// ��ȡ��ǰ�ƶ���������ڶ������ţ��������ػ�������פ������ֱ�����ء����𡢷������ɰ죩
		///// </summary>
		///// <param name="strObjType">Ҫ��ѯ����Ķ���GROUPS��USERS����ORGANIZATIONS��</param>
		///// <param name="strObjValue">���������е�����ֵ</param>
		///// <param name="soc">���������е��������ͣ�GUID��OBJ_NAME��ORIGINAL_SORT��GLOBAL_SORT�ȣ�</param>
		///// <param name="strAttrs">Ҫ���ȡ�������ֶ�</param>
		///// <param name="da">���ݿ��������</param>
		///// <returns>��ȡ��ǰ�ƶ���������ڶ�������</returns>
		//public static DataSet GetIndependOrganizationOfUser(string strObjType, 
		//    string strObjValue, 
		//    SearchObjectColumn soc, 
		//    string strAttrs)
		//{
		//    if (strAttrs.IndexOf("ORG_CLASS") < 0)
		//    {
		//        if (strAttrs.Length > 0)
		//            strAttrs += ",ORG_CLASS";
		//        else
		//            strAttrs = "ORG_CLASS";
		//    }

		//    DataSet resultDS = GetObjectDepOrgs(strObjType, strObjValue, soc, 3, strAttrs, da);
		//    DataTable oTable = resultDS.Tables[0];
		//    if (oTable.Rows.Count > 0)
		//    {
		//        if (false==(oTable.Rows[0]["ORG_CLASS"] is DBNull))
		//            if (((int)oTable.Rows[0]["ORG_CLASS"] & (32 + 64)) != 0)
		//                return resultDS;
		//    }

		//    resultDS = GetObjectDepOrgs(strObjType, strObjValue, soc, 2, strAttrs, da);

		//    return resultDS;
		//}
		#endregion

		#region private functions
		/// <summary>
		/// �γɸ���ָ��������ѯ��������������SQL���
		/// </summary>
		/// <param name="strOrgValues">ָ���ķ�Χ������ʶ�����֮�����","�ָ���</param>
		/// <param name="soc">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�
		/// </param>
		/// <param name="strLikeName">�����ϵ�����ģ��ƥ�����</param>
		/// <param name="bLike">�Ƿ����ģ��ƥ��</param>
		/// <param name="strOrgRankCodeName">����Ҫ��ļ���Χ</param>
		/// <param name="strAttr">Ҫ���ȡ���ֶ�����</param>
		/// <param name="iDep">Ҫ���ѯ�����</param>
		/// <param name="strHideType">Ҫ�����ε���������</param>
		/// <param name="rootPath">�����ORIGINAL_SORT</param>
		/// <returns>�γɸ���ָ��������ѯ������SQL���</returns>
		/// 
		//2009-05-11
		private static string QueryOrganizationsByCondition(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			bool bLike,
			string strOrgRankCodeName,
			string strAttr,
			int iDep,
			string strHideType,
			string rootPath)
		{
			string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);

			StringBuilder strB = new StringBuilder(1024);
			strB.Append("	SELECT 'ORGANIZATIONS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttr, "ORGANIZATIONS", "RANK_DEFINE") + @"
							FROM ORGANIZATIONS JOIN RANK_DEFINE 
								ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME
									{0} 
							WHERE ( " + GetSqlSearchParOriginal2("ORGANIZATIONS", iDep, rootPath) + " ) ");

			string strListDelete = GetSqlSearchStatus("ORGANIZATIONS", ListObjectDelete.ALL_TYPE);
			if (strListDelete.Length > 0)
				strB.Append(" \n AND ( " + strListDelete + " ) ");

			string strRankLimit = string.Empty;
			if (strOrgRankCodeName.Length > 0)
				strRankLimit = " AND RANK_CLASS = 1 AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
					+ TSqlBuilder.Instance.CheckQuotationMark(strOrgRankCodeName, true) + " ) ";

			strB.Append(BuildSearchCondition("ORGANIZATIONS.SEARCH_NAME", strLikeName, bLike));

			if (strHideType.Length > 0)
				strB.Append(GetHideTypeFromXmlForLike(strHideType, "ORGANIZATIONS"));

			return string.Format(strB.ToString(), strRankLimit);
		}

		//2009-05-06 ɾ�� RANK_DEFINE Լ��������ORIGINAL_SORTԼ��
		private static string QueryOrganizationsByCondition2(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			bool bLike,
			string strAttr,
			ListObjectDelete iLod,
			int iDep,
			string strHideType,
			string rootPath)
		{
			string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);

			StringBuilder strB = new StringBuilder(1024);
			strB.Append("	SELECT 'ORGANIZATIONS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttr, "ORGANIZATIONS") + @"
							FROM ORGANIZATIONS 								
							WHERE ( " + GetSqlSearchParOriginal2("ORGANIZATIONS", iDep, rootPath) + " ) ");

			string strListDelete = GetSqlSearchStatus("ORGANIZATIONS", iLod);
			if (strListDelete.Length > 0)
				strB.Append(" \n AND ( " + strListDelete + " ) ");

			strB.Append(BuildSearchCondition("ORGANIZATIONS.SEARCH_NAME", strLikeName, bLike));

			if (strHideType.Length > 0)
				strB.Append(GetHideTypeFromXmlForLike(strHideType, "ORGANIZATIONS"));

			return strB.ToString();
		}

		/// <summary>
		/// �γɸ���ָ��������ѯ����Ա�顱��SQL���
		/// </summary>
		/// <param name="strOrgValues">ָ���ķ�Χ������ʶ�����֮�����","�ָ���</param>
		/// <param name="soc">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strLikeName">����Ա�顱�ϵ�����ģ��ƥ�����</param>
		/// <param name="bLike">�Ƿ����ģ��ƥ��</param>
		/// <param name="strAttr">Ҫ���ȡ���ֶ�����</param>
		/// <param name="iDep">Ҫ���ѯ�����</param>
		/// <param name="strHideType">Ҫ�����ε���������</param>
		/// <param name="rootPath">�����ORIGINAL_SORT</param>
		/// <returns>�γɸ���ָ��������ѯ����Ա�顱��SQL���</returns>
		/// 
		//2009-05-11
		private static string QueryGroupsByCondition(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			bool bLike,
			string strAttr,
			int iDep,
			string strHideType,
			string rootPath)
		{
			string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);

			StringBuilder strB = new StringBuilder(1024);
			strB.Append(@"	SELECT 'GROUPS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttr, "GROUPS") + @"
							FROM GROUPS 
							WHERE ( " + GetSqlSearchParOriginal2("GROUPS", iDep, rootPath) + " )");

			string strListDelete = GetSqlSearchStatus("GROUPS", ListObjectDelete.ALL_TYPE);
			if (strListDelete.Length > 0)
				strB.Append(" \n AND ( " + strListDelete + " ) ");

			strB.Append(BuildSearchCondition("GROUPS.SEARCH_NAME", strLikeName, bLike));

			if (strHideType.Length > 0)
				strB.Append(GetHideTypeFromXmlForLike(strHideType, "GROUPS"));

			return strB.ToString();
		}

		/// <summary>
		/// �γɸ���ָ��������ѯ����Ա�顱��SQL���
		/// </summary>
		/// <param name="strOrgValues">ָ���ķ�Χ������ʶ�����֮�����","�ָ���</param>
		/// <param name="soc">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strLikeName">����Ա�顱�ϵ�����ģ��ƥ�����</param>
		/// <param name="bLike">�Ƿ����ģ��ƥ��</param>
		/// <param name="strAttr">Ҫ���ȡ���ֶ�����</param>
		/// <param name="iLod">��ѯɾ���Ķ������</param>
		/// <param name="iDep">Ҫ���ѯ�����</param>
		/// <param name="strHideType">Ҫ�����ε���������</param>
		/// <param name="rootPath">�����ORIGINAL_SORT</param>
		/// <returns>�γɸ���ָ��������ѯ����Ա�顱��SQL���</returns>
		/// 
		//2009-05-06 ɾ�� RANK_DEFINE Լ��������ORIGINAL_SORTԼ��
		private static string QueryGroupsByCondition2(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			bool bLike,
			string strAttr,
			ListObjectDelete iLod,
			int iDep,
			string strHideType,
			string rootPath)
		{
			string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);

			StringBuilder strB = new StringBuilder(1024);
			strB.Append(@"	SELECT 'GROUPS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(strAttr, "GROUPS") + @"
							FROM GROUPS
							WHERE ( " + GetSqlSearchParOriginal2("GROUPS", iDep, rootPath) + " )");

			string strListDelete = GetSqlSearchStatus("GROUPS", iLod);
			if (strListDelete.Length > 0)
				strB.Append(" \n AND ( " + strListDelete + " ) ");

			strB.Append(BuildSearchCondition("GROUPS.SEARCH_NAME", strLikeName, bLike));

			if (strHideType.Length > 0)
				strB.Append(GetHideTypeFromXmlForLike(strHideType, "GROUPS"));

			return strB.ToString();
		}

		/// <summary>
		/// �γɸ���ָ��������ѯ����Ա����SQL���
		/// </summary>
		/// <param name="strOrgValues">ָ���ķ�Χ������ʶ�����֮�����","�ָ���</param>
		/// <param name="soc">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strLikeName">����Ա���ϵ�����ģ��ƥ�����</param>
		/// <param name="bLike">�Ƿ����ģ��ƥ��</param>
		/// <param name="bFirstPerson">��ѯ�����Ƿ���һ����</param>
		/// <param name="strUserRankCodeName">����Ա��Ҫ��ļ���Χ</param>
		/// <param name="strAttr">Ҫ���ȡ���ֶ�����</param>
		/// <param name="iListObjType">�����Ƿ��ѯ��ְ����</param>
		/// <param name="iDep">Ҫ���ѯ�����</param>
		/// <param name="strHideType">Ҫ�����ε���������</param>
		/// <param name="rootPath">�����ORIGINAL_SORT</param>
		/// <returns>�γɸ���ָ��������ѯ����Ա����SQL���</returns>
		/// 
		//2009-05-11
		private static string QueryUsersByCondition(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			bool bLike,
			bool bFirstPerson,
			string strUserRankCodeName,
			string strAttr,
			int iListObjType,
			int iDep,
			string strHideType,
			string rootPath)
		{
			string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);

			StringBuilder strB = new StringBuilder(1024);
			strB.Append(@"	SELECT 'USERS' AS OBJECTCLASS, "
							+ DatabaseSchema.Instence.GetTableColumns(strAttr, "USERS", "OU_USERS", "RANK_DEFINE") + @"
							FROM USERS JOIN RANK_DEFINE 
								ON RANK_DEFINE.CODE_NAME = USERS.RANK_CODE {0}, OU_USERS  
							WHERE USERS.GUID = OU_USERS.USER_GUID 
								AND ( " + GetSqlSearchParOriginal2("OU_USERS", iDep, rootPath) + " )");

			string strListDelete = GetSqlSearchStatus("OU_USERS", ListObjectDelete.ALL_TYPE);
			if (strListDelete.Length > 0)
				strB.Append(" \n AND ( " + strListDelete + " ) ");

			string strRankLimit = string.Empty;
			if (strUserRankCodeName.Length > 0)
				strRankLimit = " AND RANK_CLASS = 2 AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
					+ TSqlBuilder.Instance.CheckQuotationMark(strUserRankCodeName, true) + " ) ";

			if ((iListObjType & (int)ListObjectType.SIDELINE) == 0)
				strB.Append(" \n AND OU_USERS.SIDELINE = 0 ");

			strB.Append(BuildSearchCondition("OU_USERS.SEARCH_NAME", strLikeName, bLike));

			if (bFirstPerson)//Ҫ���ǲ���һ����
				strB.Append("	AND OU_USERS.INNER_SORT = " + TSqlBuilder.Instance.CheckQuotationMark(CommonResource.OriginalSortDefault, true));

			if (strHideType.Length > 0)
				strB.Append(GetHideTypeFromXmlForLike(strHideType, "OU_USERS"));

			return string.Format(strB.ToString(), strRankLimit);
		}

		/// <summary>
		/// �γɸ���ָ��������ѯ����Ա����SQL���
		/// </summary>
		/// <param name="strOrgValues">ָ���ķ�Χ������ʶ�����֮�����","�ָ���</param>
		/// <param name="soc">�û�����������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strLikeName">����Ա���ϵ�����ģ��ƥ�����</param>
		/// <param name="bLike">�Ƿ����ģ��ƥ��</param> 
		/// <param name="strAttr">Ҫ���ȡ���ֶ�����</param>
		/// <param name="iListObjType">�����Ƿ��ѯ��ְ����</param>
		/// <param name="iLod">��ѯɾ���Ķ������</param>
		/// <param name="iDep">Ҫ���ѯ�����</param>
		/// <param name="strHideType">Ҫ�����ε���������</param>
		/// <param name="rootPath">�����ORIGINAL_SORT</param>
		/// <returns>�γɸ���ָ��������ѯ����Ա����SQL���</returns>
		/// 
		//2009-05-06 ɾ�� RANK_DEFINE Լ��������ORIGINAL_SORTԼ��
		private static string QueryUsersByCondition2(string strOrgValues,
			SearchObjectColumn soc,
			string strLikeName,
			bool bLike,
			string strAttr,
			int iListObjType,
			ListObjectDelete iLod,
			int iDep,
			string strHideType,
			string rootPath)
		{
			string strColName = OGUCommonDefine.GetSearchObjectColumn(soc);

			StringBuilder strB = new StringBuilder(1024);
			strB.Append(@"	SELECT 'USERS' AS OBJECTCLASS, "
							+ DatabaseSchema.Instence.GetTableColumns(strAttr, "USERS", "OU_USERS") + @"
							FROM USERS , OU_USERS
							WHERE USERS.GUID = OU_USERS.USER_GUID 
								AND ( " + GetSqlSearchParOriginal2("OU_USERS", iDep, rootPath) + " )");

			string strListDelete = GetSqlSearchStatus("OU_USERS", iLod);

			if (strListDelete.Length > 0)
				strB.Append(" \n AND ( " + strListDelete + " ) ");

			if ((iListObjType & (int)ListObjectType.SIDELINE) == 0)
				strB.Append(" \n AND OU_USERS.SIDELINE = 0 ");

			if (strLikeName != SearchAllTerm)
				strB.Append(string.Format(@" AND (OU_USERS.SEARCH_NAME  like '%{0}%')", strLikeName));
			else
				strB.Append(BuildSearchCondition("OU_USERS.SEARCH_NAME", strLikeName, bLike));

			//strB.Append(BuildSearchCondition("OU_USERS.SEARCH_NAME", strLikeName, bLike));

			if (strHideType.Length > 0)
				strB.Append(GetHideTypeFromXmlForLike(strHideType, "OU_USERS"));

			return strB.ToString();
		}

		private static string BuildSearchCondition(string columnName, string searchTerm, bool bLike)
		{
			string result = string.Empty;

			if (string.Compare(searchTerm, SearchAllTerm, true) != 0)
			{
				if (bLike)
				{
					if (AccreditSection.GetConfig().AccreditSettings.FuzzySearch)
						searchTerm = string.Format("\"*{0}*\"", searchTerm);
					else
						searchTerm = string.Format("\"{0}\"", searchTerm);
				}

				result = string.Format(" AND (CONTAINS({0}, {1}))", columnName, TSqlBuilder.Instance.CheckQuotationMark(searchTerm, true));
			}

			return result;
		}

		/// <summary>
		/// ͨ�������ڲ�ѯ�����ε�ָ��Ҫ�����ε����ݶ����չ��
		/// </summary>
		/// <param name="strHideType">��Ӧ�������ļ��е��������ƣ����֮�����","�ָ���</param>
		/// <param name="strObjClass">Ҫ�����ε����ݶ�������</param>
		/// <returns>�����γ�SQL����е�������䣨Ҫ�����ε����ݶ���</returns>
		private static string GetHideTypeFromXml(string strHideType, string strObjClass)
		{
			StringBuilder strB = new StringBuilder(1024);

			XmlDocument xmlDoc = GetMaskObjectDocument();// (new SysConfig()).GetConfigXmlDocument("MaskObjects");

			if (xmlDoc != null)
			{
				string[] strArrs = strHideType.Split(',', ' ');

				for (int i = 0; i < strArrs.Length; i++)
				{
					XmlNode hideNode = xmlDoc.DocumentElement.SelectSingleNode("HideType[@name=\"" + strArrs[i] + "\"]");
					ExceptionHelper.TrueThrow(hideNode == null, "�������ļ����Ҳ���Ҫ�����ε��������ã�");

					foreach (XmlElement elem in hideNode.SelectNodes(strObjClass))
					{
						if (strB.Length > 0)
							strB.Append(",");
						string strValue = elem.GetAttribute("ALL_PATH_NAME");

						ExceptionHelper.TrueThrow(strValue.Length == 0, "���������ļ��е����ò���ȷ��");
						strB.Append(TSqlBuilder.Instance.CheckQuotationMark(strValue, true));
					}
				}
			}

			return strB.ToString();
		}

		private static XmlDocument GetMaskObjectDocument()
		{
			if (OGUReader.MaskObjectDocument == null)
			{
				string filePath = AccreditSection.GetConfig().AccreditSettings.MaskObjects;

				bool IsFileExist = true;
				if (false == File.Exists(filePath))
				{
					if (HttpContext.Current != null)
						filePath = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath) + filePath;
					else
						filePath = AppDomain.CurrentDomain.BaseDirectory + filePath;

					IsFileExist = File.Exists(filePath);
				}

				if (IsFileExist)
				{
					OGUReader.MaskObjectDocument = XmlHelper.LoadDocument(filePath);
				}
			}

			return OGUReader.MaskObjectDocument;
		}


		/// <summary>
		/// ͨ�������ڲ�ѯ�����ε�ָ��Ҫ�����ε����ݶ����չ��
		/// </summary>
		/// <param name="strHideType">��Ӧ�������ļ��е��������ƣ����֮�����","�ָ���</param>
		/// <param name="strObjClass">Ҫ�����ε����ݶ�������</param>
		/// <returns>ͨ�������ڲ�ѯ�����ε�ָ��Ҫ�����ε����ݶ����չ��</returns>
		private static string GetHideTypeFromXmlForLike(string strHideType, string strObjClass)
		{
			StringBuilder strB = new StringBuilder(1024);

			XmlDocument xmlDoc = GetMaskObjectDocument();// (new SysConfig()).GetConfigXmlDocument("MaskObjects");

			if (xmlDoc != null)
			{
				string[] strArrs = strHideType.Split(',', ' ');

				for (int i = 0; i < strArrs.Length; i++)
				{
					XmlNode hideNode = xmlDoc.DocumentElement.SelectSingleNode("HideType[@name=\"" + strArrs[i] + "\"]");
					ExceptionHelper.TrueThrow(hideNode == null, "�������ļ����Ҳ���Ҫ�����ε��������ã�");

					foreach (XmlElement elem in hideNode.SelectNodes("ORGANIZATIONS"))
					{
						string strValue = elem.GetAttribute("ALL_PATH_NAME");

						ExceptionHelper.TrueThrow(strValue.Length == 0, "���������ļ��е����ò���ȷ��");
						strB.Append(" AND " + strObjClass + ".ALL_PATH_NAME NOT LIKE " + TSqlBuilder.Instance.CheckQuotationMark(strValue, true) + " + '%' ");
					}

					if (strObjClass == "GROUPS" || strObjClass == "OU_USERS")
					{
						foreach (XmlElement elem in hideNode.SelectNodes(strObjClass))
						{
							string strValue = elem.GetAttribute("ALL_PATH_NAME");

							ExceptionHelper.TrueThrow(strValue.Length == 0, "���������ļ��е����ò���ȷ��");
							strB.Append(" AND " + strObjClass + ".ALL_PATH_NAME <> " + TSqlBuilder.Instance.CheckQuotationMark(strValue, true) + " ");
						}
					}
				}
			}

			return strB.ToString();
		}

		/// <summary>
		/// ��HashTable�е�����ת�����ַ����У�����SQL��ѯʹ�ã�����ǰ�󶼵�Ҫ�ӵ����ţ����Ҳ��á�,���ָ���
		/// </summary>
		/// <param name="hash">Ҫ��ת����HashTable</param>
		/// <returns>ת���Ժ���ַ���</returns>
		private static string TransHashToSqlString(SortedList<string, string> hash)
		{
			StringBuilder strB = new StringBuilder(1024);
			foreach (KeyValuePair<string, string> dict in hash)
			{
				if (strB.Length > 0)
					strB.Append(", ");
				strB.Append(TSqlBuilder.Instance.CheckQuotationMark(dict.Value.ToString(), true));
			}
			return strB.ToString();
		}

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

		//                DataRow[] drs = table.Select("CNAME=" + TSqlBuilder.Instance.CheckQuotationMark(strAttrArr[i]));
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

		/// <summary>
		/// ����ָ���Ĳ�ѯ�����࣬����ϵͳ�ж��ڡ��������Ĳ�ѯSQL���
		/// </summary>
		/// <param name="scc">��ѯ���������</param>
		/// <param name="rootPath">�����ORIGINAL_SORT</param>
		/// <returns>����ڡ��������Ĳ�ѯSQL���</returns>
		/// 
		//2009-05-11
		private static string GetOrganizationsSqlByScc(SearchOrgChildrenCondition scc, string rootPath)
		{
			StringBuilder strB = new StringBuilder(1024);
			string strRootGuids = TransHashToSqlString(scc.RootGuids);

			strB.Append(@"	SELECT 'ORGANIZATIONS' AS OBJECTCLASS, "
				+ DatabaseSchema.Instence.GetTableColumns(scc.ObjAttrs, "ORGANIZATIONS", "RANK_DEFINE") + @"
							FROM ORGANIZATIONS JOIN RANK_DEFINE 
								ON ORGANIZATIONS.RANK_CODE = RANK_DEFINE.CODE_NAME
									{1} 
							WHERE(	GUID IN(" + strRootGuids + @") 
									OR	
									(
											( " + GetSqlSearchParOriginal2("ORGANIZATIONS", scc.Depth, rootPath) + @" ) 
											{0}
									)
								)");

			string strListDelete = GetSqlSearchStatus("ORGANIZATIONS", scc.ListObjDelete);
			if (strListDelete.Length > 0)
				strListDelete = " AND (" + strListDelete + ")";

			string strRankLimit = string.Empty;
			if (scc.OrgRankCN.Length > 0)
				strRankLimit = " AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
					+ TSqlBuilder.Instance.CheckQuotationMark(scc.OrgRankCN, true) + " ) ";

			if (scc.HideType != string.Empty)
			{
				string strHideType = GetHideTypeFromXml(scc.HideType, "ORGANIZATIONS");
				if (strHideType.Length > 0)
					strB.Append(" \n AND ORGANIZATIONS.ALL_PATH_NAME NOT IN ( " + strHideType + " ) ");
			}

			if (scc.OrgClass != 1024 * 1024 - 1)
				strB.Append(" \n AND (ORGANIZATIONS.ORG_CLASS = 0 OR ORGANIZATIONS.ORG_CLASS & " + scc.OrgClass + " <> 0 ) ");

			if (scc.OrgType != 1024 * 1024 - 1)
				strB.Append(" \n AND (ORGANIZATIONS.ORG_TYPE = 0 OR ORGANIZATIONS.ORG_TYPE & " + scc.OrgType + " <> 0 ) ");

			return string.Format(strB.ToString(), strListDelete, strRankLimit);
		}

		//2009-05-07ɾ��RANK_DEFINEԼ�����޸�ORIGINAL_SORTԼ��
		private static string GetOrganizationsSqlByScc2(SearchOrgChildrenCondition scc, string rootPath)
		{
			StringBuilder strB = new StringBuilder(1024);
			string strRootGuids = TransHashToSqlString(scc.RootGuids);

			strB.Append(@"	SELECT 'ORGANIZATIONS' AS OBJECTCLASS, "
				+ DatabaseSchema.Instence.GetTableColumns(scc.ObjAttrs, "ORGANIZATIONS") + @"
							FROM ORGANIZATIONS							
							WHERE(	GUID IN(" + strRootGuids + @") 
									OR	
									(
											( " + GetSqlSearchParOriginal2("ORGANIZATIONS", scc.Depth, rootPath) + @" ) 
											{0}
									)
								)");

			string strListDelete = GetSqlSearchStatus("ORGANIZATIONS", scc.ListObjDelete);
			if (strListDelete.Length > 0)
				strListDelete = " AND (" + strListDelete + ")";

			if (scc.HideType != string.Empty)
			{
				string strHideType = GetHideTypeFromXml(scc.HideType, "ORGANIZATIONS");
				if (strHideType.Length > 0)
					strB.Append(" \n AND ORGANIZATIONS.ALL_PATH_NAME NOT IN ( " + strHideType + " ) ");
			}

			if (scc.OrgClass != 1024 * 1024 - 1)
				strB.Append(" \n AND (ORGANIZATIONS.ORG_CLASS = 0 OR ORGANIZATIONS.ORG_CLASS & " + scc.OrgClass + " <> 0 ) ");

			if (scc.OrgType != 1024 * 1024 - 1)
				strB.Append(" \n AND (ORGANIZATIONS.ORG_TYPE = 0 OR ORGANIZATIONS.ORG_TYPE & " + scc.OrgType + " <> 0 ) ");

			return string.Format(strB.ToString(), strListDelete);
		}

		/// <summary>
		/// ����ָ���Ĳ�ѯ�����࣬����ϵͳ�ж��ڡ���Ա�顱�Ĳ�ѯSQL���
		/// </summary>
		/// <param name="scc">��ѯ���������</param>
		/// <param name="rootPath">�����ORIGINAL_SORT</param>
		/// <returns>����ڡ���Ա�顱�Ĳ�ѯSQL���</returns>
		/// 
		//2009-05-11
		private static string GetGroupsSqlByScc(SearchOrgChildrenCondition scc, string rootPath)
		{
			StringBuilder strB = new StringBuilder(1024);
			strB.Append(@" SELECT 'GROUPS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(scc.ObjAttrs, "GROUPS") + @"
							FROM GROUPS 
							WHERE ( " + GetSqlSearchParOriginal2("GROUPS", scc.Depth, rootPath) + " )");

			string strListDelete = GetSqlSearchStatus("GROUPS", scc.ListObjDelete);
			if (strListDelete.Length > 0)
				strB.Append(" \n AND ( " + strListDelete + " ) ");

			if (scc.HideType != string.Empty)
			{
				string strHideType = GetHideTypeFromXml(scc.HideType, "GROUPS");
				if (strHideType.Length > 0)
					strB.Append(" \n AND GROUPS.ALL_PATH_NAME NOT IN ( " + strHideType + " ) ");
			}

			return strB.ToString();
		}

		/// <summary>
		/// ����ָ���Ĳ�ѯ�����࣬����ϵͳ�ж��ڡ���Ա�顱�Ĳ�ѯSQL���
		/// </summary>
		/// <param name="scc">��ѯ���������</param>
		/// <param name="rootPath">�����ORIGINAL_SORT</param>
		/// <returns>����ڡ���Ա�顱�Ĳ�ѯSQL���</returns>
		/// 
		//2009-05-07ɾ��RANK_DEFINEԼ�����޸�ORIGINAL_SORTԼ��
		private static string GetGroupsSqlByScc2(SearchOrgChildrenCondition scc, string rootPath)
		{
			StringBuilder strB = new StringBuilder(1024);
			strB.Append(@" SELECT 'GROUPS' AS OBJECTCLASS, " + DatabaseSchema.Instence.GetTableColumns(scc.ObjAttrs, "GROUPS") + @"
							FROM GROUPS 
							WHERE ( " + GetSqlSearchParOriginal2("GROUPS", scc.Depth, rootPath) + " )");

			string strListDelete = GetSqlSearchStatus("GROUPS", scc.ListObjDelete);
			if (strListDelete.Length > 0)
				strB.Append(" \n AND ( " + strListDelete + " ) ");

			if (scc.HideType != string.Empty)
			{
				string strHideType = GetHideTypeFromXml(scc.HideType, "GROUPS");
				if (strHideType.Length > 0)
					strB.Append(" \n AND GROUPS.ALL_PATH_NAME NOT IN ( " + strHideType + " ) ");
			}

			return strB.ToString();
		}

		/// <summary>
		/// ����ָ���Ĳ�ѯ�����࣬����ϵͳ�ж��ڡ���Ա���Ĳ�ѯSQL���
		/// </summary>
		/// <param name="scc">��ѯ���������</param>
		/// <param name="rootPath">�����ORIGINAL_SORT</param>
		/// <returns>����ڡ���Ա���Ĳ�ѯSQL��</returns>
		/// 
		//2009-05-11
		private static string GetUsersSqlByScc(SearchOrgChildrenCondition scc, string rootPath)
		{
			StringBuilder strB = new StringBuilder(1024);
			strB.Append(@"	SELECT 'USERS' AS OBJECTCLASS, "
				+ DatabaseSchema.Instence.GetTableColumns(scc.ObjAttrs, "USERS", "OU_USERS", "RANK_DEFINE") + @"
							FROM USERS JOIN RANK_DEFINE 
								ON RANK_DEFINE.CODE_NAME = USERS.RANK_CODE
									{0}, OU_USERS 
							WHERE USERS.GUID = OU_USERS.USER_GUID 
								AND ( " + GetSqlSearchParOriginal2("OU_USERS", scc.Depth, rootPath) + " )");

			string strListDelete = GetSqlSearchStatus("OU_USERS", scc.ListObjDelete);
			if (strListDelete.Length > 0)
				strB.Append(" \n AND ( " + strListDelete + " ) ");

			string strRankLimit = string.Empty;
			if (scc.UserRankCN.Length > 0)
				strRankLimit = " AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = " + TSqlBuilder.Instance.CheckQuotationMark(scc.UserRankCN, true) + " ) ";

			if (scc.HideType != string.Empty)
			{
				string strHideType = GetHideTypeFromXml(scc.HideType, "OU_USERS");
				if (strHideType.Length > 0)
					strB.Append(" \n AND OU_USERS.ALL_PATH_NAME NOT IN ( " + strHideType + " ) ");
			}
			if ((scc.ListObjType & ListObjectType.SIDELINE) == 0)
				strB.Append(" \n AND OU_USERS.SIDELINE = 0 ");

			return string.Format(strB.ToString(), strRankLimit);
		}

		/// <summary>
		/// ����ָ���Ĳ�ѯ�����࣬����ϵͳ�ж��ڡ���Ա���Ĳ�ѯSQL���
		/// </summary>
		/// <param name="scc">��ѯ���������</param>
		/// <param name="rootPath">�����ORIGINAL_SORT</param>
		/// <returns>����ڡ���Ա���Ĳ�ѯSQL��</returns>
		/// 
		//2009-05-07ɾ��RANK_DEFINEԼ�����޸�ORIGINAL_SORTԼ��
		private static string GetUsersSqlByScc2(SearchOrgChildrenCondition scc, string rootPath)
		{
			StringBuilder strB = new StringBuilder(1024);
			strB.Append(@"	SELECT 'USERS' AS OBJECTCLASS, "
				+ DatabaseSchema.Instence.GetTableColumns(scc.ObjAttrs, "USERS", "OU_USERS") + @"
							FROM USERS , OU_USERS
							WHERE USERS.GUID = OU_USERS.USER_GUID 
								AND ( " + GetSqlSearchParOriginal2("OU_USERS", scc.Depth, rootPath) + " )");

			string strListDelete = GetSqlSearchStatus("OU_USERS", scc.ListObjDelete);
			if (strListDelete.Length > 0)
				strB.Append(" \n AND ( " + strListDelete + " ) ");

			if (scc.HideType != string.Empty)
			{
				string strHideType = GetHideTypeFromXml(scc.HideType, "OU_USERS");
				if (strHideType.Length > 0)
					strB.Append(" \n AND OU_USERS.ALL_PATH_NAME NOT IN ( " + strHideType + " ) ");
			}
			if ((scc.ListObjType & ListObjectType.SIDELINE) == 0)
				strB.Append(" \n AND OU_USERS.SIDELINE = 0 ");

			return strB.ToString();
		}

		/// <summary>
		/// ����Ҫ���ѯ�������Ƿ�չ�ֱ����߼���ɾ���Ķ����������ݲ�ѯ����
		/// </summary>
		/// <param name="strTableName">Ҫ���ѯ�����ݱ�</param>
		/// <param name="lod">Ҫ��չ�ֵ�����ɾ�����ͣ�Ĭ�϶�������ɾ���ϵ���ͨ����</param>
		/// <returns>SQL��������ɾ����ص�</returns>
		private static string GetSqlSearchStatus(string strTableName, ListObjectDelete lod)
		{
			string strListDelete = string.Empty;
			if ((lod & ListObjectDelete.COMMON) != ListObjectDelete.None)
				strListDelete = " (" + strTableName + ".STATUS = " + ((int)ListObjectDelete.COMMON).ToString() + ") ";
			if ((lod & ListObjectDelete.DIRECT_LOGIC) != ListObjectDelete.None)
			{
				if (strListDelete.Length > 0)
					strListDelete += " OR ";
				strListDelete += " (" + strTableName + ".STATUS & " + ((int)ListObjectDelete.DIRECT_LOGIC).ToString() + ") <> 0 ";
			}
			if ((lod & ListObjectDelete.CONJUNCT_ORG_LOGIC) != ListObjectDelete.None)
			{
				if (strListDelete.Length > 0)
					strListDelete += " OR ";
				strListDelete += " (" + strTableName + ".STATUS & " + ((int)ListObjectDelete.CONJUNCT_ORG_LOGIC).ToString() + ") <> 0 ";
			}
			if ((lod & ListObjectDelete.CONJUNCT_USER_LOGIC) != ListObjectDelete.None)
			{
				if (strListDelete.Length > 0)
					strListDelete += " OR ";
				strListDelete += " (" + strTableName + ".STATUS & " + ((int)ListObjectDelete.CONJUNCT_USER_LOGIC).ToString() + ") <> 0 ";
			}
			return strListDelete;
		}

		/// <summary>
		/// ���ö��ڲ�ѯ���漰�Ĳ�ѯ��ȵ����ݴ���
		/// </summary>
		/// <param name="strTableName">Ҫ���ѯ�����ݱ�</param>
		/// <param name="iDep">Ҫ���ѯ����ȣ�0��ʾ������ȣ�</param>
		/// <returns>SQL���������ص����ݲ�ѯ����</returns>
		private static string GetSqlSearchParOriginal(string strTableName, int iDep)
		{
			string strOriginalSort = string.Empty;
			if (iDep == 0)
				strOriginalSort = " " + strTableName + ".ORIGINAL_SORT LIKE RootOrg.ORIGINAL_SORT + '%'";
			else
			{
				strOriginalSort = " " + strTableName + ".ORIGINAL_SORT = RootOrg.ORIGINAL_SORT ";
				string strDepth = string.Empty;
				for (int i = 0; i < iDep; i++)
				{
					strDepth += "______";
					strOriginalSort += " OR " + strTableName + ".ORIGINAL_SORT LIKE RootOrg.ORIGINAL_SORT + "
						+ TSqlBuilder.Instance.CheckQuotationMark(strDepth, true);
				}
			}
			return strOriginalSort;
		}

		//2009-05-06
		private static string GetSqlSearchParOriginal2(string strTableName, int iDep, string rootPath)
		{
			string strOriginalSort = string.Empty;
			if (iDep == 0)
				strOriginalSort = " " + strTableName + ".ORIGINAL_SORT LIKE " + TSqlBuilder.Instance.CheckQuotationMark(rootPath + "%", true);
			else
			{
				strOriginalSort = " " + strTableName + ".ORIGINAL_SORT = " + TSqlBuilder.Instance.CheckQuotationMark(rootPath, true);

				string strDepth = string.Empty;
				for (int i = 0; i < iDep; i++)
				{
					strDepth += "______";
					strOriginalSort += " OR " + strTableName + ".ORIGINAL_SORT LIKE " + TSqlBuilder.Instance.CheckQuotationMark(rootPath, true) + " + "
						+ TSqlBuilder.Instance.CheckQuotationMark(strDepth, true);
				}
			}
			return strOriginalSort;
		}

		/// <summary>
		/// ���ݲ�ѯ����ж��Ƿ���ϲ�ѯ���
		/// </summary>
		/// <param name="strB"></param>
		/// <param name="bFitAll"></param>
		/// <returns></returns>
		private static bool CheckIsUserInOrganizations(StringBuilder strB, bool bFitAll)
		{
			bool bResult = false;
			if (strB.Length > 0)
			{
				DataSet ds;
				using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
				{
					Database database = DatabaseFactory.Create(context);

					ds = database.ExecuteDataSet(CommandType.Text, strB.ToString());
				}
				if (bFitAll)
				{
					bResult = true;
					foreach (DataTable table in ds.Tables)
					{
						if (table.Rows.Count <= 0)
						{
							bResult = false;
							break;
						}
					}
				}
				else
				{
					bResult = false;
					foreach (DataTable table in ds.Tables)
					{
						if (table.Rows.Count > 0)
						{
							bResult = true;
							break;
						}
					}
				}
			}

			return bResult;
		}

		/// <summary>
		/// CheckUserInOrganizations�����л�ȡ�û�����������
		/// </summary>
		/// <param name="xmlDoc"></param>
		/// <param name="strUserColName"></param>
		/// <returns></returns>
		private static string GetUserLimitInCheckUserInOrganizations(XmlDocument xmlDoc, string strUserColName)
		{
			StringBuilder strB = new StringBuilder(512);

			XmlElement root = xmlDoc.DocumentElement;
			ExceptionHelper.TrueThrow(root.ChildNodes.Count == 0, "�Բ������������Ա�ṹ��Ϣ���ݽṹ�����������ݲ�������");
			foreach (XmlElement elem in root.ChildNodes)
			{
				if (strB.Length > 0)
					strB.Append(" OR ");
				strB.Append(" (" + DatabaseSchema.Instence.GetTableColumns(strUserColName, "USERS", "OU_USERS") + " = " + TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true));

				string strParentGuid = elem.GetAttribute("parentGuid");
				if (strParentGuid != null && strParentGuid.Length > 0)
					strB.Append(" AND OU_USERS.PARENT_GUID = " + TSqlBuilder.Instance.CheckQuotationMark(strParentGuid, true));

				strB.Append(" ) ");
			}

			return " ( " + strB.ToString() + " ) ";
		}

		//        /// <summary>
		//        /// 
		//        /// </summary>
		//        private static void InitHashTable()
		//        {
		//            if (_DataSet_Schema == null)
		//            {
		//                string[] strAllTables = { "ORGANIZATIONS", "GROUPS", "USERS", "OU_USERS", "RANK_DEFINE", "GROUP_USERS", "SECRETARIES" };
		//                StringBuilder strB = new StringBuilder(512);
		//                for (int i = 0; i < strAllTables.Length; i++)
		//                {
		//                    strB.Append(@"
		//						SELECT SYSOBJECTS.NAME AS TNAME, SYSCOLUMNS.NAME AS CNAME
		//						FROM SYSOBJECTS, SYSCOLUMNS
		//						WHERE SYSOBJECTS.ID = SYSCOLUMNS.ID
		//							AND SYSOBJECTS.NAME IN (" + TSqlBuilder.Instance.CheckQuotationMark(strAllTables[i]) + @")
		//						ORDER BY TNAME, SYSCOLUMNS.COLID;
		//						");
		//                }
		//                _DataSet_Schema = OGUCommonDefine.ExecuteDataset(strB.ToString(), da, strAllTables);
		//            }
		//        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlObjDoc"></param>
		/// <param name="strUserColName"></param>
		/// <param name="strObjColName"></param>
		/// <param name="strDirect"></param>
		/// <param name="strExtraWhere"></param>
		/// <returns></returns>
		private static StringBuilder GetSqlSearchForCheckUserInObjects(XmlDocument xmlObjDoc,
			string strUserColName,
			string strObjColName,
			string strDirect,
			string strExtraWhere)
		{
			StringBuilder strBuilder = new StringBuilder(1024);

			string strUserList = string.Empty, strOrganizationList = string.Empty, strGroupList = string.Empty;

			foreach (XmlElement elem in xmlObjDoc.DocumentElement.ChildNodes)
			{
				string strRankCode = elem.GetAttribute("rankCode");//������������������

				switch (elem.LocalName)
				{
					case "ORGANIZATIONS":
						if (strRankCode != null && strRankCode.Length > 0)//�С��������𡱵�����
						{
							strBuilder.Append(@"
							SELECT DISTINCT 'ORGANIZATIONS' AS OBJECTCLASS, " + TSqlBuilder.Instance.CheckQuotationMark(strRankCode, true) + " AS RANK_CODE, "
									+ DatabaseSchema.Instence.GetTableColumns(strUserColName, "USERS", "OU_USERS") + " AS USER_VALUE, "
									+ DatabaseSchema.Instence.GetTableColumns(strObjColName, "ORGANIZATIONS") + @" AS OBJ_VALUE
							FROM ORGANIZATIONS, OU_USERS, USERS JOIN RANK_DEFINE 
								ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
									AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
									+ TSqlBuilder.Instance.CheckQuotationMark(strRankCode, true) + @") 
							WHERE OU_USERS.USER_GUID = USERS.GUID
								AND OU_USERS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + "
								+ TSqlBuilder.Instance.CheckQuotationMark(strDirect, true) + @"
								" + strExtraWhere + @"
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "ORGANIZATIONS") + " = "
								+ TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true) + @"
							ORDER BY OBJ_VALUE;" + "\n");
						}
						else
						{
							if (elem.GetAttribute("oValue").Trim().Length > 0)
							{
								if (strOrganizationList.Length > 0)
									strOrganizationList += ",";
								strOrganizationList += TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true);
							}
						}
						break;
					case "GROUPS":
						if (strRankCode != null && strRankCode.Length > 0)
						{
							strBuilder.Append(@"
							SELECT DISTINCT 'GROUPS' AS OBJECTCLASS, "
								+ TSqlBuilder.Instance.CheckQuotationMark(strRankCode, true) + " AS RANK_CODE, "
								+ DatabaseSchema.Instence.GetTableColumns(strUserColName, "USERS", "OU_USERS") + " AS USER_VALUE, "
								+ DatabaseSchema.Instence.GetTableColumns(strObjColName, "GROUPS") + @" AS OBJ_VALUE
							FROM GROUPS, GROUP_USERS, OU_USERS, USERS JOIN RANK_DEFINE 
								ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
									AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
								+ TSqlBuilder.Instance.CheckQuotationMark(strRankCode, true) + @")
							WHERE GROUPS.GUID = GROUP_USERS.GROUP_GUID
								AND GROUP_USERS.USER_GUID = OU_USERS.USER_GUID
								AND GROUP_USERS.USER_PARENT_GUID = OU_USERS.PARENT_GUID
								AND OU_USERS.USER_GUID = USERS.GUID
								" + strExtraWhere + @"
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "GROUPS") + "="
								+ TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true) + @"
							ORDER BY OBJ_VALUE;" + "\n");
						}
						else
						{
							if (elem.GetAttribute("oValue").Trim().Length > 0)
							{
								if (strGroupList.Length > 0)
									strGroupList += ",";
								strGroupList += TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true);
							}
						}
						break;
					case "USERS":
						if (strRankCode != null && strRankCode.Length > 0)
						{
							strBuilder.Append(@"
							SELECT DISTINCT 'USERS' AS OBJECTCLASS, " + TSqlBuilder.Instance.CheckQuotationMark(strRankCode, true) + " AS RANK_CODE, "
								+ DatabaseSchema.Instence.GetTableColumns(strUserColName, "USERS", "OU_USERS") + " AS USER_VALUE, "
								+ DatabaseSchema.Instence.GetTableColumns(strObjColName, "USERS", "OU_USERS") + @" AS OBJ_VALUE
							FROM OU_USERS, USERS
							WHERE OU_USERS.USER_GUID = USERS.GUID
								" + strExtraWhere + @"
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "USERS", "OU_USERS") + "="
								+ TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true) + @"
								AND RANK_DEFINE.SORT_ID <= (SELECT SORT_ID FROM RANK_DEFINE WHERE CODE_NAME = "
								+ TSqlBuilder.Instance.CheckQuotationMark(strRankCode, true) + @")
							ORDER BY OBJ_VALUE;" + "\n");
						}
						else
						{
							if (elem.GetAttribute("oValue").Trim().Length > 0)
							{
								if (strUserList.Length > 0)
									strUserList += ",";
								strUserList += TSqlBuilder.Instance.CheckQuotationMark(elem.GetAttribute("oValue"), true);
							}
						}
						break;
					default: ExceptionHelper.TrueThrow(true, "�Բ���,ϵͳû�ж�Ӧ����" + elem.LocalName + "������Ӧ����");
						break;
				}
			}

			if (strOrganizationList.Length > 0)
			{
				strBuilder.Append(@"
							SELECT DISTINCT 'ORGANIZATIONS' AS OBJECTCLASS, NULL AS RANK_CODE, "
								+ DatabaseSchema.Instence.GetTableColumns(strUserColName, "USERS", "OU_USERS") + " AS USER_VALUE, "
								+ DatabaseSchema.Instence.GetTableColumns(strObjColName, "ORGANIZATIONS") + @" AS OBJ_VALUE
							FROM ORGANIZATIONS, OU_USERS, USERS JOIN RANK_DEFINE 
								ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
							WHERE OU_USERS.USER_GUID = USERS.GUID
								AND OU_USERS.ORIGINAL_SORT LIKE ORGANIZATIONS.ORIGINAL_SORT + "
								+ TSqlBuilder.Instance.CheckQuotationMark(strDirect, true) + @"
								" + strExtraWhere + @"
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "ORGANIZATIONS")
									  + " IN ( " + strOrganizationList + @" )
							ORDER BY OBJ_VALUE;" + "\n");
			}

			if (strGroupList.Length > 0)
			{
				strBuilder.Append(@"
							SELECT DISTINCT 'GROUPS' AS OBJECTCLASS, NULL AS RANK_CODE, "
								+ DatabaseSchema.Instence.GetTableColumns(strUserColName, "USERS", "OU_USERS") + " AS USER_VALUE, "
								+ DatabaseSchema.Instence.GetTableColumns(strObjColName, "GROUPS") + @" AS OBJ_VALUE
							FROM GROUPS, GROUP_USERS, OU_USERS, USERS JOIN RANK_DEFINE 
								ON USERS.RANK_CODE = RANK_DEFINE.CODE_NAME
							WHERE GROUPS.GUID = GROUP_USERS.GROUP_GUID
								AND GROUP_USERS.USER_GUID = OU_USERS.USER_GUID
								AND GROUP_USERS.USER_PARENT_GUID = OU_USERS.PARENT_GUID
								AND OU_USERS.USER_GUID = USERS.GUID
								" + strExtraWhere + @"
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "GROUPS") + " IN ( " + strGroupList + @" )
							ORDER BY OBJ_VALUE;" + "\n");
			}

			if (strUserList.Length > 0)
			{
				strBuilder.Append(@"
							SELECT DISTINCT 'USERS' AS OBJECTCLASS, NULL AS RANK_CODE, "
								+ DatabaseSchema.Instence.GetTableColumns(strUserColName, "USERS", "OU_USERS") + " AS USER_VALUE, "
								+ DatabaseSchema.Instence.GetTableColumns(strObjColName, "USERS", "OU_USERS") + @" AS OBJ_VALUE
							FROM OU_USERS, USERS
							WHERE OU_USERS.USER_GUID = USERS.GUID
								" + strExtraWhere + @"
								AND " + DatabaseSchema.Instence.GetTableColumns(strObjColName, "USERS", "OU_USERS")
									  + " IN ( " + strUserList + @" )
							ORDER BY OBJ_VALUE;" + "\n");
			}

			return strBuilder;
		}
		#endregion
	}
}

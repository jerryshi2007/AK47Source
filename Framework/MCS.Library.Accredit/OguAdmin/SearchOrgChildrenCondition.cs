using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using MCS.Library.Data;
using MCS.Library.Core;
using MCS.Library.Accredit.Properties;
using MCS.Library.Accredit.Configuration;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// ��ѯ�����Ӷ��󼯺����ݵ�������
	/// </summary>
	public class SearchOrgChildrenCondition
	{
		#region ���캯��
		/// <summary>
		/// ���캯�����������ݶ����ѯ������
		/// </summary>
		/// <param name="strRootGuids">ָ���Ļ�����ʶ�����֮����á�,���ָ���</param>
		/// <param name="strAttrs">Ҫ���ȡ������</param>
		public SearchOrgChildrenCondition(string strRootGuids, string strAttrs)
		{
			InitSearchOrgChildrenCondition(strRootGuids, strAttrs, true);
		}
		//public SearchOrgChildrenCondition(string strRootGuids, string strAttrs, DataAccess da)
		//{
		//    InitSearchOrgChildrenCondition(strRootGuids, strAttrs, da);
		//}

		/// <summary>
		/// ���캯�����������ݶ����ѯ������
		/// </summary>
		/// <param name="strRootValues">ָ���Ļ�����ʶ�����֮����á�,���ָ���</param>
		/// <param name="soc">��ѯҪ��Ĳ�ѯ������
		/// ���ǣգɣġ��գӣţңߣǣգɣġ��̣ϣǣϣΣߣΣ��ͣš��ϣңɣǣɣΣ��̣ߣӣϣңԡ��ǣ̣ϣ£��̣ߣӣϣңԡ����̣̣ߣУ��ԣȣߣΣ��ͣţ�</param>
		/// <param name="strAttrs">Ҫ���ȡ������</param>
		public SearchOrgChildrenCondition(string strRootValues, SearchObjectColumn soc, string strAttrs)
		{
			using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
			{
				string strRootGuids = string.Empty;
				if (strRootValues.Length > 0 && soc != SearchObjectColumn.SEARCH_GUID)
				{
					Database database = DatabaseFactory.Create(context);
					DataSet ds = OGUReader.GetObjectsDetail("ORGANIZATIONS",
						strRootValues,
						soc,
						string.Empty,
						SearchObjectColumn.SEARCH_NULL,
						string.Empty);

					foreach (DataRow row in ds.Tables[0].Rows)
					{
						if (strRootGuids.Length > 0)
							strRootGuids += ",";

						strRootGuids += row["GUID"].ToString();
					}
				}
				else
					strRootGuids = strRootValues;

				InitSearchOrgChildrenCondition(strRootGuids, strAttrs, string.IsNullOrEmpty(strRootValues) ? true : false);
			}
		}
		//public SearchOrgChildrenCondition(string strRootValues, SearchObjectColumn soc, string strAttrs, DataAccess da)
		//{
		//    string strRootGuids = string.Empty;
		//    if (strRootValues.Length > 0 && soc != SearchObjectColumn.SEARCH_GUID)
		//    {
		//			DataSet ds = OGUReader.GetObjectsDetail("ORGANIZATIONS", 
		//				strRootValues, 
		//				soc, 
		//				string.Empty, 
		//				SearchObjectColumn.SEARCH_NULL, 
		//				string.Empty, 
		//				da);
		//        foreach (DataRow row in ds.Tables[0].Rows)
		//        {
		//            if (strRootGuids.Length > 0)
		//                strRootGuids += ",";
		//            strRootGuids += XmlHelper.DBValueToString(row["GUID"]);
		//        }
		//    }
		//    else
		//        strRootGuids = strRootValues;

		//    InitSearchOrgChildrenCondition(strRootGuids, strAttrs, da);
		//}

		#endregion

		#region Private Function Define
		private void InitSearchOrgChildrenCondition(string strRootGuids, string strAttrs, bool isNeedDefault)
		{
			using (DbContext context = DbContext.GetContext(CommonResource.AccreditConnAlias))
			{
				Database database = DatabaseFactory.Create(context);
				if (0 == strRootGuids.Length && true == isNeedDefault)
					strRootGuids = OGUReader.GetRootDSE().Tables[0].Rows[0]["GUID"].ToString();

				string[] strRoots = strRootGuids.Split(',');
				_HashRoot = new SortedList<string, string>();// new Hashtable();
				for (int i = 0; i < strRoots.Length; i++)
					_HashRoot.Add(strRoots[i], strRoots[i]);

				strAttrs = OGUCommonDefine.CombinateAttr(strAttrs);
				_StrObjAttrs = strAttrs;
			}
		}

		#endregion

		#region Property Define
		private ListObjectType _ListObjectType = ListObjectType.ORGANIZATIONS | ListObjectType.USERS | ListObjectType.GROUPS;
		/// <summary>
		/// ��ѯ��Ҫ���ȡ�Ķ�����������
		/// </summary>
		public ListObjectType ListObjType
		{
			get
			{
				return _ListObjectType;
			}
			set
			{
				_ListObjectType = value;
			}
		}

		private ListObjectDelete _ListObjectDelete = ListObjectDelete.COMMON;
		/// <summary>
		/// ��ѯ��Ҫ���ȡ�Ķ����������ͣ�ɾ����ǣ�
		/// </summary>
		public ListObjectDelete ListObjDelete
		{
			get
			{
				return _ListObjectDelete;
			}
			set
			{
				_ListObjectDelete = value;
			}
		}

		private SortedList<string, string> _HashRoot = null;
		/// <summary>
		/// ��ѯҪ�����õ�Organization��Ӧ��GUID
		/// </summary>
		public SortedList<string, string> RootGuids
		{
			get
			{
				return _HashRoot;
			}
		}

		private int _IDepth = 0;
		/// <summary>
		/// ��ѯҪ������
		/// </summary>
		public int Depth
		{
			get
			{
				return _IDepth;
			}
			set
			{
				_IDepth = value;
			}
		}

		private string _StrOrgRankCN;
		/// <summary>
		/// ���Ż���Ҫ�����ͼ���
		/// </summary>
		public string OrgRankCN
		{
			get
			{
				return _StrOrgRankCN;
			}
			set
			{
				_StrOrgRankCN = value;
			}
		}

		private string _StrUserRankCN;
		/// <summary>
		/// ��ԱҪ�����ͼ���
		/// </summary>
		public string UserRankCN
		{
			get
			{
				return _StrUserRankCN;
			}
			set
			{
				_StrUserRankCN = value;
			}
		}

		private string _StrObjAttrs = string.Empty;
		/// <summary>
		/// ��ѯҪ���ȡ����������
		/// </summary>
		public string ObjAttrs
		{
			get
			{
				return _StrObjAttrs;
			}
		}

		private string _HideType = string.Empty;
		/// <summary>
		/// ��ѯ�����Ҫ�����ε����ݶ���
		/// </summary>
		public string HideType
		{
			get
			{
				if (_HideType.Length == 0)
					_HideType = AccreditSection.GetConfig().AccreditSettings.AutohideType;// (new SysConfig()).GetDataFromConfig("AutohideType", string.Empty);

				return _HideType;
			}
			set
			{
				_HideType = OGUCommonDefine.GetHideType(value);
			}
		}

		private int _OrgClass = 1024 * 1024 - 1;
		/// <summary>
		/// Ҫ��չ�ֻ���������(cgac\yuan_yong 20041030)
		/// </summary>
		public int OrgClass
		{
			get
			{
				if (_OrgClass == 0 || _OrgClass == 1024 * 1024 - 1)
					_OrgClass = 1024 * 1024 - 1;

				return _OrgClass;
			}
			set
			{
				_OrgClass = value;
			}
		}

		private int _OrgType = 1024 * 1024 - 1;
		/// <summary>
		/// Ҫ��չ�ֻ���������(cgac\yuan_yong 20041030)
		/// </summary>
		public int OrgType
		{
			get
			{
				if (_OrgType == 0 || _OrgType == 1024 * 1024 - 1)
					_OrgType = 1024 * 1024 - 1;

				return _OrgType;
			}
			set
			{
				_OrgType = value;
			}
		}

		#endregion

		#region Internal
		internal string GetHashString()
		{
			StringBuilder builder = new StringBuilder(1024);
			builder.Append(this.HideType);
			builder.Append(this.Depth);
			builder.Append(this.ListObjDelete);
			builder.Append(this.ListObjType);
			builder.Append(this.OrgClass);
			builder.Append(this.OrgType);
			builder.Append(this.ObjAttrs);
			builder.Append(this.OrgRankCN);
			builder.Append(this.UserRankCN);

			foreach (KeyValuePair<string, string> dict in this.RootGuids)
			{
				if (builder.Length > 0)
					builder.Append(", ");
				builder.Append(dict.Value.ToString());
			}

			return builder.ToString();
		}
		#endregion
	}
}

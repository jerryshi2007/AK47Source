using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// �û��������ϵ��
	/// </summary>
	public class OuUsers : Interfaces.IOuUsers
	{
		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="strOUGuid"></param>
		/// <param name="strUserGuid"></param>
		/// <param name="strUserDisplayName"></param>
		/// <param name="strUserObjName"></param>
		/// <param name="strInnerSort"></param>
		/// <param name="strOriginalSort"></param>
		/// <param name="strGlobalSort"></param>
		/// <param name="strAllPathName"></param>
		/// <param name="strSideline"></param>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		public OuUsers(string strOUGuid, string strUserGuid, string strUserDisplayName, string strUserObjName, string strInnerSort,
			string strOriginalSort, string strGlobalSort, string strAllPathName, string strSideline, object startTime, object endTime)
		{
			_StrOUGuid = strOUGuid;
			_StrUserGuid = strUserGuid;
			_StrUserDisplayName = strUserDisplayName;
			_StrUserObjName = strUserObjName;
			_StrInnerSort = strInnerSort;
			_StrOriginalSort = strOriginalSort;
			_StrGlobalSort = strGlobalSort;
			_StrAllPathName = strAllPathName;
			if (strSideline == "0")
				_BSideline = false;

			if (startTime is DBNull)
				_StartTime = System.DateTime.MinValue;
			else
				_StartTime = (DateTime)startTime;

			if (endTime is DBNull)
				_EndTime = DateTime.MaxValue;
			else
				_EndTime = (DateTime)endTime;
		}

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <param name="row"></param>
		public OuUsers(DataRow row)
		{
			_StrOUGuid = OGUCommonDefine.DBValueToString(row["PARENT_GUID"]);
			_StrUserGuid = OGUCommonDefine.DBValueToString(row["USER_GUID"]);
			_StrUserDisplayName = OGUCommonDefine.DBValueToString(row["DISPLAY_NAME"]);
			_StrUserObjName = OGUCommonDefine.DBValueToString(row["OBJ_NAME"]);
			_StrInnerSort = OGUCommonDefine.DBValueToString(row["INNER_SORT"]);
			_StrOriginalSort = OGUCommonDefine.DBValueToString(row["ORIGINAL_SORT"]);
			_StrGlobalSort = OGUCommonDefine.DBValueToString(row["GLOBAL_SORT"]);
			_StrAllPathName = OGUCommonDefine.DBValueToString(row["ALL_PATH_NAME"]);
			_StrUserDescription = OGUCommonDefine.DBValueToString(row["DESCRIPTION"]);

			if ((int)row["SIDELINE"] == 0)
				_BSideline = false;

			if (row["START_TIME"] is DBNull)
				_StartTime = System.DateTime.MinValue;
			else
				_StartTime = (DateTime)row["START_TIME"];

			if (row["END_TIME"] is DBNull)
				_EndTime = DateTime.MaxValue;
			else
				_EndTime = (DateTime)row["END_TIME"];
		}

		private string _StrUserGuid = string.Empty;
		/// <summary>
		/// ��ǰ�û�������ϵ���û���Ӧ�ı�ʶ
		/// </summary>
		public string UserGuid
		{
			get
			{
				return _StrUserGuid;
			}
		}

		private string _StrOUGuid = string.Empty;
		/// <summary>
		/// ��ǰ�û�������ϵ�л�����Ӧ�ı�ʶ
		/// </summary>
		public string OUGuid
		{
			get
			{
				return _StrOUGuid;
			}
		}

		private string _StrUserDisplayName = string.Empty;
		/// <summary>
		/// ��ǰ�û�������ϵ�и��û��ġ���ʾ���ơ�
		/// </summary>
		public string UserDisplayName
		{
			get
			{
				return _StrUserDisplayName;
			}
		}

		private string _StrUserObjName = string.Empty;
		/// <summary>
		/// ��ǰ�û�������ϵ���û��ġ��������ơ�������һ�������п��ܳ�����Ϊ����������ɵ����ݳ�ͻ��
		/// </summary>
		public string UserObjName
		{
			get
			{
				return _StrUserObjName;
			}
		}

		private string _StrInnerSort = string.Empty;
		/// <summary>
		/// ��ǰ�û�������ϵ���û��ڸû����е������ڲ�����
		/// </summary>
		public string InnerSort
		{
			get
			{
				return _StrInnerSort;
			}
		}

		private string _StrGlobalSort = string.Empty;
		/// <summary>
		/// ��ǰ�û�������ϵ���û���ȫ��ַ�����ڲ���ϵ�����
		/// </summary>
		public string GlobalSort
		{
			get
			{
				return _StrGlobalSort;
			}
		}

		private string _StrOriginalSort = string.Empty;
		/// <summary>
		/// ��ǰ�û�������ϵ���û���ȫ��ַ����ι�ϵ����������������
		/// </summary>
		public string OriginalSort
		{
			get
			{
				return _StrOriginalSort;
			}
		}

		private string _StrAllPathName = string.Empty;
		/// <summary>
		/// ��ǰ�û�������ϵ���û���ȫ��ַ��ʾ������������
		/// </summary>
		public string AllPathName
		{
			get
			{
				return _StrAllPathName;
			}
		}

		private string _StrUserDescription = string.Empty;
		/// <summary>
		/// ��ǰ�û��ĸ���������Ϣ
		/// </summary>
		public string UserDescription
		{
			get
			{
				return _StrUserDescription;
			}
		}
		private bool _BSideline = true;
		/// <summary>
		/// ��ǰ�û�������ϵ�Ƿ��ְ��ϵ����ְ��true����ְ��false��
		/// </summary>
		public bool Sideline
		{
			get
			{
				return _BSideline;
			}
		}

		private DateTime _StartTime;
		/// <summary>
		/// ��ǰ�û�������ϵ������ʱ��
		/// </summary>
		public DateTime StartTime
		{
			get
			{
				return _StartTime;
			}
		}

		private DateTime _EndTime;
		/// <summary>
		/// ��ǰ�û�������ϵ�Ľ���ʱ��
		/// </summary>
		public DateTime EndTime
		{
			get
			{
				return _EndTime;
			}
		}

		/// <summary>
		/// ��������ʱ��ͽ���ʱ���жϵ�ǰ�û�������ϵ�Ƿ���Ч
		/// </summary>
		public bool InUse
		{
			get
			{
				bool bResult = false;
				if (_StartTime < DateTime.Now && DateTime.Now < _EndTime)
					bResult = true;
				return bResult;
			}
		}
	}
}

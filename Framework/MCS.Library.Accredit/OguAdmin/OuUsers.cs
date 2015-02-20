using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace MCS.Library.Accredit.OguAdmin
{
	/// <summary>
	/// 用户与机构关系类
	/// </summary>
	public class OuUsers : Interfaces.IOuUsers
	{
		/// <summary>
		/// 构造函数
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
		/// 构造函数
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
		/// 当前用户机构关系中用户对应的标识
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
		/// 当前用户机构关系中机构对应的标识
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
		/// 当前用户机构关系中该用户的“显示名称”
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
		/// 当前用户机构关系中用户的“对象名称”（考虑一个部门中可能出现因为对象名称造成的数据冲突）
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
		/// 当前用户机构关系中用户在该机构中的排序（内部排序）
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
		/// 当前用户机构关系中用户的全地址（关于层次上的排序）
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
		/// 当前用户机构关系中用户的全地址（层次关系描述，不用于排序）
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
		/// 当前用户机构关系中用户的全地址表示（文字描述）
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
		/// 当前用户的附加描述信息
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
		/// 当前用户机构关系是否兼职关系（兼职：true；主职：false）
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
		/// 当前用户机构关系的启用时间
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
		/// 当前用户机构关系的结束时间
		/// </summary>
		public DateTime EndTime
		{
			get
			{
				return _EndTime;
			}
		}

		/// <summary>
		/// 根据启用时间和结束时间判断当前用户机构关系是否生效
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

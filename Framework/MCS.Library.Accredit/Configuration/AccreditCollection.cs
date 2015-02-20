using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Library.Accredit.Configuration
{
	/// <summary>
	/// 配置信息集定义
	/// </summary>
	public sealed class AccreditCollection : NamedConfigurationElementCollection<AccreditElement>
	{
		/// <summary>
		/// 构造函数定义
		/// </summary>
		public AccreditCollection()
		{ }
		/// <summary>
		/// 自动屏蔽功能实现节点内容
		/// </summary>
		public string AutohideType
		{
			get
			{
				AccreditElement result = (AccreditElement)BaseGet("autohideType");

				return result == null ? string.Empty : result.Description;
			}
		}

		/// <summary>
		/// 是否模糊查询（全文检索的时候使用*search term *）
		/// </summary>
		public bool FuzzySearch
		{
			get
			{
				AccreditElement result = (AccreditElement)BaseGet("fuzzySearch");

				return result == null ? true : bool.Parse(result.Description);
			}
		}

		/// <summary>
		/// 机构人员的根部门设定【默认为“中国海关”】
		/// </summary>
		public string OguRootName
		{
			get
			{
				AccreditElement result = (AccreditElement)BaseGet("oguRootName");

				string temp = result == null ? "中国海关" : result.Description;

				return string.IsNullOrEmpty(temp) ? "中国海关" : temp;
			}
		}
		/// <summary>
		/// 自动屏蔽对应设置的文件
		/// </summary>
		public string MaskObjects
		{
			get
			{
				AccreditElement result = (AccreditElement)BaseGet("maskObjects");

				return result == null ? string.Empty : result.Description;
			}
		}
		/// <summary>
		/// 模拟帐户的设置文件实现
		/// </summary>
		public string ImpersonateUser
		{
			get
			{
				AccreditElement result = (AccreditElement)BaseGet("impersonateUser");

				return result == null ? string.Empty : result.Description;
			}
		}
		/// <summary>
		/// 本级部门的设定级别
		/// </summary>
		public string CurDepartLevel
		{
			get
			{
				AccreditElement result = (AccreditElement)BaseGet("curDepartLevel");

				return result == null ? string.Empty : result.Description;
			}
		}

		/// <summary>
		/// 系统是否使用授权信息
		/// </summary>
		/// <remarks>
		/// false:不使用授权管理控制
		/// true：受授权管理系统权限控制【默认】
		/// </remarks>
		public bool CustomsAuthentication
		{
			get
			{
				AccreditElement result = (AccreditElement)BaseGet("customsAuthentication");

				return result == null ? true : bool.Parse(result.Description);
			}
		}

		/// <summary>
		/// 用户的角色和用户的父部门相关（移动部门后，用户角色失效）
		/// </summary>
		public bool RoleRelatedUserParentDept
		{
			get
			{
				AccreditElement result = (AccreditElement)BaseGet("roleRelatedUserParentDept");

				return result == null ? true : bool.Parse(result.Description);
			}
		}

		/// <summary>
		/// 人员组中用户展现界面每一页展现的数据条数
		/// </summary>
		public int GroupUsersPageSize
		{
			get
			{
				AccreditElement result = (AccreditElement)BaseGet("groupUsersPageSize");
				int pageSize = result == null ? 0 : int.Parse(result.Description);
				return pageSize <= 0 ? 20 : pageSize;
			}
		}
		/// <summary>
		/// 系统数据缓存存在的slideTime，以分钟为单位，默认180分钟
		/// </summary>
		public int CacheSlideMinutes
		{
			get
			{
				AccreditElement result = (AccreditElement)BaseGet("cacheSlideMinutes");
				int minutes = result == null ? 180 : int.Parse(result.Description);
				return minutes <= 0 ? 180 : minutes;
			}
		}
		/// <summary>
		/// 授权界面展现的最大数集条数【默认为100】
		/// </summary>
		public int AppListMaxCount
		{
			get
			{
				AccreditElement result = (AccreditElement)BaseGet("appListMaxCount");
				int count = result == null ? 100 : int.Parse(result.Description);
				return count <= 0 ? 100 : count;
			}
		}
		/// <summary>
		/// Soap记录中是否记录输入，默认为false
		/// </summary>
		public bool SoapRecordInput
		{
			get
			{
				AccreditElement result = (AccreditElement)BaseGet("soapRecordInput");
				return result == null ? false : bool.Parse(result.Description);
			}
		}

		/// <summary>
		/// Soap记录中是否记录输出,默认为false
		/// </summary>
		public bool SoapRecordOutput
		{
			get
			{
				AccreditElement result = (AccreditElement)BaseGet("soapRecordOutput");
				return result == null ? false : bool.Parse(result.Description);
			}
		}
		/// <summary>
		/// 是否记录Soap信息
		/// </summary>
		public bool SoapRecord
		{
			get
			{
				AccreditElement result = (AccreditElement)BaseGet("soapRecord");
				return result == null ? false : bool.Parse(result.Description);
			}
		}

		/// <summary>
		/// 是否清除Local的Cache
		/// </summary>
		public bool ClearLocalCache
		{
			get
			{
				AccreditElement result = (AccreditElement)BaseGet("clearLocalCache");

				return result == null ? true : bool.Parse(result.Description);
			}
		}

		/// <summary>
		/// 是否通过UDP清除远程Cache
		/// </summary>
		public bool ClearRemoteCache
		{
			get
			{
				AccreditElement result = (AccreditElement)BaseGet("clearRemoteCache");

				return result == null ? true : bool.Parse(result.Description);
			}
		}
	}
}

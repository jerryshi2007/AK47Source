using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

using MCS.Library.Core;
using MCS.Library.Configuration;

namespace MCS.Library.Accredit.Configuration
{
	/// <summary>
	/// ������Ϣ������
	/// </summary>
	public sealed class AccreditCollection : NamedConfigurationElementCollection<AccreditElement>
	{
		/// <summary>
		/// ���캯������
		/// </summary>
		public AccreditCollection()
		{ }
		/// <summary>
		/// �Զ����ι���ʵ�ֽڵ�����
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
		/// �Ƿ�ģ����ѯ��ȫ�ļ�����ʱ��ʹ��*search term *��
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
		/// ������Ա�ĸ������趨��Ĭ��Ϊ���й����ء���
		/// </summary>
		public string OguRootName
		{
			get
			{
				AccreditElement result = (AccreditElement)BaseGet("oguRootName");

				string temp = result == null ? "�й�����" : result.Description;

				return string.IsNullOrEmpty(temp) ? "�й�����" : temp;
			}
		}
		/// <summary>
		/// �Զ����ζ�Ӧ���õ��ļ�
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
		/// ģ���ʻ��������ļ�ʵ��
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
		/// �������ŵ��趨����
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
		/// ϵͳ�Ƿ�ʹ����Ȩ��Ϣ
		/// </summary>
		/// <remarks>
		/// false:��ʹ����Ȩ�������
		/// true������Ȩ����ϵͳȨ�޿��ơ�Ĭ�ϡ�
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
		/// �û��Ľ�ɫ���û��ĸ�������أ��ƶ����ź��û���ɫʧЧ��
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
		/// ��Ա�����û�չ�ֽ���ÿһҳչ�ֵ���������
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
		/// ϵͳ���ݻ�����ڵ�slideTime���Է���Ϊ��λ��Ĭ��180����
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
		/// ��Ȩ����չ�ֵ��������������Ĭ��Ϊ100��
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
		/// Soap��¼���Ƿ��¼���룬Ĭ��Ϊfalse
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
		/// Soap��¼���Ƿ��¼���,Ĭ��Ϊfalse
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
		/// �Ƿ��¼Soap��Ϣ
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
		/// �Ƿ����Local��Cache
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
		/// �Ƿ�ͨ��UDP���Զ��Cache
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

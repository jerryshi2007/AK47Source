using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Properties;

namespace MCS.Library.Configuration
{
	/// <summary>
	/// ��Config�쳣�йص��쳣��������
	/// </summary>
	public static class ConfigurationExceptionHelper
	{
		/// <summary>
		/// ���section�Ƿ�Ϊ�գ����Ϊ�գ����׳��쳣
		/// </summary>
		/// <param name="section">section����</param>
		/// <param name="sectionName">section�����ƣ������쳣��Ϣ�г���</param>
		public static void CheckSectionNotNull(this ConfigurationSection section, string sectionName)
		{
			if (section == null)
				throw new ConfigurationErrorsException(string.Format(Resource.CanNotFoundConfigSection, sectionName));
		}

		/// <summary>
		/// ���section��source�Ƿ�Ϊ�գ����Ϊ�գ����׳��쳣�����������ִ��CheckSectionNotNull
		/// </summary>
		/// <param name="section">section����</param>
		/// <param name="sectionName">section�����ƣ������쳣��Ϣ�г���</param>
		public static void CheckSectionSource(this ConfigurationSection section, string sectionName)
		{
			CheckSectionNotNull(section, sectionName);

			if (section.ElementInformation.Source == null)
				throw new ConfigurationErrorsException(string.Format(Resource.CanNotFoundConfigSectionElement, sectionName));
		}
	}
}

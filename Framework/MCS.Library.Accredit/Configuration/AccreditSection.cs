using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

using MCS.Library.Configuration;
using MCS.Library.Core;

namespace MCS.Library.Accredit.Configuration
{
	/// <summary>
	/// ��Ȩƽ̨��ͳһ������Ϣ�ڶ���
	/// </summary>
	public sealed class AccreditSection : ConfigurationSection
	{
		/// <summary>
		/// ��ȡ������Ϣ����
		/// </summary>
		/// <returns></returns>
		public static AccreditSection GetConfig()
		{
			AccreditSection result = (AccreditSection)ConfigurationBroker.GetSection("accreditSection");

			ConfigurationExceptionHelper.CheckSectionNotNull(result, "accreditSection");

			return result;
		}

		/// <summary>
		/// ������Ϣ�������ݼ�
		/// </summary>
		[ConfigurationProperty("accreditSettings")]
		public AccreditCollection AccreditSettings
		{
			get
			{
				return (AccreditCollection)this["accreditSettings"];
			}
		}
	}
}

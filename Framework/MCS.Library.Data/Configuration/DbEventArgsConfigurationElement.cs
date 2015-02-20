#region Using
using System;
using System.Configuration;
#endregion
namespace MCS.Library.Data.Configuration
{
	/// <summary>
	/// �Զ������ݿ�ִ�й����¼�������������
	/// </summary>
	class DbEventArgsConfigurationElement : ConfigurationElement
	{
		[ConfigurationProperty("type", IsRequired = true)]
		public virtual string Type
		{
			get
			{
				return (string)this["type"];
			}
		}
	}
}

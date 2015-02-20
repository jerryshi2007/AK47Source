using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MCS.Library.Configuration
{
	/// <summary>
	/// 配置节的基类。允许不识别的属性存在
	/// </summary>
	public class DeluxeConfigurationSection : ConfigurationSection
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			base.OnDeserializeUnrecognizedAttribute(name, value);

			return true;
		}
	}
}

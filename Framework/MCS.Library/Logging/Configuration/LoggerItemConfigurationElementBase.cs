using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;

namespace MCS.Library.Logging
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class LoggerItemConfigurationElementBase : TypeConfigurationElement
	{
		private Dictionary<string, string> extendedAttributes = new Dictionary<string, string>();

		///// <summary>
		///// 
		///// </summary>
		//public Dictionary<string, string> ExtendedAttributes
		//{
		//    get
		//    {
		//        return this.extendedAttributes;
		//    }
		//}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			this.extendedAttributes[name] = value;

			return true;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Configuration;
using System.Configuration;

namespace MCS.Library.SOA.DocServiceClient
{
	public class ClientInfoConfigurationElement : NamedConfigurationElement
	{
		#region Private const
		private const string ServiceUrlItem = "serviceUrl";
		#endregion

		/// <summary>
		/// 用户编号
		/// </summary>
		[ConfigurationProperty(ClientInfoConfigurationElement.ServiceUrlItem)]
		public string ServiceUrl
		{
			get
			{
				return base[ClientInfoConfigurationElement.ServiceUrlItem] as string;
			}
		}
	}

	[ConfigurationCollection(typeof(ClientInfoConfigurationElement),
	CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public class ClientInfoConfigurationElementCollection : NamedConfigurationElementCollection<ClientInfoConfigurationElement>
	{
	}

}

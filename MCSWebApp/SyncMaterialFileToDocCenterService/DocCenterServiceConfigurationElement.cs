using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MCS.Library.Configuration;
using MCS.Library.SOA.DocServiceClient;

namespace SyncMaterialFileToDocCenterService
{
	public class DocCenterServiceConfigurationElement : NamedConfigurationElement
	{
		[ConfigurationProperty("documentLibraryName")]
		public string DocumentLibraryName
		{
			get { return base["documentLibraryName"] as string; }
		}

		[ConfigurationProperty("mossServerName")]
		public string MossServerName
		{
			get { return base["mossServerName"] as string; }
		}
	}

	[ConfigurationCollection(typeof(ClientInfoConfigurationElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public class DocCenterServiceConfigurationElementCollection : NamedConfigurationElementCollection<DocCenterServiceConfigurationElement>
	{
	}
}
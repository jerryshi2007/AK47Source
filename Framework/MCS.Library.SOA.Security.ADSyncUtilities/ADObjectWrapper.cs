using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using MCS.Library.OGUPermission;
using MCS.Library;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	public class ADObjectWrapper
	{
		private System.Collections.Specialized.HybridDictionary properties = null;
		private object propertiesSyncObj = new object();

		public ADSchemaType ObjectType { get; set; }
		public string DN
		{
			get
			{
				return this.Properties["distinguishedName"] as string;
			}
		}
		public string NativeGuid
		{
			get
			{
				return this.Properties["objectGUID"] as string;
			}
		}

		public IDictionary Properties
		{
			get
			{
				if (this.properties == null)
				{
					lock (this.propertiesSyncObj)
					{
						if (this.properties == null)
							this.properties = new System.Collections.Specialized.HybridDictionary();
					}
				}

				return this.properties;
			}
		}
	}
}

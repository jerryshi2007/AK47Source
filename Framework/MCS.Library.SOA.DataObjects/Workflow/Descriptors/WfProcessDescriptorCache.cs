using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;
using System.Xml.Linq;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	internal class WfProcessDescriptorXmlCache : CacheQueue<string, XElement>
	{
		public static readonly WfProcessDescriptorXmlCache Instance = CacheManager.GetInstance<WfProcessDescriptorXmlCache>();
	}
}

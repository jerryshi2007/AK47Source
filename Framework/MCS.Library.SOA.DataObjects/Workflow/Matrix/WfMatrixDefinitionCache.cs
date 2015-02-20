using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Caching;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	internal class WfMatrixDefinitionCache : CacheQueue<string, WfMatrixDefinition>
	{
		public static readonly WfMatrixDefinitionCache Instance = CacheManager.GetInstance<WfMatrixDefinitionCache>();
	}
}

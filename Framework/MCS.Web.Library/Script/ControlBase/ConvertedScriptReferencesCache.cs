using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using MCS.Library.Caching;

namespace MCS.Web.Library.Script
{
	internal class ConvertedScriptReferencesCache : ContextCacheQueueBase<string, ScriptReference>
	{
		public static ConvertedScriptReferencesCache Instance
		{
			get
			{
				return ContextCacheManager.GetInstance<ConvertedScriptReferencesCache>();
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Caching;

namespace PermissionCenter.Extensions
{
	/// <summary>
	/// 
	/// </summary>
	public class PCServiceMethodCache : ServiceMethodCache
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="instanceName"></param>
		/// <param name="maxLength"></param>
		public PCServiceMethodCache(string instanceName, int maxLength) :
			base(instanceName, maxLength)
		{
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Caching;

namespace AUCenterServices.Services
{
	/// <summary>
	/// 
	/// </summary>
	public class AUServiceMethodCache : ServiceMethodCache
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="instanceName"></param>
		/// <param name="maxLength"></param>
		public AUServiceMethodCache(string instanceName, int maxLength) :
			base(instanceName, maxLength)
		{
		}
	}
}
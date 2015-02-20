using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.OGUPermission
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class SoapHeaderConnectionMappingItem
	{
		/// <summary>
		/// 
		/// </summary>
		public SoapHeaderConnectionMappingItem()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		public SoapHeaderConnectionMappingItem(string source, string destination)
		{
			this.Source = source;
			this.Destination = destination;
		}

		/// <summary>
		/// 
		/// </summary>
		public string Source
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		public string Destination
		{
			get;
			set;
		}
	}
}

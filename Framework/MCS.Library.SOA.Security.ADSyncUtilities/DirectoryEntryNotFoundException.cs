using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.Security.ADSyncUtilities
{
	[Serializable]
	public class DirectoryEntryNotFoundException : Exception
	{
		private string name;

		public DirectoryEntryNotFoundException(string name)
			: base("未找到Directory Entry:" + name)
		{
			this.name = name;
		}

		public DirectoryEntryNotFoundException(string name, string message) : base(message) { this.name = name; }
		public DirectoryEntryNotFoundException(string message, Exception inner) : base(message, inner) { }
		protected DirectoryEntryNotFoundException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}

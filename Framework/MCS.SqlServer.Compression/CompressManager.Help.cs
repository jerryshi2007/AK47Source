using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace MCS.SqlServer.Compression
{
	public partial class CompressManager
	{
		[SqlFunction]
		public static SqlString AboutCompression()
		{
			return Assembly.GetExecutingAssembly().FullName;
		}
	}
}

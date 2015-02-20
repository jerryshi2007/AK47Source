using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;

namespace MCS.Library.Accredit
{
	/// <summary>
	/// 处理AD对象前的事件
	/// </summary>
	/// <param name="sr"></param>
	/// <param name="context"></param>
	/// <param name="bContinue"></param>
	public delegate void BeforeProcessADObjectDelegate(SearchResult sr, AD2DBInitialParams initParams, ref bool bContinue);
}

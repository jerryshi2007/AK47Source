using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using MCS.Library.Globalization;

namespace MCS.Library.Services
{
	/// <summary>
	/// 测试用的后台线程
	/// </summary>
	public class TestServiceThread : ThreadTaskBase
	{
		public override void OnThreadTaskStart()
		{
			this.Params.Log.Write("TestServiceThread执行");
		}
	}
}

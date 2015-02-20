using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Core
{
	/// <summary>
	/// 如何将处理过程输出的接口
	/// </summary>
	public interface IProcessProgressResponser
	{
		/// <summary>
		/// 注册Responser，这里可以初始化一些参数，如Output和Error
		/// </summary>
		/// <param name="progress"></param>
		void Register(ProcessProgress progress);

		/// <summary>
		/// 输出Progress信息
		/// </summary>
		/// <param name="progress"></param>
		void Response(ProcessProgress progress);
	}
}

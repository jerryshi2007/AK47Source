using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security
{
	/// <summary>
	/// 定义进行导入操作的接口
	/// </summary>
	public interface IImportAction
	{
		/// <summary>
		/// 执行导入操作
		/// </summary>
		/// <param name="objectSet">已上传的对象集</param>
		/// <param name="context">导入操作上下文</param>
		void DoImport(SCObjectSet objectSet, IImportContext context);
	}
}

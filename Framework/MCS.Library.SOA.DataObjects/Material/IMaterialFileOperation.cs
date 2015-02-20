using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 附件文件操作的接口类
	/// </summary>
	public interface IMaterialFileOperation
	{
		/// <summary>
		/// 执行附件的文件操作
		/// </summary>
		/// <param name="rootPath"></param>
		/// <param name="fileOperation"></param>
		/// <param name="content">附件的内容，可以为空，如果为空，则内部生成一个</param>
		void DoModifyFileOperations(string rootPath, MaterialFileOeprationInfo fileOperation, MaterialContent content);

		/// <summary>
		/// 在附件加载后，修饰附件的属性。例如修改ShowFileUrl属性
		/// </summary>
		/// <param name="materials"></param>
		void DecorateMaterialListAfterLoad(MaterialList materials);
	}
}

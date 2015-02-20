using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 附件内容存储的管理器
	/// </summary>
	public interface IMaterialContentPersistManager
	{
		bool CheckSourceFileExists
		{
			get;
			set;
		}

		FileInfo SourceFileInfo
		{
			get;
			set;
		}

		FileInfo DestFileInfo
		{
			get;
			set;
		}

		string PathRoot
		{
			get;
			set;
		}

		/// <summary>
		/// 保存附件的内容
		/// </summary>
		/// <param name="content"></param>
		void SaveMaterialContent(MaterialContent content);

		/// <summary>
		/// 加载附件的内容
		/// </summary>
		/// <param name="contentID"></param>
		/// <returns></returns>
		Stream GetMaterialContent(string contentID);

		/// <summary>
		/// 删除附件内容
		/// </summary>
		/// <param name="content"></param>
		void DeleteMaterialContent(MaterialContent content);


		/// <summary>
		/// 内容是否存在
		/// </summary>
		/// <param name="contentID"></param>
		/// <returns></returns>
		bool ExistsContent(string contentID);
	}
}

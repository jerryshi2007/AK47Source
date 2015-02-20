using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 附件内容
	/// </summary>
	[Serializable]
	[ORTableMapping("WF.MATERIAL_CONTENT")]
	public class MaterialContent
	{
		[ORFieldMapping("CONTENT_ID", PrimaryKey = true)]
		public string ContentID
		{
			get;
			set;
		}

		[ORFieldMapping("RELATIVE_ID")]
		public string RelativeID
		{
			get;
			set;
		}

		[ORFieldMapping("CLASS")]
		public string Class
		{
			get;
			set;
		}

		[ORFieldMapping("FILE_NAME")]
		public string FileName
		{
			get;
			set;
		}

		[ORFieldMapping("FILE_SIZE")]
		public long FileSize
		{
			get;
			set;
		}

		private IUser _Creator = null;

		[SubClassORFieldMapping("ID", "CREATOR_ID")]
		[SubClassORFieldMapping("DisplayName", "CREATOR_NAME")]
		[SubClassType(typeof(OguUser))]
		public IUser Creator
		{
			get
			{
				return this._Creator;
			}
			set
			{
				this._Creator = (IUser)OguBase.CreateWrapperObject(value);
			}
		}

		[ORFieldMapping("CREATE_TIME")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select | ClauseBindingFlags.Where)]
		public DateTime CreateTime
		{
			get;
			set;
		}

		[ORFieldMapping("UPDATE_TIME")]
		[SqlBehavior(DefaultExpression = "GETDATE()")]
		public DateTime UpdateTime
		{
			get;
			set;
		}

		[ORFieldMapping("CONTENT_DATA")]
		public byte[] ContentData
		{
			get;
			set;
		}

		/// <summary>
		/// 源内容的物理文件地址
		/// </summary>
		[NoMapping]
		public FileInfo PhysicalSourceFilePath
		{
			get;
			set;
		}

		/// <summary>
		/// 内容是否为空
		/// </summary>
		/// <returns></returns>
		public bool IsEmpty()
		{
			return this.ContentData == null && PhysicalSourceFilePath == null;
		}

		/// <summary>
		/// 克隆附件的内容
		/// </summary>
		/// <returns></returns>
		public MaterialContent Clone()
		{
			MaterialContent result = new MaterialContent();

			result.ContentID = this.ContentID;
			result.RelativeID = this.RelativeID;
			result._Creator = this._Creator;
			result.Class = this.Class;
			result.ContentData = this.ContentData;
			result.CreateTime = this.CreateTime;
			result.FileName = this.FileName;
			result.FileSize = this.FileSize;
			result.UpdateTime = this.UpdateTime;
			result.PhysicalSourceFilePath = this.PhysicalSourceFilePath;

			return result;
		}
	}

	[Serializable]
	public class MaterialContentCollection : EditableDataObjectCollectionBase<MaterialContent>
	{
	}
}

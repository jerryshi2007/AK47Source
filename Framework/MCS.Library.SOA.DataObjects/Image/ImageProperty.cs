using System;
using System.IO;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MCS.Library.Principal;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[ORTableMapping("WF.IMAGE")]
    [TenantRelativeObject]
	public class ImageProperty
	{
		private string id = string.Empty;
		private int width;
		private int height;
		private string name = string.Empty;
		private string resourceID = string.Empty;
		private string newName = string.Empty;
		private string originalName = string.Empty;
		private MaterialContent _Content = null;
		private string filePath = string.Empty;

		public ImageProperty()
		{ }

		/// <summary>
		/// 图片的ID
		/// </summary>
		[ORFieldMapping("ID", PrimaryKey = true, IsNullable = false)]
		public string ID
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		[NoMapping]
		public string FilePath
		{
			get { return filePath; }
			set { filePath = value; }
		}

		[ORFieldMapping("WIDTH")]
		public int Width
		{
			get
			{
				return this.width;
			}
			set
			{
				this.width = value;
			}
		}

		[ORFieldMapping("HEIGHT")]
		public int Height
		{
			get
			{
				return this.height;
			}
			set
			{
				this.height = value;
			}
		}

		[ORFieldMapping("NAME")]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		[ORFieldMapping("ORIGINAL_NAME")]
		public string OriginalName
		{
			get
			{
				return this.originalName;
			}
			set
			{
				this.originalName = value;
			}
		}

		[ORFieldMapping("NEW_NAME")]
		public string NewName
		{
			get
			{
				return this.newName;
			}
			set
			{
				this.newName = value;
			}
		}

		[ORFieldMapping("RESOURCE_ID")]
		public string ResourceID
		{
			get
			{
				return this.resourceID;
			}
			set
			{
				this.resourceID = value;
			}
		}

		[ORFieldMapping("CLASS")]
		public string Class
		{
			get;
			set;
		}

		[NoMapping]
		public string Src
		{
			get;
			set;
		}

		/// <summary>
		/// 数据是否改变
		/// </summary>
		[NoMapping]
		public bool Changed
		{
			get;
			set;
		}

		/// <summary>
		/// 最后更新时间
		/// </summary>
		[ORFieldMapping("UPDATE_TIME")]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Where | ClauseBindingFlags.Select | ClauseBindingFlags.Update, DefaultExpression = "GETDATE()")]
		public DateTime UpdateTime
		{
			get;
			set;
		}

		/// <summary>
		/// 如果
		/// NewName为空，那么我们认为内容就是空
		/// </summary>
		/// <returns></returns>
		public bool IsEmpty()
		{
			return this.NewName.IsNullOrEmpty();
		}

		/// <summary>
		/// 图片内容数据
		/// </summary>
		[NoMapping]
		[ScriptIgnore]
		[NoXmlObjectMapping]
		[System.Xml.Serialization.XmlIgnore]
		public MaterialContent Content
		{
			get
			{
				return this._Content;
			}
			set
			{
				this._Content = value;
			}
		}

		/// <summary>
		/// Clone的源图片ID，用于Ensure等操作
		/// </summary>
		[NoMapping]
		public string SourceImageID
		{
			get;
			private set;
		}

		/// <summary>
		/// 克隆一个图片信息
		/// </summary>
		/// <returns></returns>
		public ImageProperty Clone()
		{
			ImageProperty imageProp = new ImageProperty();

			imageProp.id = this.id;
			imageProp.name = this.name;
			imageProp.newName = this.newName;
			imageProp.originalName = this.originalName;
			imageProp.resourceID = this.resourceID;
			imageProp.Src = this.Src;
			imageProp.width = this.width;
			imageProp.height = this.height;
			imageProp.Changed = this.Changed;
			imageProp.Class = this.Class;
			imageProp.SourceImageID = this.id;

			if (this._Content != null)
				imageProp._Content = this._Content.Clone();

			return imageProp;
		}

		/// <summary>
		/// 初始化，或者准备MaterialContent
		/// </summary>
		public void EnsureMaterialContent()
		{
			this._Content = GenerateMaterialContent();

			if (this.IsEmpty() == false)
			{
				EnsureEmptyImage();
			}
			else
			{
				this._Content.ContentData = null;
				this._Content.PhysicalSourceFilePath = null;
			}
		}

		private void EnsureEmptyImage()
		{
			if (this.Changed)
				EnsureChangedImage();
			else
				EnsureNotChangedImage();
		}

		private void EnsureChangedImage()
		{
			if (this.SourceImageID.IsNullOrEmpty())
			{
				this._Content.PhysicalSourceFilePath =
					ImagePropertyAdapter.Instance.GetPhysicalUploadImagePath(this._Content.FileName);
			}
			else
			{
				IMaterialContentPersistManager manager = MaterialContentSettings.GetConfig().PersistManager;

				manager.DestFileInfo = ImagePropertyAdapter.Instance.GetPhysicalUploadImagePath(this._Content.FileName);
				if (!File.Exists(manager.DestFileInfo.DirectoryName))
					Directory.CreateDirectory(manager.DestFileInfo.DirectoryName);
				using (Stream srcContent = manager.GetMaterialContent(this.SourceImageID))
				{
					using (FileStream fs = new FileStream(manager.DestFileInfo.ToString(), FileMode.Create, FileAccess.Write))
					{
						srcContent.CopyTo(fs);
					}

					this._Content.PhysicalSourceFilePath = manager.DestFileInfo;
				}
			}
		}

		private void EnsureNotChangedImage()
		{
			if (this._Content.ContentData == null)
			{
				IMaterialContentPersistManager manager = MaterialContentSettings.GetConfig().PersistManager;

				manager.DestFileInfo = ImagePropertyAdapter.Instance.GetPhysicalDestinationImagePath(this._Content.FileName);

				this._Content.ContentData = manager.GetMaterialContent(this._Content.ContentID).ToBytes();
			}
		}

		/// <summary>
		/// 根据图片信息生成MaterialContent
		/// </summary>
		/// <returns></returns>
		private MaterialContent GenerateMaterialContent()
		{
			MaterialContent content = new MaterialContent();

			content.ContentID = this.ID;
			content.FileName = this.NewName;
			content.RelativeID = this.ResourceID;
			content.Class = this.Class;
			content.UpdateTime = this.UpdateTime;

			if (DeluxePrincipal.IsAuthenticated)
				content.Creator = DeluxeIdentity.CurrentUser;

			if (this._Content != null)
				content.ContentData = this._Content.ContentData;

			return content;
		}
	}

	/// <summary>
	/// 图片信息的属性
	/// </summary>
	public class ImagePropertyCollection : EditableDataObjectCollectionBase<ImageProperty>
	{
		/// <summary>
		/// 初始化，或者准备MaterialContent
		/// </summary>
		public void EnsureMaterialContent()
		{
			this.ForEach(img => img.EnsureMaterialContent());
		}

		/// <summary>
		/// 克隆图片信息集合，其中的每一项信息也是Clone的。
		/// </summary>
		/// <returns></returns>
		public ImagePropertyCollection Clone()
		{
			ImagePropertyCollection result = new ImagePropertyCollection();

			this.ForEach(img => result.Add(img.Clone()));

			return result;
		}
	}
}

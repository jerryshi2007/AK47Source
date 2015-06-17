#region
// -------------------------------------------------
// Assembly	：	HB.DataEntities
// FileName	：	Material.cs
// Remark	：	
// -------------------------------------------------
// VERSION		AUTHOR		DATE			CONTENT
// 1.0			张梁		20070724		创建
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Data.Mapping;
using MCS.Library.OGUPermission;
using MCS.Library.Principal;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 版本类型
	/// </summary>
	public enum MaterialVersionType
	{
		/// <summary>
		/// 正常版本
		/// </summary>
		Normal = 0,
		/// <summary>
		/// 副本
		/// </summary>
		CopyVersion = 1
	}

	/// <summary>
	/// Material实体类
	/// </summary>
	[Serializable]
	[ORTableMapping("WF.MATERIAL")]
    [TenantRelativeObject]
	public class Material
	{
		#region 私有变量

		private string id = string.Empty;
		private IOrganization department;
		private string resourceID;
		private int sortID;
		private string materialClass;
		private string title = string.Empty;
		private int pageQuantity;
		private string relativeFilePath = string.Empty;
		private string tempPhysicalFilePath = string.Empty;
		private string originalName = string.Empty;
		private IUser creator;
		private string lastUploadTag;
		private DateTime createDateTime;
		private DateTime modifyTime;
		private string wfProcessID;
		private string wfActivityID;
		private string wfActivityName;
		private string parentID;
		private Material sourceMaterial = null;
		private MaterialVersionType versionType = MaterialVersionType.Normal;
		private string extraData;
		private Dictionary<string, string> extraDataDictionary;
		private string showFileUrl = string.Empty;

		private MaterialContent _Content = null;
		private string _SourceMaterialID;
		#endregion

		public Material()
		{

		}

		/// <summary>
		/// 生成副本
		/// </summary>
		/// <returns>生成的副本</returns>
		public Material GenerateCopyVersion()
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(this == null, "this");

			Material material = new Material();

			material.id = Guid.NewGuid().ToString();
			material.Department = this.department;
			material.resourceID = this.resourceID;
			material.sortID = this.sortID;
			material.materialClass = this.materialClass;
			material.title = this.title;
			material.pageQuantity = this.pageQuantity;
			material.relativeFilePath = this.relativeFilePath.Replace(this.id, material.ID);
			material.originalName = this.originalName;

			if (DeluxePrincipal.IsAuthenticated)
				material.Creator = (IUser)OguUser.CreateWrapperObject(DeluxeIdentity.CurrentRealUser);

			material.lastUploadTag = this.lastUploadTag;
			material.createDateTime = DateTime.Now;
			material.modifyTime = this.modifyTime;
			material.wfProcessID = this.wfProcessID;
			material.wfActivityID = this.wfActivityID;
			material.wfActivityName = this.wfActivityName;
			material.parentID = this.id;
			material.sourceMaterial = this;
			material.versionType = MaterialVersionType.CopyVersion;
			material.extraData = this.extraData;
			material.showFileUrl = this.showFileUrl;

			return material;
		}

		/// <summary>
		/// 产生文件版本
		/// </summary>
		/// <param name="rootPath">文件夹所在根目录路径</param>
		/// <param name="resourceID">新表单ID</param>
		/// <param name="wfProcessID">新工作流ID</param>
		/// <param name="wfActivityID">新工作流节点ID</param>
		/// <param name="wfActivityName">新工作流节点名称</param>
		/// <param name="department">产生的副本所在的部门</param>
		/// <returns>生成的版本</returns>
		public Material GenerateOtherVersion(string resourceID, string wfProcessID, string wfActivityID, string wfActivityName, IOrganization department)
		{
			ExceptionHelper.TrueThrow<ArgumentNullException>(this == null, "this");
			ExceptionHelper.CheckStringIsNullOrEmpty(resourceID, "resourceID");
			ExceptionHelper.CheckStringIsNullOrEmpty(wfProcessID, "wfProcessID");
			ExceptionHelper.CheckStringIsNullOrEmpty(wfActivityID, "wfActivityID");

			Material material = new Material();

			material.id = Guid.NewGuid().ToString();
			material.Department = department;
			material.resourceID = resourceID;
			material.sortID = this.sortID;
			material.materialClass = this.materialClass;
			material.title = this.title;
			material.pageQuantity = this.pageQuantity;
			material.relativeFilePath = this.relativeFilePath.Replace(this.id, material.ID);
			material.originalName = this.originalName;
			material.creator = this.creator;
			material.lastUploadTag = this.lastUploadTag;
			material.createDateTime = this.createDateTime;
			material.modifyTime = this.modifyTime;
			material.wfProcessID = wfProcessID;
			material.wfActivityID = wfActivityID;
			material.wfActivityName = wfActivityName;
			material.parentID = this.id;
			material.sourceMaterial = this;
			material.versionType = MaterialVersionType.Normal;
			material.extraData = this.extraData;
			material.showFileUrl = this.showFileUrl;

			return material;
		}

		/// <summary>
		/// 转化为附件修改对象
		/// </summary>
		/// <returns>附件修改对象</returns>
		internal MaterialModifyObject ConvertToMaterialModifyObject()
		{
			MaterialModifyObject obj = new MaterialModifyObject();

			obj.ID = this.id;
			obj.OriginalName = this.originalName;
			obj.RelativeFilePath = this.relativeFilePath;
			obj.SortID = this.sortID;
			obj.Title = this.title;
			obj.WfActivityID = this.wfActivityID;
			obj.WfProcessID = this.wfProcessID;
			obj.CreatorFullPath = this.Creator.FullPath;

			return obj;
		}

		#region 属性
		[NoMapping]
		[ScriptIgnore]
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
		/// 附件的ID
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

		/// <summary>
		/// 所属部门
		/// </summary>
		[SubClassORFieldMapping("ID", "DEPARTMENT_ID")]
		[SubClassORFieldMapping("Name", "DEPARTMENT_NAME")]
		[SubClassORFieldMapping("GlobalSortID", "DEPARTMENT_GLOBALSORT_ID")]
		[SubClassType(typeof(OguOrganization))]
		public IOrganization Department
		{
			get
			{
				return this.department;
			}
			set
			{
				this.department = (IOrganization)OguOrganization.CreateWrapperObject(value);
			}
		}

		/// <summary>
		/// 表单ID
		/// </summary>
		[ORFieldMapping("RESOURCE_ID", IsNullable = true)]
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

		/// <summary>
		/// 排序号
		/// </summary>
		[ORFieldMapping("SORT_ID", IsNullable = true)]
		public int SortID
		{
			get
			{
				return this.sortID;
			}
			set
			{
				this.sortID = value;
			}
		}

		/// <summary>
		/// 类别
		/// </summary>
		[ORFieldMapping("CLASS", IsNullable = true)]
		public string MaterialClass
		{
			get
			{
				return this.materialClass;
			}
			set
			{
				this.materialClass = value;
			}
		}

		/// <summary>
		/// 文件标题
		/// </summary>
		[ORFieldMapping("TITLE", IsNullable = false)]
		public string Title
		{
			get
			{
				return this.title;
			}
			set
			{
				this.title = value;
			}
		}

		/// <summary>
		/// 页数
		/// </summary>
		[ORFieldMapping("PAGE_QUANTITY", IsNullable = true)]
		public int PageQuantity
		{
			get
			{
				return this.pageQuantity;
			}
			set
			{
				this.pageQuantity = value;
			}
		}

		/// <summary>
		/// 相对路径
		/// </summary> 
		[ORFieldMapping("RELATIVE_FILE_PATH", IsNullable = false)]
		public string RelativeFilePath
		{
			get
			{
				return this.relativeFilePath;
			}
			set
			{
				this.relativeFilePath = value;
			}
		}

		/// <summary>
		/// 临时物理文件路径。是RelativeFilePath和具体的根路径组合在一起的路径。
		/// 出于安全考虑，这个路径通常在服务器端生成，且不会进行JSON序列化。
		/// </summary>
		[NoMapping]
		public string TempPhysicalFilePath
		{
			get
			{
				return this.tempPhysicalFilePath;
			}
			set
			{
				this.tempPhysicalFilePath = value;
			}
		}

		/// <summary>
		/// 文件原始名称
		/// </summary>
		[ORFieldMapping("ORIGINAL_NAME", IsNullable = true)]
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

		/// <summary>
		/// 创建者
		/// </summary>
		[SubClassORFieldMapping("ID", "CREATOR_ID", IsNullable = false)]
		[SubClassORFieldMapping("DisplayName", "CREATOR_USER_NAME", IsNullable = false)]
		[SubClassType(typeof(OguUser))]
		public IUser Creator
		{
			get
			{
				return this.creator;
			}
			set
			{
				this.creator = (IUser)OguUser.CreateWrapperObject(value);
			}
		}

		/// <summary>
		/// 最后上传标记
		/// </summary>
		[ORFieldMapping("LAST_UPLOAD_TAG", IsNullable = false)]
		public string LastUploadTag
		{
			get
			{
				return this.lastUploadTag;
			}
			set
			{
				this.lastUploadTag = value;
			}
		}

		/// <summary>
		/// 创建时间
		/// </summary>
		[ORFieldMapping("CREATE_DATETIME", IsNullable = false)]
		[SqlBehavior(BindingFlags = ClauseBindingFlags.Select, DefaultExpression = "getdate()")]
		public DateTime CreateDateTime
		{
			get
			{
				return this.createDateTime;
			}
			set
			{
				this.createDateTime = value;
			}
		}

		/// <summary>
		/// 最后修改时间
		/// </summary>
		[ORFieldMapping("MODIFY_TIME", IsNullable = true)]
		public DateTime ModifyTime
		{
			get
			{
				return this.modifyTime;
			}
			set
			{
				this.modifyTime = value;
			}
		}

		/// <summary>
		/// 工作流流程ID
		/// </summary>
		[ORFieldMapping("WF_PROCESS_ID", IsNullable = true)]
		public string WfProcessID
		{
			get
			{
				return this.wfProcessID;
			}
			set
			{
				this.wfProcessID = value;
			}
		}

		/// <summary>
		/// 工作流活动ID
		/// </summary>
		[ORFieldMapping("WF_ACTIVITY_ID", IsNullable = true)]
		public string WfActivityID
		{
			get
			{
				return this.wfActivityID;
			}
			set
			{
				this.wfActivityID = value;
			}
		}

		/// <summary>
		/// 工作流活动名称
		/// </summary>
		[ORFieldMapping("WF_ACTIVITY_NAME", IsNullable = true)]
		public string WfActivityName
		{
			get
			{
				return this.wfActivityName;
			}
			set
			{
				this.wfActivityName = value;
			}
		}

		/// <summary>
		/// 父版本ID
		/// </summary>
		[ORFieldMapping("PARENT_ID", IsNullable = true)]
		public string ParentID
		{
			get
			{
				return this.parentID;
			}
			set
			{
				this.parentID = value;
			}
		}

		/// <summary>
		/// 原始附件对象。 副本使用
		/// </summary>
		[SqlBehavior(BindingFlags = ClauseBindingFlags.None)]
		public Material SourceMaterial
		{
			get
			{
				return this.sourceMaterial;
			}
		}

		/// <summary>
		/// 版本类型
		/// </summary>
		[ORFieldMapping("VERSION_TYPE", IsNullable = false)]
		public MaterialVersionType VersionType
		{
			get
			{
				return this.versionType;
			}
			set
			{
				this.versionType = value;
			}
		}

		/// <summary>
		/// 附加数据
		/// </summary>
		[Obsolete("请使用ExtraDataDictionary")]
		[ORFieldMapping("EXTRA_DATA", IsNullable = true)]
		public string ExtraData
		{
			get
			{
				return this.extraData;
			}
			set
			{
				this.extraData = value;
			}
		}

		/// <summary>
		/// 附加数据字典
		/// </summary>
		[NoMapping]
		public Dictionary<string, string> ExtraDataDictionary
		{
			get
			{
				if (this.extraDataDictionary == null)
					this.extraDataDictionary = LoadExtraDataFromRawExtData(this.extraData);

				return this.extraDataDictionary;
			}
		}
		/// <summary>
		///在IE中打开文件的路径
		/// </summary>
		[SqlBehavior(BindingFlags = ClauseBindingFlags.None)]
		public string ShowFileUrl
		{
			get
			{
				return this.showFileUrl;
			}
			set
			{
				this.showFileUrl = value;
			}
		}
		#endregion

		public Material Clone()
		{
			Material newMaterial = new Material();

			newMaterial.id = this.id;
			newMaterial.Department = this.department;
			newMaterial.resourceID = this.resourceID;
			newMaterial.sortID = this.sortID;
			newMaterial.materialClass = this.materialClass;
			newMaterial.title = this.title;
			newMaterial.pageQuantity = this.pageQuantity;
			newMaterial.relativeFilePath = this.relativeFilePath.Replace(this.id, newMaterial.ID);
			newMaterial.originalName = this.originalName;
			newMaterial.creator = this.creator;
			newMaterial.lastUploadTag = this.lastUploadTag;
			newMaterial.createDateTime = this.createDateTime;
			newMaterial.modifyTime = this.modifyTime;
			newMaterial.wfProcessID = wfProcessID;
			newMaterial.wfActivityID = wfActivityID;
			newMaterial.wfActivityName = wfActivityName;
			newMaterial._SourceMaterialID = this.id;
			newMaterial.versionType = MaterialVersionType.Normal;
			newMaterial.extraData = this.extraData;
			newMaterial.showFileUrl = this.showFileUrl;

			if (this._Content != null)
				newMaterial._Content = this._Content.Clone();

			return newMaterial;
		}

		public MaterialContent GenerateMaterialContent()
		{
			MaterialContent content = new MaterialContent();

			content.ContentID = this.ID;
			content.FileName = this.OriginalName;
			content.RelativeID = this.ResourceID;
			content.Class = this.MaterialClass;
			content.Creator = this.Creator;

			return content;
		}

		/// <summary>
		/// 得到文件的物理路径
		/// </summary>
		/// <param name="rootPathName"></param>
		public void GenerateTempPhysicalFilePath(string rootPathName)
		{
			this.tempPhysicalFilePath = GetTempPhysicalFilePath(rootPathName);
			//if (this.showFileUrl.IsNotEmpty())
			//    this.tempPhysicalFilePath = GetTempPhysicalFilePath(rootPathName);
			//else
			//    this.tempPhysicalFilePath = string.Empty;
		}

		/// <summary>
		/// 得到附件的物理路径
		/// </summary>
		/// <returns></returns>
		public string GetTempPhysicalFilePath(string rootPathName)
		{
			if (string.IsNullOrEmpty(rootPathName))
				rootPathName = MaterialAdapter.DefaultUploadPathName;

			string uploadRootPath = AppPathConfigSettings.GetConfig().Paths[rootPathName].Dir;

			ExceptionHelper.CheckStringIsNullOrEmpty(uploadRootPath, "uploadRootPath");

			FileInfo sourceFile = new FileInfo(uploadRootPath + @"Temp\" + Path.GetFileName(this.RelativeFilePath));

			return sourceFile.FullName;
		}

		/// <summary>
		/// 得到临时文件夹下的附件的内容（还没有正式提交）
		/// </summary>
		/// <param name="rootPathName"></param>
		/// <returns></returns>
		public Stream GetTemporaryContent(string rootPathName)
		{
			FileInfo sourceFile = new FileInfo(GetTempPhysicalFilePath(rootPathName));

			return sourceFile.OpenRead();
		}

		// <summary>
		/// 初始化，或者准备MaterialContent
		/// </summary>
		public void EnsureMaterialContent()
		{
			this._Content = GenerateExistsMaterialContent();

			EnsureNotChangedMaterial();
		}

		internal static string ConvertExtraDataToXmlString(IDictionary<string, string> data)
		{
			var xml = XmlHelper.CreateDomDocument("<Data/>");

			data.IsNotNull(i =>
			{
				foreach (var kp in data)
				{
					var item = xml.CreateElement("Item");
					item.SetAttribute("Name", kp.Key);
					item.SetAttribute("Value", kp.Value);
					xml.DocumentElement.AppendChild(item);
				}
			});

			return xml.OuterXml;
		}

		internal void FillExtraData(Dictionary<string, string> data)
		{
			this.extraDataDictionary = data;
			this.extraData = Material.ConvertExtraDataToXmlString(data);
		}

		private static Dictionary<string, string> LoadExtraDataFromRawExtData(string raw)
		{
			var result = new Dictionary<string, string>();
			raw.IsNotWhiteSpace(i =>
			{
				try
				{
					var items = XmlHelper.CreateDomDocument(raw).SelectNodes("Data/Item");

					foreach (XmlNode item in items)
					{
						result.Add(XmlHelper.GetAttributeText(item, "Name"), XmlHelper.GetAttributeText(item, "Value"));
					}
				}
				catch
				{
				}
			});

			return result;
		}

		#region Private Method
		/// <summary>
		/// 根据附件信息生成MaterialContent
		/// </summary>
		/// <returns></returns>
		private MaterialContent GenerateExistsMaterialContent()
		{
			MaterialContent content = new MaterialContent();

			content.ContentID = this.ID;
			content.FileName = this.OriginalName;
			content.RelativeID = this.ResourceID;
			content.Class = this.MaterialClass;

			if (DeluxePrincipal.IsAuthenticated)
				content.Creator = DeluxeIdentity.CurrentUser;

			if (this._Content != null)
				content.ContentData = this._Content.ContentData;

			return content;
		}

		private void EnsureNotChangedMaterial()
		{
			if (this._Content.ContentData == null)
			{
				if (this.TempPhysicalFilePath.IsNotEmpty())
				{
					FileInfo tempFile = new FileInfo(this.TempPhysicalFilePath);

					if (tempFile.Exists)
						this._Content.ContentData = tempFile.OpenRead().ToBytes();
				}

				if (this._Content.ContentData == null)
				{
					IMaterialContentPersistManager manager = MaterialContentSettings.GetConfig().PersistManager;

					//在数据库存储附件的模式下，这个参数其实没用
					manager.DestFileInfo = new FileInfo(GetTempPhysicalFilePath(string.Empty));

					this._Content.ContentData = manager.GetMaterialContent(this._Content.ContentID).ToBytes();
				}
			}
		}
		#endregion
	}
}

#region
// -------------------------------------------------
// Assembly	：	HB.DataObjects
// FileName	：	OguBase.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    李苗	    20070628		创建
// -------------------------------------------------
#endregion

using System;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Data.Mapping;
using MCS.Library.Logging;
using System.Xml.Linq;

namespace MCS.Library.SOA.DataObjects
{
	#region OguBase
	/// <summary>
	/// 对象人员的基类,继承了IOguObject接口
	/// </summary>
	[Serializable]
	[DebuggerDisplay("Name = {name}")]
	[XElementSerializable]
	[XmlRootMapping("OguBase", true)]
	public class OguBase : IOguObject, ISerializable, ISimpleXmlSerializer
	{
		private string id = string.Empty;
		private string name = null;
		private string displayName = null;
		private string description = null;
		private string fullPath = null;
		private SchemaType objectType;

		[NonSerialized]
		private IOrganization parent = null;
		[NonSerialized]
		private IOrganization topOU = null;

		private string sortID = null;
		private string globalSortID = null;
		private int levels = -1;

		[NonSerialized]
		private IDictionary properties = null;

		[NonSerialized]
		private string tag = string.Empty;

		[NonSerialized]
		private IOguObject ogu = null;

		[NonSerialized]
		private Dictionary<string, object> clientContext = null;

		private bool hasLoadingError = false;

		#region IOguObject 成员
		/// <summary>
		/// 对象ID
		/// </summary>
		[XmlObjectMapping]
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
		/// 对象名称
		/// </summary>
		[XmlObjectMapping]
		public string Name
		{
			get
			{
				if (this.name == null)
				{
					this.name = this.displayName;

					if (this.name == null && Ogu != null)
						this.name = Ogu.Name;
				}

				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		/// <summary>
		/// 对象显示名称
		/// </summary>
		[XmlObjectMapping]
		public string DisplayName
		{
			get
			{
				if (this.displayName == null)
				{
					if (Ogu != null)
						this.displayName = Ogu.DisplayName;
					else
						this.displayName = this.name;
				}

				return this.displayName;
			}
			set
			{
				this.displayName = value;
			}
		}

		/// <summary>
		/// 对象描述
		/// </summary>
		public string Description
		{
			get
			{
				if (this.description == null && Ogu != null)
					this.description = Ogu.Description;

				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		/// <summary>
		/// 对象路径
		/// </summary>
		[XmlObjectMapping]
		public string FullPath
		{
			get
			{
				if (this.fullPath == null && Ogu != null)
					this.fullPath = Ogu.FullPath;

				return this.fullPath;
			}
			set
			{
				this.fullPath = value;
			}
		}

		/// <summary>
		/// 对象类型
		/// </summary>
		public virtual SchemaType ObjectType
		{
			get
			{
				return this.objectType;
			}
			set
			{
				this.objectType = value;
			}
		}

		/// <summary>
		/// 对象的上一层组织机构
		/// </summary>
		[NoMapping]
		public IOrganization Parent
		{
			get
			{
				if (parent == null && Ogu != null)
					this.parent = Ogu.Parent;

				return this.parent;
			}
		}

		/// <summary>
		/// 对象本级的序号
		/// </summary>
		public string SortID
		{
			get
			{
				if (this.sortID == null && Ogu != null)
					this.sortID = Ogu.SortID;

				return this.sortID;
			}
			set
			{
				this.sortID = value;
			}
		}

		/// <summary>
		/// 对象的全局序号
		/// </summary>
		public string GlobalSortID
		{
			get
			{
				if (this.globalSortID == null && Ogu != null)
					this.globalSortID = Ogu.GlobalSortID;

				return this.globalSortID;
			}
			set
			{
				this.globalSortID = value;
			}
		}

		/// <summary>
		/// 对象的顶级部门
		/// </summary>
		[NoMapping]
		public IOrganization TopOU
		{
			get
			{
				if (this.topOU == null && Ogu != null)
					this.topOU = Ogu.TopOU;

				return this.topOU;
			}
		}

		/// <summary>
		/// 对象的级别
		/// </summary>
		public int Levels
		{
			get
			{
				if (this.levels < 0 && this.fullPath != null)
					this.levels = this.FullPath.Split('\\').Length;

				return this.levels;
			}
			set
			{
				this.levels = value;
			}
		}

		/// <summary>
		/// 判断对象是否是人家的子对象
		/// </summary>
		/// <param name="parent"></param>
		/// <returns></returns>
		public bool IsChildrenOf(IOrganization parent)
		{
			bool result = false;

			if (Ogu != null)
				result = Ogu.IsChildrenOf(parent);

			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		public IDictionary Properties
		{
			get
			{
				if (this.properties == null)
				{
					if (this.ogu != null)
						this.properties = Ogu.Properties;
					else
						this.properties = new Dictionary<string, object>();
				}

				return this.properties;
			}
		}

		#endregion

		#region 基础类型

		protected IOguObject Ogu
		{
			get
			{
				if (this.ogu == null && this.id.IsNotEmpty() && this.hasLoadingError == false)
				{
					ExceptionHelper.CheckStringIsNullOrEmpty(this.id, "ID");

					SchemaType objType = this.ObjectType & (SchemaType.Users | SchemaType.Organizations | SchemaType.Groups);

					IList objList = null;

					switch (objType)
					{
						case SchemaType.Users:
							string db = this.displayName;

							if (string.IsNullOrEmpty(this.fullPath))
							{
								objList = FilterUserBySideline(OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, this.id));
							}
							else
							{
								objList = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.FullPath, this.fullPath);

								if (objList.Count == 0)
								{
									objList = FilterUserBySideline(OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, this.id));
								}
							}
							break;
						case SchemaType.Organizations:
							objList = OguMechanismFactory.GetMechanism().GetObjects<IOrganization>(SearchOUIDType.Guid, this.id);
							break;
						case SchemaType.Groups:
							objList = OguMechanismFactory.GetMechanism().GetObjects<IGroup>(SearchOUIDType.Guid, this.id);
							break;
						default:
							ExceptionHelper.FalseThrow(false, "不支持的对象类型{0}", objType);
							break;
					}

					if (objList.Count == 0)
					{
						//人员不存在。设置状态为3

						this.hasLoadingError = true;

						Logger logger = LoggerFactory.Create("WfRuntime");

						if (logger != null)
							logger.Write(string.Format("无法查询到ID为{0}的对象人员对象", this.id), LogPriority.Normal, 8009, TraceEventType.Error, "对象人员信息");
					}
					else
						this.ogu = (IOguObject)objList[0];
				}
				if (this.hasLoadingError)
				{
					if (this.properties == null)
					{
						this.properties = new Dictionary<string, object>();
						this.properties.Add("STATUS", 3);
					}
					else
					{
						this.properties["STATUS"] = 3;
					}
				}

				return ogu;
			}
		}

		private static IList FilterUserBySideline(IList<IUser> users)
		{
			List<IUser> result = new List<IUser>();

			foreach (IUser user in users)
			{
				if (user.IsSideline == false)
				{
					result.Add(user);
					break;
				}
			}

			if (result.Count == 0 && users.Count > 0)
				result.Add(users[0]);

			return result;
		}

		#endregion

		/// <summary>
		/// 一些额外的标记
		/// </summary>
		public string Tag
		{
			get { return this.tag; }
			set { this.tag = value; }
		}

		/// <summary>
		/// 这是和客户端Json序列化时可以交换的数据
		/// </summary>
		public Dictionary<string, object> ClientContext
		{
			get
			{
				if (this.clientContext == null)
					this.clientContext = new Dictionary<string, object>();

				return this.clientContext;
			}
			internal set
			{
				this.clientContext = value;
			}
		}

        /// <summary>
        /// 名称属性是否被初始化过
        /// </summary>
        /// <returns></returns>
        public bool IsNameInitialized()
        {
            return this.name.IsNotEmpty() || this.displayName.IsNotEmpty();
        }

		/// <summary>
		/// 是否存在加载错误
		/// </summary>
		public bool HasLoadingError
		{
			get { return this.hasLoadingError; }
			set { this.hasLoadingError = value; }
		}

		public override string ToString()
		{
			string result = string.Empty;

			if (this.displayName.IsNotEmpty())
				result += this.displayName;
			else
				if (this.name.IsNotEmpty())
					result += this.name;

			if (this.id.IsNotEmpty())
				result += string.Format("({0})", this.id);

			if (result.IsNullOrEmpty())
				result = base.ToString();

			return result;
		}
		#region 保护的成员

		#region ISerializable 成员

		protected OguBase(SchemaType st)
		{

		}

		protected OguBase(string id, SchemaType st)
		{
			//ExceptionHelper.TrueThrow(id == string.Empty, "对象ID不能为空！");

			this.id = id;
			this.objectType = st;
		}

		protected OguBase(IOguObject obj, SchemaType st)
		{
			ExceptionHelper.TrueThrow(obj == null, "对象不能为空");

			this.id = obj.ID;
			this.objectType = st;
			this.ogu = obj;
			this.fullPath = obj.FullPath;

			if ((obj is OguBase) == false)
			{
				this.name = obj.Name;
				this.displayName = obj.DisplayName;
				this.description = obj.Description;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected OguBase(SerializationInfo info, StreamingContext context)
		{
			this.id = info.GetString("ID");
			this.name = info.GetString("Name");
			this.displayName = info.GetString("DisplayName");
			this.fullPath = info.GetString("FullPath");
			this.globalSortID = info.GetString("GlobalSortID");
			this.sortID = info.GetString("SortID");
			this.objectType = (SchemaType)info.GetValue("ObjectType", typeof(SchemaType));
			this.hasLoadingError = info.GetBoolean("hasLoadingError");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("ID", this.id);
			info.AddValue("Name", this.name);
			info.AddValue("DisplayName", this.displayName);
			info.AddValue("FullPath", this.fullPath);
			info.AddValue("GlobalSortID", this.globalSortID);
			info.AddValue("SortID", this.sortID);
			info.AddValue("ObjectType", this.objectType);
			info.AddValue("hasLoadingError", this.hasLoadingError);
		}

		#endregion

		#endregion 保护的成员

		#region 静态的成员
		public static IOguObject CreateWrapperObject(IOguObject obj)
		{
			IOguObject result = null;

			if (obj is OguBase || obj == null)
				result = obj;
			else
			{
				if (obj is IUser)
					result = new OguUser((IUser)obj);
				else
					if (obj is IOrganization)
						result = new OguOrganization((IOrganization)obj);
					else
						if (obj is IGroup)
							result = new OguGroup((IGroup)obj);
						else
							ExceptionHelper.TrueThrow(true, "不能生成类型为{0}的对象人员包装类", obj.GetType().Name);
			}

			return result;
		}

		public static IOguObject CreateWrapperObject(string id, SchemaType type)
		{
			IOguObject result = null;

			switch (type)
			{
				case SchemaType.Organizations:
					result = new OguOrganization(id);
					break;
				case SchemaType.Users:
					result = new OguUser(id);
					break;
				case SchemaType.Groups:
					result = new OguGroup(id);
					break;

				default:
					ExceptionHelper.TrueThrow(true, string.Format("SchemaType错误{0}", type));
					break;
			}

			return result;
		}

		/// <summary>
		/// 判断对象是否为空(Null或ID为空)
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static bool IsNullOrEmpty(IOguObject obj)
		{
			return obj == null || string.IsNullOrEmpty(obj.ID);
		}

		/// <summary>
		/// 判断对象是否不为空(Null或ID为空)
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static bool IsNotNullOrEmpty(IOguObject obj)
		{
			return IsNullOrEmpty(obj) == false;
		}
		#endregion

		#region ISimpleXmlSerializer Members

		void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
		{
			element.NullCheck("element");

			if (refNodeName.IsNotEmpty())
				element = element.AddChildElement(refNodeName);

			element.SetAttributeValue("ID", this.ID);

			if (this.DisplayName.IsNotEmpty())
				element.SetAttributeValue("Name", this.DisplayName);
		}

		#endregion
	}

	#endregion OguBase
}

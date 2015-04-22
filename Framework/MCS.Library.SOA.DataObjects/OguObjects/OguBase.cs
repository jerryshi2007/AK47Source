#region
// -------------------------------------------------
// Assembly	��	HB.DataObjects
// FileName	��	OguBase.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070628		����
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
	/// ������Ա�Ļ���,�̳���IOguObject�ӿ�
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

		#region IOguObject ��Ա
		/// <summary>
		/// ����ID
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
		/// ��������
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
		/// ������ʾ����
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
		/// ��������
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
		/// ����·��
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
		/// ��������
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
		/// �������һ����֯����
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
		/// ���󱾼������
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
		/// �����ȫ�����
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
		/// ����Ķ�������
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
		/// ����ļ���
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
		/// �ж϶����Ƿ����˼ҵ��Ӷ���
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

		#region ��������

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
							ExceptionHelper.FalseThrow(false, "��֧�ֵĶ�������{0}", objType);
							break;
					}

					if (objList.Count == 0)
					{
						//��Ա�����ڡ�����״̬Ϊ3

						this.hasLoadingError = true;

						Logger logger = LoggerFactory.Create("WfRuntime");

						if (logger != null)
							logger.Write(string.Format("�޷���ѯ��IDΪ{0}�Ķ�����Ա����", this.id), LogPriority.Normal, 8009, TraceEventType.Error, "������Ա��Ϣ");
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
		/// һЩ����ı��
		/// </summary>
		public string Tag
		{
			get { return this.tag; }
			set { this.tag = value; }
		}

		/// <summary>
		/// ���ǺͿͻ���Json���л�ʱ���Խ���������
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
        /// ���������Ƿ񱻳�ʼ����
        /// </summary>
        /// <returns></returns>
        public bool IsNameInitialized()
        {
            return this.name.IsNotEmpty() || this.displayName.IsNotEmpty();
        }

		/// <summary>
		/// �Ƿ���ڼ��ش���
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
		#region �����ĳ�Ա

		#region ISerializable ��Ա

		protected OguBase(SchemaType st)
		{

		}

		protected OguBase(string id, SchemaType st)
		{
			//ExceptionHelper.TrueThrow(id == string.Empty, "����ID����Ϊ�գ�");

			this.id = id;
			this.objectType = st;
		}

		protected OguBase(IOguObject obj, SchemaType st)
		{
			ExceptionHelper.TrueThrow(obj == null, "������Ϊ��");

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

		#endregion �����ĳ�Ա

		#region ��̬�ĳ�Ա
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
							ExceptionHelper.TrueThrow(true, "������������Ϊ{0}�Ķ�����Ա��װ��", obj.GetType().Name);
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
					ExceptionHelper.TrueThrow(true, string.Format("SchemaType����{0}", type));
					break;
			}

			return result;
		}

		/// <summary>
		/// �ж϶����Ƿ�Ϊ��(Null��IDΪ��)
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static bool IsNullOrEmpty(IOguObject obj)
		{
			return obj == null || string.IsNullOrEmpty(obj.ID);
		}

		/// <summary>
		/// �ж϶����Ƿ�Ϊ��(Null��IDΪ��)
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

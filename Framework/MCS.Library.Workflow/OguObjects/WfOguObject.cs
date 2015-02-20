using System;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.Workflow.OguObjects
{
    /// <summary>
    /// 封装的机构人员对象基类
    /// </summary>
	[DebuggerDisplay("Name = {name}")]
    public abstract class WfOguObject : IOguObject, ISerializable
    {
        private string id = null;
        private string name = null;
        private string displayName = null;
        private string description = null;
        private string fullPath = null;
        private SchemaType objectType;
        private IOrganization parent = null;
        private string sortID = null;
        private string globalSortID = null;
        private IOrganization topOU = null;
        private int levels = -1;
        private IDictionary properties = null;

        private IOguObject baseObject = null;

		internal WfOguObject(SchemaType st)
		{
			this.objectType = st;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="st"></param>
        protected WfOguObject(string id, SchemaType st)
        {
            this.id = id;
            this.objectType = st;
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		protected WfOguObject(IOguObject obj, SchemaType st)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(obj != null, "obj");

			this.id = obj.ID;
			this.baseObject = obj;
			this.objectType = st;
		}

        #region IOguObject 成员

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get
            {
                if (this.description == null)
                    this.description = BaseObject.Description;

                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string DisplayName
        {
            get
            {
				if (this.displayName == null)
				{
					try
					{
						this.displayName = BaseObject.DisplayName;
					}
					catch (System.Exception)
					{
						this.displayName = this.name;
					}
				}

                return this.displayName;
            }
            set
            {
                this.displayName = value;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		public string Name
		{
			get
			{
				if (this.name == null)
					this.name = BaseObject.Name;

				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        public string FullPath
        {
            get
            {
                if (this.fullPath == null)
                    this.fullPath = BaseObject.FullPath;

                return this.fullPath;
            }
            set
            {
                this.fullPath = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string GlobalSortID
        {
            get
            {
                if (this.globalSortID == null)
                    this.globalSortID = BaseObject.GlobalSortID;

                return this.globalSortID;
            }
            set
            {
                this.globalSortID = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
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
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public bool IsChildrenOf(IOrganization parent)
        {
            return BaseObject.IsChildrenOf(parent);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Levels
        {
            get
            {
                if (this.levels < 0)
                    this.levels = this.FullPath.Split('\\').Length;

                return this.levels;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SchemaType ObjectType
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
        /// 
        /// </summary>
        public IOrganization Parent
        {
            get
            {
                if (this.parent == null)
                    this.parent = BaseObject.Parent;

                return this.parent;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDictionary Properties
        {
            get
            {
                if (this.properties == null)
                    this.properties = BaseObject.Properties;

                return this.properties;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SortID
        {
            get
            {
                if (this.sortID == null)
                    this.sortID = BaseObject.SortID;

                return this.sortID;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IOrganization TopOU
        {
            get
            {
                if (this.topOU == null)
                    this.topOU = BaseObject.TopOU;

                return this.topOU;
            }
        }

        #endregion

        /// <summary>
        /// 基础类型
        /// </summary>
        protected IOguObject BaseObject
        {
            get
            {
                if (this.baseObject == null)
                {
                    ExceptionHelper.CheckStringIsNullOrEmpty(this.id, "ID");

                    SchemaType objType = this.objectType & (SchemaType.Users | SchemaType.Organizations | SchemaType.Groups);

                    IList objList = null;

                    switch (objType)
                    {
                        case SchemaType.Users:
							if (string.IsNullOrEmpty(this.fullPath))
							{
								objList = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, this.id);
							}
							else
							{
								objList = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.FullPath, this.fullPath);

								if (objList.Count == 0)
									objList = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.Guid, this.id);
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

                    ExceptionHelper.FalseThrow(objList.Count > 0, "无法查询到ID为{0}的机构人员对象", this.id);

                    this.baseObject = (IOguObject)objList[0];
                }

                return this.baseObject;
            }
        }
        #region ISerializable 成员
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected WfOguObject(SerializationInfo info, StreamingContext context)
        {
            this.id = info.GetString("ID");
            this.name = info.GetString("Name");
            this.displayName = info.GetString("DisplayName");
            this.fullPath = info.GetString("FullPath");
            this.globalSortID = info.GetString("GlobalSortID");
            this.sortID = info.GetString("SortID");
            this.objectType = (SchemaType)info.GetValue("ObjectType", typeof(SchemaType));
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
        }

        #endregion
    }
}

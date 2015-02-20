using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.Adapters
{
	#region 幻象类型

	/// <summary>
	/// 幻象类型帮助类
	/// </summary>
	internal static class PhantomOguHelper
	{
		/// <summary>
		/// 将指定的<see cref="SchemaObjectBase"/>的派生类转换为<see cref="PhantomOguBase"/>的对应派生类
		/// </summary>
		/// <param name="oguObj"></param>
		/// <returns></returns>
		public static PhantomOguBase ToPhantom(this SchemaObjectBase oguObj)
		{
			var category = oguObj.Schema.Category;
			switch (category)
			{
				case "Users":
					return new PhantomUser(oguObj);
				case "Groups":
					return new PhantomGroup(oguObj);
				case "Organizations":
					return new PhantomOrganization(oguObj);
				default:
					throw new ArgumentException("不支持的SchemaObject类型", "oguObj");
			}
		}
	}

	/// <summary>
	/// 空壳组织机构对象，内部使用
	/// </summary>
	internal class DummyOguObject : IOguObject
	{
		private IDictionary _properties;

		/// <summary>
		/// 没用
		/// </summary>
		public string Description
		{
			get { return string.Empty; }
		}

		/// <summary>
		/// 没用
		/// </summary>
		public string DisplayName
		{
			get { return string.Empty; }
		}

		/// <summary>
		/// 没用
		/// </summary>
		public string FullPath
		{
			get { return string.Empty; }
		}

		/// <summary>
		/// 没用
		/// </summary>
		public string GlobalSortID
		{
			get { return string.Empty; }
		}

		/// <summary>
		/// 没用
		/// </summary>
		public string ID
		{
			get { return string.Empty; }
		}

		public bool IsChildrenOf(IOrganization parent)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// 没用
		/// </summary>
		public int Levels
		{
			get { return 0; }
		}

		public string Name
		{
			get { return string.Empty; }
		}

		/// <summary>
		/// 没用
		/// </summary>
		public SchemaType ObjectType
		{
			get { return SchemaType.Unspecified; }
		}

		public IOrganization Parent
		{
			get;
			set;
		}

		public System.Collections.IDictionary Properties
		{
			get
			{
				if (this._properties == null)
					this._properties = new Dictionary<string, object>();
				return this._properties;
			}

			set
			{
				this._properties = value;
			}
		}

		public string SortID
		{
			get { return string.Empty; }
		}

		public IOrganization TopOU
		{
			get { throw new NotImplementedException(); }
		}
	}

	/// <summary>
	/// 影子类型基类
	/// </summary>
	internal abstract class PhantomOguBase : OguBase, IOguObject
	{
		public PhantomOguBase(SchemaObjectBase src)
			: base(new DummyOguObject(), GetSchemaType(src.Schema.Category))
		{
			this.ID = src.ID;
			this.ObjectType = GetSchemaType(src.Schema.Category);
			this.Name = src.Properties.GetValue<string>("Name", string.Empty);
			this.DisplayName = src.Properties.GetValue<string>("DisplayName", this.Name);
			if (string.IsNullOrEmpty(this.DisplayName))
				this.DisplayName = this.Name;
			if (src.Status == SchemaObjectStatus.Deleted || src.Status == SchemaObjectStatus.DeletedByContainer)
			{
				this.Properties["STATUS"] = 3;
			}
			else
			{
				this.Properties["STATUS"] = 1;
			}
		}

		public PhantomOguBase()
			: base(new DummyOguObject(), SchemaType.Unspecified)
		{
		}

		private static SchemaType GetSchemaType(string category)
		{
			switch (category)
			{
				case "Users":
					return SchemaType.Users;
				case "Organizations":
					return SchemaType.Organizations;
				case "Groups":
					return SchemaType.Groups;
				default:
					throw new ArgumentOutOfRangeException("category");
			}
		}

		internal protected DummyOguObject Dummy
		{
			get
			{
				return this.Ogu as DummyOguObject;
			}
		}
	}

	/// <summary>
	/// 表示幻象组织
	/// </summary>
	[Serializable]
	internal class PhantomOrganization : PhantomOguBase, IOrganization, IVirtualOrganization
	{
		private string _customCode;
		private DepartmentClassType _deptClass;
		private DepartmentTypeDefine _deptType;
		private DepartmentRankType _rank;
		private bool _excludeVD;

		public PhantomOrganization(SchemaObjectBase obj)
			: base(obj)
		{
			this._customCode = obj.Properties.GetValue<string>("CustomCode", string.Empty);
			this._deptClass = obj.Properties.GetValue<DepartmentClassType>("DepartmentClass", DepartmentClassType.Unspecified);
			this._deptType = obj.Properties.GetValue<DepartmentTypeDefine>("DepartmentType", DepartmentTypeDefine.Unspecified);
			this._rank = obj.Properties.GetValue<DepartmentRankType>("DepartmentRank", DepartmentRankType.None);
		}

		public PhantomOrganization()
			: base()
		{
			this.ObjectType = SchemaType.Organizations;
		}

		public OguObjectCollection<IOguObject> Children
		{
			get { throw new NotImplementedException(); }
		}

		public string CustomsCode
		{
			get { return this._customCode; }
			set { this._customCode = value; }
		}

		public DepartmentClassType DepartmentClass
		{
			get { return this._deptClass; }
			set { this._deptClass = value; }
		}

		public DepartmentTypeDefine DepartmentType
		{
			get { return this._deptType; }
			set { this._deptType = value; }
		}

		public OguObjectCollection<T> GetAllChildren<T>(bool includeSideLine) where T : IOguObject
		{
			throw new NotImplementedException();
		}

		public bool IsTopOU
		{
			get { throw new NotImplementedException(); }
		}

		public OguObjectCollection<T> QueryChildren<T>(string matchString, bool includeSideLine, SearchLevel level, int returnCount) where T : IOguObject
		{
			throw new NotImplementedException();
		}

		public DepartmentRankType Rank
		{
			get { return this._rank; }
			set { this._rank = value; }
		}

		public bool ExcludeVirtualDepartment
		{
			get
			{
				return this._excludeVD;
			}

			set
			{
				this._excludeVD = value;
			}
		}
	}

	/// <summary>
	/// 表示幻象用户
	/// </summary>
	[Serializable]
	internal class PhantomUser : PhantomOguBase, IUser
	{
		private UserAttributesType _attributes;
		private string _email;
		private string _logOnName;
		private string _occupation;
		private UserRankType _userRank;
		private bool _isSideline;

		public PhantomUser(SchemaObjectBase obj)
			: base(obj)
		{
			this._attributes = obj.Properties.GetValue("CadreType", UserAttributesType.Unspecified);
			this._email = obj.Properties.GetValue<string>("Mail", string.Empty);
			this._logOnName = obj.Properties.GetValue<string>("CodeName", string.Empty);
			this._occupation = obj.Properties.GetValue("Occupation", string.Empty);
			this._userRank = obj.Properties.GetValue<UserRankType>("UserRank", UserRankType.Unspecified);
		}

		public PhantomUser()
			: base()
		{
			this.ObjectType = SchemaType.Users;
		}

		public OguObjectCollection<IUser> AllRelativeUserInfo
		{
			get { throw new NotImplementedException(); }
		}

		public UserAttributesType Attributes
		{
			get { return this._attributes; }
		}

		public string Email
		{
			get { return this._email; }
			set { this._email = value; }
		}

		public bool IsChildrenOf(IOrganization parent, bool includeSideline)
		{
			throw new NotImplementedException();
		}

		public bool IsInGroups(params IGroup[] groups)
		{
			throw new NotImplementedException();
		}

		public bool IsSideline
		{
			get { return this._isSideline; }
			set { this._isSideline = value; }
		}

		public string LogOnName
		{
			get { return this._logOnName; }
			set { this._logOnName = value; }
		}

		public OguObjectCollection<IGroup> MemberOf
		{
			get { throw new NotImplementedException(); }
		}

		public string Occupation
		{
			get { return this._occupation; }
			set { this._occupation = value; }
		}

		public UserPermissionCollection Permissions
		{
			get { throw new NotImplementedException(); }
		}

		public UserRankType Rank
		{
			get { return this._userRank; }
			set { this._userRank = value; }
		}

		public UserRoleCollection Roles
		{
			get { throw new NotImplementedException(); }
		}

		public OguObjectCollection<IUser> Secretaries
		{
			get { throw new NotImplementedException(); }
		}

		public OguObjectCollection<IUser> SecretaryOf
		{
			get { throw new NotImplementedException(); }
		}
	}

	/// <summary>
	/// 表示幻象群组
	/// </summary>
	[Serializable]
	internal class PhantomGroup : PhantomOguBase, IGroup
	{
		public PhantomGroup(SchemaObjectBase obj)
			: base(obj)
		{
		}

		public PhantomGroup()
			: base()
		{
			this.ObjectType = SchemaType.Groups;
		}

		public OguObjectCollection<IUser> Members
		{
			get { throw new NotImplementedException(); }
		}
	}

	#endregion

	#region 幻象类型Converter

	internal class PhantomOguConverter : OguObjectConverter
	{
		/// <summary>
		/// 反序列化OguObject
		/// </summary>
		/// <param name="dictionary">对象类型</param>
		/// <param name="type">对象类型</param>
		/// <param name="serializer">JS序列化器</param>
		/// <returns>反序列化出的对象</returns>
		public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
		{
			SchemaType oguType;
			OguBase oguObj;

			this.ExtractOguObject(dictionary, out oguType, out oguObj);

			switch (oguType)
			{
				case SchemaType.Organizations:
					((PhantomOrganization)oguObj).CustomsCode = DictionaryHelper.GetValue(dictionary, "customsCode", string.Empty);
					((PhantomOrganization)oguObj).DepartmentClass = DictionaryHelper.GetValue(dictionary, "departmentClass", DepartmentClassType.Unspecified);
					((PhantomOrganization)oguObj).DepartmentType = DictionaryHelper.GetValue(dictionary, "departmentType", DepartmentTypeDefine.Unspecified);
					((PhantomOrganization)oguObj).Rank = DictionaryHelper.GetValue(dictionary, "rank", DepartmentRankType.None);
					((PhantomOrganization)oguObj).ExcludeVirtualDepartment = DictionaryHelper.GetValue(dictionary, "excludeVirtualDepartment", false);
					break;
				case SchemaType.Users:
					((PhantomUser)oguObj).Email = DictionaryHelper.GetValue(dictionary, "email", string.Empty);
					((PhantomUser)oguObj).IsSideline = DictionaryHelper.GetValue(dictionary, "isSideline", false);
					((PhantomUser)oguObj).LogOnName = DictionaryHelper.GetValue(dictionary, "logOnName", string.Empty);
					((PhantomUser)oguObj).Occupation = DictionaryHelper.GetValue(dictionary, "occupation", string.Empty);
					((PhantomUser)oguObj).Rank = DictionaryHelper.GetValue(dictionary, "rank", UserRankType.Unspecified);
					break;
				case SchemaType.Groups:
					break;
			}

			return oguObj;
		}

		protected override OguBase CreateOguObject(SchemaType oguType, string id)
		{
			switch (oguType)
			{
				case SchemaType.Groups:
					return new PhantomGroup() { ID = id };
				case SchemaType.Organizations:
				case SchemaType.OrganizationsInRole:
					return new PhantomOrganization() { ID = id };
				case SchemaType.Sideline:
				case SchemaType.Users:
					return new PhantomUser() { ID = id };
				default:
					throw new ArgumentOutOfRangeException("oguType");
			}
		}

		private static SchemaType GetObjectSchemaType(IOguObject obj)
		{
			SchemaType type = obj.ObjectType;

			if (type == SchemaType.Unspecified)
			{
				if (obj is IUser)
					type = SchemaType.Users;
				else
					if (obj is IOrganization)
						type = SchemaType.Organizations;
					else
						if (obj is IGroup)
							type = SchemaType.Groups;
			}

			return type;
		}

		/// <summary>
		/// 获取此Converter支持的类型
		/// </summary>
		public override IEnumerable<Type> SupportedTypes
		{
			get
			{
				return new Type[] { typeof(PhantomUser), typeof(PhantomGroup), typeof(PhantomOrganization) };
			}
		}
	}

	#endregion
}

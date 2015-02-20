using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[XElementSerializable]
	public class OguRole : IRole, ISimpleXmlSerializer
	{
		private string fullCodeName = null;
		private string codeName = null;
		private string description = null;
		private string id = null;
		private string name = null;

		[NonSerialized]
		private IRole innerRole = null;

		private IRole InnerRole
		{
			get
			{
				if (this.innerRole == null)
				{
					this.fullCodeName.CheckStringIsNullOrEmpty<InvalidOperationException>("角色代码名称为空");
					string[] parts = this.fullCodeName.Split(':');

					string appCode = parts[0];
					string roleCode = parts[1];

					ApplicationCollection apps = PermissionMechanismFactory.GetMechanism().GetApplications(appCode);

					ExceptionHelper.FalseThrow(apps.Count > 0,
						Translator.Translate(Define.DefaultCulture, "不能在授权系统中找到CodeName为'{0}'的应用", appCode));
					ExceptionHelper.FalseThrow(apps[0].Roles.ContainsKey(roleCode),
						Translator.Translate(Define.DefaultCulture, "不能在授权系统中找到CodeName为'{0}'的角色", roleCode));

					this.innerRole = apps[0].Roles[roleCode];
				}

				return this.innerRole;
			}
		}

		public OguRole()
		{
		}

		public OguRole(IRole role)
		{
			this.innerRole = role;
			this.fullCodeName = role.FullCodeName;
		}

		public OguRole(string key)
		{
			this.FullCodeName = key;
		}

		public string FullCodeName
		{
			get
			{
				return this.fullCodeName;
			}
			set
			{
				ExceptionHelper.FalseThrow<ArgumentException>(Regex.IsMatch(((string)value), @"[a-zA-Z0-9]+:[a-zA-Z0-9]+", RegexOptions.Singleline),
					Translator.Translate(Define.DefaultCulture, "OguRole的Key:{0}不合法，正确格式应为 appCode:roleCode", (string)value));

				this.fullCodeName = value;
			}
		}

		#region IRole Members

		public virtual OguObjectCollection<IOguObject> ObjectsInRole
		{
			get
			{
				return InnerRole.ObjectsInRole;
			}
		}

		#endregion

		#region IApplicationObject Members

		public IApplication Application
		{
			get
			{
				return InnerRole.Application;
			}
		}
		#endregion

		#region IPermissionObject Members

		public string CodeName
		{
			get
			{
				if (this.codeName == null)
					this.codeName = InnerRole.CodeName;

				return this.codeName;
			}
			set
			{
				this.codeName = value;
			}
		}

		public string Description
		{
			get
			{
				if (this.description == null)
					this.description = InnerRole.Description;

				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		public string ID
		{
			get
			{
				if (this.id == null)
					this.id = InnerRole.ID;

				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		public string Name
		{
			get
			{
				if (this.name == null)
					this.name = InnerRole.Name;

				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		#endregion

		/// <summary>
		/// 生成可序列化的包装类
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static IRole CreateWrapperObject(IRole obj)
		{
			IRole result = null;

			if (obj is OguRole || obj == null)
				result = obj;
			else
			{
				result = new OguRole(obj);
			}

			return result;
		}

		#region ISimpleXmlSerializer Members

		void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
		{
			element.NullCheck("element");

			if (refNodeName.IsNotEmpty())
				element = element.AddChildElement(refNodeName);

			element.SetAttributeValue("ID", this.ID);

			if (this.name != null)
				element.SetAttributeValue("Name", this.name);

			if (this.codeName != null)
				element.SetAttributeValue("CodeName", this.codeName);

			if (this.fullCodeName != null)
				element.SetAttributeValue("FullCodeName", this.fullCodeName);
		}

		#endregion
	}

	[Serializable]
	public class OguRoleCollection : OguPermissionObjectCollectionBase<IRole>
	{
		public OguRoleCollection()
		{
		}

		public OguRoleCollection(int capacity)
		{
		}

		public OguRoleCollection(IEnumerable<IRole> roles)
			: base(roles)
		{
		}

		protected override IRole CreateWrapperObject(IRole obj)
		{
			return OguRole.CreateWrapperObject(obj);
		}
	}
}

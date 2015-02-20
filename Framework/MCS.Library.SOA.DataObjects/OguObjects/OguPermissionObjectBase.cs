using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects
{
	/// <summary>
	/// 权限对象的基类
	/// </summary>
	[Serializable]
	public abstract class OguPermissionObjectBase : IPermissionObject, ISimpleXmlSerializer
	{
		private string id = string.Empty;
		private string name = null;
		private string codeName = null;
		private string description = null;

		[NonSerialized]
		private IPermissionObject _PermissionObject = null;

		protected IPermissionObject PermissionObject
		{
			get
			{
				if (this._PermissionObject == null)
					this._PermissionObject = GetPermissionObject();

				return this._PermissionObject;
			}
		}

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
		/// 对象的名称
		/// </summary>
		[XmlObjectMapping]
		public string Name
		{
			get
			{
				if (this.name == null && this.PermissionObject != null)
					this.name = this.PermissionObject.Name;

				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		/// <summary>
		/// 对象的代码名称
		/// </summary>
		[XmlObjectMapping]
		public string CodeName
		{
			get
			{
				if (this.codeName == null && this.PermissionObject != null)
					this.codeName = this.PermissionObject.CodeName;

				return this.codeName;
			}
			set
			{
				this.codeName = value;
			}
		}

		/// <summary>
		/// 对象的描述
		/// </summary>
		[XmlObjectMapping]
		public string Description
		{
			get
			{
				if (this.description == null && this.PermissionObject != null)
					this.description = this.PermissionObject.Description;

				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		protected abstract IPermissionObject GetPermissionObject();

		#region ISimpleXmlSerializer Members

		void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
		{
			element.NullCheck("element");

			if (refNodeName.IsNotEmpty())
				element = element.AddChildElement(refNodeName);

			element.SetAttributeValue("ID", this.ID);

			if (this.Name.IsNotEmpty())
				element.SetAttributeValue("Name", this.Name);
		}

		#endregion
	}
}

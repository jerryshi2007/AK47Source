using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.OGUPermission;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.SOA.DataObjects
{
	[Serializable]
	[XElementSerializable]
	public class OguApplication : OguPermissionObjectBase, IApplication
	{
		private string _ResourceLevel = null;

		[NonSerialized]
		private IApplication _InnerApplication = null;

		public OguApplication()
		{
		}

		public OguApplication(string id)
		{
			this.ID = id;
		}

		public OguApplication(IApplication app)
		{
			this._InnerApplication = app;
		}

		private IApplication InnerApplication
		{
			get
			{
				if (this._InnerApplication == null)
				{
					if (this.ID.IsNotEmpty())
						this._InnerApplication = GetApplicationByID(this.ID);
					else
						if (this.CodeName.IsNotEmpty())
							this._InnerApplication = GetApplicationByCodeName(this.CodeName);
				}

				return this._InnerApplication;
			}
		}

		public PermissionCollection Permissions
		{
			get
			{
				return this.InnerApplication.Permissions;
			}
		}

		public string ResourceLevel
		{
			get
			{
				if (this._ResourceLevel == null && this.InnerApplication != null)
					this._ResourceLevel = this.InnerApplication.ResourceLevel;

				return this._ResourceLevel;
			}
			set
			{
				this._ResourceLevel = value;
			}
		}

		public RoleCollection Roles
		{
			get
			{
				return this.InnerApplication.Roles;
			}
		}

		public static IApplication CreateWrapperObject(IApplication app)
		{
			IApplication result = app;

			if (app is OguApplication || app == null)
				result = app;
			else
				result = new OguApplication(app);

			return result;
		}

		protected override IPermissionObject GetPermissionObject()
		{
			return this.InnerApplication;
		}

		private static IApplication GetApplicationByID(string id)
		{
			ApplicationCollection apps = PermissionMechanismFactory.GetMechanism().GetAllApplications();

			IApplication app = apps.Find(a => string.Compare(a.ID, id, true) == 0);

			ExceptionHelper.FalseThrow(app != null,
				Translator.Translate(Define.DefaultCulture, "不能在授权系统中找到ID为'{0}'的应用", id));

			return app;
		}

		private static IApplication GetApplicationByCodeName(string codeName)
		{
			ApplicationCollection apps = PermissionMechanismFactory.GetMechanism().GetApplications(codeName);

			ExceptionHelper.FalseThrow(apps.Count > 0,
				Translator.Translate(Define.DefaultCulture, "不能在授权系统中找到CodeName为'{0}'的应用", codeName));

			return apps[0];
		}
	}

	[Serializable]
	public class OguApplicationCollection : OguPermissionObjectCollectionBase<IApplication>
	{
		public OguApplicationCollection()
		{
		}

		public OguApplicationCollection(int capacity)
			: base(capacity)
		{
		}

		public OguApplicationCollection(IEnumerable<IApplication> apps)
			: base(apps)
		{
		}

		protected override IApplication CreateWrapperObject(IApplication obj)
		{
			return OguApplication.CreateWrapperObject(obj);
		}
	}
}

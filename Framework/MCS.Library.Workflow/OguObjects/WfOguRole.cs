using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Workflow.Properties;

namespace MCS.Library.Workflow.OguObjects
{
	[Serializable]
	[DebuggerDisplay("CodeName = {codeName}")]
	public class WfOguRole : IRole, ISerializable
	{
		#region Private Fields

        private string fullCodeName = null;
        private string codeName = null;
        private string description = null;
        private string id = null;
        private string name = null;
        private IRole innerRole = null;

        private IRole InnerRole
        {
            get
            {
                if (this.innerRole == null)
                {
					string[] parts = this.fullCodeName.Split(':');

					string appCode = parts[0];
					string roleCode = parts[1];

                    ApplicationCollection apps = PermissionMechanismFactory.GetMechanism().GetApplications(appCode);

					ExceptionHelper.FalseThrow(apps.Count > 0, Resource.CanNotFoundApplicationCodeName, appCode);
					ExceptionHelper.FalseThrow(apps[0].Roles.ContainsKey(roleCode), Resource.CanNotFoundRoleCodeName, codeName);

                    this.innerRole = apps[0].Roles[roleCode];
                }

                return this.innerRole;
            }
        }

		#endregion

		#region Properties

		public string FullCodeName
		{
			get
			{
				return this.fullCodeName;
			}
			private set
			{
				Regex regEx = new Regex("[a-zA-Z0-9]+:[a-zA-Z0-9]+", RegexOptions.Singleline);

				ExceptionHelper.FalseThrow<ArgumentException>(regEx.IsMatch((string)value), string.Format(Resource.InvalidWfOguRoleID, (string)value));

				this.fullCodeName = value;
			}
		}

		#endregion

		#region Constructors

		public WfOguRole(string fcn)
		{
			FullCodeName = fcn;
		}

		public WfOguRole(IRole role)
		{
			this.innerRole = role;
			this.fullCodeName = role.FullCodeName;
		}

		#endregion

		#region Methods

		#endregion

		#region IRole Members

		public OguObjectCollection<IOguObject> ObjectsInRole
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
		}

		public string Description
		{
			get
			{
                if (this.description == null)
                    this.description = InnerRole.Description;

                return this.description;
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
		}

		public string Name
		{
			get
			{
                if (this.name == null)
                    this.name = InnerRole.Name;

                return this.name;
			}
		}

		#endregion

		#region ISerializable Members

		protected WfOguRole(SerializationInfo info, StreamingContext context)
		{
			this.fullCodeName = info.GetString("fullCodeName");
            this.codeName = info.GetString("CodeName");
            this.description = info.GetString("Description");
            this.id = info.GetString("ID");
            this.name = info.GetString("Name");
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("fullCodeName", this.fullCodeName);
			info.AddValue("CodeName", this.codeName);
            info.AddValue("Description", this.description);
            info.AddValue("ID", this.id);
            info.AddValue("Name", this.name);
		}

		#endregion
	}
}

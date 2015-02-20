using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Security.Permissions
{
	[Serializable]
	public class SCAclContainer
	{
		private SCAclMemberCollection _Members = null;

		public SCAclContainer()
		{
		}

		public SCAclContainer(SchemaObjectBase container)
		{
			container.NullCheck("container");

			this.ContainerID = container.ID;
			this.ContainerSchemaType = container.SchemaType;
		}

		public string ContainerID
		{
			get;
			set;
		}

		public string ContainerSchemaType
		{
			get;
			set;
		}

		public DateTime CreateDate
		{
			get;
			set;
		}

		private IUser _Creator = null;

		public IUser Creator
		{
			get
			{
				return this._Creator;
			}

			set
			{
				this._Creator = (IUser)OguBase.CreateWrapperObject(value);
			}
		}

		public SCAclMemberCollection Members
		{
			get
			{
				if (this._Members == null)
					this._Members = new SCAclMemberCollection();

				return this._Members;
			}
		}

		/// <summary>
		/// 将容器的属性复制到成员的共性属性中
		/// </summary>
		public void FillMembersProperties()
		{
			for (int i = 0; i < this.Members.Count; i++)
			{
				SCAclItem member = this.Members[i];

				member.ContainerID = this.ContainerID;
				member.ContainerSchemaType = this.ContainerSchemaType;
				member.SortID = i;
			}
		}
	}
}

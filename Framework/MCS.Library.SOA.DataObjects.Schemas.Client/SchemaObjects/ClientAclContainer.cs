using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Schemas.Client
{
	public class ClientAclContainer
	{
		private ClientAclItemCollection _Members = null;


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

		public ClientAclItemCollection Members
		{
			get
			{
				if (this._Members == null)
					this._Members = new ClientAclItemCollection();

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
				ClientAclItem member = this.Members[i];

				member.ContainerID = this.ContainerID;
				member.ContainerSchemaType = this.ContainerSchemaType;
				member.SortID = i;
			}
		}
	}
}

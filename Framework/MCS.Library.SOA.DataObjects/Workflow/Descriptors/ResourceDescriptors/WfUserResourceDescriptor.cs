using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	[Serializable]
	[XElementSerializable]
	public class WfUserResourceDescriptor : WfResourceDescriptor
	{
		private IUser _User = null;

        public static readonly WfUserResourceDescriptor EmptyInstance = new WfUserResourceDescriptor();

		public WfUserResourceDescriptor()
		{
		}

		public WfUserResourceDescriptor(IUser user)
		{
			this._User = (IUser)OguUser.CreateWrapperObject(user);
		}

		public IUser User
		{
			get { return this._User; }
			set { this._User = (IUser)OguUser.CreateWrapperObject(value); }
		}

		public bool IsSameUser(IUser user)
		{
			bool result = false;

			if (this._User != null && user != null)
			{
				result = string.Compare(this._User.ID, user.ID, true) == 0;
			}

			return result;
		}

		protected internal override void FillUsers(OguDataCollection<IUser> users)
		{
			if (this._User != null)
				users.Add(this._User);
		}

		protected override void ToXElement(XElement element)
		{
			if (this._User != null)
				((ISimpleXmlSerializer)this._User).ToXElement(element, string.Empty);
		}
	}
}

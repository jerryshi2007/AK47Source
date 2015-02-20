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
	public class WfGroupResourceDescriptor :WfResourceDescriptor 
	{
		private IGroup _Group = null;

        public static readonly WfGroupResourceDescriptor EmptyInstance = new WfGroupResourceDescriptor();

		public IGroup Group 
		{
			get { return this._Group; }
			set { this._Group = (IGroup)OguGroup.CreateWrapperObject(value); }
		}

		public WfGroupResourceDescriptor() { }

		public WfGroupResourceDescriptor(IGroup group)
		{
			this._Group = (IGroup)OguGroup.CreateWrapperObject(group);
		}

		protected internal override void FillUsers(OguDataCollection<OGUPermission.IUser> users)
		{
			if (this._Group != null)
			{
				OguObjectCollection<IUser> children = this._Group.Members;

				children.ForEach(u => users.Add(u));
			}
		}

		protected override void ToXElement(XElement element)
		{
			if (this._Group != null)
				((ISimpleXmlSerializer)this._Group).ToXElement(element, string.Empty);
		}
	}
}

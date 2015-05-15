using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Security;
using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PermissionCenter.Handlers
{
    public class OrgTreeNode : TreeNodeBase<OrgTreeNode, OrgTreeNodeCollection>, ITreeNodeSize, INamedTreeNode
    {
        public OrgTreeNode()
        {
        }

        public OrgTreeNode(SCOrganization organization)
        {
            this.ID = organization.ID;
            this.Name = organization.DisplayName.IsNotEmpty() ? organization.DisplayName : organization.Name;
            this.Description = string.Format("包含{0:#,##0}个直接成员", organization.CurrentChildren.Count);
        }

        public string ID
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public int MaxWidth
        {
            get;
            set;
        }

        public int MaxLevel
        {
            get;
            set;
        }
    }

    public class OrgTreeNodeCollection : TreeNodeBaseCollection<OrgTreeNode, OrgTreeNodeCollection>
    {
    }
}
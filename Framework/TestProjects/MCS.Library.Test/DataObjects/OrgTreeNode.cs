using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Data.DataObjects;

namespace MCS.Library.Test.DataObjects
{
    public class OrgTreeNode : TreeNodeBase<OrgTreeNode, OrgTreeNodeCollection>, ITreeNodeSize, INamedTreeNode
    {
        public OrgTreeNode()
        {
        }

        public OrgTreeNode(string name)
        {
            this.Name = name;
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

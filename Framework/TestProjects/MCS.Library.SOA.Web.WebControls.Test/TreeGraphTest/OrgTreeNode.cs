using MCS.Library.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MCS.Library.SOA.Web.WebControls.Test.TreeGraphTest
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

        public static OrgTreeNode PreareOrgTree()
        {
            OrgTreeNode root = new OrgTreeNode("集团总部");

            AppendChildren(root, 21);

            foreach (OrgTreeNode child in root.Children)
            {
                AppendChildren(child, 10);

                foreach (OrgTreeNode cc in child.Children)
                    AppendChildren(cc, 10);
            }

            return root;
        }

        private static void AppendChildren(OrgTreeNode parent, int count)
        {
            for (int i = 0; i < count; i++)
            {
                OrgTreeNode child = new OrgTreeNode("我们的组织成员2015" + i);

                parent.Children.Add(child);
            }
        }

        //public static OrgTreeNode PreareOrgTree()
        //{
        //    OrgTreeNode root = new OrgTreeNode("集团") { Description = "集团总部" };

        //    OrgTreeNode aa = root.Children.Add(new OrgTreeNode("行政部"));

        //    aa.Children.Add(new OrgTreeNode("办公室"));

        //    OrgTreeNode finance = root.Children.Add(new OrgTreeNode("财务部"));

        //    OrgTreeNode accountant = finance.Children.Add(new OrgTreeNode("会计"));

        //    accountant.Children.Add(new OrgTreeNode("审核会计"));

        //    finance.Children.Add(new OrgTreeNode("出纳"));

        //    OrgTreeNode hr = root.Children.Add(new OrgTreeNode("人事部"));

        //    hr.Children.Add(new OrgTreeNode("入职专员"));
        //    hr.Children.Add(new OrgTreeNode("福利专员"));
        //    hr.Children.Add(new OrgTreeNode("薪酬专员"));

        //    return root;
        //}
    }

    public class OrgTreeNodeCollection : TreeNodeBaseCollection<OrgTreeNode, OrgTreeNodeCollection>
    {
    }
}
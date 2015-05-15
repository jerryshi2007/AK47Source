using MCS.Library.Data.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MCS.Web.Responsive.WebControls.Test.TreeGraphTest
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
            OrgTreeNode root = new OrgTreeNode("集团");

            AppendChildren(root);

            foreach (OrgTreeNode child in root.Children)
            {
                AppendChildren(child);
            }

            return root;
        }

        private static void AppendChildren(OrgTreeNode parent)
        {
            for (int i = 0; i < 20; i++)
            {
                OrgTreeNode child = new OrgTreeNode("组织" + i);

                parent.Children.Add(child);
            }
        }

        //public static OrgTreeNode PreareOrgTree()
        //{
        //    OrgTreeNode root = new OrgTreeNode("集团");

        //    OrgTreeNode aa = root.Children.Add(new OrgTreeNode("行政部"));

        //    aa.Children.Add(new OrgTreeNode("办公室"));

        //    OrgTreeNode finance = root.Children.Add(new OrgTreeNode("财务部"));

        //    OrgTreeNode accountant = finance.Children.Add(new OrgTreeNode("会计"));

        //    accountant.Children.Add(new OrgTreeNode("审核会计"));

        //    finance.Children.Add(new OrgTreeNode("出纳"));

        //    OrgTreeNode hr = root.Children.Add(new OrgTreeNode("人事部"));

        //    hr.Children.Add(new OrgTreeNode("入职专员"));

        //    OrgTreeNode fl = new OrgTreeNode("福利专员");
        //    hr.Children.Add(fl);
        //    hr.Children.Add(new OrgTreeNode("薪酬专员"));

        //    for (int i = 0; i < 32; i++)
        //        hr.Children.Add(new OrgTreeNode("组织" + i));

        //    for (int i = 0; i < 100; i++)
        //        fl.Children.Add(new OrgTreeNode("组织" + i));

        //    return root;
        //}
    }

    public class OrgTreeNodeCollection : TreeNodeBaseCollection<OrgTreeNode, OrgTreeNodeCollection>
    {
    }
}
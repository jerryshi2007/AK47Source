using MCS.Library.Data.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Test.DataObjects
{
    [TestClass]
    public class TreeGraphGeneratorTest
    {
        [TestMethod]
        public void CalculateOrgTreeSize()
        {
            OrgTreeNode root = PreareOrgTree();

            root.CalculateMaxWidth();
            root.CalculateMaxLevel();

            OutputOrgTreehSizeRecursively(root);
        }

        [TestMethod]
        public void GenerateAllGraphNodes()
        {
            OrgTreeNode root = PreareOrgTree();

            TreeGraph graph = root.GenerateGraph();

            OutputTreeGraph(graph);
        }

        private static OrgTreeNode PreareOrgTree()
        {
            OrgTreeNode root = new OrgTreeNode("集团");

            OrgTreeNode aa = root.Children.Add(new OrgTreeNode("行政部"));

            aa.Children.Add(new OrgTreeNode("办公室"));

            OrgTreeNode finance = root.Children.Add(new OrgTreeNode("财务部"));

            OrgTreeNode accountant = finance.Children.Add(new OrgTreeNode("会计"));

            accountant.Children.Add(new OrgTreeNode("审核会计"));

            finance.Children.Add(new OrgTreeNode("出纳"));

            OrgTreeNode hr = root.Children.Add(new OrgTreeNode("人事部"));

            hr.Children.Add(new OrgTreeNode("入职专员"));
            hr.Children.Add(new OrgTreeNode("福利专员"));
            hr.Children.Add(new OrgTreeNode("薪酬专员"));

            return root;
        }

        private static void OutputTreeGraph(TreeGraph graph)
        {
            Console.WriteLine("Width: {0}, Height: {1}", graph.Width, graph.Height);

            OutputTreeGraphRecursively(graph.Root);
        }

        private static void OutputTreeGraphRecursively(TreeGraphNodeBase graphNode)
        {
            OutputTreeGraphNode(graphNode);

            foreach (TreeGraphNodeBase child in graphNode.Children)
                OutputTreeGraphRecursively(child);
        }

        private static void OutputOrgTreehSizeRecursively(OrgTreeNode node)
        {
            OutputOrgTreeNodeSize(node);

            foreach (OrgTreeNode child in node.Children)
                OutputOrgTreehSizeRecursively(child);
        }

        private static void OutputOrgTreeNodeSize(OrgTreeNode node)
        {
            if (node is ITreeNodeSize)
                Console.WriteLine("Name = {0}, MaxWidth = {1}, MaxLevel = {2} ", node.Name, node.MaxWidth, node.MaxLevel);
            else
                Console.WriteLine("Name = {0}, MaxWidth = {1}, MaxLevel = {2} ", node.Name, node.CalculateMaxWidth(), node.CalculateMaxLevel());
        }

        private static void OutputTreeGraphNode(TreeGraphNodeBase graphNode)
        {
            if (graphNode.NodeType == TreeGraphNodeType.Label)
                Console.WriteLine("Name = {0}, X = {1}, Y = {2} ", ((TreeGraphLabelNode)graphNode).Name, graphNode.X, graphNode.Y);
            else
                Console.WriteLine("Type = {0}, X = {1}, Y = {2} ", graphNode.NodeType, graphNode.X, graphNode.Y);
        }
    }
}

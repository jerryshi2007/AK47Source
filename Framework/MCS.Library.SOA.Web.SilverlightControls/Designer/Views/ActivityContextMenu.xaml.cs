using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Northwoods.GoXam;
using Designer.Utils;
using Designer.Models;

namespace Designer.Views
{
    public partial class ActivityContextMenu : UserControl
    {
        public ActivityContextMenu()
        {
            InitializeComponent();
        }

        private void btnCondition_Click(object sender, RoutedEventArgs e)
        {
            if (!DiagramUtils.IsMainDiagram(sender as UIElement)) return;

            DiagramUtils.WebInterAct.OpenEditor(sender, EditorType.Condition);
        }

        private void btnResource_Click(object sender, RoutedEventArgs e)
        {
            if (!DiagramUtils.IsMainDiagram(sender as UIElement)) return;

            DiagramUtils.WebInterAct.OpenEditor(sender, EditorType.Resource);
        }

        private void btnRelativeLink_Click(object sender, RoutedEventArgs e)
        {
            if (!DiagramUtils.IsMainDiagram(sender as UIElement)) return;

            DiagramUtils.WebInterAct.OpenEditor(sender, EditorType.RelativeLink);
        }

        private void btnBranchProcess_Click(object sender, RoutedEventArgs e)
        {
            if (!DiagramUtils.IsMainDiagram(sender as UIElement)) return;

            DiagramUtils.WebInterAct.OpenEditor(sender, EditorType.BranchProcess);
        }

        private void btnEnterReceivers_Click(object sender, RoutedEventArgs e)
        {
            if (!DiagramUtils.IsMainDiagram(sender as UIElement)) return;

            DiagramUtils.WebInterAct.OpenEditor(sender, EditorType.EnterReceivers);
        }

        private void btnLeaveReceivers_Click(object sender, RoutedEventArgs e)
        {
            if (!DiagramUtils.IsMainDiagram(sender as UIElement)) return;

            DiagramUtils.WebInterAct.OpenEditor(sender, EditorType.LeaveReceivers);
        }

        private void btnActivityTempalte_Click(object sender, RoutedEventArgs e)
        {
            var element = sender as UIElement;
            if (!DiagramUtils.IsMainDiagram(element)) return;

            var diagram = Part.FindAncestor<Diagram>(element);
            if (diagram == null) return;


            var nodeData = diagram.SelectedNode.Data as ActivityNode;

            if (nodeData == null) return;
            if (nodeData.Category != ActivityType.Normal.ToString())
            {
                MessageBox.Show("只能为Normal节点设置模板");
                return;
            }

            string templateID = Guid.NewGuid().ToString();
            //保存到数据库
            DiagramUtils.WebInterAct.SaveActivityTemplate(templateID);
            //保存到palette中
            DiagramUtils.AddActivityTemplate(diagram, nodeData.WfClone(templateID));
            DiagramUtils.GetTemplateKeys(diagram).Add(templateID);
        }

        private void btnVariable_Click(object sender, RoutedEventArgs e)
        {
            if (!DiagramUtils.IsMainDiagram(sender as UIElement)) return;

            DiagramUtils.WebInterAct.OpenEditor(sender, EditorType.Variables);
        }

        private void btnInternalUser_Click(object sender, RoutedEventArgs e)
        {
            if (!DiagramUtils.IsMainDiagram(sender as UIElement)) return;

            DiagramUtils.WebInterAct.OpenEditor(sender, EditorType.InternalUsers);
        }

        private void btnExternalUser_Click(object sender, RoutedEventArgs e)
        {
            if (!DiagramUtils.IsMainDiagram(sender as UIElement)) return;

            DiagramUtils.WebInterAct.OpenEditor(sender, EditorType.ExternalUsers);
        }

        private void btnWfMatrix_Click(object sender, RoutedEventArgs e)
        {
            if (!DiagramUtils.IsMainDiagram(sender as UIElement)) return;

            DiagramUtils.WebInterAct.OpenEditor(sender, EditorType.ImportWfMatrix);
        }

        private void btnLeaveService_Click(object sender, RoutedEventArgs e)
        {
            if (!DiagramUtils.IsMainDiagram(sender as UIElement)) return;

            DiagramUtils.WebInterAct.OpenEditor(sender, EditorType.LeaveService);
        }

        private void btnEnterService_Click(object sender, RoutedEventArgs e)
        {
            if (!DiagramUtils.IsMainDiagram(sender as UIElement)) return;

            DiagramUtils.WebInterAct.OpenEditor(sender, EditorType.EnterService);
        }

        private void btnParameters_Click(object sender, RoutedEventArgs e)
        {
            if (!DiagramUtils.IsMainDiagram(sender as UIElement)) return;

            DiagramUtils.WebInterAct.OpenEditor(sender, EditorType.ParametersNeedToBeCollected);
        }

    }
}

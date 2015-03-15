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
using System.Windows.Navigation;
using Designer.Utils;

namespace Designer.Views
{
	public partial class WfContextMenu : UserControl
	{
		public WfContextMenu()
		{
			InitializeComponent();
		}

		private void btnRelativeLink_Click(object sender, RoutedEventArgs e)
		{
			if (!DiagramUtils.IsMainDiagram(sender as UIElement)) return;

			DiagramUtils.WebInterAct.OpenEditor(sender, EditorType.RelativeLink);
		}

		private void btnCancelReceivers_Click(object sender, RoutedEventArgs e)
		{
			if (!DiagramUtils.IsMainDiagram(sender as UIElement)) return;

			DiagramUtils.WebInterAct.OpenEditor(sender, EditorType.CancelReceivers);
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

        private void btnParameters_Click(object sender, RoutedEventArgs e)
        {
            if (!DiagramUtils.IsMainDiagram(sender as UIElement)) return;

            DiagramUtils.WebInterAct.OpenEditor(sender, EditorType.ParametersNeedToBeCollected);
        }

	}
}

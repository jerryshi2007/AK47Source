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
using System.Windows.Data;

namespace WorkflowRuntime.Views
{
	public partial class BranchProcessPage : UserControl
	{
		public BranchProcessPage()
		{
			InitializeComponent();
		}

		private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
		{
			var column = this.branchProcGrid.CurrentColumn;
			var item = this.branchProcGrid.SelectedItem;
		}
	}

	public class Brach
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Category { get; set; }

		public static List<Brach> All()
		{
			List<Brach> result = new List<Brach>();
			string category = "A";
			for (int i = 0; i < 10; i++)
			{
				if (i % 2 == 0)
				{
					category = "B";
				}
				else
				{
					category = "A";
				}
				result.Add(new Brach()
				{
					ID = i,
					Name = category + i.ToString(),
					Category = category
				});
			}
			return result;
		}
	}
}

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

namespace WorkflowRuntime.Views
{
	public partial class ErrorWindow : ChildWindow
	{
		public ErrorWindow(string message, string fullStackTrace)
		{
			InitializeComponent();
			this.IntroductoryText.Text = message;
			this.ErrorTextBox.Text = fullStackTrace;
		}

		public static void CreateNew(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException("exception");
			}

			string fullStackTrace = exception.StackTrace;

			// Account for nested exceptions
			Exception innerException = exception.InnerException;
			while (innerException != null)
			{
				fullStackTrace += "\nCaused by: " + exception.Message + "\n\n" + exception.StackTrace;
				innerException = innerException.InnerException;
			}

			ErrorWindow window = new ErrorWindow(exception.Message, fullStackTrace);
			window.Show();
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}
	}
}


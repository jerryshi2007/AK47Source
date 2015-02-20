using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Designer.Models
{
	public class TransitionInfo
	{
		public string Key { get; set; }
		public string Name { get; set; }
		public bool Enabled { get; set; }
		public string FromActivityKey { get; set; }
		public string ToActivityKey { get; set; }
		public bool IsReturn { get; set; }
	}
}

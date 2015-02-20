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
	public interface IWfInfo
	{
		WfInfoType InfoType { get; }
	}

	public enum WfInfoType
	{
		Workflow = 0,
		Activity = 1,
		Transition = 2
	}
}

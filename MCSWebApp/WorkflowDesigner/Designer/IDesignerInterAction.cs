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
using System.Collections.Generic;
using Designer.Models;

namespace Designer
{
	public interface IDesignerInterAction
	{
		void CreateNewWorkflow(string key);
		void OpenWorkflow(string wfJsonStr);
		void LayoutCurrentDiagram();
		void UpdateDiagramData(string upType, string tagKey, string name, string value);
		string GetWorkflowGraph(string wfKey);
		/// <summary>
		/// 
		/// </summary>
		/// <returns>返回移除的模板ID，格式  id1,id2,id3</returns>
		string RemoveActivityTemplate();

		void AddActivitySelfLink();
	}
}

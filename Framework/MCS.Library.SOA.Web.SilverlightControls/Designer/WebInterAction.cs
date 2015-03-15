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
using System.Windows.Browser;
using Newtonsoft.Json;
using Designer.Models;
using System.Collections.Generic;
using Designer.Utils;

namespace Designer
{
	public class WebInterAction : IWebInterAction
	{
		public void LoadProperty(string loadType, string proKey, string childKey)
		{
			HtmlPage.Window.Invoke("LoadProperty", loadType, proKey, childKey);
		}


		public void UpdateProcess(string exType, string proKey, string childKey, string jsonInfo)
		{
			HtmlPage.Window.Invoke("UpdateProcess", exType, proKey, childKey, jsonInfo);
		}


		public void DeleteProcess(string delType, string procKey, string childKey)
		{
			HtmlPage.Window.Invoke("DeleteProcess", delType, procKey, childKey);
		}

		public void OpenEditor(object sender, EditorType type)
		{
			HtmlPage.Window.Invoke("OpenEditor", type.ToString());
		}

		public List<ActivityInfo> LoadActivityTemplate()
		{
			List<ActivityInfo> result = new List<ActivityInfo>();

			var jsonInfo = HtmlPage.Window.Invoke("LoadActivityTemplate");

			if (jsonInfo != null)
			{
				result = JsonConvert.DeserializeObject<List<ActivityInfo>>(jsonInfo.ToString());
			}

			return result;
		}

		public void SaveActivityTemplate(string templateID)
		{
			HtmlPage.Window.Invoke("SaveActivityTemplate", templateID);
		}

		public void LoadProcessInstanceDescription()
		{
			HtmlPage.Window.Invoke("LoadProcessInstanceDescription");
		}
	}
}

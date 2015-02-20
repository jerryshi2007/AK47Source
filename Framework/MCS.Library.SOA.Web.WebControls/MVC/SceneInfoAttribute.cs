using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.Library.MVC
{
	/// <summary>
	/// 定义了场景信息的属性
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class SceneInfoAttribute : Attribute
	{
		public string SceneFileVirtualPath { get; set; }
		public string ActID { get; set; }
		public string ReadOnlySceneID { get; set; }
		public string DefaultWorkflowSceneID { get; set; }

		public SceneInfoAttribute(string filePath, string actID)
		{
			this.SceneFileVirtualPath = filePath;
			this.ActID = actID;
		}
	}
}

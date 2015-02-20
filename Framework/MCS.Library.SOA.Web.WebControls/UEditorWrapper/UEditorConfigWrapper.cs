using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// UEditor组件配置信息的包装类，这里面没有封装所有的属性，仅仅是部分常用的。
	/// </summary>
	internal class UEditorConfigWrapper
	{
		public string ImagePath { get; set; }
		public int CompressSide { get; set; }
		public int MaxImageSideLength { get; set; }
		public bool RelativePath { get; set; }
		public string CatcherUrl { get; set; }
		public string UEDITOR_HOME_URL { get; set; }
		public string[] Toolbars { get; set; }
		public string InitialContent { get; set; }
		public bool AutoClearInitialContent { get; set; }
		public bool PastePlain { get; set; }
		public string TextArea { get; set; }
		public bool AutoHeightEnabled { get; set; }
		public bool ElementPathEnabled { get; set; }
		public bool WordCount { get; set; }
		public int MaximumWords { get; set; }
		public string HighlightJsUrl { get; set; }
		public string HighlightCssUrl { get; set; }
	}
}

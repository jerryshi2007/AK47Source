using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 支持页分析器生成<see cref="RelaxedTabStrip"/>控件及子控件。
	/// </summary>
	public class RelaxedTabStripControlBuilder : ControlBuilder
	{
		/// <summary>
		/// 为属于容器控件的任何子控件将生成器添加到 <see cref="System.Web.UI.ControlBuilder"/> 对象。
		/// </summary>
		/// <param name="subBuilder">分配给子控件的 <see cref="System.Web.UI.ControlBuilder"/> 对象。</param>
		public override void AppendSubBuilder(ControlBuilder subBuilder)
		{
			base.AppendSubBuilder(subBuilder);
		}
	}
}
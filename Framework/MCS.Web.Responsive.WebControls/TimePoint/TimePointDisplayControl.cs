using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Globalization;

namespace MCS.Web.Responsive.WebControls
{
	/// <summary>
	/// 显示当前的TimePoint。如果使用当前时间，则不渲染
	/// </summary>
	[DefaultProperty("Text")]
	[ToolboxData("<{0}:TimePointDisplayControl runat=server></{0}:TimePointDisplayControl>")]
	public class TimePointDisplayControl : Control
	{
		/// <summary>
		/// 渲染方法
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(HtmlTextWriter writer)
		{
			if (this.DesignMode)
			{
				writer.Write("显示模拟时间");
			}
			else
			{
				CultureInfo culture = Thread.CurrentThread.CurrentUICulture;
				string dateFormat = "yyyy-MM-dd HH:mm:ss";

				if (culture != null)
					dateFormat = culture.DateTimeFormat.FullDateTimePattern;

				if (TimePointContext.Current.UseCurrentTime == false)
					writer.Write(string.Format("{0}:{1}",
						Translator.Translate(Define.DefaultCategory, "模拟时间"),
						TimePointContext.Current.SimulatedTime.ToString(dateFormat)));
			}
		}
	}
}

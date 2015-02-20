using System;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Web.Controls;
using MCS.OA.Portal.Common;
using MCS.Web.Library;

namespace MCS.OA.Portal
{
	/// <summary>
	/// 列表上一些公用的操作
	/// </summary>
	internal static class GridCommon
	{
		/// <summary>
		/// 设置当鼠标移动到数据行时的样式
		/// </summary>
		/// <param name="row"></param>
		internal static void SetRowStyleWhenMouseOver(GridViewRow row)
		{
			row.Attributes["onmouseover"] = "this.className = 'selecteditem';";
			if (row.RowState == DataControlRowState.Alternate)
			{
				row.Attributes["onmouseout"] = "this.className = 'aitem';";
			}
			else
			{
				row.Attributes["onmouseout"] = "this.className = 'item';";
			}
		}
		/// <summary>
		/// 设置当鼠标移动到数据行时的样式
		/// </summary>
		/// <param name="row"></param>
		internal static void SetRowStyleWhenMouseOver(GridViewRow row, string selectedCss, string alternatingRowCss, string rowCss)
		{
			row.Attributes["onmouseover"] = string.Format("this.className = '{0}';", selectedCss);
			if (row.RowState == DataControlRowState.Alternate)
			{
				row.Attributes["onmouseout"] = string.Format("this.className = '{0}';", alternatingRowCss);
			}
			else
			{
				row.Attributes["onmouseout"] = string.Format("this.className = '{0}';", rowCss);
			}
		}
		/// <summary>
		/// 设置未读条目样式
		/// </summary>
		/// <param name="row"></param>
		internal static void SetUnreadItemBold(GridViewRow row)
		{
			//由于ORMMapping入库时会把NULL或DataTime.MinValue存成NULL，如果直接从库取值再赋值，会把READ_TIME置成1900-01-01 00:00:00.000
			//为了防止其他意外情况发生，这里再加一个1900年的判断
			//沈峥说不用，这个责任该归到写的人身上，于是去掉了--080516
			//if (((UserTask)row.DataItem).ReadTime < new DateTime(1949, 10, 1, 0, 0, 0))
			if (((UserTask)row.DataItem).ReadTime == DateTime.MinValue)
			{
				row.Style["font-weight"] = "bold";
			}
		}

		/// <summary>
		/// 将为DateTime.MinValue的时间显示为空，有效的时间不显示秒，鼠标移至时显示全时间。
		/// </summary>
		/// <param name="time"></param>
		/// <returns></returns>
		internal static string TimeDisplayFormat(DateTime time)
		{
			string display = string.Empty;
			if (time != DateTime.MinValue)
			{
				display = string.Format("<span title=\"{0}\">{1}</span>",
					time.ToString("yyyy-MM-dd HH:mm:ss"), time.ToString("yyyy-MM-dd HH:mm"));
			}

			return display;
		}

		/// <summary>
		/// 获取缓急程度列展示的内容
		/// </summary>
		/// <param name="emergency">缓急程度</param>
		/// <returns>展示缓急程度图片的Html代码</returns>
		internal static string GetEmergencyImageHtml(FileEmergency emergency)
		{
			string result = string.Empty;
			int imageNum = EnumItemDescriptionAttribute.GetAttribute(emergency).ShortName.Length;

			//根据该枚举属性的感叹号数量放等量的感叹号图
			for (int i = 0; i < imageNum - 1; i++)
			{
				result += "<img src=\"../images/import.gif\" alt=\"{0}\"/>";
			}

			if (imageNum > 0)
			{
				result = string.Format(result, EnumItemDescriptionAttribute.GetAttribute(emergency).Description);
			}

			return result;
		}

		/// <summary>
		/// 设置置顶项目样式
		/// </summary>
		/// <param name="row"></param>
		internal static void HighlightTopItem(GridViewRow row)
		{
			if (1 == ((UserTask)row.DataItem).TopFlag)
			{
				row.CssClass = "itemattop";
				row.Attributes["onmouseout"] = string.Format("this.className = 'itemattop';");
			}
		}

		/// <summary>
		/// 获取消息的标题链接
		/// </summary>
		/// <param name="task">消息实例</param>
		/// <returns>html格式的消息标题</returns>
		internal static string GetTaskURL(UserTask task)
		{
			string url = task.NormalizedUrl;

			string defaultTarget = WebUtility.GetRequestQueryString("defaultTarget", "_blank");

			return String.Format("<a href=\"{0}\" taskid=\"{1}\" unreadflag=\"{2}\" onclick=\"onTaskLinkClick(this.href,'{3}');\" target=\"{4}\">{5}</a>",
				url,
				task.TaskID,
				task.ReadTime == DateTime.MinValue,
				GetFeature(task),
				defaultTarget,
				HttpUtility.HtmlEncode(task.TaskTitle).ToString().Replace(" ", "&nbsp;"));
		}

        ///// <summary>
        ///// 转义like有关的通配符
        ///// </summary>
        ///// <param name="condition">原查询条件</param>
        ///// <returns>转义后的查询条件</returns>
        //internal static string EscapeLikeString(string condition)
        //{
        //    string result = condition;

        //    result = result.Replace("[", "[[]");
        //    result = result.Replace("-", "[-]");
        //    result = result.Replace("_", "[_]");
        //    result = result.Replace("%", "[%]");

        //    return result;
        //}

		/// <summary>
		/// 获取应用对应的窗口控制代码
		/// </summary>
		/// <param name="task">消息对象</param>
		/// <returns>用于JS的弹出窗口控制代码</returns>
		internal static string GetFeature(UserTask task)
		{
			string appName = task.ApplicationName;
			string feature = string.Empty;
			string featureID = string.Empty;
			//判断消息级别，同时消息状态是阅
			//老系统中的url会自动添加 http:// 字符串，新系统中没有
			if ((task.Level < TaskLevel.Normal && task.Status == TaskStatus.Yue && !task.Url.StartsWith("http://"))
				|| task.Url.StartsWith("/MCSWebApp/MCSOAPortal/TaskList/NoticeTaskDetail.aspx"))
			{
				if (ResourceUriSettings.GetConfig().Features.ContainsKey("MessageRemind"))
				{
					feature = ResourceUriSettings.GetConfig().Features["MessageRemind"].Feature.ToWindowFeatureClientString();
				}
				else
				{
					feature = "width=800,height=600,left=' + ((window.screen.width - 800) / 2) + ',top=' + ((window.screen.height - 600) / 2) + ',resizable=yes,scrollbars=yes,toolbar=no,location=no";
				}
			}
			else
			{ //Note:获取js的窗口形态（待办）ApplicationInfoConfig.GetConfig().Applications.ContainsKey(appName) ? ApplicationInfoConfig.GetConfig().Applications[appName].FeatureID : string.Empty;
				feature = "width=800,height=600,left=' + ((window.screen.width - 800) / 2) + ',top=' + ((window.screen.height - 600) / 2) + ',resizable=yes,scrollbars=yes,toolbar=no,location=no";
			}
			return feature;
		}
	}
}

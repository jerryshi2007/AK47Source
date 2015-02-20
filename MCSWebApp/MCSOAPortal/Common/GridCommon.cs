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
	/// �б���һЩ���õĲ���
	/// </summary>
	internal static class GridCommon
	{
		/// <summary>
		/// ���õ�����ƶ���������ʱ����ʽ
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
		/// ���õ�����ƶ���������ʱ����ʽ
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
		/// ����δ����Ŀ��ʽ
		/// </summary>
		/// <param name="row"></param>
		internal static void SetUnreadItemBold(GridViewRow row)
		{
			//����ORMMapping���ʱ���NULL��DataTime.MinValue���NULL�����ֱ�Ӵӿ�ȡֵ�ٸ�ֵ�����READ_TIME�ó�1900-01-01 00:00:00.000
			//Ϊ�˷�ֹ����������������������ټ�һ��1900����ж�
			//���˵���ã�������θù鵽д�������ϣ�����ȥ����--080516
			//if (((UserTask)row.DataItem).ReadTime < new DateTime(1949, 10, 1, 0, 0, 0))
			if (((UserTask)row.DataItem).ReadTime == DateTime.MinValue)
			{
				row.Style["font-weight"] = "bold";
			}
		}

		/// <summary>
		/// ��ΪDateTime.MinValue��ʱ����ʾΪ�գ���Ч��ʱ�䲻��ʾ�룬�������ʱ��ʾȫʱ�䡣
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
		/// ��ȡ�����̶���չʾ������
		/// </summary>
		/// <param name="emergency">�����̶�</param>
		/// <returns>չʾ�����̶�ͼƬ��Html����</returns>
		internal static string GetEmergencyImageHtml(FileEmergency emergency)
		{
			string result = string.Empty;
			int imageNum = EnumItemDescriptionAttribute.GetAttribute(emergency).ShortName.Length;

			//���ݸ�ö�����Եĸ�̾�������ŵ����ĸ�̾��ͼ
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
		/// �����ö���Ŀ��ʽ
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
		/// ��ȡ��Ϣ�ı�������
		/// </summary>
		/// <param name="task">��Ϣʵ��</param>
		/// <returns>html��ʽ����Ϣ����</returns>
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
        ///// ת��like�йص�ͨ���
        ///// </summary>
        ///// <param name="condition">ԭ��ѯ����</param>
        ///// <returns>ת���Ĳ�ѯ����</returns>
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
		/// ��ȡӦ�ö�Ӧ�Ĵ��ڿ��ƴ���
		/// </summary>
		/// <param name="task">��Ϣ����</param>
		/// <returns>����JS�ĵ������ڿ��ƴ���</returns>
		internal static string GetFeature(UserTask task)
		{
			string appName = task.ApplicationName;
			string feature = string.Empty;
			string featureID = string.Empty;
			//�ж���Ϣ����ͬʱ��Ϣ״̬����
			//��ϵͳ�е�url���Զ���� http:// �ַ�������ϵͳ��û��
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
			{ //Note:��ȡjs�Ĵ�����̬�����죩ApplicationInfoConfig.GetConfig().Applications.ContainsKey(appName) ? ApplicationInfoConfig.GetConfig().Applications[appName].FeatureID : string.Empty;
				feature = "width=800,height=600,left=' + ((window.screen.width - 800) / 2) + ',top=' + ((window.screen.height - 600) / 2) + ',resizable=yes,scrollbars=yes,toolbar=no,location=no";
			}
			return feature;
		}
	}
}

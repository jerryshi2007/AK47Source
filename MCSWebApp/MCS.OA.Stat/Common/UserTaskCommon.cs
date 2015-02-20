using System;
using System.Web;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;
using MCS.Library.Core;
using MCS.Web.Library;

namespace MCS.OA.Stat.Common
{
    /// <summary>
    /// �б���һЩ���õĲ���
    /// </summary>
    internal static class UserTaskCommon
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

            return String.Format("<a class=\"textover\" href=\"#\" taskid=\"{1}\" unreadflag=\"{2}\" onclick=\"onTaskLinkClick('{0}','{3}');\" title=\"{4}\">{4}</a>",
                url,
                task.TaskID,
                task.ReadTime == DateTime.MinValue,
                GetFeature(task),
                HttpUtility.HtmlEncode(task.TaskTitle).ToString().Replace(" ", "&nbsp;"));
        }

        /// <summary>
        /// ת��like�йص�ͨ���
        /// </summary>
        /// <param name="condition">ԭ��ѯ����</param>
        /// <returns>ת���Ĳ�ѯ����</returns>
        internal static string EscapeLikeString(string condition)
        {
            string result = condition;

            result = result.Replace("[", "[[]");
            result = result.Replace("-", "[-]");
            result = result.Replace("_", "[_]");
            result = result.Replace("%", "[%]");

            return result;
        }

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
                || task.Url.StartsWith("/SinoOceanWebApp/SinoOceanOAPortal/TaskList/NoticeTaskDetail.aspx"))
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
            {
                featureID = string.Empty; //ApplicationInfoConfig.GetConfig().Applications.ContainsKey(appName) ? ApplicationInfoConfig.GetConfig().Applications[appName].FeatureID : string.Empty;
                if (task.Url.StartsWith("http://"))
                {
                    switch (appName)
                    {
                        case "Cooperation":
                            featureID = "shouwendengji";
                            break;
                        case "SHOUWEN":
                            featureID = "shouwendengji";
                            break;
                        case "GW_FAWEN":
                            featureID = "oldfawen";
                            break;
                        case "GW_QIANBAO":
                            featureID = "oldqianbao";
                            break;
                        case "ZS_DUBAN":
                            featureID = "shouwendengji";
                            break;
                    }
                }

                if (string.IsNullOrEmpty(featureID) == false)
                {
                    if (ResourceUriSettings.GetConfig().Features.ContainsKey(featureID))
                        feature = ResourceUriSettings.GetConfig().Features[featureID].Feature.ToWindowFeatureClientString();
                }
            }
            return feature;
        }
    }
}

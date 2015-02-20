using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Text;

namespace CheckCompileStatus
{
    public class MailHelper
    {
        private string smtpServer;
        private int smtpServerPort;
        private string userName;
        private string password;
        private string domain;
        private string adServer;

        public MailHelper(string smtpServer, int smtpServerPort, string userName, string password, string domain, string adServer)
        {
            this.smtpServer = smtpServer;
            this.smtpServerPort = smtpServerPort;
            this.userName = userName;
            this.password = password;
            this.domain = domain;
            this.adServer = adServer;
        }

        [SecurityCritical]
        public void Send(string mailFrom, string mailCC, string workspaceServerPath, DateTime startTime, DateTime endTime, string timeElapsed, Collection<CompileResult> errors, Collection<CompileResult> warnings)
        {
            ADHelper adHelper = new ADHelper(userName, password, adServer);

            string bodyHtml = MailHelper.GetBodyHtml();

            List<string> mailTo = new List<string>();
            List<string> displayNames = new List<string>();

            string mailSubject = "【待解决】 编译问题";
            string descTD = "<td width=\"60\"><p align=\"center\" style=\"text-align: center\"><span class=\"desc\">{0}</span></p></td>";
            string detailTDBold = "<td width=\"800\"><p align=\"left\" style=\"text-align: left\"><span class=\"detail\" style=\"font-weight: bold\">{0}</span></p></td>";
            string detailTD = "<td width=\"800\"><p align=\"left\" style=\"text-align: left\"><span class=\"detail\">{0}</span></p></td>";

            #region  errorBody
            if (errors != null && errors.Count > 0)
            {
                StringBuilder errorBody = new StringBuilder();
                errorBody.Append("<br /><div style=\"margin-left: .5in\"><span class=\"errorTitle\">编译错误：</span>");
                foreach (var error in errors)
                {
                    mailTo.Add(error.OwnerMail);
                    errorBody.Append("<table border=\"1\">");
                    errorBody.Append("<tr>");
                    errorBody.AppendFormat(descTD, "责任人");
                    error.OwnerDisplayName = adHelper.GetUserDisplayName(error.OwnerName);
                    displayNames.Add(error.OwnerDisplayName);
                    errorBody.AppendFormat(detailTDBold, string.Format(CultureInfo.InvariantCulture, "{0}({1})", error.OwnerDisplayName, error.OwnerName));
                    errorBody.Append("</tr>");
                    errorBody.Append("<tr>");
                    errorBody.AppendFormat(descTD, "签入时间");
                    errorBody.AppendFormat(detailTD, error.CreationDate);
                    errorBody.Append("</tr>");
                    errorBody.Append("<tr>");
                    errorBody.AppendFormat(descTD, "文件位置");
                    errorBody.AppendFormat(detailTD, error.ServerPath);
                    errorBody.Append("</tr>");
                    errorBody.Append("<tr>");
                    errorBody.AppendFormat(descTD, "详细信息");
                    errorBody.AppendFormat(detailTD, error.CompileDetail);
                    errorBody.Append("</tr>");
                    errorBody.Append("</table>");
                }
                errorBody.Append("</div>");
                bodyHtml = bodyHtml.Replace("{%errorBody%}", errorBody.ToString());
            }
            else
            {
                bodyHtml = bodyHtml.Replace("{%errorBody%}", string.Empty);
            }
            #endregion

            #region warningBody
            if (warnings != null && warnings.Count > 0)
            {
                StringBuilder warningBody = new StringBuilder();
                warningBody.Append("<br /><div style=\"margin-left: .5in\"><span class=\"warningTitle\">编译警告：</span>");
                foreach (var warning in warnings)
                {
                    mailTo.Add(warning.OwnerMail);
                    warningBody.Append("<table border=\"1\">");
                    warningBody.Append("<tr>");
                    warningBody.AppendFormat(descTD, "责任人");
                    warning.OwnerDisplayName = adHelper.GetUserDisplayName(warning.OwnerName);
                    displayNames.Add(warning.OwnerDisplayName);
                    warningBody.AppendFormat(detailTDBold, string.Format(CultureInfo.InvariantCulture, "{0}({1})", warning.OwnerDisplayName, warning.OwnerName));
                    warningBody.Append("</tr>");
                    warningBody.Append("<tr>");
                    warningBody.AppendFormat(descTD, "签入时间");
                    warningBody.AppendFormat(detailTD, warning.CreationDate);
                    warningBody.Append("</tr>");
                    warningBody.Append("<tr>");
                    warningBody.AppendFormat(descTD, "文件位置");
                    warningBody.AppendFormat(detailTD, warning.ServerPath);
                    warningBody.Append("</tr>");
                    warningBody.Append("<tr>");
                    warningBody.AppendFormat(descTD, "详细信息");
                    warningBody.AppendFormat(detailTD, warning.CompileDetail);
                    warningBody.Append("</tr>");
                    warningBody.Append("</table>");
                }
                warningBody.Append("</div>");
                bodyHtml = bodyHtml.Replace("{%warningBody%}", warningBody.ToString());
            }
            else
            {
                bodyHtml = bodyHtml.Replace("{%warningBody%}", string.Empty);
            }
            #endregion

            displayNames = displayNames.Distinct().ToList<string>();
            StringBuilder mailNames = new StringBuilder();
            foreach (var t in displayNames)
            {
                mailNames.Append(t.Format<string>("{0}、"));
            }
            if (mailNames.Length > 0)
            {
                mailNames = mailNames.Remove(mailNames.Length - 1, 1);
            }

            bodyHtml = bodyHtml.Replace("{%workspaceServerPath%}", workspaceServerPath);
            bodyHtml = bodyHtml.Replace("{%startTime%}", startTime.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture));
            bodyHtml = bodyHtml.Replace("{%endTime%}", endTime.ToString("yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture));
            bodyHtml = bodyHtml.Replace("{%timeElapsed%}", timeElapsed);
            bodyHtml = bodyHtml.Replace("{%mailNames%}", mailNames.ToString());

            mailTo = mailTo.Distinct().ToList<string>();

            SendMail(mailFrom, mailTo, mailCC, mailSubject, bodyHtml);
        }

        private void SendMail(string mailFrom, List<string> mailTo, string mailCC, string mailSubject, string mailBody)
        {
            using (MailMessage email = new MailMessage())
            {

                email.From = new MailAddress(mailFrom);
                // email.To.Add(mailFrom);
                foreach (var t in mailTo)
                {
                    email.To.Add(t);
                }
                if(!string.IsNullOrEmpty(mailFrom))
                    email.CC.Add(mailFrom);
                if (!string.IsNullOrEmpty(mailCC))
                    email.CC.Add(mailCC);
                email.Subject = mailSubject;
                email.Body = mailBody;
                email.BodyEncoding = Encoding.UTF8;
                email.IsBodyHtml = true;

                using (SmtpClient sc = new SmtpClient(smtpServer))
                {
                    sc.Port = smtpServerPort;
                    sc.Credentials = new NetworkCredential(userName, password, domain);
                    sc.UseDefaultCredentials = false;
                    sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                    sc.Send(email);
                }
            }
        }

        private static string GetBodyHtml()
        {
            string result;

            string fileName = string.Concat(AppDomain.CurrentDomain.BaseDirectory, "NotifyBodyTemplate.htm");
            using (StreamReader sr = new StreamReader(fileName, Encoding.UTF8))
            {
                result = sr.ReadToEnd();
            }
            return result;
        }
    }
}

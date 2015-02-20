using System;
using System.Text;
using System.Web.UI;
using System.Collections.Generic;
using MCS.Web.Library;

[assembly: WebResource("MCS.Web.WebControls.Resources.Images.activity.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Resources.Images.anchor.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Resources.Images.completeActivity.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Resources.Images.initialActivity.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Resources.Images.delay.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Resources.Images.cancelled.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Resources.Images.hourglass.gif", "image/gif")]

[assembly: WebResource("MCS.Web.WebControls.Resources.Images.roles.gif", "image/gif")]

[assembly: WebResource("MCS.Web.WebControls.UserControl.user.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.UserControl.disabledUser.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.UserControl.ou.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.UserControl.disabledOu.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.UserControl.group.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.UserControl.disabledGroup.gif", "image/gif")]

[assembly: WebResource("MCS.Web.WebControls.UserPresence.Resources.ucStatus.png", "image/png")]


// ������
[assembly: WebResource("MCS.Web.WebControls.Images.application16.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.application32.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.function16.png", "image/png")]
[assembly: WebResource("MCS.Web.WebControls.Images.function32.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.group16.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.group32.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.ou16.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.ou32.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.role16.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.role32.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.user16.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.Images.user32.gif", "image/gif")]






namespace MCS.Web.WebControls
{
	public static class ControlResources
	{
		private static Dictionary<string, string> _LogoImageDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) 
		{
 			{ "activity.gif", "MCS.Web.WebControls.Resources.Images.activity.gif"},
			{ "anchor.gif", "MCS.Web.WebControls.Resources.Images.anchor.gif"},
			{ "initialActivity.gif", "MCS.Web.WebControls.Resources.Images.initialActivity.gif"},
			{ "completeActivity.gif", "MCS.Web.WebControls.Resources.Images.completeActivity.gif"},

			{ "delay.gif", "MCS.Web.WebControls.Resources.Images.delay.gif"},
			{ "cancelled.gif", "MCS.Web.WebControls.Resources.Images.cancelled.gif"},
			{ "hourglass.gif", "MCS.Web.WebControls.Resources.Images.hourglass.gif"},
			
			{ "roles.gif", "MCS.Web.WebControls.Resources.Images.roles.gif"},
			{ "user.gif", "MCS.Web.WebControls.UserControl.user.gif"},
			{ "disabledUser.gif", "MCS.Web.WebControls.UserControl.disabledUser.gif"},
			{ "ou.gif", "MCS.Web.WebControls.UserControl.ou.gif"},
			{ "disabledOu.gif", "MCS.Web.WebControls.UserControl.disabledOu.gif"},
			{ "group.gif", "MCS.Web.WebControls.UserControl.group.gif"},
			{ "disabledGroup.gif", "MCS.Web.WebControls.UserControl.disabledGroup.gif"},
			{ "ucStatus.png", "MCS.Web.WebControls.UserPresence.Resources.ucStatus.png"},
			//������ͼ��
			{ "application16","MCS.Web.WebControls.Images.application16.gif"},
			{ "application32","MCS.Web.WebControls.Images.application32.gif"},
			{ "function16","MCS.Web.WebControls.Images.function16.png"},
			{ "function32","MCS.Web.WebControls.Images.function32.gif"},
			{ "group16","MCS.Web.WebControls.Images.group16.gif"},
			{ "group32","MCS.Web.WebControls.Images.group32.gif"},
			{ "ou16","MCS.Web.WebControls.Images.ou16.gif"},
			{ "ou32","MCS.Web.WebControls.Images.ou32.gif"},
			{ "role16","MCS.Web.WebControls.Images.role16.gif"},
			{ "role32","MCS.Web.WebControls.Images.role32.gif"},
			{ "user16","MCS.Web.WebControls.Images.user16.gif"},
			{ "user32","MCS.Web.WebControls.Images.user32.gif"},
		};

		public static string ActivityLogoUrl
		{
			get
			{
				return GetWebResourceUrl("MCS.Web.WebControls.Resources.Images.activity.gif");
			}
		}

		public static string AncnhorActivityLogoUrl
		{
			get
			{
				return GetWebResourceUrl("MCS.Web.WebControls.Resources.Images.anchor.gif");
			}
		}

		public static string InitialActivityLogoUrl
		{
			get
			{
				return GetWebResourceUrl("MCS.Web.WebControls.Resources.Images.initialActivity.gif");
			}
		}

		public static string CompletedActivityLogoUrl
		{
			get
			{
				return GetWebResourceUrl("MCS.Web.WebControls.Resources.Images.completeActivity.gif");
			}
		}

		public static string DelayLogoUrl
		{
			get
			{
				return GetWebResourceUrl("MCS.Web.WebControls.Resources.Images.delay.gif");
			}
		}

		public static string CancelledLogoUrl
		{
			get
			{
				return GetWebResourceUrl("MCS.Web.WebControls.Resources.Images.cancelled.gif");
			}
		}

		public static string HourglassLogoUrl
		{
			get
			{
				return GetWebResourceUrl("MCS.Web.WebControls.Resources.Images.hourglass.gif");
			}
		}

		public static string RoleLogoUrl
		{
			get
			{
				return GetWebResourceUrl("MCS.Web.WebControls.Resources.Images.roles.gif");
			}
		}

		public static string UserLogoUrl
		{
			get
			{
				return GetWebResourceUrl("MCS.Web.WebControls.UserControl.user.gif");
			}
		}

		public static string DisabledUserLogoUrl
		{
			get
			{
				return GetWebResourceUrl("MCS.Web.WebControls.UserControl.disabledUser.gif");
			}
		}

		public static string OULogoUrl
		{
			get
			{
				return GetWebResourceUrl("MCS.Web.WebControls.UserControl.ou.gif");
			}
		}

		public static string DisabledOULogoUrl
		{
			get
			{
				return GetWebResourceUrl("MCS.Web.WebControls.UserControl.disabledOu.gif");
			}
		}

		public static string GroupLogoUrl
		{
			get
			{
				return GetWebResourceUrl("MCS.Web.WebControls.UserControl.group.gif");
			}
		}

		public static string DisabledGroupLogoUrl
		{
			get
			{
				return GetWebResourceUrl("MCS.Web.WebControls.UserControl.disabledGroup.gif");
			}
		}

		public static string UCStatusUrl
		{
			get
			{
				return GetWebResourceUrl("MCS.Web.WebControls.UserPresence.Resources.ucStatus.png");
			}
		}

		/// <summary>
		/// ����key����ͼ���url��keyһ����ͼƬ���ļ���������group.gif�������ִ�Сд��
		/// ���key�������򷵻�null
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static string GetResourceByKey(string key)
		{
			string resourceName = null;
			string result = null;

			if (_LogoImageDictionary.TryGetValue(key, out resourceName))
				result = GetWebResourceUrl(resourceName);

			return result;
		}

		private static string GetWebResourceUrl(string resourceName)
		{
			Page page = WebUtility.GetCurrentPage();

			if (page == null)
				page = new Page();

			return page.ClientScript.GetWebResourceUrl(typeof(ControlResources), resourceName);
		}
	}
}

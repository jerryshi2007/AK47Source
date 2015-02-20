using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.WebControls;
using MCS.Library.OGUPermission;
using System.Web.UI.HtmlControls;
using System.Text;

namespace MCS.Library.SOA.Web.WebControls.Test.UserPresenceControl
{
	public partial class userPresenceTest : System.Web.UI.Page
	{
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                HtmlGenericControl div = new HtmlGenericControl("div");

                UserPresence presence0 = new UserPresence(); //初始化用户状态控件

                presence0.UserID = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, "changdm")[0].ID;// "22c3b351-a713-49f2-8f06-6b888a280fff";

                presence0.UserDisplayName = "常冬梅";

				//presence0.AccountDisabled=true;

                UserPresence presence = new UserPresence(); //初始化用户状态控件

                presence.UserID = "22c3b351-a713-49f2-8f06-6b888a280fff";
                presence.StatusImage = StatusImageType.ShortBar;
                presence.ShowUserIcon = true;
                presence.UserIconUrl = "cdm.png";
                presence.ShowUserDisplayName = true;
                presence.UserDisplayName = "常冬梅";
				presence.AccountDisabled = true;

                UserPresence presence1 = new UserPresence(); //初始化用户状态控件

                presence1.UserID = "22c3b351-a713-49f2-8f06-6b888a280fff";
                presence1.StatusImage = StatusImageType.LongBar;
                presence1.ShowUserIcon = true;
                presence1.UserIconUrl = "cdm.png";
                presence1.UserDisplayName = "cdm";
				presence1.AccountDisabled = true;

                div.Controls.Add(presence0);
                div.Controls.Add(presence);            
                div.Controls.Add(presence1);
                usersPresenceContainer.Controls.Add(div);
            }
        }

	    protected void showPresenceBtn_Click(object sender, EventArgs e)
		{
			usersPresenceContainer.Controls.Clear();
            HtmlGenericControl div = new HtmlGenericControl("div");
			foreach (IOguObject obj in userInput.SelectedOuUserData)
			{
				
				UserPresence presence = new UserPresence();   //初始化用户状态控件

				presence.UserID = obj.ID;   //所需要显示的用户ID
				presence.UserDisplayName = obj.DisplayName; //用户名称
                presence.StatusImage = StatusImageType.ShortBar;

				div.Controls.Add(presence);  //放入到显示区域
				
			}
            usersPresenceContainer.Controls.Add(div);
		}

		protected void showPresenceWithScript_Click(object sender, EventArgs e)
		{
			StringBuilder strB = new StringBuilder();

			int index = 0;

			foreach (IOguObject obj in userInput.SelectedOuUserData)
			{
				strB.Append(UserPresence.GetUsersPresenceHtml(obj.ID, obj.DisplayName, "up_" + index, true,false,true,StatusImageType.Ball,""));
				index++;
			}

			usersPresenceScriptResultContainer.InnerHtml = strB.ToString();
		}
	}
}
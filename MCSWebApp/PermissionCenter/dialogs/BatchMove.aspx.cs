using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PermissionCenter.Dialogs
{
	public partial class BatchMove : System.Web.UI.Page
	{
		#region 受保护的方法

		protected void Page_Load(object sender, EventArgs e)
		{
			Util.EnsureOperationSafe();
			this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
		}

		protected override void OnPreRender(EventArgs e)
		{
			bool inAction = this.Request.Form["action"] == "yes" && this.Request.HttpMethod == "POST";

			if (inAction)
			{
				this.actions.DataSource = this.GetDataSource(Util.GetAvailableOperation(Request.Form.GetValues("key")), this.Request.QueryString["nomove"] != "1");
				this.actions.DataBind();
			}

			base.OnPreRender(e);
		}
		#endregion

		#region 私有的方法
		private System.Collections.IEnumerable GetDataSource(TransferObjectType type, bool moveable)
		{
			switch (type)
			{
				case TransferObjectType.None:
					break;
				case TransferObjectType.Members:
					yield return new { Description = "人员添加到群组", Mode = TransferActionType.UserCopyToGroup };
					yield return new { Description = "人员添加到组织", Mode = TransferActionType.UserCopyToOrg };
					if (moveable)
					{
						yield return new { Description = "人员移动到组织", Mode = TransferActionType.UserMoveToOrg };
					}

					break;
				case TransferObjectType.Groups:
					if (moveable)
					{
						yield return new { Description = "群组移动到组织", Mode = TransferActionType.GroupMoveToOrg };
					}

					break;
				case TransferObjectType.Orgnizations:
					if (moveable)
					{
						yield return new { Description = "组织转移", Mode = TransferActionType.OrgTransfer };
					}

					break;
				case TransferObjectType.RootOrgnizations:
					break;
				default:
					if (moveable)
					{
						yield return new { Description = "混合移动到组织", Mode = TransferActionType.MixedToOrg };
					}

					break;
			}

			yield break;
		}
		#endregion
	}
}
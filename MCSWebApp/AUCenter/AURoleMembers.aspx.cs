using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AU = MCS.Library.SOA.DataObjects.Security.AUObjects;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects;
using MCS.Library.OGUPermission;
using System.Collections;
using MCS.Web.WebControls;
using System.Text;

namespace AUCenter
{
	[SceneUsage("~/App_Data/ListScene.xml")]
	public partial class AURoleMembers : System.Web.UI.Page, INormalSceneDescriptor
	{
		[Serializable]
		public class PostData
		{
			public string RoleID { get; set; }

			public string SchemaRoleID { get; set; }

			public string UserID { get; set; }

			public int Type { get; set; }
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (Page.IsPostBack == false)
			{
				string id = Request.QueryString["id"];

				var unit = DbUtil.GetEffectiveObject<AU.AdminUnit>(id);

				this.schemaInfoLabel.Text = unit.Name;
				this.unitIDField.Value = unit.ID;

				var items = AU.Adapters.AUSnapshotAdapter.Instance.LoadAURoleDisplayItems(unit.ID, true, DateTime.MinValue);

				AssignUsers(items);

				this.initialData.Value = MCS.Web.Library.Script.JSONSerializerExecute.Serialize(items);
			}
		}

		public void AfterNormalSceneApplied()
		{
			
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
		}

		protected void CommitClick(object sender, EventArgs e)
		{
			string unitID = Request.Form["unitIDField"];
		}

		private static void AssignUsers(List<AU.AURoleDisplayItem> items)
		{
			AU.AUCommon.DoDbAction(() =>
			{
				foreach (var item in items)
				{
					var members = PC.Adapters.SCMemberRelationAdapter.Instance.LoadByContainerID(item.ID).FilterByStatus(PC.SchemaObjectStatusFilterTypes.Normal);

					List<IUser> users = new List<IUser>();
					foreach (var m in members)
					{
						users.Add((IUser)OguBase.CreateWrapperObject(new OguUser(m.ID)));
					}

					item.Users = users;
				}
			});
		}

		protected void postControl_DoPostedData(object sender, MCS.Web.WebControls.PostProgressDoPostedDataEventArgs e)
		{
			string unitID = e.ClientExtraPostedData;

			var unit = DbUtil.GetEffectiveObject<AU.AdminUnit>(unitID);
			var status = new UploadProgressStatus();
			StringBuilder output = new StringBuilder();
			try
			{
				status.MinStep = 1;
				status.MaxStep = e.Steps.Count + 1;
				status.CurrentStep = 1;

				AU.AUSchemaRole currentRole = null;

				foreach (Dictionary<string, object> item in e.Steps)
				{
					currentRole = DoAction(unit, status, output, currentRole, item);

					status.Response();
					status.CurrentStep++;
				}
			}
			catch (Exception ex)
			{
				status.StatusText = ex.Message;
				output.AppendLine(ex.ToString());
				status.Response();
			}
			finally
			{
				output.AppendLine("结束");
				e.Result.CloseWindow = false;
				e.Result.ProcessLog = output.ToString();
			}
		}

		private static AU.AUSchemaRole DoAction(AU.AdminUnit unit, UploadProgressStatus status, StringBuilder output, AU.AUSchemaRole currentRole, Dictionary<string, object> item)
		{
			string roleID, schemaRoleID, userID;
			int type;

			try
			{
				roleID = (string)item["RoleID"];
				schemaRoleID = (string)item["SchemaRoleID"];
				userID = (string)item["UserID"];
				type = (int)item["Type"];

				if (currentRole == null || currentRole.ID != roleID)
					currentRole = DbUtil.GetEffectiveObject<AU.AUSchemaRole>(schemaRoleID);

				status.StatusText = string.Format("正在向角色{0}{1}用户{2}", roleID, type == 0 ? "添加" : "删除", userID);
				output.AppendLine(status.StatusText);


				if (type == 0)
				{
					AU.Operations.Facade.InstanceWithPermissions.AddUserToRole(new PC.SCUser() { ID = userID }, unit, currentRole);
				}
				else
				{
					AU.Operations.Facade.InstanceWithPermissions.RemoveUserFromRole(new PC.SCUser() { ID = userID, CodeName = "abc", Name = "abc" }, unit, currentRole);
				}
			}
			catch (Exception ex)
			{
				status.StatusText = ex.Message;
				output.AppendLine(ex.ToString());
			}
			return currentRole;
		}
	}
}
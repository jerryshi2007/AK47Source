using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Data.DataObjects;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Script;
using MCS.Web.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.AutoCompleteWithSelectorControl
{
	public partial class test2 : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			JSONSerializerExecute.RegisterConverter(typeof(OguObjectConverter));

			if (!IsPostBack)
			{
				var datas = new List<CommonData>();
				datas.Add(new CommonData() { Code = "123456789", Name = "Server初始数据", Detail = "TestDataDetail" });
				CommonAutoCompleteWithSelectorControl1.SelectedData = datas;
                //CommonAutoCompleteWithSelectorControl1.CallBackContext = "1";
			}
		}

		private List<CommonData> DoValidate(string str, object context)
		{
			var datas = new List<CommonData>();

			if (str == "1")
			{
				datas.Add(new CommonData() { Code = "Code_" + 1, Name = "TestDataName" + 1, Detail = "TestDataDetail" + 1 });
			}
			else if (str == "2")
			{
				datas.Add(new CommonData() { Code = "Code_" + 2, Name = "TestDataName" + 2, Detail = "TestDataDetail" + 2 });
			}
			else if (str == "3")
			{
				datas.Add(new CommonData() { Code = "Code_" + 3, Name = "TestDataName" + 3, Detail = "TestDataDetail" + 3 });
			}
			else if (str == "*")
			{
				for (int i = 0; i < 5; i++)
				{
					datas.Add(new CommonData() { Code = "Code_" + i, Name = "TestDataName" + i, Detail = "TestDataDetail" + i });
				}
			}
			if (context != null)
			{
				datas.RemoveAll(p => p.Code != "Code_" + context.ToString());
			}
			return datas;
		}

		private OguDataCollection<IOguObject> InnerCheckInputOuUser(string chkString)
		{
			//ServiceBrokerContext.Current.Timeout = QueryUserTimeout;

			IOrganization dept = OguMechanismFactory.GetMechanism().GetObjects<IOrganization>(SearchOUIDType.FullPath, OguPermissionSettings.GetConfig().RootOUPath)[0];

			OguDataCollection<IOguObject> users = QueryChildrenBySelectMask(dept, chkString);
			OguDataCollection<IOguObject> forSelected = new OguDataCollection<IOguObject>();

			for (int i = 0; i < users.Count; i++)
			{
				forSelected.Add(OguBase.CreateWrapperObject(users[i]));
			}

			if (dept.DisplayName.IndexOf(chkString) == 0)
			{
				forSelected.Add(OguBase.CreateWrapperObject(dept));
			}

			IOrganization root = OguMechanismFactory.GetMechanism().GetRoot();

			foreach (OguBase obj in forSelected)
			{
				string path = string.Empty;
				if (obj.Parent != null)
					path = obj.Parent.FullPath;

				if (path.IndexOf(root.FullPath) == 0)
				{
					path = path.Substring(root.FullPath.Length);
					path = path.Trim('\\');
				}

				if (obj is IUser)
					((OguUser)obj).Description = string.Format("{0} {1}", ((OguUser)obj).Occupation, path);
				else if (obj is OguOrganization)
					((OguOrganization)obj).Description = path;
				else
					((OguUser)obj).Description = path;
			}

			return forSelected;
		}

		private OguDataCollection<IOguObject> QueryChildrenBySelectMask(IOrganization dept, string searchString)
		{
			ServiceBrokerContext.Current.SaveContextStates();
			var SelectMask = UserControlObjectMask.All;
			var MaxQueryCount = 15;

			try
			{
				ServiceBrokerContext.Current.ListObjectCondition = ListObjectMask.All;

				OguDataCollection<IOguObject> result = new OguDataCollection<IOguObject>();

				if (SelectMask == UserControlObjectMask.Organization)
				{
					OguObjectCollection<IOrganization> orgs = dept.QueryChildren<IOrganization>(searchString, true, SearchLevel.SubTree, MaxQueryCount);
					foreach (IOrganization org in orgs)
						result.Add(org);
				}
				else
					if (SelectMask == UserControlObjectMask.Group)
					{
						OguObjectCollection<IGroup> groups = dept.QueryChildren<IGroup>(searchString, true, SearchLevel.SubTree, MaxQueryCount);
						foreach (IOrganization group in groups)
							result.Add(group);
					}
					else
						if (SelectMask == UserControlObjectMask.User || SelectMask == (UserControlObjectMask.User | UserControlObjectMask.Sideline))
						{
							OguObjectCollection<IUser> users = dept.QueryChildren<IUser>(searchString, true, SearchLevel.SubTree, MaxQueryCount);
							foreach (IUser user in users)
								result.Add(user);
						}
						else
						{
							if ((int)(SelectMask & (UserControlObjectMask.Group | UserControlObjectMask.User | UserControlObjectMask.Organization)) != 0)
							{
								OguObjectCollection<IOguObject> objs = dept.QueryChildren<IOguObject>(searchString, true, SearchLevel.SubTree, MaxQueryCount);

								foreach (IOguObject obj in objs)
									result.Add(obj);
							}
						}

				return result;
			}
			finally
			{
				ServiceBrokerContext.Current.RestoreSavedStates();
			}
		}

		protected void Button1_Click(object sender, EventArgs e)
		{

		}

		protected IList CommonAutoCompleteWithSelectorControl1_ValidateInput(string chkString, object context)
		{
			return DoValidate(chkString, context) as IList;
		}

		protected IEnumerable CommonAutoCompleteWithSelectorControl1_GetDataSource(string chkString, object context)
		{
			IEnumerable result = null;

			var datas = new List<object>();

			if (chkString == "1" && context.ToString() == "1")
			{
				datas.Add(new CommonData() { Code = "Code_" + 1, Name = "TestDataName" + 1, Detail = "TestDataDetail" + 1 });
			}
			else if (chkString == "2" && context.ToString() == "2")
			{
				datas.Add(new CommonData() { Code = "Code_" + 2, Name = "TestDataName" + 2, Detail = "TestDataDetail" + 2 });
			}
			else if (chkString == "3" && context.ToString() == "3")
			{
				datas.Add(new CommonData() { Code = "Code_" + 3, Name = "TestDataName" + 3, Detail = "TestDataDetail" + 3 });
			}
			else if (chkString == "*")
			{
				for (int i = 0; i < 5; i++)
				{
					datas.Add(new CommonData() { Code = "Code_" + i, Name = "TestDataName" + i, Detail = "TestDataDetail" + i });
				}
			}

			result = datas;

			return result;
		}
	}
}
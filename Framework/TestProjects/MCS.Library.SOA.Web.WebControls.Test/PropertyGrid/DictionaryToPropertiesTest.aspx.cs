using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library.Script;
using MCS.Web.WebControls;
using MCS.Library.OGUPermission;

namespace MCS.Library.SOA.Web.WebControls.Test.PropertyGrid
{
	public partial class DictionaryToPropertiesTest : System.Web.UI.Page
	{
		protected override void OnInit(EventArgs e)
		{
			PropertyEditorRegister();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.Page.IsPostBack == false)
			{
				Dictionary<string, object> dictionary = PrepareBasicDictionary();

				propertyGrid.Properties.CopyFrom(dictionary.ToProperties());
			}
		}

		protected void bT_Click(object sender, EventArgs e)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();

			propertyGrid.Properties.FillDictionary(dictionary);

			StringBuilder strB = new StringBuilder();

			foreach (KeyValuePair<string, object> kp in dictionary)
			{
				strB.AppendFormat("<div><B>{0}</B>: {1}</div>", HttpUtility.HtmlEncode(kp.Key), HttpUtility.HtmlEncode(kp.Value.ToString()));
			}

			dictionaryItems.InnerHtml = strB.ToString();
		}

		private static Dictionary<string, object> PrepareBasicDictionary()
		{
			Dictionary<string, object> result = new Dictionary<string, object>();

			result.Add("Name", "Shen Zheng");
			result.Add("Age", (DateTime.Now.Year - 1972));
			result.Add("Salary", (Decimal)10000.00);
			result.Add("Birthday", new DateTime(1972, 4, 26));
			result.Add("IsMale", true);

			IUser user = OguMechanismFactory.GetMechanism().GetObjects<IUser>(SearchOUIDType.LogOnName, "wanhw").FirstOrDefault();

			result.Add("Owner", user);
			result.Add("OwnerDepartment", user.Parent);

			IGroup group = OguMechanismFactory.GetMechanism().GetObjects<IGroup>(SearchOUIDType.FullPath, "机构人员\\远洋地产\\全体党员").FirstOrDefault();

			result.Add("Group", group);

			OguDataCollection<IUser> multiUsers = new OguDataCollection<IUser>();

			multiUsers.Add(user);

			result.Add("MultiUsers", multiUsers);

			OguDataCollection<IOrganization> multiOrgs = new OguDataCollection<IOrganization>();

			multiOrgs.Add(user.Parent);

			result.Add("MultiOrgs", multiOrgs);

			OguDataCollection<IGroup> multiGroups = new OguDataCollection<IGroup>();

			multiGroups.Add(group);

			result.Add("MultiGroups", multiGroups);

			OguDataCollection<IOguObject> multiObjs = new OguDataCollection<IOguObject>();

			multiObjs.Add(user);
			multiObjs.Add(user.Parent);

			result.Add("MultiObjs", multiObjs);

			return result;
		}

		private void PropertyEditorRegister()
		{
			PropertyEditorHelper.RegisterEditor(new StandardPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new BooleanPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new EnumPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new ObjectPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new DatePropertyEditor());
			PropertyEditorHelper.RegisterEditor(new DateTimePropertyEditor());
			PropertyEditorHelper.RegisterEditor(new CustomObjectListPropertyEditor());

			PropertyEditorHelper.RegisterEditor(new ImageUploaderPropertyEditor());
			PropertyEditorHelper.RegisterEditor(new ImageUploaderPropertyEditorForGrid());
			PropertyEditorHelper.RegisterEditor(new OUUserInputPropertyEditor());

		}
	}
}
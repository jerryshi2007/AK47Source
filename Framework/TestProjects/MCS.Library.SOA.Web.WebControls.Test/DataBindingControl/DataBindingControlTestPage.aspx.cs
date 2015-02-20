using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using MCS.OA.DataObjects;
using MCS.Library.Principal;
using MCS.Library.Core;
using MCS.Library.Validation;
using System.Globalization;
using System.Threading;

namespace MCS.OA.Web.WebControls.Test.DataBindingControl
{
	public partial class DataBindingControlTestPage : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				this.bindingControl.Data = PrepareData();
				this.bindingControl.OnValidateDatas.Add(new ValidateData(bindingControl_OnValidateData));
				this.bindingControl.DataBind();

				this.DataBindingControl2.Data = this.bindingControl.Data;
				this.DataBindingControl2.DataBind();
			}
		}

		protected override void OnInitComplete(EventArgs e)
		{
			base.OnInitComplete(e);
		}

		protected void collectDataBtn_Click(object sender, EventArgs e)
		{
			this.bindingControl.Data = PrepareData();
			this.bindingControl.CollectData();
			this.bindingControl.DataBind();

			this.DataBindingControl2.Data = this.bindingControl.Data;
			this.DataBindingControl2.CollectData();
			this.DataBindingControl2.DataBind();
		}

		public ValidationResult bindingControl_OnValidateData(object sender, EventArgs e)
		{
			ValidationResult vresult = null;
			TestDataObject tobject = (TestDataObject)sender;
			if (tobject.UserAge != 20)
			{
				vresult = new ValidationResult("我们只需要20岁的。",tobject,string.Empty,string.Empty,null);
			}
			return vresult;
		}

		public ValidationResult bindingControl_OnValidateData1(object sender, EventArgs e)
		{
			ValidationResult vresult = null;
			TestDataObject tobject = (TestDataObject)sender;
			if (tobject.UserAge != 30)
			{
				vresult = new ValidationResult("我们只需要30岁的。", tobject, string.Empty, string.Empty, null);
			}
			return vresult;
		}

		private TestDataObject PrepareData()
		{
			TestDataObject data = new TestDataObject();

			data.UserName = "李安";
			data.Birthday = new DateTime(1973, 1, 1);
			data.Gender = GenderDefine.None;
			data.PassawayDay = new DateTime(1973, 1, 1);

			Material m1 = new Material();

			m1.ID = Guid.NewGuid().ToString();
			m1.Title = "犯罪记录";
			m1.OriginalName = "犯罪记录.doc";
			m1.RelativeFilePath = "..\\";
			m1.Creator = DeluxeIdentity.CurrentUser;

			data.Materials.Add(m1);

			data.Creator = DeluxeIdentity.CurrentUser;

			return data;
		}
	}
}

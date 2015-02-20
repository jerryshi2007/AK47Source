using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using MCS.Web.Library.Script;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using System.Drawing;

[assembly: WebResource("MCS.Web.WebControls.UserSelector.ExtOuUserInputControl.js", "application/x-javascript")]

namespace MCS.Web.WebControls
{
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource("MCS.Web.WebControls.ExtOuUserInputControl", "MCS.Web.WebControls.UserSelector.ExtOuUserInputControl.js")]
	[ToolboxData("<{0}:ExtOuUserInputControl runat=server></{0}:ExtOuUserInputControl>")]
	public class ExtOuUserInputControl : ScriptControlBase, INamingContainer
	{
		private UserOUGraphControl userTree = new UserOUGraphControl() { InvokeWithoutViewState = true };
		private OuUserInputControl consignUserInput = new OuUserInputControl() { InvokeWithoutViewState = true, Height=Unit.Pixel(20) };
		private OuUserInputControl circulatorInput = new OuUserInputControl() { InvokeWithoutViewState = true, Height = Unit.Pixel(20) };
		private HtmlInputButton consignUserButton = new HtmlInputButton();
		private HtmlInputButton circulatorButton = new HtmlInputButton();
		private IOrganization root = null;

		public ExtOuUserInputControl()
			: base(true, HtmlTextWriterTag.Div)
		{
		}

		private OguDataCollection<IOguObject> _consignUsers;

		/// <summary>
		/// 选择的数据
		/// </summary>
		/// <remarks>
		/// OU，User的数据
		/// </remarks>
		[Description("选择的会签人员")]
		[Browsable(false)]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("consignUsers")]//对应的客户端属性
		public OguDataCollection<IOguObject> ConsignUsers
		{
			get
			{
				if (this._consignUsers == null)
					this._consignUsers = new OguDataCollection<IOguObject>();

				return this._consignUsers;

			}
			set { this._consignUsers = value; }
		}

		private OguDataCollection<IOguObject> _circulators;

		/// <summary>
		/// 选择的数据
		/// </summary>
		/// <remarks>
		/// OU，User的数据
		/// </remarks>
		[Description("选择的会签人员")]
		[Browsable(false)]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("circulators")]//对应的客户端属性
		public OguDataCollection<IOguObject> Circulators
		{
			get
			{
				if (this._circulators == null)
					this._circulators = new OguDataCollection<IOguObject>();

				return this._circulators;

			}
			set { this._circulators = value; }
		}

		/// <summary>
		/// 是否可以选择根节点
		/// </summary>
		/// <remarks>
		/// 是否可以选择根节点
		/// </remarks>
		[Description("是否可以选择根节点")]
		[ClientPropertyName("canSelectRoot")]//对应的客户端属性
		public bool CanSelectRoot
		{
			get { return GetPropertyValue<bool>("CanSelectRoot", true); }
			set { SetPropertyValue<bool>("CanSelectRoot", value); }
		}

		/// <summary>
		/// 是否显示传阅的用户
		/// </summary>
		[Description("是否显示传阅的用户")]
		public bool ShowCirculateUsers
		{
			get { return GetPropertyValue<bool>("ShowCirculateUsers", true); }
			set { SetPropertyValue<bool>("ShowCirculateUsers", value); }
		}

		[Description("显示范围")]
		public UserControlObjectMask ListMask
		{
			get { return GetPropertyValue<UserControlObjectMask>("ListMask", UserControlObjectMask.All); }
			set { SetPropertyValue<UserControlObjectMask>("ListMask", value); }
		}

		[Description("选择范围")]
		public UserControlObjectMask SelectMask
		{
			get { return GetPropertyValue<UserControlObjectMask>("SelectMask", UserControlObjectMask.All); }
			set { SetPropertyValue<UserControlObjectMask>("SelectMask", value); }
		}

		/// <summary>
		/// 根机构
		/// </summary>
		[Browsable(false)]
		public IOrganization Root
		{
			get
			{
				if (this.root != null)
					if (string.Compare(this.root.FullPath, RootPath, true) != 0)
						this.root = null;

				if (this.root == null)
				{
					if (string.IsNullOrEmpty(this.RootPath))
						this.root = OguMechanismFactory.GetMechanism().GetRoot();
					else
						this.root = OguMechanismFactory.GetMechanism().GetObjects<IOrganization>(SearchOUIDType.Guid, this.RootPath)[0];
				}

				return this.root;
			}
			set
			{
				this.root = value;

				if (this.root != null)
					this.RootPath = this.root.FullPath;
				else
					this.RootPath = string.Empty;
			}
		}

		/// <summary>
		/// 根OU的路径
		/// </summary>
		public string RootPath
		{
			get
			{
				return GetPropertyValue("RootPath", string.Empty);
			}
			set
			{
				SetPropertyValue("RootPath", value);
				this.root = null;
			}
		}

		/// <summary>
		/// 是否显示兼职人员
		/// </summary>
		/// <remarks>
		/// 是否显示兼职人员
		/// </remarks>
		[Description("是否显示兼职人员")]
		[ClientPropertyName("showSideLine")]//对应的客户端属性
		public bool ShowSideLine
		{
			get { return GetPropertyValue<bool>("ShowSideLine", true); }
			set { SetPropertyValue<bool>("ShowSideLine", value); }
		}

		/// <summary>
		/// 是否可以多选
		/// </summary>
		/// <remarks>
		/// 是否可以多选
		/// </remarks>
		[Description("是否可以多选  Single:单选   Multiple:多选")]
		public bool MultiSelect
		{
			get { return GetPropertyValue<bool>("MultiSelect", true); }
			set { SetPropertyValue<bool>("MultiSelect", value); }
		}

		[ScriptControlProperty]
		[ClientPropertyName("userTreeClientID")]
		private string UserTreeClientID
		{
			get
			{
				return this.userTree.ClientID;
			}
		}

		[ScriptControlProperty]
		[ClientPropertyName("consignUserInputClientID")]
		private string ConsignUserInputClientID
		{
			get
			{
				return this.consignUserInput.ClientID;
			}
		}

		[ScriptControlProperty]
		[ClientPropertyName("circulatorInputClientID")]
		private string CirculatorInputClientID
		{
			get
			{
				return this.circulatorInput.ClientID;
			}
		}

		[ScriptControlProperty]
		[ClientPropertyName("consignUserButtonClientID")]
		private string ConsignUserButtontClientID
		{
			get
			{
				return this.consignUserButton.ClientID;
			}
		}

		[ScriptControlProperty]
		[ClientPropertyName("circulatorButtonClientID")]
		private string CirculatorButtonClientID
		{
			get
			{
				return this.circulatorButton.ClientID;
			}
		}

		protected override void OnInit(EventArgs e)
		{
			InitChildControls();

			base.OnInit(e);

			if (this.Page.IsCallback)
				EnsureChildControls();
		}

		protected override void OnPagePreLoad(object sender, EventArgs e)
		{
			base.OnPagePreLoad(sender, e);

			EnsureChildControls();
		}

		protected override void CreateChildControls()
		{
			base.CreateChildControls();
		}

		protected override void OnPreRender(EventArgs e)
		{
			SetInnerControlAttributes();

			base.OnPreRender(e);
		}

		#region Private
		private void SetInnerControlAttributes()
		{
			this.userTree.Root = this.Root;
			this.userTree.ListMask = this.ListMask;
			this.userTree.SelectMask = this.SelectMask;
			this.userTree.MultiSelect = this.MultiSelect;

			this.consignUserInput.RootPath = this.RootPath;
			this.consignUserInput.ListMask = this.ListMask;
			this.consignUserInput.SelectMask = this.SelectMask;
			this.consignUserInput.MultiSelect = this.MultiSelect;

			this.circulatorInput.RootPath = this.RootPath;
			this.circulatorInput.ListMask = this.ListMask;
			this.circulatorInput.SelectMask = this.SelectMask;
			this.circulatorInput.MultiSelect = this.MultiSelect;
		}

		private void InitChildControls()
		{
			HtmlTable containerTable = new HtmlTable();

			containerTable.Border = 0;
			containerTable.CellPadding = 0;
			containerTable.CellSpacing = 0;
			containerTable.Style["width"] = "100%";
			containerTable.Style["height"] = "100%";

			Controls.Add(containerTable);

			HtmlTableRow row = new HtmlTableRow();

			containerTable.Controls.Add(row);

			HtmlTableCell cellLeft = new HtmlTableCell();
			row.Controls.Add(cellLeft);

			cellLeft.Width = "50%";
			cellLeft.Style["vertical-align"] = "top";
			cellLeft.Style["overflow"] = "auto";
			cellLeft.Style["border"] = "solid 1px silver";

			this.userTree.ID = "userTree";
			this.userTree.ShowingMode = ControlShowingMode.Normal;
			this.userTree.Width = Unit.Percentage(100);
			this.userTree.Height = Unit.Percentage(100);
			cellLeft.Controls.Add(this.userTree);

			HtmlTableCell cellRight = new HtmlTableCell();
			cellRight.Style["vertical-align"] = "middle";
			//cellRight.Width = "50%";
			row.Controls.Add(cellRight);

			CreateUserInputTable(cellRight);

			SetInnerControlAttributes();
		}

		private void CreateUserInputTable(Control parent)
		{
			HtmlTable table = new HtmlTable();

			table.Border = 0;
			table.CellPadding = 0;
			table.CellSpacing = 0;
			table.Style["width"] = "100%";
			table.Style["height"] = "180px";

			parent.Controls.Add(table);

			HtmlTableRow row1 = new HtmlTableRow();
			table.Controls.Add(row1);

			HtmlTableCell row1Cell0 = new HtmlTableCell();
			row1Cell0.Style["vertical-align"] = "middle";
			row1Cell0.Style["width"] = "108px";
			row1.Controls.Add(row1Cell0);

			this.consignUserButton.ID = "consignUserButton";
			this.consignUserButton.Value = "会签>>";
			this.consignUserButton.Attributes["class"] = "formButton";
			row1Cell0.Controls.Add(consignUserButton);

			HtmlTableCell row1Cell1 = new HtmlTableCell();
			row1Cell1.Style["vertical-align"] = "middle";

			row1.Controls.Add(row1Cell1);

			this.consignUserInput.ID = "consignUserInput";
			row1Cell1.Controls.Add(this.consignUserInput);

			if (ShowCirculateUsers)
			{
				HtmlTableRow row2 = new HtmlTableRow();
				table.Controls.Add(row2);

				HtmlTableCell row2Cell0 = new HtmlTableCell();
				row2Cell0.Style["vertical-align"] = "middle";
				row2.Controls.Add(row2Cell0);

				this.circulatorButton.ID = "circulatorButton";
				this.circulatorButton.Value = "传阅>>";
				this.circulatorButton.Attributes["class"] = "formButton";
				row2Cell0.Controls.Add(circulatorButton);

				HtmlTableCell row2Cell1 = new HtmlTableCell();
				row2Cell1.Style["vertical-align"] = "middle";
				row2.Controls.Add(row2Cell1);

				this.circulatorInput.ID = "circulatorInput";
				row2Cell1.Controls.Add(this.circulatorInput);
			}
		}
		#endregion
	}
}

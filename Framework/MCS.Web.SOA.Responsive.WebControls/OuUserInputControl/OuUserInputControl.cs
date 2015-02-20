using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Web.Responsive.Library;
using MCS.Web.Responsive.Library.Script;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

#region
[assembly: WebResource("MCS.Web.Responsive.WebControls.OuUserInputControl.OuUserInputControl.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.OuUserInputControl.user.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.OuUserInputControl.ou.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.OuUserInputControl.group.gif", "image/gif")]
//[assembly: WebResource("MCS.Web.Responsive.WebControls.OuUserInputControl.propertyDialog.htm", "text/html")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.OuUserInputControl.selectObjectDialog.htm", "text/html")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.OuUserInputControl.checkUser.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.OuUserInputControl.OuUserInputDefaultCss.css", "text/css")]
#endregion

namespace MCS.Web.Responsive.WebControls
{
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[RequiredScript(typeof(AutoCompleteExtender), 3)]
	[ClientScriptResource("MCS.Web.WebControls.OuUserInputControl", "MCS.Web.Responsive.WebControls.OuUserInputControl.OuUserInputControl.js")]
	[ClientCssResource("MCS.Web.Responsive.WebControls.OuUserInputControl.OuUserInputDefaultCss.css")]
	[ToolboxData("<{0}:OuUserInputControl runat=server></{0}:OuUserInputControl>")]
	public class OuUserInputControl : AutoCompleteWithSelectorControlBase, INamingContainer
	{
		private const int MaxQueryCount = 15;

		public event AutoCompleteExtender.GetDataSourceDelegate GetDataSource;
		public event ValidateInputOuUserHandler ValidateInputOuUser;
		public event OuUserObjectsLoadedHandler ObjectsLoaded;

		#region 属性
		/// <summary>
		/// 数据显示字段名称，支持逗号分割
		/// </summary>
		[Bindable(true), Description("数据显示字段名称")]
		public override string DataTextFields
		{
			get
			{
				return GetPropertyValue("DataTextField", "displayName,description");
			}

			set
			{
				SetPropertyValue("DataTextField", value);
			}
		}

		/// <summary>
		/// 是否可以选择根节点
		/// </summary>
		/// <remarks>
		/// 是否可以选择根节点
		/// </remarks>
		[Description("是否可以选择根节点")]
		public bool CanSelectRoot
		{
			get { return GetPropertyValue<bool>("CanSelectRoot", true); }
			set { SetPropertyValue<bool>("CanSelectRoot", value); }
		}

		[Description("显示范围")]
		[ScriptControlProperty]
		[ClientPropertyName("listMask")]
		public UserControlObjectMask ListMask
		{
			get { return GetPropertyValue<UserControlObjectMask>("ListMask", UserControlObjectMask.All); }
			set { SetPropertyValue<UserControlObjectMask>("ListMask", value); }
		}

		[Description("选择范围")]
		[ScriptControlProperty]
		[ClientPropertyName("selectMask")]
		public UserControlObjectMask SelectMask
		{
			get { return GetPropertyValue<UserControlObjectMask>("SelectMask", UserControlObjectMask.All); }
			set { SetPropertyValue<UserControlObjectMask>("SelectMask", value); }
		}

		[Description("查询机构人员的超时时间")]
		public TimeSpan QueryUserTimeout
		{
			get { return GetPropertyValue<TimeSpan>("QueryUserTimeout", TimeSpan.FromSeconds(10)); }
			set { SetPropertyValue<TimeSpan>("QueryUserTimeout", value); }
		}

		[Description("是否合并选择结果")]
		[DefaultValue(true)]
		[ScriptControlProperty(), ClientPropertyName("mergeSelectResult")]
		public bool MergeSelectResult
		{
			get { return GetPropertyValue<bool>("MergeSelectResult", true); }
			set { SetPropertyValue<bool>("MergeSelectResult", value); }
		}

		/// <summary>
		/// 是否显示逻辑删除的对象
		/// </summary>
		[DefaultValue(false)]
		[ScriptControlProperty(), ClientPropertyName("showDeletedObjects")]
		public bool ShowDeletedObjects
		{
			get { return GetPropertyValue<bool>("ShowDeletedObjects", false); }
			set { SetPropertyValue<bool>("ShowDeletedObjects", value); }
		}

		/// <summary>
		/// 是否显示兼职人员
		/// </summary>
		/// <remarks>
		/// 是否显示兼职人员
		/// </remarks>
		[Description("是否显示兼职人员")]
		//[ScriptControlProperty]//设置此属性要输出到客户端
		//[ClientPropertyName("showSideLine")]//对应的客户端属性
		public bool ShowSideLine
		{
			get { return GetPropertyValue<bool>("ShowSideLine", true); }
			set { SetPropertyValue<bool>("ShowSideLine", value); }
		}

		/// <summary>
		/// 数据类型
		/// </summary>
		[Description("数据类型")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("dataType")]//对应的客户端属性
		public override string DataType
		{
			get
			{
				return GetPropertyValue<string>("DataType", "oguObject");
			}
			set
			{
				SetPropertyValue<string>("DataType", value);
			}
		}

		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(true)]
		[Localizable(true)]
		[ScriptControlProperty(), ClientPropertyName("enableUserPresence")]
		public bool EnableUserPresence
		{
			get
			{
				return ViewState.GetViewStateValue("EnableUserPresence", true);
			}
			set
			{
				ViewState.SetViewStateValue("EnableUserPresence", value);
			}
		}

		/// <summary>
		/// 数据检索字段名称，支持逗号分割
		/// </summary>
		[DefaultValue("")]
		[Bindable(true), Description("数据检索字段名称")]
		protected override string DataCompareFields
		{
			get
			{
				return "fullPath";
			}
		}

		/// <summary>
		/// 是否可以多选
		/// </summary>
		/// <remarks>
		/// 是否可以多选
		/// </remarks>
		[Description("是否可以多选  Single:单选   Multiple:多选")]
		[ScriptControlProperty]
		[ClientPropertyName("multiSelect")]
		public override bool MultiSelect
		{
			get { return GetPropertyValue<bool>("MultiSelect", true); }
			set
			{
				SetPropertyValue<bool>("MultiSelect", value);

				if (this.foOUCtrl != null)
					this.foOUCtrl.MultiSelect = value;
			}
		}

		private string sRootPath = string.Empty;

		[ScriptControlProperty]
		[ClientPropertyName("rootPath")]
		public string RootPath
		{
			get
			{
				string result = this.sRootPath;

				if (result.IsNullOrEmpty())
					result = OguPermissionSettings.GetConfig().RootOUPath;

				return result;
			}
			set
			{
				if (value.IsNullOrEmpty())
					this.sRootPath = OguPermissionSettings.GetConfig().RootOUPath;
				else
					this.sRootPath = value;
			}
		}

		/// <summary>
		/// 选择人员的图标
		/// </summary>
		/// <remarks>
		/// 选择人员的图标
		/// </remarks>
		[Description("选择人员的图标")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("userImg")]//对应的客户端属性
		public string UserImg
		{
			get
			{
				return GetPropertyValue<string>("UserImg",
					Page.ClientScript.GetWebResourceUrl(typeof(OuUserInputControl),
						"MCS.Web.Responsive.WebControls.OuUserInputControl.user.gif"));
			}
			set { SetPropertyValue<string>("UserImg", value); }
		}

		[Description("执行检察录入项目的图标")]
		[ScriptControlProperty]
		[ClientPropertyName("checkImg")]
		protected override string CheckImg
		{
			get
			{
				return GetPropertyValue<string>("CheckImg",
					Page.ClientScript.GetWebResourceUrl(typeof(OuUserInputControl),
						"MCS.Web.Responsive.WebControls.OuUserInputControl.checkUser.gif"));
			}
		}

		/// <summary>
		/// 可选择人员也可选择机构的图标
		/// </summary>
		/// <remarks>
		/// 可选择人员也可选择机构的图标
		/// </remarks>
		[Description("可选择人员也可选择机构的图标")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("ouUserImg")]//对应的客户端属性
		public string OuUserImg
		{
			get
			{
				return GetPropertyValue<string>("OuUserImg",
					Page.ClientScript.GetWebResourceUrl(typeof(OuUserInputControl),
						"MCS.Web.Responsive.WebControls.OuUserInputControl.group.gif"));
			}
			set { SetPropertyValue<string>("OuUserImg", value); }
		}

		private OguDataCollection<IOguObject> _selectedOuUserData;

		/// <summary>
		/// 选择的数据
		/// </summary>
		/// <remarks>
		/// OU，User的数据
		/// </remarks>
		[Description("选择的数据")]
		[Browsable(false)]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("selectedOuUserData")]//对应的客户端属性
		public OguDataCollection<IOguObject> SelectedOuUserData
		{
			get
			{
				if (this._selectedOuUserData != null)
					return this._selectedOuUserData;

				if (!DesignMode)
				{
					this._selectedOuUserData = new OguDataCollection<IOguObject>();
				}

				return this._selectedOuUserData;

			}
			set
			{
				this._selectedOuUserData = value;
			}
		}

		[Description("单选的数据")]
		[Browsable(false)]
		public IOguObject SelectedSingleData
		{
			get
			{
				IOguObject result = null;

				if (SelectedOuUserData.Count > 0)
					result = SelectedOuUserData[0];

				return result;
			}
			set
			{
				SelectedOuUserData.Clear();

				if (OguBase.IsNotNullOrEmpty(value))
					SelectedOuUserData.Add(value);
			}
		}

		[ScriptControlProperty(), ClientPropertyName("selectObjectDialogUrl")]
		[Browsable(false)]
		protected override string SelectObjectDialogUrl
		{
			get
			{
				return Page.ClientScript.GetWebResourceUrl(typeof(OuUserInputControl),
					"MCS.Web.Responsive.WebControls.OuUserInputControl.selectObjectDialog.htm");
			}
		}

		/// <summary>
		/// UserOUGraphControl客户端ID
		/// </summary>
		/// <remarks>
		///UserOUGraphControl客户端ID
		/// </remarks>
		[Browsable(false)]
		[Description("userOUGraphControlID客户端ID")]
		[ScriptControlProperty]
		[ClientPropertyName("userOUGraphControlID")]
		public string UserOUGraphControlID
		{
			get
			{
				string result = string.Empty;

				if (foOUCtrl != null)
					result = foOUCtrl.ClientID;

				return result;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[Browsable(true)]
		[Description("是否显示机构人员树选择控件")]
		[DefaultValue(true)]
		public bool ShowTreeSelector
		{
			get
			{
				return this.ShowSelector;
			}
			set
			{
				this.ShowSelector = value;
			}
		}

		/// <summary>
		/// 检查输入的回调方法名称
		/// </summary>
		[ScriptControlProperty]
		[ClientPropertyName("checkInputCallBackMethod")]
		public override string CheckInputCallBackMethod
		{
			get { return "CheckInputOuUser"; }
		}
		#endregion

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <remarks>
		/// 构造函数,客户端标记是一个DIV
		/// </remarks>
		public OuUserInputControl()
			: base(true, HtmlTextWriterTag.Div)
		{
			JSONSerializerExecute.RegisterConverter(typeof(OguObjectConverter));

			//在客户端，客户端控件对应的DomElement为DIV控件
			//this.inputAttribute = new System.Web.UI.AttributeCollection(this.ViewState);
		}

		protected void InnerOuUserGraphControl_TargetControlLoaded(Control targetControl)
		{
			if (targetControl is UserOUGraphControl && targetControl.ID.IndexOf(this.ClientID) > 0)
			{
				UserOUGraphControl userOUGraph = (UserOUGraphControl)targetControl;
				userOUGraph.ObjectsLoaded += new OuUserObjectsLoadedHandler(OnObjectsLoaded);
			}
		}

		//private System.Web.UI.AttributeCollection inputAttribute = null;
		private UserOUGraphControl foOUCtrl = new UserOUGraphControl();

		#region protected

		protected override void OnInit(EventArgs e)
		{
			StaticCallBackProxy.Instance.TargetControlLoaded += new StaticCallBackProxyControlLoadedEventHandler(InnerOuUserGraphControl_TargetControlLoaded);
			base.OnInit(e);
			EnsureChildControls();
		}

		protected override void OnPreRender(EventArgs e)
		{
			JSONSerializerExecute.RegisterTypeToClient(this.Page, "oguObject", typeof(OguBase));

			foOUCtrl.SelectMask = this.SelectMask;
			foOUCtrl.MergeSelectResult = this.MergeSelectResult;
			foOUCtrl.ShowDeletedObjects = this.ShowDeletedObjects;
			foOUCtrl.EnableUserPresence = this.EnableUserPresence;

			base.OnPreRender(e);

			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "请选择机构人员");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "请选择：");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "选择(S)");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "取消(C)");

			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "名称：");
			DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "位置：");
		}

		protected override void CreateChildControls()
		{
			if (!DesignMode)
			{
				foOUCtrl.ID = this.ClientID;
				foOUCtrl.ListMask = this.ListMask;//UserControlObjectMask.All;
				foOUCtrl.MultiSelect = this.MultiSelect;
				foOUCtrl.DialogTitle = "选择组织机构人员";
				foOUCtrl.ControlToShowDialog = base.TreeSelectorButton;
				foOUCtrl.RootExpanded = true;
				foOUCtrl.ShowingMode = ControlShowingMode.Dialog;
				foOUCtrl.ObjectsLoaded += new OuUserObjectsLoadedHandler(OnObjectsLoaded);

				HtmlGenericControl ctlSelectSpan = new HtmlGenericControl("SPAN");
				foOUCtrl.SelectMask = this.SelectMask;
				ctlSelectSpan.Controls.Add(foOUCtrl);

				this.Controls.Add(ctlSelectSpan);
				this.Controls.Add(new UserPresence());
			}

			base.CreateChildControls();
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);

			this._selectedOuUserData = (OguDataCollection<IOguObject>)ViewState["SelectedData"];
		}

		/// <summary>
		/// 将ClientState字符串的信息加载到ClientState中
		/// </summary>
		/// <remarks>
		/// 将ClientState字符串的信息加载到ClientState中
		/// </remarks>
		/// <param name="clientState">ClientState字符串</param>
		protected override void LoadClientState(string clientState)
		{
			if (clientState == null || clientState == "null")
				return;

			object[] foArray = JSONSerializerExecute.Deserialize<object[]>(clientState);

			if (null != foArray && foArray.Length > 0)
			{
				OguDataCollection<IOguObject> objs =
					(OguDataCollection<IOguObject>)JSONSerializerExecute.DeserializeObject(foArray[0], typeof(OguDataCollection<IOguObject>));

				this._selectedOuUserData = objs;

				if (foArray.Length > 1 && null != foArray[1])
					this.Text = foArray[1].ToString();
				else
					this.Text = "";
			}
			else
			{
				this.SelectedOuUserData = new OguDataCollection<IOguObject>();
			}
		}

		/// <summary>
		/// 将ClenteState中的信息生成ClientState字符串
		/// </summary>
		/// <returns>ClientState字符串</returns>
		protected override string SaveClientState()
		{
			object[] foArray = new object[2];
			var tmpRoot = UserOUControlSettings.GetConfig().UserOUControlQuery.GetOrganizationByPath(this.RootPath);

			if (tmpRoot != null)
				this.foOUCtrl.Root = tmpRoot;

			for (int i = 0; i < this.SelectedOuUserData.Count; i++)
				this.SelectedOuUserData[i] = (IOguObject)OguBase.CreateWrapperObject(this.SelectedOuUserData[i]);

			OccupyUserPresenceAddress(this.SelectedOuUserData);

			foArray[0] = this.SelectedOuUserData;
			foArray[1] = this.Text;

			return JSONSerializerExecute.Serialize(foArray);
		}

		protected override object SaveViewState()
		{
			ViewState["SelectedData"] = this._selectedOuUserData;

			return base.SaveViewState();
		}

		protected override void AutoCompleteExtender_GetDataSource(string sPrefix, int iCount, object context, ref IEnumerable result)
		{
			if (this.GetDataSource != null)
				this.GetDataSource(sPrefix, iCount, context, ref result);
			else
				InnerGetDataSource(sPrefix, iCount, context, ref result);

			OnObjectsLoaded(this, result);
		}

		protected void InnerGetDataSource(string sPrefix, int iCount, object context, ref IEnumerable result)
		{
			ServiceBrokerContext.Current.Timeout = QueryUserTimeout;

			IOrganization rootOrg = UserOUControlSettings.GetConfig().UserOUControlQuery.GetOrganizationByPath(this.RootPath);
			OguDataCollection<IOguObject> users = QueryChildrenBySelectMask(rootOrg, sPrefix);

			ArrayList arrList = new ArrayList();

			if (this.CanSelectRoot)
			{
				if (rootOrg.DisplayName.IndexOf(sPrefix) == 0)
				{
					arrList.Add(OguBase.CreateWrapperObject(rootOrg));
				}
			}

			for (int i = 0; i < users.Count; i++)
			{
				//资源类型过滤
				IOguObject oguObject = OguBase.CreateWrapperObject(users[i]);

				if (this.SelectMask == UserControlObjectMask.All || (((int)this.SelectMask & (int)oguObject.ObjectType)) != 0)
				{
					arrList.Add(oguObject);
				}
			}

			//修饰一下结果
			foreach (OguBase obj in arrList)
			{
				string path = string.Empty;
				if (obj.Parent != null)
					path = obj.Parent.FullPath;

				if (path.IndexOf(rootOrg.FullPath) == 0)
				{
					path = path.Substring(rootOrg.FullPath.Length);
					path = path.Trim('\\');
				}

				if (obj is OguBase)
				{
					// v-weirf changed : obj must be a OguBase to use Description
					if (obj is IUser)
						((OguBase)obj).Description = string.Format("{0} {1}", ((IUser)obj).Occupation, path);
					else if (obj is OguOrganization)
						((OguBase)obj).Description = path;
				}
			}

			result = arrList;
		}
		#endregion

		private void OnObjectsLoaded(object sender, IEnumerable result)
		{
			if (this.EnableUserPresence)
				OccupyUserPresenceAddress(result);

			if (ObjectsLoaded != null)
				ObjectsLoaded(this, result);
		}

		private static void OccupyUserPresenceAddress(IEnumerable objs)
		{
			List<string> userIDs = new List<string>();

			foreach (IOguObject obj in objs)
			{
				if (obj is IUser)
					userIDs.Add(obj.ID);
			}

			UserIMAddressCollection usersExtendedInfo = UserOUControlSettings.GetConfig().UserOUControlQuery.QueryUsersIMAddress(userIDs.ToArray());

			foreach (OguBase obj in objs)
			{
				if (obj is IUser)
				{
					UserIMAddress userIMAddresses = usersExtendedInfo.Find(e => string.Compare(e.UserID, obj.ID, true) == 0);

					if (userIMAddresses != null && userIMAddresses.IMAddress.IsNotEmpty())
						obj.ClientContext["IMAddress"] = UserPresence.NormalizeIMAddress(userIMAddresses.IMAddress);
				}
			}
		}

		private OguDataCollection<IOguObject> InnerCheckInputOuUser(string chkString)
		{
			ServiceBrokerContext.Current.Timeout = QueryUserTimeout;

			IOrganization root = UserOUControlSettings.GetConfig().UserOUControlQuery.GetOrganizationByPath(this.RootPath);

			OguDataCollection<IOguObject> users = QueryChildrenBySelectMask(root, chkString);
			OguDataCollection<IOguObject> forSelected = new OguDataCollection<IOguObject>();

			for (int i = 0; i < users.Count; i++)
			{
				if (((int)users[i].ObjectType & (int)this.SelectMask) != 0)
				{
					forSelected.Add(OguBase.CreateWrapperObject(users[i]));
				}
			}

			if (this.CanSelectRoot)
			{
				if (root.DisplayName.IndexOf(chkString) == 0)
				{
					forSelected.Add(OguBase.CreateWrapperObject(root));
				}
			}

			//沈峥添加，修饰一下Description
			//IOrganization root = OguMechanismFactory.GetMechanism().GetRoot();

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

				if (obj is OguBase)
				{
					//v-weirf changed, for Description setable,obj must be OguBase
					if (obj is IUser)
						((OguBase)obj).Description = string.Format("{0} {1}", ((IUser)obj).Occupation, path);
					else if (obj is IOrganization)
						((OguBase)obj).Description = path;
					else
						((OguBase)obj).Description = path;
				}
			}

			return forSelected;
		}

		#region 实现
		private OguDataCollection<IOguObject> QueryChildrenBySelectMask(IOrganization dept, string searchString)
		{
			ServiceBrokerContext.Current.SaveContextStates();
			try
			{
				var queryImpl = UserOUControlSettings.GetConfig().UserOUControlQuery;

				ServiceBrokerContext.Current.ListObjectCondition = ShowDeletedObjects ? ListObjectMask.All : ListObjectMask.Common;

				OguDataCollection<IOguObject> result = new OguDataCollection<IOguObject>();

				if (SelectMask == UserControlObjectMask.Organization)
				{
					OguObjectCollection<IOrganization> orgs = queryImpl.QueryDescendants<IOrganization>(SchemaQueryType.Organizations, dept, searchString, MaxQueryCount);  //dept.QueryChildren<IOrganization>(searchString, true, SearchLevel.SubTree, MaxQueryCount);
					foreach (IOrganization org in orgs)
						result.Add(org);
				}
				else
					if (SelectMask == UserControlObjectMask.Group)
					{
						OguObjectCollection<IGroup> groups = queryImpl.QueryDescendants<IGroup>(SchemaQueryType.Groups, dept, searchString, MaxQueryCount); // dept.QueryChildren<IGroup>(searchString, true, SearchLevel.SubTree, MaxQueryCount);
						foreach (IGroup group in groups)
							result.Add(group);
					}
					else
						if (SelectMask == UserControlObjectMask.User || SelectMask == (UserControlObjectMask.User | UserControlObjectMask.Sideline))
						{
							OguObjectCollection<IUser> users = queryImpl.QueryDescendants<IUser>(SchemaQueryType.Groups, dept, searchString, MaxQueryCount); //dept.QueryChildren<IUser>(searchString, true, SearchLevel.SubTree, MaxQueryCount);
							foreach (IUser user in users)
								result.Add(user);
						}
						else
						{
							if ((int)(SelectMask & (UserControlObjectMask.Group | UserControlObjectMask.User | UserControlObjectMask.Organization)) != 0)
							{
								OguObjectCollection<IOguObject> objs = queryImpl.QueryDescendants<IOguObject>(SchemaQueryType.Users | SchemaQueryType.Groups | SchemaQueryType.Organizations, dept, searchString, MaxQueryCount); //dept.QueryChildren<IOguObject>(searchString, true, SearchLevel.SubTree, MaxQueryCount);

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
		#endregion

		#region 回调事件
		/// <summary>
		/// 从客户端回掉，验证输入的内容
		/// </summary>
		/// <param name="chkString">通过这个参数传入的信息进行数据校验</param>
		[ScriptControlMethod]
		public OguDataCollection<IOguObject> CheckInputOuUser(string chkString, object context)
		{
			OguDataCollection<IOguObject> result = null;

			if (ValidateInputOuUser != null)
				result = ValidateInputOuUser(chkString, context);
			else
				result = InnerCheckInputOuUser(chkString);

			OnObjectsLoaded(this, result);

			return result;
		}
		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;
using MCS.Web.Library.Script;
using MCS.Web.Responsive.Library;
using MCS.Web.Responsive.Library.Script;
using MCS.Web.Responsive.WebControls;

[assembly: WebResource("MCS.Web.Responsive.WebControls.UserControl.UserOUGraphControl.js", "application/x-javascript")]

namespace MCS.Web.Responsive.WebControls
{
	/// <summary>
	/// 用户和机构选择控件
	/// </summary>
	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[ClientScriptResource("MCS.Web.WebControls.UserOUGraphControl", "MCS.Web.Responsive.WebControls.UserControl.UserOUGraphControl.js")]
	[ToolboxData("<{0}:UserOUGraphControl runat=server></{0}:UserOUGraphControl>")]
	public class UserOUGraphControl : DialogControlBase
	{
		private DeluxeTree tree = null;

		private HiddenButtonWrapper buttonWrapper = new HiddenButtonWrapper();

		private const string TreeExtendedDataKey = "TreeExtendedData";
		private DeluxeTreeNodeCollection rootNodesData = new DeluxeTreeNodeCollection(null);
		private OguDataCollection<IOguObject> selectedOuUserData = new OguDataCollection<IOguObject>();
		//private Control rootContainer = null;

		/// <summary>
		/// 构造方法
		/// </summary>
		public UserOUGraphControl()
		{
			JSONSerializerExecute.RegisterConverter(typeof(DeluxeTreeNodeConverter));
			JSONSerializerExecute.RegisterConverter(typeof(DeluxeTreeNodeListConverter));
			JSONSerializerExecute.RegisterConverter(typeof(OguObjectConverter));
		}

		private class InnerTreeContext
		{
			public bool MultiSelect
			{
				get;
				set;
			}

			public bool ShowDeletedObjects
			{
				get;
				set;
			}

			private UserControlObjectMask _ListMask = UserControlObjectMask.All;

			public UserControlObjectMask ListMask
			{
				get { return this._ListMask; }
				set { this._ListMask = value; }
			}

			private UserControlObjectMask _SelectMask = UserControlObjectMask.All;

			public UserControlObjectMask SelectMask
			{
				get { return this._SelectMask; }
				set { this._SelectMask = value; }
			}
		}

		public event LoadingObjectToTreeNodeDelegate LoadingObjectToTreeNode;
		public event GetChildrenDelegate GetChildren;
		public event OuUserObjectsLoadedHandler ObjectsLoaded;

		#region 属性

		/// <summary>
		/// 对话框的标题
		/// </summary>
		[DefaultValue("请选择机构")]
		[ScriptControlProperty(), ClientPropertyName("dialogTitle")]
		public override string DialogTitle
		{
			get
			{
				return GetPropertyValue<string>("DialogTitle", "请选择机构");
			}
			set
			{
				SetPropertyValue("DialogTitle", value);
			}
		}

		/// <summary>
		/// 对话框的宽度
		/// </summary>
		[DefaultValue("400px")]
		[ScriptControlProperty(), ClientPropertyName("dialogWidth")]
		public override string DialogWidth
		{
			get
			{
				return GetPropertyValue<string>("DialogWidth", "400px");
			}
			set
			{
				SetPropertyValue("DialogWidth", value);
			}
		}

		/// <summary>
		/// 对话框的高度
		/// </summary>
		[DefaultValue("450px")]
		[ScriptControlProperty(), ClientPropertyName("dialogHeight")]
		public override string DialogHeight
		{
			get
			{
				return GetPropertyValue<string>("DialogHeight", "450px");
			}
			set
			{
				SetPropertyValue("DialogHeight", value);
			}
		}

		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue(true)]
		[Localizable(true)]
		[ScriptControlProperty(), ClientPropertyName("enableUserPresence")]
		public bool EnableUserPresence
		{
			get { return GetPropertyValue<bool>("EnableUserPresence", true); }
			set { SetPropertyValue<bool>("EnableUserPresence", value); }
		}

		/// <summary>
		/// 回调时的上下文，由使用者提供
		/// </summary>
		[ScriptControlProperty]
		[DefaultValue("")]
		[ClientPropertyName("callBackContext")]//对应的客户端属性
		[Bindable(true), Description("回调时的上下文，由使用者提供")]
		public string CallBackContext
		{
			get { return GetPropertyValue<string>("callBackContext", ""); }
			set { SetPropertyValue<string>("callBackContext", value); }
		}

		/// <summary>
		/// 根机构
		/// </summary>
		[Browsable(false)]
		public IOrganization Root
		{
			get { return GetPropertyValue<IOrganization>("Root", null); }
			set { SetPropertyValue<IOrganization>("Root", value); }
		}

		[ScriptControlProperty(), ClientPropertyName("rootPath"), DefaultValue("")]
		public string RootPath
		{
			get { return GetPropertyValue<string>("RootPath", ""); }
			set { SetPropertyValue<string>("RootPath", value); }
		}

		/// <summary>
		/// 根节点是否展开（缺省为true）
		/// </summary>
		[DefaultValue(true)]
		public bool RootExpanded
		{
			get
			{
				return GetPropertyValue<bool>("RootExpanded", true);
			}
			set
			{
				SetPropertyValue<bool>("RootExpanded", value);
			}
		}

		/// <summary>
		/// 点击后，能够弹出对话框的控件ID
		/// </summary>
		[DefaultValue(""), IDReferenceProperty(), TypeConverter(typeof(DialogStartUpControlConverter))]
		public string ControlIDToShowDialog
		{
			get
			{
				return buttonWrapper.TargetControlID;
			}
			set
			{
				buttonWrapper.TargetControlID = value;
			}
		}

		/// <summary>
		/// 点击后，能够弹出对话框的控件的实例
		/// </summary>
		[Browsable(false)]
		public IAttributeAccessor ControlToShowDialog
		{
			get
			{
				return buttonWrapper.TargetControl;
			}
			set
			{
				buttonWrapper.TargetControl = value;
			}
		}

		/// <summary>
		/// 能够列出哪些对象（机构、人员、组）
		/// </summary>
		[ScriptControlProperty(), ClientPropertyName("listMask"), DefaultValue(UserControlObjectMask.All)]
		public UserControlObjectMask ListMask
		{
			get
			{
				return GetPropertyValue<UserControlObjectMask>("ListMask", UserControlObjectMask.All);
			}
			set
			{
				SetPropertyValue<UserControlObjectMask>("ListMask", value);
			}
		}

		/// <summary>
		/// 能够选择哪些对象（机构、人员、组）
		/// </summary>
		[ScriptControlProperty(), ClientPropertyName("selectMask"), DefaultValue(UserControlObjectMask.All)]
		public UserControlObjectMask SelectMask
		{
			get
			{
				return GetPropertyValue<UserControlObjectMask>("SelectMask", UserControlObjectMask.All);
			}
			set
			{
				SetPropertyValue<UserControlObjectMask>("SelectMask", value);
			}
		}

		/// <summary>
		/// 是否多选
		/// </summary>
		[ScriptControlProperty(), ClientPropertyName("multiSelect")]
		[DefaultValue(false)]
		public bool MultiSelect
		{
			get
			{
				return GetPropertyValue<bool>("MultiSelect", false);
			}
			set
			{
				SetPropertyValue<bool>("MultiSelect", value);
			}
		}

		/// <summary>
		/// 是否合并选择结果
		/// </summary>
		[ScriptControlProperty(), ClientPropertyName("mergeSelectResult")]
		[DefaultValue(false)]
		public bool MergeSelectResult
		{
			get
			{
				return GetPropertyValue<bool>("MergeSelectResult", false);
			}
			set
			{
				SetPropertyValue<bool>("MergeSelectResult", value);
			}
		}

		/// <summary>
		/// 是否显示逻辑删除的对象
		/// </summary>
		[DefaultValue(false)]
		[ScriptControlProperty(), ClientPropertyName("showDeletedObjects")]
		public bool ShowDeletedObjects
		{
			get
			{
				return GetPropertyValue<bool>("ShowDeletedObjects", false);
			}
			set
			{
				SetPropertyValue<bool>("ShowDeletedObjects", value);
			}
		}

		/// <summary>
		/// 树控件的客户端ID
		/// </summary>
		[ScriptControlProperty(), ClientPropertyName("treeControlID")]
		[Browsable(false)]
		private string TreeControlClientID
		{
			get
			{
				EnsureChildControls();

				return tree.ClientID;
			}
		}

		/// <summary>
		/// 已经选择的对象
		/// </summary>
		[Browsable(false)]
		public OguDataCollection<IOguObject> SelectedOuUserData
		{
			get
			{
				return this.selectedOuUserData;
			}
			set
			{
				this.selectedOuUserData = value;
			}
		}
		#endregion

		#region 客户端事件
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("isChildrenOf")]
		[Bindable(true), Category("ClientEventsHandler"), Description("客户端判断两个对象的父子关系")]
		public string OnIsChildrenOf
		{
			get
			{
				string result = string.Empty;

				if (CurrentMode == ControlShowingMode.Normal)
					result = GetPropertyValue("isChildrenOf", string.Empty);

				return result;
			}
			set
			{
				SetPropertyValue("isChildrenOf", value);
			}
		}

		/// <summary>
		/// 选择用户和机构的对话框确定关闭后的客户端事件
		/// </summary>
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("dialogConfirmed")]
		[Bindable(true), Category("ClientEventsHandler"), Description("选择用户和机构的对话框确定关闭后的客户端事件")]
		public string OnDialogConfirmed
		{
			get
			{
				string result = string.Empty;

				if (CurrentMode == ControlShowingMode.Normal)
					result = GetPropertyValue("dialogConfirmed", string.Empty);

				return result;
			}
			set
			{
				SetPropertyValue("dialogConfirmed", value);
			}
		}

		/// <summary>
		/// 节点在选中前的客户端事件
		/// </summary>
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("nodeSelecting")]
		[Bindable(true), Category("ClientEventsHandler"), Description("节点在选中前的客户端事件")]
		public string OnNodeSelecting
		{
			get
			{
				string result = string.Empty;

				if (CurrentMode == ControlShowingMode.Normal)
					result = GetPropertyValue("onNodeSelecting", string.Empty);

				return result;
			}
			set
			{
				SetPropertyValue("onNodeSelecting", value);
			}
		}
		#endregion

		#region private
		private DeluxeTreeNodeCollection RootNodesData
		{
			get
			{
				return this.rootNodesData;
			}
		}

		/// <summary>
		/// 初始化树的上下文属性
		/// </summary>
		private void InitTreeContext()
		{
			InnerTreeContext context = new InnerTreeContext();

			context.MultiSelect = this.MultiSelect;
			context.ListMask = this.ListMask;
			context.SelectMask = this.SelectMask;
			context.ShowDeletedObjects = this.ShowDeletedObjects;

			this.tree.CallBackContext = JSONSerializerExecute.Serialize(context);
		}

		private void InnerTree_TargetControlLoaded(Control targetControl)
		{
			if ((targetControl is DeluxeTree) && targetControl.ID.IndexOf("oguInnerTree") >= 0)
			{
				DeluxeTree tree = (DeluxeTree)targetControl;

				tree.GetChildrenData += new DeluxeTree.GetChildrenDataDelegate(tree_GetChildrenData);
			}
		}

		private void InitShowDialogControl()
		{
			if (ControlToShowDialog != null)
				ControlToShowDialog.SetAttribute("onclick", string.Format("$find('{0}').showDialog(); return false;", this.ClientID));
		}

		private void InitRootTreeNode()
		{
			IOrganization innerRoot = this.Root;

			ServiceBrokerContext.Current.SaveContextStates();

			try
			{
				if (ShowDeletedObjects)
				{
					ServiceBrokerContext.Current.UseLocalCache = false;
					ServiceBrokerContext.Current.ListObjectCondition = ListObjectMask.All;
				}

				if (innerRoot == null)
				{
					var tmpRootPath = this.RootPath;
					if (string.IsNullOrEmpty(tmpRootPath))
					{
						tmpRootPath = OguPermissionSettings.GetConfig().RootOUPath;
					}
					innerRoot = UserOUControlSettings.GetConfig().UserOUControlQuery.GetOrganizationByPath(tmpRootPath); // OguMechanismFactory.GetMechanism().GetRoot();
				}

				DeluxeTreeNode rootNode = new DeluxeTreeNode();

				BindOguObjToTreeNode((IOrganization)OguBase.CreateWrapperObject(innerRoot), rootNode, MultiSelect, SelectMask);

				rootNodesData.Add(rootNode);

				if (RootExpanded)
				{
					rootNode.ChildNodesLoadingType = ChildNodesLoadingTypeDefine.Normal;
					rootNode.Expanded = true;
					BindChildren(rootNode.Nodes, OnGetChildren(innerRoot), MultiSelect, ListMask, SelectMask);
				}
			}
			finally
			{
				ServiceBrokerContext.Current.RestoreSavedStates();
			}
		}

		private void tree_GetChildrenData(DeluxeTreeNode parentNode, DeluxeTreeNodeCollection result, string callBackContext)
		{
			ServiceBrokerContext.Current.SaveContextStates();
			try
			{
				InnerTreeContext context = JSONSerializerExecute.Deserialize<InnerTreeContext>(callBackContext);

				if (context.ShowDeletedObjects)
				{
					ServiceBrokerContext.Current.UseLocalCache = false;
					ServiceBrokerContext.Current.ListObjectCondition = ListObjectMask.All;
				}
				else
					ServiceBrokerContext.Current.ListObjectCondition = ListObjectMask.Common;

				OguObjectCollection<IOguObject> parents = UserOUControlSettings.GetConfig().UserOUControlQuery.GetObjects(((IOguObject)parentNode.ExtendedData).ID);
				//; OguMechanismFactory.GetMechanism().GetObjects<IOguObject>(
				//    SearchOUIDType.Guid, ((IOguObject)parentNode.ExtendedData).ID);

				BindChildren(result, OnGetChildren(parents[0]), context.MultiSelect, context.ListMask, context.SelectMask);
			}
			finally
			{
				ServiceBrokerContext.Current.RestoreSavedStates();
			}
		}

		private IEnumerable<IOguObject> OnGetChildren(IOguObject parent)
		{
			IEnumerable<IOguObject> result = null;

			if (GetChildren != null)
				result = GetChildren(this, parent);
			else
				result = UserOUControlSettings.GetConfig().UserOUControlQuery.GetChildren((IOrganization)parent);

			return result;
		}

		private void BindChildren(DeluxeTreeNodeCollection nodes, IEnumerable<IOguObject> objects, bool multiSelect, UserControlObjectMask listMask, UserControlObjectMask selectMask)
		{
			OguDataCollection<IOguObject> wrappedObjects = CreateWrappedObjects(objects);

			foreach (IOguObject obj in wrappedObjects)
			{
				DeluxeTreeNode treeNode = new DeluxeTreeNode();
				bool cancel = false;

				BindOguObjToTreeNode(obj, treeNode, multiSelect, selectMask);

				FilterObjectToTreeNode(obj, treeNode, listMask, ref cancel);

				if (cancel == false)
				{
					if (LoadingObjectToTreeNode != null)
						LoadingObjectToTreeNode(this, obj, treeNode, ref cancel);
				}

				if (this.selectedOuUserData.FindSingleObjectByFullPath(obj.FullPath) != null)
					treeNode.Checked = true;

				if (cancel == false)
					nodes.Add(treeNode);
			}

			if (ObjectsLoaded != null)
				ObjectsLoaded(this, wrappedObjects);
		}

		private OguDataCollection<IOguObject> CreateWrappedObjects(IEnumerable<IOguObject> objects)
		{
			OguDataCollection<IOguObject> wrappedObjects = new OguDataCollection<IOguObject>();

			OguDataCollection<IUser> usersNeedToGetPresence = new OguDataCollection<IUser>();

			foreach (IOguObject obj in objects)
			{
				if (this.EnableUserPresence && obj is IUser)
					usersNeedToGetPresence.Add((IUser)obj);

				wrappedObjects.Add(OguBase.CreateWrapperObject(obj));
			}

			if (this.EnableUserPresence)
				OccupyUserPresenceAddress(wrappedObjects, usersNeedToGetPresence);

			return wrappedObjects;
		}

		private static void OccupyUserPresenceAddress(IEnumerable<IOguObject> wrappedObjects, IEnumerable<IUser> usersNeedToGetPresence)
		{
			List<string> userIDs = new List<string>();

			usersNeedToGetPresence.ForEach(u => userIDs.Add(u.ID));

			UserIMAddressCollection usersExtendedInfo = UserOUControlSettings.GetConfig().UserOUControlQuery.QueryUsersIMAddress(userIDs.ToArray());

			foreach (OguBase obj in wrappedObjects)
			{
				if (obj is IUser)
				{
					UserIMAddress userIMAddresses = usersExtendedInfo.Find(e => string.Compare(e.UserID, obj.ID, true) == 0);

					if (userIMAddresses != null && userIMAddresses.IMAddress.IsNotEmpty())
						obj.ClientContext["IMAddress"] = UserPresence.NormalizeIMAddress(userIMAddresses.IMAddress);
				}
			}
		}

		private static void FilterObjectToTreeNode(IOguObject obj, DeluxeTreeNode treeNode, UserControlObjectMask listMask, ref bool cancel)
		{
			int mask = (int)obj.ObjectType & (int)listMask;

			if (mask == 0)
				cancel = true;
		}

		private void BindOguObjToTreeNode(IOguObject obj, DeluxeTreeNode treeNode, bool multiSelect, UserControlObjectMask selectMask)
		{
			treeNode.Text = obj.DisplayName;

			if (obj.ObjectType == SchemaType.Organizations)
				treeNode.ChildNodesLoadingType = ChildNodesLoadingTypeDefine.LazyLoading;

			string nodeImg = GetImageUrlByObjectType(obj, treeNode);

			treeNode.NodeOpenImg = nodeImg;
			treeNode.NodeCloseImg = nodeImg;

			treeNode.ShowCheckBox = multiSelect && (((int)obj.ObjectType & (int)selectMask) != 0);
			treeNode.ExtendedData = obj;
			treeNode.ExtendedDataKey = TreeExtendedDataKey;
		}

		private string GetImageUrlByObjectType(IOguObject obj, DeluxeTreeNode treeNode)
		{
			string result = ControlResources.OULogoUrl;

			SchemaType objType = obj.ObjectType;

			switch (objType)
			{
				case SchemaType.Organizations:
					result = ControlResources.OULogoUrl;

					if ((int)obj.Properties.GetValue("STATUS", 1) == 3)
						result = ControlResources.DisabledOULogoUrl;
					break;
				case SchemaType.Users:
					if (this.EnableUserPresence)
					{
						result = UserPresence.GetDefaultStatusImageUrl();
						treeNode.ImgHeight = 12;
					}
					else
					{
						result = ControlResources.UserLogoUrl;
					}

					if ((int)obj.Properties.GetValue("STATUS", 1) == 3)
						result = ControlResources.DisabledUserLogoUrl;

					break;
				case SchemaType.Groups:
					result = ControlResources.GroupLogoUrl;

					if ((int)obj.Properties.GetValue("STATUS", 1) == 3)
						result = ControlResources.DisabledGroupLogoUrl;
					break;
			}

			return result;
		}
		#endregion

		#region protected
		protected override void OnInit(EventArgs e)
		{
			JSONSerializerExecute.RegisterTypeToClient(this.Page, TreeExtendedDataKey, typeof(OguBase));

			StaticCallBackProxy.Instance.TargetControlLoaded += new StaticCallBackProxyControlLoadedEventHandler(InnerTree_TargetControlLoaded);

			if (this.RenderMode.OnlyRenderSelf && ShowingMode == ControlShowingMode.Dialog)
			{
				LoadDataFromQueryString();
			}

			base.OnInit(e);
		}

		private void LoadDataFromQueryString()
		{
			this.RootPath = Request.GetRequestQueryValue("ucpRoot", string.Empty);
			this.ListMask = Request.GetRequestQueryValue("ucpListMask", UserControlObjectMask.All);
			this.SelectMask = Request.GetRequestQueryValue("ucpSelectMask", UserControlObjectMask.All);
			this.MultiSelect = Request.GetRequestQueryValue("ucpMultiSelect", false);
			this.MergeSelectResult = Request.GetRequestQueryValue("msr", false);
			this.ShowDeletedObjects = Request.GetRequestQueryValue("sdo", false);
			this.EnableUserPresence = Request.GetRequestQueryValue("eup", true);
		}

		//public event GetChildrenDelegete
		protected override void OnPagePreLoad(object sender, EventArgs e)
		{
			EnsureChildControls();

			base.OnPagePreLoad(sender, e);
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);

			HiddenButtonWrapper bw = GetPropertyValue("ButtonWrapper", (HiddenButtonWrapper)null);
			buttonWrapper.TargetControlID = bw.TargetControlID;

			this.selectedOuUserData = (OguDataCollection<IOguObject>)this.ViewState["SelectedData"];
		}

		protected override object SaveViewState()
		{
			SetPropertyValue("ButtonWrapper", this.buttonWrapper);
			this.ViewState["SelectedData"] = this.selectedOuUserData;

			return base.SaveViewState();
		}

		///// <summary>
		///// 设置对话框窗口的参数
		///// </summary>
		///// <returns></returns>
		//protected override string GetDialogFeature()
		//{
		//    WindowFeature feature = new WindowFeature();

		//    feature.Width = 360;
		//    feature.Height = 400;
		//    feature.Center = true;
		//    feature.Resizable = true;
		//    feature.ShowScrollBars = false;
		//    feature.ShowStatusBar = false;

		//    return feature.ToDialogFeatureClientString();
		//}

		/// <summary>
		/// OnPreRender
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			if (Page.IsCallback == false)
			{
				if (CurrentMode == ControlShowingMode.Dialog)
					Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
				else
				{
					if (ShowingMode == ControlShowingMode.Dialog)
						InitShowDialogControl();
				}

				if (ShowingMode == ControlShowingMode.Normal || CurrentMode == ControlShowingMode.Dialog)
					InitRootTreeNode();

				if (this.EnableUserPresence)
				{
					Controls.Add(new UserPresence());
				}

				InitTreeContext();

				DeluxeNameTableContextCache.Instance.Add(Define.DefaultCulture, "请选择机构或人员。");
			}

			base.OnPreRender(e);
		}

		protected override void CreateChildControls()
		{
			this.Style["overflow"] = "auto";

			this.tree = new DeluxeTree();
			this.tree.ID = "oguInnerTree";
			this.tree.GetChildrenData += new DeluxeTree.GetChildrenDataDelegate(tree_GetChildrenData);
			this.tree.Width = Unit.Percentage(100);
			this.tree.Height = Unit.Percentage(100);
			this.tree.ShowLines = false;

			Controls.Add(this.tree);

			base.CreateChildControls();
		}

		//protected override void InitDialogContent(Control container)
		//{
		//    this.rootContainer = container;

		//    base.InitDialogContent(container);

		//    container.Controls.Add(this.tree);

		//    HtmlForm form = (HtmlForm)WebControlUtility.FindParentControl(this, typeof(HtmlForm), true);

		//    if (form != null)
		//    {
		//        form.Style["width"] = "100%";
		//        form.Style["height"] = "100%";
		//    }

		//    this.Width = Unit.Percentage(100);
		//    this.Height = Unit.Percentage(100);
		//}

		//protected override void InitConfirmButton(HtmlInputButton confirmButton)
		//{
		//    base.InitConfirmButton(confirmButton);

		//    confirmButton.Attributes["treeControlID"] = tree.ClientID;
		//    confirmButton.Attributes["userControlID"] = this.ClientID;
		//    confirmButton.Attributes["multiSelect"] = this.MultiSelect.ToString().ToLower();
		//    confirmButton.Attributes["onclick"] = "onConfirmButtonClick();";
		//}

		/// <summary>
		/// 保存状态到客户端
		/// </summary>
		/// <returns>序列化后的JSON字符串</returns>
		protected override string SaveClientState()
		{
			if (this.Page.IsCallback == false)
				return JSONSerializerExecute.Serialize(new object[] { RootNodesData, this.selectedOuUserData });
			else
				return string.Empty;
		}

		protected override void LoadClientState(string clientState)
		{
			if (string.IsNullOrEmpty(clientState) == false)
			{
				object[] state = (object[])JSONSerializerExecute.DeserializeObject(clientState, typeof(object[]));
				object[] state0 = (object[])state[0];

				List<IOguObject> selectedResult = new List<IOguObject>();

				foreach (IOguObject obj in state0)
					selectedResult.Add(obj);

				this.selectedOuUserData = new OguDataCollection<IOguObject>(selectedResult);
			}
		}
		#endregion
	}
}

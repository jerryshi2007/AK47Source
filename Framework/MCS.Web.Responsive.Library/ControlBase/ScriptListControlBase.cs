using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Web.Responsive.Library.Script
{
	[RequiredScript(typeof(ControlBaseScript))]
	public partial class ScriptListControlBase : ListControl, IScriptControl, INamingContainer, IControlResolver, IPostBackDataHandler, IPostBackEventHandler, ICallbackEventHandler, IClientStateManager
	{
		#region [ Fields ]

		private ScriptManager _scriptManager;
		private bool _enableClientState;
		private bool _autoClearClientStateFieldValue = true;
		private string _cachedClientStateFieldID;
		private string _callbackArgument;
		private string _tagName;
		//private HtmlTextWriterTag _tagKey;
		private ControlRenderMode _RenderMode = null;

		#endregion

		#region [ Properties ]
		/// <summary>
		/// 控件输出模式
		/// </summary>        
		protected virtual ControlRenderMode RenderMode
		{
			get
			{
				if (_RenderMode == null)
				{
					_RenderMode =
						!this.DesignMode ?
						ScriptControlHelper.GetControlRenderMode(this) :
						new ControlRenderMode();
				}
				return _RenderMode;
			}
		}
		/// <summary>
		/// 是否只读
		/// </summary>
		[DefaultValue(false)]
		[Category("Appearance")]
		[Description("是否只读")]
		public bool ReadOnly
		{
			get { return GetPropertyValue<bool>("ReadOnly", false); }
			set { SetPropertyValue<bool>("ReadOnly", value); }
		}

		/// <summary>
		/// For debugging - setting this causes the extender to load the specified script instead of the one out of the resources.  This
		/// lets you set breakpoints and modify the script without rebuilding, etc.
		/// </summary>
		/// <remarks>
		/// Note to inheritors: If you do not wish the user to set the script path, override script path and throw a NotSupportedException on set.  Also decorate the ScriptPath attribute with a [Browsable(false)] and [EditorBrowsableState(EditorBrowsableState.Never)]
		/// </remarks>
		[DefaultValue("")]
		[Browsable(false)]
		public virtual string ScriptPath
		{
			get { return (string)(ViewState["ScriptPath"] ?? string.Empty); }
			set { ViewState["ScriptPath"] = value; }
		}

		/// <summary>
		/// The script type to use for the ScriptControl
		/// </summary>
		protected virtual string ClientControlType
		{
			get
			{
				ClientScriptResourceAttribute attr = (ClientScriptResourceAttribute)TypeDescriptor.GetAttributes(this)[typeof(ClientScriptResourceAttribute)];
				ExceptionHelper.TrueThrow(attr == null, string.Format("{0}控件中没有ClientScriptResourceAttribute声明！", this.GetType().Name));

				return attr.ComponentType;
			}
		}

		/// <summary>
		/// Whether this control supports ClientState
		/// </summary>
		/// <remarks>
		/// Note to inheritors: You should either pass true to the constructor for enableClientState or override this property to enable client state for inherited controls.
		/// </remarks>
		protected virtual bool SupportsClientState
		{
			get { return _enableClientState; }
		}

		/// <summary>
		/// Gets the ScriptManager for the page
		/// </summary>
		protected ScriptManager ScriptManager
		{
			get
			{
				EnsureScriptManager();
				return _scriptManager;
			}
		}

		/// <summary>
		/// 是否自动清除客户端ClientStateField中的值
		/// </summary>
		protected virtual bool AutoClearClientStateFieldValue
		{
			get
			{
				return this._autoClearClientStateFieldValue;
			}
		}

		/// <summary>
		/// The ID of the ClientState field
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", Justification = "Following ASP.NET AJAX pattern")]
		protected string ClientStateFieldID
		{
			get
			{
				if (_cachedClientStateFieldID == null)
				{
					_cachedClientStateFieldID = ClientID + "_ClientState";
				}
				return _cachedClientStateFieldID;
			}
		}

		///// <summary>
		///// Gets the tag key used when rendering the outer wrapper element for this user control
		///// </summary>
		//protected override HtmlTextWriterTag TagKey
		//{
		//    get { return _tagKey; }
		//}

		/// <summary>
		/// Gets the tag name used when rendering the outer wrapper element for this user control
		/// </summary>
		protected override string TagName
		{
			get
			{
				if (_tagName == null && TagKey != HtmlTextWriterTag.Unknown)
				{
					_tagName = Enum.Format(typeof(HtmlTextWriterTag), TagKey, "G").ToLower(CultureInfo.InvariantCulture);
				}
				return _tagName;
			}
		}

		#endregion

		#region [ Methods ]

		/// <summary>
		/// Ensures a ScriptManager exists on the Page for this Control
		/// </summary>
		private void EnsureScriptManager()
		{
			ScriptControlHelper.EnsureScriptManager(ref this._scriptManager, this.Page);

			//if (_scriptManager == null)
			//{
			//    _scriptManager = ScriptManager.GetCurrent(Page);
			//    if (_scriptManager == null)
			//    {
			//        ExceptionHelper.TrueThrow(this.Page.Form.Controls.IsReadOnly, Resources.DeluxeWebResource.E_NoScriptManager);

			//        _scriptManager = new ScriptManager();
			//        _scriptManager.ScriptMode = ScriptMode.Release;
			//        _scriptManager.EnableScriptGlobalization = true;
			//        this.Page.Form.Controls.Add(_scriptManager);
			//        //throw new HttpException(Resources.DeluxeWebResource.E_NoScriptManager);
			//    }
			//}
		}

		/// <summary>
		/// 重载OnInit，增加事件
		/// </summary>
		/// <param name="e">事件参数</param>
		protected override void OnInit(EventArgs e)
		{
			ScriptControlHelper.CheckOnlyRenderSelf(this, this.RenderMode);
			base.OnInit(e);
		}

		/// <summary>
		/// Finds a control reference by its ID
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public override Control FindControl(string id)
		{
			Control control = base.FindControl(id);
			if (control != null)
			{
				return control;
			}
			for (Control container = NamingContainer; container != null; container = container.NamingContainer)
			{
				control = container.FindControl(id);
				if (control != null)
				{
					return control;
				}
			}
			// NOTE: [rb] I'm not implementing ResolveControlID just yet. I prefer to use a colon (:) seperated ID name to get to controls inside of a naming container
			// e.g. TargetControlID="LoginView1:LoginButton" works just as well
			return null;
		}

		/// <summary>
		/// 在页面Load之前，确保ScriptManager加载
		/// </summary>
		/// <param name="sender">事件触发对象</param>
		/// <param name="e">事件参数</param>
		protected override void OnPagePreLoad(object sender, EventArgs e)
		{
			base.OnPagePreLoad(sender, e);
			EnsureScriptManager();
		}

		/// <summary>
		/// Fires the PreRender event
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			EnsureID();

			// EnsureScriptManager();

			if (SupportsClientState)
			{
				Page.ClientScript.RegisterHiddenField(ClientStateFieldID, SaveClientState());
				Page.RegisterRequiresPostBack(this);
			}

			ScriptManager.RegisterScriptControl<ScriptListControlBase>(this);

			ScriptObjectBuilder.RegisterCssReferences(this);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(HtmlTextWriter writer)
		{
			base.Render(writer);
			if (!base.DesignMode)
			{
				this.ScriptManager.RegisterScriptDescriptors(this);
			}
		}

		/// <summary>
		/// Loads the client state data
		/// </summary>
		/// <param name="clientState"></param>
		protected virtual void LoadClientState(string clientState)
		{
		}

		/// <summary>
		/// Saves the client state data
		/// </summary>
		/// <returns></returns>
		protected virtual string SaveClientState()
		{
			return null;
		}

		/// <summary>
		/// Executed when post data is loaded from the request
		/// </summary>
		/// <param name="postDataKey"></param>
		/// <param name="postCollection"></param>
		/// <returns></returns>
		protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
		{
			if (SupportsClientState)
			{
				string clientState = postCollection[ClientStateFieldID];
				if (!string.IsNullOrEmpty(clientState) && !Page.IsCallback)
				{
					LoadClientState(clientState);
				}
			}
			return false;
		}

		/// <summary>
		/// Executed when post data changes should invoke a chagned event
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Maintaining consistency with IPostBackDataHandler")]
		protected virtual void RaisePostDataChangedEvent()
		{
		}

		/// <summary>
		/// Gets the ScriptDescriptors that make up this control
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
		{
			if (!IsRenderScript) return null;

			EnsureID();

			// store descriptors for this object
			List<ScriptDescriptor> descriptors = new List<ScriptDescriptor>();

			// build the default description
			ScriptControlDescriptor descriptor = new ScriptControlDescriptor(ClientControlType, ClientID);
			DescribeComponent(descriptor);
			descriptors.Add(descriptor);

			// return the description
			return descriptors;
		}

		/// <summary>
		/// Gets the script references for this control
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerable<ScriptReference> GetScriptReferences()
		{
			if (!IsRenderScript) return null;

			return ScriptControlHelper.GetScriptReferences(this.GetType(), this.ScriptPath);
		}

		/// <summary>
		/// 是否输出本控件脚本
		/// </summary>
		protected virtual bool IsRenderScript
		{
			get
			{
				return Visible && !ReadOnly && RenderMode.IsHtmlRender;
			}
		}

		/// <summary>
		/// Describes the settings for this control.
		/// </summary>
		/// <param name="descriptor"></param>
		protected virtual void DescribeComponent(ScriptComponentDescriptor descriptor)
		{
			ScriptObjectBuilder.DescribeComponent(this, descriptor, this, this);
			if (SupportsClientState)
			{
				descriptor.AddElementProperty("clientStateField", ClientStateFieldID);
				descriptor.AddProperty("_autoClearClientStateFieldValue", AutoClearClientStateFieldValue);
			}
		}

		/// <summary>
		/// Handles a callback event
		/// </summary>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Method has a side-effect")]
		protected virtual string GetCallbackResult()
		{
			string argument = _callbackArgument;
			_callbackArgument = null;

			Dictionary<string, object> callInfo = JSONSerializerExecute.DeserializeObject(argument, typeof(Dictionary<string, object>)) as Dictionary<string, object>;

			return ScriptObjectBuilder.ExecuteCallbackMethod(this, callInfo);
		}

		/// <summary>
		/// Raises a callback event
		/// </summary>
		/// <param name="eventArgument"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Maintaining consistency with ICallbackEventHandler")]
		protected virtual void RaiseCallbackEvent(string eventArgument)
		{
			_callbackArgument = eventArgument;
		}

		/// <summary>
		/// 处理将窗体发送到服务器时引发的事件
		/// </summary>
		/// <param name="eventArgument">表示要传递到事件处理程序的可选事件参数的 String</param>
		protected virtual void RaisePostBackEvent(string eventArgument)
		{
		}

		/// <summary>
		/// Restores view-state information from a previous request that was saved with the SaveViewState method.
		/// 并在LoadViewState之前缓存ViewState中所有IStateManager类型项
		/// 以及在LoadViewState之后从缓存中恢复将ViewState中ViewSateItemInternal类型项恢复成IStateManager类型项
		/// </summary>
		/// <param name="savedState">An object that represents the control state to restore.</param>
		protected override void LoadViewState(object savedState)
		{
            StateBag backState = ViewState.PreLoadViewState();
			base.LoadViewState(savedState);
            ViewState.AfterLoadViewState(backState);
		}

		/// <summary>
		/// Saves any state that was modified after the TrackViewState method was invoked
		/// 并在SaveViewState之前，将ViewState中所有IStateManager类型项转换为可序列化的ViewSateItemInternal类型项
		/// 以及在SaveViewState之后，将ViewState中所有ViewSateItemInternal类型项恢复成IStateManager类型项
		/// </summary>
		/// <returns></returns>
		protected override object SaveViewState()
		{
            ViewState.PreSaveViewState();
			object o = base.SaveViewState();
            ViewState.AfterSavedViewState();

			return o;
		}

		/// <summary>
		/// Causes the control to track changes to its view state so they can be stored in the object's ViewState property
		/// 并将ViewState中所有IStateManager类型项标记为TrackViewState
		/// </summary>
		protected override void TrackViewState()
		{
			base.TrackViewState();
            ViewState.TrackViewState();
		}

		/// <summary>
		/// 判断是否只是输出本控件，如果是则进行只输出本控件处理，否则进行正常的控件输出
		/// </summary>
		/// <param name="writer">接收控件内容的 HtmlTextWriter 对象。</param>
		public override void RenderControl(HtmlTextWriter writer)
		{
			if (this.RenderMode.OnlyRenderSelf)
			{
				StringBuilder strB = new StringBuilder();
				StringWriter sw = new StringWriter(strB);
				using (HtmlTextWriter baseWriter = new HtmlTextWriter(sw))
				{
					base.RenderControl(baseWriter);
				}

				this.RenderControlOnlySelf(strB.ToString(), this.RenderMode);
			}
			else
				base.RenderControl(writer);
		}

		#endregion

		#region [ IScriptControl Members ]
		IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors()
		{
			return this.GetScriptDescriptors();
		}

		IEnumerable<ScriptReference> IScriptControl.GetScriptReferences()
		{
			return this.GetScriptReferences();
		}
		#endregion

		#region [ IControlResolver Members ]
		/// <summary>
		/// 通过controlId，查找相应Control
		/// </summary>
		/// <param name="controlId">controlId</param>
		/// <returns>Control</returns>
		public Control ResolveControl(string controlId)
		{
			return FindControl(controlId);
		}

		#endregion

		#region [ IPostBackDataHandler Members ]

		bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection)
		{
			return LoadPostData(postDataKey, postCollection);
		}

		void IPostBackDataHandler.RaisePostDataChangedEvent()
		{
			RaisePostDataChangedEvent();
		}

		#endregion

		#region [ ICallbackEventHandler Members ]

		string ICallbackEventHandler.GetCallbackResult()
		{
			return GetCallbackResult();
		}

		void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
		{
			RaiseCallbackEvent(eventArgument);
		}

		#endregion

		#region IPostBackEventHandler Members
		void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
		{
			this.RaisePostBackEvent(eventArgument);
		}
		#endregion

		#region [ IClientStateManager Members ]

		bool IClientStateManager.SupportsClientState
		{
			get { return SupportsClientState; }
		}

		void IClientStateManager.LoadClientState(string clientState)
		{
			LoadClientState(clientState);
		}

		string IClientStateManager.SaveClientState()
		{
			return SaveClientState();
		}

		#endregion

		//add new
		#region PropertySupportMethods
		/// <summary>
		/// Checks if all properties are valid
		/// </summary>
		/// <param name="throwException">true iff an exception is to be thrown for invalid parameters</param>
		/// <returns>true iff all parameters are valid</returns>
		protected virtual bool CheckIfValid(bool throwException)
		{
			bool valid = true;
			foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(this))
			{
				// If the property is tagged with RequiredPropertyAttribute, but doesn't have a value, throw an exception
				if ((null != prop.Attributes[typeof(RequiredPropertyAttribute)]) && ((null == prop.GetValue(this)) || !prop.ShouldSerializeValue(this)))
				{
					valid = false;
					if (throwException)
					{
						throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "{0} missing required {1} property value for {2}.", GetType().ToString(), prop.Name, ID), prop.Name);
					}
				}
			}
			return valid;
		}

		/// <summary>
		/// Called during rendering to give derived classes a chance to validate their properties
		/// </summary>
		/// <remarks>
		/// If the properties aren't valid, an exception of type ArgumentException should be thrown
		/// </remarks>
		public virtual void EnsureValid()
		{
			CheckIfValid(true);
		}

		/// <summary>
		/// 从ViewState中获取某属性值，如果为空则返回默认值nullValue
		/// </summary>
		/// <typeparam name="V">属性类型</typeparam>
		/// <param name="propertyName">属性名称</param>
		/// <param name="nullValue">默认值</param>
		/// <returns>属性值</returns>
		protected V GetPropertyValue<V>(string propertyName, V nullValue)
		{
            return ViewState.GetViewStateValue<V>(propertyName, nullValue);
		}

		/// <summary>
		/// 设置某属性的值
		/// </summary>
		/// <typeparam name="V">属性类型</typeparam>
		/// <param name="propertyName">属性名称</param>
		/// <param name="value">属性值</param>
		protected void SetPropertyValue<V>(string propertyName, V value)
		{
			ViewState.SetViewStateValue<V>(propertyName, value);
		}
		#endregion
	}
}

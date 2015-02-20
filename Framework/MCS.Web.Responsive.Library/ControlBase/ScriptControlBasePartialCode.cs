// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.
//此文件中各个类代码各个部分 基本完全一样，如果某个类中代码改变，要复制修改到其他类中代码，以保持一致
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using MCS.Library.Core;
using MCS.Web.Library.Script;
using MCS.Web.Responsive.Library;

//此文件中各个类代码各个部分 基本完全一样，如果某个类中代码改变，要复制修改到其他类中代码，以保持一致
namespace MCS.Web.Responsive.Library.Script
{

    /// <summary>
    /// ScriptControl is used to define complex custom controls which support ASP.NET AJAX script extensions
    /// </summary>
    [RequiredScript(typeof(ControlBaseScript))]
    public partial class ScriptControlBase : WebControl, IScriptControl, INamingContainer, IControlResolver, IPostBackDataHandler, IPostBackEventHandler, ICallbackEventHandler, IClientStateManager
    {
        #region [ Fields ]

        private ScriptManager _scriptManager;

        private bool _enableClientState;
        private bool _autoClearClientStateFieldValue = true;
        private string _cachedClientStateFieldID;

        private string _callbackArgument;

        private string _tagName;
        private HtmlTextWriterTag _tagKey;
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
        public virtual bool ReadOnly
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
        /// Invoke的时候是否需要ViewState
        /// </summary>
        [DefaultValue(true)]
        [ScriptControlProperty]
        [ClientPropertyName("invokeWithoutViewState")]
        public virtual bool InvokeWithoutViewState
        {
            get { return GetPropertyValue<bool>("InvokeWithoutViewState", true); }
            set { SetPropertyValue<bool>("InvokeWithoutViewState", value); }
        }

        /// <summary>
        /// 
        /// </summary>
        [ScriptControlProperty]
        [ClientPropertyName("serverControlType")]
        protected string ServerControlType
        {
            get
            {
                return this.GetType().AssemblyQualifiedName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [ScriptControlProperty]
        [ClientPropertyName("staticCallBackProxyID")]
        protected string StaticCallBackProxyID
        {
            get
            {
                return StaticCallBackProxy.STATIC_CALLBACKPROXY_CONTROL_ID;
            }
        }

        /// <summary>
        /// 对应的客户端控件类型
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
        /// 获取控件是否支持ClientState
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
        /// 获取当前页面的ScriptManager，如果没有则自动生成
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
        /// ClientState客户端ID
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

        /// <summary>
        /// 控件HtmlTextWriterTag
        /// Gets the tag key used when rendering the outer wrapper element for this user control
        /// </summary>
        protected override HtmlTextWriterTag TagKey
        {
            get { return _tagKey; }
        }

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
        }

        /// <summary>
        /// 通过ID查找与此控件最相关的控件
        /// Finds a control reference by its ID
        /// </summary>
        /// <param name="id">控件ID</param>
        /// <returns>查找出的控件</returns>
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
        /// 重载OnInit，增加事件
        /// </summary>
        /// <param name="e">事件参数</param>
        protected override void OnInit(EventArgs e)
        {
            ScriptControlHelper.CheckOnlyRenderSelf(this, this.RenderMode);

            if (Page != null)
                Page.InitComplete += new EventHandler(OnPageInitComplete);

            base.OnInit(e);

            if (Page != null)
            {
                Page.PreLoad += new EventHandler(OnPagePreLoad);
                Page.PreRenderComplete += new EventHandler(OnPagePreRenderComplete);
            }
        }

        private void OnPageInitComplete(object sender, EventArgs e)
        {
            StaticCallBackProxy.RegisterStaticCallbackProxyControl(this.Page);

            if (InvokeWithoutViewState)
            {
                Type baseType = ScriptControlHelper.GetBasePageTypeInfo(this.Page.GetType());

                string callbackControlID = HttpContext.Current.Request["__CALLBACKID"];

                if (string.IsNullOrEmpty(callbackControlID) == false)
                {
                    try
                    {
                        baseType.GetMethod("PrepareCallback",
                            BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this.Page, new object[] { callbackControlID });

                        baseType.GetMethod("RenderCallback",
                            BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this.Page, null);
                    }
                    finally
                    {
                        HttpContext.Current.Response.End();
                    }
                }
            }
        }

        /// <summary>
        /// 在页面Load之前，确保ScriptManager加载
        /// </summary>
        /// <param name="sender">事件触发对象</param>
        /// <param name="e">事件参数</param>
        protected virtual void OnPagePreLoad(object sender, EventArgs e)
        {
            EnsureScriptManager();
        }

        /// <summary>
        /// 在页面准备输出之前，确保Control输出
        /// </summary>
        /// <param name="sender">事件触发对象</param>
        /// <param name="e">事件参数</param>
        protected virtual void OnPagePreRenderComplete(object sender, EventArgs e)
        {
            if (RenderMode.OnlyRenderSelf && !this.Visible)
            {
                this.Visible = true;
                Control parent = this.Parent;

                while (parent != null && !parent.Visible)
                {
                    parent.Visible = true;
                    parent = parent.Parent;
                }
            }
        }

        /// <summary>
        /// 重载OnPreRender，输出ClientStateField，注册控件，注册Css文件
        /// Fires the PreRender event
        /// </summary>
        /// <param name="e">事件参数</param>
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

            ScriptManager.RegisterScriptControl<ScriptControlBase>(this);

            ScriptObjectBuilder.RegisterCssReferences(this);
        }

        /// <summary>
        /// 重载Render，输出控件，注册控件脚本描述
        /// </summary>
        /// <param name="writer">控件输出HtmlTextWriter</param>
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            if (!base.DesignMode)
            {
                this.ScriptManager.RegisterScriptDescriptors(this);
            }
        }

        /// <summary>
        /// 继承控件通过重载此方法，处理客户端传回的clientState
        /// Loads the client state data
        /// </summary>
        /// <param name="clientState">客户端传回的clientState</param>
        protected virtual void LoadClientState(string clientState)
        {
        }

        /// <summary>
        /// 继承控件通过重载此方法，返回传回客户端的clientState
        /// Saves the client state data
        /// </summary>
        /// <returns>传回客户端的clientState</returns>
        protected virtual string SaveClientState()
        {
            return null;
        }

        /// <summary>
        /// 为控件处理回发数据，在此处理了clientState，调用LoadClientState方法
        /// Executed when post data is loaded from the request
        /// </summary>
        /// <param name="postDataKey">控件的主要标识符</param>
        /// <param name="postCollection">所有传入名称值的集合。</param>
        /// <returns>如果服务器控件的状态在回发发生后更改，则为 true；否则为 false。</returns>
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
        /// 当由子类重载时，用信号要求服务器控件对象通知 ASP.NET 应用程序该控件的状态已更改
        /// Executed when post data changes should invoke a chagned event
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Maintaining consistency with IPostBackDataHandler")]
        protected virtual void RaisePostDataChangedEvent()
        {
        }

        /// <summary>
        /// 获取客户端脚本控件描述，包括脚本控件类型、属性、事件
        /// Gets the ScriptDescriptors that make up this control
        /// </summary>
        /// <returns>控件描述集合</returns>
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
        /// 获取客户端脚本控件所需脚本资源引用集合
        /// Gets the script references for this control
        /// </summary>
        /// <returns>脚本资源引用集合</returns>
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
                //return Visible && !ReadOnly && RenderMode.IsHtmlRender;
                return Visible && RenderMode.IsHtmlRender;
            }
        }

        /// <summary>
        /// 产生客户端控件描述，包括客户端脚本控件属性、事件
        /// Describes the settings for this control.
        /// </summary>
        /// <param name="descriptor">控件描述</param>
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
        /// 获取回调函数产生的结果
        /// Handles a callback event
        /// </summary>
        /// <returns>回调函数产生的结果</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Method has a side-effect")]
        protected virtual string GetCallbackResult()
        {
            string argument = _callbackArgument;
            _callbackArgument = null;

            Dictionary<string, object> callInfo = JSONSerializerExecute.DeserializeObject(argument, typeof(Dictionary<string, object>)) as Dictionary<string, object>;

            return ScriptObjectBuilder.ExecuteCallbackMethod(this, callInfo);
        }

        /// <summary>
        /// 将回调参数eventArgument放在全局变量中，以便在GetCallbackResult函数中处理
        /// Raises a callback event
        /// </summary>
        /// <param name="eventArgument">回调参数</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Maintaining consistency with ICallbackEventHandler")]
        protected virtual void RaiseCallbackEvent(string eventArgument)
        {
            _callbackArgument = eventArgument;
        }

        /// <summary>
        /// 处理将窗体发送到服务器时引发的事件
        /// </summary>
        /// <param name="eventArgument">表示要传递到事件处理程序的可选事件参数的 String</param>
        /// <remarks>处理将窗体发送到服务器时引发的事件</remarks>
        protected virtual void RaisePostBackEvent(string eventArgument)
        {
        }

        /// <summary>
        /// Restores view-state information from a previous request that was saved with the SaveViewState method.
        /// 并在LoadViewState之前缓存ViewState中所有IStateManager类型项
        /// 以及在LoadViewState之后从缓存中恢复将ViewState中ViewSateItemInternal类型项恢复成IStateManager类型项
        /// </summary>
        /// <param name="savedState">表示要还原的控件状态的 Object</param>
        /// <remarks>		
        /// Restores view-state information from a previous request that was saved with the SaveViewState method.
        /// 并在LoadViewState之前缓存ViewState中所有IStateManager类型项
        /// 以及在LoadViewState之后从缓存中恢复将ViewState中ViewSateItemInternal类型项恢复成IStateManager类型项
        ///</remarks>
        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
            StateBag backState = ViewState.PreLoadViewState();
            ViewState.AfterLoadViewState(backState);
        }

        /// <summary>
        /// Saves any state that was modified after the TrackViewState method was invoked
        /// 在SaveViewState之前，将ViewState中所有IStateManager类型项转换为可序列化的ViewSateItemInternal类型项
        /// 以及在SaveViewState之后，将ViewState中所有ViewSateItemInternal类型项恢复成IStateManager类型项
        /// </summary>
        /// <returns>返回服务器控件的当前视图状态</returns>
        /// <remarks>
        /// Saves any state that was modified after the TrackViewState method was invoked
        /// 并在SaveViewState之前，将ViewState中所有IStateManager类型项转换为可序列化的ViewSateItemInternal类型项
        /// 以及在SaveViewState之后，将ViewState中所有ViewSateItemInternal类型项恢复成IStateManager类型项
        /// </remarks>
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
        /// <remarks>
        /// Causes the control to track changes to its view state so they can be stored in the object's ViewState property
        /// 并将ViewState中所有IStateManager类型项标记为TrackViewState
        /// </remarks>
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
            if (this.RenderMode.OnlyRenderSelf && !this.RenderMode.UseNewPage)
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
        /// <remarks>通过controlId，查找相应Control</remarks>
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
        /// <remarks>从ViewState中获取某属性值，如果为空则返回默认值nullValue</remarks>
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
        /// <remarks>设置某属性的值</remarks>
        protected void SetPropertyValue<V>(string propertyName, V value)
        {
            ViewState.SetViewStateValue<V>(propertyName, value);
        }
        #endregion
    }
}

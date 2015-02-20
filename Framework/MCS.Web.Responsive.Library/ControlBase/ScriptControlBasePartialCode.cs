// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.
//���ļ��и��������������� ������ȫһ�������ĳ�����д���ı䣬Ҫ�����޸ĵ��������д��룬�Ա���һ��
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

//���ļ��и��������������� ������ȫһ�������ĳ�����д���ı䣬Ҫ�����޸ĵ��������д��룬�Ա���һ��
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
        /// �ؼ����ģʽ
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
        /// �Ƿ�ֻ��
        /// </summary>
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("�Ƿ�ֻ��")]
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
        /// Invoke��ʱ���Ƿ���ҪViewState
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
        /// ��Ӧ�Ŀͻ��˿ؼ�����
        /// The script type to use for the ScriptControl
        /// </summary>        
        protected virtual string ClientControlType
        {
            get
            {
                ClientScriptResourceAttribute attr = (ClientScriptResourceAttribute)TypeDescriptor.GetAttributes(this)[typeof(ClientScriptResourceAttribute)];
                ExceptionHelper.TrueThrow(attr == null, string.Format("{0}�ؼ���û��ClientScriptResourceAttribute������", this.GetType().Name));
                return attr.ComponentType;
            }
        }

        /// <summary>
        /// ��ȡ�ؼ��Ƿ�֧��ClientState
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
        /// ��ȡ��ǰҳ���ScriptManager�����û�����Զ�����
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
        /// �Ƿ��Զ�����ͻ���ClientStateField�е�ֵ
        /// </summary>
        protected virtual bool AutoClearClientStateFieldValue
        {
            get
            {
                return this._autoClearClientStateFieldValue;
            }
        }

        /// <summary>
        /// ClientState�ͻ���ID
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
        /// �ؼ�HtmlTextWriterTag
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
        /// ͨ��ID������˿ؼ�����صĿؼ�
        /// Finds a control reference by its ID
        /// </summary>
        /// <param name="id">�ؼ�ID</param>
        /// <returns>���ҳ��Ŀؼ�</returns>
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
        /// ����OnInit�������¼�
        /// </summary>
        /// <param name="e">�¼�����</param>
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
        /// ��ҳ��Load֮ǰ��ȷ��ScriptManager����
        /// </summary>
        /// <param name="sender">�¼���������</param>
        /// <param name="e">�¼�����</param>
        protected virtual void OnPagePreLoad(object sender, EventArgs e)
        {
            EnsureScriptManager();
        }

        /// <summary>
        /// ��ҳ��׼�����֮ǰ��ȷ��Control���
        /// </summary>
        /// <param name="sender">�¼���������</param>
        /// <param name="e">�¼�����</param>
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
        /// ����OnPreRender�����ClientStateField��ע��ؼ���ע��Css�ļ�
        /// Fires the PreRender event
        /// </summary>
        /// <param name="e">�¼�����</param>
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
        /// ����Render������ؼ���ע��ؼ��ű�����
        /// </summary>
        /// <param name="writer">�ؼ����HtmlTextWriter</param>
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            if (!base.DesignMode)
            {
                this.ScriptManager.RegisterScriptDescriptors(this);
            }
        }

        /// <summary>
        /// �̳пؼ�ͨ�����ش˷���������ͻ��˴��ص�clientState
        /// Loads the client state data
        /// </summary>
        /// <param name="clientState">�ͻ��˴��ص�clientState</param>
        protected virtual void LoadClientState(string clientState)
        {
        }

        /// <summary>
        /// �̳пؼ�ͨ�����ش˷��������ش��ؿͻ��˵�clientState
        /// Saves the client state data
        /// </summary>
        /// <returns>���ؿͻ��˵�clientState</returns>
        protected virtual string SaveClientState()
        {
            return null;
        }

        /// <summary>
        /// Ϊ�ؼ�����ط����ݣ��ڴ˴�����clientState������LoadClientState����
        /// Executed when post data is loaded from the request
        /// </summary>
        /// <param name="postDataKey">�ؼ�����Ҫ��ʶ��</param>
        /// <param name="postCollection">���д�������ֵ�ļ��ϡ�</param>
        /// <returns>����������ؼ���״̬�ڻط���������ģ���Ϊ true������Ϊ false��</returns>
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
        /// ������������ʱ�����ź�Ҫ��������ؼ�����֪ͨ ASP.NET Ӧ�ó���ÿؼ���״̬�Ѹ���
        /// Executed when post data changes should invoke a chagned event
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Maintaining consistency with IPostBackDataHandler")]
        protected virtual void RaisePostDataChangedEvent()
        {
        }

        /// <summary>
        /// ��ȡ�ͻ��˽ű��ؼ������������ű��ؼ����͡����ԡ��¼�
        /// Gets the ScriptDescriptors that make up this control
        /// </summary>
        /// <returns>�ؼ���������</returns>
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
        /// ��ȡ�ͻ��˽ű��ؼ�����ű���Դ���ü���
        /// Gets the script references for this control
        /// </summary>
        /// <returns>�ű���Դ���ü���</returns>
        protected virtual IEnumerable<ScriptReference> GetScriptReferences()
        {
            if (!IsRenderScript) return null;

            return ScriptControlHelper.GetScriptReferences(this.GetType(), this.ScriptPath);
        }

        /// <summary>
        /// �Ƿ�������ؼ��ű�
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
        /// �����ͻ��˿ؼ������������ͻ��˽ű��ؼ����ԡ��¼�
        /// Describes the settings for this control.
        /// </summary>
        /// <param name="descriptor">�ؼ�����</param>
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
        /// ��ȡ�ص����������Ľ��
        /// Handles a callback event
        /// </summary>
        /// <returns>�ص����������Ľ��</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Method has a side-effect")]
        protected virtual string GetCallbackResult()
        {
            string argument = _callbackArgument;
            _callbackArgument = null;

            Dictionary<string, object> callInfo = JSONSerializerExecute.DeserializeObject(argument, typeof(Dictionary<string, object>)) as Dictionary<string, object>;

            return ScriptObjectBuilder.ExecuteCallbackMethod(this, callInfo);
        }

        /// <summary>
        /// ���ص�����eventArgument����ȫ�ֱ����У��Ա���GetCallbackResult�����д���
        /// Raises a callback event
        /// </summary>
        /// <param name="eventArgument">�ص�����</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Maintaining consistency with ICallbackEventHandler")]
        protected virtual void RaiseCallbackEvent(string eventArgument)
        {
            _callbackArgument = eventArgument;
        }

        /// <summary>
        /// �������巢�͵�������ʱ�������¼�
        /// </summary>
        /// <param name="eventArgument">��ʾҪ���ݵ��¼��������Ŀ�ѡ�¼������� String</param>
        /// <remarks>�������巢�͵�������ʱ�������¼�</remarks>
        protected virtual void RaisePostBackEvent(string eventArgument)
        {
        }

        /// <summary>
        /// Restores view-state information from a previous request that was saved with the SaveViewState method.
        /// ����LoadViewState֮ǰ����ViewState������IStateManager������
        /// �Լ���LoadViewState֮��ӻ����лָ���ViewState��ViewSateItemInternal������ָ���IStateManager������
        /// </summary>
        /// <param name="savedState">��ʾҪ��ԭ�Ŀؼ�״̬�� Object</param>
        /// <remarks>		
        /// Restores view-state information from a previous request that was saved with the SaveViewState method.
        /// ����LoadViewState֮ǰ����ViewState������IStateManager������
        /// �Լ���LoadViewState֮��ӻ����лָ���ViewState��ViewSateItemInternal������ָ���IStateManager������
        ///</remarks>
        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
            StateBag backState = ViewState.PreLoadViewState();
            ViewState.AfterLoadViewState(backState);
        }

        /// <summary>
        /// Saves any state that was modified after the TrackViewState method was invoked
        /// ��SaveViewState֮ǰ����ViewState������IStateManager������ת��Ϊ�����л���ViewSateItemInternal������
        /// �Լ���SaveViewState֮�󣬽�ViewState������ViewSateItemInternal������ָ���IStateManager������
        /// </summary>
        /// <returns>���ط������ؼ��ĵ�ǰ��ͼ״̬</returns>
        /// <remarks>
        /// Saves any state that was modified after the TrackViewState method was invoked
        /// ����SaveViewState֮ǰ����ViewState������IStateManager������ת��Ϊ�����л���ViewSateItemInternal������
        /// �Լ���SaveViewState֮�󣬽�ViewState������ViewSateItemInternal������ָ���IStateManager������
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
        /// ����ViewState������IStateManager��������ΪTrackViewState
        /// </summary>
        /// <remarks>
        /// Causes the control to track changes to its view state so they can be stored in the object's ViewState property
        /// ����ViewState������IStateManager��������ΪTrackViewState
        /// </remarks>
        protected override void TrackViewState()
        {
            base.TrackViewState();
            ViewState.TrackViewState();
        }

        /// <summary>
        /// �ж��Ƿ�ֻ��������ؼ�������������ֻ������ؼ�����������������Ŀؼ����
        /// </summary>
        /// <param name="writer">���տؼ����ݵ� HtmlTextWriter ����</param>
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
        /// ͨ��controlId��������ӦControl
        /// </summary>
        /// <param name="controlId">controlId</param>
        /// <returns>Control</returns>
        /// <remarks>ͨ��controlId��������ӦControl</remarks>
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
        /// ��ViewState�л�ȡĳ����ֵ�����Ϊ���򷵻�Ĭ��ֵnullValue
        /// </summary>
        /// <typeparam name="V">��������</typeparam>
        /// <param name="propertyName">��������</param>
        /// <param name="nullValue">Ĭ��ֵ</param>
        /// <returns>����ֵ</returns>
        /// <remarks>��ViewState�л�ȡĳ����ֵ�����Ϊ���򷵻�Ĭ��ֵnullValue</remarks>
        protected V GetPropertyValue<V>(string propertyName, V nullValue)
        {
            return ViewState.GetViewStateValue<V>(propertyName, nullValue);
        }

        /// <summary>
        /// ����ĳ���Ե�ֵ
        /// </summary>
        /// <typeparam name="V">��������</typeparam>
        /// <param name="propertyName">��������</param>
        /// <param name="value">����ֵ</param>
        /// <remarks>����ĳ���Ե�ֵ</remarks>
        protected void SetPropertyValue<V>(string propertyName, V value)
        {
            ViewState.SetViewStateValue<V>(propertyName, value);
        }
        #endregion
    }
}

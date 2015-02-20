using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using MCS.Web.Library.Script;
using System.Web.UI.WebControls;
using System.Reflection;
using MCS.Library.Caching;
using System.Text.RegularExpressions;
using MCS.Library.Core;

namespace MCS.Web.Responsive.WebControls
{
    /// <summary>
    /// 属性编辑器具体属性编辑的基类
    /// </summary>
    public abstract class PropertyEditorBase
    {
        public string EditorKey
        {
            get
            {
                PropertyEditorDescriptionAttribute attr = GetPropertyEditorDescription();

                string result = this.GetType().Name;

                if (attr != null)
                    result = attr.EditorKey;

                return result;
            }
        }

        public string ComponentType
        {
            get
            {
                PropertyEditorDescriptionAttribute attr = GetPropertyEditorDescription();

                string result = this.GetType().FullName;

                if (attr != null)
                    result = attr.ComponentType;

                return result;
            }
        }

        public virtual bool IsCloneControlEditor
        {
            get
            {
                return false;
            }
        }

        public virtual Control CreateDefalutControl()
        {
            return null;
        }

        //private Dictionary<string, IEnumerable<ControlPropertyDefine>> controlsPropertyDefine = null;

        public ControlPropertyDefineKeyedCollection ControlsPropertyDefine
        {
            get
            {
                lock (PropertyEditorHelper.AllEditorsControlPropertyDefine)
                {
                    if (!PropertyEditorHelper.AllEditorsControlPropertyDefine.ContainsKey(this.EditorKey))
                    {
                        PropertyEditorHelper.AllEditorsControlPropertyDefine[this.EditorKey] = new ControlPropertyDefineKeyedCollection();
                    }

                    return PropertyEditorHelper.AllEditorsControlPropertyDefine[this.EditorKey];
                }
            }
        }

        public virtual void SetControlsPropertyDefineFromEditorParams(PropertyDefine propertyDefine)
        {
            var editorParams = "";

            if (string.IsNullOrEmpty(propertyDefine.EditorParamsSettingsKey) == false)
            {
                var curSettings = PropertyEditorParamsSettings.GetConfig().EditorParams[propertyDefine.EditorParamsSettingsKey];
                if (curSettings == null)
                    throw new System.Configuration.ConfigurationErrorsException(string.Format("找不到Name为{0}的EditorParamsSettings", propertyDefine.EditorParamsSettingsKey));

                editorParams = curSettings.EditorParamsValue.ValueText;


            }
            else if (string.IsNullOrEmpty(propertyDefine.EditorParams) && Regex.IsMatch(propertyDefine.EditorParams, @"\{[^{}]*}") == true)
            {
                editorParams = propertyDefine.EditorParams;
            }

            if (string.IsNullOrEmpty(editorParams) == false)
            {
                EditorParamsDefine paraDefine = JSONSerializerExecute.Deserialize<EditorParamsDefine>(editorParams);

                if (paraDefine.ServerControlProperties.Count > 0)
                {
                    if (string.IsNullOrEmpty(paraDefine.CloneControlID))
                        paraDefine.CloneControlID = this.DefaultCloneControlName();

                    if (this.IsCloneControlEditor)
                    {
                        if (this.ControlsPropertyDefine.ContainsKey(paraDefine.CloneControlID))
                        {
                            this.ControlsPropertyDefine[paraDefine.CloneControlID].ControlPropertyDefineList = paraDefine.ServerControlProperties;
                            this.ControlsPropertyDefine[paraDefine.CloneControlID].ExtendedSettings = paraDefine.ExtendedSettings;
                            this.ControlsPropertyDefine[paraDefine.CloneControlID].UseTemplate = paraDefine.UseTemplate;
                        }
                        else
                        {
                            this.ControlsPropertyDefine.Add(new ControlPropertyDefineWrapper() { ControlID = paraDefine.CloneControlID, ControlPropertyDefineList = paraDefine.ServerControlProperties, ExtendedSettings = paraDefine.ExtendedSettings, UseTemplate = paraDefine.UseTemplate });
                        }
                    }
                }

                if (string.IsNullOrEmpty(paraDefine.ExtendedSettings) == false)
                {
                    paraDefine.ExtendedSettings = null;
                    propertyDefine.EditorParams = JSONSerializerExecute.Serialize(paraDefine);
                }
            }
        }

        //private Dictionary<string, Control> listControls = null;

        //public Dictionary<string, Control> Controls
        //{
        //    get
        //    {
        //        if (this.listControls == null)
        //            this.listControls = new Dictionary<string, Control>();

        //        return this.listControls;
        //    }
        //}

        //public virtual void AddCloneControl(Control cloneControl)
        //{
        //    this.Controls.Add(cloneControl.ID, cloneControl);
        //}

        //public virtual bool ContainsControlID(string controlID)
        //{
        //    return this.ControlsPropertyDefine.ContainsKey(controlID);
        //}

        public virtual string DefaultCloneControlName()
        {
            return string.Empty;
        }

        public Control CreateOneControl(string controlID, Page page)
        {
            Control result = null;
            var key = "EditorControl_" + controlID;

            if (!page.Items.Contains(key))
            {
                result = this.CreateDefalutControl();
                result.EnableViewState = false;
                result.ID = controlID;
                page.Items.Add(key, true);
            }

            return result;
        }

        protected HtmlGenericControl GetControlsContainerInPage(Page page)
        {
            //var id = "propertyeditor-controls-container";
            HtmlGenericControl div = null;// page.Form.FindControl(id) as HtmlGenericControl;

            if (div == null)
            {
                div = new HtmlGenericControl("div") { };
                div.Style["display"] = "none";
                page.Form.Controls.AddAt(0, div);
            }

            return div;
        }

        protected virtual void CreateControls(Page page)
        {
            if (page.Form != null)
            {
                if (this.ControlsPropertyDefine.Count > 0)
                {
                    foreach (var item in this.ControlsPropertyDefine)
                    {
                        var div = GetControlsContainerInPage(page);

                        if (item.UseTemplate == false)
                        {
                            var itemControl = this.CreateOneControl(item.ControlID, page);

                            if (itemControl != null)
                            {
                                this.InitCloneControlProperties(itemControl, item);
                                div.Controls.Add(itemControl);
                            }
                        }
                    }
                }
                else
                {
                    var itemControl = this.CreateOneControl(this.DefaultCloneControlName(), page);

                    if (itemControl != null)
                    {
                        var div = GetControlsContainerInPage(page);

                        if (this.ControlsPropertyDefine.ContainsKey(itemControl.ID))
                        {
                            this.ControlsPropertyDefine[itemControl.ID].ControlPropertyDefineList = null;
                        }
                        else
                        {
                            this.ControlsPropertyDefine.Add(new ControlPropertyDefineWrapper() { ControlID = itemControl.ID, ControlPropertyDefineList = null });
                        }

                        div.Controls.Add(itemControl);
                    }
                }
            }
        }

        public virtual void InitCloneControlProperties(Control currentControl, ControlPropertyDefineWrapper propertyDefineWrapper)
        {
            if (propertyDefineWrapper != null && !propertyDefineWrapper.UseTemplate)
            {
                if (propertyDefineWrapper.ControlPropertyDefineList != null)
                {
                    foreach (ControlPropertyDefine define in propertyDefineWrapper.ControlPropertyDefineList)
                    {
                        PropertyInfo piDest = TypePropertiesCacheQueue.Instance.GetPropertyInfo(currentControl.GetType(), define.PropertyName);
                        if (piDest != null)
                        {
                            if (string.IsNullOrEmpty(define.StringValue) == true || piDest.CanWrite == false)
                                continue;

                            if (piDest.PropertyType == typeof(Unit))
                                piDest.SetValue(currentControl, Unit.Parse(define.StringValue), null);
                            else
                                piDest.SetValue(currentControl, define.GetRealValue(), null);
                        }
                    }
                }
            }
        }

        protected internal virtual void OnPageInit(Page page)
        {

        }

        protected internal virtual void RegisterScript(Page page)
        {
        }

        protected internal virtual void OnPageLoad(Page page)
        {
        }

        protected internal virtual void OnPagePreRender(Page page)
        {

        }

        private PropertyEditorDescriptionAttribute GetPropertyEditorDescription()
        {
            return AttributeHelper.GetCustomAttribute<PropertyEditorDescriptionAttribute>(this.GetType());
        }
    }
}

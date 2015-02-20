using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.IO;
using MCS.Library.Core;
using MCS.Library.Configuration;
using MCS.Library.SOA.DataObjects;
using MCS.Web.Library;
using MCS.Library.Caching;
using System.Reflection;
using System.Web.UI.WebControls;

namespace MCS.Web.WebControls
{
    public static class PropertyEditorHelper
    {
        private static Dictionary<string, PropertyEditorBase> _GlobalPropertyEditors = new Dictionary<string, PropertyEditorBase>();
        private static Dictionary<string, ControlPropertyDefineKeyedCollection> _AllEditorsControlPropertyDefine = new Dictionary<string, ControlPropertyDefineKeyedCollection>();

        public static void RegisterEditor(PropertyEditorBase editor)
        {
            editor.NullCheck("editor");

            lock (_GlobalPropertyEditors)
            {
                _GlobalPropertyEditors[editor.EditorKey] = editor;
            }
        }

        public static void AttachToPage(Page page)
        {
            page.NullCheck("page");

            if (page.Items.Contains("PropertyEditorHelper") == false)
            {
                page.Init += new EventHandler(page_Init);
                page.Load += new EventHandler(page_Load);
                //page.LoadComplete += new EventHandler(page_LoadComplete);
                page.PreRender += new EventHandler(page_PreRender);
                page.Items.Add("PropertyEditorHelper", true);
            }
        }

        internal static Dictionary<string, DropdownPropertyDataSourceConfigurationElement> AllDropdownPropertyDataSource
        {
            get
            {
                return (Dictionary<string, DropdownPropertyDataSourceConfigurationElement>)ObjectContextCache.Instance.GetOrAddNewValue("DropdownPropertyDataSource", (cache, key) =>
                {
                    Dictionary<string, DropdownPropertyDataSourceConfigurationElement> newItem = GetAllDropdownPropertyDataSource();

                    cache.Add(key, newItem);

                    return newItem;
                });
            }
        }

        internal static Dictionary<string, PropertyEditorBase> AllEditors
        {
            get
            {
                return GetAllEditors();
            }
        }

        internal static Dictionary<string, ControlPropertyDefineKeyedCollection> AllEditorsControlPropertyDefine
        {
            get
            {
                return _AllEditorsControlPropertyDefine;
            }
        }

        static void page_Init(object sender, EventArgs e)
        {
            AllEditors.ForEach(kp => { kp.Value.OnPageInit((Page)sender); });
        }

        private static void page_Load(object sender, EventArgs e)
        {
            AllEditors.ForEach(kp => { kp.Value.RegisterScript((Page)sender); });

            AllEditors.ForEach(kp => { kp.Value.OnPageLoad((Page)sender); });
        }

        private static void page_PreRender(object sender, EventArgs e)
        {
            WebUtility.RequiredScript(typeof(PropertyEditorControlBase));

            Page page = (Page)sender;

            page.ClientScript.RegisterStartupScript(typeof(PropertyEditorHelper),
                "RegisterPropertyEditorScript",
                GetRegisterEditorScript(AllEditors),
                true);

            AllEditors.ForEach(kp => { kp.Value.OnPagePreRender((Page)sender); });
        }

        private static Dictionary<string, DropdownPropertyDataSourceConfigurationElement> GetAllDropdownPropertyDataSource()
        {
            Dictionary<string, DropdownPropertyDataSourceConfigurationElement> result = new Dictionary<string, DropdownPropertyDataSourceConfigurationElement>();
            DropdownPropertyDataSourceConfigurationCollection propertiesSource = DropdownPropertyDataSourceSettings.GetConfig().PropertySources;

            foreach (DropdownPropertyDataSourceConfigurationElement item in propertiesSource)
            {
                result.Add(item.Name, item);
            }

            return result;
        }

        private static Dictionary<string, PropertyEditorBase> GetAllEditors()
        {
            Dictionary<string, PropertyEditorBase> result = new Dictionary<string, PropertyEditorBase>();

            lock (_GlobalPropertyEditors)
            {
                _GlobalPropertyEditors.ForEach(kp => result.Add(kp.Key, kp.Value));
            }

            TypeConfigurationCollection editorTypes = PropertyEditorSettings.GetConfig().Editors;

            foreach (TypeConfigurationElement typeElem in editorTypes)
            {
                PropertyEditorBase editor = (PropertyEditorBase)typeElem.CreateInstance();

                if (result.ContainsKey(editor.EditorKey) == false)
                    result.Add(editor.EditorKey, editor);
            }

            return result;
        }

        private static string GetRegisterEditorScript(Dictionary<string, PropertyEditorBase> editors)
        {
            StringBuilder strB = new StringBuilder(256);

            using (StringWriter writer = new StringWriter(strB))
            {
                writer.WriteLine("$HGRootNS.PropertyEditors = {};");

                foreach (KeyValuePair<string, PropertyEditorBase> kp in editors)
                {
                    writer.WriteLine("$HGRootNS.PropertyEditors[\"{0}\"] = {1};",
                        kp.Key,
                        "{" + string.Format("editorKey: \"{0}\", componentType: \"{1}\"",
                        kp.Key, kp.Value.ComponentType) + "}");
                }
            }

            return strB.ToString();
        }

        internal static EnumTypePropertyDescription GenerateEnumTypePropertyDescription(DropdownPropertyDataSourceConfigurationElement config)
        {
            EnumTypePropertyDescription result = new EnumTypePropertyDescription();
            result.EnumTypeName = config.Name;

            DropDownList dr = new DropDownList();
            dr.DataTextField = config.BindingText;
            dr.DataValueField = config.BindingValue;
            dr.DataSource = GenerateDropDownListSourceByConfiguration(config);
            dr.DataBind();

            foreach (ListItem item in dr.Items)
                result.Items.Add(new EnumItemPropertyDescription(item));

            return result;
        }

        private static object GenerateDropDownListSourceByConfiguration(DropdownPropertyDataSourceConfigurationElement config)
        {
            Type type = config.GetTypeInfo();

            MethodInfo mi = type.GetMethod(config.Method);

            (mi != null).FalseThrow("不能在类型{0}中找到方法{1}", type.FullName, config.Method);

            return mi.Invoke(config.CreateInstance(), new object[] { });
        }
    }
}

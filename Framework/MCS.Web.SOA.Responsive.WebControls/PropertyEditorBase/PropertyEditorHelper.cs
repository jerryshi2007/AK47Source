using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.UI;
using MCS.Web.Responsive.WebControls.Configuration;
using MCS.Web.Responsive.Library;
using MCS.Library.Configuration;
using System.IO;
using System.Web.UI.WebControls;
using MCS.Library.Caching;
using System.Web;

namespace MCS.Web.Responsive.WebControls
{
    public static class PropertyEditorHelper
    {
        private static Dictionary<string, PropertyEditorBase> _GlobalPropertyEditors = new Dictionary<string, PropertyEditorBase>();
        private static Dictionary<string, ControlPropertyDefineKeyedCollection> _AllEditorsControlPropertyDefine = new Dictionary<string, ControlPropertyDefineKeyedCollection>();
        private static readonly object PageAllEditorCacheKey = new object();

        public static void RegisterEditor(PropertyEditorBase editor)
        {
            if (editor == null)
                throw new ArgumentNullException("editor");

            lock (_GlobalPropertyEditors)
            {
                _GlobalPropertyEditors[editor.EditorKey] = editor;
            }

            var page = HttpContextExtension.GetCurrentPage(HttpContext.Current);
            if (page != null)
                page.Items.Remove(PageAllEditorCacheKey);
        }

        public static void AttachToPage(Page page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

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

        internal static IEnumerable<PropertyEditorBase> AllEditorsForIterator(Page page)
        {
            Dictionary<string, PropertyEditorBase> editors;

            if (page.Items.Contains(PageAllEditorCacheKey) == false)
            {
                page.Items[PageAllEditorCacheKey] = editors = GetAllEditors();
            }
            else
            {
                editors = (Dictionary<string, PropertyEditorBase>)page.Items[PageAllEditorCacheKey];
            }

            return editors.Values;
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
            Page p = (Page)sender;
            foreach (var item in AllEditorsForIterator(p))
            {
                item.OnPageInit(p);
            }
        }

        private static void page_Load(object sender, EventArgs e)
        {
            Page p = (Page)sender;
            foreach (var item in AllEditorsForIterator(p))
            {
                item.RegisterScript(p);
            }

            foreach (var item in AllEditorsForIterator(p))
            {
                item.OnPageLoad(p);
            }
        }

        private static void page_PreRender(object sender, EventArgs e)
        {
            WebUtility.RequiredScript(typeof(PropertyEditorControlBase));

            Page page = (Page)sender;

            page.ClientScript.RegisterStartupScript(typeof(PropertyEditorHelper),
                "RegisterPropertyEditorScript",
                GetRegisterEditorScript(GetAllEditors()),
                true);

            foreach (var item in AllEditorsForIterator(page))
            {
                item.OnPagePreRender(page);
            }
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

        public static Dictionary<string, PropertyEditorBase> GetAllEditors()
        {
            Dictionary<string, PropertyEditorBase> result = new Dictionary<string, PropertyEditorBase>();

            lock (_GlobalPropertyEditors)
            {
                foreach (var item in _GlobalPropertyEditors)
                {
                    result[item.Key] = item.Value;
                }
            }

            TypeConfigurationCollection editorTypes = PropertyEditorConfigurationSection.GetConfig().Editors;

            foreach (TypeConfigurationElement typeElem in editorTypes)
            {
                PropertyEditorBase editor = (PropertyEditorBase)typeElem.CreateInstance();

                if (result.ContainsKey(editor.EditorKey) == false)
                    result[editor.EditorKey] = editor;
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

            if (mi == null)
                throw new System.Reflection.TargetException(string.Format("不能在类型{0}中找到方法{1}", type.FullName, config.Method));

            return mi.Invoke(config.CreateInstance(), new object[] { });
        }
    }
}

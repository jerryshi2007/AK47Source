using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

namespace MCS.Web.Responsive.Library
{
    public static class PageExtension
    {
        internal const string PageRenderModeQueryStringName = "ResponsivePageRenderMode";
        internal static object PageRenderControlItemKey = new object();

        /// <summary>
        /// 为页面挂接PageModule
        /// </summary>
        /// <param name="page"></param>
        public static void AttachPageModules(this Page page)
        {
            if (page != null)
            {
                foreach (IPageModule module in ConfigSectionFactory.GetPageModulesSection().Create().Values)
                {
                    module.Init(page);
                }
            }
        }

        /// <summary>
        /// 向页面中增加与scriptType类型相关的脚本
        /// </summary>
        /// <param name="page">页面</param>
        /// <param name="scriptType">脚本相关类型</param>
        public static void RequiredScript(this Page page, Type scriptType)
        {
            if (page != null)
            {
                IEnumerable<ScriptReference> srs = Script.ScriptObjectBuilder.GetScriptReferences(scriptType);

                ScriptManager sm = ScriptManager.GetCurrent(page);
                foreach (ScriptReference sr in srs)
                {
                    if (sm != null)
                        sm.Scripts.Add(sr);
                    else
                    {
                        DeluxeClientScriptManager.RegisterHeaderScript(page, page.ClientScript.GetWebResourceUrl(scriptType, sr.Name));
                    }
                }

                Script.ScriptObjectBuilder.RegisterCssReferences(page, scriptType);
            }
        }
    }
}

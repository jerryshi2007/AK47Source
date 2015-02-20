using System;
using System.Text;
using System.Web.UI;
using System.Reflection;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Caching;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Web.WebControls;

namespace MCS.Web.Library.MVC
{
    /// <summary>
    /// 重绘控件策略基类
    /// 如需注册特殊控件的重绘方案，请继承此类，并重写RenderVisible，RenderEnabled，RenderReadOnly方法。
    /// 如不重写RenderVisible，RenderEnabled，RenderReadOnly方法，
    /// 那么以Control的方式重绘该控件
    /// </summary>
    public class SceneRenderBase
    {
        /// <summary>
        /// 要重绘的控件
        /// </summary>
        private System.Type controlType = null;

        /// <summary>
        /// 不允许该构造方法，造成类型转换错误。
        /// </summary>
        private SceneRenderBase()
        {
        }

        /// <summary>
        /// 但参数构造
        /// </summary>
        /// <param name="type">要重绘的控件类型</param>
        public SceneRenderBase(System.Type type)
        {
            this.controlType = type;
        }

        /// <summary>
        /// 设置可视属性
        /// </summary>
        /// <param name="target">目标控件</param>
        /// <param name="setValue">设置的值</param>
        public virtual void RenderVisible(object target, object setValue)
        {
            SetPropertyValue(target, "Visible", setValue);
        }

        /// <summary>
        /// 设置是否可用属性
        /// </summary>
        /// <param name="target">目标控件</param>
        /// <param name="setValue">设置的值</param>
        public virtual void RenderEnabled(object target, object setValue)
        {
            SetPropertyValue(target, "Enabled", setValue);
        }

        /// <summary>
        /// 设置是否只读属性
        /// </summary>
        /// <param name="target">目标控件</param>
        /// <param name="setValue">设置的值</param>
        public virtual void RenderReadOnly(object target, object setValue)
        {
            SetPropertyValue(target, "ReadOnly", setValue);
        }

        /// <summary>
        /// 渲染Html元素的属性
        /// </summary>
        /// <param name="target"></param>
        /// <param name="attributes"></param>
        public virtual void RenderHtmlAttributes(object target, NameValueCollection attributes)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(target != null, "target");
            ExceptionHelper.FalseThrow<ArgumentNullException>(attributes != null, "attributes");

            if (target is IAttributeAccessor)
            {
                foreach (string key in attributes)
                {
                    ((IAttributeAccessor)target).SetAttribute(key, attributes[key]);
                }
            }
        }

        /// <summary>
        /// 渲染Style属性
        /// </summary>
        /// <param name="target"></param>
        /// <param name="attributes"></param>
        public virtual void RenderHtmlStyles(object target, NameValueCollection attributes)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(target != null, "target");
            ExceptionHelper.FalseThrow<ArgumentNullException>(attributes != null, "attributes");

            if ((target is HtmlControl) || (target is WebControl))
            {
                foreach (string key in attributes)
                {
                    if (target is HtmlControl)
                        ((HtmlControl)target).Style[key] = attributes[key];
                    else
                        if (target is WebControl)
                            ((WebControl)target).Style[key] = attributes[key];
                }
            }
        }

        public virtual void RenderSubItems(object target, SceneSubItemCollection subItems)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(target != null, "target");
            ExceptionHelper.FalseThrow<ArgumentNullException>(subItems != null, "subItems");

            if ((target is HtmlControl) || (target is WebControl))
            {
                foreach (var subItem in subItems)
                {
                    if (subItem.Type == SceneSubItemType.Column)
                    {
                        if (target is ClientGrid)
                        {
                            var targetCtrl = target as ClientGrid;
                            var columns = targetCtrl.Columns.FindAll(p => p.Name == subItem.Name);
                            foreach (var column in columns)
                            {
                                column.Visible = subItem.Visible;
                                column.EditorReadOnly = subItem.ReadOnly;
                                column.EditorEnabled = subItem.Enabled;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 设置控件属性
        /// </summary>
        /// <param name="target">目标控件</param>
        /// <param name="propName">目标属性</param>
        /// <param name="setValue">目标值</param>
        private void SetPropertyValue(object target, string propName, object setValue)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(target != null, "target");
            ExceptionHelper.CheckStringIsNullOrEmpty(propName, "propName");

            try
            {
                PropertyInfo pi = TypePropertiesCacheQueue.Instance.GetPropertyInfoDirectly(this.controlType, propName);

                if (pi != null)
                    if (pi.CanWrite)
                        pi.SetValue(target, setValue, null);
            }
            catch (TargetInvocationException)
            {
            }
        }
    }
}

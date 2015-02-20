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
    /// �ػ�ؼ����Ի���
    /// ����ע������ؼ����ػ淽������̳д��࣬����дRenderVisible��RenderEnabled��RenderReadOnly������
    /// �粻��дRenderVisible��RenderEnabled��RenderReadOnly������
    /// ��ô��Control�ķ�ʽ�ػ�ÿؼ�
    /// </summary>
    public class SceneRenderBase
    {
        /// <summary>
        /// Ҫ�ػ�Ŀؼ�
        /// </summary>
        private System.Type controlType = null;

        /// <summary>
        /// ������ù��췽�����������ת������
        /// </summary>
        private SceneRenderBase()
        {
        }

        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="type">Ҫ�ػ�Ŀؼ�����</param>
        public SceneRenderBase(System.Type type)
        {
            this.controlType = type;
        }

        /// <summary>
        /// ���ÿ�������
        /// </summary>
        /// <param name="target">Ŀ��ؼ�</param>
        /// <param name="setValue">���õ�ֵ</param>
        public virtual void RenderVisible(object target, object setValue)
        {
            SetPropertyValue(target, "Visible", setValue);
        }

        /// <summary>
        /// �����Ƿ��������
        /// </summary>
        /// <param name="target">Ŀ��ؼ�</param>
        /// <param name="setValue">���õ�ֵ</param>
        public virtual void RenderEnabled(object target, object setValue)
        {
            SetPropertyValue(target, "Enabled", setValue);
        }

        /// <summary>
        /// �����Ƿ�ֻ������
        /// </summary>
        /// <param name="target">Ŀ��ؼ�</param>
        /// <param name="setValue">���õ�ֵ</param>
        public virtual void RenderReadOnly(object target, object setValue)
        {
            SetPropertyValue(target, "ReadOnly", setValue);
        }

        /// <summary>
        /// ��ȾHtmlԪ�ص�����
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
        /// ��ȾStyle����
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
        /// ���ÿؼ�����
        /// </summary>
        /// <param name="target">Ŀ��ؼ�</param>
        /// <param name="propName">Ŀ������</param>
        /// <param name="setValue">Ŀ��ֵ</param>
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

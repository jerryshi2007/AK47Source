// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using System.Security;

namespace MCS.Web.Responsive.Library.Script
{
    /// <summary>
    /// ScriptControl is used to define complex custom controls which support ASP.NET AJAX script extensions
    /// 脚本控件基类，直接从WebControl继承，实现IScriptControl接口
    /// </summary>
    [RequiredScript(typeof(ControlBaseScript))]
    public partial class ScriptControlBase : WebControl, IScriptControl, INamingContainer, IControlResolver, IPostBackDataHandler, IPostBackEventHandler, ICallbackEventHandler, IClientStateManager
    {
        #region [ Constructor ]

        /// <summary>
		/// 构造函数
        /// Initializes a new ScriptControl
        /// </summary>
        /// <param name="tag"></param>
        public ScriptControlBase(HtmlTextWriterTag tag)
            : this(false, tag)
        {
			
        }

        /// <summary>
		/// 构造函数
        /// Initializes a new ScriptControl
        /// </summary>
        protected ScriptControlBase()
            : this(true)
        {
        }

        /// <summary>
		/// 构造函数
        /// Initializes a new ScriptControl
        /// </summary>
        /// <param name="tag">控件的tagName</param>
        protected ScriptControlBase(string tag)
            : this(false, tag)
        {
        }

        /// <summary>
		/// 构造函数
        /// Initializes a new ScriptControl
        /// </summary>
        /// <param name="enableClientState">是否使用ClientState</param>
        protected ScriptControlBase(bool enableClientState)
        {
            _enableClientState = enableClientState;
        }
		 
        /// <summary>
		/// 构造函数
        /// Initializes a new ScriptControl
        /// </summary>
		/// <param name="enableClientState">是否使用ClientState</param>
		/// <param name="tag">控件的HtmlTextWriterTag</param>
        protected ScriptControlBase(bool enableClientState, HtmlTextWriterTag tag)
        {
            _tagKey = tag;
            _enableClientState = enableClientState;		
        }

        /// <summary>
		/// 构造函数
        /// Initializes a new ScriptControl
        /// </summary>
		/// <param name="enableClientState">是否使用ClientState</param>
		/// <param name="tag">控件的tagName</param>
        protected ScriptControlBase(bool enableClientState, string tag)
        {
            _tagKey = HtmlTextWriterTag.Unknown;
            _tagName = tag;
            _enableClientState = enableClientState;         
        }
        #endregion
     }

    /// <summary>
    /// ListItem事件参数
    /// </summary>
    public class ListItemDataBoundEventArgs : EventArgs
    {
        #region Content
        private ListItem _Item;
        private object _ItemData;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="item">ListItem对象</param>
        /// <param name="itemData">绑定数据</param>
        public ListItemDataBoundEventArgs(ListItem item, object itemData)
        {
            this._Item = item;
            this._ItemData = itemData;
        }

        /// <summary>
        /// ListItem对象
        /// </summary>
        public ListItem Item
        {
            get { return this._Item; }
        }

        /// <summary>
        /// 绑定数据
        /// </summary>
        public object ItemData
        {
            get { return this._ItemData; }
        }
        #endregion
    }

    /// <summary>
    /// ListItem数据绑定Handler
    /// </summary>
    /// <param name="sender">控件对象</param>
    /// <param name="args">事件参数</param>
    public delegate void ListItemDataBoundEventHanlder(object sender, ListItemDataBoundEventArgs args);

    /// <summary>
    /// 脚本ListContro控件基类
    /// </summary>
    [RequiredScript(typeof(ControlBaseScript))]
    public partial class ScriptListControlBase : ListControl, IScriptControl, INamingContainer, IControlResolver, IPostBackDataHandler, ICallbackEventHandler, IClientStateManager
    {
        private readonly static object _EventItemDataBound = new object();

        #region [ Constructor ]
        /// <summary>
        /// 
        /// </summary>
        public ScriptListControlBase()
            : this(true)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enableClientState"></param>
        public ScriptListControlBase(bool enableClientState)           
        {
            this._enableClientState = enableClientState;
        }
        #endregion

        #region DataBind
        /// <summary>
        /// 条目数据绑定事件
        /// </summary>
        public event ListItemDataBoundEventHanlder ItemDataBound
        {
            add
            {
                base.Events.AddHandler(_EventItemDataBound, value);
            }
            remove
            {
                base.Events.RemoveHandler(_EventItemDataBound, value);
            }
        }

        /// <summary>
        /// 提供文本显示的数据源属性集合
        /// </summary>
        [Category("Appearance")]
        [Description("选择项文本显示属性集合")]
        public virtual string[] DataTextFieldList
        {
            get
            {
                return this.GetPropertyValue<string[]>("DataTextFieldList", new string[0] { });
            }
            set
            {
                this.SetPropertyValue<string[]>("DataTextFieldList", value);
            }
        }

        /// <summary>
        /// ListItem数据绑定方法，继承控件可重载此方法，处理本控件的特殊绑定
        /// </summary>
        /// <param name="item"></param>
        /// <param name="data"></param>
        protected virtual void OnItemDataBound(ListItem item, object data)
        {
            ListItemDataBoundEventHanlder hanlder = this.Events[_EventItemDataBound] as ListItemDataBoundEventHanlder;
            if (hanlder != null)
                hanlder(this, new ListItemDataBoundEventArgs(item, data));
        }

        /// <summary>
        /// 进行绑定操作
        /// </summary>
        /// <param name="dataSource">数据源</param>
        protected override void PerformDataBinding (IEnumerable dataSource)
        {
            if (dataSource != null)
            {
                bool flag = false;
                bool formatFlag = false;
                string[] propNames = this.DataTextFieldList;
                string propName = this.DataTextField;
                string dataValueField = this.DataValueField;
                string format = this.DataTextFormatString;
                if (!this.AppendDataBoundItems)
                {
                    this.Items.Clear();
                }
                ICollection collection = dataSource as ICollection;
                if (collection != null)
                {
                    this.Items.Capacity = collection.Count + this.Items.Count;
                }
                if ((propNames.Length != 0) || (propName.Length != 0) || (dataValueField.Length != 0))
                {
                    flag = true;
                }
                if (format.Length != 0)
                {
                    formatFlag = true;
                }
                foreach (object itemData in dataSource)
                {
                    ListItem item = new ListItem();
                    if (flag)
                    {
                        if (propNames.Length > 0)
                        {
                            string[] objs = new string[propNames.Length];
                            for (int i = 0; i < propNames.Length; i++)
                            {
                                objs[i] = DataBinder.GetPropertyValue(itemData, propNames[i], null);
                            }
                            if (formatFlag)
                                item.Text = string.Format(format, objs);
                            else
                                item.Text = string.Join(" ", objs);
                        }
                        else
                        if (propName.Length > 0)
                        {
                            item.Text = DataBinder.GetPropertyValue(itemData, propName, format);
                        }
                        if (dataValueField.Length > 0)
                        {
                            item.Value = DataBinder.GetPropertyValue(itemData, dataValueField, null);
                        }
                    }
                    else
                    {
                        if (formatFlag)
                        {
                            item.Text = string.Format(CultureInfo.CurrentCulture, format, new object[] { itemData });
                        }
                        else
                        {
                            item.Text = itemData.ToString();
                        }
                        item.Value = itemData.ToString();
                    }
                    this.Items.Add(item);

                    OnItemDataBound(item, itemData);
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 脚本GridView控件基类
    /// </summary>
    [RequiredScript(typeof(ControlBaseScript))]
    public partial class ScriptGridViewBase : GridView, IScriptControl, INamingContainer, IControlResolver, IPostBackDataHandler, ICallbackEventHandler, IClientStateManager
    {
        #region [ Constructor ]
        /// <summary>
        /// 
        /// </summary>
        public ScriptGridViewBase()
            : this(true)
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enableClientState"></param>
        public ScriptGridViewBase(bool enableClientState)           
        {
            this._enableClientState = enableClientState;
        }
        #endregion
    }
}


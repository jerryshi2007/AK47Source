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
    /// �ű��ؼ����ֱ࣬�Ӵ�WebControl�̳У�ʵ��IScriptControl�ӿ�
    /// </summary>
    [RequiredScript(typeof(ControlBaseScript))]
    public partial class ScriptControlBase : WebControl, IScriptControl, INamingContainer, IControlResolver, IPostBackDataHandler, IPostBackEventHandler, ICallbackEventHandler, IClientStateManager
    {
        #region [ Constructor ]

        /// <summary>
		/// ���캯��
        /// Initializes a new ScriptControl
        /// </summary>
        /// <param name="tag"></param>
        public ScriptControlBase(HtmlTextWriterTag tag)
            : this(false, tag)
        {
			
        }

        /// <summary>
		/// ���캯��
        /// Initializes a new ScriptControl
        /// </summary>
        protected ScriptControlBase()
            : this(true)
        {
        }

        /// <summary>
		/// ���캯��
        /// Initializes a new ScriptControl
        /// </summary>
        /// <param name="tag">�ؼ���tagName</param>
        protected ScriptControlBase(string tag)
            : this(false, tag)
        {
        }

        /// <summary>
		/// ���캯��
        /// Initializes a new ScriptControl
        /// </summary>
        /// <param name="enableClientState">�Ƿ�ʹ��ClientState</param>
        protected ScriptControlBase(bool enableClientState)
        {
            _enableClientState = enableClientState;
        }
		 
        /// <summary>
		/// ���캯��
        /// Initializes a new ScriptControl
        /// </summary>
		/// <param name="enableClientState">�Ƿ�ʹ��ClientState</param>
		/// <param name="tag">�ؼ���HtmlTextWriterTag</param>
        protected ScriptControlBase(bool enableClientState, HtmlTextWriterTag tag)
        {
            _tagKey = tag;
            _enableClientState = enableClientState;		
        }

        /// <summary>
		/// ���캯��
        /// Initializes a new ScriptControl
        /// </summary>
		/// <param name="enableClientState">�Ƿ�ʹ��ClientState</param>
		/// <param name="tag">�ؼ���tagName</param>
        protected ScriptControlBase(bool enableClientState, string tag)
        {
            _tagKey = HtmlTextWriterTag.Unknown;
            _tagName = tag;
            _enableClientState = enableClientState;         
        }
        #endregion
     }

    /// <summary>
    /// ListItem�¼�����
    /// </summary>
    public class ListItemDataBoundEventArgs : EventArgs
    {
        #region Content
        private ListItem _Item;
        private object _ItemData;

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="item">ListItem����</param>
        /// <param name="itemData">������</param>
        public ListItemDataBoundEventArgs(ListItem item, object itemData)
        {
            this._Item = item;
            this._ItemData = itemData;
        }

        /// <summary>
        /// ListItem����
        /// </summary>
        public ListItem Item
        {
            get { return this._Item; }
        }

        /// <summary>
        /// ������
        /// </summary>
        public object ItemData
        {
            get { return this._ItemData; }
        }
        #endregion
    }

    /// <summary>
    /// ListItem���ݰ�Handler
    /// </summary>
    /// <param name="sender">�ؼ�����</param>
    /// <param name="args">�¼�����</param>
    public delegate void ListItemDataBoundEventHanlder(object sender, ListItemDataBoundEventArgs args);

    /// <summary>
    /// �ű�ListContro�ؼ�����
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
        /// ��Ŀ���ݰ��¼�
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
        /// �ṩ�ı���ʾ������Դ���Լ���
        /// </summary>
        [Category("Appearance")]
        [Description("ѡ�����ı���ʾ���Լ���")]
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
        /// ListItem���ݰ󶨷������̳пؼ������ش˷����������ؼ��������
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
        /// ���а󶨲���
        /// </summary>
        /// <param name="dataSource">����Դ</param>
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
    /// �ű�GridView�ؼ�����
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


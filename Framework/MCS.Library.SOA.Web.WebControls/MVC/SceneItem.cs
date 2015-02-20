using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MCS.Library.Core;
using System.Collections.Specialized;
using System.Collections;

namespace MCS.Web.Library.MVC
{
    /// <summary>
    /// ����������������
    /// </summary>
    public enum SceneSubItemType
    {
        /// <summary>
        /// ��
        /// </summary>
        Column
    }

	/// <summary>
	/// ����������
	/// </summary>
	[Serializable]
	public class SceneItem
	{
		private string controlID = string.Empty;
		private bool isVisible = true;
		private bool isReadOnly = false;
		private bool isEnabled = true;
		private bool recursive = false;

		private NameValueCollection attributes = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
		private NameValueCollection styles = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
        private SceneSubItemCollection subItems = new SceneSubItemCollection();

        internal SceneItem(XmlNode node)
        {
            this.controlID = XmlHelper.GetAttributeText(node, "controlID");
            this.isVisible = XmlHelper.GetAttributeValue(node, "visible", this.isVisible);
            this.isReadOnly = XmlHelper.GetAttributeValue(node, "readOnly", this.isReadOnly);
            this.isEnabled = XmlHelper.GetAttributeValue(node, "enabled", this.isEnabled);
            this.recursive = XmlHelper.GetAttributeValue(node, "recursive", this.recursive);

            InitDictionaryByNodes(this.attributes, node.SelectNodes("Attributes/Item"));
            InitDictionaryByNodes(this.styles, node.SelectNodes("Styles/Item"));
            InitSubItems(node.SelectNodes("SubItems/SubItem"));
        }

        #region Properties

        /// <summary>
		/// �ؼ�ID
		/// </summary>
		public string ControlID
		{
			get { return this.controlID; }
			set { this.controlID = value; }
		}

		/// <summary>
		///�Ƿ���� 
		/// </summary>
		public bool Visible
		{
			get { return this.isVisible; }
			set { this.isVisible = value; }
		}

		/// <summary>
		/// �Ƿ�ֻ��
		/// </summary>
		public bool ReadOnly
		{
			get { return this.isReadOnly; }
			set { this.isReadOnly = value; }
		}

		/// <summary>
		/// �Ƿ����
		/// </summary>
		public bool Enabled
		{
			get { return this.isEnabled; }
			set { this.isEnabled = value; }
		}

		/// <summary>
		/// �Ƿ�Ĭ�ϵݹ���Ԫ��
		/// </summary>
		public bool Recursive
		{
			get { return this.recursive; }
			set { this.recursive = value; }
		}

		/// <summary>
		/// Html������Ԫ��
		/// </summary>
		public NameValueCollection Attributes
		{
			get
			{
				return this.attributes;
			}
		}

		/// <summary>
		/// ��ʽ
		/// </summary>
		public NameValueCollection Styles
		{
			get
			{ 
				return this.styles; 
			}
		}

	    public SceneSubItemCollection SubItems
	    {
	        get
	        {
                return this.subItems;
	        }
	    }

        #endregion

        #region Methods

        private static void InitDictionaryByNodes(NameValueCollection dict, IEnumerable nodes)
		{
			foreach (XmlNode node in nodes)
			{
				dict[XmlHelper.GetAttributeText(node, "key")] = XmlHelper.GetAttributeText(node, "value");
			}
		}

        private void InitSubItems(IEnumerable nodes)
        {
            foreach (XmlNode node in nodes)
            {
                this.subItems.Add(new SceneSubItem(node));
            }
        }

		#endregion
        
    }


	/// <summary>
	/// �����������
	/// </summary>
	[Serializable]
	public class SceneItemCollection : KeyedCollection<string, SceneItem>
	{
		internal SceneItemCollection()
		{
		}

		protected override string GetKeyForItem(SceneItem item)
		{
			return item.ControlID;
		}
	}

    /// <summary>
    /// ������������
    /// </summary>
    [Serializable]
    public class SceneSubItem
    {
        internal SceneSubItem(XmlNode node)
		{
			this.Name = XmlHelper.GetAttributeText(node, "name");
            this.Type = XmlHelper.GetAttributeValue(node, "type", SceneSubItemType.Column);
            this.Visible = XmlHelper.GetAttributeValue(node, "visible", true);
            this.ReadOnly = XmlHelper.GetAttributeValue(node, "readOnly", false);
            this.Enabled = XmlHelper.GetAttributeValue(node, "enabled", true);
		}

        /// <summary>
        /// ����
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// ����
        /// </summary>
        public SceneSubItemType Type
        {
            get;
            set;
        }

        /// <summary>
        ///�Ƿ���� 
        /// </summary>
        public bool Visible
        {
            get;
            set;
        }

        /// <summary>
        /// �Ƿ�ֻ��
        /// </summary>
        public bool ReadOnly
        {
            get;
            set;
        }

        /// <summary>
        /// �Ƿ�����
        /// </summary>
        public bool Enabled
        {
            get;
            set;
        }
    }

    /// <summary>
    /// �������������
    /// </summary>
    [Serializable]
    public class SceneSubItemCollection : KeyedCollection<string, SceneSubItem>
    {
        internal SceneSubItemCollection()
        {
        }

        protected override string GetKeyForItem(SceneSubItem item)
        {
            return item.Name;
        }
    }
}

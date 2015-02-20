using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MCS.Web.WebControls
{
    /// <summary>
    /// 字段类型
    /// </summary>
    public enum DataType
    {
        Object = 1,
        Boolean = 3,
        Integer = 9,
        Decimal = 15,
        DateTime = 16,
        String = 18,
        Enum = 20
    }

    [Serializable]
    [ParseChildren(true, "EditTemplate")]
    public class ClientGridColumn
    {
        private string name = string.Empty;
        private bool selectColumn = false;
        private bool showSelectAll = false;
        private string dataField = string.Empty;
        private DataType dataType = DataType.String;
        private string headerText = string.Empty;
        private string headerTips = string.Empty;
        private string sortExpression = string.Empty;
        private string headerStyle = string.Empty;
        private string headerTipsStyle = string.Empty;
        private string itemStyle = string.Empty;
        private string footerStyle = string.Empty;
        private int maxLength = 0;
        private string formatString = string.Empty;
        private string editorStyle = String.Empty;
        private string editorTooltips = String.Empty;
        private bool editorReadOnly = false;
        private bool editorEnabled = true;
        private bool visible = true;
        private bool isDynamicColumn = false;
        private bool autoBindingValidation = true;
        private string tag = string.Empty;
        private bool isFixedLine = false;
		private bool isStatistic = false;
        private ClientGridColumnEditTemplate _EditTemplate = null;

        /// <summary>
        /// 构造方法
        /// </summary>
        public ClientGridColumn()
        {
        }

        [DefaultValue("")]
        public string Name
        {
            get { return this.name.IsNullOrEmpty() ? DataField : this.name; }
            set { this.name = value; }
        }

        [DefaultValue(false)]
        public bool SelectColumn
        {
            get { return this.selectColumn; }
            set { this.selectColumn = value; }
        }

        [DefaultValue(false)]
        public bool ShowSelectAll
        {
            get { return this.showSelectAll; }
            set { this.showSelectAll = value; }
        }

        [DefaultValue("")]
        public string DataField
        {
            get { return this.dataField; }
            set { this.dataField = value; }
        }

        [DefaultValue(DataType.String)]
        public DataType DataType
        {
            get { return this.dataType; }
            set { this.dataType = value; }
        }

        [DefaultValue(0)]
        public int MaxLength
        {
            get { return this.maxLength; }
            set { this.maxLength = value; }
        }

        [DefaultValue("")]
        public string FormatString
        {
            get { return this.formatString; }
            set { this.formatString = value; }
        }

        [DefaultValue("")]
        public string EditorStyle
        {
            get { return this.editorStyle; }
            set { this.editorStyle = value; }
        }

        [DefaultValue("")]
        public string HeaderText
        {
            get { return this.headerText; }
            set { this.headerText = value; }
        }

        [DefaultValue("")]
        public string HeaderTips
        {
            get { return this.headerTips; }
            set { this.headerTips = value; }
        }

        [DefaultValue("{color:'Red'}")]
        public string HeaderTipsStyle
        {
            get { return this.headerTipsStyle; }
            set { this.headerTipsStyle = value; }
        }

        [DefaultValue("")]
        public string SortExpression
        {
            get { return this.sortExpression; }
            set { this.sortExpression = value; }
        }

        [DefaultValue("")]
        public string HeaderStyle
        {
            get { return this.headerStyle; }
            set { this.headerStyle = value; }
        }

        [DefaultValue("")]
        public string ItemStyle
        {
            get { return this.itemStyle; }
            set { this.itemStyle = value; }
        }

        [DefaultValue("")]
        public string FooterStyle
        {
            get { return this.footerStyle; }
            set { this.footerStyle = value; }
        }

        [DefaultValue("")]
        public string EditorTooltips
        {
            get { return this.editorTooltips; }
            set { this.editorTooltips = value; }
        }

        [DefaultValue(false)]
        public bool EditorReadOnly
        {
            get { return this.editorReadOnly; }
            set { this.editorReadOnly = value; }
        }

        [DefaultValue(true)]
        public bool EditorEnabled
        {
            get { return this.editorEnabled; }
            set { this.editorEnabled = value; }
        }

        [DefaultValue(true)]
        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        [DefaultValue("")]
        public string Tag
        {
            get { return this.tag; }
            set { this.tag = value; }
        }

        [DefaultValue(false)]
        public bool IsDynamicColumn
        {
            get { return isDynamicColumn; }
            set { isDynamicColumn = value; }
        }

        [DefaultValue(true)]
        public bool AutoBindingValidation
        {
            get { return autoBindingValidation; }
            set { autoBindingValidation = value; }
        }

        [DefaultValue(false)]
        public bool IsFixedLine
        {
            get { return isFixedLine; }
            set { isFixedLine = value; }
        }

		[DefaultValue(false)]
		public bool IsStatistic
		{
			get { return isStatistic; }
			set { isStatistic = value; }
		}

        [Browsable(false)]
        [MergableProperty(false)]
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public ClientGridColumnEditTemplate EditTemplate
        {
            get
            {
                if (this._EditTemplate == null)
                    this._EditTemplate = new ClientGridColumnEditTemplate();

                return this._EditTemplate;
            }
            set
            {
                this._EditTemplate = value;
            }
        }

    }

    [Serializable]
    public class ClientGridColumnCollection : DataObjectCollectionBase<ClientGridColumn>
    {
        public void Add(ClientGridColumn item)
        {
            ExceptionHelper.FalseThrow<ArgumentNullException>(item != null, "item");

            InnerAdd(item);
        }

        public ClientGridColumn this[int index]
        {
            get
            {
                return (ClientGridColumn)List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        public ClientGridColumn this[string dataField]
        {
            get
            {
                return this.Find(tmp => tmp.DataField == dataField);
            }
            set
            {
                int index = this.List.IndexOf(this.Find(tmp => tmp.DataField == dataField));
                if (index >= 0)
                    this[index] = value;
            }
        }
    }
}

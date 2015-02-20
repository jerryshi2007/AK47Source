#region
// -------------------------------------------------
// Assembly	��	MCS.Web.WebControls
// FileName	��	GenericInputExtender.cs
// Remark	��  ͨ��¼��ؼ�Extender��ʽ
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		����	    20070815		����
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Web.UI.WebControls.WebParts;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.GenericInput.GenericInputExtender.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.GenericInput.GenericInput.css", "text/css", PerformSubstitution = true)]
[assembly: WebResource("MCS.Web.WebControls.GenericInput.drop-arrow.gif", "image/gif")]
namespace MCS.Web.WebControls
{
	/// <summary>
	/// ͨ��¼�����չ��ʽ�ؼ�
	/// </summary>
	/// <remarks>
	/// ͨ��¼�����չ��ʽ�ؼ�
	/// ��Ҫ����ı���,������ؼ���TargetIDָ������ı��򼴿�
	/// </remarks>
    [PersistChildren(false)]
    [ParseChildren(true, "Items")]
    [RequiredScript(typeof(ControlBaseScript))]
    [Designer(typeof(GenericInputExtenderDesigner))]
    [TargetControlType(typeof(Control))]
    [ClientCssResource("MCS.Web.WebControls.GenericInput.GenericInput.css")]
    [ClientScriptResource("MCS.Web.WebControls.GenericInput",
     "MCS.Web.WebControls.GenericInput.GenericInputExtender.js")]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public class GenericInputExtender : ExtenderControlBase
    {
        private ListItemCollection items = new ListItemCollection();
        private string text = "";

        #region Properties
       
        /// <summary>
        /// �ؼ��߿����ɫ
        /// </summary>
        /// <remarks>
        /// �ؼ��߿����ɫ
        /// </remarks>
        [DefaultValue(typeof(Color), "35,83,178")]//"#2353B2"
        [Category("Appearance")]
        [ScriptControlProperty]//���ô�����Ҫ������ͻ���
        [ClientPropertyName("highlightBorderColor")]
        public Color HighlightBorderColor
        {
            get { return GetPropertyValue("HighlightBorderColor", Color.FromArgb(35, 83, 178)); }
            set { SetPropertyValue("HighlightBorderColor", value); }
        }

		/// <summary>
		/// �����߿򶥲����
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("highlightBorderTopWidth")]
		public int HighlightBorderTopWidth
		{
			get { return GetPropertyValue("HighlightBorderTopWidth", 1); }
			set { SetPropertyValue("HighlightBorderTopWidth", value); }
		}

		/// <summary>
		/// �����߿������
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("highlightBorderLeftWidth")]
		public int HighlightBorderLeftWidth
		{
			get { return GetPropertyValue("HighlightBorderLeftWidth", 1); }
			set { SetPropertyValue("HighlightBorderLeftWidth", value); }
		}

		/// <summary>
		/// �����߿��Ҳ���
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("highlightBorderRightWidth")]
		public int HighlightBorderRightWidth
		{
			get { return GetPropertyValue("HighlightBorderRightWidth", 1); }
			set { SetPropertyValue("HighlightBorderRightWidth", value); }
		}

		/// <summary>
		/// �����߿�ײ����
		/// </summary>
		[Category("Appearance")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("highlightBorderBottomWidth")]
		public int HighlightBorderBottomWidth
		{
			get { return GetPropertyValue("HighlightBorderBottomWidth", 1); }
			set { SetPropertyValue("HighlightBorderBottomWidth", value); }
		}

        /// <summary>
        /// ѡ����ĿĬ��������ɫ
        /// </summary>
        /// <remarks>
        /// ѡ����ĿĬ��������ɫ
        /// </remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]//���ô�����Ҫ������ͻ���
        [ClientPropertyName("itemCssClass")]
        public string ItemCssClass
        {
            get { return GetPropertyValue("ItemCssClass", ""); }
            set { SetPropertyValue("ItemCssClass", value); }
        }

        /// <summary>
        /// ����ƶ���ѡ����Ŀ��ʱ��������ɫ
        /// </summary>
        /// <remarks>
        /// ����ƶ���ѡ����Ŀ��ʱ��������ɫ
        /// </remarks>
        [DefaultValue("")]
        [Category("Appearance")]
        [ScriptControlProperty]//���ô�����Ҫ������ͻ���
        [ClientPropertyName("itemHoverCssClass")]
        public string ItemHoverCssClass
        {
            get { return GetPropertyValue("ItemHoverCssClass", ""); }
            set { SetPropertyValue("ItemHoverCssClass", value); }
        }

        /// <summary>
        /// ������ͷ����ı���ɫ
        /// </summary>
        /// <remarks>
        /// ������ͷ����ı���ɫ
        /// </remarks>
        [DefaultValue(typeof(Color), "198, 225, 255")]//"#C6E1FF"
        [Category("Appearance")]
        [ScriptControlProperty]//���ô�����Ҫ������ͻ���
        [ClientPropertyName("dropArrowBackgroundColor")]
        public Color DropArrowBackgroundColor
        {
            get { return GetPropertyValue("DropArrowBackgroundColor", Color.FromArgb(198, 225, 255)); }
            set { SetPropertyValue("DropArrowBackgroundColor", value); }
        }

        /// <summary>
        /// �ؼ��ĵ�ǰ�ı�
        /// </summary>
        /// <remarks>
        /// �ؼ��ĵ�ǰ�ı�
        /// </remarks>
        [ScriptControlProperty]//���ô�����Ҫ������ͻ���
        [ClientPropertyName("text")]
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }

        /// <summary>
        /// �ı��ı���¼�
        /// </summary>
        [DefaultValue("")]
        public string OnChange
        {
            get { return GetPropertyValue("OnChange", ""); }
            set { SetPropertyValue("OnChange", value); }
        }

        /// <summary>
        /// ѡ����Ŀ���¼�
        /// </summary>
        [DefaultValue("")]
        public string OnSelectedItem
        {
            get { return GetPropertyValue("OnSelectedItem", ""); }
            set { SetPropertyValue("OnSelectedItem", value); }
        }

        /// <summary>
        /// ѡ����Ŀǰ���¼�
        /// </summary>
        [DefaultValue("")]
        public string OnSelectItem
        {
            get { return GetPropertyValue("OnSelectItem", ""); }
            set { SetPropertyValue("OnSelectItem", value); }
        }

        /// <summary>
        /// ��ǰѡ���Indexֵ
        /// </summary>
        /// <remarks>
        /// ��ǰѡ���Indexֵ
        /// </remarks>
        [DefaultValue(-1)]
        [Browsable(false)]
        [ScriptControlProperty]//���ô�����Ҫ������ͻ���
        [ClientPropertyName("selectIndex")]
        public int SelectedIndex
        {
            get { return GetPropertyValue("SelectedIndex", -1); }
            set { SetPropertyValue("SelectedIndex", value); }
        }

        /// <summary>
        /// �ؼ��е�ѡ����Ŀ����
        /// </summary>
        /// <remarks>
        /// �ؼ��е�ѡ����Ŀ����
        /// </remarks>
        [ScriptControlProperty]//���ô�����Ҫ������ͻ���
        [ClientPropertyName("items")]//���ô����Զ�Ӧ�ͻ������Ե�����
        [Editor("System.Web.UI.Design.WebControls.ListItemsCollectionEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ListItemCollection Items
        {
            get
            {
                return this.items;
            }
        }
        #endregion

        //---------------------------------�Ҿ������޳ܵķָ���---------------------------------//

        #region ClientState
        /// <summary>
        /// ����ClientState
        /// </summary>
        /// <remarks>
        /// ����ClientState
        ///     ClientState�б������һ������Ϊ3��һά����
        ///         ��һ��Ϊѡ����Ŀ������ֵ�����û��ѡ����Ϊ-1
        ///         �ڶ���Ϊѡ����Ŀ���ı�������������ı�
        ///         ��������Items
        /// </remarks>
        /// <param name="clientState">���л����clientState</param>
        protected override void LoadClientState(string clientState)
        {
            base.LoadClientState(clientState);

            object[] foArray = JSONSerializerExecute.Deserialize<object[]>(clientState);

            if (null != foArray && foArray.Length > 0)
            {
                this.SelectedIndex = Convert.ToInt32(foArray[0]);
                if (foArray.Length > 1 && null != foArray[1])
                    this.Text = foArray[1].ToString();
                else
                    this.Text = "";

                
                if (foArray.Length > 2 && null != foArray[2])
                    this.items = (ListItemCollection)JSONSerializerExecute.DeserializeObject(foArray[2], typeof(ListItemCollection));
                else
                    this.items = new ListItemCollection();
            }
            else
            {
                this.SelectedIndex = -1;
                this.Text = "";
                this.items = new ListItemCollection();
            }
        }

        /// <summary>
        /// ����ClientState
        /// </summary>
        /// <remarks>
        /// ����ClientState
        ///     ClientState�б������һ������Ϊ2��һά����
        ///         ��һ��Ϊѡ����Ŀ������ֵ�����û��ѡ����Ϊ-1
        ///         �ڶ���Ϊѡ����Ŀ���ı�������������ı�
        /// </remarks>
        /// <returns>���л����CLientState�ַ���</returns>
        protected override string SaveClientState()
        {
            object[] foArray = new object[] { this.SelectedIndex, this.Text ,this.items};

            string fsSerialize = JSONSerializerExecute.Serialize(foArray);

            return fsSerialize;
        }
        #endregion
    }
}

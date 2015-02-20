using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using MCS.Library.Core;
using MCS.Library.Globalization;
using MCS.Web.Library;
using MCS.Web.Library.Script;

[assembly: WebResource("MCS.Web.WebControls.DeluxeGrid.DeluxeGrid.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.WebControls.DeluxeGrid.DeluxeGrid.css", "text/css")]
[assembly: WebResource("MCS.Web.WebControls.DeluxeGrid.excel.gif", "image/gif")]
[assembly: WebResource("MCS.Web.WebControls.DeluxeGrid.word.gif", "image/gif")]
namespace MCS.Web.WebControls
{
	#region ö����

	/// <summary>
	/// ҳ����ʾģʽ
	/// </summary>
	/// <remarks>
	/// ҳ����ʾģʽ
	/// </remarks>
	public enum PagerCodeShowMode
	{
		/// <summary>
		/// ��ҳ��
		/// </summary>
		/// <remarks>
		/// 
		/// </remarks>
		RecordCount,

		/// <summary>
		/// ��ǰҳ/��ҳ��
		/// </summary>
		/// <remarks>
		/// 
		/// </remarks>
		CurrentRecordCount,

		/// <summary>
		/// ȫ����ʾ
		/// </summary>
		/// <remarks>
		/// 
		/// </remarks>
		All
	}


	/// <summary>
	/// ��Grid��ĳ�е�λ��
	/// </summary>
	/// <remarks>
	/// ��Grid��ĳ�е�λ��
	/// </remarks>
	public enum RowPosition
	{
		/// <summary>
		/// ��
		/// </summary>
		/// <remarks>
		/// 
		/// </remarks>
		Left,
		/// <summary>
		/// ��
		/// </summary>
		/// <remarks>
		/// 
		/// </remarks>
		Right
	}
	#endregion

	/// <summary>
	/// DeluxeGrid �ؼ� �̳���GridView
	/// </summary>
	/// <remarks>
	/// DeluxeGrid �ؼ� �̳���GridView
	/// </remarks>
	//���������Css
	[ClientCssResource("MCS.Web.WebControls.DeluxeGrid.DeluxeGrid.css")]
	[DefaultProperty("DeluxeGrid"),
	ToolboxData("<{0}:DeluxeGrid runat=server Width=\"280\"></{0}:DeluxeGrid>")]
	[ParseChildren(true),
	PersistChildren(false)]
	[RequiredScript(typeof(ControlBaseScript))]
	//[Designer(typeof(PagerTemplateDesigner))]
	//���ñ��ؼ��ű�����һ��Ϊ�ͻ��˿ؼ�������
	[ClientScriptResource("MCS.Web.WebControls.DeluxeGrid",
		"MCS.Web.WebControls.DeluxeGrid.DeluxeGrid.js")]
	public class DeluxeGrid : ScriptGridViewBase, ICascadePagedControl
	{
		/// <summary>
		/// �������̬
		/// </summary>
		internal List<IAttributeAccessor> regions;

		/// <summary>
		/// ���������̬�Ķ����õ�����
		/// </summary>
		private Panel cPanelContent;
		/// <summary>
		/// ���캯��
		/// </summary>
		/// <remarks>
		/// ���캯��
		/// </remarks>
		public DeluxeGrid()
		{
		}

		#region ˽�б���
		/// <summary>
		/// �ͻ��˵���Excel��ť
		/// </summary>
		/// <remarks>
		/// �ͻ��˵���Excel��ť
		/// </remarks>
		private HtmlImage clientExportExcelButton = new HtmlImage();

		/// <summary>
		/// �ͻ��˵���Word��ť
		/// </summary>
		/// <remarks>
		/// �ͻ��˵���Word��ť
		/// </remarks>
		private HtmlImage clientExportWordButton = new HtmlImage();

		/// <summary>
		/// �ͻ��˵���Excel���˰�ť
		/// </summary>
		/// <remarks>
		/// �ͻ��˵���Excel���˰�ť
		/// </remarks>
		private HtmlImage clientExportExcelTopButton = new HtmlImage();

		/// <summary>
		/// �ͻ��˵���Word���˰�ť
		/// </summary>
		/// <remarks>
		/// �ͻ��˵���Word���˰�ť
		/// </remarks>
		private HtmlImage clientExportWordTopButton = new HtmlImage();

		/// <summary>
		/// checked����
		/// </summary>
		int selectedCount = 0;

		/// <summary>
		/// ����checkboxģ��
		/// </summary>
		/// <remarks>
		/// ����checkboxģ��
		/// </remarks>
		TemplateField tempCheckBox = new TemplateField();

		/// <summary>
		/// ���˷�ҳ�Ƿ񴴽�
		/// </summary>
		/// <remarks>
		/// ���˷�ҳ�Ƿ񴴽�
		/// </remarks>
		private bool topPagerCreating = false;

		/// <summary>
		/// ����excelbutton
		/// </summary>
		private Button btnExcelExport = new Button();

		/// <summary>
		/// ����wordbutton
		/// </summary>
		private Button btnWordExport = new Button();

		/// <summary>
		/// ����ؼ������ϱ���ʾ��
		/// </summary>
		Label lblUpTitle = new Label();
		/// <summary>
		/// ����ؼ������±���ʾ�ģ�Ŀǰ��Ԥ���ɣ�
		/// </summary>
		Label lblLowTitle = new Label();
		#endregion

		#region ˽������

		/// <summary>
		/// ���Զ���ģ��
		/// </summary>
		/// <remarks>
		/// ���Զ���ģ��
		/// </remarks>
		private System.Web.UI.AttributeCollection properties;

		/// <summary>
		/// �������Զ������ģ��
		/// </summary>
		/// <remarks>
		/// �������Զ������ģ��
		/// </remarks>
		internal System.Web.UI.AttributeCollection Properties
		{
			get
			{
				if (this.properties == null)
				{
					StateBag oBag = new StateBag(true);
					this.properties = new System.Web.UI.AttributeCollection(oBag);
				}

				return this.properties;
			}
			set
			{
				this.properties = value;
			}
		}

		/// <summary>
		/// ��������excel��ͼ��
		/// </summary>
		/// <remarks>
		/// ��������excel��ͼ��
		/// </remarks>
		[Description("��������excel��ͼ��")]
		private string ExcelImgUrl
		{
			get
			{
				return Page.ClientScript.GetWebResourceUrl(typeof(DeluxeGrid),
						"MCS.Web.WebControls.DeluxeGrid.excel.gif");
			}
		}

		/// <summary>
		/// ��������word��ͼ��
		/// </summary>
		/// <remarks>
		/// ��������word��ͼ��
		/// </remarks>
		[Description("��������word��ͼ��")]
		private string WordImgUrl
		{
			get
			{
				return Page.ClientScript.GetWebResourceUrl(typeof(DeluxeGrid),
						"MCS.Web.WebControls.DeluxeGrid.word.gif");
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("openExportWordDocumentUrl")]
		private string OpenExportWordDocumentUrl
		{
			get
			{
				PageRenderMode pageRenderMode = new PageRenderMode(this.UniqueID, "openExportWordDocumentUrl");

				pageRenderMode.ContentTypeKey = ResponseContentTypeKey.WORD.ToString();
				pageRenderMode.DispositionType = ResponseDispositionType.Inline;
				pageRenderMode.AttachmentFileName = "exports.doc";
				pageRenderMode.UseNewPage = false;

				return WebUtility.GetRequestExecutionUrl(pageRenderMode);
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("checkItemName")]
		private string CheckItemName
		{
			get
			{
				return this.ClientID + "_" + "checkItem";
			}
		}

		[Browsable(false)]
		[ScriptControlProperty()]
		[ClientPropertyName("openExportExcelDocumentUrl")]
		private string OpenExportExcelDocumentUrl
		{
			get
			{
				PageRenderMode pageRenderMode = new PageRenderMode(this.UniqueID, "openExportExcelDocumentUrl");

				pageRenderMode.ContentTypeKey = ResponseContentTypeKey.EXCEL.ToString();
				pageRenderMode.DispositionType = ResponseDispositionType.Inline;
				pageRenderMode.AttachmentFileName = "exports.xls";
				pageRenderMode.UseNewPage = false;

				return WebUtility.GetRequestExecutionUrl(pageRenderMode);
			}
		}


		/// <summary>
		/// ��������excel��ͼ��
		/// </summary>
		/// <remarks>
		/// ��������excel��ͼ��
		/// </remarks> 
		[Category("Appearance")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("btnExcelClientID")]//���ô����Զ�Ӧ�ͻ������Ե�����
		[Description("ExcelButtonClientID�������˵�����ťclientID")]
		private string BtnExcelClientID
		{
			get
			{
				return btnExcelExport.ClientID;
			}
		}

		/// <summary>
		/// ��������excel��ͼ��
		/// </summary>
		/// <remarks>
		/// ��������excel��ͼ��
		/// </remarks>
		[Category("Appearance")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("btnWordClientID")]//���ô����Զ�Ӧ�ͻ������Ե�����
		[Description("WordButtonClientID�������˵�����ťclientID")]
		private string BtnWordClientID
		{
			get
			{
				return btnWordExport.ClientID;
			}
		}

		/// <summary>
		/// �ͻ��˵���ButtonID
		/// </summary>
		/// <remarks>
		/// �ͻ��˵���ButtonID
		/// </remarks> 
		[Category("Appearance")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("excelTopButtonClientID")]//���ô����Զ�Ӧ�ͻ������Ե�����
		[Description("ExportButton�ͻ��˵�����ťID")]
		private string ExcelTopButtonClientID
		{
			get
			{
				return this.clientExportExcelTopButton.ClientID;
			}
		}

		/// <summary>
		/// �ͻ��˵���ButtonID
		/// </summary>
		/// <remarks>
		/// �ͻ��˵���ButtonID
		/// </remarks> 
		[Category("Appearance")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("excelButtonClientID")]//���ô����Զ�Ӧ�ͻ������Ե�����
		[Description("ClientExportExcelTopButton�ͻ��˵�����ťID")]
		private string ExcelButtonClientID
		{
			get
			{
				return this.clientExportExcelButton.ClientID;
			}
		}

		/// <summary>
		/// �ͻ��˵���wordButtonID
		/// </summary>
		/// <remarks>
		/// �ͻ��˵���wordButtonID
		/// </remarks> 
		[Category("Appearance")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("wordTopButtonClientID")]//���ô����Զ�Ӧ�ͻ������Ե�����
		[Description("WordTopButtonClientID�ͻ��˵�����ťID")]
		private string WordTopButtonClientID
		{
			get
			{
				return this.clientExportWordTopButton.ClientID;
			}
		}

		/// <summary>
		/// �ͻ��˵���wordButtonID
		/// </summary>
		/// <remarks>
		/// �ͻ��˵���wordButtonID
		/// </remarks> 
		[Category("Appearance")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("wordButtonClientID")]//���ô����Զ�Ӧ�ͻ������Ե�����
		[Description("WordButtonClientID�ͻ��˵�����ťID")]
		private string WordButtonClientID
		{
			get
			{
				return this.clientExportWordButton.ClientID;
			}
		}

		/// <summary>
		/// �Ƿ����ڵ���״̬
		/// </summary>
		/// <remarks>
		/// �Ƿ����ڵ���״̬
		/// </remarks> 
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DefaultValue(false)]
		public bool ExportingDeluxeGrid
		{
			get
			{
				return WebControlUtility.GetViewStateValue<bool>(ViewState, "ExportingDeluxeGrid", false);
			}
			set
			{
				WebControlUtility.SetViewStateValue<bool>(ViewState, "ExportingDeluxeGrid", value);
			}
		}
		#endregion

		#region public ITemplate GridPagerTemplate
		/// <summary>
		/// DeluxePagerģ��
		/// </summary>
		/// <remarks>
		/// DeluxePagerģ��
		/// </remarks>
		[Browsable(false)]
		[Description("DeluxePagerģ��")]
		[MergableProperty(false)]
		[DefaultValue(null)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[TemplateContainer(typeof(GridViewRow))]
		[TemplateInstance(TemplateInstance.Single)]
		public ITemplate GridPagerTemplate
		{
			get
			{
				return WebControlUtility.GetViewStateValue<ITemplate>(ViewState, "GridPagerTemplate", null); ;
			}
			set
			{
				WebControlUtility.SetViewStateValue<ITemplate>(ViewState, "GridPagerTemplate", value);
			}
		}
		#endregion

		#region �ڲ�����
		/// <summary>
		/// ��ҳ��checkbox�Ƿ�ȫѡ
		/// </summary>
		/// <remarks>
		/// ��ҳ��checkbox�Ƿ�ȫѡ
		/// </remarks>
		[Browsable(false)]
		[Category("Appearance")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("isSelectedAll")]//���ô����Զ�Ӧ�ͻ������Ե�����
		[Description("�Ƿ�ȫѡ")]
		private bool IsSelectedAll
		{
			get
			{
				return WebControlUtility.GetViewStateValue<bool>(ViewState, "IsSelectedAll", false);
			}
			set
			{
				WebControlUtility.SetViewStateValue<bool>(ViewState, "IsSelectedAll", value);
			}
		}

		/// <summary>
		/// checkallȫѡ�ͻ�������
		/// </summary>
		/// <remarks>
		/// checkallȫѡ�ͻ�������
		/// </remarks> 
		[Category("Appearance")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("checkAll")]//���ô����Զ�Ӧ�ͻ������Ե�����
		[Description("checkboxȫѡ�ͻ�������")]
		[Browsable(false)]
		private string CheckAllID
		{
			get
			{
				return WebControlUtility.GetViewStateValue<string>(ViewState, "CheckAllID", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue<string>(ViewState, "CheckAllID", value);
			}
		}

		/// <summary>
		/// (ȫѡ)��ѡ�򵥻��¼�
		/// </summary>
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("selectAllCheckBoxClick")]
		[Bindable(true), Category("ClientEventsHandler"), Description("(ȫѡ)��ѡ�򵥻��¼�")]
		public string OnSelectAllCheckBoxClick
		{
			get
			{
				return GetPropertyValue("OnSelectAllCheckBoxClick", string.Empty);
			}
			set
			{
				SetPropertyValue("OnSelectAllCheckBoxClick", value);
			}
		}

		/// <summary>
		/// ��ѡ��(������ȫѡ)�����¼�
		/// </summary>
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("selectCheckBoxClick")]
		[Bindable(true), Category("ClientEventsHandler"), Description("��ѡ��(������ȫѡ)�����¼�")]
		public string OnSelectCheckBoxClick
		{
			get
			{
				return GetPropertyValue("OnSelectCheckBoxClick", string.Empty);
			}
			set
			{
				SetPropertyValue("OnSelectCheckBoxClick", value);
			}
		}

		private List<string> selectedKeys = null;

		/// <summary>
		/// ѡ����Key����
		/// </summary> 
		/// <remarks>
		/// ѡ����Key����
		/// </remarks> 
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("clientSelectedKeys")]//���ô����Զ�Ӧ�ͻ������Ե�����
		[Description("ѡ����Key����")]
		[Browsable(false)]
		public List<string> SelectedKeys
		{
			get
			{
				if (this.selectedKeys == null)
				{
					this.selectedKeys = new List<string>();
					WebControlUtility.SetViewStateValue(ViewState, "SelectedKeys", this.selectedKeys);
				}

				return this.selectedKeys;
			}
		}

		/// <summary>
		/// ��ǰҳ�е�checkbox�Ƿ�ȫѡ
		/// </summary>
		/// <remarks>
		/// ��ǰҳ�е�checkbox�Ƿ�ȫѡ
		/// </remarks>
		[Browsable(false), DefaultValue(false)]
		[Category("Appearance")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("isChecked")]//���ô����Զ�Ӧ�ͻ������Ե�����
		[Description("��ǰҳ�е�checkbox�Ƿ�ȫѡ")]
		public bool CheckedAll
		{
			get
			{
				return WebControlUtility.GetViewStateValue<bool>(ViewState, "CheckedAll", false);
			}
			set
			{
				WebControlUtility.SetViewStateValue<bool>(ViewState, "CheckedAll", value);
			}
		}

		/// <summary>
		/// ��ȡ��������ѡ�е�checkbox��valueֵ�����ŷָ�
		/// </summary>
		/// <remarks>
		/// ��ȡ��������ѡ�е�checkbox��valueֵ�����ŷָ�
		/// </remarks>
		[Browsable(false)]
		public string SelectedKeysValue
		{
			get
			{
				return string.Join(",", this.SelectedKeys.ToArray());
			}
		}
		#endregion

		#region ��������
		///// <summary>
		///// 
		///// </summary>
		//[DefaultValue((string)null), Editor("System.Web.UI.Design.WebControls.DataFieldEditor, System.Design", typeof(UITypeEditor)), TypeConverter(typeof(StringArrayConverter)), Category("Data")]
		//public override string[] DataKeyNames
		//{
		//    get
		//    {
		//        string[] result = (string[])ViewState["DataKeyNames"];

		//        if (result == null)
		//        {
		//            result = new string[0];
		//            ViewState["DataKeyNames"] = result;
		//        }

		//        return result;
		//    }
		//    set
		//    {
		//        ViewState["DataKeyNames"] = value;
		//    }
		//}

		/// <summary>
		/// �¼��ؼ�ID������ʵ����ICascadePagedControl�ӿ�
		/// </summary>
		[IDReferenceProperty(typeof(ICascadePagedControl))]
		public string CascadeControlID
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "CascadeControlID", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "CascadeControlID", value);
			}
		}

		/// <summary>
		/// Culture��Category
		/// </summary>
		[Browsable(true),
		Description("Culture��Category"),
		DefaultValue(""),
		Category("��չ")]
		public string Category
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "Category", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "Category", value);
			}
		}

		/// <summary>
		/// �Ƿ�����ͻ��˵����ݰ�
		/// </summary>
		[Browsable(true),
		Description("�Ƿ�����ͻ��˵����ݰ�"),
		DefaultValue(false),
		Category("��չ")]
		[ScriptControlProperty()]
		[ClientPropertyName("allowClientDataBind")]
		public bool AllowClientDataBind
		{
			get
			{
				return WebControlUtility.GetViewStateValue(ViewState, "AllowClientDataBind", false);
			}
			set
			{
				WebControlUtility.SetViewStateValue(ViewState, "AllowClientDataBind", value);
			}
		}

		/// <summary>
		/// ��ҳ������תҳ�����Text
		/// </summary>
		/// <remarks>
		/// ��ҳ������תҳ�����Text
		/// </remarks>
		[Browsable(true),
		Description("��ҳ������תҳ�����Text"),
		DefaultValue("inputStyle"),
		Category("��չ")]
		public string PagerInputCssClass
		{
			get
			{
				return WebControlUtility.GetViewStateValue<string>(ViewState, "PagerInputCssClass", "inputStyle");
			}
			set
			{
				WebControlUtility.SetViewStateValue<string>(ViewState, "PagerInputCssClass", value);
			}
		}

		/// <summary>
		/// ��תҳ�ؼ�CssClass
		/// </summary>
		/// <remarks>
		/// ��תҳ�ؼ�CssClass
		/// </remarks>
		[Description("��תҳ�ؼ�CssClass"),
		DefaultValue("portalButton"),
		Category("��չ")]
		public string GotoPageButtonCssClass
		{
			get
			{
				return WebControlUtility.GetViewStateValue<string>(ViewState, "GotoPageButtonCssClass", "portalButton");
			}
			set
			{
				WebControlUtility.SetViewStateValue<string>(ViewState, "GotoPageButtonCssClass", value);
			}
		}

		/// <summary>
		/// ������תҳ��ťText
		/// </summary>
		/// <remarks>
		/// ������תҳ��ťText
		/// </remarks>
		[Browsable(true),
		Description("������תҳ��ť��Text"),
		DefaultValue("��ת��"),
		Category("��չ")]
		public string GotoButtonText
		{
			get
			{
				return WebControlUtility.GetViewStateValue<string>(ViewState, "GotoButtonText", Translator.Translate(Define.DefaultCategory, "��ת��"));
			}
			set
			{
				WebControlUtility.SetViewStateValue<string>(ViewState, "GotoButtonText", value);
			}
		}

		/// <summary>
		/// ҳ����ʾģʽ
		/// </summary>
		/// <remarks>
		/// ҳ����ʾģʽ
		/// </remarks>
		[Browsable(true)]
		[Description("ҳ����ʾģʽ")]
		[DefaultValue(PagerCodeShowMode.All)]
		[Category("��չ")]
		public PagerCodeShowMode PageCodeShowMode
		{
			get
			{
				return WebControlUtility.GetViewStateValue<PagerCodeShowMode>(ViewState, "PageCodeShowMode", PagerCodeShowMode.All);
			}
			set
			{
				WebControlUtility.SetViewStateValue<PagerCodeShowMode>(ViewState, "PageCodeShowMode", value);
			}
		}

		/// <summary>
		/// ָ��GridView�ϵ������ݵ�����Դ���������
		/// </summary>
		/// <remarks>
		/// ָ��GridView�ϵ������ݵ�����Դ���������
		/// </remarks>
		[Browsable(false),
		Category("��չ"),
		DefaultValue(200),
		Description("ָ��DeluxeView�ϵ������ݵ�����Դ�����������")]
		public int DataSourceMaxRow
		{
			get
			{
				return WebControlUtility.GetViewStateValue<int>(ViewState, "DataSourceMaxRow", 0);
			}
			set
			{
				WebControlUtility.SetViewStateValue<int>(ViewState, "DataSourceMaxRow", value);
			}
		}

		/// <summary>
		/// ָ��GridView����ʾ��������
		/// </summary>
		/// <remarks>
		/// ָ��GridView����ʾ��������
		/// </remarks>
		[Browsable(true),
		Category("��չ"),
		DefaultValue("����"),
		Description("ָ��GridView����ʾ�������ݡ�")]
		public string GridTitle
		{
			get
			{
				return WebControlUtility.GetViewStateValue<string>(ViewState, "GridTitle", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue<string>(ViewState, "GridTitle", value);
			}
		}

		/// <summary>
		/// ����������ɫ
		/// </summary>
		/// <remarks>
		/// ����������ɫ
		/// </remarks>
		[Browsable(true),
		Category("��չ"),
		DefaultValue(typeof(System.Drawing.Color)),
		Description("����������ɫ")]
		public Color TitleColor
		{
			get
			{
				return WebControlUtility.GetViewStateValue<Color>(ViewState, "TitleColor", Color.FromArgb(141, 143, 149));
			}
			set
			{
				WebControlUtility.SetViewStateValue<Color>(ViewState, "TitleColor", value);
			}
		}
		/// <summary>
		/// ���������С
		/// </summary>
		/// <remarks>
		/// ���������С
		/// </remarks>
		[Browsable(true),
		Category("��չ"),
		DefaultValue(typeof(FontUnit)),
		Description("��������")]
		public FontUnit TitleFontSize
		{
			get
			{
				return WebControlUtility.GetViewStateValue<FontUnit>(ViewState, "TitleFontSize", FontUnit.Large);
			}
			set
			{
				WebControlUtility.SetViewStateValue<FontUnit>(ViewState, "TitleFontSize", value);
			}
		}

		/// <summary>
		/// �����Ƿ���ʾ��������
		/// </summary>
		/// <remarks>
		/// �����Ƿ���ʾ��������
		/// </remarks>
		[Browsable(true),
		Description("�Ƿ���ʾ��������"),
		DefaultValue(true),
		ScriptControlProperty,//���ô�����Ҫ������ͻ���
		ClientPropertyName("showExportControl"),//���ô����Զ�Ӧ�ͻ������Ե�����
		Category("��չ")]
		public bool ShowExportControl
		{
			get
			{
				return WebControlUtility.GetViewStateValue<bool>(ViewState, "ShowExportControl", false);
			}
			set
			{
				WebControlUtility.SetViewStateValue<bool>(ViewState, "ShowExportControl", value);
			}
		}

		[
		ScriptControlProperty,//���ô�����Ҫ������ͻ���
		ClientPropertyName("showPager"),//���ô����Զ�Ӧ�ͻ������Ե�����
		]
		private bool ShowPager
		{
			get
			{
				return this.AllowPaging;
			}
		}

		/// <summary>
		/// �����Ƿ�����checkbox��
		/// </summary>
		/// <remarks>
		/// �����Ƿ�����checkbox��
		/// </remarks>
		[Browsable(true),
		Description("�Ƿ�����ѡ����"),
		DefaultValue(false),
		ScriptControlProperty,//���ô�����Ҫ������ͻ���
		ClientPropertyName("showCheckBoxes"),//���ô����Զ�Ӧ�ͻ������Ե�����
		Category("��չ")]
		public bool ShowCheckBoxes
		{
			get
			{
				return WebControlUtility.GetViewStateValue<bool>(ViewState, "ShowCheckBoxes", false);
			}
			set
			{
				WebControlUtility.SetViewStateValue<bool>(ViewState, "ShowCheckBoxes", value);
			}
		}

		/// <summary>
		/// �����ʾѡ���У��Ƿ��Ƕ�ѡ
		/// </summary>
		[Browsable(true),
		Description("�����ʾѡ���У��Ƿ��Ƕ�ѡ"),
		DefaultValue(true),
			//ScriptControlProperty,//���ô�����Ҫ������ͻ���
			//ClientPropertyName("showCheckBoxs"),//���ô����Զ�Ӧ�ͻ������Ե�����
		Category("��չ")]
		public bool MultiSelect
		{
			get
			{
				return WebControlUtility.GetViewStateValue<bool>(ViewState, "MultiSelect", true);
			}
			set
			{
				WebControlUtility.SetViewStateValue<bool>(ViewState, "MultiSelect", value);
			}
		}

		/// <summary>
		/// ����checkbox�е�λ��
		/// </summary> 
		/// <remarks>
		/// ����checkbox�е�λ��
		/// </remarks>
		[Browsable(true)]
		[Description("ѡ���е�λ��")]
		[DefaultValue(RowPosition.Left)]
		[Category("��չ")]
		public RowPosition CheckBoxPosition
		{
			get
			{
				return WebControlUtility.GetViewStateValue<RowPosition>(ViewState, "CheckBoxPosition", RowPosition.Left);
			}
			set
			{
				WebControlUtility.SetViewStateValue<RowPosition>(ViewState, "CheckBoxPosition", value);
			}
		}

		/// <summary>
		/// checkboxģ��ͷ��ʽ
		/// </summary>
		/// <remarks>
		/// checkboxģ��ͷ��ʽ
		/// </remarks>
		[Browsable(true)]
		[Description("checkboxģ��ͷ��ʽ")]
		[Category("��չ")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public Style CheckBoxTemplateHeaderStyle
		{
			get
			{
				Style style = this.GetPropertyValue<System.Web.UI.WebControls.Style>("CheckBoxTemplateHeaderStyle", null);
				if (style == null)
				{
					style = new Style();
					this.SetPropertyValue<System.Web.UI.WebControls.Style>("CheckBoxTemplateHeaderStyle", style);
				}
				return style;
			}
		}

		/// <summary>
		/// checkboxģ�����ݶ�����ʽ
		/// </summary>
		/// <remarks>
		/// checkboxģ�����ݶ�����ʽ
		/// </remarks> 
		[Browsable(true)]
		[Description("checkboxģ�����ݶ�����ʽ")]
		[Category("��չ")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
		public Style CheckBoxTemplateItemStyle
		{
			get
			{
				Style style = this.GetPropertyValue<System.Web.UI.WebControls.Style>("CheckBoxTemplateItemStyle", null);
				if (style == null)
				{
					style = new Style();
					this.SetPropertyValue<System.Web.UI.WebControls.Style>("CheckBoxTemplateItemStyle", style);
				}
				return style;
			}
		}

		/// <summary>
		/// PagerCssҳ����ʽ
		/// </summary>
		[Browsable(true)]
		[Description("PagerCssҳ����ʽ")]
		[DefaultValue("")]
		[Category("��չ")]
		public string PagerCssClass
		{
			get
			{
				return WebControlUtility.GetViewStateValue<string>(ViewState, "PagerCss", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue<string>(ViewState, "PagerCss", value);
			}
		}

		/// <summary>
		/// TitleCssClass��ʽ
		/// </summary>
		[Browsable(true)]
		[Description("TitleCssClass��ʽ")]
		[DefaultValue("")]
		[Category("��չ")]
		public string TitleCssClass
		{
			get
			{
				return WebControlUtility.GetViewStateValue<string>(ViewState, "TitleCssClass", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue<string>(ViewState, "TitleCssClass", value);
			}
		}

		/// <summary>
		/// RowLineCssClass��ʽ
		/// </summary>
		[Browsable(true)]
		[Description("RowLineCssClass��ʽ,DeluxeGrid Header���ֵ���ʽ����")]
		[DefaultValue("")]
		[Category("��չ")]
		public string RowLineCssClass
		{
			get
			{
				return WebControlUtility.GetViewStateValue<string>(ViewState, "RowLineCssClass", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue<string>(ViewState, "RowLineCssClass", value);
			}
		}

		/// <summary>
		/// HeaderRowCssClass��ʽ
		/// </summary>
		[Browsable(true)]
		[Description("HeaderRowCssClass��ʽ��DeluxeGrid DataRow���ֵ���ʽ����")]
		[DefaultValue("")]
		[Category("��չ")]
		public string HeaderRowCssClass
		{
			get
			{
				return WebControlUtility.GetViewStateValue<string>(ViewState, "HeaderRowCssClass", string.Empty);
			}
			set
			{
				WebControlUtility.SetViewStateValue<string>(ViewState, "HeaderRowCssClass", value);
			}
		}

		/// <summary>
		/// PagerCssҳ����ʽ
		/// </summary> 
		[Description("����Դ�Ƿ��õ���DataSourceControl")]
		[Category("��չ")]
		private bool IsDataSourceControl
		{
			get
			{
				return string.IsNullOrEmpty(DataSourceID) == false;
			}
		}
		#endregion

		#region ˽�з���

		#region  �ؼ�չʾ����
		/// <summary>
		/// ��ʾ��������
		/// </summary>
		/// <param name="row">TableRow</param>
		/// <param name="showFont">bool</param>
		/// <remarks>
		/// ��ʾ��������
		/// </remarks>
		private void DrawingShowTitle(TableRow row, bool showFont)
		{
			TableCell cell = new TableCell();
			cell.Attributes.Add("align", "left");
			cell.Width = Unit.Percentage(20);

			if (!showFont)
			{
				string fonts = "";
				for (int i = 0; i < GridTitle.Length; i++)
				{
					fonts += "&nbsp;";
				}
			}
			else
			{
				BindGridTitle();
				this.lblUpTitle.CssClass = TitleCssClass;
				cell.Controls.Add(this.lblUpTitle);
			}

			row.Cells.Add(cell);
		}

		/// <summary>
		/// ��ʾ�����ؼ�
		/// </summary>
		/// <param name="row">TableRow</param>
		/// <param name="IsTop">bool</param>
		/// <remarks>
		/// ��ʾ�����ؼ�
		/// </remarks>
		private void DrawingExportControl(TableRow row, bool IsTop)
		{
			TableCell cell = new TableCell();
			cell.Attributes.Add("align", "right");
			//�ͻ���button
			if (IsTop)
			{
				this.clientExportExcelTopButton.ID = "exportTopFiles";
				this.clientExportExcelTopButton.Src = this.ExcelImgUrl;
				this.clientExportExcelTopButton.Style["cursor"] = "hand";

				this.clientExportWordTopButton.ID = "exportTopWordFiles";
				this.clientExportWordTopButton.Src = this.WordImgUrl;
				this.clientExportWordTopButton.Style["cursor"] = "hand";

				cell.Controls.Add(this.clientExportExcelTopButton);
				cell.Controls.Add(new LiteralControl("&nbsp;"));
				cell.Controls.Add(this.clientExportWordTopButton);
			}
			else
			{
				this.clientExportExcelButton.ID = "exportFiles";
				this.clientExportExcelButton.Src = this.ExcelImgUrl;
				this.clientExportExcelButton.Style["cursor"] = "hand";

				this.clientExportWordButton.ID = "exportWordFiles";
				this.clientExportWordButton.Src = this.WordImgUrl;
				this.clientExportWordButton.Style["cursor"] = "hand";

				cell.Controls.Add(this.clientExportExcelButton);
				cell.Controls.Add(new LiteralControl("&nbsp;"));
				cell.Controls.Add(this.clientExportWordButton);

				this.btnExcelExport.Style.Add(HtmlTextWriterStyle.Display, "none");
				this.btnWordExport.Style.Add(HtmlTextWriterStyle.Display, "none");
				this.btnWordExport.Click += new EventHandler(this.ExportData_Click);
				this.btnExcelExport.Click += new EventHandler(this.ExportData_Click);
				this.btnWordExport.CommandName = "word";
				this.btnExcelExport.CommandName = "excel";
				this.btnWordExport.ID = "btnWordServer";
				this.btnExcelExport.ID = "btnExcelServer";

				cell.Controls.Add(this.btnExcelExport);
				cell.Controls.Add(this.btnWordExport);

			}

			row.Cells.Add(cell);
		}
		#endregion

		#endregion

		#region �ܱ�������

		#endregion

		#region �¼�
		/// <summary>
		/// �����ӿؼ�
		/// </summary>
		/// <param name="dataSource"></param>
		/// <param name="dataBinding"></param>
		/// <returns></returns>
		/// <remarks>
		/// �����ӿؼ�
		/// </remarks>
        protected override int CreateChildControls(IEnumerable dataSource, bool dataBinding)
        {
            if (!this.RenderMode.OnlyRenderSelf && this.RenderMode.IsHtmlRender)
            {
                if (this.ShowExportControl)
                    this.PagerSettings.Position = PagerPosition.TopAndBottom;
                //�Ƿ��checkbox
                if (this.ShowCheckBoxes)
                    //�ͻ��˼�¼
                    this.CheckedAll = true;

                this.topPagerCreating =
                    this.PagerSettings.Position == PagerPosition.Top || this.PagerSettings.Position == PagerPosition.TopAndBottom;
            }

            return base.CreateChildControls(dataSource, dataBinding);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="postDataKey"></param>
		/// <param name="postCollection"></param>
		/// <returns></returns>
		protected override bool LoadPostData(string postDataKey, NameValueCollection postCollection)
		{
			string targetID = postCollection["__EVENTTARGET"];

			if (!string.IsNullOrEmpty(targetID))
			{
				if (targetID == this.btnWordExport.ClientID)
				{
					this.ExportData_Click(this.btnWordExport, new EventArgs());
				}
				else
					if (targetID == this.btnExcelExport.ClientID)
					{
						this.ExportData_Click(this.btnExcelExport, new EventArgs());
					}
			}
			return base.LoadPostData(postDataKey, postCollection);
		}
		/// <summary>
		/// �����ӿؼ�
		/// </summary>
		/// <remarks>
		/// �����ӿؼ�
		/// </remarks>
		protected override void CreateChildControls()
		{
			this.cPanelContent = new Panel();

			if (this.GridPagerTemplate != null)
				this.GridPagerTemplate.InstantiateIn(this.cPanelContent);

            if (this.EmptyDataTemplate != null)
                this.EmptyDataTemplate.InstantiateIn(this.cPanelContent);
			
            this.regions = new List<IAttributeAccessor>();
			this.regions.Add(this.cPanelContent);

			base.CreateChildControls();
		}

		/// <summary>
		/// InitializePager
		/// </summary>
		/// <param name="row"></param>
		/// <param name="columnSpan"></param>
		/// <param name="pagedDataSource"></param>
		/// <remarks>
		/// InitializePager
		/// </remarks>
		protected override void InitializePager(GridViewRow row, int columnSpan, PagedDataSource pagedDataSource)
		{
			TableCell tc = new TableCell();

			if (columnSpan > 1)
				tc.ColumnSpan = columnSpan;
			tc.CssClass = this.PagerCssClass;

			Table childTable = new Table();
			TableRow childRow = new TableRow();
			childTable.Width = Unit.Percentage(100);

			if (this.topPagerCreating)
				this.DrawingShowTitle(childRow, true);
			else
				this.DrawingShowTitle(childRow, false);

			TableCell pagerCell = new TableCell();

			DeluxePager pager = new DeluxePager();

			pager.TxtBoxCssClass = this.PagerInputCssClass;
			pager.TableStyle = this.PagerStyle;
			//pager.CssClass = PagerCssClass;
			pager.GotoPageButtonCssClass = this.GotoPageButtonCssClass;

			pager.GotoButtonText = this.GotoButtonText;
			pager.PageCodeShowMode = this.PageCodeShowMode;
			pager.PagerSettings = new PagerSettings(this.PagerSettings);
			pager.PageSize = this.PageSize;
			pager.PageCount = this.PageCount;

			pager.PageIndex = this.PageIndex;
			pager.RecordCount = pagedDataSource.DataSourceCount;
			pager.DataBoundControlID = this.ID;
			pager.IsPagedControl = true;
			pager.IsDataSourceControl = this.IsDataSourceControl;
			pager.Category = this.Category;

			pagerCell.Controls.Add(pager);

			childRow.Cells.Add(pagerCell);
			childRow.Style.Add(HtmlTextWriterStyle.TextAlign, "right"); //�Ҷ��� add new by longmark 08-04-14

			//�������ؼ�
			if (this.ShowExportControl)
				this.DrawingExportControl(childRow, this.topPagerCreating);

			childTable.Rows.Add(childRow);
			tc.Controls.Add(childTable);
			row.Cells.Add(tc);

			this.topPagerCreating = false;
		}

		/// <summary>
		/// ����ģʽ
		/// </summary>
		/// <remarks>
		/// ����ģʽ
		/// </remarks>
		private void SetStateByRenderMode()
		{
			this.AllowSorting = false;
			this.AllowPaging = false;
			this.PagerSettings.Visible = false;
			this.EnableViewState = false;

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPageIndexChanged(EventArgs e)
		{
			base.OnPageIndexChanged(e);

			if (this.CascadeControlID.IsNotEmpty())
			{
				Control cascadeControl = (Control)this.Page.FindControlByID(this.CascadeControlID, true);

				if (cascadeControl != null && cascadeControl is ICascadePagedControl)
					((ICascadePagedControl)cascadeControl).SetPageIndex(this, this.PageIndex);
			}
		}

		#region OnRowDataBound
		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnRowDataBound(GridViewRowEventArgs e)
		{
			switch (e.Row.RowType)
			{
				case DataControlRowType.Header:
					{
						for (int i = 0; i < e.Row.Cells.Count; i++)
						{
							TableCell cell = e.Row.Cells[i];

							if (cell.Controls.Count == 0)
							{
								cell.Text = Translator.Translate(Category, cell.Text);
							}
							else
							{
								if (cell.Controls[0] is IButtonControl)
								{
									IButtonControl button = (IButtonControl)cell.Controls[0];
									button.Text = Translator.Translate(Category, button.Text);
								}
							}

							//cell.CssClass = this.HeaderRowCssClass;
						}

						foreach (DataControlField column in this.Columns)
						{
							//column.HeaderText = Translator.Translate(Category, column.HeaderText);
						}

						if (this.ShowCheckBoxes)
						{
							if (this.CheckBoxPosition == RowPosition.Left)
								e.Row.Cells[0].CssClass = CheckBoxTemplateHeaderStyle.CssClass;
							else
								e.Row.Cells[e.Row.Cells.Count - 1].CssClass = CheckBoxTemplateHeaderStyle.CssClass;
						}
					}
					break;
				case DataControlRowType.EmptyDataRow:
					{
						if (e.Row.Cells.Count > 0)
						{
							string originalText = e.Row.Cells[0].Text;
							string targetText = originalText.Trim('\t', '\r', '\n');

							e.Row.Cells[0].Text = HttpUtility.HtmlEncode(Translator.Translate(this.Category, targetText));
						}
					}
					break;
			}

			if (this.ShowCheckBoxes)
			{
				if (e.Row.RowType == DataControlRowType.Header)
					CheckAllID = e.Row.ClientID + "_checkall";

				if (e.Row.RowType == DataControlRowType.DataRow)
				{
					string itemsValue = "";

					InputButton chk = ((InputButton)e.Row.FindControl("checkitem"));

					if (e.Row.DataItem.GetType().FullName == "System.Data.DataRowView")
					{
						if (chk != null)
						{
							itemsValue = ((System.Data.DataRowView)(e.Row.DataItem)).Row[DataKeyNames[0]].ToString();
							chk.Value = itemsValue;

							DeluxeGridCheckBoxEventArgs args = new DeluxeGridCheckBoxEventArgs();
							args.CheckBoxValue = itemsValue;
							args.RowEventArgs = e;
							DeluxeGrid_SetCheckBoxStatusHandler(e.Row.DataItem, args);
							chk.Visible = args.CheckBoxVisible;
						}
					}
					else
					{
						if (chk != null)
						{
							itemsValue = e.Row.DataItem.GetType().GetProperty(DataKeyNames[0]).GetGetMethod().Invoke(e.Row.DataItem, new Object[] { }).ToString();
							chk.Value = itemsValue;
							DeluxeGridCheckBoxEventArgs args = new DeluxeGridCheckBoxEventArgs();
							args.CheckBoxValue = itemsValue;
							args.RowEventArgs = e;
							DeluxeGrid_SetCheckBoxStatusHandler(e.Row.DataItem, args);
							chk.Visible = args.CheckBoxVisible;
						}
					}

					foreach (string keys in SelectedKeys)
					{
						if (itemsValue == keys && chk != null)
						{
							chk.Checked = true;

							this.selectedCount++;
						}
					}
				}
			}

			if (e.Row.RowType == DataControlRowType.DataRow)
				if (this.ExportingDeluxeGrid)
					for (int i = 0; i < e.Row.Cells.Count; i++)
					{
						if (this.ShowCheckBoxes && i == 0 && this.CheckBoxPosition == RowPosition.Left)
						{
							e.Row.Cells[i].CssClass = "col-chekbox";
						}
						else if (this.ShowCheckBoxes && i == e.Row.Cells.Count - 1 && this.CheckBoxPosition == RowPosition.Right)
						{
							e.Row.Cells[i].CssClass = "col-chekbox";
						}
						else
						{
							if (this.Columns[i].ItemStyle.CssClass.IsNullOrEmpty())
							{
								e.Row.Cells[i].CssClass = this.RowLineCssClass;
							}
							e.Row.Cells[i].Style.Add("word-break", "break-all");
						}
					}
				else
					for (int i = 0; i < e.Row.Cells.Count; i++)
					{
						if (this.ShowCheckBoxes && i == 0 && this.CheckBoxPosition == RowPosition.Left)
						{
							e.Row.Cells[i].CssClass = "col-chekbox";
						}
						else if (this.ShowCheckBoxes && i == e.Row.Cells.Count - 1 && this.CheckBoxPosition == RowPosition.Right)
						{
							e.Row.Cells[i].CssClass = "col-chekbox";
						}
						else
						{
							if (this.Columns[i].ItemStyle.CssClass.IsNullOrEmpty())
							{
								e.Row.Cells[i].CssClass = this.RowLineCssClass;
							}
							e.Row.Cells[i].Style.Add("word-wrap", "break-word");
						}
					}
			base.OnRowDataBound(e);
		}

		/// <summary>
		/// Onload�¼�
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			BindGridTitle();
		}
		#endregion

		/// <summary>
		/// �ͻ��˱��浽��������
		/// </summary>
		/// <param name="clientState"></param>
		/// <remarks>
		/// �ͻ��˱��浽��������
		/// </remarks>
		protected override void LoadClientState(string clientState)
		{
			this.selectedKeys = JSONSerializerExecute.Deserialize<List<string>>(clientState);
			/*
			StringBuilder strB = new StringBuilder();

			foreach (string s in SelectedKeys)
			{
				if (strB.Length > 0)
					strB.Append(",");

				strB.Append(s);
			}

			this.SelectedKeysValue = strB.ToString();
			*/
		}

		/// <summary>
		/// �������˱������ݵ��ͻ���
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// �������˱������ݵ��ͻ���
		/// </remarks>
		protected override string SaveClientState()
		{
			var result = new
			{
				selectedKeys = SelectedKeys,
				columnsInfo = GetClientColumnsInfo()
			};

			return JSONSerializerExecute.Serialize(result);
		}

		/// <summary>
		/// OnPreRender
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>
		/// OnPreRender
		/// </remarks>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			//this.ReadOnly = this.Rows.Count == 0;

			//ǿ�Ƶ�ҳҲ��ʾPager��������
			if (this.PagerSettings.Visible)
			{
				if (this.TopPagerRow != null && !this.TopPagerRow.Visible)
					this.TopPagerRow.Visible = true;
				if (this.BottomPagerRow != null && !this.BottomPagerRow.Visible)
					this.BottomPagerRow.Visible = true;
			}

			BindGridTitle();

			//��ӡ��ʱ�������checkbox��ɵ�����
			if (this.ExportingDeluxeGrid && this.ShowCheckBoxes)
			{
				if (this.CheckBoxPosition == RowPosition.Left)
					this.Columns[0].Visible = false;
				else
					this.Columns[this.Columns.Count - 1].Visible = false;
				//this.ExportingDeluxeGrid = false;
			}
			else if (this.ExportingDeluxeGrid)
			{
				this.Columns[0].Visible = !this.Columns[0].Visible;
				this.Columns[0].Visible = !this.Columns[0].Visible;
			}
		}

		/// <summary>
		/// ҳ�����ǰ�Ȱ�checkboxģ����
		/// </summary>
		/// <param name="e"></param>
		/// <remarks>
		/// ҳ�����ǰ�Ȱ�checkboxģ����
		/// </remarks>
		protected override void OnInit(EventArgs e)
		{
			if (DesignMode == false)
			{
				if (Page.Request.UserAgent.IndexOf("Microsoft Office") >= 0)
					Page.Response.End();
			}

			base.OnInit(e);

			//����checkboxģ����
			if (this.ShowCheckBoxes && !this.DesignMode)
				this.OnPagePreLoadCheckBox();
		}


		/// <summary>
		/// OnPagePreLoad
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks>
		/// OnPagePreLoad
		/// </remarks>
		protected override void OnPagePreLoad(object sender, EventArgs e)
		{
			EnsureChildControls();
			base.OnPagePreLoad(sender, e);
		}

		/// <summary>
		/// render
		/// </summary>
		/// <param name="writer"></param>
		/// <remarks>
		/// render
		/// </remarks>
		protected override void Render(HtmlTextWriter writer)
		{
			if (this.ShowCheckBoxes && !this.DesignMode)
				this.AddPageCheckBoxTemplateStyle();

			if (this.DesignMode)
			{
				if (this.cPanelContent != null)
					this.cPanelContent.RenderControl(writer);
			}
			else
			{
				this.CssClass = string.IsNullOrEmpty(this.CssClass) ? "deluxegrid" : this.CssClass + " deluxegrid";

				if (this.Rows.Count == 0)
				{
					writer.WriteBeginTag("table");
					writer.WriteAttribute("id", this.ClientID);
					writer.WriteAttribute("style", "display:none");
					writer.Write(">");
					writer.WriteEndTag("table");
				}
			}


			base.Render(writer);
		}



		private void BindGridTitle()
		{
			string title = Translator.Translate(this.Category, this.GridTitle);

			this.lblUpTitle.Text = title;
			this.lblLowTitle.Text = title;
		}

		/// <summary>
		/// ����checkbox��ʽ
		/// </summary>		 
		/// <summary>
		/// ����checkbox��ʽ
		/// </summary> 
		private void AddPageCheckBoxTemplateStyle()
		{
			this.tempCheckBox.HeaderStyle.CopyFrom(CheckBoxTemplateHeaderStyle);
			this.tempCheckBox.ItemStyle.CopyFrom(CheckBoxTemplateItemStyle);
		}

		/// <summary>
		/// ����checkbox��
		/// </summary>
		/// <remarks>
		/// ����checkbox��
		/// </remarks>
		private void OnPagePreLoadCheckBox()
		{
			ExceptionHelper.FalseThrow(this.DataKeyNames.Length == 1, "����CheckBox��ʱ��DataKeys����ȷ��һ����ֵ");

			this.DrawCheckAll(this.tempCheckBox);

			if (CheckBoxPosition == RowPosition.Left)
			{
				//this.Columns.RemoveAt(0);
				this.Columns.Insert(0, this.tempCheckBox);
			}
			else
			{
				//this.Columns.RemoveAt(this.Columns.Count-1);
				this.Columns.Add(this.tempCheckBox);
			}
		}

		/// <summary>
		/// ��checkboxȫѡ��ť
		/// </summary>
		/// <param name="temp"></param>
		/// <remarks>
		/// ��checkboxȫѡ��ť
		/// </remarks>
		private void DrawCheckAll(TemplateField temp)
		{
			temp.HeaderTemplate = new CheckBoxTemplate(DataControlRowType.Header, CheckItemName, MultiSelect);
			temp.ItemTemplate = new CheckBoxTemplate(DataControlRowType.DataRow, CheckItemName, MultiSelect);
		}

		private PrivateColumnInfo[] GetClientColumnsInfo()
		{
			List<PrivateColumnInfo> visibleFields = new List<PrivateColumnInfo>();

			for (var i = 0; i < this.Columns.Count; i++)
			{
				DataControlField field = this.Columns[i];

				if (field.Visible && (field is BoundField))
					visibleFields.Add(GetClientFieldInfo(i, (BoundField)field));
			}

			return visibleFields.ToArray();
		}

		private PrivateColumnInfo GetClientFieldInfo(int columnIndex, BoundField field)
		{
			PrivateColumnInfo result = new PrivateColumnInfo
			{
				ColumnIndex = columnIndex,
				DataField = field.DataField
			};

			ExceptionHelper.DoSilentAction(() => result.DataField = field.DataFormatString);
			ExceptionHelper.DoSilentAction(() => result.NullDisplayText = field.NullDisplayText);

			return result;
		}

		#endregion

		#region �����¼�
		/// <summary>
		/// ����Click�¼�
		/// </summary>
		/// <remarks>
		/// 
		/// </remarks>
		[Category("Behavior")]
		public event EventHandler ExportData;

		private void ExportData_Click(object sender, System.EventArgs e)
		{
			this.ExportingDeluxeGrid = true;
			if (ExportData != null)
				ExportData(sender, e);

			SetStateByRenderMode();
		}

		/// <summary>
		/// ����checkbox״̬���
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param> 
		public delegate void SetCheckBoxStatusEventHandler(object sender, DeluxeGridCheckBoxEventArgs args);
		/// <summary>
		/// ����checkbox״̬���
		/// </summary>
		public event SetCheckBoxStatusEventHandler SetCheckBoxStatusHandler;

		/// <summary>
		/// ����checkbox״̬���
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void DeluxeGrid_SetCheckBoxStatusHandler(object sender, DeluxeGridCheckBoxEventArgs args)
		{
			if (SetCheckBoxStatusHandler != null)
				SetCheckBoxStatusHandler(sender, args);
		}
		#endregion

		#region Private Columns Info
		private class PrivateColumnInfo
		{
			public int ColumnIndex { get; set; }
			public string DataField { get; set; }
			public string DataFormatString { get; set; }
			public string NullDisplayText { get; set; }
		}
		#endregion Private Columns Info

		#region ICascadePagedControl Members

		/// <summary>
		/// ���÷�ҳ����
		/// </summary>
		/// <param name="source"></param>
		/// <param name="pageIndex"></param>
		public void SetPageIndex(object source, int pageIndex)
		{
			this.PageIndex = pageIndex;
		}

		#endregion
	}

	/// <summary>
	/// checkbox�¼����
	/// </summary>
	public class DeluxeGridCheckBoxEventArgs : EventArgs
	{
		private string checkBoxValue = "";
		private bool checkBoxVisible = true;
		private GridViewRowEventArgs rowEventArgs = null;

		/// <summary>
		/// ����Ϣ
		/// </summary>
		public GridViewRowEventArgs RowEventArgs
		{
			get { return rowEventArgs; }
			internal set { rowEventArgs = value; }
		}

		/// <summary>
		/// checkboxֵ
		/// </summary>
		public string CheckBoxValue
		{
			get { return this.checkBoxValue; }
			set { this.checkBoxValue = value; }
		}

		/// <summary>
		/// checkbox�Ƿ���ʾ
		/// </summary>
		public bool CheckBoxVisible
		{
			get { return this.checkBoxVisible; }
			set { this.checkBoxVisible = value; }
		}
	}
}

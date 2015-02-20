#region
// -------------------------------------------------
// Assembly	��	MCS.Web.WebControls
// FileName	��	WordPrint.cs
// Remark	��  Word�ؼ��ķ������˴���
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		����	    20070815		����
// 1.0	    ����׿	    20090112		�޸ģ���ӡ������ɫ���⣬���޷����޶������ݡ������Ӵ�ӡʱ�ɵ���������ɫ�ʹ�С���ܡ���ӡ��ϲ���ʾ�����
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Collections;
using System.Web.UI.WebControls.WebParts;
using System.Web.Script.Serialization;
using MCS.Web.Library.Script;
using MCS.Web.Library;

[assembly: WebResource("MCS.Web.WebControls.WordPrint.WordPrint.js", "application/x-javascript")]
namespace MCS.Web.WebControls
{
	//public enum ButtonType
	//{
	//    /// <summary>
	//    /// HtmlInput  Type=button
	//    /// </summary>
	//    /// <remarks>
	//    /// HtmlInput  Type=button
	//    /// </remarks>
	//    InputButton = 0,
	//    /// <summary>
	//    /// Image
	//    /// </summary>
	//    /// <remarks>
	//    /// Image
	//    /// </remarks>
	//    ImageButton = 1,
	//    /// <summary>
	//    /// Link
	//    /// </summary>
	//    /// <remarks>
	//    /// Link
	//    /// </remarks>
	//    LinkButton = 2
	//}

	/// <summary>
	/// Word��ӡ������
	/// </summary>
	/// <remarks>
	/// Word��ӡ������
	/// </remarks>
	[RequiredScript(typeof(ControlBaseScript))]
	[ClientScriptResource("MCS.Web.WebControls.WordPrint", "MCS.Web.WebControls.WordPrint.WordPrint.js")]
	public class WordPrint : ScriptControlBase
	{
		/// <summary>
		/// �ص���ί�ж���
		/// </summary>
		/// <remarks>
		/// �ص���ί�ж���
		/// </remarks>
		public delegate void OnPrintDelegate(WordPrintDataSourceCollection DataSourceList);

		/// <summary>
		/// �ص����¼�����
		/// </summary>
		/// <remarks>
		/// �ص����¼�����
		/// </remarks>
		public event OnPrintDelegate Print;

		/// <summary>
		/// ����Դ����
		/// </summary>
		/// <remarks>
		/// ����Դ����
		/// </remarks>
		private WordPrintDataSourceCollection dataSourceList;

		#region Field
		/// <summary>
		/// ����ButtonTypes�޸Ŀռ��HTMLTag
		/// </summary>
		/// <remarks>
		/// ����ButtonTypes�޸Ŀռ��HTMLTag
		/// </remarks>
		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				switch (this.Type)
				{
					case ButtonType.ImageButton:
						return HtmlTextWriterTag.Img;
					case ButtonType.LinkButton:
						return HtmlTextWriterTag.A;
					case ButtonType.InputButton:
					default:
						this.Attributes.Add("type", "button");
						return HtmlTextWriterTag.Input;
				}
			}
		}

		/// <summary>
		/// ��ݰ�ť
		/// </summary>
		/// <remarks>
		/// ��ݰ�ť
		/// </remarks>
		[Description("���ð�ť�����ͣ�InputButton��ImageButton��LinkButton")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("accessKey")]//��Ӧ�Ŀͻ�������
		public override string AccessKey
		{
			get { return GetPropertyValue<string>("AccessKey", String.Empty); }
			set { SetPropertyValue<string>("AccessKey", value); }
		}

		/// <summary>
		/// ��ť�����InputButton��ImageButton��LinkButton
		/// </summary>
		/// <remarks>
		/// ��ť�����InputButton��ImageButton��LinkButton
		/// </remarks>
		[Description("���ð�ť�����ͣ�InputButton��ImageButton��LinkButton")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("type")]//��Ӧ�Ŀͻ�������
		public ButtonType Type
		{
			get { return GetPropertyValue<ButtonType>("Type", ButtonType.InputButton); }
			set { SetPropertyValue<ButtonType>("Type", value); }
		}

		/// <summary>
		/// ���ð�ť�ϵ��ı�
		/// </summary>
		/// <remarks>
		/// ���ð�ť�ϵ��ı�
		/// </remarks>
		[Description("���ð�ť�ϵ��ı�")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("text")]//��Ӧ�Ŀͻ�������
		public string Text
		{
			get
			{
				return GetPropertyValue<string>("Text", "��ӡ");
			}
			set
			{
				SetPropertyValue<string>("Text", value);
			}
		}

		/// <summary>
		/// ����Word�ĵ�ģ���ַ
		/// </summary>
		/// <remarks>
		/// ����Word�ĵ�ģ���ַ
		/// </remarks>
		[Description("����Word�ĵ�ģ���ַ")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("templeteUrl")]//��Ӧ�Ŀͻ�������
		public string TempleteUrl
		{
			get
			{
				string sUrl = GetPropertyValue<string>("TempleteUrl", string.Empty);
				if (!this.DesignMode && sUrl.IndexOf("http") < 0)//�����һ�����·��
				{
					sUrl = this.Page.Request.Url.Authority + this.ResolveUrl(sUrl);
					if (this.Page.Request.IsSecureConnection)
					{
						sUrl = "https://" + sUrl;
					}
					else
					{
						sUrl = "http://" + sUrl;
					}
				}

				return sUrl;
			}
			set { SetPropertyValue<string>("TempleteUrl", value); }
		}

		/// <summary>
		/// �����ImageButton��ͨ���������ָ����ťͼƬ
		/// </summary>
		/// <remarks>
		/// �����ImageButton��ͨ���������ָ����ťͼƬ
		/// </remarks>
		[Description("�����ImageButton��ͨ���������ָ����ťͼƬ")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("imageUrl")]//��Ӧ�Ŀͻ�������
		public string ImageUrl
		{
			get { return GetPropertyValue<string>("ImageUrl", string.Empty); }
			set { SetPropertyValue<string>("ImageUrl", value); }
		}

		/// <summary>
		/// ��ť��Css����
		/// </summary>
		/// <remarks>
		/// ��ť��Css����
		/// </remarks>
		[Description("��ť��Css����")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("cssClass")]//��Ӧ�Ŀͻ�������
		public override string CssClass
		{
			get { return GetPropertyValue<string>("CssClass", string.Empty); }
			set { SetPropertyValue<string>("CssClass", value); }
		}

		/// <summary>
		/// ����Word�ĵ�������Դ����
		/// </summary>
		/// <remarks>
		/// ����Word�ĵ�������Դ����
		/// </remarks>
		[Browsable(false)]//����ʾ�����ʱ
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("dataSourceList")]//��Ӧ�Ŀͻ�������
		public WordPrintDataSourceCollection DataSourceList
		{
			get
			{
				if (null == this.dataSourceList)
				{
					this.dataSourceList = new WordPrintDataSourceCollection();
				}

				return dataSourceList;
			}
		}

		/// <summary>
		/// ��Word�ĵ��д�����Ϻ󣬴����Ŀͻ����¼�
		/// </summary>
		/// <remarks>
		/// ��Word�ĵ��д�����Ϻ󣬴����Ŀͻ����¼�
		/// </remarks>
		[DefaultValue("")]
		[Bindable(true), Category("ClientEventsHandler"), Description("��Word�ĵ�������Ϻ󣬴����Ŀͻ����¼�")]
		[ClientPropertyName("createWordComplete")]//��Ӧ�Ŀͻ�������
		[ScriptControlEvent]
		public string OnCreateWordComplete
		{
			get { return GetPropertyValue<string>("OnCreateWordComplete", string.Empty); }
			set { SetPropertyValue<string>("OnCreateWordComplete", value); }
		}

		/// <summary>
		/// ��һ����Ŀ����֮ǰ�����Ŀͻ����¼�
		/// </summary>
		/// <remarks>
		/// ��һ����Ŀ����֮ǰ�����Ŀͻ����¼�
		/// </remarks>
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("beforeDataSourceItemCreate")]
		[Bindable(true), Category("ClientEventsHandler"), Description("��һ����Ŀ����֮ǰ�����Ŀͻ����¼�")]
		public string OnBeforeDataSourceItemCreate
		{
			get { return GetPropertyValue("OnBeforeDataSourceItemCreate", string.Empty); }
			set { SetPropertyValue("OnBeforeDataSourceItemCreate", value); }
		}

		/// <summary>
		/// ��һ����Ŀ����֮�󴥷��Ŀͻ����¼�
		/// </summary>
		/// <remarks>
		/// ��һ����Ŀ����֮�󴥷��Ŀͻ����¼�
		/// </remarks>
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("dataSourceItemCreated")]
		[Bindable(true), Category("ClientEventsHandler"), Description("��һ����Ŀ����֮�󴥷��Ŀͻ����¼�")]
		public string OnDataSourceItemCreated
		{
			get { return GetPropertyValue("OnDataSourceItemCreated", string.Empty); }
			set { SetPropertyValue("OnDataSourceItemCreated", value); }
		}
		#endregion

		/// <summary>
		/// ���캯��
		/// </summary>
		/// <remarks>
		/// ���캯��
		/// </remarks>
		public WordPrint()
			: base(true)
		{
		}

		/// <summary>
		/// ��дRender,��Render��ʱ����ݰ�ť������Ӳ�ͬ������
		/// </summary>
		/// <remarks>
		/// ��дRender,��Render��ʱ����ݰ�ť������Ӳ�ͬ������
		/// </remarks>
		/// <param name="writer">HtmlTextWriter</param>
		protected override void Render(HtmlTextWriter writer)
		{
			this.SetControlText(this.Text);

			base.Render(writer);
		}

		/// <summary>
		/// ����DataΪDataResult
		/// </summary>
		/// <remarks>
		/// ����DataΪDataResult
		/// </remarks>
		/// <param name="dataSourceList">����Դ����</param>
		protected void TranDataSourceList(WordPrintDataSourceCollection dataSourceList)
		{
			for (int i = 0; i < dataSourceList.Count; i++)
			{
				dataSourceList[i].DataResult = WebControlUtility.GetDataSourceResult(this, dataSourceList[i].Data);
			}
		}

		/// <summary>
		/// ���ݰ�ť���������ð�ť�ı�������
		/// </summary>
		/// <remarks>
		/// ���ݰ�ť���������ð�ť�ı�������
		/// </remarks>
		/// <param name="sValue">��ť���ı�</param>
		private void SetControlText(string sValue)
		{
			for (int i = 0; i < this.Controls.Count; i++)
			{
				if (this.Controls[i].ID == "WordPint_LinkText_" + this.ID)
				{
					this.Controls.Remove(this.Controls[i]);
					break;
				}
			}
			this.Attributes.Add("class", this.CssClass);
			//��ͬ��HtmlControl��Ҫ�Ӳ�ͬ������
			switch (this.Type)
			{
				case ButtonType.ImageButton:
					if (this.ImageUrl != String.Empty)
						this.Attributes.Add("src", this.ImageUrl);
					break;
				case ButtonType.LinkButton:
					Literal foLtr = new Literal();
					foLtr.ID = "WordPint_LinkText_" + this.ID;
					foLtr.Text = sValue;
					this.Controls.Add(foLtr);
					this.Attributes.Add("href", "#");
					break;
				case ButtonType.InputButton:
				default:
					this.Attributes.Add("value", sValue);
					if (this.AccessKey != String.Empty)
						this.Attributes.Add("accesskey", this.AccessKey);
					break;
			}
		}

		/// <summary>
		/// ��������Դ
		/// </summary>
		/// <remarks>
		/// ��������Դ
		/// </remarks>
		/// <param name="e">EventArgs</param>
		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			if (!this.DesignMode)
			{
				TranDataSourceList(this.DataSourceList);
			}
		}

		/// <summary>
		/// �����ص���Ӧonprint��ȡ����Դ
		/// </summary>
		/// <remarks>
		/// �����ص���Ӧonprint��ȡ����Դ
		/// </remarks>
		[ScriptControlMethod]
		public WordPrintDataSourceCollection CallBackOnPrintMethod()
		{
			WordPrintDataSourceCollection dataSourceListResult = new WordPrintDataSourceCollection();//���ص�ֵ

			if (Print != null)
			{
				Print(dataSourceListResult);
				TranDataSourceList(dataSourceListResult);
			}

			return dataSourceListResult;
		}
	}

	/// <summary>
	/// ��ӡ����Դ
	/// </summary>
	/// <remarks>
	/// ��ӡ����Դ
	/// </remarks>
	public class WordPrintDataSource
	{
		private string name = string.Empty;
		private int colorArgb = 0;
		private int fontSize = 0;
		private object data = null;
		private IEnumerable dataResult = null;

		/// <summary>
		/// ��˵�еĹ��캯��
		/// </summary>
		/// <remarks>
		/// ���캯��
		/// </remarks>
		public WordPrintDataSource()
		{
		}

		/// <summary>
		/// ��˵�еĹ��캯��
		/// </summary>
		/// <remarks>
		/// ��˵�еĹ��캯��
		/// </remarks>
		/// <param name="name">����Դ����</param>
		/// <param name="data">����Դ�е�����</param>
		public WordPrintDataSource(string name, System.Collections.IEnumerable data)
		{
			this.name = name;
			this.data = data;
		}

		/// <summary>
		/// ����Դ����
		/// </summary>
		/// <remarks>
		/// ����Դ����
		/// </remarks>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// ������ɫ
		/// </summary>
		/// <remarks>
		/// ������ɫ
		/// </remarks>
		public Color FontColor
		{
			set { colorArgb = value.R + value.G * 256 + value.B * 65536; }
		}

		/// <summary>
		/// ������ɫֵ
		/// </summary>
		/// <remarks>
		/// ������ɫֵ
		/// </remarks>
		public int ColorArgb
		{
			get { return colorArgb; }
			set { colorArgb = value; }
		}

		/// <summary>
		/// �����С
		/// </summary>
		/// <remarks>
		/// �����С
		/// </remarks>
		public int FontSize
		{
			get { return fontSize; }
			set { fontSize = value; }
		}

		/// <summary>
		/// ����Դ�е�����
		/// </summary>
		/// <remarks>
		/// ����Դ�е�����
		/// </remarks>
		[ScriptIgnore]
		public object Data
		{
			get { return data; }
			set { data = value; }
		}

		/// <summary>
		/// ����������
		/// </summary>
		/// <remarks>
		/// ����������
		/// </remarks>
		public IEnumerable DataResult
		{
			get { return this.dataResult; }
			set { this.dataResult = value; }
		}
	}

	/// <summary>
	/// ��ӡ����Դ����
	/// </summary>
	/// <remarks>
	/// ��ӡ����Դ����
	/// </remarks>
	public class WordPrintDataSourceCollection : CollectionBase
	{
		/// <summary>
		/// ���ؼ����е�index����Ŀ
		/// </summary>
		/// <remarks>���ؼ����е�index����Ŀ</remarks>
		/// <param name="index">����</param>
		/// <returns>WordPrintDataSource</returns>
		public WordPrintDataSource this[int index]
		{
			get
			{
				return ((WordPrintDataSource)List[index]);
			}
			set
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// ���һ����Ŀ
		/// </summary>
		/// <remarks>
		/// ���һ����Ŀ
		/// </remarks>
		/// <param name="name">����</param>
		/// <param name="data">����</param>
		/// <returns>����</returns>
		public int Add(string name, object data)
		{
			return this.Add(name, data);
		}

		/// <summary>
		/// ���һ����Ŀ
		/// </summary>
		/// <remarks>
		/// ���һ����Ŀ
		/// </remarks>
		/// <param name="value">WordPrintDataSource</param>
		/// <returns>����</returns>
		public int Add(WordPrintDataSource value)
		{
			int nIndex = List.Add(value);
			if (value.Name == string.Empty) value.Name = "DataSource_" + nIndex.ToString();
			return nIndex;
		}

		/// <summary>
		/// ȡ��ָ����Ŀ������
		/// </summary>
		/// <remarks>
		/// ȡ��ָ����Ŀ������
		/// </remarks>
		/// <param name="value">WordPrintDataSource</param>
		/// <returns>����</returns>
		public int IndexOf(WordPrintDataSource value)
		{
			return (List.IndexOf(value));
		}

		/// <summary>
		/// ��ָ������������һ����Ŀ
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		public void Insert(int index, WordPrintDataSource value)
		{
			List.Insert(index, value);
		}

		/// <summary>
		/// �Ƴ�һ������Դ
		/// </summary>
		/// <remarks>�Ƴ�һ������Դ</remarks>
		/// <param name="value">����Դ</param>
		public void Remove(WordPrintDataSource value)
		{
			List.Remove(value);
		}

		/// <summary>
		/// �ж��Ƿ�����������Դ
		/// </summary>
		/// <remarks>
		/// �ж��Ƿ�����������Դ
		/// </remarks>
		/// <param name="value">����Դ</param>
		/// <returns>�Ƿ����</returns>
		public bool Contains(WordPrintDataSource value)
		{
			return (List.Contains(value));
		}
	}
}

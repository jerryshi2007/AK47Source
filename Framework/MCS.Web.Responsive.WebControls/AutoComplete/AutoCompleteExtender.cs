#region
// -------------------------------------------------
// Assembly	��	MCS.Web.WebControls
// FileName	��	AutoCompleteExtender.cs
// Remark	��  �Զ���ɵķ������˴���
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
using System.Reflection;
using System.Web.UI.WebControls.WebParts;
using MCS.Library.Core;
using MCS.Web.Library.Script;
using MCS.Web.Responsive.Library.Script;

[assembly: WebResource("MCS.Web.Responsive.WebControls.AutoComplete.AutoCompleteExtender.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.AutoComplete.AutoCompleteExtender.css", "text/css")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.AutoComplete.check.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.AutoComplete.hourglass.gif", "image/gif")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.AutoComplete.ou.gif", "image/gif")]

namespace MCS.Web.Responsive.WebControls
{
	/// <summary>
	/// �Զ���ɵķ������˴���
	/// </summary>
	/// <remarks>
	/// �Զ���ɵķ������˴���
	///		����������������ͻ��ˣ���ʵ�ַ��������¼�
	/// </remarks>
	[RequiredScript(typeof(ControlBaseScript))]
	[Designer(typeof(AutoCompleteExtenderDesigner))]
	[TargetControlType(typeof(Control))]
    [ClientCssResource("MCS.Web.Responsive.WebControls.AutoComplete.AutoCompleteExtender.css")]
    [ClientScriptResource("MCS.Web.WebControls.AutoCompleteExtender", "MCS.Web.Responsive.WebControls.AutoComplete.AutoCompleteExtender.js")]
	public class AutoCompleteExtender : DataBoundExtenderControl
	{
		/// <summary>
		/// �ص���ί�ж���
		/// </summary>
		/// <remarks>
		/// �ص���ί�ж���
		/// </remarks>
		/// <param name="sPrefix">ǰ׺</param>
		/// <param name="iCount">����¼��</param>
		/// <param name="eventContext">�ؼ��ص�ʱ��������</param>
		/// <param name="result">���صĽ��</param>
		public delegate void GetDataSourceDelegate(string sPrefix, int iCount, object eventContext, ref System.Collections.IEnumerable result);

		/// <summary>
		/// �ص����¼�����
		/// </summary>
		/// <remarks>
		/// �ص����¼�����
		/// </remarks>
		public event GetDataSourceDelegate GetDataSource;

		#region Field

		private object eventContext = null;

		/// <summary>
		/// ��Word�ĵ��д�����Ϻ󣬴����Ŀͻ����¼�
		/// </summary>
		/// <remarks>
		/// ��Word�ĵ��д�����Ϻ󣬴����Ŀͻ����¼�
		/// </remarks>
		[DefaultValue("")]
		[Bindable(true), Category("ClientEventsHandler"), Description("��ѡ����һ����Ŀ�󣬴����Ŀͻ����¼�")]
		[ClientPropertyName("itemSelected")]// ��Ӧ�Ŀͻ�������
		[ScriptControlEvent]
		public string OnItemSelected
		{
			get { return this.GetPropertyValue<string>("OnItemSelected", string.Empty); }
			set { this.SetPropertyValue<string>("OnItemSelected", value); }
		}

		/// <summary>
		/// У��󣬴����Ŀͻ����¼�
		/// </summary>
		[DefaultValue("")]
		[Bindable(true), Category("ClientEventsHandler"), Description("У��󣬴����Ŀͻ����¼�")]
		[ClientPropertyName("valueValidated")]// ��Ӧ�Ŀͻ�������
		[ScriptControlEvent]
		public string OnValueValidated
		{
			get { return this.GetPropertyValue<string>("OnValueValidated", string.Empty); }
			set { this.SetPropertyValue<string>("OnValueValidated", value); }
		}

		/// <summary>
		/// У��󣬴����Ŀͻ����¼�
		/// </summary>
		[DefaultValue("")]
		[Bindable(true), Category("ClientEventsHandler"), Description("�ͻ��˷���ص�֮ǰ")]
		[ClientPropertyName("beforeInvoke")]// ��Ӧ�Ŀͻ�������
		[ScriptControlEvent]
		public string OnClientBeforeInvoke
		{
			get { return this.GetPropertyValue<string>("OnClientBeforeInvoke", string.Empty); }
			set { this.SetPropertyValue<string>("OnClientBeforeInvoke", value); }
		}

		/// <summary>
		/// �Ƿ���Զ���ɹ���
		/// </summary>
		/// <remarks>
		/// �Ƿ���Զ���ɹ���
		///     True:��
		///     False:�ر�
		/// </remarks>
		[DefaultValue(true)]
		[ScriptControlProperty]// ���ô�����Ҫ������ͻ���
		[ClientPropertyName("isAutoComplete")]// ��Ӧ�Ŀͻ�������
		public bool IsAutoComplete
		{
			get { return this.GetPropertyValue("IsAutoComplete", true); }
			set { this.SetPropertyValue("IsAutoComplete", value); }
		}

		/// <summary>
		/// �Ƿ��л�����ʱ�Զ�У��
		/// </summary>
		/// <remarks>
		/// �Ƿ��л�����ʱ�Զ�У��
		///     True:��
		///     False:�ر�
		/// </remarks>
		[DefaultValue(true)]
		[ScriptControlProperty]
		[ClientPropertyName("autoValidateOnChange")]// ��Ӧ�Ŀͻ�������
		public bool AutoValidateOnChange
		{
			get { return this.GetPropertyValue("AutoValidateOnChange", true); }
			set { this.SetPropertyValue("AutoValidateOnChange", value); }
		}

		/// <summary>
		/// ������ٸ��ַ���ʼ�Զ���ɣ�Ĭ��Ϊ3
		/// </summary>
		/// <remarks>
		/// ������ٸ��ַ���ʼ�Զ���ɣ�Ĭ��Ϊ3
		/// </remarks>
		[DefaultValue(3)]
		[ScriptControlProperty]// ���ô�����Ҫ������ͻ���
		[ClientPropertyName("minimumPrefixLength")]// ��Ӧ�Ŀͻ�������
		public int MinimumPrefixLength
		{
			get { return this.GetPropertyValue("MinimumPrefixLength", 3); }
			set { this.SetPropertyValue("MinimumPrefixLength", value); }
		}

		/// <summary>
		/// �Զ���ɼ����Ĭ��1000���롣����ֹͣ��೤ʱ�俪ʼ�Զ���ɣ���λ�����롣1000����=1��
		/// </summary>
		/// <remarks>
		/// �Զ���ɼ����Ĭ��1000���롣����ֹͣ��೤ʱ�俪ʼ�Զ���ɣ���λ�����롣1000����=1��
		/// </remarks>
		[DefaultValue(1000)]
		[ScriptControlProperty]// ���ô�����Ҫ������ͻ���
		[ClientPropertyName("completionInterval")]// ��Ӧ�Ŀͻ�������
		public int CompletionInterval
		{
			get { return this.GetPropertyValue("CompletionInterval", 1000); }
			set { this.SetPropertyValue("CompletionInterval", value); }
		}

		/// <summary>
		/// ����ƶ����Զ���ɵ���Ŀ�ϵ�CssClass
		/// </summary>
		/// <remarks>
		/// ����ƶ����Զ���ɵ���Ŀ�ϵ�CssClass
		/// </remarks>
		[DefaultValue("")]
		[Category("Appearance")]
		[ScriptControlProperty]// ���ô�����Ҫ������ͻ���
		[ClientPropertyName("itemHoverCssClass")]
		public string ItemHoverCssClass
		{
			get { return this.GetPropertyValue("ItemHoverCssClass", string.Empty); }
			set { this.SetPropertyValue("ItemHoverCssClass", string.Empty); }
		}

		/// <summary>
		/// ������������֤����������������޷���ȫƥ�䵽����Դ�е�ĳһ����CssClass
		/// </summary>
		/// <remarks>
		/// ������������֤����������������޷���ȫƥ�䵽����Դ�е�ĳһ����CssClass
		/// </remarks>
		[DefaultValue("")]
		[Category("Appearance")]
		[ScriptControlProperty]// ���ô�����Ҫ������ͻ���
		[ClientPropertyName("errorCssClass")]
		public string ErrorCssClass
		{
			get { return this.GetPropertyValue("ErrorCssClass", string.Empty); }
			set { this.SetPropertyValue("ErrorCssClass", value); }
		}

		/// <summary>
		/// �Զ���ɵ���ĿCssClass
		/// </summary>
		/// <remarks>
		/// �Զ���ɵ���ĿCssClass
		/// </remarks>
		[DefaultValue("")]
		[Category("Appearance")]
		[ScriptControlProperty]// ���ô�����Ҫ������ͻ���
		[ClientPropertyName("itemCssClass")]
		public string ItemCssClass
		{
			get { return this.GetPropertyValue("ItemCssClass", string.Empty); }
			set { this.SetPropertyValue("ItemCssClass", value); }
		}

		/// <summary>
		/// �ؼ��Ƿ�������֤,��������������������޷���ȫƥ�䵽����Դ�е�ĳһ��
		/// ��Ӧ��ErrorStyle���ؼ�
		/// </summary>
		/// <remarks>
		/// �ؼ��Ƿ�������֤,��������������������޷���ȫƥ�䵽����Դ�е�ĳһ��
		/// ��Ӧ��ErrorStyle���ؼ�
		///     True:����
		///     False:�ر�
		/// </remarks>
		[DefaultValue(false)]
		[ScriptControlProperty]// ���ô�����Ҫ������ͻ���
		[ClientPropertyName("requireValidation")]// ��Ӧ�Ŀͻ�������
		public bool RequireValidation
		{
			get { return this.GetPropertyValue("RequireValidation", false); }
			set { this.SetPropertyValue("RequireValidation", value); }
		}

		/// <summary>
		/// �¼��ص�ʱ��Context
		/// </summary>
		[DefaultValue(false)]
		[ScriptControlProperty]// ���ô�����Ҫ������ͻ���
		[ClientPropertyName("eventContext")]// ��Ӧ�Ŀͻ�������
		public object EventContext
		{
			get { return this.eventContext; }
			set { this.eventContext = value; }
		}

		/// <summary>
		/// �ؼ��Զ���ɳ������б�����ʾ������¼������
		///     Ĭ��Ϊ-1����ʾȫ������
		/// </summary>
		/// <remarks>
		/// �ؼ��Զ���ɳ������б�����ʾ������¼������
		///     Ĭ��Ϊ-1����ʾȫ������
		/// </remarks>
		[DefaultValue(-1)]
		[ScriptControlProperty]// ���ô�����Ҫ������ͻ���
		[ClientPropertyName("maxCompletionRecordCount")]// ��Ӧ�Ŀͻ�������
		public int MaxCompletionRecordCount
		{
			get { return this.GetPropertyValue("MaxCompletionRecordCount", -1); }
			set { this.SetPropertyValue("MaxCompletionRecordCount", value); }
		}

		/// <summary>
		/// �ؼ��Զ���ɵ�����ѡ�񴰿ڵ����߶ȣ�Ĭ��Ϊ260px��
		///     �����¼������С�ڵ������ֵ���������ڵĸ߶�����Ӧ������������ֵ������ʾ������
		/// </summary>
		/// <remarks>
		/// �ؼ��Զ���ɵ�����ѡ�񴰿ڵ����߶ȣ�Ĭ��Ϊ260px��
		///     �����¼������С�ڵ������ֵ���������ڵĸ߶�����Ӧ������������ֵ������ʾ������
		/// </remarks>
		[DefaultValue(260)]
		[ScriptControlProperty]// ���ô�����Ҫ������ͻ���
		[ClientPropertyName("maxPopupWindowHeight")]// ��Ӧ�Ŀͻ�������
		public int MaxPopupWindowHeight
		{
			get { return this.GetPropertyValue("MaxPopupWindowHeight", 260); }
			set { this.SetPropertyValue("MaxPopupWindowHeight", value); }
		}

		/// <summary>
		/// �Ƿ��Զ��ص�
		/// </summary>
		/// <remarks>
		/// �Ƿ��Զ��ص�
		/// </remarks>
		[DefaultValue("")]
		[ScriptControlProperty]// ���ô�����Ҫ������ͻ���
		[ClientPropertyName("autoCallBack")]// ��Ӧ�Ŀͻ�������
		public bool AutoCallBack
		{
			get { return this.GetPropertyValue("AutoCallBack", false); }
			set { this.SetPropertyValue("AutoCallBack", value); }
		}

		/// <summary>
		/// ���ÿͻ��˻���
		/// </summary>
		/// <remarks>
		/// ���ÿͻ��˻���
		/// </remarks>
		[DefaultValue(true)]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("enableCaching")]//��Ӧ�Ŀͻ�������
		public bool EnableCaching
		{
			get { return this.GetPropertyValue("EnableCaching", true); }
			set { this.SetPropertyValue("EnableCaching", value); }
		}

		/// <summary>
		/// ��������Դ
		/// </summary>
		/// <remarks>
		/// ��������Դ
		/// </remarks>
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("dataList")]//��Ӧ�Ŀͻ�������
		public System.Collections.IEnumerable DataList
		{
			//get { return base.DataSourceResult; }
			get
			{
				return this.DataSourceResult;
			}
		}

		/// <summary>
		/// �ṩ�ı���ʾ������Դ���Լ���
		/// </summary>
		/// <remarks>
		/// �ṩ�ı���ʾ������Դ���Լ���
		/// </remarks>
		[Category("Appearance")]
		[Description("ѡ�����ı���ʾ���Լ���")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("dataTextFieldList")]//��Ӧ�Ŀͻ�������
		public string[] DataTextFieldList
		{
			get
			{
				return this.GetPropertyValue<string[]>("DataTextFieldList", new string[] { });
			}

			set
			{
				this.SetPropertyValue<string[]>("DataTextFieldList", value);
			}
		}

		/// <summary>
		/// ����Щ�ֶ���ƥ���������Ŀ��ֻҪ��һ���ֶη�����������Ϊ��ƥ��ɹ�
		/// </summary>
		/// <remarks>
		/// ����Щ�ֶ���ƥ���������Ŀ��ֻҪ��һ���ֶη�����������Ϊ��ƥ��ɹ�
		/// </remarks>
		[Category("Appearance")]
		[Description("�ƶ�����Щ�ֶ���ƥ���������Ŀ��ֻҪ��һ���ֶη�����������Ϊ��ƥ��ɹ�")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("compareFieldName")]//��Ӧ�Ŀͻ�������
		public string[] CompareFieldName
		{
			get
			{
				return this.GetPropertyValue<string[]>("CompareFieldName", new string[] { });
			}

			set
			{
				this.SetPropertyValue<string[]>("CompareFieldName", value);
			}
		}

		/// <summary>
		/// ָ����Ŀ��Valueֵ
		/// </summary>
		/// <remarks>
		/// ָ����Ŀ��Valueֵ
		/// </remarks>
		[Category("Appearance")]
		[Description("ָ����Ŀ��Valueֵ")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("dataValueField")]//��Ӧ�Ŀͻ�������
		public string DataValueField
		{
			get
			{
				return this.GetPropertyValue<string>("DataValueField", string.Empty);
			}

			set
			{
				this.SetPropertyValue<string>("DataValueField", value);
			}
		}

		/// <summary>
		/// ��ʾ���ݸ�ʽ���ַ���,����������ʾ��ʽ
		/// </summary>
		/// <remarks>
		/// ��ʾ���ݸ�ʽ���ַ���,����������ʾ��ʽ
		/// </remarks>
		[Category("Appearance")]
		[Description("ָ����ʾ���ݵĸ�ʽ���ַ���")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("dataTextFormatString")]//��Ӧ�Ŀͻ�������
		public string DataTextFormatString
		{
			get
			{
				return this.GetPropertyValue<string>("DataTextFormatString", string.Empty);
			}

			set
			{
				this.SetPropertyValue<string>("DataTextFormatString", value);
			}
		}

		/// <summary>
		/// ������ı�
		/// </summary>
		/// <remarks>
		/// ������ı�
		/// </remarks>
		[DefaultValue("")]
		public string Text
		{
			get { return this.GetPropertyValue<string>("Text", string.Empty); }
			set { this.SetPropertyValue<string>("Text", value); }
		}

		/// <summary>
		/// ѡ�������ͼ��
		/// </summary>
		/// <remarks>
		/// ѡ�������ͼ��
		/// </remarks>
		[Description("����ͼ��")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("checkImage")]//��Ӧ�Ŀͻ�������
		public string CheckImage
		{
			get
			{
				return this.GetPropertyValue<string>("CheckImage",
					Page.ClientScript.GetWebResourceUrl(typeof(AutoCompleteExtender),
                        "MCS.Web.Responsive.WebControls.AutoComplete.check.gif"));
			}

			set
			{
				this.SetPropertyValue<string>("CheckImage", value);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[Description("�ȴ���ͼ��")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("waitingImage")]//��Ӧ�Ŀͻ�������
		public string WaitingImage
		{
			get
			{
				return this.GetPropertyValue<string>("WaitingImage",
					Page.ClientScript.GetWebResourceUrl(typeof(AutoCompleteExtender),
                        "MCS.Web.Responsive.WebControls.AutoComplete.hourglass.gif"));
			}

			set
			{
				this.SetPropertyValue<string>("WaitingImage", value);
			}
		}

		/// <summary>
		/// �Ƿ���ʾ����ͼ��
		/// </summary>
		[Description("�Ƿ���ʾ����ͼ��")]
		[ScriptControlProperty]//���ô�����Ҫ������ͻ���
		[ClientPropertyName("showCheckImage")]//��Ӧ�Ŀͻ�������
		public bool ShowCheckImage
		{
			get
			{
				return this.GetPropertyValue("ShowCheckImage", false);
			}

			set
			{
				this.SetPropertyValue("ShowCheckImage", value);
			}
		}

		/// <summary>
		/// �����ѡ����򱣴�ѡ���ֵ
		/// </summary>
		/// <remarks>
		/// �����ѡ����򱣴�ѡ���ֵ
		/// </remarks>
		[DefaultValue("")]
		public string SelectValue
		{
			get { return this.GetPropertyValue<string>("SelectValue", string.Empty); }
			set { this.SetPropertyValue<string>("SelectValue", value); }
		}

		/// <summary>
		/// �ص�ʱ�������ģ���ʹ�����ṩ
		/// </summary>
		[DefaultValue("")]
		[ScriptControlProperty]
		[ClientPropertyName("callBackContext")]
		[Bindable(true), Description("�ص�ʱ�������ģ���ʹ�����ṩ")]
		public string CallBackContext
		{
			get
			{
				return this.GetPropertyValue("callBackContext", string.Empty);
			}

			set
			{
				this.SetPropertyValue("callBackContext", value);
			}
		}
		#endregion

		/// <summary>
		/// ͨ�����������ص�ҳ���ϵĹ�����������ͨ��js�ű��е�invoke���������Ȼ��˷���ͨ��Ӧ��
		/// ���õ�invokeMethodָ����ҳ���ϵķ���������ҳ��Ĺ���������
		/// </summary>
		/// <remarks>
		/// ͨ�����������ص�ҳ���ϵĹ�����������ͨ��js�ű��е�invoke���������Ȼ��˷���ͨ��Ӧ��
		/// ���õ�invokeMethodָ����ҳ���ϵķ���������ҳ��Ĺ���������
		/// </remarks>
		/// <param name="sPrefix">��������ݣ���Ϊƥ���ǰ׺</param>
		/// <param name="iCount">��ȡ�����ݼ�¼�������ֵ</param>
		/// <returns>��ȡ������</returns>
		[ScriptControlMethod]
		public System.Collections.IEnumerable CallBackPageMethod(string sPrefix, int iCount)
		{
			System.Collections.IEnumerable fenumResult = null;//���ص�ֵ

			if (this.GetDataSource != null)
				this.GetDataSource(sPrefix, iCount, this.eventContext, ref fenumResult);

			return fenumResult;
		}

		#region ClientState
		/// <summary>
		/// ����ClientState
		/// </summary>
		/// <remarks>
		/// ����ClientState
		///     ClientState�б������һ������Ϊ2��һά����
		///         ��һ��Ϊ������е��ı�
		///         �ڶ���Ϊѡ����Ŀ��Value������ֹ����벻��ѡ����Ϊ ��</remarks>
		/// <param name="clientState">���л����clientState</param>
		protected override void LoadClientState(string clientState)
		{
			base.LoadClientState(clientState);

			object[] foArray = JSONSerializerExecute.Deserialize<object[]>(clientState);

			if (null != foArray && foArray.Length > 0)
			{
				this.Text = foArray[0].ToString();//����Text

				//����Value
				if (foArray.Length > 1 && null != foArray[1])
					this.SelectValue = foArray[1].ToString();
				else
					this.SelectValue = string.Empty;

				//����DataList
				if (foArray.Length > 2 && null != foArray[2])
					this.DataSource = (System.Collections.IEnumerable)foArray[2];
				else
					this.DataSource = null;

				if (foArray.Length > 3 && null != foArray[3])
					this.eventContext = foArray[3];
				else
					this.eventContext = null;
			}
			else
			{
				this.SelectValue = string.Empty;
				this.Text = string.Empty;
				this.DataSource = null;
				this.eventContext = null;
			}
		}

		/// <summary>
		/// ����ClientState
		/// </summary>
		/// <remarks>
		/// ����ClientState
		///     ClientState�б������һ������Ϊ2��һά����
		///         ��һ��Ϊ������е��ı�
		///         �ڶ���Ϊѡ����Ŀ��Value������ֹ����벻��ѡ����Ϊ String.Empty
		///         ������ΪDataList����Դ
		/// </remarks>
		/// <returns>���л����CLientState�ַ���</returns>
		protected override string SaveClientState()
		{
			object[] foArray = new object[] { this.Text, this.SelectValue, this.DataList, this.eventContext };

			return JSONSerializerExecute.Serialize(foArray);
		}
		#endregion
	}
}
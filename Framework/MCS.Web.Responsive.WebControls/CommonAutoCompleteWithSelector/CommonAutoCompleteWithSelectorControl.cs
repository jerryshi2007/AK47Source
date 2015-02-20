using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using MCS.Web.Library.Script;
using MCS.Web.Responsive.Library.Script;

#region
[assembly: WebResource("MCS.Web.Responsive.WebControls.CommonAutoCompleteWithSelector.CommonAutoCompleteWithSelectorControl.js", "application/x-javascript")]
[assembly: WebResource("MCS.Web.Responsive.WebControls.CommonAutoCompleteWithSelector.CommonAutoCompleteWithSelectorControl.css", "text/css")]
#endregion

namespace MCS.Web.Responsive.WebControls
{
	public delegate IList ValidateInputHandler(string chkString, object context);
	public delegate IEnumerable GetDataSourceHandler(string chkString, object context);
	//public delegate IList<object> LoadDataHandler(string value);
	//public delegate string SaveDataHandler(IList<object> data);

	[RequiredScript(typeof(ControlBaseScript), 1)]
	[RequiredScript(typeof(HBCommonScript), 2)]
	[RequiredScript(typeof(AutoCompleteExtender), 3)]
    [ClientScriptResource("MCS.Web.WebControls.CommonAutoCompleteWithSelectorControl", "MCS.Web.Responsive.WebControls.CommonAutoCompleteWithSelector.CommonAutoCompleteWithSelectorControl.js")]
    [ClientCssResource("MCS.Web.Responsive.WebControls.CommonAutoCompleteWithSelector.CommonAutoCompleteWithSelectorControl.css")]
	[ToolboxData("<{0}:CommonAutoCompleteWithSelectorControl runat=server></{0}:CommonAutoCompleteWithSelectorControl>")]
	public class CommonAutoCompleteWithSelectorControl : AutoCompleteWithSelectorControlBase, INamingContainer
	{
		/// <summary>
		/// 
		/// </summary>
		public CommonAutoCompleteWithSelectorControl()
			: base(true, HtmlTextWriterTag.Div)
		{
		}

		#region 属性
		private IList _selectedData;


		/// <summary>
		/// 数据类型
		/// </summary>
		[Description("数据类型")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("dataType")]//对应的客户端属性
		public override string DataType
		{
			get { return GetPropertyValue<string>("DataType", "commonData"); }
			set
			{
				SetPropertyValue<string>("DataType", value);
			}
		}

		/// <summary>
		/// 选择的数据
		/// </summary>
		/// <remarks>
		/// OU，User的数据
		/// </remarks>
		[Description("选择的数据")]
		[Browsable(false)]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("selectedData")]//对应的客户端属性
		public IList SelectedData
		{
			get
			{
				if (this._selectedData != null)
					return this._selectedData;

				if (!DesignMode)
				{
					this._selectedData = new List<object>();
				}

				return this._selectedData;

			}
			set { this._selectedData = value; }
		}

		[Description("单选的数据")]
		[Browsable(false)]
		public object SelectedSingleData
		{
			get
			{
				object result = null;

				if (SelectedData.Count > 0)
					result = SelectedData[0];

				return result;
			}
			set
			{
				SelectedData = new List<object>();
				SelectedData.Add(value);
			}
		}

		/// <summary>
		/// 检查输入的回调方法名称
		/// </summary>
		[ScriptControlProperty]
		[ClientPropertyName("checkInputCallBackMethod")]
		public override string CheckInputCallBackMethod
		{
			get { return "CheckInput"; }
		}

		//public Type TargetDataType
		//{
		//    get;
		//    set;
		//}
		#endregion

		#region 事件
		[Category("Action")]
		public event ValidateInputHandler ValidateInput;
		[Category("Action")]
		public event GetDataSourceHandler GetDataSource;

		/// <summary>
		/// 客户端粘贴时检查数据掩码时的事件
		/// </summary>
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("onCheckDataMask")]
		[Bindable(true), Category("ClientEventsHandler"), Description("客户端粘贴时检查数据掩码时的事件")]
		public string OnClientCheckDataMask
		{
			get
			{
				return GetPropertyValue("OnClientCheckDataMask", string.Empty);
			}
			set
			{
				SetPropertyValue("OnClientCheckDataMask", value);
			}
		}

		#endregion
        
		#region 方法

		protected override void AutoCompleteExtender_GetDataSource(string sPrefix, int iCount, object context, ref IEnumerable result)
		{
			if (GetDataSource != null)
			{
				var dataSource = GetDataSource(sPrefix, context);
				if (dataSource != null)
				{
					var typeName = "";
					foreach (var data in dataSource)
					{
						typeName = data.GetType().AssemblyQualifiedName;
						break;
					}
					result = new object[] { dataSource, typeName };
				}
			}
		}

		/// <summary>
		/// 将ClientState字符串的信息加载到ClientState中
		/// </summary>
		/// <remarks>
		/// 将ClientState字符串的信息加载到ClientState中
		/// </remarks>
		/// <param name="clientState">ClientState字符串</param>
		protected override void LoadClientState(string clientState)
		{
			if (!string.IsNullOrEmpty(clientState))
			{
				IList state = JSONSerializerExecute.Deserialize<IList>(clientState);
				this.SelectedData = state;
			}
		}

		/// <summary>
		/// 将ClenteState中的信息生成ClientState字符串
		/// </summary>
		/// <returns>ClientState字符串</returns>
		protected override string SaveClientState()
		{
			string result = string.Empty;

			if (this.SelectedData != null)
			{
				result = JSONSerializerExecute.SerializeWithType(this.SelectedData);
			}

			return result;
		}

		#endregion

		#region 回调事件
		/// <summary>
		/// 从客户端回掉，验证输入的内容
		/// </summary>
		/// <param name="chkString">通过这个参数传入的信息进行数据校验</param>
		/// <param name="context">用户设置的上下文</param>
		[ScriptControlMethod]
		public virtual object[] CheckInput(string chkString, object context)
		{
			if (ValidateInput != null)
			{
				var data = ValidateInput(chkString, context);
				if (data.Count > 0)
				{
					object[] result = new object[] { data, data[0].GetType().AssemblyQualifiedName };
					return result;
				}
			}

			return null;
		}

		#endregion
	}
}

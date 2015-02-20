#region
// -------------------------------------------------
// Assembly	：	MCS.Web.WebControls
// FileName	：	WordPrint.cs
// Remark	：  Word控件的服务器端代码
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		张曦	    20070815		创建
// 1.0	    徐文卓	    20090112		修改：打印字体颜色问题，、无法带修订的内容。、增加打印时可调整字体颜色和大小功能、打印完毕才显示结果。
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
	/// Word打印的主类
	/// </summary>
	/// <remarks>
	/// Word打印的主类
	/// </remarks>
	[RequiredScript(typeof(ControlBaseScript))]
	[ClientScriptResource("MCS.Web.WebControls.WordPrint", "MCS.Web.WebControls.WordPrint.WordPrint.js")]
	public class WordPrint : ScriptControlBase
	{
		/// <summary>
		/// 回掉的委托定义
		/// </summary>
		/// <remarks>
		/// 回掉的委托定义
		/// </remarks>
		public delegate void OnPrintDelegate(WordPrintDataSourceCollection DataSourceList);

		/// <summary>
		/// 回掉的事件定义
		/// </summary>
		/// <remarks>
		/// 回掉的事件定义
		/// </remarks>
		public event OnPrintDelegate Print;

		/// <summary>
		/// 数据源集合
		/// </summary>
		/// <remarks>
		/// 数据源集合
		/// </remarks>
		private WordPrintDataSourceCollection dataSourceList;

		#region Field
		/// <summary>
		/// 根据ButtonTypes修改空间的HTMLTag
		/// </summary>
		/// <remarks>
		/// 根据ButtonTypes修改空间的HTMLTag
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
		/// 快捷按钮
		/// </summary>
		/// <remarks>
		/// 快捷按钮
		/// </remarks>
		[Description("设置按钮的类型：InputButton，ImageButton，LinkButton")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("accessKey")]//对应的客户端属性
		public override string AccessKey
		{
			get { return GetPropertyValue<string>("AccessKey", String.Empty); }
			set { SetPropertyValue<string>("AccessKey", value); }
		}

		/// <summary>
		/// 按钮的类别：InputButton，ImageButton，LinkButton
		/// </summary>
		/// <remarks>
		/// 按钮的类别：InputButton，ImageButton，LinkButton
		/// </remarks>
		[Description("设置按钮的类型：InputButton，ImageButton，LinkButton")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("type")]//对应的客户端属性
		public ButtonType Type
		{
			get { return GetPropertyValue<ButtonType>("Type", ButtonType.InputButton); }
			set { SetPropertyValue<ButtonType>("Type", value); }
		}

		/// <summary>
		/// 设置按钮上的文本
		/// </summary>
		/// <remarks>
		/// 设置按钮上的文本
		/// </remarks>
		[Description("设置按钮上的文本")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("text")]//对应的客户端属性
		public string Text
		{
			get
			{
				return GetPropertyValue<string>("Text", "打印");
			}
			set
			{
				SetPropertyValue<string>("Text", value);
			}
		}

		/// <summary>
		/// 设置Word文档模板地址
		/// </summary>
		/// <remarks>
		/// 设置Word文档模板地址
		/// </remarks>
		[Description("设置Word文档模板地址")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("templeteUrl")]//对应的客户端属性
		public string TempleteUrl
		{
			get
			{
				string sUrl = GetPropertyValue<string>("TempleteUrl", string.Empty);
				if (!this.DesignMode && sUrl.IndexOf("http") < 0)//如果是一个相对路径
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
		/// 如果是ImageButton，通过这个属性指定按钮图片
		/// </summary>
		/// <remarks>
		/// 如果是ImageButton，通过这个属性指定按钮图片
		/// </remarks>
		[Description("如果是ImageButton，通过这个属性指定按钮图片")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("imageUrl")]//对应的客户端属性
		public string ImageUrl
		{
			get { return GetPropertyValue<string>("ImageUrl", string.Empty); }
			set { SetPropertyValue<string>("ImageUrl", value); }
		}

		/// <summary>
		/// 按钮的Css类名
		/// </summary>
		/// <remarks>
		/// 按钮的Css类名
		/// </remarks>
		[Description("按钮的Css类名")]
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("cssClass")]//对应的客户端属性
		public override string CssClass
		{
			get { return GetPropertyValue<string>("CssClass", string.Empty); }
			set { SetPropertyValue<string>("CssClass", value); }
		}

		/// <summary>
		/// 生成Word文档的数据源集合
		/// </summary>
		/// <remarks>
		/// 生成Word文档的数据源集合
		/// </remarks>
		[Browsable(false)]//不显示在设计时
		[ScriptControlProperty]//设置此属性要输出到客户端
		[ClientPropertyName("dataSourceList")]//对应的客户端属性
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
		/// 当Word文档中创建完毕后，触发的客户端事件
		/// </summary>
		/// <remarks>
		/// 当Word文档中创建完毕后，触发的客户端事件
		/// </remarks>
		[DefaultValue("")]
		[Bindable(true), Category("ClientEventsHandler"), Description("当Word文档创建完毕后，触发的客户端事件")]
		[ClientPropertyName("createWordComplete")]//对应的客户端属性
		[ScriptControlEvent]
		public string OnCreateWordComplete
		{
			get { return GetPropertyValue<string>("OnCreateWordComplete", string.Empty); }
			set { SetPropertyValue<string>("OnCreateWordComplete", value); }
		}

		/// <summary>
		/// 当一个项目创建之前触发的客户端事件
		/// </summary>
		/// <remarks>
		/// 当一个项目创建之前触发的客户端事件
		/// </remarks>
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("beforeDataSourceItemCreate")]
		[Bindable(true), Category("ClientEventsHandler"), Description("当一个项目创建之前触发的客户端事件")]
		public string OnBeforeDataSourceItemCreate
		{
			get { return GetPropertyValue("OnBeforeDataSourceItemCreate", string.Empty); }
			set { SetPropertyValue("OnBeforeDataSourceItemCreate", value); }
		}

		/// <summary>
		/// 当一个项目创建之后触发的客户端事件
		/// </summary>
		/// <remarks>
		/// 当一个项目创建之后触发的客户端事件
		/// </remarks>
		[DefaultValue("")]
		[ScriptControlEvent]
		[ClientPropertyName("dataSourceItemCreated")]
		[Bindable(true), Category("ClientEventsHandler"), Description("当一个项目创建之后触发的客户端事件")]
		public string OnDataSourceItemCreated
		{
			get { return GetPropertyValue("OnDataSourceItemCreated", string.Empty); }
			set { SetPropertyValue("OnDataSourceItemCreated", value); }
		}
		#endregion

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <remarks>
		/// 构造函数
		/// </remarks>
		public WordPrint()
			: base(true)
		{
		}

		/// <summary>
		/// 重写Render,在Render的时候根据按钮类型添加不同的属性
		/// </summary>
		/// <remarks>
		/// 重写Render,在Render的时候根据按钮类型添加不同的属性
		/// </remarks>
		/// <param name="writer">HtmlTextWriter</param>
		protected override void Render(HtmlTextWriter writer)
		{
			this.SetControlText(this.Text);

			base.Render(writer);
		}

		/// <summary>
		/// 处理Data为DataResult
		/// </summary>
		/// <remarks>
		/// 处理Data为DataResult
		/// </remarks>
		/// <param name="dataSourceList">数据源集合</param>
		protected void TranDataSourceList(WordPrintDataSourceCollection dataSourceList)
		{
			for (int i = 0; i < dataSourceList.Count; i++)
			{
				dataSourceList[i].DataResult = WebControlUtility.GetDataSourceResult(this, dataSourceList[i].Data);
			}
		}

		/// <summary>
		/// 根据按钮的类型设置按钮文本及属性
		/// </summary>
		/// <remarks>
		/// 根据按钮的类型设置按钮文本及属性
		/// </remarks>
		/// <param name="sValue">按钮的文本</param>
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
			//不同的HtmlControl需要加不同的属性
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
		/// 处理数据源
		/// </summary>
		/// <remarks>
		/// 处理数据源
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
		/// 用来回调相应onprint获取数据源
		/// </summary>
		/// <remarks>
		/// 用来回调相应onprint获取数据源
		/// </remarks>
		[ScriptControlMethod]
		public WordPrintDataSourceCollection CallBackOnPrintMethod()
		{
			WordPrintDataSourceCollection dataSourceListResult = new WordPrintDataSourceCollection();//返回的值

			if (Print != null)
			{
				Print(dataSourceListResult);
				TranDataSourceList(dataSourceListResult);
			}

			return dataSourceListResult;
		}
	}

	/// <summary>
	/// 打印数据源
	/// </summary>
	/// <remarks>
	/// 打印数据源
	/// </remarks>
	public class WordPrintDataSource
	{
		private string name = string.Empty;
		private int colorArgb = 0;
		private int fontSize = 0;
		private object data = null;
		private IEnumerable dataResult = null;

		/// <summary>
		/// 传说中的构造函数
		/// </summary>
		/// <remarks>
		/// 构造函数
		/// </remarks>
		public WordPrintDataSource()
		{
		}

		/// <summary>
		/// 传说中的构造函数
		/// </summary>
		/// <remarks>
		/// 传说中的构造函数
		/// </remarks>
		/// <param name="name">数据源名称</param>
		/// <param name="data">数据源中的数据</param>
		public WordPrintDataSource(string name, System.Collections.IEnumerable data)
		{
			this.name = name;
			this.data = data;
		}

		/// <summary>
		/// 数据源名称
		/// </summary>
		/// <remarks>
		/// 数据源名称
		/// </remarks>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// 字体颜色
		/// </summary>
		/// <remarks>
		/// 字体颜色
		/// </remarks>
		public Color FontColor
		{
			set { colorArgb = value.R + value.G * 256 + value.B * 65536; }
		}

		/// <summary>
		/// 字体颜色值
		/// </summary>
		/// <remarks>
		/// 字体颜色值
		/// </remarks>
		public int ColorArgb
		{
			get { return colorArgb; }
			set { colorArgb = value; }
		}

		/// <summary>
		/// 字体大小
		/// </summary>
		/// <remarks>
		/// 字体大小
		/// </remarks>
		public int FontSize
		{
			get { return fontSize; }
			set { fontSize = value; }
		}

		/// <summary>
		/// 数据源中的数据
		/// </summary>
		/// <remarks>
		/// 数据源中的数据
		/// </remarks>
		[ScriptIgnore]
		public object Data
		{
			get { return data; }
			set { data = value; }
		}

		/// <summary>
		/// 处理后的数据
		/// </summary>
		/// <remarks>
		/// 处理后的数据
		/// </remarks>
		public IEnumerable DataResult
		{
			get { return this.dataResult; }
			set { this.dataResult = value; }
		}
	}

	/// <summary>
	/// 打印数据源集合
	/// </summary>
	/// <remarks>
	/// 打印数据源集合
	/// </remarks>
	public class WordPrintDataSourceCollection : CollectionBase
	{
		/// <summary>
		/// 返回集合中第index个项目
		/// </summary>
		/// <remarks>返回集合中第index个项目</remarks>
		/// <param name="index">索引</param>
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
		/// 添加一个项目
		/// </summary>
		/// <remarks>
		/// 添加一个项目
		/// </remarks>
		/// <param name="name">名称</param>
		/// <param name="data">数据</param>
		/// <returns>索引</returns>
		public int Add(string name, object data)
		{
			return this.Add(name, data);
		}

		/// <summary>
		/// 添加一个项目
		/// </summary>
		/// <remarks>
		/// 添加一个项目
		/// </remarks>
		/// <param name="value">WordPrintDataSource</param>
		/// <returns>索引</returns>
		public int Add(WordPrintDataSource value)
		{
			int nIndex = List.Add(value);
			if (value.Name == string.Empty) value.Name = "DataSource_" + nIndex.ToString();
			return nIndex;
		}

		/// <summary>
		/// 取得指定项目的索引
		/// </summary>
		/// <remarks>
		/// 取得指定项目的索引
		/// </remarks>
		/// <param name="value">WordPrintDataSource</param>
		/// <returns>索引</returns>
		public int IndexOf(WordPrintDataSource value)
		{
			return (List.IndexOf(value));
		}

		/// <summary>
		/// 向指定的索引插入一个项目
		/// </summary>
		/// <param name="index"></param>
		/// <param name="value"></param>
		public void Insert(int index, WordPrintDataSource value)
		{
			List.Insert(index, value);
		}

		/// <summary>
		/// 移除一个数据源
		/// </summary>
		/// <remarks>移除一个数据源</remarks>
		/// <param name="value">数据源</param>
		public void Remove(WordPrintDataSource value)
		{
			List.Remove(value);
		}

		/// <summary>
		/// 判断是否包含这个数据源
		/// </summary>
		/// <remarks>
		/// 判断是否包含这个数据源
		/// </remarks>
		/// <param name="value">数据源</param>
		/// <returns>是否包含</returns>
		public bool Contains(WordPrintDataSource value)
		{
			return (List.Contains(value));
		}
	}
}

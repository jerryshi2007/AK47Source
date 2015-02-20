using System;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using MCS.Library.Core;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Web.UI;
using MCS.Library.Data.Mapping;
using MCS.Library.Data.DataObjects;
using MCS.Library.SOA.DataObjects.Workflow;

namespace MCS.Web.WebControls
{
	/// <summary>
	/// 客户端绑定的数据类型
	/// </summary>
	public enum ClientBindingDataType
	{
		None = 0,
		String = 1,
		Number = 2,
		Float = 3,
		DateTime = 4,
		Object = 5
	}

	/// <summary>
	/// 去掉文本空格方式
	/// </summary>
	public enum TrimBlankType
	{
		/// <summary>
		/// 去掉左右两端空格
		/// </summary>
		/// 
		RightAndLeft = 0,

		/// <summary>
		/// 右端空格
		/// </summary>
		Right = 1,

		/// <summary>
		/// 去掉左端空格
		/// </summary>
		/// 
		Left = 2,

		/// <summary>
		/// 不去掉
		/// </summary>
		None = 3,

		/// <summary>
		/// 去掉所有空格
		/// </summary>
		ALL = 4
	}

	/// <summary>
	/// 绑定的方向
	/// </summary>
	[Flags]
	public enum BindingDirection
	{
		/// <summary>
		/// 不进行绑定
		/// </summary>
		None = 0,

		/// <summary>
		/// 从控件绑定到数据
		/// </summary>
		ControlToData = 1,

		/// <summary>
		/// 从数据绑定到控件
		/// </summary>
		DataToControl = 2,

		/// <summary>
		/// 双向绑定
		/// </summary>
		Both = 3
	}

    //[Flags]
    //public enum ProcessParameterEvalMode
    //{
    //    CurrentProcess = 1,
    //    ApprovalRootProcess = 2,
    //    RootProcess = 4,
    //    SameResourceRootProcess = 8
    //}

	[Serializable]
	[ParseChildren(true, "SubBindings")]
	public class DataBindingItem
	{
		private string controlID = string.Empty;
		private string controlPropertyName = string.Empty;
		private string clientPropName = "value";
		private string clientSetPropName = "value";
		private bool clientIsHtmlElement = true;
		private bool isValidate = true;
		private bool isValidateOnBlur = true;
		private bool autoFormatOnBlur = true;

		private string dataPropertyName = string.Empty;
		private string clientDataPropertyName = string.Empty;
		private BindingDirection direction = BindingDirection.Both;
		private string format = string.Empty;
		private ClientBindingDataType clientDataType = ClientBindingDataType.String;

		private bool enumAutoBinding = true;
		private EnumUsageTypes enumUsage = EnumUsageTypes.UseEnumValue;
		private TrimBlankType itemTrimBlankType = TrimBlankType.RightAndLeft;

		private bool collectToProcessParameters = false;
		private string processParameterName = string.Empty;

		private bool formatDefaultValueToEmpty = true;

		private DataBindingItemCollection subBindings = null;
		private ProcessParameterEvalMode processParameterEvalMode = ProcessParameterEvalMode.CurrentProcess;
		public DataBindingItem()
		{
		}

		/// <summary>
		/// 客户端绑定的数据类型
		/// </summary>
		[DefaultValue(ClientBindingDataType.String)]
		public ClientBindingDataType ClientDataType
		{
			get { return this.clientDataType; }
			set { this.clientDataType = value; }
		}

		/// <summary>
		/// 是否在客户端onchange的时候自动格式化
		/// </summary>
		[DefaultValue(true)]
		public bool AutoFormatOnBlur
		{
			get { return this.autoFormatOnBlur; }
			set { this.autoFormatOnBlur = value; }
		}

		[DefaultValue("")]
		public string ControlID
		{
			get { return this.controlID; }
			set { this.controlID = value; }
		}

		[DefaultValue("")]
		public string ControlPropertyName
		{
			get { return this.controlPropertyName; }
			set { this.controlPropertyName = value; }
		}

		[DefaultValue("")]
		public string ClientSetPropName
		{
			get { return this.clientSetPropName; }
			set { this.clientSetPropName = value; }
		}

		/// <summary>
		/// 客户端绑定数据属性
		/// 默认是value.
		/// </summary>
		[DefaultValue("value")]
		public string ClientPropName
		{
			get { return this.clientPropName; }
			set { this.clientPropName = value; }
		}

		[DefaultValue(true)]
		public bool ClientIsHtmlElement
		{
			get { return this.clientIsHtmlElement; }
			set { this.clientIsHtmlElement = value; }
		}

		/// <summary>
		/// 是否校验该属性
		/// 默认是校验
		/// </summary>
		[DefaultValue(true)]
		public bool IsValidate
		{
			get { return isValidate; }
			set { isValidate = value; }
		}

		/// <summary>
		/// 焦点移开是否校验
		/// </summary>
		[DefaultValue(true)]
		public bool IsValidateOnBlur
		{
			get { return isValidateOnBlur; }
			set { isValidateOnBlur = value; }
		}

		/// <summary>
		/// 对象属性名称
		/// </summary>
		[DefaultValue("")]
		public string DataPropertyName
		{
			get { return this.dataPropertyName; }
			set { this.dataPropertyName = value; }
		}

		/// <summary>
		/// 客户端收集数据时的对象属性名称
		/// </summary>
		public string ClientDataPropertyName
		{
			get { return this.clientDataPropertyName; }
			set { this.clientDataPropertyName = value; }
		}

		/// <summary>
		/// 绑定方向
		/// </summary>
		[DefaultValue(BindingDirection.Both)]
		public BindingDirection Direction
		{
			get { return this.direction; }
			set { this.direction = value; }
		}

		/// <summary>
		/// 格式化数据表达式
		/// </summary>
		[DefaultValue("")]
		public string Format
		{
			get { return this.format; }
			set { this.format = value; }
		}

		/// <summary>
		/// 是否自动绑定枚举
		/// </summary>
		[DefaultValue(true)]
		public bool EnumAutoBinding
		{
			get { return this.enumAutoBinding; }
			set { this.enumAutoBinding = value; }
		}

		/// <summary>
		/// 枚举使用类型：值还是文本
		/// </summary>
		[DefaultValue(EnumUsageTypes.UseEnumValue)]
		public EnumUsageTypes EnumUsage
		{
			get { return this.enumUsage; }
			set { this.enumUsage = value; }
		}

		/// <summary>
		/// 校验时的组号，必须>=0
		/// </summary>
		[DefaultValue(0)]
		public int ValidationGroup
		{
			get;
			set;
		}

		[DefaultValue(true)]
		public bool FormatDefaultValueToEmpty
		{
			get
			{
				return formatDefaultValueToEmpty;
			}
			set
			{
				this.formatDefaultValueToEmpty = value;
			}

		}

		[Browsable(false)]
		[MergableProperty(false)]
		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		[Category("Data")]
		public DataBindingItemCollection SubBindings
		{
			get
			{
				if (this.subBindings == null)
					this.subBindings = new DataBindingItemCollection();

				return this.subBindings;
			}
		}

		/// <summary>
		/// 如果需要收集到流程的应用参数中，对应的参数名称，如果该值为空，则使用DataPropertyName
		/// </summary>
		[Category("Data")]
		[DefaultValue("")]
		public string ProcessParameterName
		{
			get
			{
				return this.processParameterName;
			}
			set
			{
				this.processParameterName = value;
			}
		}

		/// <summary>
		/// 收集数据的时候，是否收集到流程的ApplicationRuntimeParameters中
		/// </summary>
		[Category("Data")]
		[DefaultValue(false)]
		public bool CollectToProcessParameters
		{
			get
			{
				return this.collectToProcessParameters;
			}
			set
			{
				this.collectToProcessParameters = value;
			}
		}

		/// <summary>
		/// 如果CollectToProcessParameters设置为True时，使用此属性决定将收集的值赋给哪个流程实例
		/// </summary>
		[Category("Data")]
		[DefaultValue(ProcessParameterEvalMode.CurrentProcess)]
		public ProcessParameterEvalMode ProcessParameterEvalMode
		{
			get
			{
				return this.processParameterEvalMode;
			}
			set
			{
				this.processParameterEvalMode = value;
			}
		}

		[Category("Data")]
		[DefaultValue(TrimBlankType.RightAndLeft)]
		public TrimBlankType ItemTrimBlankType
		{
			get
			{
				return this.itemTrimBlankType;
			}
			set
			{
				this.itemTrimBlankType = value;
			}
		}
	}

	/// <summary>
	/// 数据绑定的集合类
	/// </summary>
	//[Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
	//[PersistenceMode(PersistenceMode.InnerProperty)]
	[Serializable]
	public class DataBindingItemCollection : DataObjectCollectionBase<DataBindingItem>
	{
		public void Add(DataBindingItem item)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(item != null, "item");

			InnerAdd(item);
		}

		public DataBindingItem this[int index]
		{
			get
			{
				return (DataBindingItem)List[index];
			}
			set
			{
				List[index] = value;
			}
		}
	}
}

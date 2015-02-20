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
	/// �ͻ��˰󶨵���������
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
	/// ȥ���ı��ո�ʽ
	/// </summary>
	public enum TrimBlankType
	{
		/// <summary>
		/// ȥ���������˿ո�
		/// </summary>
		/// 
		RightAndLeft = 0,

		/// <summary>
		/// �Ҷ˿ո�
		/// </summary>
		Right = 1,

		/// <summary>
		/// ȥ����˿ո�
		/// </summary>
		/// 
		Left = 2,

		/// <summary>
		/// ��ȥ��
		/// </summary>
		None = 3,

		/// <summary>
		/// ȥ�����пո�
		/// </summary>
		ALL = 4
	}

	/// <summary>
	/// �󶨵ķ���
	/// </summary>
	[Flags]
	public enum BindingDirection
	{
		/// <summary>
		/// �����а�
		/// </summary>
		None = 0,

		/// <summary>
		/// �ӿؼ��󶨵�����
		/// </summary>
		ControlToData = 1,

		/// <summary>
		/// �����ݰ󶨵��ؼ�
		/// </summary>
		DataToControl = 2,

		/// <summary>
		/// ˫���
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
		/// �ͻ��˰󶨵���������
		/// </summary>
		[DefaultValue(ClientBindingDataType.String)]
		public ClientBindingDataType ClientDataType
		{
			get { return this.clientDataType; }
			set { this.clientDataType = value; }
		}

		/// <summary>
		/// �Ƿ��ڿͻ���onchange��ʱ���Զ���ʽ��
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
		/// �ͻ��˰���������
		/// Ĭ����value.
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
		/// �Ƿ�У�������
		/// Ĭ����У��
		/// </summary>
		[DefaultValue(true)]
		public bool IsValidate
		{
			get { return isValidate; }
			set { isValidate = value; }
		}

		/// <summary>
		/// �����ƿ��Ƿ�У��
		/// </summary>
		[DefaultValue(true)]
		public bool IsValidateOnBlur
		{
			get { return isValidateOnBlur; }
			set { isValidateOnBlur = value; }
		}

		/// <summary>
		/// ������������
		/// </summary>
		[DefaultValue("")]
		public string DataPropertyName
		{
			get { return this.dataPropertyName; }
			set { this.dataPropertyName = value; }
		}

		/// <summary>
		/// �ͻ����ռ�����ʱ�Ķ�����������
		/// </summary>
		public string ClientDataPropertyName
		{
			get { return this.clientDataPropertyName; }
			set { this.clientDataPropertyName = value; }
		}

		/// <summary>
		/// �󶨷���
		/// </summary>
		[DefaultValue(BindingDirection.Both)]
		public BindingDirection Direction
		{
			get { return this.direction; }
			set { this.direction = value; }
		}

		/// <summary>
		/// ��ʽ�����ݱ��ʽ
		/// </summary>
		[DefaultValue("")]
		public string Format
		{
			get { return this.format; }
			set { this.format = value; }
		}

		/// <summary>
		/// �Ƿ��Զ���ö��
		/// </summary>
		[DefaultValue(true)]
		public bool EnumAutoBinding
		{
			get { return this.enumAutoBinding; }
			set { this.enumAutoBinding = value; }
		}

		/// <summary>
		/// ö��ʹ�����ͣ�ֵ�����ı�
		/// </summary>
		[DefaultValue(EnumUsageTypes.UseEnumValue)]
		public EnumUsageTypes EnumUsage
		{
			get { return this.enumUsage; }
			set { this.enumUsage = value; }
		}

		/// <summary>
		/// У��ʱ����ţ�����>=0
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
		/// �����Ҫ�ռ������̵�Ӧ�ò����У���Ӧ�Ĳ������ƣ������ֵΪ�գ���ʹ��DataPropertyName
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
		/// �ռ����ݵ�ʱ���Ƿ��ռ������̵�ApplicationRuntimeParameters��
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
		/// ���CollectToProcessParameters����ΪTrueʱ��ʹ�ô����Ծ������ռ���ֵ�����ĸ�����ʵ��
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
	/// ���ݰ󶨵ļ�����
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

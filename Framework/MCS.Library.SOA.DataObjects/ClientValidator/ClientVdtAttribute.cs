using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Validation;
using System.Reflection;

namespace MCS.Library.SOA.DataObjects
{
	///// <summary>
	///// 验证类型
	///// 区间或者表达式
	///// </summary>
	//public enum ValidateType : int
	//{
	//    Range,
	//    Expession,
	//    Empty
	//}

	///// <summary>
	///// 验证数据类型
	///// 当验证是区间类型的时候数据起用。
	///// 
	///// </summary>
	//public enum ValidteDataType : int
	//{
	//    Int = 0,
	//    String = 1,
	//    DateTime = 2,
	//    Float = 3,
	//    Length = 4,
	//    StringByteLength = 5,
	//    Object = 6,
	//    Enum = 7
	//}

	/// <summary>
	/// 客户端验证数据
	/// </summary>
	[Serializable]
	public class ClientVdtAttribute
	{
		//private string expression = string.Empty;
		private string messageTemplate = string.Empty;
		//private string tag = string.Empty;
		//private ValidateType vType = ValidateType.Expession;
		//private ValidteDataType dType = ValidteDataType.String;
		//private string lowerBound = string.Empty;
		//private string upperBound = string.Empty;
		//private string key = string.Empty;

		//add by fenglilei
		private string clientValidateMethodName = string.Empty;
		private Dictionary<string, object> additionalData = null;

		// add by yandezhi
		public ClientVdtAttribute(PropertyValidatorDescriptor propDesp)
		{
			this.messageTemplate = propDesp.MessageTemplate;
			if (propDesp.GetValidator() is IClientValidatable)
			{
				this.clientValidateMethodName =
					((IClientValidatable)propDesp.GetValidator()).ClientValidateMethodName;
			}
		}

		public ClientVdtAttribute(ValidatorAttribute validatorAttribute, PropertyInfo pi)
		{
			//this.tag = validatorAttribute.Tag;
			this.messageTemplate = validatorAttribute.MessageTemplate;

			//add by Fenglilei,2011/11/7
			//modified by Fenglilei,2012/2/27
			if (validatorAttribute.Validator != null && validatorAttribute.Validator is IClientValidatable)
			{
				this.clientValidateMethodName =
					((IClientValidatable)validatorAttribute.Validator).ClientValidateMethodName;
			}

			if (validatorAttribute is IntegerRangeValidatorAttribute)
			{
				//IntegerRangeValidatorAttribute currval = validatorAttribute as IntegerRangeValidatorAttribute;
				//this.vType = ValidateType.Range;
				//this.lowerBound = currval.LowerBound.ToString();
				//this.upperBound = currval.UpperBound.ToString();
				//this.DType = ValidteDataType.Int;
			}
			else if (validatorAttribute is RegexValidatorAttribute)
			{
				//RegexValidatorAttribute currval = validatorAttribute as RegexValidatorAttribute;
				//this.vType = ValidateType.Expession;
				//this.expression = currval.Pattern;
			}
			else if (validatorAttribute is StringLengthValidatorAttribute)
			{
				//StringLengthValidatorAttribute currval = validatorAttribute as StringLengthValidatorAttribute;
				//this.vType = ValidateType.Range;
				//this.lowerBound = currval.LowerBound.ToString();
				//this.upperBound = currval.UpperBound.ToString();
				//this.DType = ValidteDataType.Length;
			}
			else if (validatorAttribute is DateTimeRangeValidatorAttribute)
			{
				//DateTimeRangeValidatorAttribute currval = validatorAttribute as DateTimeRangeValidatorAttribute;
				//this.vType = ValidateType.Range;
				//this.lowerBound = currval.LowerBound.ToString("yyyy-MM-dd HH:mm:ss");
				//this.upperBound = currval.UpperBound.ToString("yyyy-MM-dd HH:mm:ss");
				//this.DType = ValidteDataType.DateTime;
			}
			else if (validatorAttribute is DoubleRangeValidatorAttribute)
			{
				//DoubleRangeValidatorAttribute currval = validatorAttribute as DoubleRangeValidatorAttribute;
				//this.vType = ValidateType.Range;
				//this.lowerBound = currval.LowerBound.ToString();
				//this.upperBound = currval.UpperBound.ToString();
				//this.DType = ValidteDataType.Float;
			}
			else if (validatorAttribute is FloatRangeValidatorAttribute)
			{
				//FloatRangeValidatorAttribute currval = validatorAttribute as FloatRangeValidatorAttribute;
				//this.vType = ValidateType.Range;
				//this.lowerBound = currval.LowerBound.ToString();
				//this.upperBound = currval.UpperBound.ToString();
				//this.DType = ValidteDataType.Float;
			}
			else if (validatorAttribute is DecimalRangeValidatorAttribute)
			{
				//DecimalRangeValidatorAttribute currval = validatorAttribute as DecimalRangeValidatorAttribute;
				//this.vType = ValidateType.Range;
				//this.lowerBound = currval.LowerBound.ToString();
				//this.upperBound = currval.UpperBound.ToString();
				//this.DType = ValidteDataType.Float;
			}
			else if (validatorAttribute is StringByteLengthValidatorAttribute)
			{
				//StringByteLengthValidatorAttribute currval = validatorAttribute as StringByteLengthValidatorAttribute;
				//this.vType = ValidateType.Range;
				//this.lowerBound = currval.LowerBound.ToString();
				//this.upperBound = currval.UpperBound.ToString();
				//this.DType = ValidteDataType.StringByteLength;
			}
			else if (validatorAttribute is StringEmptyValidatorAttribute)
			{
				//this.vType = ValidateType.Empty;
				//this.DType = ValidteDataType.String;
			}
			else if (validatorAttribute is DateTimeEmptyValidatorAttribute)
			{
				//this.vType = ValidateType.Empty;
				//this.DType = ValidteDataType.DateTime;
			}
			else if (validatorAttribute is ObjectNullValidatorAttribute)
			{
				//this.vType = ValidateType.Empty;
				//this.DType = ValidteDataType.Object;
			}
			else if (validatorAttribute is EnumDefaultValueValidatorAttribute)
			{
				//this.vType = ValidateType.Empty;
				//this.DType = ValidteDataType.Enum;

				//if (pi.PropertyType.IsEnum)
				//{
				//    object defaultValue = Activator.CreateInstance(pi.PropertyType);

				//    this.tag = ((int)defaultValue).ToString();
				//}
			}
		}

		/// <summary>
		/// 客户端校验函数名称
		/// </summary>
		public string ClientValidateMethodName
		{
			get { return this.clientValidateMethodName; }
			set { this.clientValidateMethodName = value; }
		}

		/// <summary>
		/// 校验上的附加信息，比如正则表达式，范围值，等等
		/// </summary>
		public Dictionary<string, object> AdditionalData
		{
			get { return this.additionalData; }
			set { this.additionalData = value; }
		}

		///// <summary>
		///// 验证表达式
		///// </summary>
		//public string Expression
		//{
		//    get { return this.expression; }
		//    set { this.expression = value; }
		//}

		/// <summary>
		/// 出错信息
		/// </summary>
		public string MessageTemplate
		{
			get { return this.messageTemplate; }
			set { this.messageTemplate = value; }
		}

		///// <summary>
		///// 标记
		///// </summary>
		//public string Tag
		//{
		//    get { return this.tag; }
		//    set { this.tag = value; }
		//}


		///// <summary>
		///// 验证类型
		///// </summary>
		//public ValidateType VType
		//{
		//    get { return this.vType; }
		//    set
		//    {
		//        this.vType = value;
		//    }
		//}

		///// <summary>
		///// 验证数据类型
		///// </summary>
		//public ValidteDataType DType
		//{
		//    get { return this.dType; }
		//    set
		//    {
		//        this.dType = value;
		//    }
		//}

		///// <summary>
		///// 区间下限
		///// </summary>
		//public string LowerBound
		//{
		//    get { return this.lowerBound; }
		//    set
		//    {
		//        this.lowerBound = value;
		//    }
		//}

		///// <summary>
		///// 区间上限
		///// </summary>
		//public string UpperBound
		//{
		//    get { return this.upperBound; }
		//    set
		//    {
		//        this.upperBound = value;
		//    }
		//}

		///// <summary>
		///// 关键字
		///// </summary>
		//public string Key
		//{
		//    get { return this.key; }
		//    set
		//    {
		//        this.key = value;
		//    }
		//}
	}

	/// <summary>
	/// 客户端验证数据包
	/// </summary>
	[Serializable]
	public class ClientVdtData
	{
		private string controlID = string.Empty;
		private bool isAnd = true;
		private string validateProp = "value";
		private bool clientIsHtmlElement = true;
		private bool isValidateOnblur = true;
		private bool isOnlyNum = false;
		private bool isFloat = false;
		private bool autoFormatOnBlur = true;
		private bool isValidateOnSubmit = true;
		private string formatString = string.Empty;
		private List<ClientVdtAttribute> cvtrList = new List<ClientVdtAttribute>();

		/// <summary>
		/// 客户端验证数据集合
		/// </summary>
		public List<ClientVdtAttribute> CvtList
		{
			get { return this.cvtrList; }
			set { this.cvtrList = value; }
		}

		/// <summary>
		/// 控件ID（客户端）
		/// </summary>
		public string ControlID
		{
			get { return this.controlID; }
			set { this.controlID = value; }
		}

		/// <summary>
		/// 客户端是不是Html Element
		/// </summary>
		public bool ClientIsHtmlElement
		{
			get { return this.clientIsHtmlElement; }
			set { this.clientIsHtmlElement = value; }
		}

		public bool AutoFormatOnBlur
		{
			get { return this.autoFormatOnBlur; }
			set { this.autoFormatOnBlur = value; }
		}

		/// <summary>
		/// 是否是与的关系
		/// </summary>
		public bool IsAnd
		{
			get { return this.isAnd; }
			set { this.isAnd = value; }
		}

		/// <summary>
		/// 客户端验证属性名称
		/// </summary>
		public string ValidateProp
		{
			get { return this.validateProp; }
			set
			{
				this.validateProp = value;
			}
		}

		/// <summary>
		/// 是否在失去焦点的时候验证
		/// </summary>
		public bool IsValidateOnBlur
		{
			get { return this.isValidateOnblur; }
			set { this.isValidateOnblur = value; }
		}

		/// <summary>
		/// 是否提交是校验
		/// </summary>
		public bool IsValidateOnSubmit
		{
			get
			{
				return this.isValidateOnSubmit;
			}
			set
			{
				this.isValidateOnSubmit = value;
			}
		}

		/// <summary>
		/// 是否只允许输入数字
		/// </summary>
		public bool IsOnlyNum
		{
			get { return this.isOnlyNum; }
			set { this.isOnlyNum = value; }
		}

		/// <summary>
		/// 是否允许输入浮点型数据
		/// </summary>
		public bool IsFloat
		{
			get { return this.isFloat; }
			set { isFloat = value; }
		}


		/// <summary>
		/// 格式化输入
		/// </summary>
		public string FormatString
		{
			get { return this.formatString; }
			set { this.formatString = value; }
		}

		/// <summary>
		/// 校验时的组号，必须>=0
		/// </summary>
		public int ValidationGroup
		{
			get;
			set;
		}
	}
}

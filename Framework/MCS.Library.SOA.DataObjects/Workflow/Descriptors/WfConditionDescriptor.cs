using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using MCS.Library.Core;
using MCS.Library.Expression;
using MCS.Library.Globalization;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	[XElementSerializable]
	public class WfConditionDescriptor : ISimpleXmlSerializer
	{
		IWfKeyedDescriptor _Owner = null;

		private string _Expression = string.Empty;

		public WfConditionDescriptor()
		{
		}

		public WfConditionDescriptor(IWfKeyedDescriptor owner)
		{
			this._Owner = owner;
		}

		/// <summary>
		/// 表达式的拥有者
		/// </summary>
		public IWfKeyedDescriptor Owner
		{
			get { return this._Owner; }
			set { this._Owner = value; }
		}

		/// <summary>
		/// 是否条件为空
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty(this._Expression);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string Expression
		{
			get
			{
				return this._Expression;
			}
			set
			{
				this._Expression = value;
			}
		}

		public WfConditionDescriptor Clone()
		{
			WfConditionDescriptor condition = new WfConditionDescriptor(this.Owner);

			condition.Expression = this.Expression;

			return condition;
		}

		public bool Evaluate(CalculateUserFunction cuf)
		{
			bool result = true;

			if (string.IsNullOrEmpty(this._Expression) == false)
			{
				try
				{
					result = (bool)ExpressionParser.Calculate(this._Expression,
						cuf,
						this);
				}
				catch (System.Exception ex)
				{
					ExceptionHelper.FalseThrow(false,
						Translator.Translate(WfHelper.CultureCategory, "表达式解析错误：({0})\n{1}", this._Expression, ex.Message));
				}
			}

			return result;
		}

		public object EvaluateObject(CalculateUserFunction cuf)
		{
			object result = true;

			if (string.IsNullOrEmpty(this._Expression) == false)
			{
				try
				{
					result = ExpressionParser.Calculate(
						this._Expression,
						cuf,
						this);
				}
				catch (System.Exception ex)
				{
					ExceptionHelper.FalseThrow(false,
						Translator.Translate(WfHelper.CultureCategory, "表达式解析错误：({0})\n{1}", this._Expression, ex.Message));
				}
			}

			return result;
		}

		public void SyncPropertiesToFields(PropertyValue property)
		{
			if (property != null)
			{
				this.Expression = string.Empty;

				if (property.StringValue.IsNotEmpty())
				{
					WfConditionDescriptor condition = JSONSerializerExecute.Deserialize<WfConditionDescriptor>(property.StringValue);

					this.Expression = condition.Expression;
				}
			}
		}
		#region ISimpleXmlSerializer Members

		void ISimpleXmlSerializer.ToXElement(XElement element, string refNodeName)
		{
			element.NullCheck("element");

			if (refNodeName.IsNotEmpty())
				element = element.AddChildElement(refNodeName);

			element.SetAttributeValue("expression", this.Expression);
		}

		#endregion
	}

	[Serializable]
	public class WfConditionEvaluationException : WfEvaluationExceptionBase
	{
		/// <summary>
		/// 
		/// </summary>
		public WfConditionEvaluationException()
			: base()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public WfConditionEvaluationException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="condition"></param>
		public WfConditionEvaluationException(string message, WfConditionDescriptor condition)
			: base(message, condition)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public WfConditionEvaluationException(string message, System.Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		/// <param name="condition"></param>
		public WfConditionEvaluationException(string message, System.Exception innerException, WfConditionDescriptor condition)
			: base(message, innerException, condition)
		{
		}
	}
}

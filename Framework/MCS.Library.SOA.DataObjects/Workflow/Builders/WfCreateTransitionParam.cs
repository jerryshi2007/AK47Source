using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow.Builders
{
	/// <summary>
	/// 创建线的描述信息
	/// </summary>
	[Serializable]
	public class WfCreateTransitionParam
	{
		private Dictionary<string, object> _Parameters = null;

		public WfCreateTransitionParam()
		{
		}

		public WfCreateTransitionParam(IWfTransitionDescriptor transition)
		{
			transition.NullCheck("transition");

			this._Parameters = transition.Properties.ToDictionary();

			if (transition.Condition.IsEmpty == false)
				this.Parameters.Add("Condition", transition.Condition.Expression);

			if (transition.Variables.Count > 0)
			{
				Dictionary<string, object>[] variables = new Dictionary<string, object>[transition.Variables.Count];

				for (int i = 0; i < variables.Length; i++)
				{
					WfVariableDescriptor variable = transition.Variables[i];
					variables[i] = variable.Properties.ToDictionary();

					variables[i]["OriginalType"] = variable.OriginalType;
					variables[i]["OriginalValue"] = variable.OriginalValue;
				}

				this.Parameters.Add("Variables", variables);
			}
		}

		public WfCreateTransitionParam(Dictionary<string, object> dictionary)
		{
			this._Parameters = new Dictionary<string, object>(dictionary);
		}

		/// <summary>
		/// 下一步的活动编码
		/// </summary>
		public string ToActivity
		{
			get;
			set;
		}

		/// <summary>
		/// 线参数集合
		/// </summary>
		public Dictionary<string, object> Parameters
		{
			get
			{
				if (this._Parameters == null)
					this._Parameters = new Dictionary<string, object>();

				return this._Parameters;
			}
			internal set
			{
				this._Parameters = value;
			}
		}

		/// <summary>
		/// 创建线，并且连接两个活动。线的起点活动由fromActDesp指定，而终点活动由toActDesp指定
		/// </summary>
		/// <param name="fromActDesp">线的起点活动</param>
		/// <param name="toActDesp">线的终点活动</param>
		/// <returns></returns>
		public IWfTransitionDescriptor CreateTransitionAndConnectActivities(IWfActivityDescriptor fromActDesp, IWfActivityDescriptor toActDesp)
		{
			fromActDesp.NullCheck("fromActDesp");
			toActDesp.NullCheck("toActDesp");

			IWfProcessDescriptor processDesp = fromActDesp.Process;

			IWfTransitionDescriptor transition = CreateTransition(processDesp);

			transition.ConnectActivities(fromActDesp, toActDesp);

			return transition;
		}

		/// <summary>
		/// 创建新的线。使用默认的Key或者是Parameter集合中Key属性
		/// </summary>
		/// <returns></returns>
		public IWfTransitionDescriptor CreateTransition()
		{
			return CreateTransition(null);
		}

		/// <summary>
		/// 创建新的线，如果processDesp不为null，则使用processDesp生成的不重复的Key
		/// </summary>
		/// <param name="processDesp"></param>
		/// <returns></returns>
		public IWfTransitionDescriptor CreateTransition(IWfProcessDescriptor processDesp)
		{
			string newKey = "TempTransitionKey";

			if (processDesp != null)
				newKey = processDesp.FindNotUsedTransitionKey();
			else
				newKey = DictionaryHelper.GetValue(this.Parameters, "Key", string.Empty);

			WfForwardTransitionDescriptor transition = new WfForwardTransitionDescriptor(newKey);

			string expression = DictionaryHelper.GetValue(this.Parameters, "Condition", string.Empty);

			if (expression.IsNotEmpty())
				transition.Condition.Expression = expression;

			MergeProperties(transition.Properties, this.Parameters);

			if (this.Parameters.ContainsKey("Variables"))
				CreateVariables(transition.Variables, (object[])this.Parameters["Variables"]);

			return transition;
		}

		private static void CreateVariables(WfVariableDescriptorCollection variables, object[] variableDicts)
		{
			foreach (Dictionary<string, object> variableDict in variableDicts)
			{
				WfVariableDescriptor variable = new WfVariableDescriptor(DictionaryHelper.GetValue(variableDict, "Key", string.Empty));

				MergeProperties(variable.Properties, variableDict);

				variable.OriginalType = DictionaryHelper.GetValue(variableDict, "OriginalType", DataType.String);
				variable.OriginalValue = DictionaryHelper.GetValue(variableDict, "OriginalValue", string.Empty);

				variables.Add(variable);
			}
		}

		private static void MergeProperties(PropertyValueCollection properties, Dictionary<string, object> parameters)
		{
			foreach (KeyValuePair<string, object> kp in parameters)
			{
				if (string.Compare(kp.Key, "Key", true) != 0)
				{
					if (properties.ContainsKey(kp.Key))
					{
						PropertyValue pv = properties[kp.Key];

						pv.StringValue = kp.Value.ToString();
					}
				}
			}
		}
	}

	/// <summary>
	/// 线的描述信息集合
	/// </summary>
	[Serializable]
	public class WfCreateTransitionParamCollection : EditableDataObjectCollectionBase<WfCreateTransitionParam>
	{
		public WfCreateTransitionParamCollection()
		{
		}

		/// <summary>
		/// 根据JSON串进行初始化
		/// </summary>
		/// <param name="json"></param>
		public WfCreateTransitionParamCollection(string json)
		{
			FromJson(json);
		}

		/// <summary>
		/// 从Json串进行初始化
		/// </summary>
		/// <param name="json"></param>
		public void FromJson(string json)
		{
			this.Clear();

			object[] dicts = JSONSerializerExecute.Deserialize<object[]>(json);

			foreach (object obj in dicts)
			{
				WfCreateTransitionParam transitionParam = null;

				if (obj is IWfTransitionDescriptor)
				{
					transitionParam = new WfCreateTransitionParam((IWfTransitionDescriptor)obj);
				}
				else
				{
					if (obj is Dictionary<string, object>)
						transitionParam = new WfCreateTransitionParam((Dictionary<string, object>)obj);
				}

				if (transitionParam != null)
					this.Add(transitionParam);
			}
		}
	}
}

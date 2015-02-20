using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Workflow
{
	/// <summary>
	/// 
	/// </summary>
	public interface IWfTransitionDescriptor : IWfKeyedDescriptor, IComparable<IWfTransitionDescriptor>
	{
		/// <summary>
		/// 
		/// </summary>
		int Priority
		{
			get;
		}

		bool AffectProcessReturnValue
		{
			get;
			set;
		}

		bool AffectedProcessReturnValue
		{
			get;
			set;
		}

		/// <summary>
		/// 
		/// </summary>
		IWfActivityDescriptor ToActivity
		{
			get;
		}

		/// <summary>
		/// 
		/// </summary>
		IWfActivityDescriptor FromActivity
		{
			get;
		}

		/// <summary>
		/// 
		/// </summary>
		WfVariableDescriptorCollection Variables
		{
			get;
		}

		/// <summary>
		/// 线上条件
		/// </summary>
		WfConditionDescriptor Condition
		{
			get;
		}

		/// <summary>
		/// 该路径是够有效
		/// </summary>
		/// <returns></returns>
		bool CanTransit();

		/// <summary>
		/// 符合线上的条件
		/// </summary>
		/// <returns></returns>
		bool ConditionMatched();

		/// <summary>
		/// 默认选择
		/// </summary>
		bool DefaultSelect { get; set; }

		/// <summary>
		/// 是否是退回线
		/// </summary>
		bool IsBackward { get; set; }

		/// <summary>
		/// 是否是动态活动的进线或出线
		/// </summary>
		bool IsDynamicActivityTransition { get; }

		/// <summary>
		/// 是否由动态模板产生的线
		/// </summary>
		bool GeneratedByTemplate { get; }

		/// <summary>
		/// 生成此活动的动态模板活动的Key
		/// </summary>
		string TemplateKey
		{
			get;
		}

		/// <summary>
		/// 设置线的FromActivity和ToActivity的相关属性，并且影响fromActivity的ToTransitions以及toActivity的FromTransition的集合
		/// </summary>
		/// <param name="fromActivity"></param>
		/// <param name="toActivity"></param>
		void ConnectActivities(IWfActivityDescriptor fromActivity, IWfActivityDescriptor toActivity);
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IWfBackwardTransitionDescriptor : IWfTransitionDescriptor
	{
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IWfForwardTransitionDescriptor : IWfTransitionDescriptor
	{
	}
}

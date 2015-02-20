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
		/// ��������
		/// </summary>
		WfConditionDescriptor Condition
		{
			get;
		}

		/// <summary>
		/// ��·���ǹ���Ч
		/// </summary>
		/// <returns></returns>
		bool CanTransit();

		/// <summary>
		/// �������ϵ�����
		/// </summary>
		/// <returns></returns>
		bool ConditionMatched();

		/// <summary>
		/// Ĭ��ѡ��
		/// </summary>
		bool DefaultSelect { get; set; }

		/// <summary>
		/// �Ƿ����˻���
		/// </summary>
		bool IsBackward { get; set; }

		/// <summary>
		/// �Ƿ��Ƕ�̬��Ľ��߻����
		/// </summary>
		bool IsDynamicActivityTransition { get; }

		/// <summary>
		/// �Ƿ��ɶ�̬ģ���������
		/// </summary>
		bool GeneratedByTemplate { get; }

		/// <summary>
		/// ���ɴ˻�Ķ�̬ģ����Key
		/// </summary>
		string TemplateKey
		{
			get;
		}

		/// <summary>
		/// �����ߵ�FromActivity��ToActivity��������ԣ�����Ӱ��fromActivity��ToTransitions�Լ�toActivity��FromTransition�ļ���
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

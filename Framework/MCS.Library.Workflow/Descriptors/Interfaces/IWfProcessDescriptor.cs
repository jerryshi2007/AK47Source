using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Workflow.Descriptors
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWfProcessDescriptor : IWfDescriptor
    {
        /// <summary>
        /// 
        /// </summary>
        string Version
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        WfExtendedPropertyDictionary ExtendedProperties
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
        /// 
        /// </summary>
        WfActivityDescriptorCollection Activities
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        IWfInitialActivityDescriptor InitialActivity
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        IWfCompletedActivityDescriptor CompletedActivity
        {
            get;
        }

		/// <summary>
		/// �õ����з�֧�Ļ������������ڵ㰴��LevelName���飩
		/// </summary>
		/// <returns></returns>
		WfActivityLevelGroupCollection GetAllBranchesLevels();

		/// <summary>
		/// �õ����з�֧�Ļ������������ڵ㰴��LevelName���飩
		/// </summary>
		/// <param name="autoCalcaulatePath">�Ƿ��Զ�����Transition�еı��ʽ���м���</param>
		/// <returns></returns>
		WfActivityLevelGroupCollection GetAllBranchesLevels(bool autoCalcaulatePath);

		/// <summary>
		/// �õ����еĻ������������ڵ㰴��LevelName���飩
		/// </summary>
		/// <returns></returns>
		WfActivityLevelGroupCollection GetAllLevels();

		/// <summary>
		/// �õ����еĻ������������ڵ㰴��LevelName���飩
		/// </summary>
		/// <param name="autoCalcaulatePath">�Ƿ��Զ�����Transition�еı��ʽ���м���</param>
		/// <returns></returns>
		WfActivityLevelGroupCollection GetAllLevels(bool autoCalcaulatePath);

		/// <summary>
		/// �Զ������һ��û���ù���ActivityKey
		/// </summary>
		/// <returns></returns>
		string FindNotUsedActivityKey();

		/// <summary>
		/// �Զ������һ��û���ù���TransitionKey
		/// </summary>
		/// <returns></returns>
		string FindNotUsedTransitionKey();

		/// <summary>
		/// �Զ������һ��û���ù���LevelName
		/// </summary>
		/// <returns></returns>
		string FindNotUsedLevelName();

		/// <summary>
		/// ����Key����Transition�����û���ҵ�������null;
		/// </summary>
		/// <param name="transitionKey"></param>
		/// <returns></returns>
		IWfTransitionDescriptor FindTransitionByKey(string transitionKey);
    }
}

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
		/// 得到所有分支的环节描述（将节点按照LevelName分组）
		/// </summary>
		/// <returns></returns>
		WfActivityLevelGroupCollection GetAllBranchesLevels();

		/// <summary>
		/// 得到所有分支的环节描述（将节点按照LevelName分组）
		/// </summary>
		/// <param name="autoCalcaulatePath">是否自动按照Transition中的表达式进行计算</param>
		/// <returns></returns>
		WfActivityLevelGroupCollection GetAllBranchesLevels(bool autoCalcaulatePath);

		/// <summary>
		/// 得到所有的环节描述（将节点按照LevelName分组）
		/// </summary>
		/// <returns></returns>
		WfActivityLevelGroupCollection GetAllLevels();

		/// <summary>
		/// 得到所有的环节描述（将节点按照LevelName分组）
		/// </summary>
		/// <param name="autoCalcaulatePath">是否自动按照Transition中的表达式进行计算</param>
		/// <returns></returns>
		WfActivityLevelGroupCollection GetAllLevels(bool autoCalcaulatePath);

		/// <summary>
		/// 自动计算出一个没有用过的ActivityKey
		/// </summary>
		/// <returns></returns>
		string FindNotUsedActivityKey();

		/// <summary>
		/// 自动计算出一个没有用过的TransitionKey
		/// </summary>
		/// <returns></returns>
		string FindNotUsedTransitionKey();

		/// <summary>
		/// 自动计算出一个没有用过的LevelName
		/// </summary>
		/// <returns></returns>
		string FindNotUsedLevelName();

		/// <summary>
		/// 根据Key查找Transition，如果没有找到，返回null;
		/// </summary>
		/// <param name="transitionKey"></param>
		/// <returns></returns>
		IWfTransitionDescriptor FindTransitionByKey(string transitionKey);
    }
}

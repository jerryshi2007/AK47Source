using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Designer.Models;
using System.Collections.Generic;

namespace Designer
{
    public interface IWebInterAction
    {
        void LoadProperty(string loadType, string proKey, string childKey);

        void UpdateProcess(string exType, string proKey, string childKey, string jsonInfo);

        void DeleteProcess(string delType, string procKey, string childKey);

        void OpenEditor(object sender, EditorType type);

        List<ActivityInfo> LoadActivityTemplate();
        void SaveActivityTemplate(string templateID);
        void LoadProcessInstanceDescription();
    }

    public enum EditorType
    {
        /// <summary>
        /// 条件表达式
        /// </summary>
        Condition,
        /// <summary>
        /// 资源
        /// </summary>
        Resource,
        /// <summary>
        /// 相关链接
        /// </summary>
        RelativeLink,
        /// <summary>
        /// 子流程
        /// </summary>
        BranchProcess,
        /// <summary>
        /// 流程撤销
        /// </summary>
        CancelReceivers,
        /// <summary>
        /// 活动点进入时的通知人
        /// </summary>
        EnterReceivers,
        /// <summary>
        /// 活动点离开时的通知人
        /// </summary>
        LeaveReceivers,

        /// <summary>
        /// 变量编辑
        /// </summary>
        Variables,
        /// <summary>
        /// 内部相关人员
        /// </summary>
        InternalUsers,
        /// <summary>
        /// 外部相关人员
        /// </summary>
        ExternalUsers,
        /// <summary>
        /// 权限矩阵
        /// </summary>
        ImportWfMatrix,
        /// <summary>
        /// 进入结点执行的服务
        /// </summary>
        EnterService,
        /// <summary>
        /// 离开结点执行的服务
        /// </summary>
        LeaveService,
        /// <summary>
        /// 流程自动收集参数
        /// </summary>
        ParametersNeedToBeCollected
    }
}

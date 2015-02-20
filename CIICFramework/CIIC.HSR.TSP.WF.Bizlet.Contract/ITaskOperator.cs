using CIIC.HSR.TSP.WF.BizObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    /// <summary>
    /// 代办流转操作接口
    /// </summary>
    public interface ITaskOperator:IRuntime
    {
        /// <summary>
        /// 产生代办
        /// </summary>
        /// <param name="tasks">任务数据</param>
        /// <param name="tasks">额外数据</param>
        void SendUserTasks(List<USER_TASKBO> tasks,Dictionary<string,string> data);
        /// <summary>
        /// 将任务由代办转为已办
        /// </summary>
        /// <param name="tasks">任务</param>
        /// <param name="data">额外数据</param>
        void SetUserTasksAccomplished(List<USER_TASKBO> tasks, Dictionary<string, string> data);
        /// <summary>
        ///删除代办
        /// </summary>
        /// <param name="tasks">任务列表</param>
        /// <param name="context">上下午数据</param>
        void DeleteUserTasks(List<USER_TASKBO> tasks, Dictionary<string, string> context);
        /// <summary>
        /// 删除代办
        /// </summary>
        /// <param name="tasks">任务</param>
        /// <param name="data">流程参数</param>
        void DeleteUserAccomplishedTasks(List<USER_TASKBO> tasks, Dictionary<string, string> data);

        /// <summary>
        /// 同步流程状态
        /// </summary>
        /// <param name="json">被序列化的流程数据</param>
        void SyncProcess(List<ProcessBO> process);

    }
}

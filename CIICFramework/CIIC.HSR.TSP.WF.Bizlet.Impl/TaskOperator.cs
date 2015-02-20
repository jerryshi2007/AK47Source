using CIIC.HSR.TSP.WF.Bizlet.Contract;
using CIIC.HSR.TSP.WF.BizObject;
using CIIC.HSR.TSP.WF.Persistence.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    /// <summary>
    /// 任务操作
    /// </summary>
    public class TaskOperator : ITaskOperator
    {
        /// <summary>
        /// 产生代办
        /// <param name="tasks">任务数据</param>
        /// <param name="tasks">额外数据</param>
        public void SendUserTasks(List<CIIC.HSR.TSP.WF.BizObject.USER_TASKBO> tasks, Dictionary<string, string> data)
        {

            //输入参数校验
            if (tasks.Count <= 0)
            {
                return;
            }
            //事例代码
            using (var uow = TenantHelper.GetUnitWOrk(this.Context.TenantCode))
            {
                var taskOperatorRepository = uow.GetRepository<USER_TASKBO, IUSER_TASKRepository>();
                string wfDepartmentJsonString = data["Department"];

                Department departmentBo = new Department();

                if (!string.IsNullOrEmpty(wfDepartmentJsonString))
                {
                    departmentBo = JsonConvert.DeserializeObject<Department>(wfDepartmentJsonString);
                }

                foreach (var item in tasks)
                {
                    item.TenantCode = this.Context.TenantCode;
                    item.TaskType = ((int)CIIC.HSR.TSP.WF.Bizlet.Common.TaskStatus.Unprocessed).ToString();
                    item.DepartmentCode = departmentBo.DepartmentCode;
                    item.DepartmentName = departmentBo.DepartmentName;

                    if (item.DELIVER_TIME == null 
                        || (item.DELIVER_TIME != null && item.DELIVER_TIME.ToString() == "0001/1/1 0:00:00"))
                    {
                        item.DELIVER_TIME = DateTime.Now;
                    }

                    taskOperatorRepository.Add(item);
                }
                uow.Commit();
            }
        }
        /// <summary>
        /// 将任务由代办转为已办
        /// </summary>
        /// <param name="tasks">任务</param>
        /// <param name="data">额外数据</param>
        public void SetUserTasksAccomplished(List<CIIC.HSR.TSP.WF.BizObject.USER_TASKBO> tasks, Dictionary<string, string> data)
        {

            //输入参数校验
            if (tasks.Count <= 0)
            {
                return;
            }
            //事例代码
            using (var uow = TenantHelper.GetUnitWOrk(this.Context.TenantCode))
            {
                var taskOperatorRepository = uow.GetRepository<USER_TASKBO, IUSER_TASKRepository>();
                taskOperatorRepository.SetUserTasksAccomplished(tasks, data,this.Context.TenantCode);
            }
        }
        /// <summary>
        /// 删除代办
        /// </summary>
        /// <param name="tasks">任务</param>
        /// <param name="data">流程参数</param>
        public void DeleteUserAccomplishedTasks(List<CIIC.HSR.TSP.WF.BizObject.USER_TASKBO> tasks, Dictionary<string, string> data)
        {
            //输入参数校验
            if (tasks.Count <= 0)
            {
                return;
            }

            //事例代码
            using (var uow = TenantHelper.GetUnitWOrk(this.Context.TenantCode))
            {
                var taskOperatorRepository = uow.GetRepository<USER_TASKBO, IUSER_TASKRepository>();
                taskOperatorRepository.DeleteUserAccomplishedTasks(tasks, data);
            }
        }
        /// <summary>
        /// 上下文数据
        /// </summary>
        public ServiceContext Context
        {
            get;
            set;
        }

        public void DeleteUserTasks(List<USER_TASKBO> tasks, Dictionary<string, string> context)
        {

            //输入参数校验
            if (tasks.Count <= 0)
            {
                return;
            }

            //事例代码
            using (var uow = TenantHelper.GetUnitWOrk(this.Context.TenantCode))
            {
                var taskOperatorRepository = uow.GetRepository<USER_TASKBO, IUSER_TASKRepository>();
                taskOperatorRepository.DeleteUserTasks(tasks, context);
            }
        }

        /// <summary>
        /// 同步流程状态
        /// </summary>
        /// <param name="json">被序列化的流程数据</param>
        public void SyncProcess(List<ProcessBO> process)
        {
            //输入参数校验
            if (process.Count <= 0)
            {
                return;
            }

            //事例代码
            using (var uow = TenantHelper.GetUnitWOrk(this.Context.TenantCode))
            {
                var taskOperatorRepository = uow.GetRepository<USER_TASKBO, IUSER_TASKRepository>();
                if (null != process)
                {
                    process.ForEach(p => p.TenantCode = this.Context.TenantCode);
                }
                taskOperatorRepository.SyncProcess(process);
            }
        }
    }
}

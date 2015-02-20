using CIIC.HSR.TSP.DataAccess;
using CIIC.HSR.TSP.DataAccess.Query;
using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Contract
{
    /// <summary>
    /// 流程管理类
    /// </summary>
    public interface IWfMatrixStorageManager:IRuntime
    {

        /// <summary>
        /// 保存流程
        /// </summary>
        /// <param name="process">流程信息</param>
        void Save(IWfMatrixProcess process);

        /// <summary>
        /// 根据流程Key加载流程
        /// </summary>
        /// <param name="key">流程Key</param>
        /// <returns>流程对象</returns>
        IWfMatrixProcess Load(string key);

        /// <summary>
        /// 根据流程Key删除流程
        /// </summary>
        /// <param name="key">流程KEY</param>
        void Delete(string key);

        /// <summary>
        /// 根据流程Key构建流程，落地操作
        /// </summary>
        /// <param name="key">流程KEY</param>
        /// <param name="name">流程名称</param>
        /// <param name="process">构建的流程</param>
        /// <param name="msg">构建失败的相关提醒信息</param>
        /// <returns>构建执行结果</returns>
        bool Build(string key,string name,out IWfMatrixProcess process,out string msg);

        /// <summary>
        /// 获取流程定义列表
        /// </summary>
        /// <param name="queryModel">查询Model</param>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="pageSize">页显示行数</param>
        /// <param name="totalCount">总记录行数</param>
        /// <returns>当前页流程定义列表</returns>
        PagedCollection<WfClientProcessDescriptorInfo> QueryProcessDescriptorListPaged(QueryModel queryModel, int pageIndex, int pageSize, int? totalCount = default(int?));
     
       
    }
}

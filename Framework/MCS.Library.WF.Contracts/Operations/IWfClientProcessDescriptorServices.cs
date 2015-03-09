using MCS.Library.WF.Contracts.Workflow.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.WF.Contracts.Workflow.DataObjects;
using System.IO;

namespace MCS.Library.WF.Contracts.Operations
{
    /// <summary>
    /// 流程定义的操作定义
    /// </summary>
    [ServiceContract]
    public interface IWfClientProcessDescriptorService
    {
        /// <summary>
        /// 加载一个流程定义（无缓存）
        /// </summary>
        /// <param name="processDespKey"></param>
        /// <returns></returns>
        [OperationContract]
        WfClientProcessDescriptor LoadDescriptor(string processDespKey);

        /// <summary>
        /// 读取一个流程定义（有缓存）
        /// </summary>
        /// <param name="processDespKey"></param>
        /// <returns></returns>
        [OperationContract]
        WfClientProcessDescriptor GetDescriptor(string processDespKey);

        /// <summary>
        /// 保存一个流程定义
        /// </summary>
        /// <param name="processDesp"></param>
        [OperationContract]
        void SaveDescriptor(WfClientProcessDescriptor processDesp);

        /// <summary>
        /// 删除一个流程定义
        /// </summary>
        /// <param name="processDespKey"></param>
        [OperationContract]
        void DeleteDescriptor(string processDespKey);


        /// <summary>
        /// 查询流程定义列表
        /// </summary>
        /// <param name="startRowIndex">从0开始的起始行，相当于分页查询的每一页的起始行</param>
        /// <param name="maximumRows">返回的最大行，相当于分页查询每页的大小</param>
        /// <param name="where">筛选条件</param>
        /// <param name="orderBy">排序字段，允许为空，如果为空，则使用修改时间降排序</param>
        /// <param name="totalCount">以前查询的总记录数，如果是第一次，则传入-1</param>
        /// <returns>分页查询结果，里面包含总行数和每一行的结果。其总行数在翻页时需要传入到totalCount参数中</returns>
        [OperationContract]
        WfClientProcessDescriptorInfoPageQueryResult QueryProcessDescriptorInfo(int startRowIndex, int maximumRows, string where, string orderBy, int totalCount);

        /// <summary>
        /// 判定流程KEY是否存在
        /// </summary>
        /// <param name="processKey">流程KEY</param>
        /// <returns>是否存在</returns>
        [OperationContract]
        bool ExsitsProcessKey(string processKey);

        /// <summary>
        /// 更新委托信息
        /// </summary>
        /// <param name="delegetion"></param>
        [OperationContract]
        void UpdateUserDelegation(WfClientDelegation delegation);

        /// <summary>
        ///  按委托人删除其所有的委托信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [OperationContract]
        void DeleteUserDelegationByUserID(string userID);

        /// <summary>
        /// 删除委托信息
        /// </summary>
        /// <param name="delegation"></param>
        [OperationContract]
        void DeleteUserDelegation(WfClientDelegation delegation);

        /// <summary>
        /// 得到委托人的所有委托信息
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [OperationContract]
        List<WfClientDelegation> LoadUserDelegations(string userID);

        /// <summary>
        /// 导入EXCEL返回具有一个动态活动流程的参数
        /// </summary>
        /// <param name="processKey">流程KEY</param>
        /// <param name="stream">EXCEL流文件</param>
        /// <returns></returns>
        [OperationContract]
        WfCreateClientDynamicProcessParams ExcelToWfCreateClientDynamicProcessParams(string processKey, Stream stream);

        /// <summary>
        /// 导入EXCEL生成具有一个动态活动流程的定义
        /// </summary>
        /// <param name="processKey"></param>
        /// <param name="stream"></param>
        [OperationContract]
        void ExcelToSaveDescriptor(string processKey, Stream stream);

        /// <summary>
        /// 导出具有一个动态活动流程的EXCEL文件
        /// </summary>
        /// <param name="processKey"></param>
        /// <returns>EXCEL流文件</returns>
        [OperationContract]
        Stream WfDynamicProcessToExcel(string processKey);

        /// <summary>
        /// 将若干已经存在流程定义导出
        /// </summary>
        /// <param name="exportParams"></param>
        /// <param name="processKeys"></param>
        /// <returns></returns>
        [OperationContract]
        Stream ExportProcessDescriptors(WfClientExportProcessDescriptorParams exportParams, string[] processKeys);

        /// <summary>
        /// 将流中的内容作为流程定义导入
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        [OperationContract]
        string ImportProcessDescriptors(Stream inputStream);

        /// <summary>
        /// 导出审批矩阵到Excel
        /// </summary>
        /// <param name="matrixID"></param>
        /// <returns></returns>
        [OperationContract]
        Stream ExportApprovalMatrix(string matrixID);

        /// <summary>
        /// 从Excel导入审批矩阵
        /// </summary>
        /// <param name="matrixID"></param>
        /// <param name="inputStream"></param>
        [OperationContract]
        void ImportApprovalMatrix(string matrixID, Stream inputStream);

        /// <summary>
        /// 删除审批矩阵
        /// </summary>
        /// <param name="matrixID"></param>
        [OperationContract]
        void DeleteApprovalMatrix(string matrixID);

        /// <summary>
        /// 审批矩阵是否存在
        /// </summary>
        /// <param name="matrixID"></param>
        /// <returns></returns>
        [OperationContract]
        bool ApprovalMatrixExists(string matrixID);
    }
}

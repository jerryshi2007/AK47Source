using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace MCS.Library.SOA.DocServiceContract
{
    /// <summary>
    /// 数据采集服务
    /// </summary>
    [ServiceContract]
    public interface IDCSDocumentAnalyzeService
    {
        /// <summary>
        /// 根据Word文档生成数据
        /// </summary>
        /// <param name="wordFileContent">word文档内容</param>
        /// <returns></returns>
        [OperationContract]
        DCTWordDataObject DCMAnalyze(byte[] wordFileContent);
    }
}

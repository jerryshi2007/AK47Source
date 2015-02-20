using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace MCS.Library.SOA.DocServiceContract
{
    [ServiceContract]
    public interface IDCSDocumentBuilderService
    {
        /// <summary>
        /// 生成文档
        /// </summary>
        /// <param name="templateUri">模板的地址</param>
        /// <param name="wordData">数据</param>
        /// <returns></returns>
        [OperationContract]
        byte[] DCMBuildDocument(string templateUri, DCTWordDataObject wordData);

        [OperationContract]
        string Hello();
    }
}

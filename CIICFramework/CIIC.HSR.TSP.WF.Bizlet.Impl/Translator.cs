using CIIC.HSR.TSP.WF.Bizlet.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    /// <summary>
    /// 将工作流中的Json格式任务实体转换为强类型
    /// 如TaskBo
    /// </summary>
    public class Translator : ITranslator
    {
        /// <summary>
        /// 解析实体
        /// </summary>
        /// <param name="josn">josn格式的数据</param>
        /// <returns></returns>
        public BizObject.USER_TASKBO Translate(string josn)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 解析多条数据
        /// </summary>
        /// <param name="josn">josn格式的数据</param>
        /// <returns></returns>
        public List<BizObject.USER_TASKBO> TranslateMutiEntity(string josn)
        {
            throw new NotImplementedException();
        }
    }
}

using CIIC.HSR.TSP.WF.Bizlet.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class WfMatrixParameterDefinitionCollection : List<IWfMatrixParameterDefinition>, IWfMatrixParameterDefinitionCollection
    {
        public List<IWfMatrixParameterDefinition> GetEnabledDefinitions()
        {
            return this.Where(p => p.Enabled).ToList();
        }

        /* 去掉和id相关的操作
        public void DeleteDefinition(Guid id)
        {
            var para = this.Where(p => p.Id == id).FirstOrDefault();
            this.Remove(para);
        }

        /// <summary>
        /// 根据Id获取参数定义
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>参数定义信息</returns>
        public IWfMatrixParameterDefinition GetById(Guid id)
        {
            return this.Where(p => p.Id == id).FirstOrDefault(); 
        }
        */
    }
}

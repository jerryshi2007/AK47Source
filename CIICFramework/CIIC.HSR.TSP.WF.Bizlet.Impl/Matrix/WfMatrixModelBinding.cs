using CIIC.HSR.TSP.WF.Bizlet.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class WfMatrixModelBinding : DefaultModelBinder
    {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            Type implementation = GetImplementation(modelType);

            if (null != implementation)
            {
                return base.CreateModel(controllerContext, bindingContext, implementation);
            }

            return base.CreateModel(controllerContext, bindingContext, modelType);
        }
        /// <summary>
        /// 获取接口实例
        /// </summary>
        /// <param name="modelType">model类型</param>
        private Type GetImplementation(Type modelType)
        {
            if (modelType.IsInterface || modelType.IsAbstract)
            {
                if (modelType.FullName == typeof(IWfMatrixActivity).FullName)
                {
                    return typeof(WfMatrixActivity);
                }
                else if (modelType.FullName == typeof(IWfMatrixActivityCollection).FullName)
                {
                    return typeof(WfMatrixActivityCollection);
                }
                else if (modelType.FullName == typeof(IWfMatrixCandidate).FullName)
                {
                    return typeof(WfMatrixCandidate);
                }
                else if (modelType.FullName == typeof(IWfMatrixCandidateCollection).FullName)
                {
                    return typeof(WfMatrixCandidateCollection);
                }
                else if (modelType.FullName == typeof(IWfMatrixCondition).FullName)
                {
                    return typeof(WfMatrixCondition);
                }
                else if (modelType.FullName == typeof(IWfMatrixConditionGroup).FullName)
                {
                    return typeof(WfMatrixConditionGroup);
                }
                else if (modelType.FullName == typeof(IWfMatrixConditionGroupCollection).FullName)
                {
                    return typeof(WfMatrixConditionGroupCollection);
                }
                else if (modelType.FullName == typeof(IWfMatrixParameterDefinition).FullName)
                {
                    return typeof(WfMatrixParameterDefinition);
                }
                else if (modelType.FullName == typeof(IWfMatrixParameterDefinitionCollection).FullName)
                {
                    return typeof(WfMatrixParameterDefinitionCollection);
                }
                else if (modelType.FullName == typeof(IWfMatrixProcess).FullName)
                {
                    return typeof(WfMatrixProcess);
                }
            }

            return null;
        }
    }
}

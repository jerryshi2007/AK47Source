using CIIC.HSR.TSP.WF.Bizlet.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CIIC.HSR.TSP.WF.Bizlet.Impl
{
    public class WfMatrixModelBinders
    {
        public static void RegisterBinders()
        {
            ModelBinders.Binders.Add(typeof(IWfMatrixActivity), new WfMatrixModelBinding());
            ModelBinders.Binders.Add(typeof(IWfMatrixActivityCollection), new WfMatrixModelBinding());
            ModelBinders.Binders.Add(typeof(IWfMatrixCandidate), new WfMatrixModelBinding());
            ModelBinders.Binders.Add(typeof(IWfMatrixCandidateCollection), new WfMatrixModelBinding());
            ModelBinders.Binders.Add(typeof(IWfMatrixCondition), new WfMatrixModelBinding());
            ModelBinders.Binders.Add(typeof(IWfMatrixConditionGroup), new WfMatrixModelBinding());
            ModelBinders.Binders.Add(typeof(IWfMatrixConditionGroupCollection), new WfMatrixModelBinding());
            ModelBinders.Binders.Add(typeof(IWfMatrixParameterDefinition), new WfMatrixModelBinding());
            ModelBinders.Binders.Add(typeof(IWfMatrixParameterDefinitionCollection), new WfMatrixModelBinding());
            ModelBinders.Binders.Add(typeof(IWfMatrixProcess), new WfMatrixModelBinding());
        }
    }
}

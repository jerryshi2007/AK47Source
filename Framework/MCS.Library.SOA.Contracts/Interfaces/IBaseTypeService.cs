using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ServiceModel;
using MCS.Library.SOA.Contracts.DataObjects;
using MCS.Library.SOA.Contracts.DataObjects.Workflow;

namespace MCS.Library.SOA.Contracts
{
    [ServiceContract]
    [ServiceKnownType(typeof(Main))]
    [ServiceKnownType(typeof(ClientOguObjectBase))]
    [ServiceKnownType(typeof(ClientOguUser))]
    [ServiceKnownType(typeof(ClientOguOrganization))]
    [ServiceKnownType(typeof(ClientOguGroup))]
    [ServiceKnownType(typeof(WfClientConditionDescriptor))]
   
    public interface IBaseTypeService
    {
        [OperationContract]
        WfClientConditionDescriptor GetCondition();
         [OperationContract]
        ClientOguDataCollection GetObjects(ClientSearchOUIDType searchType, string[] ids);
         [OperationContract]
        ClientStringCollection GetStringCollection();
         [OperationContract]
        WfClientTransitionDescriptor GetTransition();
         [OperationContract]
        WfClientTransitionDescriptorCollection GetTransitionCollection();
         [OperationContract]
        WfClientVariableDescriptor GetVariable();
         [OperationContract]
        WfClientVariableDescriptorCollection GetVariableCollection();
         [OperationContract]
        ClientPropertyValue GetWfProperty();
         [OperationContract]
        ClientPropertyValueCollection GetWfPropertyCollection();
         [OperationContract]
         WfClientActivityDescriptor GetActivity();
        [OperationContract]
         WfClientActivityDescriptorCollection GetActivityCollection();
    }
    
  
}

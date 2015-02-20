using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using MCS.Library.SOA.Contracts.DataObjects;
using MCS.Library.SOA.Contracts;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using MCS.Library.SOA.Contracts.DataObjects.Workflow;
namespace MCS.Library.SOA.Contracts.DataObjects
{
	/// <summary>
	/// 存放机构人员对象的集合
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
    [CollectionDataContract(IsReference=true)]
    public class ClientOguDataCollection : EditableDataObjectCollectionBase<IClientOguObject>
	{
		

		
	}
}

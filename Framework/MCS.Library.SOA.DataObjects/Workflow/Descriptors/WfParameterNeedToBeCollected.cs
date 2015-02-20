using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.DataObjects;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Workflow
{
    /// <summary>
    /// 流程中需要被收集的参数集合
    /// </summary>
    [Serializable]
    [XElementSerializable]
	public class WfParameterNeedToBeCollected : SerializableEditableKeyedDataObjectCollectionBase<string, WfParameterDescriptor>
    {
        /// <summary>
        /// 合并两个集合
        /// </summary>
        /// <param name="wfParameters"></param>
        public void MergeParameterItems(WfParameterNeedToBeCollected wfParameters)
        {
            foreach (WfParameterDescriptor item in wfParameters)
            {
                if (!this.ContainsKey(item.ParameterName))
                {
                    this.Add(item);
                }
            }
        }

		public void SyncPropertiesToFields(PropertyValue property)
		{
			if (property != null)
			{
				this.Clear();

				if (property.StringValue.IsNotEmpty())
				{
					IEnumerable<WfParameterDescriptor> deserializedData = (IEnumerable<WfParameterDescriptor>)JSONSerializerExecute.DeserializeObject(property.StringValue, this.GetType());

					this.CopyFrom(deserializedData);
				}
			}
		}

        /// <summary>
        /// 获得集合的Key
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override string GetKeyForItem(WfParameterDescriptor item)
        {
            return item.ParameterName;
        }
    }
}

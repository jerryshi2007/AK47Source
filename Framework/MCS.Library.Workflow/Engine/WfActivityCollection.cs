using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Workflow.Engine
{
    /// <summary>
    /// Activity����
    /// </summary>
    [Serializable]
    public class WfActivityCollection : WfKeyedCollectionBase<string, IWfActivity>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IWfActivity this[int index]
        {
            get
            {
				return this.InnerGet(index);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IWfActivity this[string key]
        {
            get
            {
                return this.InnerGet(key);
            }
        }

        /// <summary>
        /// ����һ���ڵ�
        /// </summary>
        /// <param name="act"></param>
        public void Add(IWfActivity act)
        {
			this.InnerAdd(act);
        }

        /// <summary>
        /// ɾ��һ���ڵ�
        /// </summary>
        /// <param name="act"></param>
        public void Remove(IWfActivity act)
        {
			this.InnerRemove(act);
        }

        /// <summary>
        /// ��ActivityID��ΪKey
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected override string GetKeyFromItem(IWfActivity data)
        {
            return data.ID;
        }
    }
}

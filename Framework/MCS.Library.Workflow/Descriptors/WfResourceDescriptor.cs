using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MCS.Library.Core;
using MCS.Library.OGUPermission;
using MCS.Library.Workflow.Properties;
using System.Xml;

namespace MCS.Library.Workflow.Descriptors
{
    /// <summary>
    /// ��Դ������Ļ���
    /// </summary>
    [Serializable]
    public abstract class WfResourceDescriptor : WfDescriptorBase
    {
		public virtual List<IUser> GetUserList()
		{
			ExceptionHelper.FalseThrow(false, Resource.ResourceCanNotGetUserList, this.GetType().Name);

			return null;
		}
    }

    /// <summary>
    /// ��Դ������
    /// </summary>
    [Serializable]
	public class WfResourceDescriptorCollection : WfCollectionBase<WfResourceDescriptor>
    {
        /// <summary>
        /// ������
        /// </summary>
        /// <param name="index">����</param>
        /// <returns>���ذ��������ʵ�����</returns>
        public WfResourceDescriptor this[int index]
        {
            get
            {
				return InnerGet(index);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(WfResourceDescriptor item)
        {
			InnerAdd(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resDesp"></param>
        public void Remove(WfResourceDescriptor resDesp)
        {
			InnerRemove(resDesp);
        }
    }
}


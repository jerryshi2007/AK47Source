#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	BuilderBase.cs
// Remark	��	���ڷ���Ĵ����߻��࣬�û���̳���IBuilder�ӿڡ�
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\wangxiang	    20070430		����
//	1.1			ccic\yuanyong		20070725		����BuildUp��TearDownΪabstract��ͬʱȡ���չ���
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;

namespace MCS.Library.Accessories
{
    /// <summary>
    /// ���ڷ���Ĵ����߻��࣬�û���̳���IBuilder�ӿڡ�
    /// </summary>
    /// <typeparam name="T">ʵ��������</typeparam>
    /// <remarks>���ڷ���Ĵ����߻��࣬�û���̳���IBuilder�ӿڡ�</remarks>
    public abstract class BuilderBase<T> : IBuilder<T>
    {
        /// <summary>
        /// ����ָ�����͵�ʵ����
        /// </summary>
        /// <param name="target">��Ҫ������ʵ��</param>
        /// <returns>ָ�����͵�ʵ��</returns>
        /// <remarks>
		/// ����ָ�����͵�ʵ����
        /// </remarks>
		public abstract T BuildUp(T target);

        /// <summary>
        ///��ָ�����͵�ʵ����
        /// </summary>
        /// <param name="target">��Ҫ�𿪵�ʵ��</param>
        /// <returns>�Ѳ𿪵�ʵ��</returns>
        /// <remarks>��ָ�����͵�ʵ����</remarks>
		public abstract T TearDown(T target);

        /// <summary>
        /// ����ȫ�������ƶ�̬�Ĵ���ָ�����͵�ʵ���� 
        /// </summary>
        /// <param name="typeName">��������</param>
        /// <param name="constructorParams">����ʵ���Ĳ���</param>
        /// <returns>ָ�����͵�ʵ������</returns>
        /// <remarks>
		/// ����ȫ�������ƶ�̬�Ĵ���ָ�����͵�ʵ�����ô���ʵ�������õ��˷�����ơ�
        /// </remarks>
		protected object CreateInstance(string typeName, params object[] constructorParams)
        {
            return TypeCreator.CreateInstance(typeName, constructorParams);
        }
    }
}


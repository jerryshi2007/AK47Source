#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	IFilter.cs
// Remark	��	�������Ľӿڣ��ýӿ�ʵ���˹�����ģʽ��
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\wangxiang	    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Accessories
{
    /// <summary>
    /// �������Ľӿڣ��ýӿ�ʵ���˹�����ģʽ��
    /// </summary>
    /// <typeparam name="T">��������Ҫ���˵�ʵ������</typeparam>
    /// <remarks>���磺�ڹ������д���˺ܶ��������������ʵ������������е������������������ʵ����Ҫ����ĳ�ִ������������һ�ִ���</remarks>
    public interface IFilter<T>
    {
        /// <summary>
        /// �ж�ʵ���Ƿ�����������е���������������
        /// </summary>
        /// <param name="target">��Ҫ���˵�ʵ��</param>
        /// <returns>�������ͣ���ʵ������������е��������������򷵻�true�����򷵻�false��</returns>
        /// <remarks>�ж��������ʵ���Ƿ�ƥ��,���ز���ֵ��
        /// </remarks>
        bool IsMatch(T target);

        /// <summary>
        /// �߼�����
        /// </summary>
        /// <remarks>��ʵ��ʱ���û���Ҫ�õ�pipeline���߼����ƣ�����Դ������ȡ�߼����ơ��������ǿɶ���д�ġ�</remarks>
        string Name { get; set;}
    }
}

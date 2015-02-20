#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	IBuilder.cs
// Remark	��	���ڷ���Ĵ�������ӿڣ��ýӿ�ʵ���˴�����ģʽ��
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
    /// ���ڷ���Ĵ�������ӿڣ��ýӿ�ʵ���˴�����ģʽ��
    /// </summary>
    /// <typeparam name="T">ʵ��������</typeparam>
    /// <remarks>���ڷ���Ĵ�������ӿڣ��ýӿ�ʵ���˴�����ģʽ��
    /// </remarks>
    internal interface IBuilder<T>
    {
        /// <summary>
        /// ����ָ�����͵�ʵ����
        /// </summary>
        /// <param name="target">��Ҫ������ʵ��</param>
        /// <returns>�Ѵ�����ʵ����</returns>
        /// <remarks>����ָ�����͵�ʵ����</remarks>
        T BuildUp(T target);

        /// <summary>
        /// ��ָ�����͵�ʵ����
        /// </summary>
        /// <param name="target">��Ҫ�𿪵�ʵ��</param>
        /// <returns>�Ѳ𿪵�ʵ��</returns>
        /// <remarks>��ָ�����͵�ʵ����</remarks>
        T TearDown(T target);
    }
}

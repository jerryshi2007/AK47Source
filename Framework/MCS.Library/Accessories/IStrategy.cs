#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	IStrategy.cs
// Remark	��	��������Stragegy���������̣�ʵ�ʿ����п��Ը��ݼ����Ҫ����� StrategyContextBase ��������֯��صļ������ݡ�
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
    /// ����ģʽ��ʵ����IStrategy�ӿڡ�
    /// </summary>
    /// <remarks>
    /// ��������Stragegy���������̣�ʵ�ʿ����п��Ը��ݼ����Ҫ����� StrategyContextBase ��������֯��صļ������ݡ�
    /// </remarks>
	/// <typeparam name="TData">�����������</typeparam>
	/// <typeparam name="TResult">����������</typeparam>
    public interface IStrategy<TData, TResult>
    {
        /// <summary>
        /// ����������Ϣ���������㷨����
        /// </summary>
        /// <param name="data">��������</param>
        /// <returns>�㷨������</returns>
		/// <remarks>����������Ϣ���������㷨����</remarks>
        TResult Calculate(TData data);
    }
}

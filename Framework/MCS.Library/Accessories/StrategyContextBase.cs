#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	StrategyContextBase.cs
// Remark	��	��� IStragegy ���������̡�
// -------------------------------------------------
// VERSION  	AUTHOR				DATE			CONTENT
// 1.0		    ccic\wangxiang	    20070430		����
// -------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using MCS.Library.Core;

namespace MCS.Library.Accessories
{
    /// <summary>
    /// ����ģʽ��ʵ����Strategy Context ��
    /// </summary>
    /// <remarks>
    /// ��� IStragegy ���������̡�
    /// </remarks>
	/// <typeparam name="TStrategy">�����������</typeparam>
	/// <typeparam name="TResult">����������</typeparam>
    public abstract class StrategyContextBase<TStrategy, TResult>
    {
        #region Private field
        /// <summary>
        /// Context ��Ҫ�������㷨����
        /// </summary>
        protected TStrategy innerStrategy;
        #endregion

        /// <summary>
        /// ���Ը��ǵĳ��� IStragegy ���͸�����������̣���Ϊ���� Context �����������ɻ����� Calculate �������á�
        /// </summary>
        /// <returns>�ڲ�����Ľ��ֵ</returns>
		/// <remarks>
		/// ���Ը��ǵĳ��� IStragegy ���͸�����������̣���Ϊ���� Context �����������ɻ����� Calculate �������á�
		/// </remarks>
        public abstract TResult DoAction();

        /// <summary>
        /// ���������㷨����
        /// </summary>
		/// <remarks>
		/// ���������㷨����
		/// </remarks>
        public TStrategy Strategy
        {
			get 
			{
				return this.innerStrategy;
			}
            set 
			{
				this.innerStrategy = value; 
			}
        }
    }
}

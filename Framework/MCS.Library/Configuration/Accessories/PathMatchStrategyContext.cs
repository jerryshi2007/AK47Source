#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	PathMatchStrategyContext.cs
// Remark	��	·��ƥ���㷨������̶��� Context
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070430		����
// -------------------------------------------------
#endregion
#region using
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MCS.Library.Core;
using MCS.Library.Accessories;
#endregion
namespace MCS.Library.Configuration.Accessories
{
    /// <summary>
    /// ·��ƥ���㷨������̶��� Context
    /// </summary>
    internal class PathMatchStrategyContext : StrategyContextBase<BestPathMatchStrategyBase, string>
    {
        /// <summary>
        /// ���캯��
        /// </summary>
        public PathMatchStrategyContext() { }

        /// <summary>
        /// ���ݳ����㷨������֯�������
        /// </summary>
        /// <returns></returns>
        public override string DoAction()
        {
			IList<KeyValuePair<string, string>> data = innerStrategy.Candidates;
			return innerStrategy.Calculate(data);
        }
    }
}

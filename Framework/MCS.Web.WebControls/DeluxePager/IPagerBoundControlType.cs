// -------------------------------------------------
// Assembly	��	MCS.Web.WebControls
// FileName	��	IPagerBoundControlType.cs
// Remark	��  
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		�����	    20070815		����
// -------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Web.WebControls
{
    /// <summary>
    /// ҳ������ͽӿ���
    /// </summary>
    /// <remarks>
    ///  ҳ������ͽӿ���
    /// </remarks>
    public interface IPagerBoundControlType
    {
        /// <summary>
        /// ��ȡ��ǰ�󶨵Ŀؼ���״̬����
        /// </summary>
        /// <param name="controlType"></param>
        /// <returns></returns>
        /// <remarks>
        ///  ��ȡ��ǰ�󶨵Ŀؼ���״̬����
        /// </remarks>
        PagerBoundControlStatus GetPagerBoundControl(Type controlType); 
    }
}

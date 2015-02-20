// -------------------------------------------------
// Assembly	��	MCS.Web.Responsive.WebControls
// FileName	��	IPageEventArgs.cs
// Remark	��  
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		�����	    20070815		����
// -------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Web.Responsive.WebControls
{
    /// <summary>
    /// ��ҳ�¼��ӿ�
    /// </summary>
    /// <remarks>
    ///  ��ҳ�¼��ӿ�
    /// </remarks>
    public interface IPageEventArgs
    {
        /// <summary>
        /// ��ȡ�󶨷�ҳ�ؼ���Ӧ�ؼ��ķ�ҳ�¼�Args
        /// </summary>
        /// <param name="controlMode">�󶨵Ŀؼ�ģ��</param>
        /// <param name="eventName">�¼���</param>
        /// <param name="commandSource">����Դ����</param>
        /// <param name="newPageIndex">��ǰ��ҳ��</param>
        /// <returns></returns>
        /// <remarks>
        ///  ��ȡ�󶨷�ҳ�ؼ���Ӧ�ؼ��ķ�ҳ�¼�Args
        /// </remarks>
        object GetPageEventArgs(DataListControlType  controlMode, string eventName, object commandSource, int newPageIndex);

        /// <summary>
        /// ���ð󶨶�Ӧ�ؼ��ķ�ҳ����
        /// </summary>
        /// <param name="objControl">�ؼ�����</param>
        /// <param name="controlMode">����ģ��</param>
        /// <param name="pageSize">��ҳ��С</param>
        /// <returns></returns>
        /// <remarks>
        ///  ���ð󶨶�Ӧ�ؼ��ķ�ҳ����
        /// </remarks>
        bool SetBoundControlPagerSetting(object objControl, DataListControlType  controlMode, int pageSize);
    }
}

using System;
using System.Collections.Generic;
using System.Text;
namespace MCS.Library.Data
{
    /// <summary>
    /// ָ���������¹��̵�ö��
    /// </summary>
    /// <remarks>
    ///     ���������������ӵ�ö��
    ///     added by wangxiang . May 21, 2008
    /// </remarks>
    public enum UpdateBehavior
    {
        /// <summary>
        /// DataAdapter�ı�׼���̣�ִ��������Ϊֹ
        /// </summary>
        Standard,

        /// <summary>
        /// �������ִ�к�������
        /// </summary>
        Continue,

        /// <summary>
        /// ������Ϊһ�������ύ
        /// </summary>
        Transactional
    }

}

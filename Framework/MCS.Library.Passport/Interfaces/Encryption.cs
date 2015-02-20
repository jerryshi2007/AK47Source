#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Passport
// FileName	��	Encryption.cs
// Remark	��	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          ����ǿ      2008-12-2       ���ע��
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Passport
{
    /// <summary>
    /// ʵ��Ticket���ܡ����ܵĽӿ�
    /// </summary>
    public interface ITicketEncryption
    {
        /// <summary>
        /// ����Ticket
        /// </summary>
        /// <param name="ticket">Ticket����</param>
        /// <param name="oParam">���ӵĲ���</param>
        /// <returns>����һ�����ܹ��Ķ�������</returns>
        byte[] EncryptTicket(ITicket ticket, object oParam);

        /// <summary>
        /// ����Ticket
        /// </summary>
        /// <param name="encryptedData">���ܹ���ticket�Ķ���������</param>
        /// <param name="oParam">���ӵĲ���</param>
        /// <returns>���ܺ��ticket����</returns>
        ITicket DecryptTicket(byte[] encryptedData, object oParam);
    }

    /// <summary>
    /// ʵ��string(Cookie)�ļ��ܡ����ܵĽӿ�
    /// </summary>
    public interface IStringEncryption
    {
        /// <summary>
        /// �����ַ���
        /// </summary>
        /// <param name="strData">�ַ�������</param>
        /// <returns>���ܺ�Ķ�������</returns>
        byte[] EncryptString(string strData);

        /// <summary>
        /// �����ַ���
        /// </summary>
        /// <param name="encryptedData">���ܹ�������</param>
        /// <returns>���ܺ���ַ���</returns>
        string DecryptString(byte[] encryptedData);
    }
}

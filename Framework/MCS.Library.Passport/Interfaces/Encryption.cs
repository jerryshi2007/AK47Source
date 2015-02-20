#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library.Passport
// FileName	：	Encryption.cs
// Remark	：	
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0
// 1.1          胡自强      2008-12-2       添加注释
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Passport
{
    /// <summary>
    /// 实现Ticket加密、解密的接口
    /// </summary>
    public interface ITicketEncryption
    {
        /// <summary>
        /// 加密Ticket
        /// </summary>
        /// <param name="ticket">Ticket对象</param>
        /// <param name="oParam">附加的参数</param>
        /// <returns>返回一个加密过的二进制流</returns>
        byte[] EncryptTicket(ITicket ticket, object oParam);

        /// <summary>
        /// 解密Ticket
        /// </summary>
        /// <param name="encryptedData">加密过的ticket的二进制数据</param>
        /// <param name="oParam">附加的参数</param>
        /// <returns>解密后的ticket对象</returns>
        ITicket DecryptTicket(byte[] encryptedData, object oParam);
    }

    /// <summary>
    /// 实现string(Cookie)的加密、解密的接口
    /// </summary>
    public interface IStringEncryption
    {
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="strData">字符串数据</param>
        /// <returns>加密后的二进制流</returns>
        byte[] EncryptString(string strData);

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="encryptedData">加密过的数据</param>
        /// <returns>解密后的字符串</returns>
        string DecryptString(byte[] encryptedData);
    }
}

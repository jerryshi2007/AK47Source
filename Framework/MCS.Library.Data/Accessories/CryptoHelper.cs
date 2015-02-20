#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Data
// FileName	��	CryptoHelper.cs
// Remark	��	����������������
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace MCS.Library.Data.Accessories
{
    /// <summary>
    /// ��������������������������/����/ɢ��/ɢ����֤����
    /// </summary>
    /// <remarks>
    ///     <list type="bullet">
    ///         <item>���ñ�׼DES�㷨���л��ڹ̶���Կ�ļ���/���ܴ���</item>
    ///         <item>���ñ�׼SHA1�㷨����ɢ��</item>
    ///     </list>
    /// </remarks>
#if DELUXEWORKSTEST
    public static class CryptoHelper
#else
    internal static class CryptoHelper
#endif
    {
        #region Private field
        /// <summary>
        /// Hash algorithm
        /// </summary>
        private static SHA1 sha;

        /// <summary>
        /// Crypto transformer of encryption
        /// </summary>
        private static ICryptoTransform et;

        /// <summary>
        /// Crypto transformer of decryption
        /// </summary>
        private static ICryptoTransform dt;
        #endregion

        #region Constructor
        /// <summary>
        /// ���캯����
        /// </summary>
        /// <remarks>�޲����Ĺ��캯������Ĭ�ϵķ�ʽ��ʼ���������ԡ�
        /// </remarks>
        static CryptoHelper()
        {
            CryptoHelper.sha = new SHA1CryptoServiceProvider();
            DESCryptoServiceProvider cryptoService = new DESCryptoServiceProvider();
            byte[] cryptoKey = { 136, 183, 142, 217, 175, 71, 90, 239 };
            byte[] cryptoIV = { 227, 105, 5, 40, 162, 158, 143, 156 };


            cryptoService.Key = cryptoKey;
            cryptoService.IV = cryptoIV;

            CryptoHelper.et = cryptoService.CreateEncryptor();
            CryptoHelper.dt = cryptoService.CreateDecryptor();
        }
        #endregion

        #region Public method

        #region hash / comparehash
        /// <overrides>
        /// Computes the hash value of plain text using the given hash provider instance
        /// </overrides>
        /// <summary>
        /// Computes the hash value of plain text using the given hash provider instance
        /// </summary>
        /// <param name="plaintext">The input for which to compute the hash.</param>
        /// <returns>The computed hash code.</returns>
        private static byte[] CreateHash(byte[] plaintext)
        {
            return CryptoHelper.sha.ComputeHash(plaintext);
        }

        /// <summary>
        /// Ϊ���ı��ַ��������ϣֵ
        /// </summary>
        /// <param name="plaintext">��Ҫ�����ϣֵ�Ĵ��ı��ַ���</param>
        /// <returns>������Ĺ�ϣֵ</returns>
        /// <remarks>
        /// Ϊ���ı��ַ��������ϣֵ��
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\CryptoHelperTest.cs" lang="cs" region="CreateHashTest" title="�����ַ�����ɢ�н��(string)"/>
        /// </remarks>
        public static string CreateHash(string plaintext)
        {
            byte[] plainTextBytes = UnicodeEncoding.Unicode.GetBytes(plaintext);
            byte[] resultBytes = CreateHash(plainTextBytes);

            return Convert.ToBase64String(resultBytes);
        }

        /// <overrides>
        /// Compares plain text input with a computed hash using the given hash provider instance.
        /// </overrides>
        /// <summary>
        /// Compares plain text input with a computed hash using the given hash provider instance.
        /// </summary>
        /// <remarks>
        /// Use this method to compare hash values. Since hashes may contain a random "salt" value, two seperately generated
        /// hashes of the same plain text may result in different values. 
        /// </remarks>
        /// <param name="plaintext">The input for which you want to compare the hash to.</param>
        /// <param name="hashedText">The hash value for which you want to compare the input to.</param>
        /// <returns><c>true</c> if plainText hashed is equal to the hashedText. Otherwise, <c>false</c>.</returns>
        private static bool HashCheck(byte[] plaintext, byte[] hashedText)
        {
            if ((hashedText == null) || (hashedText.Length <= 0))
                return false;

            byte[] hashedResult = CryptoHelper.sha.ComputeHash(plaintext);
            if (hashedText.Length != hashedResult.Length)
                return false;

            for (int i = 0; i < hashedResult.Length; i++)
            {
                if (hashedText[i] != hashedResult[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// �Ƚ��ɴ��ı���������Ĺ�ϣֵ������Ĺ�ϣֵ�Ƿ�һ�¡�
        /// </summary>
        /// <param name="plaintext">������Ҫ���й�ϣ�Ƚϵ��ַ���</param>
        /// <param name="hashedText">��ϣ��</param>
        /// <returns><c>true</c> ������ı���ϣֵ������Ĺ�ϣֵ��ȣ����򷵻�<c>false</c>.</returns>
        /// <remarks>
        /// ��������������Ƚ��ɴ��ı�������Ĺ�ϣֵ�͸����Ĺ�ϣֵ�Ƿ�һ�¡�
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\CryptoHelperTest.cs" lang="cs" region="HashCheckTest" title="��֤�ַ�����ɢ�н��(string)"/>
        /// </remarks>
        public static bool HashCheck(string plaintext, string hashedText)
        {
            byte[] plainTextBytes = UnicodeEncoding.Unicode.GetBytes(plaintext);
            byte[] hashedTextBytes = Convert.FromBase64String(hashedText);

            bool result = HashCheck(plainTextBytes, hashedTextBytes);

            return result;
        }
        #endregion

        #region nvative key encrypt / decrypt

        /// <summary>
        /// ���ַ����Ľ��ܴ���
        /// </summary>
        /// <param name="source">��Ҫ���ܵ��ַ���</param>
        /// <returns>���ܺ���ַ���</returns>
        /// <remarks>���ַ����Ľ��ܴ���
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\CryptoHelperTest.cs" lang="cs" region="DecryptTest" title="DES�㷨�ַ�������"/>
        /// </remarks>
        public static string Decrypt(string source)
        {
            byte[] buff = Convert.FromBase64String(source);
            using (MemoryStream mem = new MemoryStream())
            {
                CryptoStream stream = new CryptoStream(mem, CryptoHelper.dt, CryptoStreamMode.Write);

                stream.Write(buff, 0, buff.Length);
                stream.Flush();

                return Encoding.Unicode.GetString(mem.ToArray());
            }
        }

        /// <summary>
        /// ���ַ����ļ��ܴ���
        /// </summary>
        /// <param name="source">��Ҫ���ܵ��ַ���</param>
        /// <returns>���ܺ���ַ���</returns>
        /// <remarks>���ַ����ļ��ܴ���
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Core\CryptoHelperTest.cs" lang="cs" region="EncryptTest" title="DES�㷨�ַ�������"/>
        /// </remarks>
        public static string Encrypt(string source)
        {
            byte[] buff = Encoding.Unicode.GetBytes(source);

            MemoryStream mem = new MemoryStream();
            CryptoStream stream = new CryptoStream(mem, CryptoHelper.et, CryptoStreamMode.Write);
            stream.Write(buff, 0, buff.Length);
            stream.FlushFinalBlock();
            stream.Clear();

            return Convert.ToBase64String(mem.ToArray());
        }
        #endregion

        #endregion
    }
}

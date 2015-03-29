using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace MCS.Library.Security
{
    /// <summary>
    /// 基于DES的对称算法的基类
    /// </summary>
    public abstract class DESEncryptorBase : ISymmetricEncryption
    {
        /// <summary>
        /// 将二进制流加密为字符串
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public byte[] EncryptString(string strData)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(strData);

            MemoryStream ms = new MemoryStream();

            using (SymmetricAlgorithm algorithm = this.GetDesObject())
            {
                using (CryptoStream encStream = new CryptoStream(ms, algorithm.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    encStream.Write(bytes, 0, bytes.Length);
                }
            }

            return ms.ToArray();
        }

        /// <summary>
        /// 将二进制流解密为字符串
        /// </summary>
        /// <param name="encryptedData"></param>
        /// <returns></returns>
        public string DecryptString(byte[] encryptedData)
        {
            string strResult = string.Empty;

            MemoryStream ms = new MemoryStream();

            ms.Write(encryptedData, 0, encryptedData.Length);
            ms.Seek(0, SeekOrigin.Begin);

            using (SymmetricAlgorithm algorithm = this.GetDesObject())
            {
                using (CryptoStream decStream = new CryptoStream(ms, algorithm.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    strResult = (new StreamReader(decStream, Encoding.UTF8)).ReadToEnd();
                }
            }

            return strResult;
        }

        /// <summary>
        /// 得到DES对象（提供密钥）
        /// </summary>
        /// <returns></returns>
        protected abstract DES GetDesObject();
    }
}

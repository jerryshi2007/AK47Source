using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace MCS.Web.Apps.WeChat
{
	internal static class Common
	{
		public static string GetMd5String(string str) //MD5摘要算法
		{
			MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

			// Convert the input string to a byte array and compute the hash.
			byte[] buf = Encoding.GetEncoding("gb2312").GetBytes(str);

			byte[] data = md5Hasher.ComputeHash(buf);

			// Create a new Stringbuilder to collect the bytes  
			// and create a string.  
			StringBuilder strB = new StringBuilder();

			// Loop through each byte of the hashed data   
			// and format each one as a hexadecimal string.  
			for (int i = 0; i < data.Length; i++)
				strB.Append(data[i].ToString("x2"));

			return strB.ToString();
		}
	}
}

#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	ZipConstants.cs
// Remark	：	Zip常量
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    梁东	    20070430		创建
// -------------------------------------------------
#endregion
#region
using System;
using System.Collections.Generic;
using System.Text;
#endregion
namespace MCS.Library.Compression
{
    /// <summary>
    /// 错误类别
    /// </summary>
    internal enum CompressionError
    {
        errNone = 0,

        /// <summary>
        /// 非法Stream
        /// </summary>
        InvalidStream,

        /// <summary>
        /// 非法的压缩文件头结构
        /// </summary>
        InvalidFileHeader,

        /// <summary>
        /// 非法的压缩文件目录区
        /// </summary>
        InvalidFileDir,

        /// <summary>
        /// 非法的压缩文件目录结束
        /// </summary>
        InvalidFileEnding,

        /// <summary>
        /// 非法的SFX文件
        /// </summary>
        InvalidSFXFile,

        /// <summary>
        /// 非法的压缩算法编号
        /// </summary>
        InvalidMethod,

        /// <summary>
        /// CRC校验失败
        /// </summary>
        InvalidCRC,
    }

    /// <summary>
    /// ZipConstants中定义了压缩中使用到的常量。
    /// </summary>
    internal static class ZipConstants
    {
        public const int LOCSIG = 'P' | ('K' << 8) | (3 << 16) | (4 << 24);
        public const int CENSIG = 'P' | ('K' << 8) | (1 << 16) | (2 << 24);
        public const int ENDSIG = 'P' | ('K' << 8) | (5 << 16) | (6 << 24);
        public const int CENHDR = 46;
        /// <summary>
        /// Size of local entry header (excluding variable length fields at end)
        /// </summary>
        public const int LOCHDR = 30;

        static int defaultCodePage = 0;

        /// <summary>
        /// 默认代码页，为字符串转换提供默认编码。
        /// 0是系统默认Ansi编码。如果希望与Zip压缩兼容请不要使用unicode编码。
        /// </summary>
        public static int DefaultCodePage
        {
            get
            {
                return defaultCodePage;
            }
			//set
			//{
			//    defaultCodePage = value;
			//}
        }

        /// <summary>
        /// 将byte数组的指定部分转换为字符串
        /// </summary>		
        /// <param name="data">
        /// 需要转换的byte数组
        /// </param>
        /// <param name="length">
        /// 需要转换部分的长度，从索引0处开始计算。
        /// </param>
        /// <returns>
        /// 由data[0]..data[length - 1] 部分转换成的字符串
        /// </returns>
        public static string ConvertToString(byte[] data, int length)
        {
            return Encoding.GetEncoding(DefaultCodePage).GetString(data, 0, length);
        }

		///// <summary>
		///// Convert byte array to string
		///// </summary>
		///// <param name="data">
		///// Byte array to convert
		///// </param>
		///// <returns>
		///// <paramref name="data">data</paramref>converted to a string
		///// </returns>
		//public static string ConvertToString(byte[] data)
		//{
		//    return ConvertToString(data, data.Length);
		//}

        /// <summary>
        /// 将字符串转换成byte数组
        /// </summary>
        /// <param name="str">
        /// 需要转换的字符串
        /// </param>
        /// <returns>转换后的byte数组</returns>
        public static byte[] ConvertToArray(string str)
        {
            return Encoding.GetEncoding(DefaultCodePage).GetBytes(str);
        }

        /// <summary>
        /// Size of end of central record (excluding variable fields)
        /// </summary>
        public const int ENDHDR = 22;        
        public const byte DEFLATE = 4;	//Indicating deflate method of compression
        public const byte GZIP = 6;		//Indicating gzip method of compression       
       
    }
}

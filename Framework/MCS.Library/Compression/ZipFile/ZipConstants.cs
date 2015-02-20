#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	ZipConstants.cs
// Remark	��	Zip����
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ����	    20070430		����
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
    /// �������
    /// </summary>
    internal enum CompressionError
    {
        errNone = 0,

        /// <summary>
        /// �Ƿ�Stream
        /// </summary>
        InvalidStream,

        /// <summary>
        /// �Ƿ���ѹ���ļ�ͷ�ṹ
        /// </summary>
        InvalidFileHeader,

        /// <summary>
        /// �Ƿ���ѹ���ļ�Ŀ¼��
        /// </summary>
        InvalidFileDir,

        /// <summary>
        /// �Ƿ���ѹ���ļ�Ŀ¼����
        /// </summary>
        InvalidFileEnding,

        /// <summary>
        /// �Ƿ���SFX�ļ�
        /// </summary>
        InvalidSFXFile,

        /// <summary>
        /// �Ƿ���ѹ���㷨���
        /// </summary>
        InvalidMethod,

        /// <summary>
        /// CRCУ��ʧ��
        /// </summary>
        InvalidCRC,
    }

    /// <summary>
    /// ZipConstants�ж�����ѹ����ʹ�õ��ĳ�����
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
        /// Ĭ�ϴ���ҳ��Ϊ�ַ���ת���ṩĬ�ϱ��롣
        /// 0��ϵͳĬ��Ansi���롣���ϣ����Zipѹ�������벻Ҫʹ��unicode���롣
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
        /// ��byte�����ָ������ת��Ϊ�ַ���
        /// </summary>		
        /// <param name="data">
        /// ��Ҫת����byte����
        /// </param>
        /// <param name="length">
        /// ��Ҫת�����ֵĳ��ȣ�������0����ʼ���㡣
        /// </param>
        /// <returns>
        /// ��data[0]..data[length - 1] ����ת���ɵ��ַ���
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
        /// ���ַ���ת����byte����
        /// </summary>
        /// <param name="str">
        /// ��Ҫת�����ַ���
        /// </param>
        /// <returns>ת�����byte����</returns>
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

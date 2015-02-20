#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Data
// FileName	��	ISqlBuilder.cs
// Remark	��	������һЩSQL���ͨ���﷨�ӿڡ�
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ������	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Data.Builder
{
    /// <summary>
    /// ������һЩSQL���ͨ���﷨�ӿڣ������ɲ�ͬ��ʵ������ʵ��
    /// </summary>
    public interface ISqlBuilder
    {
        /// <summary>
        /// ���е����ż�飬��������ַ������е����ţ���ô�滻�����������ţ���ֹע��ʽ����
        /// </summary>
        /// <param name="data">�ַ�����ֵ</param>
        /// <param name="addQuotation">����ֵ�Ƿ���data��������ӵ�����</param>
        /// <returns>������ַ���</returns>
        string CheckQuotationMark(string data, bool addQuotation);

		/// <summary>
		/// ���е����ż�飬��������ַ������е����ţ���ô�滻�����������ţ���ֹע��ʽ������Ȼ����ͷβ�����һ�����š�Ȼ�����Unicode˵��
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		string CheckUnicodeQuotationMark(string data);

        /// <summary>
        /// ���ݿ�˷��ص�ǰʱ��ĺ�������
        /// </summary>
        string DBCurrentTimeFunction
        {
            get;
        }

        /// <summary>
        /// �������ݿ��л���ַ������ȵĺ�������
        /// </summary>
        /// <param name="data">�ֶΡ���������������</param>
        /// <param name="addQuotation">data�����Ƿ���Ҫ������</param>
        /// <returns>�ַ������ȵĺ�������</returns>
        string GetDBStringLengthFunction(string data, bool addQuotation);

        /// <summary>
        /// �������ݿ��л���ֽڳ��ȵĺ�������
        /// </summary>
        /// <param name="data">�ֶΡ���������������</param>
        /// <param name="addQuotation">data�����Ƿ���Ҫ������</param>
        /// <returns>�ֽڳ��ȵĺ�������</returns>
        string GetDBByteLengthFunction(string data, bool addQuotation);

        /// <summary>
        /// �������ݿ���SubString�������ַ���
        /// </summary>
        /// <param name="data">�ֶΡ���������������</param>
        /// <param name="start">��ʼλ</param>
        /// <param name="length">����</param>
        /// <param name="addQuotation">data�����Ƿ���Ҫ������</param>
        /// <returns>SubString�������ַ���</returns>
        string GetDBSubStringStr(string data, int start, int length, bool addQuotation);

        /// <summary>
        /// ��DateTime��ʽ��Ϊ���ݿ���ʶ������ڸ�ʽ
        /// </summary>
        /// <param name="dt">����</param>
        /// <returns>���ݿ��е����ڳ�����ʾ</returns>
        string FormatDateTime(DateTime dt);

		/// <summary>
		/// ��LIKE��Ӧ�Ĳ�ѯ�Ӿ�ת��
		/// </summary>
		/// <param name="likeString"></param>
		/// <returns></returns>
		string EscapeLikeString(string likeString);

		/// <summary>
		/// ��ʽ��ȫ�ļ����ַ�����Ĭ�ϰ���SQL Server�Ĺ�������˫���š����ҽ��ո��滻���߼������
		/// </summary>
		/// <param name="logicOp">�߼��������Ĭ����AND</param>
		/// <param name="searchText"></param>
		/// <returns></returns>
		string FormatFullTextString(LogicOperatorDefine logicOp, string searchText);

        /// <summary>
        /// SQL����У��ַ���֮������ӷ�
        /// </summary>
        string DBStrConcatSymbol
        {
            get;
        }

        /// <summary>
        /// �õ�SQL Server�е�ISNULL��Oracle�е�NVL
        /// </summary>
        /// <param name="data">��Ҫ����ֵ</param>
        /// <param name="nullStr">���dataΪnull, ��ת���ɵ��ַ���</param>
        /// <param name="addQuotation">data�����Ƿ���Ҫ������</param>
        /// <returns>�õ�SQL Server�е�ISNULL��Oracle�е�NVL</returns>
        string DBNullToString(string data, string nullStr, bool addQuotation);

        /// <summary>
        /// ����SQL�Ŀ�ʼ��ʶ��SQL Server��û�У�Oracle����BEGIN
        /// </summary>
        string DBStatementBegin
        {
            get;
        }

        /// <summary>
        /// ����SQL�Ľ�����ʶ��SQL Server��û�У�Oracle����BEGIN
        /// </summary>
        string DBStatementEnd
        {
            get;
        }

        /// <summary>
        /// SQL���֮��ķָ�����SQL Server���ǡ�;����CR/LF��Oracle���ǡ�;��+CR/LF
        /// </summary>
        string DBStatementSeperator
        {
            get;
        }
    }
}

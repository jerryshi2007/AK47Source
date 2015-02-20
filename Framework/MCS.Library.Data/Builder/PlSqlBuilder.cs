#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library.Data
// FileName	��	PlSqlBuilder.cs
// Remark	��	����PL/SQL��ISqlBuilder��ʵ����
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
    /// ����PL/SQL��ISqlBuilder��ʵ����
    /// </summary>
    public class PlSqlBuilder : SqlBuilderBase
    {
        /// <summary>
        /// PlSql��ʵ��
        /// </summary>
        public static readonly PlSqlBuilder Instance = new PlSqlBuilder();

        private PlSqlBuilder()
        {
        }
        /// <summary>
        ///  ���ݿ�˷��ص�ǰʱ��ĺ�������
        /// </summary>
        public override string DBCurrentTimeFunction
        {
            get
            {
                return "SYSDATE";
            }
        }
        /// <summary>
        /// ��д��������ת�����ַ�����ʽ
        /// </summary>
        /// <param name="data">�ַ�������</param>
        /// <param name="nullStr">���ַ���</param>
        /// <param name="addQuotation">�Ƿ���������</param>
        /// <returns>����ת������ַ���</returns>
        public override string DBNullToString(string data, string nullStr, bool addQuotation)
        {
            return string.Format("NVL({0}, {1})", AddQuotation(data, addQuotation), nullStr);
        }
        /// <summary>
        /// ����SQL�Ŀ�ʼ��ʶ��SQL Server��û�У�Oracle����BEGIN
        /// </summary>
        public override string DBStatementBegin
        {
            get
            {
                return "BEGIN\n";
            }
        }
        /// <summary>
        /// ����SQL�Ľ�����ʶ��SQL Server��û�У�Oracle����END
        /// </summary>
        public override string DBStatementEnd
        {
            get
            {
                return "END\n";
            }
        }
        /// <summary>
        /// QL���֮��ķָ�����SQL Server���ǡ�;����CR/LF��Oracle���ǡ�;��+CR/LF
        /// </summary>
        public override string DBStatementSeperator
        {
            get
            {
                return ";\n";
            }
        }
        /// <summary>
        /// SQL����У��ַ���֮������ӷ�
        /// </summary>
        public override string DBStrConcatSymbol
        {
            get
            {
                return "||";
            }
        }
        /// <summary>
        /// ��DateTime��ʽ��Ϊ���ݿ���ʶ������ڸ�ʽ
        /// </summary>
        /// <param name="dt">ʱ�����͵�����</param>
        /// <returns>���ݿ���ʶ������ڸ�ʽ</returns>
        public override string FormatDateTime(DateTime dt)
        {
			return string.Format("TO_DATE({0}, 'YYYY-MM-DD HH24:MI:SS')", AddQuotation(dt.ToString("yyyy-MM-dd HH:mm:ss"), true));
        }

        /// <summary>
        /// �������ݿ��л���ֽڳ��ȵĺ�������
        /// </summary>
        /// <param name="data">�ַ�������</param>
        /// <param name="addQuotation">�Ƿ����</param>
        /// <returns>�������ݿ��л���ֽڳ��ȵĺ�������</returns>
        public override string GetDBByteLengthFunction(string data, bool addQuotation)
        {
            return string.Format("LENGTHB({0})", AddQuotation(data, addQuotation));
        }
        /// <summary>
        /// �������ݿ��л���ַ������ȵĺ�������
        /// </summary>
        /// <param name="data"></param>
        /// <param name="addQuotation"></param>
        /// <returns>�������ݿ��л���ַ������ȵĺ�������</returns>
        public override string GetDBStringLengthFunction(string data, bool addQuotation)
        {
            return string.Format("LENGTH({0})", AddQuotation(data, addQuotation));
        }
        /// <summary>
        /// �������ݿ���SubString�������ַ���
        /// </summary>
        /// <param name="data">��Ҫ��ʽ��������</param>
        /// <param name="start">��ʼλ��</param>
        /// <param name="length">����λ��</param>
        /// <param name="addQuotation">�Ƿ���Ϊ�ַ�������ִ��</param>
        /// <returns>�������ݿ���SubString�������ַ���</returns>
        public override string GetDBSubStringStr(string data, int start, int length, bool addQuotation)
        {
            return string.Format("SUBSTR({0}, {1}, {2})", AddQuotation(data, addQuotation), start, length);
        }
    }
}

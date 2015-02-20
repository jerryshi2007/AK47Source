#region
// -------------------------------------------------
// Assembly	��	DeluxeWorks.Library
// FileName	��	ExpClasses.cs
// Remark	��	�������Ľڵ�
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    ���ķ�	    20070430		����
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MCS.Library.Core;
using MCS.Library.Properties;
using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.IO;

namespace MCS.Library.Expression
{
    /// <summary>
    /// �������Ľڵ�
    /// </summary>
    /// 
    [Serializable]
    public sealed class ExpTreeNode : ISerializable
    {
        private ExpTreeNode left = null;
        private ExpTreeNode right = null;
        private int position = 0;
        private Operation_IDs operationID = Operation_IDs.OI_NONE;
        private Object nodeValue = null;
        private List<ExpTreeNode> functionParams = null;
        private string functionName = string.Empty;

        /// <summary>
        /// ����ʵ�ֶ������ķ����л�
        /// </summary>
        /// <param name="info">The object to be populated with serialization information.</param>
        /// <param name="context">The destination context of the serialization.</param>
        /// <remarks>
        /// �������ķ����л�
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Expression\ExpressionParserTest.cs" region="Serialization" lang="cs" />
        /// </remarks>
        private ExpTreeNode(SerializationInfo info, StreamingContext context)
        {
            this.left = (ExpTreeNode)info.GetValue("Left", typeof(ExpTreeNode));
            this.right = (ExpTreeNode)info.GetValue("Right", typeof(ExpTreeNode));
            this.position = info.GetInt32("Position");
            this.operationID = (Operation_IDs)info.GetInt16("OperationID");

            string valueTypeName = info.GetString("ValueType");
            if (valueTypeName != "NullValue")
            {
                this.nodeValue = (Object)info.GetValue("Value", Type.GetType(valueTypeName));
            }
            else
            {
                this.nodeValue = (Object)info.GetValue("Value", typeof(Object));
            }
            this.functionParams = (List<ExpTreeNode>)info.GetValue("Params", typeof(ExpTreeNode));
            this.functionName = info.GetString("FunctionName");
        }

        /// <summary>
        /// ����GetObjectDataʵ�ֶ����������л�
        /// </summary>
        /// <param name="info">The object to be populated with serialization information. </param>
        /// <param name="context">The destination context of the serialization. </param>
        /// <remarks>�Զ�����ʵ�����л�
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Expression\ExpressionParserTest.cs" region="Serialization" lang="cs" />
        /// </remarks>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Left", this.left);
            info.AddValue("Right", this.right);
            info.AddValue("Position", this.position);
            info.AddValue("OperationID", this.operationID);
            if (this.nodeValue != null)
            {
                info.AddValue("ValueType", this.nodeValue.GetType().FullName);
            }
            else
            {
                info.AddValue("ValueType", "NullValue");
            }
            info.AddValue("Value", this.nodeValue);
            info.AddValue("Params", this.functionParams);
            info.AddValue("FunctionName", this.functionName);
        }

        internal ExpTreeNode()
        {
        }

        /// <summary>
        /// ��������������
        /// </summary>
        public ExpTreeNode Left
        {
            get { return this.left; }
            internal set { this.left = value; }
        }


        /// <summary>
        /// ��������������
        /// </summary>
        public ExpTreeNode Right
        {
            get { return this.right; }
            internal set { this.right = value; }
        }

        /// <summary>
        /// ���������ڽ���ڱ��ʽ�еľ���λ��
        /// </summary>
        public int Position
        {
            get { return this.position; }
            internal set { this.position = value; }
        }

        /// <summary>
        /// �����
        /// </summary>
        public Operation_IDs OperationID
        {
            get { return this.operationID; }
            internal set { this.operationID = value; }//add By Yuanyong 20070418
        }

        /// <summary>
        /// ��������ֵ
        /// </summary>
        public Object Value
        {
            get { return this.nodeValue; }
            internal set { this.nodeValue = value; }
        }


        /// <summary>
        /// ��������㺯���Ĳ���
        /// </summary>
        public List<ExpTreeNode> Params
        {
            get { return this.functionParams; }
            internal set { this.functionParams = value; }
        }

        /// <summary>
        /// ���������������ĺ�����
        /// </summary>
        public string FunctionName
        {
            get { return this.functionName; }
            internal set { this.functionName = value; }
        }
    }

    /// <summary>
    /// ���ʽ�����Ľ��
    /// </summary>
    /// <remarks>
    /// ���ʽ����������Ľ��
    /// </remarks>
    public sealed class ParseResult
    {
        private ExpTreeNode tree = null;
        private ParseIdentifier identifiers = null;

        internal ParseResult(ExpTreeNode tree, ParseIdentifier identifiers)
        {
            this.tree = tree;
            this.identifiers = identifiers;
        }

        private ParseResult()
        {
        }

        /// <summary>
        /// ȡ�������Ķ�����
        /// </summary>
        public ExpTreeNode Tree
        {
            get { return this.tree; }
        }

        /// <summary>
        /// ��ʶ��
        /// </summary>
        public ParseIdentifier Identifiers
        {
            get { return this.identifiers; }
        }
    }

    /// <summary>
    /// �û��Զ��庯������Ĳ���
    /// </summary>
    /// <remarks>
    /// �û�����Ĳ�������
    /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Expression\ExpressionParserTest.cs" region="UserDefinedFucntions" lang="cs" />
    /// </remarks>
    public sealed class ParamObject
    {
        private object paramValue = null;
        private int position = -1;
        private int paramIndex = 0;

        internal ParamObject(object v, int nPos, int paramIndex)
        {
            this.paramValue = v;
            this.position = nPos;
            this.paramIndex = paramIndex;
        }

        /// <summary>
        /// ����ֵ
        /// </summary>
        public object Value
        {
            get { return this.paramValue; }
            internal set { this.paramValue = Value; }
        }

        /// <summary>
        /// �����ڱ��ʽ�е�λ��
        /// </summary>
        public int Position
        {
            get { return this.position; }
            internal set { this.position = value; }
        }

        /// <summary>
        /// �ǵڼ�������
        /// </summary>
        public int ParamIndex
        {
            get { return this.paramIndex; }
            internal set { this.paramIndex = value; }
        }

        /// <summary>
        /// ������������
        /// </summary>
        /// <typeparam name="T">�����Ƿ���ָ�����ͣ�����������������</typeparam>
        /// <remarks>
        /// �����������Ƿ�Ϊָ������
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Expression\ExpressionParserTest.cs" region="UserDefinedFucntions" lang="cs" />
        /// </remarks>
        public void CheckParameterType<T>()
        {
            if (this.Value != null)
            {
                System.Type expectedType = typeof(T);

                //bool temp = !(this.Value.GetType().IsSubclassOf(expectedType) || this.Value.GetType() == expectedType);

                if (false == (this.Value.GetType().IsSubclassOf(expectedType) || this.Value.GetType() == expectedType))
                {
                    throw ParsingException.NewParsingException(ParseError.InvalidParameterType, 0, expectedType.Name, this.Value.GetType().Name);
                }
            }
        }
    }

    /// <summary>
    /// �������͵ļ���
    /// </summary>
    /// <remarks>
    /// �û��Զ��庯������Ĳ�������
    /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Expression\ExpressionParserTest.cs" region="UserDefinedFucntions" lang="cs" />
    /// </remarks>
    public sealed class ParamObjectCollection : ReadOnlyCollection<ParamObject>
    {
        internal ParamObjectCollection(List<ParamObject> list)
            : base(list)
        {
        }

        /// <summary>
        /// ȡ���������еĲ���
        /// </summary>
        /// <param name="i">�ڼ�������</param>
        /// <returns>���ز���������ΪParamObject</returns>
        public new ParamObject this[int i]
        {
            get { return base[i]; }
        }

        /// <summary>
        /// �������ĸ���
        /// </summary>
        /// <param name="nLimit">��������С����</param>
        public void CheckParamsLength(int nLimit)
        {
            ExceptionHelper.FalseThrow(this.Count >= nLimit, ExpressionParserRes.InvalidParamsCount, nLimit);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class ParseIdentifier
    {
        private Operation_IDs operationID = Operation_IDs.OI_NONE;
        private string identifier = string.Empty;
        private int position = -1;
        private ParseIdentifier prevIdentifier = null;
        private ParseIdentifier nextIdentifier = null;
        private ParseIdentifier subIdentifier = null;
        private ParseIdentifier parentIdentifier = null;

        /// <summary>
        /// ��ʶ��������
        /// </summary>
        public Operation_IDs OperationID
        {
            get { return this.operationID; }
            internal set { this.operationID = value; }
        }

        /// <summary>
        /// ��ʶ�����ı�
        /// </summary>
        public string Identifier
        {
            get { return this.identifier; }
            internal set { this.identifier = value; }
        }

        /// <summary>
        /// ��ʶ��������
        /// </summary>
        public int Position
        {
            get { return this.position; }
            internal set { this.position = value; }
        }

        /// <summary>
        /// ǰһ����ʶ��
        /// </summary>
        public ParseIdentifier PrevIdentifier
        {
            get { return this.prevIdentifier; }
            internal set { this.prevIdentifier = value; }
        }

        /// <summary>
        /// ��һ����ʶ��
        /// </summary>
        public ParseIdentifier NextIdentifier
        {
            get { return this.nextIdentifier; }
            internal set { this.nextIdentifier = value; }
        }

        /// <summary>
        /// �ӱ�ʶ��
        /// </summary>
        public ParseIdentifier SubIdentifier
        {
            get { return this.subIdentifier; }
            internal set { this.subIdentifier = value; }
        }

        /// <summary>
        /// ����ʶ��
        /// </summary>
        public ParseIdentifier ParentIdentifier
        {
            get { return this.parentIdentifier; }
            internal set { this.parentIdentifier = value; }
        }

        /// <summary>
        /// �������
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder strB = new StringBuilder();

            using (StringWriter writer = new StringWriter(strB))
            {
                WriteIdentifierInfoRecursively(writer);
                /*
                ParseIdentifier identifier = this;

                while (identifier != null)
                {
                    identifier.WriteIdentifierInfoRecursively(writer, 0);

                    identifier = identifier.NextIdentifier;
                }*/
            }

            return strB.ToString();
        }

        private void WriteIdentifierInfoRecursively(StringWriter writer, int indent = 0)
        {
            ParseIdentifier identifier = this;

            while (identifier != null)
            {
                identifier.WriteIdentifierInfo(writer);

                indent++;

                if (identifier.SubIdentifier != null)
                    identifier.SubIdentifier.WriteIdentifierInfoRecursively(writer, indent);

                indent--;

                identifier = identifier.NextIdentifier;
            }
        }

        private void WriteIdentifierInfo(StringWriter writer, int indent = 0)
        {
            string tab = new string(' ', indent);

            writer.WriteLine("{0}'{1}': OpID={2}, Position={3}", tab, Identifier, OperationID, Position);
        }

        internal ParseIdentifier()
        {
        }

        internal ParseIdentifier(Operation_IDs oID, string strID, int nPos, ParseIdentifier prev)
        {
            this.operationID = oID;
            this.identifier = strID;
            this.position = nPos;
            this.prevIdentifier = prev;
        }
    }

    /// <summary>
    /// Ϊ���ʽʶ������װ���쳣
    /// </summary>
    /// <remarks>
    /// ��װ�Ľ��������ڱ��ʽ���������лᱨ���쳣����ʾ��Ϣ��������ԭ�򡢳���λ��
    /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Expression\ExpressionParserTest.cs" region="parse_error" lang="cs" />
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable"), 
    System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly")]
    public sealed class ParsingException : System.Exception
    {
        private readonly ParseError reason = ParseError.peNone;
        private readonly int position = -1;

        /// <summary>
        /// ���캯�������ݴ������͡�����λ�ù����쳣
        /// </summary>
        /// <param name="pe">��������</param>
        /// <param name="nPosition">����λ��</param>
        /// <param name="strMsg">������Ϣ</param>
        public ParsingException(ParseError pe, int nPosition, string strMsg)
            : base(strMsg)
        {
            this.reason = pe;
            this.position = nPosition;
        }

        /// <summary>
        /// ����ԭ��
        /// </summary>
        public ParseError Reason
        {
            get
            {
                return this.reason;
            }
        }

        /// <summary>
        /// ����λ��
        /// </summary>
        public int Position
        {
            get
            {
                return this.position;
            }
        }

        /// <summary>
        /// ����һ���µı��ʽʶ���쳣
        /// </summary>
        /// <param name="pe">����ԭ��</param>
        /// <param name="nPosition">����λ��</param>
        /// <param name="strParams">�ڴ�����Ϣ�еĲ���</param>
        /// <returns>���ʽʶ���쳣����</returns>
        /// <remarks>
        /// �Ա��ʽģ���в����Ĵ�����з�װ
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Expression\ExpressionParserTest.cs" region="parse_error" lang="cs" title="�쳣�ķ�װ" />
        /// </remarks>
        static public ParsingException NewParsingException(ParseError pe, int nPosition, params string[] strParams)
        {
            string strText = ExpressionParserRes.ResourceManager.GetString(pe.ToString());
            strText = string.Format(strText, strParams);

            if (nPosition >= 0)
                strText = string.Format(ExpressionParserRes.position, nPosition + 1) + ", " + strText;

            return new ParsingException(pe, nPosition, strText);
        }
    }

    /// <summary>
    /// �û��Զ��庯����ί�ж���
    /// </summary>
    /// <param name="funcName">�û��Զ��庯����</param>
    /// <param name="arrParams">��������</param>
    /// <param name="callerContext">����������</param>
    /// <returns>������</returns>
    /// <remarks>
    /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Expression\ExpressionParserTest.cs" region="UserDefinedFucntions" lang="cs" title="�û��Զ��庯���ķ���" />
    /// </remarks>
    public delegate object CalculateUserFunction(string funcName, ParamObjectCollection arrParams, object callerContext);
}

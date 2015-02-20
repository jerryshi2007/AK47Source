#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	ExpressionParser.cs
// Remark	：	对表达式进行解析的类
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    龚文芳	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Collections.Generic;

namespace MCS.Library.Expression
{
    /// <summary>
    /// 表达式识别类
    /// </summary>
    /// <remarks>
    /// 对表达式进行解析的类
    /// </remarks>
    public sealed partial class ExpressionParser
    {
        #region 构造函数
        private ExpressionParser()
        {
        }

        #endregion

        #region private define
        #region private variable define
        private static ExpressionParser instance = new ExpressionParser();
        #endregion

        #region ParsingContext
        private class ParsingContext
        {
            private int position = 0;
            private bool outputIdentifiers = false;
            private ParseIdentifier identifiers = null;

            private char[] expressionChars = null;

            private ParseIdentifier currentIdentifier = null;
            private ParseIdentifier parentIdentifier = null;

            public ParsingContext(string expression)
            {
                this.expressionChars = new Char[expression.Length + 1];

                expression.CopyTo(0, this.expressionChars, 0, expression.Length);
                this.expressionChars[expression.Length] = '\0';
            }

            public char CurrentChar
            {
                get { return this.expressionChars[this.position]; }
            }

            public ParseIdentifier CurrentIdentifier
            {
                get { return this.currentIdentifier; }
                set { this.currentIdentifier = value; }
            }

            public ParseIdentifier ParentIdentifier
            {
                get { return this.parentIdentifier; }
                set { this.parentIdentifier = value; }
            }

            public char[] ExpressionChars
            {
                get { return this.expressionChars; }
            }

            public int Position
            {
                get { return this.position; }
                set { this.position = value; }
            }

            public bool OutputIdentifiers
            {
                get { return this.outputIdentifiers; }
                set { this.outputIdentifiers = value; }
            }

            public ParseIdentifier Identifiers
            {
                get { return this.identifiers; }
                set { this.identifiers = value; }
            }
        }
        #endregion ParsingContext

        #region private function define

        /// <summary>
        /// 开始分析一段表达式
        /// </summary>
        /// <returns></returns>
        private ExpTreeNode DoExpression(ParsingContext context)
        {
            return DoLogicalOR(context);
        }

        private static ExpTreeNode DoLogical_AND_OR(ParsingContext context, char chSensitive, Operation_IDs oID, Func<ParsingContext, ExpTreeNode> doNextOP)
        {
            ExpTreeNode node = null;
            ExpTreeNode node1 = doNextOP(context);
            ExpTreeNode node2 = null;

            while (context.CurrentChar == chSensitive)
            {
                int nPos = context.Position;
                char op = context.ExpressionChars[context.Position++];

                if (op == chSensitive)
                {
                    OutputID(context, oID, chSensitive.ToString() + chSensitive.ToString(), nPos);
                    context.Position++;
                }

                node2 = doNextOP(context);

                node = NewTreeNode(context, node1, node2, oID, nPos);

                node1 = node;
            }

            return node1;
        }

        private ExpTreeNode DoLogicalOR(ParsingContext context)
        {
            return DoLogical_AND_OR(context, '|', Operation_IDs.OI_LOGICAL_OR, DoLogicalAND);
        }

        /// <summary>
        /// 逻辑与
        /// </summary>
        /// <returns></returns>
        private ExpTreeNode DoLogicalAND(ParsingContext context)
        {
            return DoLogical_AND_OR(context, '&', Operation_IDs.OI_LOGICAL_AND, DoLogicalOP);
        }

        /// <summary>
        /// 逻辑比较运算
        /// </summary>
        /// <returns></returns>
        private ExpTreeNode DoLogicalOP(ParsingContext context)
        {
            ExpTreeNode node = null;
            ExpTreeNode node1 = DoAddSub(context);
            ExpTreeNode node2 = null;

            char op = context.CurrentChar;

            string strID = string.Empty;

            while (op == '>' || op == '<' || op == '=')
            {
                int nPos = context.Position;

                Operation_IDs oID = Operation_IDs.OI_NONE;

                if (context.ExpressionChars[++context.Position] == '=')
                {
                    switch (op)
                    {
                        case '>':	//>=
                            oID = Operation_IDs.OI_GREATEQUAL;
                            strID = ">=";
                            break;
                        case '<':	//<=
                            oID = Operation_IDs.OI_LESSEQUAL;
                            strID = "<=";
                            break;
                        case '=':	//==
                            oID = Operation_IDs.OI_EQUAL;
                            strID = "==";
                            break;
                        default: throw ParsingException.NewParsingException(ParseError.peInvalidOperator,
                                        context.Position, op.ToString());
                    }

                    context.Position++;
                }
                else
                {
                    if (context.CurrentChar == '>')
                    {
                        if (op == '<')	//<>
                        {
                            strID = "<>";
                            oID = Operation_IDs.OI_NOT_EQUAL;
                            context.Position++;
                        }
                        else
                            throw ParsingException.NewParsingException(ParseError.peInvalidOperator,
                                    context.Position, op.ToString());
                    }
                    else
                    {
                        switch (op)
                        {
                            case '>':
                                oID = Operation_IDs.OI_GREAT;
                                strID = ">";
                                break;
                            case '<':
                                oID = Operation_IDs.OI_LESS;
                                strID = "<";
                                break;
                            default:
                                throw ParsingException.NewParsingException(ParseError.peInvalidOperator,
                                     context.Position, op.ToString());
                        }
                    }
                }

                OutputID(context, oID, strID, nPos);

                node2 = DoAddSub(context);

                node = NewTreeNode(context, node1, node2, oID, nPos);

                node1 = node;

                op = context.CurrentChar;
            }

            return node1;
        }

        /// <summary>
        /// 处理加减运算
        /// </summary>
        /// <returns></returns>
        private ExpTreeNode DoAddSub(ParsingContext context)
        {
            ExpTreeNode node = null;
            ExpTreeNode node1 = DoMulDiv(context);
            ExpTreeNode node2 = null;

            char ch = context.CurrentChar;
            while (ch == '-' || ch == '+')
            {
                Operation_IDs oID = Operation_IDs.OI_NONE;

                oID = (ch == '-') ? Operation_IDs.OI_MINUS : Operation_IDs.OI_ADD;

                OutputID(context, oID, ch.ToString(), context.Position);

                int nPos = context.Position;

                context.Position++;

                node2 = DoMulDiv(context);

                node = NewTreeNode(context, node1, node2, oID, nPos);

                node1 = node;

                ch = context.CurrentChar;
            }

            return node1;
        }

        /// <summary>
        /// 处理乘除运算
        /// </summary>
        /// <returns></returns>
        private ExpTreeNode DoMulDiv(ParsingContext context)
        {
            ExpTreeNode node1 = DoSgOP(context);

            char ch = context.CurrentChar;
            while (ch == '*' || ch == '/')
            {
                Operation_IDs oID = Operation_IDs.OI_NONE;

                oID = (ch == '*') ? Operation_IDs.OI_MUL : Operation_IDs.OI_DIV;

                OutputID(context, oID, ch.ToString(), context.Position);

                int nPos = context.Position;

                context.Position++;
                ExpTreeNode node2 = DoSgOP(context);

                node1 = NewTreeNode(context, node1, node2, oID, nPos);

                ch = context.CurrentChar;
            }

            return node1;
        }

        /// <summary>
        /// 处理逻辑非"!"运算符
        /// </summary>
        /// <returns></returns>
        private ExpTreeNode DoSgOP(ParsingContext context)
        {
            SkipSpaces(context);

            ExpTreeNode node = null;

            char ch = context.CurrentChar;

            if (ch == '!')
            {
                OutputID(context, Operation_IDs.OI_NOT, "!", context.Position);

                int nPos = context.Position;

                context.Position++;

                ExpTreeNode node2 = DoSgOP(context);
                node = NewTreeNode(context, null, node2, Operation_IDs.OI_NOT, nPos);
            }
            else
                node = DoFactor(context);

            return node;
        }

        /// <summary>
        /// 处理各种系数、负数、括号等
        /// </summary>
        /// <returns></returns>
        private ExpTreeNode DoFactor(ParsingContext context)
        {
            SkipSpaces(context);

            ExpTreeNode node = null;

            char ch = context.CurrentChar;

            int nPos = context.Position;

            switch (ch)
            {
                case '-':   //处理负号
                    OutputID(context, Operation_IDs.OI_NEG, "-", context.Position);

                    context.Position++;

                    ExpTreeNode node2 = DoExpression(context);

                    ExpTreeNode left = NewTreeNode(context);
                    left.OperationID = Operation_IDs.OI_NEG;
                    left.Value = (decimal)-1;
                    left.Position = nPos;

                    node = NewTreeNode(context, left, node2, Operation_IDs.OI_MUL, nPos);
                    break;
                case '(':
                    OutputID(context, Operation_IDs.OI_LBRACKET, "(", context.Position);
                    context.Position++;
                    node = DoExpression(context);

                    SkipSpaces(context);

                    if (context.CurrentChar != ')')
                        throw ParsingException.NewParsingException(ParseError.peCharExpected, context.Position, ")");
                    else
                        OutputID(context, Operation_IDs.OI_RBRACKET, ")", context.Position);

                    context.Position++;
                    break;
                default:
                    node = DoIdentifier(context);
                    break;
            }

            SkipSpaces(context);

            return node;
        }

        /// <summary>
        /// 处理各种数字、标识符、自定义函数、字符串等
        /// </summary>
        /// <returns></returns>
        private ExpTreeNode DoIdentifier(ParsingContext context)
        {
            ExpTreeNode result = null;

            SkipSpaces(context);

            char ch = context.CurrentChar;

            if (ch != '\0')
            {
                switch (ch)
                {
                    case '#':   //datetime
                        result = DoDatetime(context);
                        break;
                    case '"':   //string
                        result = DoString(context);
                        break;
                    default:
                        if (Char.IsDigit(ch) || ch == '.')
                            result = DoNumber(context);
                        else
                            if (Char.IsLetter(ch) || ch == '_')
                                result = DoFunctionID(context);
                            else
                                throw ParsingException.NewParsingException(ParseError.peInvalidOperator, context.Position, ch.ToString());
                        break;
                }

                SkipSpaces(context);
            }

            return result;
        }

        private static ExpTreeNode DoDatetime(ParsingContext context)
        {
            int nPos = context.Position;
            char ch = context.ExpressionChars[++context.Position];

            StringBuilder strB = new StringBuilder(256);

            strB.Append("#");

            while (ch != '\0')
            {
                strB.Append(ch);

                if (ch == '#')
                {
                    context.Position++;
                    break;
                }

                ch = context.ExpressionChars[++context.Position];
            }

            if (ch == '\0')
                throw ParsingException.NewParsingException(ParseError.peCharExpected, context.Position, "#");

            ExpTreeNode node = NewTreeNode(context);

            node.Position = nPos;
            node.OperationID = Operation_IDs.OI_DATETIME;

            try
            {
                string strID = strB.ToString();
                node.Value = DateTime.Parse(strID);

                OutputID(context, Operation_IDs.OI_DATETIME, strID, nPos);

                return node;
            }
            catch (System.FormatException)
            {
                throw ParsingException.NewParsingException(ParseError.peFormatError, nPos);
            }
        }

        private static ExpTreeNode DoString(ParsingContext context)
        {
            int nPos = context.Position;
            char ch = context.ExpressionChars[++context.Position];

            StringBuilder strB = new StringBuilder(256);
            StringBuilder strIDB = new StringBuilder(256);

            strIDB.Append('"');

            while (ch != '\0')
            {
                if (ch != '"')
                {
                    strB.Append(ch);
                    strIDB.Append(ch);
                    context.Position++;
                }
                else
                    if (context.ExpressionChars[context.Position + 1] == '"')
                    {
                        strB.Append('"');
                        strIDB.Append("\"\"");
                        context.Position += 2;
                    }
                    else
                    {
                        strIDB.Append('"');
                        context.Position++;
                        break;
                    }

                ch = context.CurrentChar;
            }

            if (ch == '\0')
                throw ParsingException.NewParsingException(ParseError.peCharExpected, context.Position, "\"");

            string strID = strIDB.ToString();

            OutputID(context, Operation_IDs.OI_STRING, strID, nPos);
            ExpTreeNode node = NewTreeNode(context);

            node.Position = nPos;
            node.OperationID = Operation_IDs.OI_STRING;
            node.Value = strB.ToString();

            return node;
        }

        private static ExpTreeNode DoNumber(ParsingContext context)
        {
            int nPos = context.Position;
            char ch = context.CurrentChar;

            while (Char.IsDigit(ch) || (ch == '.'))
                ch = context.ExpressionChars[++context.Position];

            ExpTreeNode node = NewTreeNode(context);

            node.Position = nPos;
            node.OperationID = Operation_IDs.OI_NUMBER;

            string ns = new String(context.ExpressionChars, nPos, context.Position - nPos);
            node.Value = decimal.Parse(ns);

            OutputID(context, Operation_IDs.OI_NUMBER, ns, nPos);

            return node;
        }

        private ExpTreeNode DoFunctionID(ParsingContext context)
        {
            int nPos = context.Position;
            char ch = context.CurrentChar;

            while (char.IsLetterOrDigit(ch) || ch == '_' || ch == '.')
                ch = context.ExpressionChars[++context.Position];

            string strID = new string(context.ExpressionChars, nPos, context.Position - nPos);

            return DoFunction(context, strID);
        }

        private ExpTreeNode DoFunction(ParsingContext context, string strId)
        {
            ExpTreeNode node = null;

            if (string.Compare(strId, "true", true) == 0 || string.Compare(strId, "false", true) == 0)
            {
                node = NewTreeNode(context);
                node.Position = context.Position - strId.Length;
                node.OperationID = Operation_IDs.OI_BOOLEAN;
                node.Value = bool.Parse(strId.ToLower());

                OutputID(context, Operation_IDs.OI_BOOLEAN, strId, node.Position);
            }
            else
                node = GetFunctionNode(context, Operation_IDs.OI_USERDEFINE, strId);

            return node;
        }

        private ExpTreeNode GetFunctionNode(ParsingContext context, Operation_IDs funcID, string strID)
        {
            ExpTreeNode node = null;
            ExpTreeNode nodeTemp = null;
            List<ExpTreeNode> paramBase = new List<ExpTreeNode>(4);

            int nStartFunction = context.Position - strID.Length;

            OutputID(context, Operation_IDs.OI_USERDEFINE, strID, nStartFunction);

            if (context.CurrentChar == '(')	//有参数
            {
                OutputIDToSubLevel(context);
                OutputID(context, Operation_IDs.OI_LBRACKET, "(", context.Position);

                do
                {
                    context.Position++;
                    SkipSpaces(context);

                    nodeTemp = DoExpression(context);

                    if (nodeTemp != null)
                    {
                        paramBase.Add(nodeTemp);

                        SkipSpaces(context);
                    }
                    else
                        break;

                    if (context.CurrentChar == ',')
                        OutputID(context, Operation_IDs.OI_COMMA, ",", context.Position);
                }
                while (context.CurrentChar == ',');

                if (context.CurrentChar == ')')
                {
                    OutputID(context, Operation_IDs.OI_RBRACKET, ")", context.Position);
                    OutputIDToParentLevel(context);

                    context.Position++;
                    node = NewTreeNode(context);
                    node.Position = nStartFunction;
                    node.Params = paramBase;
                    node.OperationID = funcID;

                    if (funcID == Operation_IDs.OI_USERDEFINE)
                        node.FunctionName = strID;
                }
                else
                    throw ParsingException.NewParsingException(ParseError.peCharExpected, context.Position, ")");

                SkipSpaces(context);
            }
            else	//没有参数
            {
                node = NewTreeNode(context);
                node.Position = nStartFunction;
                node.Params = paramBase;
                node.OperationID = funcID;

                if (funcID == Operation_IDs.OI_USERDEFINE)
                    node.FunctionName = strID;
            }

            return node;
        }

        private static void OutputID(ParsingContext context, Operation_IDs oID, string strID, int nPos)
        {
            if (context.OutputIdentifiers)
            {
                ParseIdentifier pi = new ParseIdentifier(oID, strID, nPos, context.CurrentIdentifier);

                if (context.CurrentIdentifier == null)
                {
                    if (context.ParentIdentifier == null)
                        context.Identifiers = pi;
                    else
                        context.ParentIdentifier.SubIdentifier = pi;
                }
                else
                    context.CurrentIdentifier.NextIdentifier = pi;

                pi.ParentIdentifier = context.ParentIdentifier;
                context.CurrentIdentifier = pi;
            }
        }

        private static void OutputIDToSubLevel(ParsingContext context)
        {
            context.ParentIdentifier = context.CurrentIdentifier;
            context.CurrentIdentifier = null;
        }

        private static void OutputIDToParentLevel(ParsingContext context)
        {
            if (context.ParentIdentifier != null)
            {
                context.CurrentIdentifier = context.ParentIdentifier;
                context.ParentIdentifier = context.ParentIdentifier.ParentIdentifier;
            }
        }

        /// <summary>
        /// 生成一个新的二叉树节点
        /// </summary>
        private static ExpTreeNode NewTreeNode(ParsingContext context)
        {
            ExpTreeNode node = new ExpTreeNode();

            node.Position = context.Position;

            return node;
        }

        /// <summary>
        /// 生成一个新的二叉树节点
        /// </summary>
        /// <param name="context">运行上下文</param>
        /// <param name="left">左子树</param>
        /// <param name="right">右子树</param>
        /// <param name="oID">操作类型</param>
        /// <returns></returns>
        private static ExpTreeNode NewTreeNode(ParsingContext context, ExpTreeNode left, ExpTreeNode right, Operation_IDs oID)
        {
            ExpTreeNode node = NewTreeNode(context);

            node.Left = left;
            node.Right = right;
            node.OperationID = oID;

            return node;
        }

        private static ExpTreeNode NewTreeNode(ParsingContext context, ExpTreeNode left, ExpTreeNode right, Operation_IDs oID, int nPosition)
        {
            ExpTreeNode node = NewTreeNode(context, left, right, oID);

            node.Position = nPosition;

            return node;
        }

        private static void SkipSpaces(ParsingContext context)
        {
            while (context.CurrentChar <= ' ' && context.CurrentChar != '\0')
                context.Position++;
        }
        #endregion
        #endregion
    }
}

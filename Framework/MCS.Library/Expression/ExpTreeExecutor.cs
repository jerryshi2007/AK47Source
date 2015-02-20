#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	ExpTreeExecutor.cs
// Remark	：	语法分析Tree的计算类
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    龚文芳	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;
using MCS.Library.Properties;

namespace MCS.Library.Expression
{
    /// <summary>
    /// 语法分析Tree的计算类
    /// </summary>
    internal class ExpTreeExecutor
    {
        public static readonly ExpTreeExecutor Instance = new ExpTreeExecutor();

        private ExpTreeExecutor()
        {
        }

        /// <summary>
        /// 获取树结点值
        /// </summary>
        /// <param name="tree">二叉树节点</param>
        /// <param name="calcUDF">用户自定义函数的委托</param>
        /// <param name="callerContext">调用者上下文</param>
        /// <param name="optimize">最优化选项</param>
        /// <returns>树结点值</returns>
        public object GetValue(ExpTreeNode tree, CalculateUserFunction calcUDF, object callerContext, bool optimize)
        {
            object result = null;

            if (tree != null)
            {
                CalculateContext calcContext = new CalculateContext();

                calcContext.Optimize = optimize;
                calcContext.CallerContxt = callerContext;
                calcContext.CalculateUserFunction = calcUDF;

                result = VExp(tree, calcContext);
            }

            return result;
        }

        /// <summary>
        /// 获取树结点值
        /// </summary>
        /// <param name="tree">二叉树节点</param>
        /// <param name="builtInFunctionsWrapper">包含内置函数的实现类</param>
        /// <param name="callerContext">调用者上下文</param>
        /// <param name="optimize">最优化选项</param>
        /// <returns>树结点值</returns>
        public object GetValue(ExpTreeNode tree, object builtInFunctionsWrapper, object callerContext, bool optimize)
        {
            object result = null;

            if (tree != null)
            {
                CalculateContext calcContext = new CalculateContext();

                calcContext.Optimize = optimize;
                calcContext.CallerContxt = callerContext;
                calcContext.BuiltInFunctionsWrapper = builtInFunctionsWrapper;

                result = VExp(tree, calcContext);
            }

            return result;
        }

        private object VExp(ExpTreeNode node, CalculateContext calcContext)
        {
            object oValue = null;

            if (node != null)
            {
                try
                {
                    switch (node.OperationID)
                    {
                        case Operation_IDs.OI_NUMBER:
                        case Operation_IDs.OI_STRING:
                        case Operation_IDs.OI_NEG:
                        case Operation_IDs.OI_BOOLEAN:
                        case Operation_IDs.OI_DATETIME:
                            oValue = node.Value;
                            break;
                        case Operation_IDs.OI_ADD:
                            oValue = AddOP(VExp(node.Left, calcContext), VExp(node.Right, calcContext), node.Position);
                            break;
                        case Operation_IDs.OI_MINUS:
                            {
                                object p1 = VExp(node.Left, calcContext);
                                object p2 = VExp(node.Right, calcContext);

                                CheckOperandNull(p1, p2, node.Position);
                                oValue = NToD(p1) - NToD(p2);
                            }
                            break;
                        case Operation_IDs.OI_MUL:
                            {
                                object p1 = VExp(node.Left, calcContext);
                                object p2 = VExp(node.Right, calcContext);

                                CheckOperandNull(p1, p2, node.Position);
                                oValue = NToD(p1) * NToD(p2);
                            }
                            break;
                        case Operation_IDs.OI_DIV:
                            {
                                object p1 = VExp(node.Left, calcContext);
                                object p2 = VExp(node.Right, calcContext);

                                CheckOperandNull(p1, p2, node.Position);

                                if (NToD(p2) == 0.0M)
                                {
                                    throw ParsingException.NewParsingException(ParseError.peFloatOverflow, node.Position);
                                }

                                oValue = NToD(p1) / NToD(p2);
                            }
                            break;
                        case Operation_IDs.OI_LOGICAL_OR:
                            {
                                oValue = (bool)VExp(node.Left, calcContext);
                                object oRight = (bool)false;

                                if ((bool)oValue == false)
                                    oRight = VExp(node.Right, calcContext);

                                CheckOperandNull(oValue, oRight, node.Position);
                                oValue = (bool)oValue || (bool)oRight;
                            }
                            break;
                        case Operation_IDs.OI_LOGICAL_AND:
                            {
                                oValue = (bool)VExp(node.Left, calcContext);
                                object oRight = (bool)true;

                                if ((bool)oValue == true)
                                    oRight = VExp(node.Right, calcContext);

                                CheckOperandNull(oValue, oRight, node.Position);
                                oValue = (bool)oValue && (bool)oRight;
                            }
                            break;
                        case Operation_IDs.OI_NOT:
                            oValue = VExp(node.Right, calcContext);
                            CheckOperandNull(oValue, node.Position);
                            oValue = !(bool)oValue;
                            break;
                        case Operation_IDs.OI_GREAT:
                            oValue = CompareGreatOP(VExp(node.Left, calcContext), VExp(node.Right, calcContext), node.Position);
                            break;
                        case Operation_IDs.OI_GREATEQUAL:
                            oValue = CompareGreatEqualOP(VExp(node.Left, calcContext), VExp(node.Right, calcContext), node.Position);
                            break;
                        case Operation_IDs.OI_LESS:
                            oValue = CompareLessOP(VExp(node.Left, calcContext), VExp(node.Right, calcContext), node.Position);
                            break;
                        case Operation_IDs.OI_LESSEQUAL:
                            oValue = CompareLessEqualOP(VExp(node.Left, calcContext), VExp(node.Right, calcContext), node.Position);
                            break;
                        case Operation_IDs.OI_NOT_EQUAL:
                            oValue = CompareNotEqualOP(VExp(node.Left, calcContext), VExp(node.Right, calcContext), node.Position);
                            break;
                        case Operation_IDs.OI_EQUAL:
                            oValue = CompareEqualOP(VExp(node.Left, calcContext), VExp(node.Right, calcContext), node.Position);
                            break;
                        case Operation_IDs.OI_USERDEFINE:
                            {
                                oValue = CalculateFunction(node, node.Params, calcContext);

                                if (oValue == null)
                                {
                                    ParamObjectCollection funcParams = GetParams(node.Params, calcContext);

                                    oValue = CalculateFunctionWithParameters(node.FunctionName, funcParams, calcContext);

                                    if (oValue == null)
                                        oValue = calcContext.GetUserFunctionValue(node.FunctionName, funcParams);
                                }
                            }
                            break;
                        default:
                            throw ParsingException.NewParsingException(
                                ParseError.peInvalidOperator,
                                node.Position,
                                EnumItemDescriptionAttribute.GetAttribute(node.OperationID).ShortName);
                    }
                }
                catch (System.InvalidCastException)
                {
                    throw ParsingException.NewParsingException(ParseError.peTypeMismatch, node.Position);
                }
            }

            return oValue;
        }

        private object CalculateFunction(ExpTreeNode funcNode, List<ExpTreeNode> arrParams, CalculateContext calcContext)
        {
            object oValue = null;

            switch (funcNode.FunctionName.ToLower())
            {
                case "in":
                    oValue = this.DoInFunction(funcNode, arrParams, calcContext);
                    break;
                case "iif":
                    oValue = this.DoIIFFunction(funcNode, arrParams, calcContext);
                    break;
            }

            return oValue;
        }

        /// <summary>
        /// 计算所有参数都已经准备好了的函数
        /// </summary>
        /// <param name="strFuncName"></param>
        /// <param name="arrParams"></param>
        /// <param name="calcContext"></param>
        /// <returns></returns>
        private static object CalculateFunctionWithParameters(string strFuncName, ParamObjectCollection arrParams, CalculateContext calcContext)
        {
            object oValue = null;

            try
            {
                switch (strFuncName.ToLower())
                {
                    case "now":
                        oValue = DateTime.Now;
                        break;
                    case "today":
                        oValue = DateTime.Today;
                        break;
                    case "dateinterval.day":
                        oValue = "d";
                        break;
                    case "dateinterval.hour":
                        oValue = "h";
                        break;
                    case "dateinterval.minute":
                        oValue = "n";
                        break;
                    case "dateinterval.second":
                        oValue = "s";
                        break;
                    case "dateinterval.millisecond":
                        oValue = "ms";
                        break;
                    case "datediff":
                        oValue = DoDateDiff(arrParams);
                        break;
                    case "mindate":
                        oValue = DateTime.MinValue;
                        break;
                    case "maxdate":
                        oValue = DateTime.MaxValue;
                        break;
                    default:
                        {
                            if (calcContext.CalculateUserFunction != null)
                                oValue = calcContext.CalculateUserFunction(strFuncName, arrParams, calcContext.CallerContxt);

                            break;
                        }
                }

                return oValue;
            }
            catch (ParsingException)
            {
                throw;
            }
            catch (System.Exception ex)
            {
                throw new SystemSupportException(string.Format(ExpressionParserRes.FunctionError, strFuncName, ex.Message));
            }
        }

        private object DoInFunction(ExpTreeNode funcNode, IReadOnlyList<ExpTreeNode> arrParams, CalculateContext calcContext)
        {
            bool result = false;

            if (arrParams.Count > 0)
            {
                object sourceData = this.VExp(arrParams[0], calcContext);

                if (sourceData != null)
                {
                    for (int i = 1; i < arrParams.Count; i++)
                    {
                        object itemValue = this.VExp(arrParams[i], calcContext);

                        if (itemValue != null)
                        {
                            if ((bool)CompareEqualOP(sourceData, itemValue, 0))
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                }
            }

            return result;
        }

        private object DoIIFFunction(ExpTreeNode funcNode, IReadOnlyList<ExpTreeNode> arrParams, CalculateContext calcContext)
        {
            if (arrParams.Count != 3)
                throw ParsingException.NewParsingException(ParseError.peInvalidParam,
                                        funcNode.Position, funcNode.FunctionName, "3");
            object result = false;

            if (arrParams.Count > 0)
            {
                object sourceData = this.VExp(arrParams[0], calcContext);

                if (sourceData != null)
                {
                    bool sourceCondition = (bool)DataConverter.ChangeType(sourceData, typeof(bool));

                    if (sourceCondition)
                        result = this.VExp(arrParams[1], calcContext);
                    else
                        result = this.VExp(arrParams[2], calcContext);
                }
            }

            return result;
        }

        private static object DoInFunction(ParamObjectCollection arrParams, CalculateContext calcContext)
        {
            bool result = false;

            if (arrParams.Count > 0)
            {
                object sourceData = arrParams[0].Value;

                for (int i = 1; i < arrParams.Count; i++)
                {
                    if ((bool)CompareEqualOP(sourceData, arrParams[i].Value, 0))
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        private static object DoDateDiff(ParamObjectCollection arrParams)
        {
            arrParams.CheckParamsLength(3);
            arrParams[0].CheckParameterType<string>();
            arrParams[1].CheckParameterType<DateTime>();
            arrParams[2].CheckParameterType<DateTime>();

            DateTime startTime = (DateTime)arrParams[1].Value;
            DateTime endTime = (DateTime)arrParams[2].Value;

            TimeSpan ts = endTime - startTime;

            double result = 0;

            string intervalType = arrParams[0].Value.ToString().ToLower();

            switch (intervalType)
            {
                case "d":
                    result = ts.TotalDays;
                    break;
                case "h":
                    result = ts.TotalHours;
                    break;
                case "n":
                    result = ts.TotalMinutes;
                    break;
                case "s":
                    result = ts.TotalSeconds;
                    break;
                case "ms":
                    result = ts.TotalMilliseconds;
                    break;
                default:
                    throw new SystemSupportException(string.Format(ExpressionParserRes.InvalidDateDiffType, intervalType));
            }

            return Math.Ceiling(Convert.ToDecimal(result));
        }

        private ParamObjectCollection GetParams(List<ExpTreeNode> arrParams, CalculateContext calcContext)
        {
            List<ParamObject> list = new List<ParamObject>();

            for (int i = 0; i < arrParams.Count; i++)
            {
                ExpTreeNode node = (ExpTreeNode)arrParams[i];

                list.Add(new ParamObject(VExp(node, calcContext), node.Position, i));
            }

            return new ParamObjectCollection(list);
        }

        private static void CheckOperandNull(object p, int nPos)
        {
            if (p == null)
                throw ParsingException.NewParsingException(ParseError.peNeedOperand, nPos);
        }

        private static void CheckOperandNull(object p1, object p2, int nPos)
        {
            if (p1 == null || p2 == null)
                throw ParsingException.NewParsingException(ParseError.peNeedOperand, nPos);
        }

        private static object AddOP(object p1, object p2, int nPos)
        {
            CheckOperandNull(p1, p2, nPos);
            object result;

            if (p1 is System.String || p2 is System.String)
                result = p1.ToString() + p2.ToString();
            else
                result = NToD(p1) + NToD(p2);

            return result;
        }

        private static object CompareGreatOP(object p1, object p2, int nPos)
        {
            CheckOperandNull(p1, p2, nPos);
            bool result;

            if (p1 is System.DateTime || p2 is System.DateTime)
                result = (DateTime)DataConverter.ChangeType(p1, typeof(DateTime)) > (DateTime)DataConverter.ChangeType(p2, typeof(DateTime));
            else
                if (p1 is System.String || p2 is System.String)
                    result = p1.ToString().CompareTo(p2.ToString()) > 0;
                else
                    result = NToD(p1) > NToD(p2);

            return result;
        }

        private static object CompareLessOP(object p1, object p2, int nPos)
        {
            CheckOperandNull(p1, p2, nPos);
            bool result;

            if (p1 is System.DateTime || p2 is System.DateTime)
                result = (DateTime)DataConverter.ChangeType(p1, typeof(DateTime)) < (DateTime)DataConverter.ChangeType(p2, typeof(DateTime));
            else
                if (p1 is System.String || p2 is System.String)
                    result = p1.ToString().CompareTo(p2.ToString()) < 0;
                else
                    result = NToD(p1) < NToD(p2);

            return result;
        }

        private static object CompareEqualOP(object p1, object p2, int nPos)
        {
            CheckOperandNull(p1, p2, nPos);
            bool result;

            if (p1 is System.DateTime || p2 is System.DateTime)
                result = (DateTime)DataConverter.ChangeType(p1, typeof(DateTime)) == (DateTime)DataConverter.ChangeType(p2, typeof(DateTime));
            else
                if (p1 is System.String || p2 is System.String)
                    result = p1.ToString() == p2.ToString();
                else
                    result = NToD(p1) == NToD(p2);

            return result;
        }

        private static object CompareNotEqualOP(object p1, object p2, int nPos)
        {
            CheckOperandNull(p1, p2, nPos);
            bool result;

            if (p1 is System.DateTime || p2 is System.DateTime)
                result = (DateTime)DataConverter.ChangeType(p1, typeof(DateTime)) != (DateTime)DataConverter.ChangeType(p2, typeof(DateTime));
            else
                if (p1 is System.String || p2 is System.String)
                    result = p1.ToString() != p2.ToString();
                else
                    result = NToD(p1) != NToD(p2);

            return result;
        }

        private static object CompareGreatEqualOP(object p1, object p2, int nPos)
        {
            CheckOperandNull(p1, p2, nPos);
            bool result;

            if (p1 is System.DateTime || p2 is System.DateTime)
                result = (DateTime)DataConverter.ChangeType(p1, typeof(DateTime)) >= (DateTime)DataConverter.ChangeType(p2, typeof(DateTime));
            else
                if (p1 is System.String || p2 is System.String)
                    result = p1.ToString().CompareTo(p2.ToString()) >= 0;
                else
                    result = NToD(p1) >= NToD(p2);

            return result;
        }

        private static object CompareLessEqualOP(object p1, object p2, int nPos)
        {
            CheckOperandNull(p1, p2, nPos);
            bool result;

            if (p1 is System.DateTime || p2 is System.DateTime)
                result = (DateTime)DataConverter.ChangeType(p1, typeof(DateTime)) <= (DateTime)DataConverter.ChangeType(p2, typeof(DateTime));
            else
                if (p1 is System.String || p2 is System.String)
                    result = p1.ToString().CompareTo(p2.ToString()) <= 0;
                else
                    result = NToD(p1) <= NToD(p2);

            return result;
        }

        /// <summary>
        /// 将数字转换为Decimal
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static decimal NToD(object p)
        {
            return Convert.ToDecimal(p);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.Expression
{
    public sealed partial class ExpressionParser
    {
        /// <summary>
        /// 分析表达式，
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>分析结果</returns>
        /// <remarks>
        /// 对传入的表达式进行分析
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Expression\ExpressionParserTest.cs" region="parse" lang="cs" title="调用分析表达式的函数" />
        /// </remarks>
        public static ParseResult Parse(string expression)
        {
            ParseResult result = null;

            if (string.IsNullOrEmpty(expression) == false)
            {
                ParsingContext context = new ParsingContext(expression);

                context.OutputIdentifiers = true;
                ExpTreeNode tree = ExpressionParser.instance.DoExpression(context);

                if (context.CurrentChar != '\0')
                    throw ParsingException.NewParsingException(ParseError.peInvalidOperator, context.Position, context.CurrentChar.ToString());

                result = new ParseResult(tree, context.Identifiers);
            }
            else
                result = new ParseResult(null, null);

            return result;
        }

        /// <summary>
        /// 计算表达式，直接获得结果
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>运算结果</returns>
        /// <remarks>
        /// 直接计算出表达式的结果
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Expression\ExpressionParserTest.cs" region="Calculate" lang="cs" title="计算表达式" />
        /// </remarks>
        public static object Calculate(string expression)
        {
            return Calculate(expression, null, null);
        }

        /// <summary>
        /// 计算表达式，直接获得结果
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="calculateUserFunction">自定义函数</param>
        /// <param name="callerContext">自定义函数上下文</param>
        /// <returns>运算值</returns>
        /// <remarks>
        /// 对包含自定义函数的表达式进行运算
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Expression\ExpressionParserTest.cs" region="Calculate" lang="cs" title="计算表达式" />
        /// </remarks>
        public static object Calculate(string expression, CalculateUserFunction calculateUserFunction, object callerContext)
        {
            return Calculate(expression, calculateUserFunction, callerContext, true);
        }

        /// <summary>
        /// 计算表达式，直接获得结果
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="builtInFunctionsWrapper">包含内置函数的实现类</param>
        /// <param name="callerContext">自定义函数上下文</param>
        /// <returns>运算值</returns>
        /// <remarks>
        /// 对包含自定义函数和自定义上下文的表达式进行运算
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Expression\ExpressionParserTest.cs" region="Calculate" lang="cs" title="计算表达式" />
        /// </remarks>
        public static object Calculate(string expression, object builtInFunctionsWrapper, object callerContext)
        {
            object result = null;
            ParseResult pr = Parse(expression);

            if (pr != null)
                result = GetTreeValue(pr.Tree, builtInFunctionsWrapper, callerContext, true);

            return result;
        }

        /// <summary>
        /// 计算表达式，直接获得结果
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="calculateUserFunction">自定义函数</param>
        /// <param name="callerContext">自定义函数上下文</param>
        /// <param name="optimize">是否进行bool运算优化，缺省为true</param>
        /// <returns>运算值</returns>
        /// <remarks>
        /// 对包含自定义函数和自定义上下文的表达式进行运算
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Expression\ExpressionParserTest.cs" region="Calculate" lang="cs" title="计算表达式" />
        /// </remarks>
        public static object Calculate(string expression, CalculateUserFunction calculateUserFunction, object callerContext, bool optimize)
        {
            object result = null;
            ParseResult pr = Parse(expression);

            if (pr != null)
                result = GetTreeValue(pr.Tree, calculateUserFunction, callerContext, optimize);

            return result;
        }

        /// <summary>
        /// 计算表达式，直接获得结果
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <param name="builtInFunctionsWrapper">包含内置函数的实现类</param>
        /// <param name="callerContext">自定义函数上下文</param>
        /// <param name="optimize">是否进行bool运算优化，缺省为true</param>
        /// <returns>运算值</returns>
        /// <remarks>
        /// 对包含自定义函数和自定义上下文的表达式进行运算
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Expression\ExpressionParserTest.cs" region="Calculate" lang="cs" title="计算表达式" />
        /// </remarks>
        public static object Calculate(string expression, object builtInFunctionsWrapper, object callerContext, bool optimize)
        {
            object result = null;
            ParseResult pr = Parse(expression);

            if (pr != null)
                result = GetTreeValue(pr.Tree, builtInFunctionsWrapper, callerContext, optimize);

            return result;
        }

        /// <summary>
        /// 根据语法解析完的Tree，计算出结果
        /// </summary>
        /// <param name="tree">语法解析树</param>
        /// <returns>结果返回值</returns>
        /// <remarks>
        /// 计算解析出的二叉树，得到运算结果
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Expression\ExpressionParserTest.cs" region="getreevalue" lang="cs" title="对解析生成的二叉树进行计算" />
        /// </remarks>
        public static object GetTreeValue(ExpTreeNode tree)
        {
            return GetTreeValue(tree, null, null, true);
        }

        /// <summary>
        /// 根据语法解析完的Tree，计算出结果
        /// </summary>
        /// <param name="tree">解析生成的二叉树</param>
        /// <param name="calculateUserFunction">用户自定义函数的实现</param>
        /// <param name="callerContext">自定义函数上下文</param>
        /// <returns>运算结果</returns>
        /// <remarks>
        ///  对含自定义函数的表达式进行解析后生成的二叉树，调用该函数进行运算得出结果值
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Expression\ExpressionParserTest.cs" region="parse" lang="cs" title="对解析生成的二叉树进行计算" />
        /// </remarks>
        public static object GetTreeValue(ExpTreeNode tree, CalculateUserFunction calculateUserFunction, object callerContext)
        {
            return GetTreeValue(tree, calculateUserFunction, callerContext, true);
        }

        /// <summary>
        /// 根据语法解析完的Tree，计算出结果
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="builtInFunctionsWrapper">包含内置函数的实现类</param>
        /// <param name="callerContext">调用者上下文</param>
        /// <returns>树结点值</returns>
        public static object GetTreeValue(ExpTreeNode tree, object builtInFunctionsWrapper, object callerContext)
        {
            return ExpTreeExecutor.Instance.GetValue(tree, builtInFunctionsWrapper, callerContext, true);
        }

        /// <summary>
        /// 根据语法解析完的Tree，计算出结果
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="calculateUserFunction">用户自定义函数的实现</param>
        /// <param name="callerContext"></param>
        /// <param name="optimize">是否进行bool运算优化，缺省为true</param>
        /// <returns></returns>
        /// <remarks>
        /// 对含自定义函数的表达式进行解析后生成的二叉树，调用该函数进行运算得出结果值
        /// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Expression\ExpressionParserTest.cs" region="parse" lang="cs" title="对解析生成的二叉树进行计算" />
        /// </remarks>
        public static object GetTreeValue(ExpTreeNode tree, CalculateUserFunction calculateUserFunction, object callerContext, bool optimize)
        {
            return ExpTreeExecutor.Instance.GetValue(tree, calculateUserFunction, callerContext, optimize);
        }

        /// <summary>
        /// 根据语法解析完的Tree，计算出结果
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="builtInFunctionsWrapper">包含内置函数的实现类</param>
        /// <param name="callerContext">调用者上下文</param>
        /// <param name="optimize">最优化选项</param>
        /// <returns>树结点值</returns>
        public static object GetTreeValue(ExpTreeNode tree, object builtInFunctionsWrapper, object callerContext, bool optimize)
        {
            return ExpTreeExecutor.Instance.GetValue(tree, builtInFunctionsWrapper, callerContext, optimize);
        }
    }
}

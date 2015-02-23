#region
// -------------------------------------------------
// Assembly	：	DeluxeWorks.Library
// FileName	：	ExpConstants.cs
// Remark	：	表达式解析的一些错误封装
// -------------------------------------------------
// VERSION  	AUTHOR		DATE			CONTENT
// 1.0		    龚文芳	    20070430		创建
// -------------------------------------------------
#endregion
using System;
using System.Text;
using System.Collections.Generic;
using MCS.Library.Core;

namespace MCS.Library.Expression
{
    /// <summary>
    /// 错误类别
    /// </summary>
    /// <remarks>
    /// 表达式解析的一些错误封装
    /// </remarks>
    public enum ParseError
    {
        /// <summary>
        /// 无异常
        /// </summary>
        peNone = 0,

        /// <summary>
        /// 非法字符
        /// </summary>
        peInvalidChar,

        /// <summary>
        /// 非法的字符串
        /// </summary>
        peInvalidString,

        /// <summary>
        /// 非法操作符
        /// </summary>
        peInvalidOperator,

        /// <summary>
        /// 类型不匹配
        /// </summary>
        peTypeMismatch,

        /// <summary>
        /// 非法的参数
        /// </summary>
        peInvalidParam,

        /// <summary>
        /// 非法的用户自定义函数返回值
        /// </summary>
        peInvalidUFValue,

        /// <summary>
        /// 语法错误
        /// </summary>
        peSyntaxError,

        /// <summary>
        /// 浮点运算溢出
        /// </summary>
        peFloatOverflow,

        /// <summary>
        /// 需要某个字符
        /// </summary>
        peCharExpected,

        /// <summary>
        /// 函数错误
        /// </summary>
        peFuncError,

        /// <summary>
        /// 需要操作数
        /// </summary>
        peNeedOperand,

        /// <summary>
        /// 格式错误
        /// </summary>
        peFormatError,

        /// <summary>
        /// 参数类型错误
        /// </summary>
        InvalidParameterType,


        ///// <summary>
        ///// 参数个数错误
        ///// </summary>
        //InvalidParameterNum, 
    }

    /// <summary>
    /// 操作类型，作为语法分析二叉树节点中的运算符标识
    /// </summary>
    //public enum ParseOperation
    public enum Operation_IDs
    {
        /// <summary>
        /// 无操作符
        /// </summary>
        [EnumItemDescription("无操作符", ShortName = "")]
        OI_NONE = 0,

        /// <summary>
        /// 非
        /// </summary>
        [EnumItemDescription("非操作符", ShortName = "!")]
        OI_NOT = 120,

        /// <summary>
        /// 加
        /// </summary>
        [EnumItemDescription("加操作符", ShortName = "+")]
        OI_ADD,

        /// <summary>
        /// 减
        /// </summary>
        [EnumItemDescription("减操作符", ShortName = "-")]
        OI_MINUS,

        /// <summary>
        /// 乘
        /// </summary>
        [EnumItemDescription("乘操作符", ShortName = "*")]
        OI_MUL,

        /// <summary>
        /// 除
        /// </summary>
        [EnumItemDescription("除操作符", ShortName = "/")]
        OI_DIV,

        /// <summary>
        /// 负号
        /// </summary>
        [EnumItemDescription("负操作符", ShortName = "-")]
        OI_NEG,

        /// <summary>
        /// 等于
        /// </summary>
        [EnumItemDescription("等于操作符", ShortName = "==")]
        OI_EQUAL,

        /// <summary>
        /// 不等于
        /// </summary>
        [EnumItemDescription("不等于操作符", ShortName = "<>")]
        OI_NOT_EQUAL,

        /// <summary>
        /// 大于
        /// </summary>
        [EnumItemDescription("大于操作符", ShortName = ">")]
        OI_GREAT,

        /// <summary>
        /// 大于等于
        /// </summary>
        [EnumItemDescription("大于等于操作符", ShortName = ">=")]
        OI_GREATEQUAL,

        /// <summary>
        /// 小于
        /// </summary>
        [EnumItemDescription("小于操作符", ShortName = "<")]
        OI_LESS,

        /// <summary>
        /// 小于等于
        /// </summary>
        [EnumItemDescription("小于等于操作符", ShortName = "<=")]
        OI_LESSEQUAL,

        /// <summary>
        ///逻辑与 
        /// </summary>
        [EnumItemDescription("逻辑与操作符", ShortName = "&&")]
        OI_LOGICAL_AND,

        /// <summary>
        /// 逻辑或
        /// </summary>
        [EnumItemDescription("逻辑或操作符", ShortName = "||")]
        OI_LOGICAL_OR,

        /// <summary>
        /// 左括号
        /// </summary>
        [EnumItemDescription("左括号操作符", ShortName = "(")]
        OI_LBRACKET,

        /// <summary>
        /// 右括号
        /// </summary>
        [EnumItemDescription("右括号操作符", ShortName = ")")]
        OI_RBRACKET,

        /// <summary>
        /// 逗号
        /// </summary>
        [EnumItemDescription("逗号操作符", ShortName = ",")]
        OI_COMMA,

        /// <summary>
        /// 自定义函数
        /// </summary>
        [EnumItemDescription("自定义函数", ShortName = "自定义函数")]
        OI_USERDEFINE,

        /// <summary>
        /// 字符串
        /// </summary>
        [EnumItemDescription("字符串", ShortName = "字符串")]
        OI_STRING,

        /// <summary>
        /// 数字
        /// </summary>
        [EnumItemDescription("数字", ShortName = "数字")]
        OI_NUMBER,

        /// <summary>
        /// 布尔型
        /// </summary>
        [EnumItemDescription("布尔型", ShortName = "布尔型")]
        OI_BOOLEAN,

        /// <summary>
        /// 日期型
        /// </summary>
        [EnumItemDescription("日期型", ShortName = "日期型")]
        OI_DATETIME,
    }

    /// <summary>
    /// 表达式中的数据类型
    /// </summary>
    public enum ExpressionDataType
    {
        /// <summary>
        /// 字符串
        /// </summary>
        [EnumItemDescription("字符串", ShortName = "字符串")]
        String,

        /// <summary>
        /// 数字
        /// </summary>
        [EnumItemDescription("数字", ShortName = "数字")]
        Number,

        /// <summary>
        /// 布尔型
        /// </summary>
        [EnumItemDescription("布尔型", ShortName = "布尔型")]
        Boolean,

        /// <summary>
        /// 日期型
        /// </summary>
        [EnumItemDescription("日期型", ShortName = "日期型")]
        DateTime
    }
}

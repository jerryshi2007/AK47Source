using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCS.Library.Core;

namespace MCS.Library.HtmlParser
{
    /// <summary>
    /// 
    /// </summary>
    public static class HtmlStyleParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public static HtmlStyleAttributeCollection Parse(string style)
        {
            HtmlStyleAttributeCollection result = new HtmlStyleAttributeCollection();

            if (style.IsNullOrEmpty())
                return result;

            using (HtmlStyleParserContext context = new HtmlStyleParserContext())
            {
                HtmlStyleAttribute currentStyleAttr = new HtmlStyleAttribute();

                while (context.Index < style.Length)
                {
                    switch (context.Stage)
                    {
                        case HtmlStyleParsingStage.None:
                            {
                                if (SkipNotLetter(context, style))
                                {
                                    context.Stage = HtmlStyleParsingStage.KeyStage;
                                    currentStyleAttr = new HtmlStyleAttribute();
                                }
                                break;
                            }
                        case HtmlStyleParsingStage.KeyStage:
                            {
                                if (style[context.Index] != ':')
                                    context.Writer.Write(style[context.Index++]);
                                else
                                {
                                    currentStyleAttr.Key = context.ChangeStage(HtmlStyleParsingStage.ValueStage);
                                    context.Index++;
                                }

                                break;
                            }
                        case HtmlStyleParsingStage.ValueStage:
                            {
                                if (style[context.Index] != ';')
                                    context.Writer.Write(style[context.Index++]);
                                else
                                {
                                    currentStyleAttr.Value = context.ChangeStage(HtmlStyleParsingStage.None);
                                    currentStyleAttr.Expression = ParseValue(currentStyleAttr.Value);
                                    result.AddNotExistsItem(currentStyleAttr);

                                    currentStyleAttr = null;
                                    context.Index++;
                                }

                                break;
                            }
                    }
                }

                currentStyleAttr = DoStylePostOperation(context, currentStyleAttr);

                if (currentStyleAttr != null)
                    result.AddNotExistsItem(currentStyleAttr);
            }

            return result;
        }

        /// <summary>
        /// 分析样式的值，分解出其中的表达式和参数部分
        /// </summary>
        /// <param name="styleValue"></param>
        /// <returns></returns>
        public static HtmlStyleValueExpression ParseValue(string styleValue)
        {
            HtmlStyleValueExpression result = new HtmlStyleValueExpression();

            result.Expression = styleValue;

            if (styleValue.IsNotEmpty())
            {
                using (HtmlStyleValueParserContext context = new HtmlStyleValueParserContext())
                {
                    while (context.Index < styleValue.Length)
                    {
                        switch (context.Stage)
                        {
                            case HtmlStyleValueParsingStage.None:
                                {
                                    if (SkipNotLetterAndDigit(context, styleValue))
                                        context.Stage = HtmlStyleValueParsingStage.ExpressionStage;
                                    break;
                                }
                            case HtmlStyleValueParsingStage.ExpressionStage:
                                {
                                    if (styleValue[context.Index] != '(')
                                    {
                                        context.Writer.Write(styleValue[context.Index++]);
                                    }
                                    else
                                    {
                                        context.ParenthesesLevel++;
                                        result.Expression = context.ChangeStage(HtmlStyleValueParsingStage.ValueStage);

                                        context.Index++;
                                    }
                                    break;
                                }
                            case HtmlStyleValueParsingStage.ValueStage:
                                {
                                    if (styleValue[context.Index] != ')')
                                        context.Writer.Write(styleValue[context.Index++]);
                                    else
                                    {
                                        context.ParenthesesLevel--;

                                        if (context.ParenthesesLevel == 0)
                                            result.Value = context.ChangeStage(HtmlStyleValueParsingStage.None);

                                        context.Index++;
                                    }

                                    break;
                                }
                        }
                    }

                    DoStyleValuePostOperation(context, result);
                }
            }

            return result;
        }

        private static HtmlStyleAttribute DoStylePostOperation(HtmlStyleParserContext context, HtmlStyleAttribute currentStyleAttr)
        {
            if (currentStyleAttr != null)
            {
                switch (context.Stage)
                {
                    case HtmlStyleParsingStage.KeyStage:
                        currentStyleAttr.Key = context.ChangeStage(HtmlStyleParsingStage.ValueStage);
                        currentStyleAttr.Expression = ParseValue(currentStyleAttr.Value);
                        break;
                    case HtmlStyleParsingStage.ValueStage:
                        currentStyleAttr.Value = context.ChangeStage(HtmlStyleParsingStage.None);
                        currentStyleAttr.Expression = ParseValue(currentStyleAttr.Value);
                        break;
                }
            }

            return currentStyleAttr;
        }

        private static HtmlStyleValueExpression DoStyleValuePostOperation(HtmlStyleValueParserContext context, HtmlStyleValueExpression styleExpression)
        {
            if (styleExpression != null)
            {
                switch (context.Stage)
                {
                    case HtmlStyleValueParsingStage.ExpressionStage:
                        styleExpression.Expression = context.ChangeStage(HtmlStyleValueParsingStage.None);
                        break;
                    case HtmlStyleValueParsingStage.ValueStage:
                        styleExpression.Value = context.ChangeStage(HtmlStyleValueParsingStage.None);
                        break;
                }
            }

            return styleExpression;
        }

        private static bool SkipNotLetter<T>(HtmlStyleParserContextBase<T> context, string style)
        {
            bool result = false;

            while (context.Index < style.Length)
            {
                char currentChar = style[context.Index];

                if (Char.IsSeparator(currentChar) || Char.IsControl(currentChar))
                    context.Index++;
                else
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        private static bool SkipNotLetterAndDigit<T>(HtmlStyleParserContextBase<T> context, string style)
        {
            bool result = false;

            while (context.Index < style.Length)
            {
                char currentChar = style[context.Index];

                if (Char.IsSeparator(currentChar) || Char.IsControl(currentChar))
                    context.Index++;
                else
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}

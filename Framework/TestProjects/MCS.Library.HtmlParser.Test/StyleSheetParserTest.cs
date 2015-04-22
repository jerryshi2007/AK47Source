using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MCS.Library.HtmlParser.Test
{
    [TestClass]
    public class StyleSheetParserTest
    {
        [TestMethod]
        public void OneStyleTest()
        {
            string style = "color:red";

            HtmlStyleAttributeCollection result = HtmlStyleParser.Parse(style);

            Output(result);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("red", result["color"].Value);
        }

        [TestMethod]
        public void TwoStylesTest()
        {
            string style = "color:red;font-weight:bold";

            HtmlStyleAttributeCollection result = HtmlStyleParser.Parse(style);

            Output(result);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("red", result["color"].Value);
            Assert.AreEqual("bold", result["font-weight"].Value);
        }

        [TestMethod]
        public void TwoStyleWithBlankTest()
        {
            string style = " color: red; font-weight: bold ";

            HtmlStyleAttributeCollection result = HtmlStyleParser.Parse(style);

            Output(result);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("red", result["color"].Value);
            Assert.AreEqual("bold", result["font-weight"].Value);
        }

        [TestMethod]
        public void TwoStyleWithQuotationTest()
        {
            string style = " color: red; font-weight: 'bold' ";

            HtmlStyleAttributeCollection result = HtmlStyleParser.Parse(style);

            Output(result);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("red", result["color"].Value);
            Assert.AreEqual("'bold'", result["font-weight"].Value);
        }

        [TestMethod]
        public void TwoStyleWithoutLastValueTest()
        {
            string style = " color: red; font-weight";

            HtmlStyleAttributeCollection result = HtmlStyleParser.Parse(style);

            Output(result);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("red", result["color"].Value);
            Assert.AreEqual(null, result["font-weight"].Value);
        }

        [TestMethod]
        public void OneStyleWithoutLastValueTest()
        {
            string style = " color";

            HtmlStyleAttributeCollection result = HtmlStyleParser.Parse(style);

            Output(result);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(null, result["color"].Value);
        }

        [TestMethod]
        public void SimpleExpressionTest()
        {
            string styleValue = "expression(26 * 7)";

            HtmlStyleValueExpression expression = HtmlStyleParser.ParseValue(styleValue);

            Output(expression);
        }

        [TestMethod]
        public void DoubleParenthesesExpressionTest()
        {
            string styleValue = "expression((26 + 7） * 9)";

            HtmlStyleValueExpression expression = HtmlStyleParser.ParseValue(styleValue);

            Output(expression);
        }

        private static void Output(HtmlStyleAttributeCollection styles)
        {
            foreach (HtmlStyleAttribute style in styles)
            {
                Console.WriteLine("{0}: {1}; Expression: {2}, Value: {3}",
                    style.Key, style.Value, style.Expression.Expression, style.Expression.Value);
            }
        }

        private static void Output(HtmlStyleValueExpression expression)
        {
            if (expression != null)
                Console.WriteLine("{0}: {1}", expression.Expression, expression.Value);
        }
    }
}

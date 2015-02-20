using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Expression;

namespace ExpressionTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "表达式测试";

            Console.WriteLine("Please input expression...");

            string cmd = Console.ReadLine();

            while (cmd.ToLower() != "exit")
            {
                if (string.IsNullOrEmpty(cmd) == false)
                {
                    try
                    {
                        Console.WriteLine("This result is \n{0}", ExpressionParser.Calculate(cmd, UserFunctionImpl, null));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                cmd = Console.ReadLine();
            }
        }

        private static object UserFunctionImpl(string funcName, ParamObjectCollection arrParams, object callerContext)
        {
            object result = null;

            switch (funcName.ToLower())
            {
                case "power":
                    result = Math.Pow(Decimal.ToDouble((decimal)arrParams[0].Value),
                        Decimal.ToDouble((decimal)arrParams[1].Value));
                    break;
                case "expparse":
                    result = ExpressionParser.Parse((string)arrParams[0].Value).Identifiers.ToString();
                    break;
                case "author":
                    result = "Shen Zheng";
                    break;
                default:
                    if (BuiltInFunctionsCalculator.Instance.IsFunction(funcName))
                        result = BuiltInFunctionHelper.ExecuteFunction(funcName, BuiltInFunctionsCalculator.Instance, arrParams, callerContext);
                    else
                        throw new ApplicationException(string.Format("'{0}'是一个无法识别的标识符.", funcName));
                    break;
            }

            return result;
        }
    }
}

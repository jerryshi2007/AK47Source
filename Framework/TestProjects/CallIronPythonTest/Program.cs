using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Threading;

namespace CallIronPythonTest
{
    class Program
    {
        private static readonly string _ShowThreadIDFunctionScript = GetPythonFunctionScript();

        static void Main(string[] args)
        {
            var engine = Python.CreateEngine();
            ScriptSource source = engine.CreateScriptSourceFromString(_ShowThreadIDFunctionScript);
            var code = source.Compile();

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < 100; i++)
                tasks.Add(Task.Factory.StartNew(() => GetCurrentThreadID(engine, code)));

            Task.WaitAll(tasks.ToArray());
            //var scope = engine.CreateScope();
            //var source = engine.CreateScriptSourceFromString(
            //    "def adder(arg1, arg2):\n" +
            //    "   return arg1 + arg2\n" +
            //    "\n" +
            //    "class MyClass(object):\n" +
            //    "   def __init__(self, value):\n" +
            //    "       self.value = value\n");
            //source.Execute(scope);

            //var adder = scope.GetVariable<Func<object, object, object>>("adder");

            //Console.WriteLine(adder(2, 2));
            //Console.WriteLine(adder(2.0, 2.5));

            //var myClass = scope.GetVariable<Func<object, object>>("MyClass");
            //var myInstance = myClass("hello");

            //Console.WriteLine(engine.Operations.GetMember(myInstance, "value"));
        }

        private static void GetCurrentThreadID(ScriptEngine engine, CompiledCode code)
        {
            ScriptScope scope = engine.CreateScope();

            code.Execute(scope);
            var showThread = scope.GetVariable<Func<object, object>>("showThread");

            Console.WriteLine("C# ThreadID: {0}", Thread.CurrentThread.ManagedThreadId);
            var pythodResult = showThread(string.Format("Host Thread ID: {0}", Thread.CurrentThread.ManagedThreadId));

            Console.WriteLine(pythodResult);
        }

        private static string GetPythonFunctionScript()
        {
            StringBuilder strB = new StringBuilder();

            strB.AppendLine("def showThread(threadInfo):");
            strB.AppendLine("\treturn threadInfo");

            return strB.ToString();
        }
    }
}

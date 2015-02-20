using System;
using System.Diagnostics;
using System.Security;
using System.Text;

namespace CheckCompileStatus
{
    public static class CompileHelper
    {
        [SecurityCritical]
        public static string Compile(string workspaceLocalPath)
        {
            StringBuilder result = new StringBuilder();

            using (Process p = new Process())
            {

                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                p.Start();
                p.OutputDataReceived += (s, e) =>
                {
                    result.AppendLine(e.Data);
                    Console.WriteLine(e.Data);
                };

                p.StandardInput.WriteLine(workspaceLocalPath.Format<string>("\"%windir%\"\\microsoft.net\\framework\\v4.0.30319\\msbuild.exe {0}All.csproj"));
                p.StandardInput.WriteLine(@"xcopy .\Bin\*.xap .\MCSWebApp\Xap /Y /D /R");
                p.StandardInput.WriteLine("exit");

                p.BeginOutputReadLine();

                p.WaitForExit(6 * 30 * 1000);
            }

            return result.ToString();
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Net.Mail;

[assembly: CLSCompliant(true)]

namespace CheckCompileStatus
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime startTime = DateTime.Now;
            try
            {
                Uri tfsUri = new Uri(ConfigurationManager.AppSettings["tfsUri"]);
                string workspaceName = ConfigurationManager.AppSettings["workspaceName"];
                string workspaceServerPath = ConfigurationManager.AppSettings["workspaceServerPath"];
                string workspaceLocalPath = ConfigurationManager.AppSettings["workspaceLocalPath"];
                string userName = ConfigurationManager.AppSettings["userName"];
                string password = ConfigurationManager.AppSettings["password"];
                string domain = ConfigurationManager.AppSettings["domain"];
                string smtpServer = ConfigurationManager.AppSettings["smtpServer"];
                int smtpServerPort = int.Parse(ConfigurationManager.AppSettings["smtpServerPort"]);
                string adServer = ConfigurationManager.AppSettings["adServer"];
                string adminMail = ConfigurationManager.AppSettings["adminMail"];
                string ccMail = ConfigurationManager.AppSettings["ccMail"];

                WriteInitInfo();

                if (args.Length == 1 && args[0].ToUpperInvariant() == "/ALL")
                {
                    TFSHelper.GetLatestVersion(userName, password, domain, tfsUri, workspaceName, workspaceServerPath, true);
                }
                else
                {
                    TFSHelper.GetLatestVersion(userName, password, domain, tfsUri, workspaceName, workspaceServerPath, false);
                }


                WriteStartCompile();
                string compileResult = CompileHelper.Compile(workspaceLocalPath);

                WriteStartAnalyse();
                string timeElapsed = CompileResultAnalyzingHelper.AnalyzingTimeElapsed(compileResult);
                var errors = CompileResultAnalyzingHelper.AnalyzingError(compileResult, workspaceServerPath, workspaceLocalPath);
                var warnings = CompileResultAnalyzingHelper.AnalyzingWarning(compileResult, workspaceServerPath, workspaceLocalPath);
                TFSHelper.GetAlias(userName, password, domain, tfsUri, errors, warnings);
                WriteCompileResult(errors, warnings, timeElapsed);

                DateTime endTime = DateTime.Now;

                if ((errors.Count > 0) || (warnings.Count > 0))
                {
                    MailHelper mailHelper = new MailHelper(smtpServer, smtpServerPort, userName, password, domain, adServer);
                    mailHelper.Send(adminMail, ccMail, workspaceServerPath, startTime, endTime, timeElapsed, errors, warnings);
                }
            }
            catch (PathTooLongException ex)
            {
                ExceptionHelper.SendException(ex.ToString());
            }
            catch (SmtpFailedRecipientsException ex)
            {
                ExceptionHelper.SendException(ex.ToString());
            }
            catch (SmtpException ex)
            {
                ExceptionHelper.SendException(ex.ToString());
            }
            catch (Exception ex)
            {
                ExceptionHelper.SendException(ex.ToString());
            }


            // Console.Write("Press any key to continue . . . ");
            // Console.ReadKey(true);
        }

        static void WriteInitInfo()
        {
            ConsoleColorHelper.Set(MessageType.High);
            Console.WriteLine(Resource.Default.CheckCompileStatusInfo);
            ConsoleColorHelper.Reset();
            Console.WriteLine(Resource.Default.AuthorInfo);
            ConsoleColorHelper.Reset();
        }
        static void WriteStartCompile()
        {
            ConsoleColorHelper.Set(MessageType.High);
            Console.WriteLine();
            Console.WriteLine(Resource.Default.CompileStartingInfo);
            ConsoleColorHelper.Reset();
        }

        static void WriteStartAnalyse()
        {
            ConsoleColorHelper.Set(MessageType.High);
            Console.WriteLine();
            Console.WriteLine(Resource.Default.AnalyzingStartingInfo);
            ConsoleColorHelper.Reset();
        }

        static void WriteCompileResult(Collection<CompileResult> errors, Collection<CompileResult> warnings, string timeElapsed)
        {
            ConsoleColorHelper.Set(MessageType.High);
            Console.WriteLine();
            Console.WriteLine(timeElapsed.Format<string>("Time Elapsed {0}"));
            ConsoleColorHelper.Reset();

            if (errors.Count > 0)
            {
                ConsoleColorHelper.Set(MessageType.Error);
                Console.WriteLine();
                Console.WriteLine(Resource.Default.BuildFAILEDInfo);
                ConsoleColorHelper.Reset();

                foreach (var error in errors)
                {
                    ConsoleColorHelper.Set(MessageType.Error);
                    Console.WriteLine();
                    Console.WriteLine(error.ToString());
                    ConsoleColorHelper.Reset();
                }
            }
            else if (warnings.Count > 0)
            {
                ConsoleColorHelper.Set(MessageType.Warning);
                Console.WriteLine();
                Console.WriteLine(Resource.Default.BuildWARNINGInfo);
                ConsoleColorHelper.Reset();

                foreach (var warning in warnings)
                {
                    ConsoleColorHelper.Set(MessageType.Warning);
                    Console.WriteLine();
                    Console.WriteLine(warning.ToString());
                    ConsoleColorHelper.Reset();
                }
            }
            else
            {
                ConsoleColorHelper.Set(MessageType.High);
                Console.WriteLine();
                Console.WriteLine(Resource.Default.BuildsucceededInfo);
                ConsoleColorHelper.Reset();
            }
        }
    }
}
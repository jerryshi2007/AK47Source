using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace CheckCompileStatus
{
    public static class CompileResultAnalyzingHelper
    {
        public static string AnalyzingTimeElapsed(string content)
        {
            string result = string.Empty;
            string pattern = @"Time Elapsed (?<timeElapsed>.*)";
            Regex reg = new Regex(pattern);
            Match mt = reg.Match(content);
            if (mt.Success)
            {
                result = mt.Groups["timeElapsed"].Value;
            }
            return result.Replace("\r", string.Empty);
        }

        public static Collection<CompileResult> AnalyzingError(string content, string workspaceServerPath, string workspaceLocalPath)
        {
            Collection<CompileResult> result = new Collection<CompileResult>();
            string pattern = @"  (?<fileName>.*?)\(.*?,.*?\): error(?<buildError>.*?)]";

            Regex reg = new Regex(pattern);
            Match mt = reg.Match(content);
            while (mt.Success)
            {
                result.Add(new CompileResult(workspaceServerPath, workspaceLocalPath)
                {
                    ResultType = CompileResultType.Error,
                    FileName = mt.Groups["fileName"].Value,
                    CompileDetail = mt.Groups["buildError"].Value.Format<string>("error {0}]"),

                });
                mt = mt.NextMatch();
            }

            return result;
        }

        public static Collection<CompileResult> AnalyzingWarning(string content, string workspaceServerPath, string workspaceLocalPath)
        {
            Collection<CompileResult> result = new Collection<CompileResult>();
            string pattern = @"  (?<fileName>.*?)\(.*?,\S*?\): warning(?<buildWarning>.*?)]";
            Regex reg = new Regex(pattern);
            Match mt = reg.Match(content);
            while (mt.Success)
            {
                result.Add(new CompileResult(workspaceServerPath, workspaceLocalPath)
                {
                    ResultType = CompileResultType.Warning,
                    FileName = CheckFileName(mt),
                    CompileDetail = mt.Groups["buildWarning"].Value.Format<string>("warning {0}]"),

                });
                mt = mt.NextMatch();
            }
            return result;
        }

        private static string CheckFileName(Match mt)
        {
            string result = mt.Groups["fileName"].Value;
            if (result.IndexOf("Microsoft.Common.targets") != -1)
            {
                string pattern = @"\[(?<fileName>.*?).csproj";
                Regex reg = new Regex(pattern);
                Match mt1 = reg.Match(mt.Groups["buildWarning"].Value);
                if (mt1.Success)
                {
                    result = mt1.Groups["fileName"].Value.Format<string>("{0}.csproj");
                    result = result.Substring(result.LastIndexOf("\\") + 1);
                }
            }
            return result;
        }
    }

    public class CompileResult
    {
        private string workspaceServerPath;
        private string workspaceLocalPath;
        public CompileResult(string workspaceServerPath, string workspaceLocalPath)
        {
            this.workspaceServerPath = workspaceServerPath;
            this.workspaceLocalPath = workspaceLocalPath;
        }

        public CompileResultType ResultType { get; set; }

        public string FileName { get; set; }

        public string CompileDetail { get; set; }

        public string Owner { get; set; }

        public DateTime CreationDate { get; set; }

        public string ServerPath
        {
            get
            {
                string result = string.Empty;
                string prjName = string.Empty;
                Regex reg = new Regex("\\[(?<prjName>.*)]");
                Match mt = reg.Match(CompileDetail);
                if (mt.Success)
                {
                    prjName = mt.Groups["prjName"].Value;
                }
                if (!string.IsNullOrEmpty(prjName))
                {
                    result = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", Path.GetDirectoryName(prjName), FileName);
                    result = result.Replace(this.workspaceLocalPath, this.workspaceServerPath).Replace("\\", "/");
                }

                return result;
            }
        }

        public string OwnerMail
        {
            get
            {
                return OwnerName.Format<string>("{0}@sinooceanland.com");
            }
        }

        public string OwnerName
        {
            get
            {
                return Owner.Replace("SINOOCEANLAND\\", string.Empty);
            }
        }

        public string OwnerDisplayName { get; set; }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[ResultType:{0}; Owner:{1}; CreationDate:{2}; ServerPath:{3}; CompileDetail:{4}]",
                ResultType.ToString(), Owner, CreationDate, ServerPath, CompileDetail);
        }
    }

    public enum CompileResultType
    {
        None = 0,
        Error = 1,
        Warning = 2,
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.SOA.DataObjects.Security
{
	public interface IImportContext
	{
		void AppendLog(string message);
		void AppendLogFormat(string format, params object[] args);
		void AppendLogFormat(string format, object arg);
		void AppendLogFormat(string format, object arg, object arg1);
		void AppendLogFormat(string format, object arg, object arg1, object arg2);
		void SetStatus(int currentStep, int maxStep, string message);
		void SetStatusAndLog(int currentStep, int maxStep, string message);
		void SetSubStatusAndLog(int currentStep, int maxStep, string message);
		int ErrorCount { get; set; }
	}
}

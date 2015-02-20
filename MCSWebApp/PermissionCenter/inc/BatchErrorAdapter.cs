using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MCS.Library.Core;

namespace PermissionCenter
{
	public interface IBatchErrorAdapter
	{
		void AddError(Exception ex);

		void AddException(Exception ex);

		void AddError(string message);
	}

	public sealed class ProgressBatchErrorAdapter : IBatchErrorAdapter
	{
		public static readonly ProgressBatchErrorAdapter Instance = new ProgressBatchErrorAdapter();

		public void AddError(Exception ex)
		{
			ProcessProgress.Current.Output.WriteLine(ex.Message);
		}

		public void AddException(Exception ex)
		{
			ProcessProgress.Current.Output.WriteLine(ex.Message);
		}

		public void AddError(string message)
		{
			ProcessProgress.Current.Output.WriteLine(message);
		}
	}

	public class ListErrorAdapter : IBatchErrorAdapter
	{
		private IList<object> target;

		public ListErrorAdapter(IList<object> target)
		{
			this.target = target;
		}

		public void AddError(Exception ex)
		{
			this.target.Add(ex);
		}

		public void AddException(Exception ex)
		{
			this.target.Add(ex);
		}

		public void AddError(string message)
		{
			this.target.Add(message);
		}
	}
}
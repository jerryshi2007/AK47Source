using System;
using System.Collections.Generic;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Workflow.Descriptors;
using MCS.Library.Workflow.Configuration;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Data;

namespace MCS.Library.Workflow.Engine
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class WfAnchorActivity : WfActivityBase, IWfAnchorActivity
	{
		private WfOperationCollection _Operations = null;

		/// <summary>
		/// 
		/// </summary>
		protected WfAnchorActivity()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="descriptor"></param>
		public WfAnchorActivity(IWfAnchorActivityDescriptor descriptor)
			: base(descriptor)
		{
		}

		#region ISerialiable

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected WfAnchorActivity(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			_Operations = (WfOperationCollection)info.GetValue("Operations", typeof(WfOperationCollection));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("Operations", _Operations, typeof(WfOperationCollection));
		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		public WfOperationCollection Operations
		{
			get
			{
				if (_Operations == null)
					if (LoadingType == DataLoadingType.External)
						_Operations = LoadOperations();
					else
						_Operations = new WfOperationCollection();

				return _Operations;
			}
			set
			{
				_Operations = value;
			}
		}

		/// <summary>
		/// 分支流程结束后的，Anchor点能够流转的下一个点
		/// </summary>
		public IWfActivityDescriptor NextAutoTransferActivityDescriptor
		{
			get
			{
				IList<IWfTransitionDescriptor> transitions = this.Descriptor.ToTransitions.GetCanMovableTransitions();

				IWfActivityDescriptor target = null;

				if (this.Descriptor.ToTransitions.Count == 1)
					target = transitions[0].ToActivity;
				else
					if (this.FromTransition != null)
						target = this.FromTransition.FromActivity.Descriptor;

				return target;
			}
		}

		/// <summary>
		/// 分支流程结束后的，Anchor自动返回的点是否是上一个点
		/// </summary>
		public bool AutoReturnToPreviousActivity
		{
			get
			{
				IList<IWfTransitionDescriptor> transitions = this.Descriptor.ToTransitions.GetCanMovableTransitions();

				return (transitions.Count != 1 && this.FromTransition != null);
			}
		}

		protected virtual WfOperationCollection LoadOperations()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		/// <summary>
		/// 
		/// </summary>
		public virtual IWfOperation AddNewOperation(WfBranchesTransferParams transferParams)
		{
			IWfOperation newOperation = this.Process.Factory.CreateOperation(this, transferParams);
			this.Operations.Add(newOperation);

			WorkflowSettings.GetConfig().EnqueueWorkItemExecutor.EnqueueAddNewOperationWorkItem(newOperation);

			return newOperation;
		}

		public virtual void RemoveOperations(params IWfOperation[] operations)
		{
			WorkflowSettings.GetConfig().EnqueueWorkItemExecutor.EnqueueRemoveOperationsWorkItem(operations);

			for (int i = 0; i < operations.Length; i++)
				Operations.Remove(operations[i]);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public virtual bool AllOperationsCompleted()
		{
			bool flag = true;

			foreach (WfOperation operation in this.Operations)
			{
				if (operation.Completed() == false)
				{
					flag = false;
					break;
				}
			}

			return flag;
		}

		public override bool AbleToMoveTo()
		{
			return base.AbleToMoveTo() && this.AllOperationsCompleted();
		}
	}
}

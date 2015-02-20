using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using MCS.Library.Core;
using MCS.Library.Data;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Conditions;
using MCS.Web.Library.Script;
using MCS.Library.SOA.DataObjects.Security.Logs;

namespace MCS.Library.SOA.DataObjects.Security.Executors
{
	/// <summary>
	/// </summary>
	public class SCUpdateObjectImageExecutor : SCExecutorBase
	{
		private SchemaObjectBase _Object;
		private ImageProperty _Image;
		private string _PropertyName;
		private bool _NeedStatusCheck = false;

		public SCUpdateObjectImageExecutor(SCOperationType opType, SchemaObjectBase obj, string propertyName, ImageProperty image)
			: base(opType)
		{
			obj.NullCheck("obj");
			propertyName.IsNullOrEmpty().TrueThrow("propertyName不能为空!");

			this._Object = obj;
			this._PropertyName = propertyName;
			this._Image = image;
		}

		public bool NeedStatusCheck
		{
			get
			{
				return this._NeedStatusCheck;
			}
			set
			{
				this._NeedStatusCheck = value;
			}
		}

		protected override void PrepareData(SchemaObjectOperationContext context)
		{
			base.PrepareData(context);

			CheckObjectStatus(this._Object);
		}

		protected override object DoOperation(SchemaObjectOperationContext context)
		{
			using (TransactionScope scope = TransactionScopeFactory.Create())
			{
				if (_Image == null || _Image.IsEmpty())
				{
					_Object.Properties[this._PropertyName].StringValue = "";
				}
				else
				{
					ImagePropertyAdapter.Instance.UpdateWithContent(this._Image);
					_Object.Properties[this._PropertyName].StringValue = JSONSerializerExecute.Serialize(this._Image);
				}

				SchemaObjectAdapter.Instance.Update(this._Object);
				scope.Complete();
			}

			return this._Object;
		}

		protected override void PrepareOperationLog(SchemaObjectOperationContext context)
		{
			SCOperationLog log = SCOperationLog.CreateLogFromEnvironment();

			log.ResourceID = this._Object.ID;
			log.SchemaType = this._Object.SchemaType;
			log.OperationType = this.OperationType;
			log.Category = this._Object.Schema.Category;
			log.Subject = string.Format("{0}: {1}",
				EnumItemDescriptionAttribute.GetDescription(this.OperationType), ((SCBase)this._Object).Name);

			log.SearchContent = this._Object.ToFullTextString();

			context.Logs.Add(log);
		}
	}
}

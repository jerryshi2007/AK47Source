using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Security.Actions;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Logs;

namespace MCS.Library.SOA.DataObjects.Security.Executors
{
	/// <summary>
	/// 修改Owner操作
	/// </summary>
	public class SCChangeOwnerExecutor : SCExecutorBase
	{
		private SCBase _Object = null;
		private SCOrganization _TargetOrganization = null;
		private bool _NeedStatusCheck = false;

		/// <summary>
		/// 修改对象的Owner信息
		/// </summary>
		/// <param name="opType">操作类型</param>
		/// <param name="obj">需要调整Owner的对象</param>
		/// <param name="targetOrg">调整后的Owner</param>
		public SCChangeOwnerExecutor(SCOperationType opType, SCBase obj, SCOrganization targetOrg)
			: base(opType)
		{
			obj.NullCheck("obj");
			targetOrg.NullCheck("targetOrg");

			obj.ClearRelativeData();
			targetOrg.ClearRelativeData();

			this._Object = obj;
			this._TargetOrganization = targetOrg;
		}

		/// <summary>
		/// 是否需要状态检查
		/// </summary>
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

		protected void CheckStatus()
		{
			if (this.NeedStatusCheck)
			{
				CheckObjectStatus(this._Object, this._TargetOrganization);
			}
		}

		protected override void PrepareOperationLog(SchemaObjectOperationContext context)
		{
			SCOperationLog log = SCOperationLog.CreateLogFromEnvironment();

			log.ResourceID = this._Object.ID;
			log.SchemaType = this._Object.SchemaType;
			log.OperationType = this.OperationType;
			log.Category = this._Object.Schema.Category;
			log.Subject = string.Format("{0}: {1} 新的所有者 {2}",
				EnumItemDescriptionAttribute.GetDescription(this.OperationType), this._Object.Name,
				this._TargetOrganization.Name);

			log.SearchContent = this._Object.ToFullTextString() +
				" " + this._TargetOrganization.ToFullTextString();

			context.Logs.Add(log);
		}

		protected override void PrepareData(SchemaObjectOperationContext context)
		{
			CheckStatus();

			this._Object.Properties.ContainsKey("OwnerID").FalseThrow("对象{0}不具备OwnerID属性", this._Object.ToDescription());

			this._Object.CurrentParents.ContainsKey(_TargetOrganization.ID).FalseThrow("修改对象{0}的所有者错误，目标对象{1}不是{0}的父对象",
				this._Object.ToDescription(), this._TargetOrganization.ToDescription());

			this._Object.Properties.SetValue("OwnerID", this._TargetOrganization.ID);
			this._Object.Properties.SetValue("OwnerName", this._TargetOrganization.Properties.GetValue("Name", string.Empty));
		}

		protected override object DoOperation(Adapters.SchemaObjectOperationContext context)
		{
			SchemaObjectAdapter.Instance.Update(this._Object);

			return this._Object;
		}
	}
}

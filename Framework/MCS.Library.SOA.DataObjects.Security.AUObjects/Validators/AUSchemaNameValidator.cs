using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Validation;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Validators
{
	/// <summary>
	/// 名称的校验器
	/// </summary>
	public class AUSchemaNameValidator : Validator
	{
		protected override void DoValidate(object objectToValidate, object currentObject, string key, ValidationResults validateResults)
		{
			AUSchema schema = (AUSchema)currentObject;
			string name = (string)objectToValidate;
			var target = Adapters.AUSnapshotAdapter.Instance.LoadAUSchemaByName(name, schema.CategoryID, true).GetUniqueNormalObject<AUSchema>();

			if (target != null && schema.ID != target.ID)
			{
				this.RecordValidationResult(validateResults, string.Format("管理架构 {0} 的不唯一名称", AUCommon.DisplayNameFor(schema)), currentObject, key);
			}
		}
	}
}

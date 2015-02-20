using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Validation;
using MCS.Library.SOA.DataObjects.Security.AUObjects.Adapters;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;


namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Validators
{
	/// <summary>
	/// 校验管理架构分类的校验器
	/// </summary>
	public class CategoryIDValidator : Validator
	{
		protected override void DoValidate(object objectToValidate, object currentObject, string key, ValidationResults validateResults)
		{
			AUSchemaCategory cate = SchemaCategoryAdapter.Instance.LoadByID((string)objectToValidate);

			if (cate == null || cate.Status != SchemaObjectStatus.Normal)
			{
				this.RecordValidationResult(validateResults, string.Format("指定的类别ID \"{0}\"无效。", objectToValidate), currentObject, key);
			}
		}
	}
}

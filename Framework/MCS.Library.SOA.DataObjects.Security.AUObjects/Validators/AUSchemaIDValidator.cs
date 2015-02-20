using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Validation;
using PC = MCS.Library.SOA.DataObjects.Security;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Validators
{
	public class AUSchemaIDValidator : Validator
	{
		protected override void DoValidate(object objectToValidate, object currentObject, string key, ValidationResults validateResults)
		{
			string messageTemplate = string.IsNullOrEmpty(this.MessageTemplate) ? "{0}不是有效的管理架构ID。" : this.MessageTemplate;
			string schemaID = objectToValidate.ToString();

			SchemaObjectBase schema = null;
			AUCommon.DoDbAction(() =>
			{
				schema = PC.Adapters.SchemaObjectAdapter.Instance.Load(schemaID);
			});

			if (schema == null || schema.Status != Schemas.SchemaProperties.SchemaObjectStatus.Normal)
			{
				this.RecordValidationResult(validateResults, string.Format(messageTemplate, objectToValidate), currentObject, key);
			}

		}
	}
}

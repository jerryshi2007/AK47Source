using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Validation;
using PC = MCS.Library.SOA.DataObjects.Security;
using MCS.Library.SOA.DataObjects.Security.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Validators
{
	/// <summary>
	/// 校验管理范围的类型
	/// </summary>
	public class AUAdminScopeValidator : Validator
	{
		protected override void DoValidate(object objectToValidate, object currentObject, string key, ValidationResults validateResults)
		{
			bool isValid = false;
			var types = SchemaInfo.FilterByCategory("AdminScopeItems");
			foreach (var item in types)
			{
				if (item.Name.Equals(objectToValidate))
				{
					isValid = true;
					break;
				}
			}

			if (isValid == false)
			{
				SchemaObjectBase doValidateObj = (SchemaObjectBase)currentObject;
				ObjectSchemaConfigurationElement config = ObjectSchemaSettings.GetConfig().Schemas[doValidateObj.SchemaType];
				this.RecordValidationResult(validateResults, string.Format(this.MessageTemplate, string.IsNullOrEmpty(config.Description) ? config.Description : config.Name, doValidateObj.Properties["Name"].StringValue, doValidateObj.ID), doValidateObj, key);
			}
		}
	}
}

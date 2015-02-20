using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.Data.Builder;
using MCS.Library.SOA.DataObjects.Schemas.Configuration;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.SOA.DataObjects.Security.Configuration;
using MCS.Library.Validation;

namespace MCS.Library.SOA.DataObjects.Security.Validators
{
	internal sealed class CodeNameUniqueValidator : MCS.Library.Validation.Validator
	{
		private bool includingDeleted = true;

		public CodeNameUniqueValidator(bool includingDeleted, string messageTemplate, string tag)
			: base(messageTemplate, tag)
		{
			this.includingDeleted = includingDeleted;
		}

		protected override void DoValidate(object objectToValidate, object currentObject, string key, ValidationResults validateResults)
		{
			SchemaObjectBase doValidateObj = currentObject as SchemaObjectBase;
			if (doValidateObj != null)
			{
				string strValue = objectToValidate.ToString();

				ExceptionHelper.TrueThrow(strValue.IsNullOrEmpty(), "代码名称不能为空");

				switch (doValidateObj.Schema.CodeNameValidationMethod)
				{
					case SchemaObjectCodeNameValidationMethod.ByCodeNameKey:
						this.CodeNameValidationMethod(strValue, doValidateObj, key, validateResults);
						break;
					case SchemaObjectCodeNameValidationMethod.ByContainerAndCodeNameKey:
						this.ByContainerAndCodeNameKey(strValue, doValidateObj, key, validateResults);
						break;
				}
			}
		}

		private void ByContainerAndCodeNameKey(string strValue, SchemaObjectBase doValidateObj, string key, ValidationResults validateResults)
		{
			SchemaObjectBase container = SchemaPropertyValidatorContext.Current.Container;

			bool result = CodeNameUniqueValidatorFacade.ValidateCodeNameWithContainer(strValue, CodeNameUniqueValidatorFacade.GetScopeSchemas2(doValidateObj.Schema.CodeNameKey).ToArray(), doValidateObj.ID, container.ID, this.includingDeleted == false, false, DateTime.MinValue);

			if (result == false)
			{
				ObjectSchemaConfigurationElement config = ObjectSchemaSettings.GetConfig().Schemas[doValidateObj.SchemaType];
				RecordValidationResult(validateResults, string.Format(this.MessageTemplate, string.IsNullOrEmpty(config.Description) ? config.Description : config.Name, doValidateObj.Properties["Name"].StringValue, doValidateObj.ID), doValidateObj, key);
			}
		}

		private void CodeNameValidationMethod(string strToValue, SchemaObjectBase doValidateObj, string key, ValidationResults validateResults)
		{
			bool result = CodeNameUniqueValidatorFacade.ValidateCodeName(strToValue, CodeNameUniqueValidatorFacade.GetScopeSchemas2(doValidateObj.Schema.CodeNameKey).ToArray(), doValidateObj.ID, this.includingDeleted == false, false, DateTime.MinValue);
			if (result == false)
			{
				ObjectSchemaConfigurationElement config = ObjectSchemaSettings.GetConfig().Schemas[doValidateObj.SchemaType];
				RecordValidationResult(validateResults, string.Format(this.MessageTemplate, string.IsNullOrEmpty(config.Description) ? config.Description : config.Name, doValidateObj.Properties["Name"].StringValue, doValidateObj.ID), doValidateObj, key);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Validation;

namespace MCS.Library.SOA.DataObjects.Security.AUObjects.Validators
{
	public class AUSchemaAdminScopeValidator : Validator
	{
		public AUSchemaAdminScopeValidator()
		{
		}

		public AUSchemaAdminScopeValidator(string messageTemplate)
			: base(messageTemplate)
		{
		}

		protected override void DoValidate(object objectToValidate, object currentObject, string key, ValidationResults validateResults)
		{
			string typeString = (string)objectToValidate;
			string messageTemplate = string.IsNullOrEmpty(this.MessageTemplate) ? "{0}包含无效或重复的的管理范围类别字符串：{1}" : this.MessageTemplate;
			string[] parts = typeString.Split(AUCommon.Spliter, StringSplitOptions.RemoveEmptyEntries);

			// 准备有效项的集合
			HashSet<string> effected = new HashSet<string>();
			HashSet<string> repeated = new HashSet<string>();
			foreach (var item in SchemaInfo.FilterByCategory("AUScopeItems"))
				effected.Add(item.Name);

			// 检查有效项
			foreach (var item in parts)
			{
				if (effected.Contains(item) == false)
				{
					this.RecordValidationResult(validateResults, string.Format(messageTemplate, AUCommon.DisplayNameFor((SchemaObjectBase)currentObject), item), currentObject, key);
					break;
				}

				if (repeated.Contains(item))
				{
					this.RecordValidationResult(validateResults, string.Format(messageTemplate, AUCommon.DisplayNameFor((SchemaObjectBase)currentObject), item), currentObject, key);
					break;
				}
				else
				{
					repeated.Add(item);
				}
			}
		}
	}
}

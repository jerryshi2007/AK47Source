using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Library.SOA.DataObjects.Security.Adapters;
using MCS.Library.Validation;

namespace MCS.Library.SOA.DataObjects.Security.Validators
{
	internal sealed class FullPathUniqueValidator : Validator
	{
		private bool includingDeleted = true;

		public FullPathUniqueValidator(bool includingDeleted, string messageTemplate, string tag)
			: base(messageTemplate, tag)
		{
			this.includingDeleted = includingDeleted;
		}

		/// <summary>
		/// 执行校验
		/// </summary>
		/// <param name="objectToValidate">属性值，这里应该是FullPath的值</param>
		/// <param name="currentObject">属性的容器对象，这里应该是SCRelationObject</param>
		/// <param name="key">属性名，这里应该是FullPath</param>
		/// <param name="validateResults">校验结果</param>
		protected override void DoValidate(object objectToValidate, object currentObject, string key, ValidationResults validateResults)
		{
			SCRelationObject relation = currentObject as SCRelationObject;

			if (relation != null)
			{
				string fullPath = (string)objectToValidate;

				if (fullPath.IsNotEmpty())
				{
					SchemaDefine schema = SchemaDefine.GetSchema(relation.ChildSchemaType);

					if (schema.FullPathValidationMethod == SCRelationFullPathValidationMethod.UniqueInParent)
						DoFullPathValidate(relation, fullPath, key, validateResults);
				}

				if (relation.ID == relation.ParentID)
				{
					RecordValidationResult(validateResults, "无效的关系，此关系的父子对象为同一ID，这是不允许的。", relation, key);
				}
			}
		}

		private void DoFullPathValidate(SCRelationObject relation, string fullPath, string key, ValidationResults validateResults)
		{
			SCRelationObjectCollection relations = SchemaRelationObjectAdapter.Instance.LoadByFullPath(fullPath, this.includingDeleted, DateTime.MinValue);

			SCRelationObject existedRelation = relations.Find(r => r.ParentID == relation.ParentID && r.ID != relation.ID);

			if (existedRelation != null)
				RecordValidationResult(validateResults, string.Format(this.MessageTemplate, existedRelation.FullPath), relation, key);
		}
	}
}

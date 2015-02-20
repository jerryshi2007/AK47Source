using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using MCS.Library.Caching;

namespace MCS.Library.Validation
{
	internal class MetadataValidatorBuilder
	{
		public static readonly MetadataValidatorBuilder Instance = new MetadataValidatorBuilder();

		private MetadataValidatorBuilder()
		{
		}

		public Validator CreateValidator(Type type, string ruleset, List<string> unValidates)
		{
			List<Validator> list = null;

			if (ValidatorCache.validatorCacheQueue.TryGetValue(type.FullName + ruleset, out list) == false)
			{
				list = new List<Validator>();

				GetValidatorsFromType(type, ruleset, list);
				GetValidatorsFromProperties(type, ruleset, list);
				GetValidatorsFromFields(type, ruleset, list);

				ValidatorCache.validatorCacheQueue.Add(type.FullName + ruleset, list);
			}

			List<Validator> clonedValidators = new List<Validator>();

			foreach (Validator v in list)
			{
				if (unValidates == null || unValidates.Exists(s => s == v.Source) == false)
					clonedValidators.Add(v);
			}

			return new AndCompositeValidator(clonedValidators);
		}

		#region GetValidatorsFromProperties

		private void GetValidatorsFromProperties(Type type, string ruleset, List<Validator> list)
		{
			PropertyInfo[] pInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

			foreach (PropertyInfo pi in pInfos)
			{
				FillValidatorsFromProperty(pi, ruleset, list);
			}
		}

		private void FillValidatorsFromProperty(PropertyInfo info, string ruleset, List<Validator> list)
		{
			CompositeValidatorBuilder builder = GetCompositeValidatorBuilder(info, ruleset);

			foreach (ValidatorAttribute attr in info.GetCustomAttributes(typeof(ValidatorAttribute), true))
			{
				if (attr.Ruleset == ruleset && info.CanRead)
				{
					ValueAccessValidator valueValidator = new ValueAccessValidator(
						new PropertyValueAccess(info),
						attr.CreateValidator(info.PropertyType, info.ReflectedType));

					builder.AddValueValidator(valueValidator);
				}
			}

			if (builder.GetCompositeValidatorsCount() != 0)
			{
				Validator result = builder.GetValidator();
				result.Source = info.Name;
				list.Add(result);
			}
		}

		#endregion

		#region GetValidatorsFromFields

		private void GetValidatorsFromFields(Type type, string ruleset, List<Validator> list)
		{
			FieldInfo[] fieldInfo = type.GetFields(BindingFlags.Instance | BindingFlags.Public);

			foreach (FieldInfo info in fieldInfo)
			{
				FillValidatorsFromField(info, ruleset, list);
			}
		}

		private void FillValidatorsFromField(FieldInfo info, string ruleset, List<Validator> list)
		{
			CompositeValidatorBuilder builder = GetCompositeValidatorBuilder(info, ruleset);

			foreach (ValidatorAttribute attr in info.GetCustomAttributes(typeof(ValidatorAttribute), true))
			{
				if (attr.Ruleset == ruleset)
				{
					ValueAccessValidator valueValidator = new ValueAccessValidator(
						new FieldValueAccess(info),
						attr.CreateValidator(info.FieldType, info.ReflectedType));

					builder.AddValueValidator(valueValidator);
				}
			}

			if (builder.GetCompositeValidatorsCount() != 0)
			{
				Validator result = builder.GetValidator();
				result.Source = info.Name;
				list.Add(result);
			}
		}

		#endregion

		#region GetValidatorsFromType

		private void GetValidatorsFromType(Type type, string ruleset, List<Validator> list)
		{
			CompositeValidatorBuilder builder = GetCompositeValidatorBuilder(type, ruleset);

			foreach (ValidatorAttribute attr in type.GetCustomAttributes(typeof(ValidatorAttribute), true))
			{
				if (attr.Ruleset == ruleset)
				{
					builder.AddValueValidator(attr.CreateValidator(type, type.ReflectedType));
				}
			}

			if (builder.GetCompositeValidatorsCount() != 0)
			{
				Validator result = builder.GetValidator();
				result.Source = type.FullName;
				list.Add(result);
			}
		}

		#endregion

		private CompositeValidatorBuilder GetCompositeValidatorBuilder(MemberInfo info, string ruleset)
		{
			CompositeValidatorBuilder builder = null;

			foreach (ValidatorCompositionAttribute attr in info.GetCustomAttributes(typeof(ValidatorCompositionAttribute), true))
			{
				if (attr.Ruleset == ruleset)
				{
					builder = new CompositeValidatorBuilder(attr.CompositionType, attr.MessageTemplate);
					break;
				}
			}

			if (builder == null)
				builder = new CompositeValidatorBuilder(CompositionType.And);

			return builder;
		}
	}
}

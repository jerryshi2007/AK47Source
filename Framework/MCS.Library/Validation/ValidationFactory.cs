using System;
using System.Collections.Generic;
using System.Text;

namespace MCS.Library.Validation
{
	/// <summary>
	/// 校验器的工厂类，用户通过这个类来创建校验器
	/// </summary>
	/// <remarks>
	/// 校验器的工厂类，用户通过这个类来创建并获取目标类型上预定义的校验器
	/// </remarks>
	public static class ValidationFactory
	{
		/// <summary>
		/// 获取指定类型上定义的校验器
		/// </summary>
		/// <param name="targetType">目标类型</param>
		/// <returns>校验器</returns>
		/// <remarks>
		/// <code>
		/// 
		/// </code>
		/// </remarks>
		public static Validator CreateValidator(Type targetType)
		{
			return CreateValidator(targetType, string.Empty);
		}

		/// <summary>
		/// 获取指定类型上定义的校验器
		/// </summary>
		/// <param name="targetType">目标类型</param>
		/// <param name="unValidates">忽略的属性集合</param>
		/// <returns></returns>
		public static Validator CreateValidator(Type targetType, List<string> unValidates)
		{
			return CreateValidator(targetType, string.Empty, unValidates);
		}

		/// <summary>
		/// 获取指定类型上定义的并属于指定规则集合的校验器
		/// </summary>
		/// <param name="targetType">目标类型</param>
		/// <param name="ruleset">校验器所属的规则集合</param>
		/// <returns>校验器</returns>
		/// <remarks>
		/// <code source="..\Framework\TestProjects\DeluxeWorks.Library.Test\Validation\ValidationFactoryTest.cs" region="CreateValidatorForSpecificTypeAndRuleset" lang="cs" title="如何创建校验器" />
		/// </remarks>
		public static Validator CreateValidator(Type targetType, string ruleset)
		{
			return MetadataValidatorBuilder.Instance.CreateValidator(targetType, ruleset, null);
		}

		/// <summary>
		/// 获取指定类型上定义的校验器
		/// </summary>
		/// <param name="targetType">目标类型</param>
		/// <param name="ruleset">校验器所属的规则集合</param>
		/// <param name="unValidates">忽略的属性集合</param>
		/// <returns></returns>
		public static Validator CreateValidator(Type targetType, string ruleset, List<string> unValidates)
		{
			return MetadataValidatorBuilder.Instance.CreateValidator(targetType, ruleset, unValidates);
		}
	}
}

using System;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Generic;

using MCS.Library.Core;
using MCS.Library.Configuration;
using MCS.Library.Principal;
using MCS.Library.OGUPermission;

namespace MCS.Library.Passport
{
	/// <summary>
	/// 逻辑角色配置的配置节。这个配置节配置了每一个逻辑角色，应该包含哪些授权系统中的角色
	/// </summary>
	public class RolesDefineConfig : ConfigurationSection
	{
		private RolesDefineConfig() { }

		private const string configNodeName = "rolesDefineConfig";

		/// <summary>
		/// 得到配置节
		/// </summary>
		/// <returns></returns>
		public static RolesDefineConfig GetConfig()
		{
			RolesDefineConfig config = (RolesDefineConfig)ConfigurationBroker.GetSection(configNodeName);

			ConfigurationExceptionHelper.CheckSectionNotNull(config, configNodeName);

			return config;
		}

		/// <summary>
		/// 每一个具体配置项
		/// </summary>
		[ConfigurationProperty("rolesDefine")]
		public RolesDefineCollection RolesDefineCollection
		{
			get
			{
				return (RolesDefineCollection)base["rolesDefine"];
			}
		}

		/// <summary>
		/// 获取配置节中，指定Key所对应的角色
		/// </summary>
		/// <param name="roleConfigKeys"></param>
		/// <returns></returns>
		public IRole[] GetRolesInstances(params string[] roleConfigKeys)
		{
			roleConfigKeys.NullCheck("roleConfigKeys");

			Dictionary<string, IRole> roleDict = new Dictionary<string, IRole>(StringComparer.OrdinalIgnoreCase);

			foreach (string roleKey in roleConfigKeys)
			{
				RolesDefine rf = RolesDefineCollection[roleKey];

				if (rf != null)
				{
					IRole[] roles = rf.GetRolesInstances();

					roles.ForEach(r => roleDict[r.FullCodeName] = r);
				}
			}

			List<IRole> result = new List<IRole>();

			foreach (KeyValuePair<string, IRole> kp in roleDict)
				result.Add(kp.Value);

			return result.ToArray();
		}

		/// <summary>
		/// 当前用户是否在已经配置的角色中
		/// </summary>
		/// <param name="roleConfigKeys"></param>
		/// <returns></returns>
		public bool IsCurrentUserInRoles(params string[] roleConfigKeys)
		{
			return IsCurrentUserInRoles(DeluxeIdentity.CurrentUser, roleConfigKeys);
		}

		/// <summary>
		/// 某个用户是否在已经配置的角色中
		/// </summary>
		/// <param name="user"></param>
		/// <param name="roleConfigKeys"></param>
		/// <returns></returns>
		public bool IsCurrentUserInRoles(IUser user, params string[] roleConfigKeys)
		{
			user.NullCheck("user");
			ExceptionHelper.FalseThrow<ArgumentNullException>(roleConfigKeys != null, "roleConfigKeys");

			bool result = false;

			foreach (string roleKey in roleConfigKeys)
			{
				RolesDefine rf = RolesDefineCollection[roleKey];

				if (rf != null)
				{
					if (DeluxePrincipal.IsInRole(user, rf.Roles))
					{
						result = true;
						break;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// 检查当前用户是否属于某角色
		/// </summary>
		/// <param name="roleConfigKey"></param>
		public void CheckCurrentUserInRole(string roleConfigKey)
		{
			CheckCurrentUserInRole(DeluxeIdentity.CurrentUser, roleConfigKey);
		}

		/// <summary>
		/// 检查某个用户是否属于某角色
		/// </summary>
		/// <param name="user"></param>
		/// <param name="roleConfigKey"></param>
		public void CheckCurrentUserInRole(IUser user, string roleConfigKey)
		{
			ExceptionHelper.CheckStringIsNullOrEmpty("roleConfigKey", "roleConfigKey");

			RolesDefine rf = RolesDefineCollection[roleConfigKey];

			ExceptionHelper.FalseThrow(rf != null, "您没有权限执行此操作，不能查到角色配置信息\"{0}\"，请检查rolesDefineConfig配置节", roleConfigKey);
			ExceptionHelper.FalseThrow(DeluxePrincipal.IsInRole(user, rf.Roles), "您不属于\"{0}\"，没有权限执行此操作", rf.Description);
		}
	}

	/// <summary>
	/// 逻辑角色定义的集合
	/// </summary>
	public class RolesDefineCollection : NamedConfigurationElementCollection<RolesDefine> { }

	/// <summary>
	/// 逻辑角色
	/// </summary>
	public class RolesDefine : NamedConfigurationElement
	{
		/// <summary>
		/// 授权系统中的角色
		/// </summary>
		[ConfigurationProperty("roles", IsRequired = true)]
		public string Roles
		{
			get
			{
				return this["roles"].ToString();
			}
		}

		/// <summary>
		/// 得到定义的角色的实例
		/// </summary>
		/// <returns></returns>
		public IRole[] GetRolesInstances()
		{
			return DeluxePrincipal.GetRoles(Roles);
		}
	}
}

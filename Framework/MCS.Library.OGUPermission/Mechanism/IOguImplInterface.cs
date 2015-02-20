using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.OGUPermission
{
	/// <summary>
	/// 机构人员对象实现时所用到的方法
	/// </summary>
	public interface IOguImplInterface
	{
		/// <summary>
		/// 返回机构对象的子对象
		/// </summary>
		/// <typeparam name="T">期望返回的结果对象的类型，IOrganization、IUser、IGroup或IOguObject</typeparam>
		/// <param name="parent">父机构对象</param>
		/// <param name="includeSideline">对象的类型</param>
		/// <param name="searchLevel">查询的深度，单级或所有子对象</param>
		/// <returns></returns>
		OguObjectCollection<T> GetChildren<T>(IOrganization parent, bool includeSideline, SearchLevel searchLevel) where T : IOguObject;

		/// <summary>
		/// 在子对象进行查询（所有级别深度）
		/// </summary>
		/// <typeparam name="T">期望的类型</typeparam>
		/// <param name="parent">父机构对象</param>
		/// <param name="matchString">模糊查询的字符串</param>
		/// <param name="includeSideLine">是否包含兼职的人员</param>
		/// <param name="level">查询的深度</param>
		/// <param name="returnCount">返回的记录数</param>
		/// <returns>得到查询的子对象</returns>
		OguObjectCollection<T> QueryChildren<T>(IOrganization parent, string matchString, bool includeSideLine, SearchLevel level, int returnCount) where T : IOguObject;

		/// <summary>
		/// 初始化祖先的OUs
		/// </summary>
		/// <param name="currentObj"></param>
		void InitAncestorOUs(IOguObject currentObj);

		/// <summary>
        /// 得到某用户的所有相关用户信息，包括主职和兼职的
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <returns>所有相关用户信息，包括主职和兼职的</returns>
		OguObjectCollection<IUser> GetAllRelativeUserInfo(IUser user);

		/// <summary>
		/// 得到某用户属于的所有组
		/// </summary>
		/// <param name="user">用户对象</param>
		/// <returns>用户属于的组</returns>
		OguObjectCollection<IGroup> GetGroupsOfUser(IUser user);

		/// <summary>
        /// 得到组当中的用户
        /// </summary>
        /// <param name="group">组对象</param>
        /// <returns>组当中的用户</returns>
		OguObjectCollection<IUser> GetGroupMembers(IGroup group);

		/// <summary>
        /// 得到某个用户的秘书
        /// </summary>
        /// <param name="user">某个用户</param>
        /// <returns>用户的秘书</returns>
		OguObjectCollection<IUser> GetSecretaries(IUser user);

		/// <summary>
        /// 得到某个用户，属于谁的秘书
        /// </summary>
        /// <param name="user">某个用户</param>
        /// <returns>是谁的秘书</returns>
		OguObjectCollection<IUser> GetSecretaryOf(IUser user);
	}
}

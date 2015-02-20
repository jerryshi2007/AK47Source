using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MCS.Library.Core;

namespace MCS.Library.OGUPermission
{
	/// <summary>
	/// 机构对象的缺省实现类
	/// </summary>
	[Serializable]
	public class OguOrganizationImpl : OguBaseImpl, IOrganization, MCS.Library.OGUPermission.IOrganizationPropertyAccessible
	{
		private string customsCode = string.Empty;
		private DepartmentTypeDefine departmentType;
		private DepartmentClassType departmentClass;
		private DepartmentRankType rank;
		private OguObjectCollection<IOguObject> children = null;

		#region Sync Objects
		private object childrenSyncObj = new object();
		#endregion

		/// <summary>
		/// 构造方法
		/// </summary>
		public OguOrganizationImpl()
		{
		}

		#region IOrganization 成员

		/// <summary>
		/// 关区号
		/// </summary>
		public string CustomsCode
		{
			get { return this.customsCode; }
			set { this.customsCode = value; }
		}

		/// <summary>
		/// 部门的类型
		/// </summary>
		public DepartmentTypeDefine DepartmentType
		{
			get { return this.departmentType; }
			set { this.departmentType = value; }
		}

		/// <summary>
		/// 部门的类别
		/// </summary>
		public DepartmentClassType DepartmentClass
		{
			get { return this.departmentClass; }
			set { this.departmentClass = value; }
		}

		/// <summary>
		/// 部门的级别
		/// </summary>
		public DepartmentRankType Rank
		{
			get { return this.rank; }
			set { this.rank = value; }
		}

		/// <summary>
		/// 是否是顶级部门
		/// </summary>
		public bool IsTopOU
		{
			get
			{
				return TopOU.ID == this.ID;
			}
		}

		/// <summary>
		/// 部门的第一级子成员
		/// </summary>
		public OguObjectCollection<IOguObject> Children
		{
			get
			{
				if (this.children == null)
				{
					lock (this.childrenSyncObj)
					{
						if (this.children == null)
							this.children = GetChildren<IOguObject>(true, SearchLevel.OneLevel);
					}
				}

				return this.children;
			}
		}

		/// <summary>
		/// 部门的所有子成员(递归)
		/// </summary>
		/// <typeparam name="T">子成员的类型</typeparam>
		/// <param name="includeSideLine">是否包含兼职</param>
		/// <returns>部门的所有子成员</returns>
		public OguObjectCollection<T> GetAllChildren<T>(bool includeSideLine) where T : IOguObject
		{
			return GetChildren<T>(includeSideLine, SearchLevel.SubTree);
		}

		/// <summary>
		/// 查询子成员
		/// </summary>
		/// <typeparam name="T">子成员的类型</typeparam>
		/// <param name="matchString">查询串</param>
		/// <param name="includeSideLine">是否包含兼职</param>
		/// <param name="level">是否递归查找</param>
		/// <param name="returnCount">返回的记录数</param>
		/// <returns>查询结果</returns>
		public OguObjectCollection<T> QueryChildren<T>(string matchString, bool includeSideLine, SearchLevel level, int returnCount) where T : IOguObject
		{
			return OguPermissionSettings.GetConfig().OguObjectImpls.QueryChildren<T>(this, matchString, includeSideLine, level, returnCount);
		}

		#endregion

		/// <summary>
		/// 初始化属性
		/// </summary>
		/// <param name="row"></param>
		public override void InitProperties(DataRow row)
		{
			base.InitProperties(row);

			this.rank = ConvertDeptRankCode(Common.GetDataRowTextValue(row, "RANK_CODE"));
			this.customsCode = Common.GetDataRowTextValue(row, "CUSTOMS_CODE");

			this.departmentType = ConvertDepartmentType(Common.GetDataRowValue(row, "ORG_TYPE", 0));
			ObjectType = SchemaType.Organizations;
			this.departmentClass = ConvertDepartmentClass(Common.GetDataRowValue(row, "ORG_CLASS", 0));
		}

		/// <summary>
		/// 得到该部门的子成员
		/// </summary>
		/// <typeparam name="T">子成员的类型</typeparam>
		/// <param name="includeSideLine">是否包含兼职</param>
		/// <param name="searchLevel">是否递归</param>
		/// <returns>该部门的子成员</returns>
		protected virtual OguObjectCollection<T> GetChildren<T>(bool includeSideLine, SearchLevel searchLevel) where T : IOguObject
		{
			OguObjectCollection<T> result = OguPermissionSettings.GetConfig().OguObjectImpls.GetChildren<T>(this, includeSideLine, searchLevel);

			NormalizeChildrenFullPath(this, result);

			return result;
		}

		/// <summary>
		/// 填充子对象的FullPath属性。
		/// 有些情况下，子对象没有FullPath。当它没有时，可以根据父对象的FullPath和自身的名字拼接出来
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="parent"></param>
		/// <param name="children"></param>
		private static void NormalizeChildrenFullPath<T>(IOrganization parent, OguObjectCollection<T> children) where T : IOguObject
		{
			if (parent.FullPath.IsNotEmpty())
			{
				foreach (IOguObject obj in children)
				{
					if (obj is OguBaseImpl && obj.FullPath.IsNullOrEmpty() && obj.Name.IsNotEmpty())
						((OguBaseImpl)obj).FullPath = parent.FullPath + "\\" + obj.Name;
				}
			}
		}

		private static DepartmentTypeDefine ConvertDepartmentType(int nType)
		{
			DepartmentTypeDefine attr = DepartmentTypeDefine.Unspecified;

			if ((nType & 1) != 0)
				attr |= DepartmentTypeDefine.XuNiJiGou;

			if ((nType & 2) != 0)
				attr |= DepartmentTypeDefine.YiBanBuMen;

			if ((nType & 4) != 0)
				attr |= DepartmentTypeDefine.BanGongShi;

			if ((nType & 8) != 0)
				attr |= DepartmentTypeDefine.ZongHeChu;

			if ((nType & 16) != 0)
				attr |= DepartmentTypeDefine.JiSiJu;

			return attr;
		}

		private DepartmentClassType ConvertDepartmentClass(int nClass)
		{
			this.departmentClass = DepartmentClassType.Unspecified;

			if ((nClass & 32) != 0)
				this.departmentClass |= DepartmentClassType.LiShuHaiGuan;

			if ((nClass & 64) != 0)
				this.departmentClass |= DepartmentClassType.PaiZhuJiGou;

			if ((nClass & 128) != 0)
				this.departmentClass |= DepartmentClassType.NeiSheJiGou;

			if ((nClass & 256) != 0)
				this.departmentClass |= DepartmentClassType.QiTaJiGou;

			return this.departmentClass;
		}

		private static DepartmentRankType ConvertDeptRankCode(string strRCode)
		{
			DepartmentRankType innerRank = DepartmentRankType.YiBanBuMen;

			switch (strRCode.ToUpper())
			{
				case "POS_MINISTRY_D":
					innerRank = DepartmentRankType.ZhengBuJi;
					break;
				case "SUB_MINISTRY_D":
					innerRank = DepartmentRankType.FuBuJi;
					break;
				case "POS_OFFICE_D":
					innerRank = DepartmentRankType.ZhengJuJi;
					break;
				case "SUB_OFFICE_D":
					innerRank = DepartmentRankType.FuJuJi;
					break;
				case "POS_ORGAN_D":
					innerRank = DepartmentRankType.ZhengChuJi;
					break;
				case "SUB_ORGAN_D":
					innerRank = DepartmentRankType.FuChuJi;
					break;
				case "POS_DEPART_D":
					innerRank = DepartmentRankType.ZhengKeJi;
					break;
				case "SUB_DEPART_D":
					innerRank = DepartmentRankType.FuKeji;
					break;
				case "COMMON_D":
					innerRank = DepartmentRankType.YiBanBuMen;
					break;
				case "SUSCEPTIVITY_D":
					innerRank = DepartmentRankType.MinGanJiBie;
					break;
			}

			return innerRank;
		}
	}

	/// <summary>
	/// 角色中的机构信息
	/// </summary>
	[Serializable]
	public class OguOrganizationInRoleImpl : OguOrganizationImpl, IOrganizationInRole
	{
		private UserRankType accessLevel = UserRankType.Unspecified;

		/// <summary>
		/// 构造方法
		/// </summary>
		public OguOrganizationInRoleImpl()
		{
		}

		#region IOrganizationInRoles 成员

		/// <summary>
		/// 访问级别
		/// </summary>
		public UserRankType AccessLevel
		{
			get { return this.accessLevel; }
			set { this.accessLevel = value; }
		}

		#endregion

		/// <summary>
		/// 初始化属性
		/// </summary>
		/// <param name="row"></param>
		public override void InitProperties(DataRow row)
		{
			base.InitProperties(row);

			this.accessLevel = Common.ConvertUserRankCode(Common.GetDataRowTextValue(row, "ACCESS_LEVEL"));
		}

		/// <summary>
		/// 得到该部门的子成员
		/// </summary>
		/// <typeparam name="T">子成员的类型</typeparam>
		/// <param name="includeSideLine">是否包含兼职</param>
		/// <param name="searchLevel">是否递归</param>
		/// <returns>该部门的子成员</returns>
		protected override OguObjectCollection<T> GetChildren<T>(bool includeSideLine, SearchLevel searchLevel)
		{
			OguObjectCollection<T> children = base.GetChildren<T>(includeSideLine, searchLevel);

			List<T> list = new List<T>();

			foreach (T obj in children)
			{
				if (obj.ObjectType == SchemaType.Users)
				{
					IUser user = (IUser)obj;

					if (user.Rank >= this.AccessLevel)
						list.Add(obj);
				}
				else
					list.Add(obj);
			}

			return new OguObjectCollection<T>(list);
		}
	}
}

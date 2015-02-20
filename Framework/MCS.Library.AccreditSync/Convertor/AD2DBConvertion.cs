using System;
using System.Transactions;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.DirectoryServices;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MCS.Library.Core;
using MCS.Library;
using MCS.Library.Data;
using MCS.Library.Data.Builder;

namespace MCS.Library.Accredit
{
	/// <summary>
	/// AD2DB的转换程序
	/// </summary>
	public class AD2DBConvertion
	{
		private AD2DBInitialParams initialParams = null;

		public AD2DBConvertion(AD2DBInitialParams initParams)
		{
			ExceptionHelper.FalseThrow<ArgumentNullException>(initParams != null, "initParams");

			this.initialParams = initParams;
		}

		public void DoConvert()
		{
			this.initialParams.Log.Write("初始化RootOU: " + this.initialParams.Root.Path + "  时间是：" + DateTime.Now.ToString());

			AD2DBTransferContext transferContext = new AD2DBTransferContext(this.initialParams);
			transferContext.InitContext();

			ADDataOperation ado = new ADDataOperation(transferContext);
			ado.ExecuteConvertion(transferContext);

			CompareADAndDBData(transferContext);

			this.initialParams.Log.Write("结束同步，时间是：" + DateTime.Now.ToString());
		}

		/// <summary>
		/// 对比AD和DB中的数据，然后更新到OA数据库中
		/// </summary>
		private void CompareADAndDBData(AD2DBTransferContext context)
		{
			//查询对比组织机构、用户、和用户组织机构关系表
			OrginalOguDataOperation ogu = new OrginalOguDataOperation(context);
			ogu.CompareAndModifyData();
			OrginalUsersDataOperation users = new OrginalUsersDataOperation(context);
			users.CompareAndModifyData();
			OrginalOUUsersDataOperation ouusers = new OrginalOUUsersDataOperation(context);
			ouusers.CompareAndModifyData();
			OrginalUserInfoExtendDataOperation userinfo = new OrginalUserInfoExtendDataOperation(context);
			userinfo.CompareAndModifyData();

			using (DbContext ctx = DbContext.GetContext(this.initialParams.AccreditAdminConnectionName))
			{
				DeleteDB(context);//先删除3个表的信息、然后更新3个表的信息、最后添加三个表的信息。
				UpdateDB(context);
				AddDB(context);
			}

			using (DbContext ctx = DbContext.GetContext(this.initialParams.UserInfoExtend))
			{
				OrginalUserInfoExtendDataOperation userinfoExtend = new OrginalUserInfoExtendDataOperation(context);
				userinfoExtend.DeleteOperation();
				userinfoExtend.UpdateOperation();
				userinfoExtend.AddOperation();
			}
		}

		private void DeleteDB(AD2DBTransferContext context)
		{
			OrginalOguDataOperation ogu = new OrginalOguDataOperation(context);
			ogu.DeleteOperation();
			OrginalUsersDataOperation users = new OrginalUsersDataOperation(context);
			users.DeleteOperation();
			OrginalOUUsersDataOperation ouusers = new OrginalOUUsersDataOperation(context);
			ouusers.DeleteOperation();

			//开始记录更新变化量

			this.initialParams.Log.Write(string.Format("ORGANIZATIONS表删除数量是：{0},USERS表删除数量是：{1},OUUSERS表删除数量是：{2}",
									ogu.DeleteCount.ToString(), users.DeleteCount.ToString(), ouusers.DeleteCount.ToString()));
		}

		private void UpdateDB(AD2DBTransferContext context)
		{
			OrginalOguDataOperation ogu = new OrginalOguDataOperation(context);
			ogu.UpdateOperation();
			OrginalUsersDataOperation users = new OrginalUsersDataOperation(context);
			users.UpdateOperation();
			OrginalOUUsersDataOperation ouusers = new OrginalOUUsersDataOperation(context);
			ouusers.UpdateOperation();

			this.initialParams.Log.Write(string.Format("ORGANIZATIONS表更新数量是：{0},USERS表更新数量是：{1},OUUSERS表更新数量是：{2}",
									ogu.UpdateCount.ToString(), users.UpdateCount.ToString(), ouusers.UpdateCount.ToString()));
		}

		private void AddDB(AD2DBTransferContext context)
		{
			OrginalOguDataOperation ogu = new OrginalOguDataOperation(context);
			ogu.AddOperation();
			OrginalUsersDataOperation users = new OrginalUsersDataOperation(context);
			users.AddOperation();
			OrginalOUUsersDataOperation ouusers = new OrginalOUUsersDataOperation(context);
			ouusers.AddOperation();

			this.initialParams.Log.Write(string.Format("ORGANIZATIONS表添加数量是：{0},USERS表添加数量是：{1},OUUSERS表添加数量是：{2}",
								   ogu.AddCount.ToString(), users.AddCount.ToString(), ouusers.AddCount.ToString()));
		}
	}
}

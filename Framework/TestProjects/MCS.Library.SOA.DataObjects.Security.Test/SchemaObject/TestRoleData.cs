using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.SOA.DataObjects.Security;
using System.IO;

namespace MCS.Library.SOA.DataObjects.Security.Test.SchemaObject
{
	/// <summary>
	/// 用于测试的Role所包含的相关信息
	/// </summary>
	public class TestRoleData
	{
		public SCRole Role
		{
			get;
			set;
		}

		public SCUser UserInOrg
		{
			get;
			set;
		}

		public SCUser SidelineUserInOrg
		{
			get;
			set;
		}

		public SCUser UserInGroup
		{
			get;
			set;
		}

		public SCUser UserInConditionGroup
		{
			get;
			set;
		}

		public SCUser UserInRole
		{
			get;
			set;
		}

		public SCUser UserInConditionRole
		{
			get;
			set;
		}

		public SCOrganization Organization
		{
			get;
			set;
		}

		public SCOrganization SidelineOrganization
		{
			get;
			set;
		}

		public SCGroup Group
		{
			get;
			set;
		}

		public SCRole BuiltInFunctionRole
		{
			get;
			set;
		}

		public override string ToString()
		{
			string result = base.ToString();

			if (this.Role != null)
			{
				StringBuilder strB = new StringBuilder();

				using (TextWriter writer = new StringWriter(strB))
				{
					writer.WriteLine("Role = {0}", this.Role.ID);
					writer.WriteLine("BuiltInFunctionRole = {0}", this.BuiltInFunctionRole.ID);
					writer.WriteLine("Organization = {0}", this.Organization.ID);
					writer.WriteLine("SidelineOrganization = {0}", this.SidelineOrganization.ID);
					writer.WriteLine("SidelineUserInOrg = {0}", this.SidelineUserInOrg.ID);
					writer.WriteLine("Group = {0}", this.Group.ID);
					writer.WriteLine("UserInRole = {0}", this.UserInRole.ID);
					writer.WriteLine("UserInOrg = {0}", this.UserInOrg.ID);
					writer.WriteLine("UserInConditionRole = {0}", this.UserInConditionRole.ID);
					writer.WriteLine("UserInConditionGroup = {0}", this.UserInConditionGroup.ID);
				}

				result = strB.ToString();
			}

			return result;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MCS.Library.Core;
using MCS.Library.SOA.DataObjects.Schemas.SchemaProperties;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.DataObjects.Security.Persiter
{
	/// <summary>
	/// 用户口令属性的Persister
	/// </summary>
	public class UserPasswordPersister : UserPasswordPersisterBase<SchemaPropertyValue>
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="currentProperty"></param>
		/// <param name="context"></param>
		public override void Read(SchemaPropertyValue currentProperty, PersisterContext<SchemaPropertyValue> context)
		{
			base.Read(currentProperty, context);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="currentProperty"></param>
		/// <param name="context"></param>
		public override void Write(SchemaPropertyValue currentProperty, PersisterContext<SchemaPropertyValue> context)
		{
			/*string value = currentProperty.StringValue;

		EditorParamsDefine paraDefine = null;
		if (currentProperty.Definition.EditorParams.IsNotEmpty())
		{
			base.Register();
			paraDefine = JSONSerializerExecute.Deserialize<EditorParamsDefine>(currentProperty.Definition.EditorParams);
		}
		
		UserPassword currentUserPassword = new UserPassword();

		if (paraDefine.ContainsKey("UserIDMapping") == true)
			currentUserPassword.UserID = context.Properties[paraDefine["UserIDMapping"]].StringValue;
		else
			currentUserPassword.UserID = context.Properties["CodeName"].StringValue;

		if (paraDefine.ContainsKey("UserPasswordIDMapping") == true)
			currentUserPassword.UserID = context.Properties[paraDefine["UserPasswordIDMapping"]].StringValue;
		else
			currentUserPassword.UserID = context.Properties["ID"].StringValue;

		if (value.IsNotEmpty())
		{
			if (string.Compare(currentProperty.StringValue, currentProperty.Definition.DefaultValue, true) != 0)
				currentUserPassword.Password = currentProperty.StringValue;
			else
				currentUserPassword.Password = currentProperty.Definition.DefaultValue;
		}
		else
			currentUserPassword.Password = currentProperty.Definition.DefaultValue;

		currentProperty.StringValue = PwdCalculate("", string.Format("{0},{1}", currentUserPassword.UserID, currentUserPassword.Password));
		currentUserPassword.Password = currentProperty.StringValue;

		UserPasswordAdapter.Instance.Update(currentUserPassword); */
		}

	}
}

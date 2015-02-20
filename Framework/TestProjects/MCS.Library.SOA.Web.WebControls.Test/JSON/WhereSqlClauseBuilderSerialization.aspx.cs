using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Data.Builder;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.Web.WebControls.Test.JSON
{
	public partial class WhereSqlClauseBuilderSerialization : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			WhereSqlClauseBuilder builder = GenerateBuilder();

			string data = JSONSerializerExecute.Serialize(GenerateBuilder());

			dataDiv.InnerText = data;
		}

		private static WhereSqlClauseBuilder GenerateBuilder()
		{
			WhereSqlClauseBuilder builder = new WhereSqlClauseBuilder();

			builder.AppendItem("ID", UuidHelper.NewUuidString());
			builder.AppendItem("Name", "Turtle");
			builder.AppendItem("Age", 250);

			builder.LogicOperator = LogicOperatorDefine.Or;

			return builder;
		}
	}
}
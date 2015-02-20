using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;
using MCS.Library.Passport;
using MCS.Library.Core;

namespace MCS.Web.Passport.TestPages
{
	public partial class DesKeyGenerator : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void generateButton_Click(object sender, EventArgs e)
		{
			DES des = DES.Create();

			des.GenerateKey();
			des.GenerateIV();

			keyValue.InnerText = des.Key.ToBase16String();
			IVValue.InnerText = des.IV.ToBase16String();
		}
	}
}

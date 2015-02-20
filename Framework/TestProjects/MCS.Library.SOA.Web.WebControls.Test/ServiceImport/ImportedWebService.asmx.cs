﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace MCS.Library.SOA.Web.WebControls.Test.ServiceImport
{
	/// <summary>
	/// Summary description for ImportedWebService
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[System.ComponentModel.ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
	// [System.Web.Script.Services.ScriptService]
	public class ImportedWebService : System.Web.Services.WebService
	{

		[WebMethod]
		public string HelloWorld()
		{
			return "Hello World";
		}

		[WebMethod]
		public int Add(int a, int b)
		{
			return a + b;
		}

		[WebMethod]
		public TestUser GetAlternatedUser(TestUser user)
		{
			user.Name = "周杨";
			user.Age = 250;

			return user;
		}
	}
}

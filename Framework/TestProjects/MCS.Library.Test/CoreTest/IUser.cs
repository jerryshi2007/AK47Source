using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCS.Library.Test
{
	public interface IUser
	{
		string ID
		{
			get;
			set;
		}

		string Name
		{
			get;
			set;
		}
	}
}

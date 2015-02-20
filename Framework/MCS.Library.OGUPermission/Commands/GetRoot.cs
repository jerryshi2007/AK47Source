using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Data;

namespace MCS.Library.OGUPermission.Commands
{
	/// <summary>
	/// 
	/// </summary>
    public class GetRoot : CommandBase
    {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
        public GetRoot(string name)
            : base(name)
        {

        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="argument"></param>
        public override void Execute(string argument)
        {
            var root = OGUPermission.OguMechanismFactory.GetMechanism().GetRoot();
            OutputHelper.OutputObjectInfo(root);
        }

		/// <summary>
		/// 
		/// </summary>
        public override string HelperString
        {
            get
            {
                return "getRoot";
            }
        }
    }
}

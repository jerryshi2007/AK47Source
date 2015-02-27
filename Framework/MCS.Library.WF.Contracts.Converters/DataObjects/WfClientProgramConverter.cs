using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Library.WF.Contracts.Converters.DataObjects
{
    public class WfClientProgramConverter
    {
        public static readonly WfClientProgramConverter Instance = new WfClientProgramConverter();

        private WfClientProgramConverter()
        {
        }
    }
}

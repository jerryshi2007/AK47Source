using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MCS.Library.Services;
using MCS.Library.Accredit;

namespace MCS.Services.AD2Accredit
{
    public class ADGroupToAccreditService : ThreadTaskBase
    {
        public override void OnThreadTaskStart()
        {
            ADGroup2DBConvertion converter = new ADGroup2DBConvertion();
            converter.DoConvert();
        }
    }
}

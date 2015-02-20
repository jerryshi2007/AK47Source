using MCS.Library;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestADHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            var helper = ADHelper.GetInstance();

            DirectoryEntry entry = helper.CreateEntry("DC=sinooceanland,DC=com");


        }
    }
}

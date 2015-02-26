using MCS.Library.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCS.Web.Library.Script.Json.Test
{
    public class JsonTestObj
    {
        public string ID
        {
            get;
            set;
        }

        public int Age
        {
            get;
            set;
        }

        public DateTime Birthday
        {
            get;
            set;
        }

        public static JsonTestObj PrepareData()
        {
            JsonTestObj result = new JsonTestObj();

            result.ID = UuidHelper.NewUuidString();
            result.Age = 41;
            result.Birthday = new DateTime(1972, 4, 26, 12, 40, 0, DateTimeKind.Local);

            return result;
        }
    }
}

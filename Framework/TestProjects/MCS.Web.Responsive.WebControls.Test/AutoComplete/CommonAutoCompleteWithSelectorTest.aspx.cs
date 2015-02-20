using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Library.Data.DataObjects;

namespace MCS.Web.Responsive.WebControls.Test
{
    public partial class CommonAutoCompleteWithSelectorTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //暂时注释掉初始化的数据
                //var data = new List<CommonData>();

                //data.Add(new CommonData() { Code = "123456789", Name = "Server初始数据1", Detail = "TestDataDetail1" });
                //data.Add(new CommonData() { Code = "12345678910", Name = "Server初始数据2", Detail = "TestDataDetail2" });
                //data.Add(new CommonData() { Code = "123456789102", Name = "Server初始数据3", Detail = "TestDataDetail3" });

                //CommonAutoCompleteWithSelectorControl1.SelectedData = data;
            }
        }
        private List<CommonData> DoValidate(string str, object context)
        {
            var data = new List<CommonData>();

            if (str == "1")
            {
                data.Add(new CommonData() { Code = "Code_" + 1, Name = "TestDataName" + 1, Detail = "TestDataDetail" + 1 });
                data.Add(new CommonData() { Code = "Code1_" + 1, Name = "TestDataName1" + 1, Detail = "TestDataDetail1" + 1 });
            }
            else if (str == "2")
            {
                data.Add(new CommonData() { Code = "Code_" + 2, Name = "TestDataName" + 2, Detail = "TestDataDetail" + 2 });
            }
            else if (str == "3")
            {
                data.Add(new CommonData() { Code = "Code_" + 3, Name = "TestDataName" + 3, Detail = "TestDataDetail" + 3 });
            }
            else if (str == "*")
            {
                for (int i = 0; i < 5; i++)
                {
                    data.Add(new CommonData() { Code = "Code_" + i, Name = "TestDataName" + i, Detail = "TestDataDetail" + i });
                }
            }

            if (context != null)
            {
                data.RemoveAll(p => p.Code != "Code_" + context.ToString());
            }

            return data;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

        }

        protected IList CommonAutoCompleteWithSelectorControl1_ValidateInput(string chkString, object context)
        {
            return DoValidate(chkString, context) as IList;
        }

        protected IEnumerable CommonAutoCompleteWithSelectorControl1_GetDataSource(string chkString, object context)
        {
            IEnumerable result = null;

            var data = new List<object>();

            if (chkString == "1")
            {
                data.Add(new CommonData() { Code = "Code_" + 1, Name = "TestDataName" + 1, Detail = "TestDataDetail" + 1 });
            }
            else if (chkString == "2" && context.ToString() == "2")
            {
                data.Add(new CommonData() { Code = "Code_" + 2, Name = "TestDataName" + 2, Detail = "TestDataDetail" + 2 });
            }
            else if (chkString == "3" && context.ToString() == "3")
            {
                data.Add(new CommonData() { Code = "Code_" + 3, Name = "TestDataName" + 3, Detail = "TestDataDetail" + 3 });
            }
            else if (chkString == "*")
            {
                for (int i = 0; i < 5; i++)
                {
                    data.Add(new CommonData() { Code = "Code_" + i, Name = "TestDataName" + i, Detail = "TestDataDetail" + i });
                }
            }

            result = data;

            return result;
        }
    }
}
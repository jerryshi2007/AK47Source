using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using MCS.Library.Core;
using MCS.Web.Library.Script;

namespace MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest
{
    public partial class dateTimeToJsonTest : System.Web.UI.Page
    {
        public class DateTimeWrapper
        {
            public DateTime DateTimeData
            {
                get;
                set;
            }
        }

        private DateTimeWrapper Data = new DateTimeWrapper();

        protected override void OnPreInit(EventArgs e)
        {
            //JSONSerializerExecute.RegisterConverter(typeof(DateTimeTestConverter));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                doWork();
            }
        }

        private void doWork()
        {
            Data.DateTimeData = DateTime.Now;

            Label0.Text = "当前值：" + Data.DateTimeData.ToString("yyyy-MM-dd HH:mm:ss");

            Label1.Text = SaveClientState();
        }

        #region  1
        protected string SaveClientState()
        {
            string result = string.Empty;

            result = JSONSerializerExecute.Serialize(Data);

            return result;
        }

        protected void LoadClientState(string clientState)
        {
            this.Data = JSONSerializerExecute.Deserialize<DateTimeWrapper>(clientState);
        }
        #endregion
        

        protected void Button1_Click(object sender, EventArgs e)
        {
            LoadClientState(Label1.Text);
            Label2.Text = "反序列化值：" + this.Data.DateTimeData.ToString();
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            doWork();
        }

    }

    public class DateTimeTestConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            DateTime entity = new DateTime();

            //entity.myDateTime = DictionaryHelper.GetValue(dictionary, "myDateTime", DateTime.MinValue);

            return entity;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            DateTime entity = (DateTime)obj;

            Dictionary<string, object> dictionary = new Dictionary<string, object>();


            dictionary.Add("", "ss");

            return dictionary;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new System.Type[] { typeof(DateTime) };
            }
        }
    }
}
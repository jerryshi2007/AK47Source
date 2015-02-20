using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MCS.Web.WebControls;

namespace MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest
{
    public partial class ServerDefine : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            clientGrid1.InitialData = dataSource();
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DefineClientGridColumns1();
        }


        private List<PaymentDetails1> dataSource()
        {
            List<PaymentDetails1> list = new List<PaymentDetails1>();
            for (int i = 0; i < 10; i++)
            {
                PaymentDetails1 entity = new PaymentDetails1();
                entity.index = i + 1;
                entity.PaymentItem = "垂虹园二期项目";
                entity.ExchangeRate = 0.8;
                entity.RMB = 100999.0909;
                entity.Totle = 50.00;

                if (i % 2 == 0)
                    entity.Currency = "0.91"; //港币
                else
                    entity.Currency = "1.1920007"; //人民币

                list.Add(entity);
            }
            return list;
        }

        /// <summary>
        /// 定义ClientGrid的列
        /// </summary>
        private void DefineClientGridColumns1()
        {

            //            public class PaymentDetails1
            //{
            //    public int index { get; set; }
            //    public string PaymentItem { get; set; }
            //    public double RMB { get; set; }
            //    public string Currency { get; set; }
            //    public double ExchangeRate { get; set; }
            //    public double Totle { get; set; }
            //}

            clientGrid1.Columns.Clear();

            clientGrid1.Columns.Add(new ClientGridColumn()
            {
                HeaderText = "index",
                DataField = "index",
                EditTemplate = new ClientGridColumnEditTemplate()
                {
                    EditMode = ClientGridColumnEditMode.TextBox
                }
            });

            clientGrid1.Columns.Add(new ClientGridColumn()
            {
                HeaderText = "PaymentItem",
                DataField = "PaymentItem",
                EditTemplate = new ClientGridColumnEditTemplate()
                {
                    EditMode = ClientGridColumnEditMode.TextBox
                }
            });
        }

        /// <summary>
        /// 定义ClientGrid的列
        /// </summary>
        private void DefineClientGridColumns2()
        {
            clientGrid1.Columns.Clear();

            //            public class PaymentDetails1
            //{
            //    public int index { get; set; }
            //    public string PaymentItem { get; set; }
            //    public double RMB { get; set; }
            //    public string Currency { get; set; }
            //    public double ExchangeRate { get; set; }
            //    public double Totle { get; set; }
            //}

            clientGrid1.Columns.Add(new ClientGridColumn()
            {
                HeaderText = "index",
                DataField = "index",
                EditTemplate = new ClientGridColumnEditTemplate()
                {
                    EditMode = ClientGridColumnEditMode.TextBox
                }
            });

            clientGrid1.Columns.Add(new ClientGridColumn()
            {
                HeaderText = "RMB",
                DataField = "RMB",
                EditTemplate = new ClientGridColumnEditTemplate()
                {
                    EditMode = ClientGridColumnEditMode.TextBox
                }
            });

            clientGrid1.Columns.Add(new ClientGridColumn()
            {
                HeaderText = "Totle",
                DataField = "Totle",
                EditTemplate = new ClientGridColumnEditTemplate()
                {
                    EditMode = ClientGridColumnEditMode.TextBox
                }
            });
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (HiddenField1.Value.Length > 0)
            {
                DefineClientGridColumns1();
                clientGrid1.InitialData = dataSource();

                HiddenField1.Value = "";
            }
            else
            {
                DefineClientGridColumns2();
                clientGrid1.InitialData = dataSource();
                HiddenField1.Value = "s";
            }
        }
    }
}
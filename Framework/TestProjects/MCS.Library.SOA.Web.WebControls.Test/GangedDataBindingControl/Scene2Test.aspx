<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Scene2Test.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.Scene2Test" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript">

        Province_Data = [{ "ID": "1", "Name": "河北" }, { "ID": "2", "Name": "山东" }, { "ID": "3", "Name": "海南"}];
        City_Data = {
            "1": [{ "ID": "11", "Name": "石家庄" }, { "ID": "12", "Name": "邢台" }, { "ID": "13", "Name": "保定"}]
            , "2": [{ "ID": "21", "Name": "济南" }, { "ID": "22", "Name": "青岛" }, { "ID": "23", "Name": "烟台"}]
            , "3": [{ "ID": "31", "Name": "海口" }, { "ID": "32", "Name": "三亚"}]
        };
        County_Data = {
            "11": [{ "ID": "111", "Name": "石家庄县" }, { "ID": "112", "Name": "正定"}]
            , "12": [{ "ID": "121", "Name": "邢台县"}]
            , "13": [{ "ID": "131", "Name": "保定县"}]
            , "21": [{ "ID": "211", "Name": "济南县"}]
            , "22": [{ "ID": "221", "Name": "青岛县"}]
            , "23": [{ "ID": "231", "Name": "烟台县"}]
             , "31": [{ "ID": "311", "Name": "海口县"}]
            , "32": [{ "ID": "321", "Name": "三亚县"}]
        };

        function dataBind(viewData) {
            bindDataToDDL($get("DropDownList1"), viewData.provinceData);
            bindDataToDDL($get("DropDownList2"), viewData.cityData);
            bindDataToDDL($get("DropDownList3"), viewData.countyData);

            $addHandler($get("DropDownList1"), "change", function (eventElement) {
                viewData.setValue("selectedProvince", eventElement.target.options[eventElement.target.selectedIndex].value);
            });
        }

        //用户实现
        function onCreateDefaultViewData(bindingCtrl, e) {
            e.viewData = { provinceData: bindingCtrl.makeObservable(Province_Data), selectedProvince: bindingCtrl.makeObservable("2") };

            e.viewData.cityData = bindingCtrl.makeComputable(function () {
                //这里this是e.viewData,由第三个参数指定
                return City_Data[this.selectedProvince.val()];
            }, e.viewData);

            e.viewData.selectedCity = bindingCtrl.makeComputable(function () {
                return this.cityData.val()[0].ID;
            }, e.viewData);

            e.viewData.countyData = bindingCtrl.makeComputable(function () {
                return County_Data[this.selectedCity.val()];
            }, e.viewData);

            e.viewData.selectedCounty = bindingCtrl.makeComputable(function () {
                return this.countyData.val()[0].ID;
            }, e.viewData);

            e.viewData.cityData.subscribeFrom(["selectedProvince"], e.viewData);
            e.viewData.countyData.subscribeFrom(["selectedCity"], e.viewData);

            e.viewData.selectedCity.subscribeFrom(["cityData"], e.viewData);
            e.viewData.selectedCounty.subscribeFrom(["countyData"], e.viewData);
        }
        

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
       <HB:GangedDataBindingControl runat="server" ID="bindingControl" OnCreateDefaultViewData="onCreateDefaultViewData">
            <ItemBindings>
                <HB:GangedDataBindingItem ControlClientID="DropDownList1" BindingSettings="{options:'provinceData',value:'selectedProvince',optionValue:'ID',optionName:'Name'}">
                </HB:GangedDataBindingItem>
                <HB:GangedDataBindingItem ControlClientID="DropDownList2" BindingSettings="{options:'cityData',value:'selectedCity',optionValue:'ID',optionName:'Name'}">
                </HB:GangedDataBindingItem>
                <HB:GangedDataBindingItem ControlClientID="DropDownList3" BindingSettings="{options:'countyData',value:'selectedCounty',optionValue:'ID',optionName:'Name'}">
                </HB:GangedDataBindingItem>
            </ItemBindings>
        </HB:GangedDataBindingControl>
      
        <asp:DropDownList ID="DropDownList1" runat="server">
        </asp:DropDownList>
        <br />
        <asp:DropDownList ID="DropDownList2" runat="server">
        </asp:DropDownList>
        <br />
        <asp:DropDownList ID="DropDownList3" runat="server">
        </asp:DropDownList>
    </div>
    </form>
</body>
</html>

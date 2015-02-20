<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Scene5Test.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.Scene5Test" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        var continents = [{ Id: '1', Name: '亚洲' }, { Id: '2', Name: '美洲'}];
        var countries = {
            '1': [{ Id: '11', Name: '中国' }, { Id: '12', Name: '朝鲜'}],
            '2': [{ Id: '21', Name: '美国' }, { Id: '22', Name: '加拿大'}]
        };
        var units = {
            '11': '元',
            '12': '朝鲜元',
            '21': '美元',
            '22': '加币'
        };
        var exchangeRate = {
            '11': 1,
            '12': 0.001,
            '21': 6,
            '22': 3
        };

        function onCreateDefaultViewData(dataBindingControl, e) {
            e.viewData = function () {
                var my = {};

                my.continentOption = continents;
                my.continentId = dataBindingControl.makeObservable('1');
                my.coefficient = dataBindingControl.makeObservable(0.8);
                my.items = dataBindingControl.observableArray([]);
                my.result = dataBindingControl.makeComputable(function () {
                    var source = this.items.val();
                    var result = 0;
                    for (var i in source) {
                        result += source[i].val().rowResult.val();
                    }
                    return result;
                }, my);

                my.addNewItem = function () {
                    var newItem = {
                        countries: dataBindingControl.makeComputable(function () {
                            return countries[this.continentId.val()];
                        }, my),
                        s1: dataBindingControl.makeObservable(0),
                        s2: dataBindingControl.makeObservable(0),
                        countrieOption: dataBindingControl.makeComputable(function () {
                            return countries[this.continentId.val()];
                        }, my)
                    };
                    newItem.s3 = dataBindingControl.makeComputable(function () {
                        return this.countrieOption.val()[0].Id;
                    }, newItem);
                    newItem.s4 = dataBindingControl.makeComputable(function () { return units[this.s3.val()]; }, newItem);
                    newItem.s5 = dataBindingControl.makeComputable(function () { return exchangeRate[this.s3.val()]; }, newItem);

                    newItem.rowResult = dataBindingControl.makeComputable(function () {
                        return (this.item.s1.val() * 1 + this.item.s2.val() * 1) * exchangeRate[this.item.s3.val()] * this.viewModel.coefficient.val();
                    }, { viewModel: my, item: newItem });

                    newItem.countrieOption.subscribeFrom([["continentId"]], my);
                    newItem.s3.subscribeFrom(["countrieOption"], newItem);
                    newItem.s4.subscribeFrom(["s3"], newItem);
                    newItem.s5.subscribeFrom(["s3"], newItem);
                    newItem.rowResult.subscribeFrom(["coefficient"], my);
                    newItem.rowResult.subscribeFrom(["s1", "s2", "s3", "s4", "s5"], newItem);
                    my.result.subscribeFrom(["rowResult"], newItem);

                    my.items.push(newItem);

                    return newItem;
                };

                return my;
            } ();

        }

        var createRowItemBindings = function () {
            return {
                's1': { value: 's1' },
                's2': { value: 's2' },
                's3': { value: 's3', options: "countrieOption", optionValue: 'Id', optionName: 'Name' },
                's4': { value: 's4' },
                's5': { value: 's5' },
                'rowResult': { value: 'rowResult',format:'{0:N2}' }
            };
        };

        //ClientGrid的添加行之前的事件
        function OnPreRowAdd(control, e) {
            e.rowData = $find("bindingControl").get_viewData().addNewItem();
            e.rowData.itemBindings = createRowItemBindings();
        }

        //ClientGrid的单元格数据绑定后的事件
        function onCellDataBound(grid, e) {
            var settings = e.data.itemBindings[e.column.dataField];
            var control = e.cell.childNodes[0];
            control.id = control.uniqueID;
            $clearHandlers(control);
            $find("bindingControl").dataBindByItemBinding({ ControlClientID: control.id, BindingSettings: settings }, e.data);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <HB:GangedDataBindingControl runat="server" ID="bindingControl" OnCreateDefaultViewData="onCreateDefaultViewData">
        <ItemBindings>
            <HB:GangedDataBindingItem ControlClientID="ddlContinent" BindingSettings="{options:'continentOption',value:'continentId', optionValue: 'Id', optionName: 'Name'}"/>
            <HB:GangedDataBindingItem ControlClientID="txtCoefficient" BindingSettings="{value:'coefficient'}"/>
            <HB:GangedDataBindingItem ControlClientID="container03" BindingSettings="{value:'result',format:'{0:N2}'}"/>
        </ItemBindings>
    </HB:GangedDataBindingControl>
    <div>
        <div style="width: 95%; margin-left: auto; margin-right: auto;">
            洲：<select id="ddlContinent"></select>系数：<input id="txtCoefficient" type="text" />
            <HB:ClientGrid runat="server" ID="clientGrid" Width="100%" ShowEditBar="True" OnPreRowAdd="OnPreRowAdd"
                OnClientCellDataBound="onCellDataBound">
                <Columns>
                    <HB:ClientGridColumn DataField="s1" HeaderText="输入1" ItemStyle="{textAlign:'right'}"
                        EditorStyle="{textAlign:'right'}">
                        <EditTemplate EditMode="TextBox"></EditTemplate>
                    </HB:ClientGridColumn>
                    <HB:ClientGridColumn DataField="s2" HeaderText="输入2" ItemStyle="{textAlign:'right'}"
                        EditorStyle="{textAlign:'right'}">
                        <EditTemplate EditMode="TextBox"></EditTemplate>
                    </HB:ClientGridColumn>
                    <HB:ClientGridColumn DataField="s3" HeaderText="国家" ItemStyle="{textAlign:'center'}">
                        <EditTemplate EditMode="DropdownList"></EditTemplate>
                    </HB:ClientGridColumn>
                    <HB:ClientGridColumn DataField="s4" HeaderText="单位">
                        <EditTemplate EditMode="None"></EditTemplate>
                    </HB:ClientGridColumn>
                    <HB:ClientGridColumn DataField="s5" HeaderText="汇率">
                        <EditTemplate EditMode="None"></EditTemplate>
                    </HB:ClientGridColumn>
                    <HB:ClientGridColumn DataField="rowResult" HeaderText="结果">
                        <EditTemplate EditMode="None"></EditTemplate>
                    </HB:ClientGridColumn>
                </Columns>
            </HB:ClientGrid>
            总计：<span id="container03"></span>
        </div>
    </div>
    </form>
</body>
</html>

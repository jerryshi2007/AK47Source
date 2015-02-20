<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ObserverDemo.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.DataBindingControl.ObserverDemo" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="jquery-1.7.1.min.js" type="text/javascript"></script>
    <script src="DataBindingControl.js" type="text/javascript"></script>
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

        function dataBind() {
            dataBindingControl.dataBindByItems(viewModel, mainScene);
        }

        //以下是开发人员需要完成的部分
        var viewModel = function () {
            var my = {};

            my.continentOption = continents;
            my.continentId = dataBindingControl.observable('');
            my.coefficient = dataBindingControl.observable(0.8);
            my.items = dataBindingControl.observableArray([]);
            my.result = dataBindingControl.computed(function () {
                var source = this.items();
                var result = 0;
                for (var i in source) {
                    result += source[i]().rowResult();
                }
                return result;
            }, my);

            my.addNewItem = function () {

                var newItem = {
                    countries: dataBindingControl.computed(function () {
                        return countries[this.continentId()];
                    }, my),
                    s1: dataBindingControl.observable(0),
                    s2: dataBindingControl.observable(0),
                    countrieOption: dataBindingControl.computed(function () { return countries[this.continentId()]; }, my),
                    s3: dataBindingControl.observable('')
                };
                newItem.s4 = dataBindingControl.computed(function () { return units[this.s3()]; }, newItem);
                newItem.s5 = dataBindingControl.computed(function () { return exchangeRate[this.s3()]; }, newItem);

                newItem.rowResult = dataBindingControl.computed(function () { return (this.item.s1() + this.item.s2()) * exchangeRate[this.item.s3()] * this.viewModel.coefficient(); }, { viewModel: my, item: newItem });

                my.items.push(newItem);

                return newItem;
            };

            return my;
        } ();

        var mainScene = [
            { ContainerID: 'container01', EditMode: 'select', DataPropertyName: 'continentId', Options: viewModel.continentOption, OptionValue: 'Id', OptionText: 'Name' },
            { ContainerID: 'container02', EditMode: 'my:moneyinput', DataPropertyName: 'coefficient' },
            { ContainerID: 'container03', EditMode: 'span', DataPropertyName: 'result', Format: '{0:N2}' }
        ];
        var createRowScene = function (row) {
            return {
                's1': { EditMode: 'my:moneyinput', DataPropertyName: 's1' },
                's2': { EditMode: 'my:moneyinput', DataPropertyName: 's2' },
                's3': { EditMode: 'select', DataPropertyName: 's3', Options: row.countrieOption, OptionValue: 'Id', OptionText: 'Name' },
                's4': { EditMode: 'span', DataPropertyName: 's4' },
                's5': { EditMode: 'span', DataPropertyName: 's5' },
                'rowResult': { EditMode: 'span', DataPropertyName: 'rowResult', Format: '{0:N2}' }
            };
        };

        function OnPreRowAdd(control, e) {
            var data = viewModel.addNewItem();
            e.rowData = { rowData: data, scene: createRowScene(data) };
        }

        function OnCellCreatingEditor(control, e) {
            var scene = e.rowData.scene[e.column.dataField];
            scene.Container = e.container;
            dataBindingControl.dataBindByItems(e.rowData.rowData, scene);
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <HB:DataBindingControl runat="server" ID="bindingControl">
        <ItemBindings>
        </ItemBindings>
    </HB:DataBindingControl>
    <div>
        <div style="width: 95%; margin-left: auto; margin-right: auto;">
            <input type="button" onclick="dataBind();" value="Start" />
            洲：<span id="container01"> </span>系数：<span id="container02"></span>
            <HB:ClientGrid runat="server" ID="clientGrid" Width="100%" ShowEditBar="True" OnPreRowAdd="OnPreRowAdd"
                OnCellCreatingEditor="OnCellCreatingEditor">
                <Columns>
                    <HB:ClientGridColumn DataField="s1" HeaderText="输入1">
                    </HB:ClientGridColumn>
                    <HB:ClientGridColumn DataField="s2" HeaderText="输入2">
                    </HB:ClientGridColumn>
                    <HB:ClientGridColumn DataField="s3" HeaderText="国家">
                    </HB:ClientGridColumn>
                    <HB:ClientGridColumn DataField="s4" HeaderText="单位">
                    </HB:ClientGridColumn>
                    <HB:ClientGridColumn DataField="s5" HeaderText="汇率">
                    </HB:ClientGridColumn>
                    <HB:ClientGridColumn DataField="rowResult" HeaderText="结果">
                    </HB:ClientGridColumn>
                </Columns>
            </HB:ClientGrid>
            总计：<span id="container03"></span>
        </div>
    </div>
    </form>
</body>
</html>

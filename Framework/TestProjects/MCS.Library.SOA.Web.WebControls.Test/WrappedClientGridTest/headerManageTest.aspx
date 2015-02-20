<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="headerManageTest.aspx.cs"
    Inherits="MCS.Library.SOA.Web.WebControls.Test.WrappedClientGridTest.headerManageTest" %>

<%@ Register Assembly="MCS.Library.SOA.Web.WebControls" Namespace="MCS.Web.WebControls"
    TagPrefix="HB" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="ClientGrid.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript">
        function OnPreHeaderRowCreate(grid, e) {

            var tr = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "tr"
				},
				e.container
			);

            var td1 = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "td",
				    properties:
					{
					    colSpan: 3,
					    style: { border: "1px solid #fff" },
					    innerText: 1
					}
				},
                tr
			);

            var td2 = $HGDomElement.createElementFromTemplate(
				{
				    nodeName: "td",
				    properties:
					{
					    colSpan: 2,
					    style: { border: "1px solid #fff" },
					    innerText: 2
					}
				},
				tr
			);
        }
    
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table style="width: 100%" id="WorkItemControl_testtest_ctl04" class="clientGrid"
            cellspacing="0" cellpadding="0">
            <thead>
                <tr>
                    <td colspan="2" feeee="linbinfffffok">
                        <table style="width: 100%; height: 100%">
                            <tbody>
                                <tr class="pager">
                                    <td class="caption">
                                    </td>
                                    <td style="display: none; visibility: hidden">
                                        <span>共1页9条记录</span><a class="pagerButton" disabled href="#">首页</a><span> </span>
                                        <a class="pagerButton" disabled href="#">上一页</a><span> </span><a class="pagerButton"
                                            disabled href="#">下一页</a><span> </span><a class="pagerButton" disabled href="#">末页</a><span>
                                            </span>
                                        <input style="width: 35px" value="1"><input class="pagerGotoButton" value="跳转到" type="button">
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </thead>
            <tfoot>
                <tr>
                    <td colspan="2" feeee="linbinfffffok">
                        <table style="width: 100%; height: 100%">
                            <tbody>
                                <tr class="pager">
                                    <td style="display: none; visibility: hidden">
                                        <span>共1页9条记录</span><a class="pagerButton" disabled href="#">首页</a><span> </span>
                                        <a class="pagerButton" disabled href="#">上一页</a><span> </span><a class="pagerButton"
                                            disabled href="#">下一页</a><span> </span><a class="pagerButton" disabled href="#">末页</a><span>
                                            </span>
                                        <input style="width: 35px" value="1"><input class="pagerGotoButton" value="跳转到" type="button">
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </tfoot>
            <tbody>
                <tr>
                    <td style="vertical-align: top">
                        <table style="width: 100%" border="0" cellspacing="0" cellpadding="0" theid="_mainTable_left">
                            <tbody>
                                <tr>
                                    <td>
                                        <table style="width: 100%; height: 100%" class="mainTable" border="0" cellspacing="0"
                                            cellpadding="0">
                                            <tbody>
                                                <tr class="header">
                                                    <td colspan="4" style="text-align: center; width: 50px; height: 35px; color: #fff;
                                                        border: 1px solid #fff">
                                                        1
                                                    </td>
                                                    <td colspan="5" style="text-align: center; width: 50px; height: 35px; color: #fff;
                                                        border: 1px solid #fff">
                                                        2
                                                    </td>
                                                </tr>
                                                <tr class="header">
                                                    <td style="text-align: center; width: 50px; height: 35px; color: #fff">
                                                        下达任务<input value="" type="checkbox">
                                                    </td>
                                                    <td style="text-align: center; width: 50px; height: 30px; color: #fff">
                                                        序号
                                                    </td>
                                                    <td style="text-align: center; width: 70px; height: 30px; color: #fff">
                                                        编号
                                                    </td>
                                                    <td style="text-align: center; height: 30px; color: #fff">
                                                        任务名称
                                                    </td>
                                                    <td style="text-align: center; width: 70px; height: 30px; color: #fff">
                                                        工期(日)
                                                    </td>
                                                    <td style="text-align: center; width: 80px; height: 30px; color: #fff">
                                                        前置任务
                                                    </td>
                                                    <td style="text-align: center; width: 70px; height: 30px; color: #fff">
                                                        节点计划
                                                    </td>
                                                    <td style="text-align: center; width: 100px; height: 30px; color: #fff">
                                                        制定人
                                                    </td>
                                                    <td style="text-align: center; width: 100px; height: 30px; color: #fff">
                                                        关联指标
                                                    </td>
                                                </tr>
                                            </tbody>
                                            <tbody>
                                                <tr>
                                                </tr>
                                                <tr style="display: block" class="item" oldclassname="item">
                                                    <td style="text-align: center; width: 50px">
                                                        <input value="" type="checkbox">
                                                    </td>
                                                    <td style="text-align: center; width: 50px">
                                                        <span>1</span>
                                                    </td>
                                                    <td style="text-align: left; width: 70px">
                                                        <span>1</span>
                                                    </td>
                                                    <td style="text-align: left">
                                                        <table style="width: 100%">
                                                            <tbody>
                                                                <tr style="width: 100%">
                                                                    <td>
                                                                        <div style="display: block">
                                                                            <img style="margin-left: 10px; cursor: pointer" title="点击收缩" src="/WebResource.axd?d=ILXEOxfTl9X71PL5LglqQGpBaYmrwS3hnMvR7nhOwEFjUokdSjbrjgGNcLR6RFSqqoe8cNzcHUAU9QMnyvVhIg7L7fTXTGO2kvaJzeRUW9AoIyKHYQlziixCpIE8PjOTMIL8ZrklEITSFRSzqk1Th05EWAC0CLnJsVkRYOs033I3b2_-O-0-OE_e3KrfP3950&amp;t=634474660923793476"><span
                                                                                title="立项"></span><span style="color: #fdc01d; margin-left: 2px; cursor: pointer;
                                                                                    font-weight: bold; text-decoration: underline" title="点击修改">立项</span></div>
                                                                    </td>
                                                                    <td style="width: 110px">
                                                                        <div style="text-align: right; width: 110px; display: none">
                                                                            <img style="margin-left: 2px; cursor: hand" title="添加子节点" src="/WebResource.axd?d=vm9rJAgN1TVYU6ISEBYVSP3Vsuo15hJwV4mBpjFe8SoVVgyFJaVypaat5w8mhk80kDFbYRIwJWOT3IxThv8IxSWR_237q8nFgKp814A4LMCHKtIn0jFHPMh1BtYQEbSoV0rr6n3UCQHFbrtdm5D9r7aepcn04s98lMVxkLSglR9W3gB_MKXqWcljq4BAo0mP0&amp;t=634474660923793476"><img
                                                                                style="margin-left: 2px; cursor: hand" title="插入节点" src="/WebResource.axd?d=ByeIdmgJMAZuIAT19kIIDJq3VSmBEzqA4Z7FGs2MIbPw8cOhaMTVwrzACp9g3LPe5wnvhMFwjx5RlRHDqULlrI15EzNqixKCGqRU86EO8qRf1FZTQ8MAUuqMd45f8QmUB-BpRNfbBvDgFj85-K-E13qqyad3tjMvsB5Jd8aeNKJu2beb2SxHLqi5ymDY2H7Z0&amp;t=634474660923793476"><img
                                                                                    style="margin-left: 2px; cursor: hand" title="导入模板" src="/WebResource.axd?d=1_P_wCJ8qLFqvYy0hPOyBpRFQrfnsd_-COG_4x7HJpVUQ5MO8CkPV9ZRKTxm_GMSBQ8Gj4VeV1cw8NyLFR8nxho3m32BOaPFYvHseZok8ux9ptF0Msp0ehljZeLglrfN_3M8zSoWjB06iLgQp6cgZs1uMFSd2kiFU-IpFLL1MNIDeqPMkPQMxZSrfTxNqlPm0&amp;t=634474660923793476"><img
                                                                                        style="margin-left: 2px; cursor: hand" title="上移" src="/WebResource.axd?d=DNomk80hacA8M7RopYCIUuc5OlkSoru0iegwSMwsSDxiQb1N1CyFDiZOP5Ie98tGNDm1gUXo6jRQ9Q8srDjrkIlIsIZvu52wTeOpmPR9_A0y0Sikr7nzFJrM4Rv-oj4Pdu_eXwoFBvjBaPoL-J88qynhE0j7his5kNlqgV9cbDFWNuUrJXit44LFADZreiPC0&amp;t=634474660923793476"><img
                                                                                            style="margin-left: 2px; cursor: hand" title="下移" src="/WebResource.axd?d=atoQyHUSwPIl0zNG7OLOM_9SkBAS2L8xD0xxXDp_7muT5DH_H_iGqTWlsjI9SG4pCej5MALseh0_Zg-JRea1g7gwJiA8zcaXQD6QkuohKcYuiHfzGYlJRRWG1uQ_MVBfFKI2BqI6q5A-_1r6QG36P_A9-F7VcIl5SU0XwFesyBcQ6xHyBx0HWX5AN0b430zR0&amp;t=634474660923793476"><img
                                                                                                style="margin-left: 2px; cursor: hand" title="删除节点" src="/WebResource.axd?d=z_NHxCUXvyXdBs4l3PUqRGXoMNzbAnTqKT-FNuzOq0p3JvYye59O6Wts9uZpuHohXbQpEuOYZsatd7r6dKA_fqplY2i-ilpuysyk3cLNr_LXEGnuBbQEQDS9Am1R4Am_ND2qf0AJv8PWTZ32trheHfHVy0WR2f5QdPbOjf4ftY_mrIXgMG9kCeUTYFvKT5xo0&amp;t=634474660923793476"></div>
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                    <td style="text-align: right; width: 70px">
                                                        <input style="text-align: right; width: 85%" value="0">
                                                    </td>
                                                    <td style="text-align: right; width: 80px">
                                                        <input style="text-align: left; width: 90%" title="请输入行号">
                                                    </td>
                                                    <td style="text-align: center">
                                                        <input title="节点计划" value="" type="checkbox">
                                                    </td>
                                                    <td style="text-align: center; width: 100px">
                                                        <input style="text-align: right; width: 90%; cursor: pointer" title="不填默认为自己，指派他人请双击"
                                                            readonly>
                                                    </td>
                                                    <td style="text-align: right; width: 100px">
                                                        <input style="text-align: right; width: 85%; cursor: pointer" readonly>
                                                    </td>
                                                </tr>
                                                <tr style="display: block" class="alternatingItem" oldclassname="alternatingItem">
                                                    <td style="text-align: center; width: 50px">
                                                        <input value="" type="checkbox">
                                                    </td>
                                                    <td style="text-align: center; width: 50px">
                                                        <span>2</span>
                                                    </td>
                                                    <td style="text-align: left; width: 70px">
                                                        <span>1.1</span>
                                                    </td>
                                                    <td style="text-align: left">
                                                        <table style="width: 100%">
                                                            <tbody>
                                                                <tr style="width: 100%">
                                                                    <td>
                                                                        <div style="display: block">
                                                                            <img style="margin-left: 20px; cursor: pointer" title="" src="/WebResource.axd?d=U6sLVLpcoMb7KoCjSt7OMbXyE63BhjyiMnaRe-LF9amKZsxtKz777YHH9GMInNbiYVRgNUMwlpcG9QAArvH9MkWgZxwfBZQEaPgHYbdbHBFix6a7XwfoI4KTie5RPI514_F2B5rxHP8FCCOMqn1H90pCyOkRWaAZugBAOz6MmPxEXdYt9qG8btFWyhkCDl2_0&amp;t=634474660923793476"><span
                                                                                title="编制立项报告"></span><span style="color: #fdc01d; margin-left: 2px; cursor: pointer;
                                                                                    font-weight: bold; text-decoration: underline" title="点击修改">编制立项报告</span></div>
                                                                    </td>
                                                                    <td style="width: 110px">
                                                                        <div style="text-align: right; width: 110px; display: none">
                                                                            <img style="margin-left: 2px; cursor: hand" title="添加子节点" src="/WebResource.axd?d=vm9rJAgN1TVYU6ISEBYVSP3Vsuo15hJwV4mBpjFe8SoVVgyFJaVypaat5w8mhk80kDFbYRIwJWOT3IxThv8IxSWR_237q8nFgKp814A4LMCHKtIn0jFHPMh1BtYQEbSoV0rr6n3UCQHFbrtdm5D9r7aepcn04s98lMVxkLSglR9W3gB_MKXqWcljq4BAo0mP0&amp;t=634474660923793476"><img
                                                                                style="margin-left: 2px; cursor: hand" title="插入节点" src="/WebResource.axd?d=ByeIdmgJMAZuIAT19kIIDJq3VSmBEzqA4Z7FGs2MIbPw8cOhaMTVwrzACp9g3LPe5wnvhMFwjx5RlRHDqULlrI15EzNqixKCGqRU86EO8qRf1FZTQ8MAUuqMd45f8QmUB-BpRNfbBvDgFj85-K-E13qqyad3tjMvsB5Jd8aeNKJu2beb2SxHLqi5ymDY2H7Z0&amp;t=634474660923793476"><img
                                                                                    style="margin-left: 2px; cursor: hand" title="导入模板" src="/WebResource.axd?d=1_P_wCJ8qLFqvYy0hPOyBpRFQrfnsd_-COG_4x7HJpVUQ5MO8CkPV9ZRKTxm_GMSBQ8Gj4VeV1cw8NyLFR8nxho3m32BOaPFYvHseZok8ux9ptF0Msp0ehljZeLglrfN_3M8zSoWjB06iLgQp6cgZs1uMFSd2kiFU-IpFLL1MNIDeqPMkPQMxZSrfTxNqlPm0&amp;t=634474660923793476"><img
                                                                                        style="margin-left: 2px; cursor: hand" title="上移" src="/WebResource.axd?d=DNomk80hacA8M7RopYCIUuc5OlkSoru0iegwSMwsSDxiQb1N1CyFDiZOP5Ie98tGNDm1gUXo6jRQ9Q8srDjrkIlIsIZvu52wTeOpmPR9_A0y0Sikr7nzFJrM4Rv-oj4Pdu_eXwoFBvjBaPoL-J88qynhE0j7his5kNlqgV9cbDFWNuUrJXit44LFADZreiPC0&amp;t=634474660923793476"><img
                                                                                            style="margin-left: 2px; cursor: hand" title="下移" src="/WebResource.axd?d=atoQyHUSwPIl0zNG7OLOM_9SkBAS2L8xD0xxXDp_7muT5DH_H_iGqTWlsjI9SG4pCej5MALseh0_Zg-JRea1g7gwJiA8zcaXQD6QkuohKcYuiHfzGYlJRRWG1uQ_MVBfFKI2BqI6q5A-_1r6QG36P_A9-F7VcIl5SU0XwFesyBcQ6xHyBx0HWX5AN0b430zR0&amp;t=634474660923793476"><img
                                                                                                style="margin-left: 2px; cursor: hand" title="删除节点" src="/WebResource.axd?d=z_NHxCUXvyXdBs4l3PUqRGXoMNzbAnTqKT-FNuzOq0p3JvYye59O6Wts9uZpuHohXbQpEuOYZsatd7r6dKA_fqplY2i-ilpuysyk3cLNr_LXEGnuBbQEQDS9Am1R4Am_ND2qf0AJv8PWTZ32trheHfHVy0WR2f5QdPbOjf4ftY_mrIXgMG9kCeUTYFvKT5xo0&amp;t=634474660923793476"></div>
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                    <td style="text-align: right; width: 70px">
                                                        <input style="text-align: right; width: 85%" value="0">
                                                    </td>
                                                    <td style="text-align: right; width: 80px">
                                                        <input style="text-align: left; width: 90%" title="请输入行号">
                                                    </td>
                                                    <td style="text-align: center">
                                                        <input title="节点计划" value="" type="checkbox">
                                                    </td>
                                                    <td style="text-align: center; width: 100px">
                                                        <input style="text-align: right; width: 90%; cursor: pointer" title="不填默认为自己，指派他人请双击"
                                                            readonly>
                                                    </td>
                                                    <td style="text-align: right; width: 100px">
                                                        <input style="text-align: right; width: 85%; cursor: pointer" readonly>
                                                    </td>
                                                </tr>
                                                <tr style="display: block" class="item" oldclassname="item">
                                                    <td style="text-align: center; width: 50px">
                                                        <input value="" type="checkbox">
                                                    </td>
                                                    <td style="text-align: center; width: 50px">
                                                        <span>3</span>
                                                    </td>
                                                    <td style="text-align: left; width: 70px">
                                                        <span>1.2</span>
                                                    </td>
                                                    <td style="text-align: left">
                                                        <table style="width: 100%">
                                                            <tbody>
                                                                <tr style="width: 100%">
                                                                    <td>
                                                                        <div style="display: block">
                                                                            <img style="margin-left: 20px; cursor: pointer" title="" src="/WebResource.axd?d=U6sLVLpcoMb7KoCjSt7OMbXyE63BhjyiMnaRe-LF9amKZsxtKz777YHH9GMInNbiYVRgNUMwlpcG9QAArvH9MkWgZxwfBZQEaPgHYbdbHBFix6a7XwfoI4KTie5RPI514_F2B5rxHP8FCCOMqn1H90pCyOkRWaAZugBAOz6MmPxEXdYt9qG8btFWyhkCDl2_0&amp;t=634474660923793476"><span
                                                                                title="立项汇报"></span><span style="color: #fdc01d; margin-left: 2px; cursor: pointer;
                                                                                    font-weight: bold; text-decoration: underline" title="点击修改">立项汇报</span></div>
                                                                    </td>
                                                                    <td style="width: 110px">
                                                                        <div style="text-align: right; width: 110px; display: none">
                                                                            <img style="margin-left: 2px; cursor: hand" title="添加子节点" src="/WebResource.axd?d=vm9rJAgN1TVYU6ISEBYVSP3Vsuo15hJwV4mBpjFe8SoVVgyFJaVypaat5w8mhk80kDFbYRIwJWOT3IxThv8IxSWR_237q8nFgKp814A4LMCHKtIn0jFHPMh1BtYQEbSoV0rr6n3UCQHFbrtdm5D9r7aepcn04s98lMVxkLSglR9W3gB_MKXqWcljq4BAo0mP0&amp;t=634474660923793476"><img
                                                                                style="margin-left: 2px; cursor: hand" title="插入节点" src="/WebResource.axd?d=ByeIdmgJMAZuIAT19kIIDJq3VSmBEzqA4Z7FGs2MIbPw8cOhaMTVwrzACp9g3LPe5wnvhMFwjx5RlRHDqULlrI15EzNqixKCGqRU86EO8qRf1FZTQ8MAUuqMd45f8QmUB-BpRNfbBvDgFj85-K-E13qqyad3tjMvsB5Jd8aeNKJu2beb2SxHLqi5ymDY2H7Z0&amp;t=634474660923793476"><img
                                                                                    style="margin-left: 2px; cursor: hand" title="导入模板" src="/WebResource.axd?d=1_P_wCJ8qLFqvYy0hPOyBpRFQrfnsd_-COG_4x7HJpVUQ5MO8CkPV9ZRKTxm_GMSBQ8Gj4VeV1cw8NyLFR8nxho3m32BOaPFYvHseZok8ux9ptF0Msp0ehljZeLglrfN_3M8zSoWjB06iLgQp6cgZs1uMFSd2kiFU-IpFLL1MNIDeqPMkPQMxZSrfTxNqlPm0&amp;t=634474660923793476"><img
                                                                                        style="margin-left: 2px; cursor: hand" title="上移" src="/WebResource.axd?d=DNomk80hacA8M7RopYCIUuc5OlkSoru0iegwSMwsSDxiQb1N1CyFDiZOP5Ie98tGNDm1gUXo6jRQ9Q8srDjrkIlIsIZvu52wTeOpmPR9_A0y0Sikr7nzFJrM4Rv-oj4Pdu_eXwoFBvjBaPoL-J88qynhE0j7his5kNlqgV9cbDFWNuUrJXit44LFADZreiPC0&amp;t=634474660923793476"><img
                                                                                            style="margin-left: 2px; cursor: hand" title="下移" src="/WebResource.axd?d=atoQyHUSwPIl0zNG7OLOM_9SkBAS2L8xD0xxXDp_7muT5DH_H_iGqTWlsjI9SG4pCej5MALseh0_Zg-JRea1g7gwJiA8zcaXQD6QkuohKcYuiHfzGYlJRRWG1uQ_MVBfFKI2BqI6q5A-_1r6QG36P_A9-F7VcIl5SU0XwFesyBcQ6xHyBx0HWX5AN0b430zR0&amp;t=634474660923793476"><img
                                                                                                style="margin-left: 2px; cursor: hand" title="删除节点" src="/WebResource.axd?d=z_NHxCUXvyXdBs4l3PUqRGXoMNzbAnTqKT-FNuzOq0p3JvYye59O6Wts9uZpuHohXbQpEuOYZsatd7r6dKA_fqplY2i-ilpuysyk3cLNr_LXEGnuBbQEQDS9Am1R4Am_ND2qf0AJv8PWTZ32trheHfHVy0WR2f5QdPbOjf4ftY_mrIXgMG9kCeUTYFvKT5xo0&amp;t=634474660923793476"></div>
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                    <td style="text-align: right; width: 70px">
                                                        <input style="text-align: right; width: 85%" value="0">
                                                    </td>
                                                    <td style="text-align: right; width: 80px">
                                                        <input style="text-align: left; width: 90%" title="请输入行号">
                                                    </td>
                                                    <td style="text-align: center">
                                                        <input title="节点计划" value="" type="checkbox">
                                                    </td>
                                                    <td style="text-align: center; width: 100px">
                                                        <input style="text-align: right; width: 90%; cursor: pointer" title="不填默认为自己，指派他人请双击"
                                                            readonly>
                                                    </td>
                                                    <td style="text-align: right; width: 100px">
                                                        <input style="text-align: right; width: 85%; cursor: pointer" readonly>
                                                    </td>
                                                </tr>
                                                <tr style="display: block" class="alternatingItem" oldclassname="alternatingItem">
                                                    <td style="text-align: center; width: 50px">
                                                        <input value="" type="checkbox">
                                                    </td>
                                                    <td style="text-align: center; width: 50px">
                                                        <span>4</span>
                                                    </td>
                                                    <td style="text-align: left; width: 70px">
                                                        <span>1.3</span>
                                                    </td>
                                                    <td style="text-align: left">
                                                        <table style="width: 100%">
                                                            <tbody>
                                                                <tr style="width: 100%">
                                                                    <td>
                                                                        <div style="display: block">
                                                                            <img style="margin-left: 20px; cursor: pointer" title="" src="/WebResource.axd?d=U6sLVLpcoMb7KoCjSt7OMbXyE63BhjyiMnaRe-LF9amKZsxtKz777YHH9GMInNbiYVRgNUMwlpcG9QAArvH9MkWgZxwfBZQEaPgHYbdbHBFix6a7XwfoI4KTie5RPI514_F2B5rxHP8FCCOMqn1H90pCyOkRWaAZugBAOz6MmPxEXdYt9qG8btFWyhkCDl2_0&amp;t=634474660923793476"><span
                                                                                title="上传签报"></span><span style="color: #fdc01d; margin-left: 2px; cursor: pointer;
                                                                                    font-weight: bold; text-decoration: underline" title="点击修改">上传签报</span></div>
                                                                    </td>
                                                                    <td style="width: 110px">
                                                                        <div style="text-align: right; width: 110px; display: none">
                                                                            <img style="margin-left: 2px; cursor: hand" title="添加子节点" src="/WebResource.axd?d=vm9rJAgN1TVYU6ISEBYVSP3Vsuo15hJwV4mBpjFe8SoVVgyFJaVypaat5w8mhk80kDFbYRIwJWOT3IxThv8IxSWR_237q8nFgKp814A4LMCHKtIn0jFHPMh1BtYQEbSoV0rr6n3UCQHFbrtdm5D9r7aepcn04s98lMVxkLSglR9W3gB_MKXqWcljq4BAo0mP0&amp;t=634474660923793476"><img
                                                                                style="margin-left: 2px; cursor: hand" title="插入节点" src="/WebResource.axd?d=ByeIdmgJMAZuIAT19kIIDJq3VSmBEzqA4Z7FGs2MIbPw8cOhaMTVwrzACp9g3LPe5wnvhMFwjx5RlRHDqULlrI15EzNqixKCGqRU86EO8qRf1FZTQ8MAUuqMd45f8QmUB-BpRNfbBvDgFj85-K-E13qqyad3tjMvsB5Jd8aeNKJu2beb2SxHLqi5ymDY2H7Z0&amp;t=634474660923793476"><img
                                                                                    style="margin-left: 2px; cursor: hand" title="导入模板" src="/WebResource.axd?d=1_P_wCJ8qLFqvYy0hPOyBpRFQrfnsd_-COG_4x7HJpVUQ5MO8CkPV9ZRKTxm_GMSBQ8Gj4VeV1cw8NyLFR8nxho3m32BOaPFYvHseZok8ux9ptF0Msp0ehljZeLglrfN_3M8zSoWjB06iLgQp6cgZs1uMFSd2kiFU-IpFLL1MNIDeqPMkPQMxZSrfTxNqlPm0&amp;t=634474660923793476"><img
                                                                                        style="margin-left: 2px; cursor: hand" title="上移" src="/WebResource.axd?d=DNomk80hacA8M7RopYCIUuc5OlkSoru0iegwSMwsSDxiQb1N1CyFDiZOP5Ie98tGNDm1gUXo6jRQ9Q8srDjrkIlIsIZvu52wTeOpmPR9_A0y0Sikr7nzFJrM4Rv-oj4Pdu_eXwoFBvjBaPoL-J88qynhE0j7his5kNlqgV9cbDFWNuUrJXit44LFADZreiPC0&amp;t=634474660923793476"><img
                                                                                            style="margin-left: 2px; cursor: hand" title="下移" src="/WebResource.axd?d=atoQyHUSwPIl0zNG7OLOM_9SkBAS2L8xD0xxXDp_7muT5DH_H_iGqTWlsjI9SG4pCej5MALseh0_Zg-JRea1g7gwJiA8zcaXQD6QkuohKcYuiHfzGYlJRRWG1uQ_MVBfFKI2BqI6q5A-_1r6QG36P_A9-F7VcIl5SU0XwFesyBcQ6xHyBx0HWX5AN0b430zR0&amp;t=634474660923793476"><img
                                                                                                style="margin-left: 2px; cursor: hand" title="删除节点" src="/WebResource.axd?d=z_NHxCUXvyXdBs4l3PUqRGXoMNzbAnTqKT-FNuzOq0p3JvYye59O6Wts9uZpuHohXbQpEuOYZsatd7r6dKA_fqplY2i-ilpuysyk3cLNr_LXEGnuBbQEQDS9Am1R4Am_ND2qf0AJv8PWTZ32trheHfHVy0WR2f5QdPbOjf4ftY_mrIXgMG9kCeUTYFvKT5xo0&amp;t=634474660923793476"></div>
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                    <td style="text-align: right; width: 70px">
                                                        <input style="text-align: right; width: 85%" value="0">
                                                    </td>
                                                    <td style="text-align: right; width: 80px">
                                                        <input style="text-align: left; width: 90%" title="请输入行号">
                                                    </td>
                                                    <td style="text-align: center">
                                                        <input title="节点计划" value="" type="checkbox">
                                                    </td>
                                                    <td style="text-align: center; width: 100px">
                                                        <input style="text-align: right; width: 90%; cursor: pointer" title="不填默认为自己，指派他人请双击"
                                                            readonly>
                                                    </td>
                                                    <td style="text-align: right; width: 100px">
                                                        <input style="text-align: right; width: 85%; cursor: pointer" readonly>
                                                    </td>
                                                </tr>
                                                <tr style="display: block" class="item" oldclassname="item">
                                                    <td style="text-align: center; width: 50px">
                                                        <input value="" type="checkbox">
                                                    </td>
                                                    <td style="text-align: center; width: 50px">
                                                        <span>5</span>
                                                    </td>
                                                    <td style="text-align: left; width: 70px">
                                                        <span>2</span>
                                                    </td>
                                                    <td style="text-align: left">
                                                        <table style="width: 100%">
                                                            <tbody>
                                                                <tr style="width: 100%">
                                                                    <td>
                                                                        <div style="display: block">
                                                                            <img style="margin-left: 10px; cursor: pointer" title="点击收缩" src="/WebResource.axd?d=ILXEOxfTl9X71PL5LglqQGpBaYmrwS3hnMvR7nhOwEFjUokdSjbrjgGNcLR6RFSqqoe8cNzcHUAU9QMnyvVhIg7L7fTXTGO2kvaJzeRUW9AoIyKHYQlziixCpIE8PjOTMIL8ZrklEITSFRSzqk1Th05EWAC0CLnJsVkRYOs033I3b2_-O-0-OE_e3KrfP3950&amp;t=634474660923793476"><span
                                                                                title="合同"></span><span style="color: #fdc01d; margin-left: 2px; cursor: pointer;
                                                                                    font-weight: bold; text-decoration: underline" title="点击修改">合同</span></div>
                                                                    </td>
                                                                    <td style="width: 110px">
                                                                        <div style="text-align: right; width: 110px; display: none">
                                                                            <img style="margin-left: 2px; cursor: hand" title="添加子节点" src="/WebResource.axd?d=vm9rJAgN1TVYU6ISEBYVSP3Vsuo15hJwV4mBpjFe8SoVVgyFJaVypaat5w8mhk80kDFbYRIwJWOT3IxThv8IxSWR_237q8nFgKp814A4LMCHKtIn0jFHPMh1BtYQEbSoV0rr6n3UCQHFbrtdm5D9r7aepcn04s98lMVxkLSglR9W3gB_MKXqWcljq4BAo0mP0&amp;t=634474660923793476"><img
                                                                                style="margin-left: 2px; cursor: hand" title="插入节点" src="/WebResource.axd?d=ByeIdmgJMAZuIAT19kIIDJq3VSmBEzqA4Z7FGs2MIbPw8cOhaMTVwrzACp9g3LPe5wnvhMFwjx5RlRHDqULlrI15EzNqixKCGqRU86EO8qRf1FZTQ8MAUuqMd45f8QmUB-BpRNfbBvDgFj85-K-E13qqyad3tjMvsB5Jd8aeNKJu2beb2SxHLqi5ymDY2H7Z0&amp;t=634474660923793476"><img
                                                                                    style="margin-left: 2px; cursor: hand" title="导入模板" src="/WebResource.axd?d=1_P_wCJ8qLFqvYy0hPOyBpRFQrfnsd_-COG_4x7HJpVUQ5MO8CkPV9ZRKTxm_GMSBQ8Gj4VeV1cw8NyLFR8nxho3m32BOaPFYvHseZok8ux9ptF0Msp0ehljZeLglrfN_3M8zSoWjB06iLgQp6cgZs1uMFSd2kiFU-IpFLL1MNIDeqPMkPQMxZSrfTxNqlPm0&amp;t=634474660923793476"><img
                                                                                        style="margin-left: 2px; cursor: hand" title="上移" src="/WebResource.axd?d=DNomk80hacA8M7RopYCIUuc5OlkSoru0iegwSMwsSDxiQb1N1CyFDiZOP5Ie98tGNDm1gUXo6jRQ9Q8srDjrkIlIsIZvu52wTeOpmPR9_A0y0Sikr7nzFJrM4Rv-oj4Pdu_eXwoFBvjBaPoL-J88qynhE0j7his5kNlqgV9cbDFWNuUrJXit44LFADZreiPC0&amp;t=634474660923793476"><img
                                                                                            style="margin-left: 2px; cursor: hand" title="下移" src="/WebResource.axd?d=atoQyHUSwPIl0zNG7OLOM_9SkBAS2L8xD0xxXDp_7muT5DH_H_iGqTWlsjI9SG4pCej5MALseh0_Zg-JRea1g7gwJiA8zcaXQD6QkuohKcYuiHfzGYlJRRWG1uQ_MVBfFKI2BqI6q5A-_1r6QG36P_A9-F7VcIl5SU0XwFesyBcQ6xHyBx0HWX5AN0b430zR0&amp;t=634474660923793476"><img
                                                                                                style="margin-left: 2px; cursor: hand" title="删除节点" src="/WebResource.axd?d=z_NHxCUXvyXdBs4l3PUqRGXoMNzbAnTqKT-FNuzOq0p3JvYye59O6Wts9uZpuHohXbQpEuOYZsatd7r6dKA_fqplY2i-ilpuysyk3cLNr_LXEGnuBbQEQDS9Am1R4Am_ND2qf0AJv8PWTZ32trheHfHVy0WR2f5QdPbOjf4ftY_mrIXgMG9kCeUTYFvKT5xo0&amp;t=634474660923793476"></div>
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                    <td style="text-align: right; width: 70px">
                                                        <input style="text-align: right; width: 85%" value="0">
                                                    </td>
                                                    <td style="text-align: right; width: 80px">
                                                        <input style="text-align: left; width: 90%" title="请输入行号">
                                                    </td>
                                                    <td style="text-align: center">
                                                        <input title="节点计划" value="" type="checkbox">
                                                    </td>
                                                    <td style="text-align: center; width: 100px">
                                                        <input style="text-align: right; width: 90%; cursor: pointer" title="不填默认为自己，指派他人请双击"
                                                            readonly>
                                                    </td>
                                                    <td style="text-align: right; width: 100px">
                                                        <input style="text-align: right; width: 85%; cursor: pointer" readonly>
                                                    </td>
                                                </tr>
                                                <tr style="display: block" class="alternatingItem" oldclassname="alternatingItem">
                                                    <td style="text-align: center; width: 50px">
                                                        <input value="" type="checkbox">
                                                    </td>
                                                    <td style="text-align: center; width: 50px">
                                                        <span>6</span>
                                                    </td>
                                                    <td style="text-align: left; width: 70px">
                                                        <span>2.1</span>
                                                    </td>
                                                    <td style="text-align: left">
                                                        <table style="width: 100%">
                                                            <tbody>
                                                                <tr style="width: 100%">
                                                                    <td>
                                                                        <div style="display: block">
                                                                            <img style="margin-left: 20px; cursor: pointer" title="" src="/WebResource.axd?d=U6sLVLpcoMb7KoCjSt7OMbXyE63BhjyiMnaRe-LF9amKZsxtKz777YHH9GMInNbiYVRgNUMwlpcG9QAArvH9MkWgZxwfBZQEaPgHYbdbHBFix6a7XwfoI4KTie5RPI514_F2B5rxHP8FCCOMqn1H90pCyOkRWaAZugBAOz6MmPxEXdYt9qG8btFWyhkCDl2_0&amp;t=634474660923793476"><span
                                                                                title="合同文本修订"></span><span style="color: #fdc01d; margin-left: 2px; cursor: pointer;
                                                                                    font-weight: bold; text-decoration: underline" title="点击修改">合同文本修订</span></div>
                                                                    </td>
                                                                    <td style="width: 110px">
                                                                        <div style="text-align: right; width: 110px; display: none">
                                                                            <img style="margin-left: 2px; cursor: hand" title="添加子节点" src="/WebResource.axd?d=vm9rJAgN1TVYU6ISEBYVSP3Vsuo15hJwV4mBpjFe8SoVVgyFJaVypaat5w8mhk80kDFbYRIwJWOT3IxThv8IxSWR_237q8nFgKp814A4LMCHKtIn0jFHPMh1BtYQEbSoV0rr6n3UCQHFbrtdm5D9r7aepcn04s98lMVxkLSglR9W3gB_MKXqWcljq4BAo0mP0&amp;t=634474660923793476"
                                                                                width="15" height="15"><img style="margin-left: 2px; cursor: hand" title="插入节点" src="/WebResource.axd?d=ByeIdmgJMAZuIAT19kIIDJq3VSmBEzqA4Z7FGs2MIbPw8cOhaMTVwrzACp9g3LPe5wnvhMFwjx5RlRHDqULlrI15EzNqixKCGqRU86EO8qRf1FZTQ8MAUuqMd45f8QmUB-BpRNfbBvDgFj85-K-E13qqyad3tjMvsB5Jd8aeNKJu2beb2SxHLqi5ymDY2H7Z0&amp;t=634474660923793476"><img
                                                                                    style="margin-left: 2px; cursor: hand" title="导入模板" src="/WebResource.axd?d=1_P_wCJ8qLFqvYy0hPOyBpRFQrfnsd_-COG_4x7HJpVUQ5MO8CkPV9ZRKTxm_GMSBQ8Gj4VeV1cw8NyLFR8nxho3m32BOaPFYvHseZok8ux9ptF0Msp0ehljZeLglrfN_3M8zSoWjB06iLgQp6cgZs1uMFSd2kiFU-IpFLL1MNIDeqPMkPQMxZSrfTxNqlPm0&amp;t=634474660923793476"
                                                                                    width="15" height="15"><img style="margin-left: 2px; cursor: hand" title="上移" src="/WebResource.axd?d=DNomk80hacA8M7RopYCIUuc5OlkSoru0iegwSMwsSDxiQb1N1CyFDiZOP5Ie98tGNDm1gUXo6jRQ9Q8srDjrkIlIsIZvu52wTeOpmPR9_A0y0Sikr7nzFJrM4Rv-oj4Pdu_eXwoFBvjBaPoL-J88qynhE0j7his5kNlqgV9cbDFWNuUrJXit44LFADZreiPC0&amp;t=634474660923793476"><img
                                                                                        style="margin-left: 2px; cursor: hand" title="下移" src="/WebResource.axd?d=atoQyHUSwPIl0zNG7OLOM_9SkBAS2L8xD0xxXDp_7muT5DH_H_iGqTWlsjI9SG4pCej5MALseh0_Zg-JRea1g7gwJiA8zcaXQD6QkuohKcYuiHfzGYlJRRWG1uQ_MVBfFKI2BqI6q5A-_1r6QG36P_A9-F7VcIl5SU0XwFesyBcQ6xHyBx0HWX5AN0b430zR0&amp;t=634474660923793476"><img
                                                                                            style="margin-left: 2px; cursor: hand" title="删除节点" src="/WebResource.axd?d=z_NHxCUXvyXdBs4l3PUqRGXoMNzbAnTqKT-FNuzOq0p3JvYye59O6Wts9uZpuHohXbQpEuOYZsatd7r6dKA_fqplY2i-ilpuysyk3cLNr_LXEGnuBbQEQDS9Am1R4Am_ND2qf0AJv8PWTZ32trheHfHVy0WR2f5QdPbOjf4ftY_mrIXgMG9kCeUTYFvKT5xo0&amp;t=634474660923793476"
                                                                                            width="15" height="15"></div>
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                    <td style="text-align: right; width: 70px">
                                                        <input style="text-align: right; width: 85%" value="0">
                                                    </td>
                                                    <td style="text-align: right; width: 80px">
                                                        <input style="text-align: left; width: 90%" title="请输入行号">
                                                    </td>
                                                    <td style="text-align: center">
                                                        <input title="节点计划" value="" type="checkbox">
                                                    </td>
                                                    <td style="text-align: center; width: 100px">
                                                        <input style="text-align: right; width: 90%; cursor: pointer" title="不填默认为自己，指派他人请双击"
                                                            readonly>
                                                    </td>
                                                    <td style="text-align: right; width: 100px">
                                                        <input style="text-align: right; width: 85%; cursor: pointer" readonly>
                                                    </td>
                                                </tr>
                                                <tr style="display: block" class="item" oldclassname="item">
                                                    <td style="text-align: center; width: 50px">
                                                        <input value="" type="checkbox">
                                                    </td>
                                                    <td style="text-align: center; width: 50px">
                                                        <span>7</span>
                                                    </td>
                                                    <td style="text-align: left; width: 70px">
                                                        <span>2.2</span>
                                                    </td>
                                                    <td style="text-align: left">
                                                        <table style="width: 100%">
                                                            <tbody>
                                                                <tr style="width: 100%">
                                                                    <td>
                                                                        <div style="display: block">
                                                                            <img style="margin-left: 20px; cursor: pointer" title="" src="/WebResource.axd?d=U6sLVLpcoMb7KoCjSt7OMbXyE63BhjyiMnaRe-LF9amKZsxtKz777YHH9GMInNbiYVRgNUMwlpcG9QAArvH9MkWgZxwfBZQEaPgHYbdbHBFix6a7XwfoI4KTie5RPI514_F2B5rxHP8FCCOMqn1H90pCyOkRWaAZugBAOz6MmPxEXdYt9qG8btFWyhkCDl2_0&amp;t=634474660923793476"><span
                                                                                title="合同提交法务审核"></span><span style="color: #fdc01d; margin-left: 2px; cursor: pointer;
                                                                                    font-weight: bold; text-decoration: underline" title="点击修改">合同提交法务审核</span></div>
                                                                    </td>
                                                                    <td style="width: 110px">
                                                                        <div style="text-align: right; width: 110px; display: none">
                                                                            <img style="margin-left: 2px; cursor: hand" title="添加子节点" src="/WebResource.axd?d=vm9rJAgN1TVYU6ISEBYVSP3Vsuo15hJwV4mBpjFe8SoVVgyFJaVypaat5w8mhk80kDFbYRIwJWOT3IxThv8IxSWR_237q8nFgKp814A4LMCHKtIn0jFHPMh1BtYQEbSoV0rr6n3UCQHFbrtdm5D9r7aepcn04s98lMVxkLSglR9W3gB_MKXqWcljq4BAo0mP0&amp;t=634474660923793476"
                                                                                width="15" height="15"><img style="margin-left: 2px; cursor: hand" title="插入节点" src="/WebResource.axd?d=ByeIdmgJMAZuIAT19kIIDJq3VSmBEzqA4Z7FGs2MIbPw8cOhaMTVwrzACp9g3LPe5wnvhMFwjx5RlRHDqULlrI15EzNqixKCGqRU86EO8qRf1FZTQ8MAUuqMd45f8QmUB-BpRNfbBvDgFj85-K-E13qqyad3tjMvsB5Jd8aeNKJu2beb2SxHLqi5ymDY2H7Z0&amp;t=634474660923793476"
                                                                                    width="15" height="15"><img style="margin-left: 2px; cursor: hand" title="导入模板" src="/WebResource.axd?d=1_P_wCJ8qLFqvYy0hPOyBpRFQrfnsd_-COG_4x7HJpVUQ5MO8CkPV9ZRKTxm_GMSBQ8Gj4VeV1cw8NyLFR8nxho3m32BOaPFYvHseZok8ux9ptF0Msp0ehljZeLglrfN_3M8zSoWjB06iLgQp6cgZs1uMFSd2kiFU-IpFLL1MNIDeqPMkPQMxZSrfTxNqlPm0&amp;t=634474660923793476"
                                                                                        width="15" height="15"><img style="margin-left: 2px; cursor: hand" title="上移" src="/WebResource.axd?d=DNomk80hacA8M7RopYCIUuc5OlkSoru0iegwSMwsSDxiQb1N1CyFDiZOP5Ie98tGNDm1gUXo6jRQ9Q8srDjrkIlIsIZvu52wTeOpmPR9_A0y0Sikr7nzFJrM4Rv-oj4Pdu_eXwoFBvjBaPoL-J88qynhE0j7his5kNlqgV9cbDFWNuUrJXit44LFADZreiPC0&amp;t=634474660923793476"
                                                                                            width="15" height="15"><img style="margin-left: 2px; cursor: hand" title="下移" src="/WebResource.axd?d=atoQyHUSwPIl0zNG7OLOM_9SkBAS2L8xD0xxXDp_7muT5DH_H_iGqTWlsjI9SG4pCej5MALseh0_Zg-JRea1g7gwJiA8zcaXQD6QkuohKcYuiHfzGYlJRRWG1uQ_MVBfFKI2BqI6q5A-_1r6QG36P_A9-F7VcIl5SU0XwFesyBcQ6xHyBx0HWX5AN0b430zR0&amp;t=634474660923793476"
                                                                                                width="15" height="15"><img style="margin-left: 2px; cursor: hand" title="删除节点" src="/WebResource.axd?d=z_NHxCUXvyXdBs4l3PUqRGXoMNzbAnTqKT-FNuzOq0p3JvYye59O6Wts9uZpuHohXbQpEuOYZsatd7r6dKA_fqplY2i-ilpuysyk3cLNr_LXEGnuBbQEQDS9Am1R4Am_ND2qf0AJv8PWTZ32trheHfHVy0WR2f5QdPbOjf4ftY_mrIXgMG9kCeUTYFvKT5xo0&amp;t=634474660923793476"
                                                                                                    width="15" height="15"></div>
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                    <td style="text-align: right; width: 70px">
                                                        <input style="text-align: right; width: 85%" value="0">
                                                    </td>
                                                    <td style="text-align: right; width: 80px">
                                                        <input style="text-align: left; width: 90%" title="请输入行号">
                                                    </td>
                                                    <td style="text-align: center">
                                                        <input title="节点计划" value="" type="checkbox">
                                                    </td>
                                                    <td style="text-align: center; width: 100px">
                                                        <input style="text-align: right; width: 90%; cursor: pointer" title="不填默认为自己，指派他人请双击"
                                                            readonly>
                                                    </td>
                                                    <td style="text-align: right; width: 100px">
                                                        <input style="text-align: right; width: 85%; cursor: pointer" readonly>
                                                    </td>
                                                </tr>
                                                <tr style="display: block" class="alternatingItem" oldclassname="alternatingItem">
                                                    <td style="text-align: center; width: 50px">
                                                        <input value="" type="checkbox">
                                                    </td>
                                                    <td style="text-align: center; width: 50px">
                                                        <span>8</span>
                                                    </td>
                                                    <td style="text-align: left; width: 70px">
                                                        <span>2.3</span>
                                                    </td>
                                                    <td style="text-align: left">
                                                        <table style="width: 100%">
                                                            <tbody>
                                                                <tr style="width: 100%">
                                                                    <td>
                                                                        <div style="display: block">
                                                                            <img style="margin-left: 20px; cursor: pointer" title="" src="/WebResource.axd?d=U6sLVLpcoMb7KoCjSt7OMbXyE63BhjyiMnaRe-LF9amKZsxtKz777YHH9GMInNbiYVRgNUMwlpcG9QAArvH9MkWgZxwfBZQEaPgHYbdbHBFix6a7XwfoI4KTie5RPI514_F2B5rxHP8FCCOMqn1H90pCyOkRWaAZugBAOz6MmPxEXdYt9qG8btFWyhkCDl2_0&amp;t=634474660923793476"
                                                                                width="1" height="1"><span title="提交合同审批"></span><span style="color: #fdc01d; margin-left: 2px;
                                                                                    cursor: pointer; font-weight: bold; text-decoration: underline" title="点击修改">提交合同审批</span></div>
                                                                    </td>
                                                                    <td style="width: 110px">
                                                                        <div style="text-align: right; width: 110px; display: none">
                                                                            <img style="margin-left: 2px; cursor: hand" title="添加子节点" src="/WebResource.axd?d=vm9rJAgN1TVYU6ISEBYVSP3Vsuo15hJwV4mBpjFe8SoVVgyFJaVypaat5w8mhk80kDFbYRIwJWOT3IxThv8IxSWR_237q8nFgKp814A4LMCHKtIn0jFHPMh1BtYQEbSoV0rr6n3UCQHFbrtdm5D9r7aepcn04s98lMVxkLSglR9W3gB_MKXqWcljq4BAo0mP0&amp;t=634474660923793476"
                                                                                width="15" height="15"><img style="margin-left: 2px; cursor: hand" title="插入节点" src="/WebResource.axd?d=ByeIdmgJMAZuIAT19kIIDJq3VSmBEzqA4Z7FGs2MIbPw8cOhaMTVwrzACp9g3LPe5wnvhMFwjx5RlRHDqULlrI15EzNqixKCGqRU86EO8qRf1FZTQ8MAUuqMd45f8QmUB-BpRNfbBvDgFj85-K-E13qqyad3tjMvsB5Jd8aeNKJu2beb2SxHLqi5ymDY2H7Z0&amp;t=634474660923793476"
                                                                                    width="15" height="15"><img style="margin-left: 2px; cursor: hand" title="导入模板" src="/WebResource.axd?d=1_P_wCJ8qLFqvYy0hPOyBpRFQrfnsd_-COG_4x7HJpVUQ5MO8CkPV9ZRKTxm_GMSBQ8Gj4VeV1cw8NyLFR8nxho3m32BOaPFYvHseZok8ux9ptF0Msp0ehljZeLglrfN_3M8zSoWjB06iLgQp6cgZs1uMFSd2kiFU-IpFLL1MNIDeqPMkPQMxZSrfTxNqlPm0&amp;t=634474660923793476"
                                                                                        width="15" height="15"><img style="margin-left: 2px; cursor: hand" title="上移" src="/WebResource.axd?d=DNomk80hacA8M7RopYCIUuc5OlkSoru0iegwSMwsSDxiQb1N1CyFDiZOP5Ie98tGNDm1gUXo6jRQ9Q8srDjrkIlIsIZvu52wTeOpmPR9_A0y0Sikr7nzFJrM4Rv-oj4Pdu_eXwoFBvjBaPoL-J88qynhE0j7his5kNlqgV9cbDFWNuUrJXit44LFADZreiPC0&amp;t=634474660923793476"
                                                                                            width="15" height="15"><img style="margin-left: 2px; cursor: hand" title="下移" src="/WebResource.axd?d=atoQyHUSwPIl0zNG7OLOM_9SkBAS2L8xD0xxXDp_7muT5DH_H_iGqTWlsjI9SG4pCej5MALseh0_Zg-JRea1g7gwJiA8zcaXQD6QkuohKcYuiHfzGYlJRRWG1uQ_MVBfFKI2BqI6q5A-_1r6QG36P_A9-F7VcIl5SU0XwFesyBcQ6xHyBx0HWX5AN0b430zR0&amp;t=634474660923793476"
                                                                                                width="15" height="15"><img style="margin-left: 2px; cursor: hand" title="删除节点" src="/WebResource.axd?d=z_NHxCUXvyXdBs4l3PUqRGXoMNzbAnTqKT-FNuzOq0p3JvYye59O6Wts9uZpuHohXbQpEuOYZsatd7r6dKA_fqplY2i-ilpuysyk3cLNr_LXEGnuBbQEQDS9Am1R4Am_ND2qf0AJv8PWTZ32trheHfHVy0WR2f5QdPbOjf4ftY_mrIXgMG9kCeUTYFvKT5xo0&amp;t=634474660923793476"
                                                                                                    width="15" height="15"></div>
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                    <td style="text-align: right; width: 70px">
                                                        <input style="text-align: right; width: 85%" value="0">
                                                    </td>
                                                    <td style="text-align: right; width: 80px">
                                                        <input style="text-align: left; width: 90%" title="请输入行号">
                                                    </td>
                                                    <td style="text-align: center">
                                                        <input title="节点计划" value="" type="checkbox">
                                                    </td>
                                                    <td style="text-align: center; width: 100px">
                                                        <input style="text-align: right; width: 90%; cursor: pointer" title="不填默认为自己，指派他人请双击"
                                                            readonly>
                                                    </td>
                                                    <td style="text-align: right; width: 100px">
                                                        <input style="text-align: right; width: 85%; cursor: pointer" readonly>
                                                    </td>
                                                </tr>
                                                <tr style="display: block" class="item" oldclassname="item">
                                                    <td style="text-align: center; width: 50px">
                                                        <input value="" type="checkbox">
                                                    </td>
                                                    <td style="text-align: center; width: 50px">
                                                        <span>9</span>
                                                    </td>
                                                    <td style="text-align: left; width: 70px">
                                                        <span>2.4</span>
                                                    </td>
                                                    <td style="text-align: left">
                                                        <table style="width: 100%">
                                                            <tbody>
                                                                <tr style="width: 100%">
                                                                    <td>
                                                                        <div style="display: block">
                                                                            <img style="margin-left: 20px; cursor: pointer" title="" src="/WebResource.axd?d=U6sLVLpcoMb7KoCjSt7OMbXyE63BhjyiMnaRe-LF9amKZsxtKz777YHH9GMInNbiYVRgNUMwlpcG9QAArvH9MkWgZxwfBZQEaPgHYbdbHBFix6a7XwfoI4KTie5RPI514_F2B5rxHP8FCCOMqn1H90pCyOkRWaAZugBAOz6MmPxEXdYt9qG8btFWyhkCDl2_0&amp;t=634474660923793476"
                                                                                width="1" height="1"><span title="完成合同签署"></span><span style="color: #fdc01d; margin-left: 2px;
                                                                                    cursor: pointer; font-weight: bold; text-decoration: underline" title="点击修改">完成合同签署</span></div>
                                                                    </td>
                                                                    <td style="width: 110px">
                                                                        <div style="text-align: right; width: 110px; display: none">
                                                                            <img style="margin-left: 2px; cursor: hand" title="添加子节点" src="/WebResource.axd?d=vm9rJAgN1TVYU6ISEBYVSP3Vsuo15hJwV4mBpjFe8SoVVgyFJaVypaat5w8mhk80kDFbYRIwJWOT3IxThv8IxSWR_237q8nFgKp814A4LMCHKtIn0jFHPMh1BtYQEbSoV0rr6n3UCQHFbrtdm5D9r7aepcn04s98lMVxkLSglR9W3gB_MKXqWcljq4BAo0mP0&amp;t=634474660923793476"
                                                                                width="15" height="15"><img style="margin-left: 2px; cursor: hand" title="插入节点" src="/WebResource.axd?d=ByeIdmgJMAZuIAT19kIIDJq3VSmBEzqA4Z7FGs2MIbPw8cOhaMTVwrzACp9g3LPe5wnvhMFwjx5RlRHDqULlrI15EzNqixKCGqRU86EO8qRf1FZTQ8MAUuqMd45f8QmUB-BpRNfbBvDgFj85-K-E13qqyad3tjMvsB5Jd8aeNKJu2beb2SxHLqi5ymDY2H7Z0&amp;t=634474660923793476"
                                                                                    width="15" height="15"><img style="margin-left: 2px; cursor: hand" title="导入模板" src="/WebResource.axd?d=1_P_wCJ8qLFqvYy0hPOyBpRFQrfnsd_-COG_4x7HJpVUQ5MO8CkPV9ZRKTxm_GMSBQ8Gj4VeV1cw8NyLFR8nxho3m32BOaPFYvHseZok8ux9ptF0Msp0ehljZeLglrfN_3M8zSoWjB06iLgQp6cgZs1uMFSd2kiFU-IpFLL1MNIDeqPMkPQMxZSrfTxNqlPm0&amp;t=634474660923793476"
                                                                                        width="15" height="15"><img style="margin-left: 2px; cursor: hand" title="上移" src="/WebResource.axd?d=DNomk80hacA8M7RopYCIUuc5OlkSoru0iegwSMwsSDxiQb1N1CyFDiZOP5Ie98tGNDm1gUXo6jRQ9Q8srDjrkIlIsIZvu52wTeOpmPR9_A0y0Sikr7nzFJrM4Rv-oj4Pdu_eXwoFBvjBaPoL-J88qynhE0j7his5kNlqgV9cbDFWNuUrJXit44LFADZreiPC0&amp;t=634474660923793476"
                                                                                            width="15" height="15"><img style="margin-left: 2px; cursor: hand" title="下移" src="/WebResource.axd?d=atoQyHUSwPIl0zNG7OLOM_9SkBAS2L8xD0xxXDp_7muT5DH_H_iGqTWlsjI9SG4pCej5MALseh0_Zg-JRea1g7gwJiA8zcaXQD6QkuohKcYuiHfzGYlJRRWG1uQ_MVBfFKI2BqI6q5A-_1r6QG36P_A9-F7VcIl5SU0XwFesyBcQ6xHyBx0HWX5AN0b430zR0&amp;t=634474660923793476"
                                                                                                width="15" height="15"><img style="margin-left: 2px; cursor: hand" title="删除节点" src="/WebResource.axd?d=z_NHxCUXvyXdBs4l3PUqRGXoMNzbAnTqKT-FNuzOq0p3JvYye59O6Wts9uZpuHohXbQpEuOYZsatd7r6dKA_fqplY2i-ilpuysyk3cLNr_LXEGnuBbQEQDS9Am1R4Am_ND2qf0AJv8PWTZ32trheHfHVy0WR2f5QdPbOjf4ftY_mrIXgMG9kCeUTYFvKT5xo0&amp;t=634474660923793476"
                                                                                                    width="15" height="15"></div>
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                    <td style="text-align: right; width: 70px">
                                                        <input style="text-align: right; width: 85%" value="0">
                                                    </td>
                                                    <td style="text-align: right; width: 80px">
                                                        <input style="text-align: left; width: 90%" title="请输入行号">
                                                    </td>
                                                    <td style="text-align: center">
                                                        <input title="节点计划" value="" type="checkbox">
                                                    </td>
                                                    <td style="text-align: center; width: 100px">
                                                        <input style="text-align: right; width: 90%; cursor: pointer" title="不填默认为自己，指派他人请双击"
                                                            readonly>
                                                    </td>
                                                    <td style="text-align: right; width: 100px">
                                                        <input style="text-align: right; width: 85%; cursor: pointer" readonly>
                                                    </td>
                                                </tr>
                                            </tbody>
                                            <tbody style="display: none; visibility: hidden">
                                                <tr class="footer">
                                                    <td>
                                                        <input value="" type="checkbox">
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                    <td>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <br />
    <br />
    <div>
        <HB:ClientGrid runat="server" ID="clientGrid1" Caption="标题头~~测试" Width="570px" ShowEditBar="true"
            OnPreHeaderRowCreate="OnPreHeaderRowCreate">
            <Columns>
                <HB:ClientGridColumn SelectColumn="true" ShowSelectAll="true" HeaderStyle="{width:'20px',border:'1px solid #fff'}"
                    ItemStyle="{width:'20px'}" />
                <HB:ClientGridColumn DataField="Index" HeaderText="行" SortExpression="Index" DataType="Integer"
                    HeaderStyle="{width:'50px',border:'1px solid #fff'}" ItemStyle="{width:'50px',textAlign:'center'}" />
                <HB:ClientGridColumn DataField="Date" HeaderText="Date" HeaderStyle="{width:'180px'}"
                    ItemStyle="{width:'180px',background-color:'#e6e6e6',textAlign:'right'}">
                    <EditTemplate EditMode="TextBox" />
                </HB:ClientGridColumn>
                <HB:ClientGridColumn DataField="DateTime" HeaderText="DateTime" HeaderStyle="{width:'180px',border:'1px solid #fff'}"
                    ItemStyle="{width:'180px',background-color:'#e6e6e6',textAlign:'right'}">
                    <EditTemplate EditMode="TextBox" />
                </HB:ClientGridColumn>
                <HB:ClientGridColumn DataField="Money" HeaderText="Money" DataType="Decimal" FormatString="{0:N2}"
                    HeaderStyle="{width:'180px',border:'1px solid #fff'}" ItemStyle="{width:'180px',background-color:'#e6e6e6',textAlign:'right'}">
                    <EditTemplate EditMode="TextBox" />
                </HB:ClientGridColumn>
            </Columns>
        </HB:ClientGrid>
    </div>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DrawDocument.aspx.cs" Inherits="MCS.OA.Portal.frames.DrawDocument" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>拟单</title>
    <script type="text/javascript" src="../javascript/taskLink.js"></script>
    <style type="text/css">
        .unUse
        {
            color: #ddd;
            cursor: no-drop;
        }
        .ddItem
        {
            float: left;
        }
        .ddItem *
        {
            width: 100%;
            float: left;
        }
        .ddItem li
        {
            width: 110px;
            background: url(../img/bg_ul.gif) center no-repeat;
            line-height: 30px;
            margin: 1px;
            float: left;
            text-align: center;
        }
        .ddItem p
        {
            height: 30px;
            line-height: 30px;
            text-indent: 2em;
            width: 100%;
            padding-left: 3px;
        }
        .ddItem p a
        {
            width: 90px;
            background: url(../img/icon1.gif) left no-repeat;
            text-indent: 3em;
        }
        .tagContent
        {
            display: none;
        }
        #tagContent .selectTag1
        {
            display: block;
        }
    </style>
    <link rel="Stylesheet" href="../css.css" />
    <script type="text/javascript">
        function onDraftLinkClick() {
            var a = event.srcElement;

            event.returnValue = false;

            var feature = "height=600,width=800,status=no,resizable=yes,toolbar=no,menubar=no,location=no,scrollbars=yes";

            window.open(a.href, "_blank", feature);
            event.cancelBubble = true;
        }

        function selectTag(showContent, selfObj) {
            // 操作标签
            var tag = document.getElementById("tags").getElementsByTagName("li");
            var tag1 = document.getElementById("tags1").getElementsByTagName("li");
            var taglength = tag.length;
            var taglength1 = tag1.length;
            for (i = 0; i < taglength; i++) {
                tag[i].className = "";
            }
            for (j = 0; j < taglength1; j++) {
                tag1[j].className = "";
            }

            selfObj.parentNode.className = "selectTag";
            // 操作内容
            for (i = 0; j = document.getElementById("tagContent" + i); i++) {
                j.style.display = "none";
            }
            document.getElementById(showContent).style.display = "block";
        }

        //测试临时流程选择
        function onSelectProcessUsers() {
            event.returnValue = false;

            window.open("../../WebTestProject/workflow/SelectProcessUsers.aspx", "selectApprover", 'height=500px, width=600px, top=100, left=300, toolbar=no, menubar=no, scrollbars=no, resizable=no,location=no, status=no');
        }
    </script>
</head>
<body style="background-color: #f8f8f8">
    <div id="topContainer" style="width: 99%; margin: 8px 0 0 5px; border: solid 1px #ddd;
        background-color: White;">
        <div style="width: 100%; text-indent: 2em; font-size: 11pt; font-weight: 600; background: url(../images/h2_icon.gif) no-repeat 5px 5px;
            line-height: 30px; padding-bottom: 0px; border-bottom: solid 1px silver;">
            <asp:Label ID="LblTitle" runat="server" Text="拟单"></asp:Label>
        </div>
        <div id="mainframe" style="width: 100%;">
            <dt>服务流程</dt>
            <dd class="ddItem" style="width: 100%; background-color: White;">
                <ul id="tags">
                    <li class="selectTag"><a onclick="selectTag('tagContent0',this)" href="#">秘书服务</a></li>
                    <li><a onclick="selectTag('tagContent2',this)" href="#">员工关系管理</a></li>
                    <li><a onclick="selectTag('tagContent1',this)" href="#">行政服务</a></li>
                    <li><a class="unUse">政府关系</a></li>
                    <li><a class="unUse">媒体关系</a></li>
                    <li><a class="unUse">人员激励服务</a></li>
                    <li><a onclick="selectTag('tagContent25',this)" href="#">人员考核</a></li>
                    <li><a class="unUse">人员获取服务</a></li>
                    <li><a class="unUse">企业文化</a></li>
                    <li><a class="unUse">董事会服务</a></li>
                    <li><a onclick="selectTag('tagContent15',this)" href="#">现金管理</a></li>
                    <li><a onclick="selectTag('tagContent26',this)" href="#">税费管理</a></li>
                    <li><a class="unUse">财务账务</a></li>
                    <li><a onclick="selectTag('tagContent16',this)" href="#">固定资产管理</a></li>
                    <li><a class="unUse">财务内审</a></li>
                    <li><a class="unUse">品牌管理</a></li>
                    <li><a class="unUse" onclick="selectTag('tagContent17',this)" href="#">经营性资质办理</a></li>
                    <li><a class="unUse" onclick="selectTag('tagContent19',this)" href="#">流程管理</a></li>
                    <li><a class="unUse">知识管理</a></li>
                    <li><a onclick="selectTag('tagContent22',this)" href="#">内部审计</a></li>
                    <li><a class="unUse">信息管理服务</a></li>
                    <li><a onclick="selectTag('tagContent13',this)" href="#">资料管理</a></li>
                    <li><a onclick="selectTag('tagContent14',this)" href="#">档案管理</a></li>
                    <li><a class="unUse">投资者关系</a></li>
                    <li><a class="unUse">供应商关系</a></li>
                    <li><a class="unUse">联盟商家</a></li>
                    <li><a class="unUse">教育机构管理</a></li>
                    <li><a class="unUse">会员关系</a></li>
                    <li><a onclick="selectTag('tagContent23',this)" href="#">客户满意度</a></li>
                    <li><a class="unUse">部品材料管理</a></li>
                    <li><a class="unUse">案件办理</a></li>
                    <li><a onclick="selectTag('tagContent21',this)" href="#">股权管理</a></li>
                    <li><a class="unUse">职业发展规划</a></li>
                    <li><a class="unUse">经营管理</a></li>
                    <li><a onclick="selectTag('tagContent6',this)" href="#">设计变更与签证</a></li>
                    <li><a onclick="selectTag('tagContent10',this)" href="#">支付</a></li>
                    <li><a onclick="selectTag('tagContent20',this)" href="#">收费</a></li>
                    <li id="hrSystemEntry" runat="server"><a onclick="selectTag('tagContent11',this)"
                        href="#">人力资源</a></li>
                    <li><a onclick="selectTag('tagContent18',this)" href="#">采购管理</a></li>
                    <li><a onclick="selectTag('tagContentProvider',this)" href="#">人员提供</a></li>
                    <li><a onclick="selectTag('tagContentEmpEncourage',this)" href="#">人员激励</a></li>
                </ul>
                <div id="tagContent">
                    <div class="tagContent selectTag1" id="tagContent0">
                        <p>
                            <a id="ADMINISTRATION1" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/SignReportFrontController.aspx?appName=ADMINISTRATION&programName=QIANBAO"
                                onclick="onDraftLinkClick();">签报</a></p>
                        <div class="clear">
                        </div>
                        <p>
                            <a id="ADMINISTRATION2" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/ReceiveFrontController.aspx?appName=ADMINISTRATION&programName=SHOUWEN"
                                onclick="onDraftLinkClick();">收文</a></p>
                        <div class="clear">
                        </div>
                        <p>
                            <a id="ADMINISTRATION3" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/DispatchFrontController.aspx?appName=ADMINISTRATION&programName=FAWEN_HAN"
                                onclick="onDraftLinkClick();">函</a> <a id="ADMINISTRATION4" visible="false" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/DispatchFrontController.aspx?appName=ADMINISTRATION&programName=FAWEN"
                                    onclick="onDraftLinkClick();">正式发文</a></p>
                        <div class="clear">
                        </div>
                        <p>
                            <a id="ADMINISTRATION5" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/MeetingNotesFrontController.aspx?appName=ADMINISTRATION&programName=MEETING"
                                onclick="onDraftLinkClick();">会议纪要</a></p>
                        <div class="clear">
                        </div>
                        <p>
                            <a id="ADMINISTRATION6" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/NoticeFrontController.aspx?appName=ADMINISTRATION&programName=NOTICE"
                                onclick="onDraftLinkClick();">通知</a></p>
                        <div class="clear">
                        </div>
                        <p>
                            <%--<a id="ADMINISTRATION7" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/SealApplicationFrontController.aspx?appName=ADMINISTRATION&programName=SEALAPP"
                                onclick="onDraftLinkClick();">印章申请</a>--%>
                            <a href="../../SinoOcean.Seagull2.Seal/Seal/Register/SealRegisterController.ashx?processDescKey=Seal_Register&programName=SEALREG&appName=ADMINISTRATION"
                                onclick="onDraftLinkClick();">印章登记</a> <a href="../../SinoOcean.Seagull2.Seal/Seal/Alter/SealAlterTypeSelect.aspx?processDescKey=Seal_Alter&programName=SEALALTER&appName=ADMINISTRATION"
                                    onclick="onDraftLinkClick();">印章变更</a> <a href="../../SinoOcean.Seagull2.Seal/Seal/Destroy/SealDestroyController.ashx?processDescKey=Seal_Destroy&programName=SEALDESTROY&appName=ADMINISTRATION"
                                        onclick="onDraftLinkClick();">印章作废</a>
                            <%--<a style="width: 120px" href="../../SinoOcean.Seagull2.Seal/Seal/Register/SealLoanRequest.ashx?processDescKey=Seal_Loan" onclick="onDraftLinkClick();">印章借用</a> --%>
                            <a href="../../SinoOcean.Seagull2.Seal/Seal/Stamp/SealStampController.ashx?processDescKey=Seal_Use&programName=SEALUSE&appName=ADMINISTRATION"
                                onclick="onDraftLinkClick();">印章用章</a>
                        </p>
                        <div class="clear">
                        </div>
                        <p>
                            <a id="ADMINISTRATION8" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/ContractFrontController.aspx?appName=ADMINISTRATION&programName=CONTRACT"
                                onclick="onDraftLinkClick();">合同</a><a id="ADMINISTRATION9" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/DataLendFormController.aspx?appName=ADMINISTRATION&programName=DATALEND"
                                    onclick="onDraftLinkClick();">资料借出</a></p>
                    </div>
                    <div class="tagContent " id="tagContent1">
                        <div class="clear">
                        </div>
                        <p>
                            <a style="width: 120px" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/AssetApplicationFormController.aspx?appName=ADMINISTRATIVE&programName=ASSETAPP"
                                onclick="onDraftLinkClick();">固定资产申请</a><a style="width: 120px" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/LaptopApplicationFrontController.aspx?appName=ADMINISTRATIVE&programName=LAPTOPAPP"
                                    onclick="onDraftLinkClick();">笔记本电脑申请</a><a style="width: 120px" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/MonthlyPurchaseFrontController.aspx?appName=ADMINISTRATIVE&programName=MonthlyPurchase"
                                        onclick="onDraftLinkClick();">月度采购</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.BooksLending/BooksLending/BooksLendingHomeView.aspx"
                                            onclick="onDraftLinkClick();">图书借阅</a></p>
                    </div>
                    <div class="tagContent " id="tagContent2">
                        <div class="clear">
                        </div>
                        <p>
                            <a href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/LeaveApplicationFormController.aspx?appName=HUMANRESOURCES&programName=LEAVE"
                                onclick="onDraftLinkClick();">请假申请</a><a href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/OverTimeApplicationFormController.aspx?appName=HUMANRESOURCES&programName=OVERTIME"
                                    onclick="onDraftLinkClick();">加班申请</a><a href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/AdjustmentVacationFormController.aspx?appName=HUMANRESOURCES&programName=ADJUSTMENTVACATIOIN"
                                        onclick="onDraftLinkClick();">调休申请</a> <a href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/PublicTravelApplicationFormController.aspx?appName=HUMANRESOURCES&programName=PUBLICTRAVEL"
                                            onclick="onDraftLinkClick();">公出申请</a> <a href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/TravelApplicationFormController.aspx?appName=HUMANRESOURCES&programName=TRAVEL"
                                                onclick="onDraftLinkClick();">出差申请</a> <a style="width: 260px" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/TravelApplicationCtripFrontController.aspx?appName=HUMANRESOURCES&programName=TRAVELCTRIP"
                                                    onclick="onDraftLinkClick();">出差申请（集团总部人员专用）</a></p>
                        <div class="clear">
                        </div>
                        <p>
                            <a style="width: 120px" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/JobReassignmentFormController.aspx?appName=HUMANRESOURCES&programName=JOBRESS"
                                onclick="onDraftLinkClick();">工作变动申请</a> <a style="width: 120px" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/BoardingFrontController.aspx?appName=HUMANRESOURCES&programName=Boarding"
                                    onclick="onDraftLinkClick();">员工转正申请</a></p>
                    </div>
                    <div class="tagContent " id="tagContent6">
                        <p>
                            <a style="width: 120px" href="../../SinoOcean.OA.BizProcesses/SinoOc
                            eanForms/DesignChangeFrontController.aspx?appName=DesignChangeAndVisa&programName=DesignChange"
                                onclick="onDraftLinkClick();">设计变更审批单</a><a style="width: 120px" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/NegotiationRecordsFrontController.aspx?appName=DesignChangeAndVisa&programName=NegotiationRecords"
                                    onclick="onDraftLinkClick();">洽商记录审批单</a>
                        </p>
                    </div>
                    <div class="clear">
                    </div>
                    <div class="tagContent " id="tagContent3">
                        <div class="clear">
                        </div>
                        <p>
                            <a style="width: 120px" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/expenseManager/EvectionFrontController.aspx?appName=EXPENSEMANAGER&programName=EVECTION"
                                onclick="onDraftLinkClick();">差旅费</a> <a style="width: 120px" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/expenseManager/PositionFrontController.aspx?appName=EXPENSEMANAGER&programName=Position"
                                    onclick="onDraftLinkClick();">岗位经费</a> <a style="width: 120px" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/expenseManager/DrawFrontController.aspx?appName=EXPENSEMANAGER&programName=Draw"
                                        onclick="onDraftLinkClick();">款项借支</a> <a style="width: 120px" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/expenseManager/NormalFrontController.aspx?appName=EXPENSEMANAGER&programName=Normal"
                                            onclick="onDraftLinkClick();">一般费用</a> <a style="width: 140px" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/expenseManager/HumanResourcesFrontController.aspx?appName=EXPENSEMANAGER&programName=HumanResources"
                                                onclick="onDraftLinkClick();">人力资源成本费用</a> <a style="width: 140px" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/expenseManager/AssetsFrontController.aspx?appName=EXPENSEMANAGER&programName=Assets"
                                                    onclick="onDraftLinkClick();">固定资产交验</a> <a style="width: 140px" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/expenseManager/ContractPayFrontController.aspx?appName=EXPENSEMANAGER&programName=ContractPay"
                                                        onclick="onDraftLinkClick();">合同类支付</a>
                        </p>
                    </div>
                    <div class="clear">
                    </div>
                    <div class="tagContent" id="tagContent10">
                        <p>
                            <%-- <a style="width: 160px" href="#" onclick ="return openStandardForm('../../Finance/Payment/GeneralPayment/GeneralPaymentController.ashx?processDescKey=GeneralPayment&appName=流程&programName=一般流程支付申请&relativeParams=ProjectName%3d流程-2011');">一般费用支付申请单</a>--%>
                            <a style="width: 120px" href="#" onclick="return openSmallForm('../../Finance/Payment/Index.aspx');">
                                支付申请单</a><a style="width: 120px" href="#" onclick="return	    openSmallForm('../../Finance/LoanForm/Index.aspx');">
                                    借款申请单</a><a style="width: 120px" href="#" onclick="return	    openSmallForm('../../Finance/Payment/GenerateCertificate/CertificateListView.aspx');">
                                        生成凭证</a><a style="width: 120px" href="#" onclick="return openSmallForm('../../Finance/Payment/ModalDialog/CurrentPreselection.aspx');">
                                            往来款</a><a style="width: 120px" href="#" onclick="return openSmallForm('../../Finance/Payment/Index.aspx?applicationCategoryCode=012');">
                                                支出凭单</a> <a style="width: 160px" href="#" onclick="return openSmallForm('../../Finance/Payment/ModalDialog/ComputerPreselection.aspx');">
                                                    笔记本设备申请单</a>
                        </p>
                        <div class="clear">
                        </div>
                    </div>
                    <div class="tagContent" id="tagContent11">
                        <p>
                            <%-- <a style="width: 160px" href="#" onclick ="return openStandardForm('../../Finance/Payment/GeneralPayment/GeneralPaymentController.ashx?processDescKey=GeneralPayment&appName=流程&programName=一般流程支付申请&relativeParams=ProjectName%3d流程-2011');">一般费用支付申请单</a>--%>
                            <a style="width: 120px" href="#" runat="server" id="hrEmployeeInformation" onclick="return openSmallForm('../../HumanResource/Employ/LaunchInitial/LaunchInitialController.ashx?processDescKey=LaunchInitial')">
                                员工信息收集</a> <a style="width: 100px" href="#" runat="server" id="hrEmployeeEntry" onclick="return openSmallForm('../../HumanResource/EmployeeEntry/EmployeeEntryMain.aspx')">
                                    员工入职</a><a style="width: 120px" href="#" runat="server" id="hrJobTransfer" onclick="return openSmallForm('../../SinoOcean.Seagull2.JobTransfer/EmployeeJobTransfer/JobTransferApply/JobTransferApplyController.ashx?processDescKey=HR_JOBTRANSFER')">
                                        员工调动(批量)</a> <a style="width: 100px" href="#" runat="server" id="A20" onclick="return openSmallForm('../../SinoOcean.Seagull2.JobTransfer/EmployeeJobTransfer/JobTransferApply/JobTransferFrontPage.aspx')">
                                            员工调动</a> <a style="width: 100px" href="#" id="hrResignationApply" onclick="return openSmallForm('../../HumanResource/Dismission/DismissionApply/ResignationApplyController.ashx?processDescKey=HR_DISMISSION')">
                                                离职申请</a> <a style="width: 120px" href="#" id="hrDismissApply" onclick="return openSmallForm('../../HumanResource/Dismission/DismissionApply/Preselection.aspx')">
                                                    被动离职申请</a> <a style="width: 120px" href="#" id="hrRetiredApply" onclick="return openSmallForm('../../HumanResource/Dismission/DismissionApply/RetiredApplyController.ashx?processDescKey=HR_DISMISSION')">
                                                        退休申请</a> <a style="width: 120px" href="#" id="A44" onclick="return openSmallForm('../../HumanResource/Dismission/DismissionApply/PreselectionDimissionSettlement.aspx')">
                                                            离职申请(结算)</a><a style="width: 120px" href="#" id="A1" onclick="return openSmallForm('../../SinoOcean.Seagull2.SalaryCheck/SalaryCheck/SalaryCheckStart/SalaryCheckStartController.ashx?processDescKey=HR_SalaryCheck&relativeParams=ProjectName%3d流程-2011')">
                                                                工资核发</a><a style="width: 120px" href="#" id="A27" onclick="return openSmallForm('../../SinoOcean.Seagull2.SalaryCheckNew/SalaryCheckNew/Defalut.aspx')">
                                                                    工资核发(新)</a> <a style="width: 120px" href="#" id="A26" onclick="return openSmallForm('../../SinoOcean.Seagull2.WelfarePayment/Entry/entry.html')">
                                                                        福利缴纳</a><a style="width: 160px" href="#" id="A2" onclick="return openSmallForm('../../SinoOcean.Seagull2.WelfarePaymentF/WelfarePaymentF/PreposePage.aspx')">
                                                                            社保公积金缴纳</a> <a style="width: 140px" href="#" id="A7" onclick="return openSmallForm('../../HumanResource/EmployeeInformationModify/EmployeeInformationModifyForEmployee/EmployeeeInformationModifyController.ashx?processDescKey=EmployeeInformationModify')">
                                                                                个人信息调整</a> <a style="width: 140px" href="#" id="A8" onclick="return openSmallForm('../../HumanResource/EmployeeInformationModify/EmployeeInformationModifyFromHRStart/EmployeeInformationModifyFromHRStartController.ashx?processDescKey=EmployeeInformationModifyFromHRStart')">
                                                                                    员工信息调整</a> <a style="width: 140px" href="#" id="A10" onclick="return openSmallForm('../../SinoOcean.Seagull2.EmployeeContract/ContractUpdate/ContractUpdateStartView.aspx')">
                                                                                        合同变更</a> <a style="width: 140px" href="#" id="A9" onclick="return openSmallForm('../../SinoOcean.Seagull2.EmployeeContract/RenewalContract/RenewalPortal.aspx')">
                                                                                            合同续签</a>
                        </p>
                        <div class="clear">
                            <p>
                            </p>
                        </div>
                        <p>
                            <a style="width: 140px" href="#" id="A18" onclick="return openSmallForm('../../SinoOcean.Seagull2.Attendence/Attendence/Calendar/FrontPage.aspx')">
                                日历表设置</a> <a style="width: 150px" href="#" id="A11" onclick="return openSmallForm('../../SinoOcean.Seagull2.Attendence/Attendence/CycleAndDaytimeSet/CycleSetStartView.aspx')">
                                    考勤周期设置</a><a style="width: 150px" href="#" id="A19" onclick="return openSmallForm('../../SinoOcean.Seagull2.Attendence/Attendence/CycleAndDaytimeSet/DaytimeSetStartView.aspx')">
                                        考勤班次设置</a><a style="width: 140px" href="#" id="A12" onclick="return openSmallForm('../../SinoOcean.Seagull2.Attendence/Attendence/AttendenceItem/AttendenceItemController.ashx?processDescKey=Att_Item')">
                                            考勤项设置</a><a style="width: 140px" href="#" id="A15" onclick="return openSmallForm('../../SinoOcean.Seagull2.Attendence/Attendence/OverTimeApply/OverTimeApplyController.ashx?processDescKey=Att_OverTime')">
                                                申请加班</a> <a style="width: 150px" href="#" id="A13" onclick="return openSmallForm('../../SinoOcean.Seagull2.Attendence/Attendence/EmployeeInfoSet/FrontPage.aspx')">
                                                    员工考勤参数设置</a> <a style="width: 140px" href="#" id="A17" onclick="return openSmallForm('../../SinoOcean.Seagull2.Attendence/Attendence/EmployeeInfoBatchSet/FrontPage.aspx')">
                                                        员工考勤参数调整</a> <a style="width: 140px" href="#" id="A14" onclick="return openSmallForm('../../SinoOcean.Seagull2.Attendence/Attendence/LeaveApply/LeaveApplyController.ashx?processDescKey=Att_Leave')">
                                                            请休假申请</a> <a style="width: 140px" href="#" id="A16" onclick="return openSmallForm('../../SinoOcean.Seagull2.Attendence/Attendence/LeaveChange/FrontPage.aspx')">
                                                                请休假变更</a>
                        </p>
                        <p>
                            <a style="width: 140px" href="#" id="A25" onclick="return openSmallForm('../../SinoOcean.Seagull2.Attendence/Attendence/Appeal/ImportCardView.aspx')">
                                打卡明细导入</a><a style="width: 140px" href="#" onclick="return openSmallForm('../../SinoOcean.Seagull2.Attendence/Attendence/Appeal/ComputeAttendenceResultView.aspx')">
                                    计算考勤结果</a><a style="width: 140px" href="#" onclick="return openSmallForm('../../SinoOcean.Seagull2.Attendence/Attendence/Appeal/FrontPage.aspx')">
                                        考勤异常申诉</a><a style="width: 170px" href="#" onclick="return openSmallForm('../../SinoOcean.Seagull2.Attendence/Attendence/Summary/FrontPage.aspx')">
                                            月度考勤情况汇总表计算</a><a style="width: 170px" href="#" onclick="return openSmallForm('../../SinoOcean.Seagull2.Attendence/Attendence/Summary/SummaryView.aspx')">
                                                月度考勤情况汇总表查询</a>
                        </p>
                        <p>
                            <a style="width: 140px" href="#" id="A21" onclick="return openSmallForm('../../SinoOcean.Seagull2.HumanResource.Unofficial/Entry/EmployeeEntryMain.aspx')">
                                非正式员工入职</a> <a style="width: 140px" href="#" id="A22" onclick="return openSmallForm('../../SinoOcean.Seagull2.HumanResource.Unofficial/UnofDismission/UnofDismissionApply/Preselection.aspx')">
                                    非正式员工离职</a> <a style="width: 150px" href="#" id="A23" onclick="return openSmallForm('../../SinoOcean.Seagull2.HumanResource.Unofficial/UnofDismission/UnofDismissionApply/UnofDismissionApplyController.ashx?processDescKey=UnofDISMISSION')">
                                        非正式员工主动离职</a> <a style="width: 150px" href="#" id="A24" onclick="return openSmallForm('../../SinoOcean.Seagull2.HR.EmployeeRegular/EmployeeRegular/Preselection.aspx?IsT=true')">
                                            员工转正</a><a style="width: 150px" href="#" id="A43" onclick="return openSmallForm('../../HumanResource/DimissionSettlement/DimissionSettlementMain.aspx')">
                                                设置补偿金政策参数</a>
                        </p>
                        <p>
                            <a style="width: 150px" href="#" id="A21" onclick="return openSmallForm('../../SinoOcean.Seagull2.WelfarePayment/WelfarePaymentF/WelfareReport/WelfareDetail/WelfareDetailEntry.aspx')">
                                社保公积金明细表</a> <a style="width: 170px" href="#" id="A22" onclick="return openSmallForm('../../SinoOcean.Seagull2.WelfarePayment/WelfarePaymentF/WelfareReport/WelfareSum/WelfareSumEntry.aspx')">
                                    社保公积金缴纳汇总表</a> <a style="width: 220px" href="#" id="A23" onclick="return openSmallForm('../../SinoOcean.Seagull2.WelfarePayment/WelfarePaymentF/WelfareReport/WelfareSumOrg/WelfareSumOrgEntry.aspx')">
                                        社保公积金汇总（业务组织）表</a> <a style="width: 150px" href="#" id="A24" onclick="return openSmallForm('../../SinoOcean.Seagull2.SalaryCheck/SalaryCheck/SalaryCheckReportF/SalaryCheckDetail/SalaryCheckDetailEntry.aspx')">
                                            工资明细表</a> <a style="width: 150px" href="#" id="A43" onclick="return openSmallForm('../../SinoOcean.Seagull2.SalaryCheck/SalaryCheck/SalaryCheckReportF/SalaryCheckSum/SalaryCheckSumEntry.aspx')">
                                                工资汇总表</a> <a style="width: 160px" href="#" id="A43" onclick="return openSmallForm('../../SinoOcean.Seagull2.SalaryCheck/SalaryCheck/SalaryCheckReportF/SalaryCheckSumOrg/SalaryCheckSumOrgEntry.aspx')">
                                                    工资汇总（汇总）表</a>
                        </p>
                        <div class="clear">
                            <p>
                            </p>
                        </div>
                        <p>
                            <a href="#" onclick="return openSmallForm('../../SinoOcean.Seagull2.Excitation/Excitation/PlanBatch/PlanCreateController.ashx?processDescKey=Excitation_PlanManage')"
                                style="width: 120px" target="_blank">计划维护</a> <a style="width: 120px" href="#" id="A3"
                                    onclick="return openSmallForm('../../SinoOcean.Seagull2.SalaryPlan/SalaryPlan/SalaryPlanSettings/SalaryPlanSettingsController.ashx?processDescKey=HR_SalaryPlan_Settings&relativeParams=ProjectName%3d流程-2011')">
                                    薪酬方案设置</a> <a style="width: 150px" href="#" id="A4" onclick="return openSmallForm('../../SinoOcean.Seagull2.SalaryPlan/SalaryPlan/EmployeeBaseInfo/SelectEmployee.aspx')">
                                        员工工资基础信息</a> <a style="width: 120px" href="#" id="A6" onclick="return openSmallForm('../../SinoOcean.Seagull2.SalaryPlan/SalaryPlan/MonthSalaryConfig/MonthSalaryConfig/chooseHRMgmtUnit.aspx')">
                                            月度工资配置</a> <a style="width: 120px" href="#" id="A5" onclick="return openSmallForm('../../SinoOcean.Seagull2.Excitation/Excitation/RestrictedStockTransfer/RestrictedStockTransferStartView.aspx')">
                                                限制性股票过户</a> <a href="#" onclick="return openSmallForm('../../SinoOcean.Seagull2.Excitation/Excitation/PlanBatch/PlanSelect.aspx')"
                                                    style="width: 120px" target="_blank">批次人维护</a> <a href="#" onclick="return openSmallForm('../../SinoOcean.Seagull2.Excitation/Excitation/ExecuteAndSelling/VestStartView.aspx')"
                                                        style="width: 150px" target="_blank">购股权行权及兑现</a> <a href="#" onclick="return openSmallForm('../../SinoOcean.Seagull2.Excitation/Excitation/RestrictedStockTransfer/RestrictedStockTransferStartView.aspx')"
                                                            style="width: 120px" target="_blank" onclick="onDraftLinkClick();">限制性股票过户</a>
                            <a href="#" onclick="return openSmallForm('../../SinoOcean.Seagull2.Excitation/Excitation/Check/UploadCheckDataController.ashx?processDescKey=Excitation_Reconciliation')"
                                style="width: 80px" target="_blank">对账</a> <a href="#" onclick="return openSmallForm('../../SinoOcean.Seagull2.Excitation/Excitation/DismissionReport/DismissionReportFrontPage.aspx')"
                                    style="width: 120px" onclick="onDraftLinkClick();">生成离职报表</a> <a href="#" onclick="return openSmallForm('../../SinoOcean.Seagull2.Excitation/Excitation/DelayEffect/SignatureController.ashx?processDescKey=Excitation_DelayEffect&appName=HUMANRESOURCES&programName=长期激励&batch=1')"
                                        style="width: 120px" target="_blank">延迟生效</a> <a href="#" onclick="return openSmallForm('../../SinoOcean.Seagull2.Excitation/Excitation/VestChange/VestChangeController.ashx?processDescKey=Excitation_ChangeDetails')"
                                            style="width: 120px" target="_blank">可行权变更</a><a href="#" onclick="return openSmallForm('../../SinoOcean.Seagull2.Excitation/Excitation/ExecuteAndSelling/VestTranferStart.aspx')"
                                                style="width: 120px" target="_blank">绩效达标确认</a>
                        </p>
                    </div>
                    <div class="tagContent" id="tagContent13">
                        <p>
                            <a style="width: 120px" href="../../SinoOcean.Seagull2.Archive/Material/Collect/MaterialCollectController.ashx?processDescKey=Material_Collect&programName=资料征集&appName=资料管理"
                                onclick="onDraftLinkClick();">资料征集</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.Archive/Material/Filling/MaterialFillingController.ashx?processDescKey=Material_Filling&programName=归资料库&appName=资料管理"
                                    onclick="onDraftLinkClick();">归资料库</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.Archive/Material/AdminKeeping/MaterialAdminKeepingController.ashx?processDescKey=Material_Admin_Keeping&programName=直接入库&appName=资料管理"
                                        onclick="onDraftLinkClick();">直接入库</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.Archive/Material/Sorting/MaterialSortingController.ashx?processDescKey=Material_Arrange&programName=资料整理&appName=资料管理"
                                            onclick="onDraftLinkClick();">资料整理</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.Archive/Material/Use/MaterialUseController.ashx?processDescKey=Material_Use_Apply&programName=资料借阅&appName=资料管理"
                                                onclick="onDraftLinkClick();">资料借阅</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.Archive/Material/Return/MaterialReturnController.ashx?processDescKey=Material_Use_Return&programName=资料归还&appName=资料管理"
                                                    onclick="onDraftLinkClick();">资料归还</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.Archive/Material/UrgeReturn/MaterialUrgeReturnController.ashx?processDescKey=Material_Use_Urge&programName=资料催还&appName=资料管理"
                                                        onclick="onDraftLinkClick();">资料催还</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.Archive/Material/Convert/MaterialConvertController.ashx?processDescKey=Material_Transfer_Archive&programName=资料转档案&appName=资料管理"
                                                            onclick="onDraftLinkClick();">资料转档案</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.Archive/Material/Destroy/MaterialDestroyController.ashx?processDescKey=Material_Destroy&programName=资料销毁&appName=资料管理"
                                                                onclick="onDraftLinkClick();">资料销毁</a> <a style="width: 180px" href="../../SinoOcean.Seagull2.Archive/Material/Check/MaterialCheckController.ashx?processDescKey=Material_Check&programName=工程档案竣工预验收&appName=资料管理"
                                                                    onclick="onDraftLinkClick();">工程档案竣工预验收</a>
                            <a style="width: 180px" href="../../SinoOcean.Seagull2.Archive/Material/Record/MaterialRecordController.ashx?processDescKey=Material_Record&programName=工程档案竣工备案&appName=资料管理"
                                onclick="onDraftLinkClick();">工程档案竣工备案</a>
                        </p>
                        <div class="clear">
                        </div>
                    </div>
                    <div class="tagContent" id="tagContent14">
                        <p>
                            <a style="width: 150px" href="../../SinoOcean.Seagull2.Archive/Archive/Prepare/ArchivePrepareController.ashx?processDescKey=Archive_Prepare&programName=工作场地准备&appName=档案管理"
                                onclick="onDraftLinkClick();">工作场地准备</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.Archive/Archive/Collect/ArchiveCollectController.ashx?processDescKey=Archive_Collect&programName=档案征集&appName=档案管理"
                                    onclick="onDraftLinkClick();">档案征集</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.Archive/Archive/Filling/ArchiveFillingController.ashx?processDescKey=Archive_Filling&programName=归档案库&appName=档案管理"
                                        onclick="onDraftLinkClick();">归档案库</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.Archive/Archive/AdminFilling/ArchiveAdminFillingController.ashx?processDescKey=Archive_Admin_Filling&programName=直接归档&appName=档案管理"
                                            onclick="onDraftLinkClick();">直接归档</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.Archive/Archive/Sorting/ArchiveSortingController.ashx?processDescKey=Archive_Arrange&programName=档案整理&appName=档案管理"
                                                onclick="onDraftLinkClick();">档案整理</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.Archive/Archive/Check/ArchiveCheckController.ashx?processDescKey=Archive_Check&programName=档案室管理&appName=档案管理"
                                                    onclick="onDraftLinkClick();">档案室管理</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.Archive/Archive/Use/ArchiveUseController.ashx?processDescKey=Archive_Use_Apply&programName=档案借阅&appName=档案管理"
                                                        onclick="onDraftLinkClick();">档案借阅</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.Archive/Archive/Return/ArchiveReturnController.ashx?processDescKey=Archive_Use_Return&programName=档案归还&appName=档案管理"
                                                            onclick="onDraftLinkClick();">档案归还</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.Archive/Archive/UrgeReturn/ArchiveUrgeReturnController.ashx?processDescKey=Archive_Use_Urge&programName=档案催还&appName=档案管理"
                                                                onclick="onDraftLinkClick();">档案催还</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.Archive/Archive/Edit/ArchiveEditController.ashx?processDescKey=Archive_Edit&programName=档案编研&appName=档案管理"
                                                                    onclick="onDraftLinkClick();">档案编研</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.Archive/Archive/Transfer/ArchiveTransferController.ashx?processDescKey=Archive_Transfer&programName=档案移交&appName=档案管理"
                                                                        onclick="onDraftLinkClick();">档案移交</a>
                            <a style="width: 150px" href="../../SinoOcean.Seagull2.Archive/Archive/Destroy/ArchiveDestroyController.ashx?processDescKey=Archive_Destroy&programName=档案销毁&appName=档案管理"
                                onclick="onDraftLinkClick();">档案销毁</a> <a style="width: 150px" href="../../SinoOcean.Seagull2.Archive/Archive/Register/RequestArchiveController.ashx?processDescKey=Archive_Register&programName=竣工档案登记&appName=档案管理"
                                    onclick="onDraftLinkClick();">竣工档案登记</a>
                        </p>
                        <div class="clear">
                        </div>
                    </div>
                    <div class="tagContent" id="tagContent26">
                        <p>
                            <a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Taxes/WebControls/CorporationSelector/CorporationSelector.aspx?title=票据领购登记前置页面&roleKey=Taxes:InvoiceExecutive&processDescKey=Taxes_TicketReceivingPurchasing&redirectUrl=/McsWebApp/SinoOcean.Seagull2.Taxes/Invoice/Register/RegisterController.ashx');"
                                href="#">票据领购登记</a><a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Taxes/WebControls/CorporationSelector/CorporationSelector.aspx?title=票据使用登记前置页面&roleKey=Taxes:BusinessPersonnel&processDescKey=Taxes_TicketUseRegister&redirectUrl=/McsWebApp/SinoOcean.Seagull2.Taxes/Invoice/CheckOut/CheckOutViewController.ashx');"
                                    href="#">票据使用登记</a> <a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Taxes/WebControls/CorporationSelector/CorporationSelector.aspx?title=票据核销登记前置页面&roleKey=Taxes:InvoiceExecutive&processDescKey=Taxes_TicketCancelAfterVerification&redirectUrl=/McsWebApp/SinoOcean.Seagull2.Taxes/Invoice/WriteOff/WriteOffController.ashx');"
                                        href="#">票据核销登记</a> <a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Taxes/WebControls/CorporationSelector/CorporationSelector.aspx?title=申报资产损失前置页面&processDescKey=Taxes_AssetsLoss&redirectUrl=/McsWebApp/SinoOcean.Seagull2.Taxes/Assets/LossOfAssets/LossOfAssetsController.ashx&roleKey=Taxes:TaxExecutive');"
                                            href="#">资产申报损失</a> <a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Taxes/Taxes/GeneralSetting/GeneralSettingController.ashx?processDescKey=Taxes_GeneralSetting_FillIn&NationCode=CHN');"
                                                href="#">通用税务基本信息维护</a> <a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Taxes/WebControls/CityAreaWithFormSelector/CityAreaWithFormSelector.aspx?title=区域税务基本信息维护前置页面&processDescKey=Taxes_AreaSetting_FillIn&redirectUrl=/MCSWebApp/SinoOcean.Seagull2.Taxes/Taxes/AreaSetting/AreaSettingController.ashx&roleKey=Taxes:TaxExecutive&NationCode=CHN');"
                                                    href="#">区域税务基本信息维护</a> <a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Taxes/WebControls/CorporationSelector/CorporationSelector.aspx?title=法人公司税务基本信息维护前置页面&roleKey=Taxes:CorporationSetting_DraftReceipt&processDescKey=Taxes_CorporationTaxesInformationSafeguard_FillIn&redirectUrl=/McsWebApp/SinoOcean.Seagull2.Taxes/Taxes/CorporationSetting/CorporationSettingController.ashx');"
                                                        href="#">法人公司税务基本信息维护</a><a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Taxes/WebControls/TaxComputeSelector/TaxComputeSelector.aspx?title=税费计算及支付申请(正常申报)前置页面&roleKey=Taxes:TaxExecutive&processDescKey=Taxes_ComputeTaxes&declaretypecode=1&redirectUrl=/McsWebApp/SinoOcean.Seagull2.Taxes/Taxes/ComputeTaxes/ComputeTaxesController.ashx');"
                                                            href="#">税费计算及支付申请(正常申报)</a><a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Taxes/WebControls/TaxComputeSelector/TaxComputeSelector.aspx?title=税费计算及支付申请(临时申报)前置页面&roleKey=Taxes:TaxExecutive&processDescKey=Taxes_ComputeTaxesTemporary&declaretypecode=2&redirectUrl=/McsWebApp/SinoOcean.Seagull2.Taxes/Taxes/ComputeTaxes/ComputeTaxesController.ashx');"
                                                                href="#">税费计算及支付申请(临时申报)</a><a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Taxes/WebControls/TaxComputeSelector/TaxComputeSelector.aspx?title=退税&roleKey=Taxes:TaxExecutive&processDescKey=Taxes_CorporationRefundTaxes&redirectUrl=/McsWebApp/SinoOcean.Seagull2.Taxes/Taxes/CorporationRefundTaxes/CorporationRefundTaxesController.ashx');"
                                                                    href="#">退税</a> <a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Taxes/WebControls/TaxComputeSelector/TaxComputeSelector.aspx?title=企业所得税汇算清缴前置页面&roleKey=Taxes:TaxExecutive&processDescKey=Taxes_ComputeTaxesSum&redirectUrl=/McsWebApp/SinoOcean.Seagull2.Taxes/Taxes/ComputeTaxesSum/ComputeTaxesSumController.ashx');"
                                                                        href="#">企业增值税汇算清缴</a><a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Taxes/WebControls/TaxComputeSelector/TaxComputeSelector.aspx?title=土地增值税清算前置页面&roleKey=Taxes:TaxExecutive&processDescKey=Taxes_ComputeTaxesLAT&redirectUrl=/McsWebApp/SinoOcean.Seagull2.Taxes/Taxes/ComputeTaxesLAT/ComputeTaxesLATController.ashx');"
                                                                            href="#">土地增值税清算</a><a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Taxes/WebControls/TaxComputeSelector/TaxComputeSelector.aspx?title=税费代扣代缴&roleKey=Taxes:TaxExecutive&processDescKey=Taxes_WithholdAndRemitTaxes&redirectUrl=/McsWebApp/SinoOcean.Seagull2.Taxes/Taxes/WithholdAndRemitTaxes/WithholdAndRemitTaxesController.ashx');"
                                                                                href="#">税费代扣代缴</a><a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Taxes/WebControls/TaxComputeSelector/TaxComputeSelector.aspx?title=缴纳滞纳金及罚款&roleKey=Taxes:TaxExecutive&processDescKey=Taxes_OverduePaymentAndPenalty&redirectUrl=/McsWebApp/SinoOcean.Seagull2.Taxes/Taxes/OverduePaymentAndPenalty/OverduePaymentAndPenaltyController.ashx');"
                                                                                    href="#">缴纳滞纳金及罚款</a>
                        </p>
                        <div class="clear">
                            <p>
                            </p>
                        </div>
                    </div>
                    <div class="tagContent" id="tagContent15">
                        <p>
                            <a style="width: 250px" onclick="return openSmallForm('../../SinoOcean.Seagull2.Funds/Funds/BankRelationship/BankSelect.aspx');"
                                href="#">新银行选择</a> <a style="width: 250px" onclick="return openSmallForm('../../SinoOcean.Seagull2.Funds/Funds/BankInvestigation/ChooseInvestigatorController.ashx?processDescKey=Funds_BankManager_BankInvestigation');"
                                    href="#">银行考察</a>
                        </p>
                        <p>
                            <a style="width: 250px" onclick="return openSmallForm('../../SinoOcean.Seagull2.Funds/Funds/OpenAccount/OpenAccountController.ashx?processDescKey=Funds_AccountManager_OpenAccount');"
                                href="#">开立账户</a> <a style="width: 250px" onclick="return openSmallForm('../../SinoOcean.Seagull2.Funds/Funds/CancelAccount/CancelAccountController.ashx?processDescKey=Funds_AccountManager_CancelAccount');"
                                    href="#">撤销账户</a> <a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Funds/Funds/ModifyAccount/ModifyAccountController.ashx?processDescKey=Funds_AccountManager_ModifyAccount');"
                                        href="#">银行账户信息变更</a>
                        </p>
                        <p>
                            <a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Funds/Funds/InternalTransfer/InternalTransferController.ashx?processDescKey=Funds_Allocate_InternalTransfer');"
                                href="javascript:void(0)">境内非现金池内部转存款</a> <a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Funds/Funds/FundsTurnoverApply/TurnoverApplyController.ashx?processDescKey=Funds_Allocate_Turnover');"
                                    href="javascript:void(0)">境内非现金池子公司申请资金上缴</a> <a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Funds/Funds/FundsTurnoverApply/TurnoverApplyController.ashx?processDescKey=Funds_Allocate_AppropriatedApply');"
                                        href="javascript:void(0)">境内非现金池子公司申请资金上缴下拨</a> <a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Funds/Funds/FundsAppropriate/Default.aspx');"
                                            href="javascript:void(0)">资金调拨支付</a>
                        </p>
                        <p>
                            <a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Funds/WebControls/CorporationSelector/CorporationSelector.aspx?title=境内非现金池子公司申请资金上缴前置页面&processDescKey=Funds_Allocate_TurnoverToPay&redirectUrl=/MCSWebApp/SinoOcean.Seagull2.Funds/Funds/FundsTurnoverToPay/FundsTurnoverToPayController.ashx&roleKey=FUND:JSJZ_XJGLZY');"
                                href="#">境内非现金池子公司申请资金上缴</a>
                        </p>
                        <p>
                            <a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Funds/Funds/CashPoolMasterAccountAdjustment/MasterAccountAdjustmentIndex.aspx');"
                                href="#">现金池主账户参数设置</a> <a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Funds/Funds/CashPoolSubAccountAdjustment/SubAccountAdjustmentIndex.aspx');"
                                    href="#">现金池子账户参数设置</a>
                        </p>
                        <p>
                            <a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Funds/Funds/BaseInfo/BaseInfoController.ashx?processDescKey=Funds_Parameter_Setting');"
                                href="javascript:void(0)">资金关键信息维护</a>
                        </p>
                        <p>
                            <a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Funds.ExtendService/MockStartup/Default.aspx');"
                                href="#">模拟发起页面汇总</a>
                        </p>
                    </div>
                    <div class="tagContent" id="tagContent16">
                        <p>
                            <a style="width: 150px" onclick="return openSmallForm('../../SinoOcean.Seagull2.FixedAssets/FixedAssets/FixedAssetRegister/AssetInfoFrontPage.aspx');"
                                href="#">固定资产信息收集</a> <a style="width: 120px" onclick="return openSmallForm('../../SinoOcean.Seagull2.FixedAssets/FixedAssets/ModalDialog/RequestTypeSelector.aspx');"
                                    href="#">资产申请</a> <a style="width: 120px" onclick="return openSmallForm('../../SinoOcean.Seagull2.FixedAssets/FixedAssets/ModalDialog/PurchaseTypeSelector.aspx');"
                                        href="#">资产采购</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.FixedAssets/FixedAssets/HandOver/AssetHandOverController.ashx?processDescKey=FixedAssets_HandOver"
                                            onclick="onDraftLinkClick()" target="_blank">资产转交</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.FixedAssets/FixedAssets/Maintain/AssetMaintainController.ashx?processDescKey=FixedAssets_Maintain"
                                                onclick="onDraftLinkClick()" target="_blank">资产维修</a> <a style="width: 120px" href="../../SinoOcean.Seagull2.FixedAssets/FixedAssets/AssetScrap/AssetScrapController.ashx?processDescKey=FixedAssetsScrap"
                                                    onclick="onDraftLinkClick()" target="_blank">资产报废</a>
                            <%--<a style="width: 180px" href="../../SinoOcean.Seagull2.FixedAssets/FixedAssets/FixedAssetAcceptance/FixedAssetAcceptanceController.ashx?processDescKey=FixedAssetsExamine" onclick="onDraftLinkClick()" target="_blank">固定资产验收</a> --%>
                            <a style="width: 120px" href="../../SinoOcean.Seagull2.FixedAssets/FixedAssets/AssetCheck/AssetCheckCompanySelector.aspx"
                                onclick="onDraftLinkClick()" target="_blank">资产盘点</a> <a style="width: 180px" onclick="return openSmallForm('../../SinoOcean.Seagull2.FixedAssets/FixedAssets/ConfigurationManage/AssetConfigModelPage.aspx');"
                                    href="#">物品规格配置模版维护 </a><a style="width: 180px" onclick="return openSmallForm('../../SinoOcean.Seagull2.FixedAssets/FixedAssets/ConfigurationManage/AssetStorageManagePage.aspx');"
                                        href="#">物品存放地点维护 </a><a style="width: 180px" onclick="return openSmallForm('../../SinoOcean.Seagull2.FixedAssets/FixedAssets/AssetInfomationMaintain/AssetInfomationMaintainController.ashx?processDescKey=FixedAssets_InfoMaintain');"
                                            href="#">固定资产信息维护 </a>
                        </p>
                    </div>
                    <div class="tagContent" id="tagContent18">
                        <p>
                            <a style="width: 150px" href="../../SinoOcean.Seagull2.Tender/Tender/ModalDialog/EntrustFrontPage.aspx"
                                onclick="onDraftLinkClick()" target="_blank">直接委托</a> <a style="width: 150px" href="../../SinoOcean.Seagull2.Tender/Tender/ModalDialog/InquiryPriceFrontPage.aspx"
                                    onclick="onDraftLinkClick()" target="_blank">询价</a> <a style="width: 150px" href="../../SinoOcean.Seagull2.Tender/Tender/ModalDialog/BiddingFrontPage.aspx"
                                        onclick="onDraftLinkClick()" target="_blank">招投标</a> <a style="width: 150px" href="../../SinoOcean.Seagull2.Tender/Tender/ModalDialog/StrategyPurchaseFrontPage.aspx"
                                            onclick="onDraftLinkClick()" target="_blank">战略采购</a> <a style="width: 130px" href="../../SinoOcean.Seagull2.Contract/ContractManagement/TemplateModify/TemplateCategoryModifyView.aspx?TemplateCategoryID=2"
                                                onclick="onDraftLinkClick()" target="_blank">招标范本管理</a>
                            <%--<a style="width: 150px" href="../../SinoOcean.Seagull2.Tender/Tender/StrategyCommissioned/StrategyCommissionedController.ashx?processDescKey=Entrust_StrategicSourcing"
                                onclick="onDraftLinkClick()" target="_blank">直接委托(战略采购)</a> --%>
                        </p>
                    </div>
                    <div class="clear">
                    </div>
                    <div class="tagContent" id="tagContentProvider">
                        <p>
                            <a style="width: 180px" href="#" onclick="return openStandardForm('http://localhost/MCSWebApp/SinoOcean.Seagull2.EmpProvider/EmpProvide/StationInstruction/StationInstructionMaintainFrontPage.aspx');">
                                岗位说明书</a> <a style="width: 250px" href="#" onclick="return openStandardForm('http://localhost/MCSWebApp/SinoOcean.Seagull2.EmpProvider/EmpProvide/StationInstruction/RedirectorController.ashx?processDescKey=StationInstructionMaintainProc');">
                                    岗位说明书（录入专员入口）</a><a href="/MCSWebApp/SinoOcean.Seagull2.EmpProvider/EmpProvide/RecruitRequest/RecruitRquestFontPage.aspx"
                                        target="_blank">招聘申请</a> <a style="width: 180px" href="#" onclick="return openStandardForm('http://localhost/MCSWebApp/SinoOcean.Seagull2.EmpProvider/EmpProvide/InternalApplyRequest/InternalRecruitRequestFront.aspx');">
                                            内部应聘</a> <a style="width: 180px" href="#" onclick="return openStandardForm('http://localhost/MCSWebApp/SinoOcean.Seagull2.EmpProvider/EmpProvide/InternalRecommendRequest/InternalRecommendRequestFront.aspx');">
                                                内部推荐</a><a href="/MCSWebApp/SinoOcean.Seagull2.EmpProvider/EmpProvide/ResumeCheck/Entry.aspx"
                                                    target="_blank">简历筛选</a> <a style="width: 180px" href="#" onclick="return openStandardForm('http://localhost/MCSWebApp/SinoOcean.Seagull2.EmpProvider/EmpProvide/EmployeExecute/EmployeExecuteDiscussController.ashx?processDescKey=StationInstructionMaintainProc');">
                                                        录用洽谈</a><a style="width: 180px" href="#" onclick="return openStandardForm('http://localhost/MCSWebApp/SinoOcean.Seagull2.EmpProvider/EmpProvide/RecruitStateMaintain/Entry.aspx');">
                                                            职位招聘状态维护</a> <a style="width: 180px" href="#" onclick="return openStandardForm('http://localhost/MCSWebApp/SinoOcean.Seagull2.EmpProvider.OutSite/Recruit/Default.aspx');">
                                                                远洋外网招聘</a> <a style="width: 180px" href="#" onclick="return openStandardForm('http://localhost/MCSWebApp/SinoOcean.Seagull2.EmpProvider/EmpProvide/ManpowerRequirePlan/MakeManpowerRequirePlan/MakeManpowerRequirePlanController.ashx?processDescKey=MakeManpowerRequirePlan');">
                                                                    编制人力需求计划</a> <a style="width: 180px" href="#" onclick="return openStandardForm('http://localhost/MCSWebApp/SinoOcean.Seagull2.EmpProvider/EmpProvide/ManpowerRequirePlan/AdjustManpowerRequirePlan/AdjustManpowerRequirePlanController.ashx?processDescKey=AdjustManpowerRequirePlan');">
                                                                        调整人力需求计划</a>
                        </p>
                    </div>
                    <div class="clear">
                    </div>
                    <div class="tagContent" id="tagContentEmpEncourage" style="height: 400px">
                        <p>
                            <a href="../../SinoOcean.Seagull2.EmployeeEncourage/SalarySchema/SalarySchemaFixedRatioSetting/SalarySchemaFixedRatioEntry.aspx"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">固浮比设置</a>
                            <a href="../../SinoOcean.Seagull2.EmployeeEncourage/SalarySchema/SalarySchemaSpecializationFixedRatioSetting/SalarySchemaSpecializationFixedRatioEntry.aspx"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">岗位类别固浮比设置</a>
                            <a href="../../SinoOcean.Seagull2.EmployeeEncourage/SalarySchema/SalarySchemaCityFactorSetting/SalarySchemaCityFactorEntry.aspx"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">城市薪酬系数设置</a>
                            <a href="../../SinoOcean.Seagull2.EmployeeEncourage/SalarySchema/SalarySchemaSetting/SalarySchemaEntry.aspx"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">薪酬水平架构设置</a>
                            <a href="../../SinoOcean.Seagull2.EmployeeEncourage/SalarySchema/DefaultSalarySchemaSetting/DefaultSalarySchemaEntry.aspx"
                                onclick="onDraftLinkClick()" style="width: 160px" target="_blank">默认薪酬水平架构设置</a>
                            <br />
                            <a href="../../SinoOcean.Seagull2.EmployeeEncourage/SalaryPlan/SalaryBusinessFieldItem/Entry.aspx"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">薪酬结构项设置</a>
                            <a href="../../SinoOcean.Seagull2.EmployeeEncourage/SalaryPlan/SalaryTemplateItem/EntryAdd.aspx"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">添加薪酬模板</a>
                            <a href="../../SinoOcean.Seagull2.EmployeeEncourage/SalaryPlan/SalaryTemplateItem/EntryModify.aspx"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">修改薪酬模板</a>
                            <a href="../../SinoOcean.Seagull2.EmployeeEncourage/SalaryPlan/SalaryCheckUnitSetting/SalaryCheckUnitMainEntry.aspx"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">工资核发单元设置</a>
                            <a href="../../SinoOcean.Seagull2.EmployeeEncourage/SalaryPlan/SalaryEmployeeCyclistDatum/SalaryEmployeeCyclistDatumMainEntry.aspx"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">周期性数据输入</a>
                            <a href="../../SinoOcean.Seagull2.EmployeeEncourage/SalaryPlan/SalaryEmployeeCyclistDatumAdjust/SalaryEmployeeCyclistDatumAdjustMainEntry.aspx"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">周期性数据调整</a>
                            <a href="../../SinoOcean.Seagull2.EmployeeEncourage/SalaryPlan/SalaryPaySign/EmployeeSalaryPaySignController.ashx?processDescKey=EmpEncourage_SalaryPaySign"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">员工发薪签报</a>
                            <a href="../../SinoOcean.Seagull2.EmployeeEncourage/SalaryPlan/SalaryTaxGradeSetting/SalaryTaxGradeController.ashx?processDescKey=EmpEncourage_SalaryTaxRuleCfg"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">个税税档设置</a>
                            <a href="../../SinoOcean.Seagull2.EmployeeEncourage/SalaryPlan/SalaryEmpCfg/SalaryEmpCfgFrontPage.aspx"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">员工薪酬参数设置</a>
                            <a href="../../SinoOcean.Seagull2.EmployeeEncourage/SalaryPlan/SalaryEmpCfgInit/SalaryEmpCfgInitFrontPage.aspx"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">员工薪酬参数初始化</a>
                            <a href="../../SinoOcean.Seagull2.EmployeeEncourage/EmpCostPredict/Entry.aspx" onclick="onDraftLinkClick()"
                                style="width: 150px" target="_blank">人工成本预算</a> <a href="../../SinoOcean.Seagull2.EmployeeEncourage/SalaryAdjust/BroadBand/SalaryAdjustBroadBandEntry.aspx"
                                    onclick="onDraftLinkClick()" style="width: 150px" target="_blank">宽带制薪酬调整</a>
                            <a href="../../SinoOcean.Seagull2.EmployeeEncourage/SalaryAdjust/Grade/SalaryAdjustGradeEntry.aspx"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">档级制薪酬调整</a>
                            <a href="../../SinoOcean.Seagull2.EmployeeEncourage/SalaryAdjust/ImmediateAdjustSalary/Entry.aspx"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">即时调薪</a>
                            <br />
                            <a href="/MCSWebApp/SinoOcean.Seagull2.BonusDistribute/BonusDistribute/EmpAssessScoreImport/ScoreImportFront.aspx"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">考核结果导入</a>
                            <a href="/MCSWebApp/SinoOcean.Seagull2.BonusDistribute/BonusDistribute/BonusPerformanceFormula/RedictorController.ashx?processDescKey=BonusDistribute_BonusPerformanceFormula"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">设置奖金分配公式</a>
                            <a href="/MCSWebApp/SinoOcean.Seagull2.BonusDistribute/BonusDistribute/BonusPerformanceGroupDown/GroupDownFront.aspx"
                                onclick="onDraftLinkClick()" style="width: 170px" target="_blank">UBI绩效奖金集团下达</a>
                            <a href="/MCSWebApp/SinoOcean.Seagull2.BonusDistribute/BonusDistribute/BonusPerformanceORGDown/UBIPerformanceBonusIssueFront.aspx"
                                onclick="onDraftLinkClick()" style="width: 170px" target="_blank">UBI绩效奖金组织下达</a>
                            <br />
                            <a href="/MCSWebApp/SinoOcean.Seagull2.BonusDistribute/BonusDistribute/UBI_Performance_Distribute/UBIDistributeFront.aspx"
                                onclick="onDraftLinkClick()" style="width: 160px" target="_blank">UBI绩效奖金分配</a>
                            <a href="/MCSWebApp/SinoOcean.Seagull2.BonusDistribute/BonusDistribute/UBI_Special_Distribute/UBISpecialDistributeFront.aspx"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">UBI特殊奖金分配</a>
                            <a href="/MCSWebApp/SinoOcean.Seagull2.BonusDistribute/BonusDistribute/RBI_CBI_Performance_Distribute/RBIDistributeFront.aspx"
                                onclick="onDraftLinkClick()" style="width: 170px" target="_blank">RBI和CBI绩效奖金分配</a>
                            <a href="/MCSWebApp/SinoOcean.Seagull2.BonusDistribute/BonusDistribute/RBI_CBI_SpecialAndBasic_Distribute/RBIOtherDistributeFront.aspx"
                                onclick="onDraftLinkClick()" style="width: 170px" target="_blank">RBI和CBI其它奖金分配</a>
                            <br />
                            <a href="/MCSWebApp/SinoOcean.Seagull2.EmployeeEncourage/SalaryPlan/BonusDivide/YearlyBonusDivide/YearlyBonusDivideFront.aspx"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">年终奖分期发放</a>
                            <a href="/MCSWebApp/SinoOcean.Seagull2.EmployeeEncourage/SalaryPlan/BonusDivide/GenericBonusDivide/GenericBonusDivideView.aspx"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">其他奖金分期发放</a>
                            <a href="/MCSWebApp/SinoOcean.Seagull2.BonusDistribute/SalaryPlan/BonusDivide/BonusDivideAdjust/BonusDivideAdjustFront.aspx"
                                onclick="onDraftLinkClick()" style="width: 150px" target="_blank">奖金分期数据调整</a>
                        </p>
                    </div>
                    <div class="clear">
                    </div>
                    <div class="clear">
                    </div>
                    <div class="tagContent" id="tagContent19">
                        <p>
                            <a style="width: 150px" href="../../SinoOcean.Seagull2.ProcessManagement/FrontPage/ProcessComposition.aspx"
                                onclick="onDraftLinkClick()" target="_blank">流程编制</a> <a style="width: 150px" href="../../SinoOcean.Seagull2.ProcessManagement/FrontPage/ProcessComposition.aspx"
                                    onclick="onDraftLinkClick()" target="_blank">流程评估和发布</a> <a style="width: 150px"
                                        href="../../SinoOcean.Seagull2.ProcessManagement/FrontPage/ProcessComposition.aspx"
                                        onclick="onDraftLinkClick()" target="_blank">流程评估和优化</a>
                        </p>
                    </div>
                    <div class="clear">
                    </div>
                    <div class="tagContent" id="tagContent17">
                        <p>
                            <a style="width: 150px" href="../../SinoOcean.Seagull2.Qualification/Qualification/QualificationTemplateChange/QualificationTemplateChangeIndex.aspx?processDescKey=QualificationTemplateManage"
                                onclick="onDraftLinkClick()" target="_blank">模板变更</a> <a style="width: 150px" href="../../SinoOcean.Seagull2.Qualification/Qualification/QualificationAnnualInspection/QualificationAnnualInspectionIndex.aspx?processDescKey=QualificationAnnualInspection"
                                    onclick="onDraftLinkClick()" target="_blank">年检资质</a>
                        </p>
                    </div>
                    <div class="tagContent" id="tagContent20">
                        <p>
                            <a style="width: 150px" href="../../SinoOcean.Seagull2.Charging/InvoiceManager/PropertyDevelopment/ToRetrieveNotes.aspx"
                                onclick="onDraftLinkClick()" target="_blank">票据兑换</a><a style="width: 150px" href="../../SinoOcean.Seagull2.Charging/Charging/MortgageRepayment/MortgageRepaymentController.ashx?processDescKey=Charge_MortgageRepayment"
                                    onclick="onDraftLinkClick()" target="_blank">按揭款到账</a> <a style="width: 160px" runat="server"
                                        href="../../SinoOcean.Seagull2.Charging/Charging/MortgageRepayment/MortgageRepaymentController.ashx?processDescKey=Charge_MortgageRepayment"
                                        onclick="onDraftLinkClick()" target="_blank">物业服务业主收费记账</a> <a style="width: 170px"
                                            href="../../SinoOcean.Seagull2.Charging/Dashboard/ReporEntrance/PropertyDevelopment/SalesReceivedPaymentDailyReportQueryView.aspx"
                                            onclick="onDraftLinkClick()" target="_blank">销售回款日报</a> <a style="width: 200px" href="../../SinoOcean.Seagull2.Charging/Dashboard/ReporEntrance/PropertyDevelopment/SalesReceivedPaymentMonthlyAndQuarterlyReportQueryView.aspx"
                                                onclick="onDraftLinkClick()" target="_blank">销售回款月报季报年报</a>
                        </p>
                    </div>
                    <div class="tagContent" id="tagContent21">
                        <p>
                            <a style="width: 150px" href="../../SinoOcean.Seagull2.Equity/Equity/CompanyInfoCollect/CompanyInfoCollectController.ashx?processDescKey=CompanyInfoCollect"
                                onclick="onDraftLinkClick()" target="_blank">法人公司信息收集</a> <a style="width: 150px"
                                    href="../../SinoOcean.Seagull2.Equity/Equity/ChangeEquity/PreparationProgramControler.ashx?processDescKey=ChangeOfEquity"
                                    onclick="onDraftLinkClick()" target="_blank">股权变更</a> <a style="width: 150px" href="#"
                                        onclick="onDraftLinkClick()" target="_blank">股权支付计划变更</a> <a style="width: 130px"
                                            href="../../SinoOcean.Seagull2.Contract/ContractManagement/TemplateModify/TemplateCategoryModifyView.aspx?TemplateCategoryID=4"
                                            onclick="onDraftLinkClick()" target="_blank">股权范本管理</a>
                        </p>
                    </div>
                    <div class="tagContent" id="tagContent22">
                        <p>
                            <a style="width: 150px" href="../../SinoOcean.Seagull2.InternalAudit/InternalAudit/DepartureAuditTest/DepartureAuditTestController.ashx?processDescKey=DepartureAuditTest"
                                onclick="onDraftLinkClick()" target="_blank">离任审计</a> <a style="width: 150px" href="../../SinoOcean.Seagull2.InternalAudit/InternalAudit/AuditStartInternal/InternalControlAuditController.ashx?processDescKey=Internal_Control_Audit"
                                    onclick="onDraftLinkClick()" target="_blank">内控审计</a> <a style="width: 150px" href="../../SinoOcean.Seagull2.InternalAudit/InternalAudit/SimulationLaunchTest/LaunchAuditProjectBudgetController.ashx?processDescKey=AuditProject_LaunchTest"
                                        onclick="onDraftLinkClick()" target="_blank">预算审计</a> <a style="width: 150px" href="../../SinoOcean.Seagull2.InternalAudit/InternalAudit/SimulationLaunchTest/LaunchAuditProjectSettlementController.ashx?processDescKey=AuditProjectSettlement_LaunchTest"
                                            onclick="onDraftLinkClick()" target="_blank">结算审计</a> <a style="width: 150px" href="../../SinoOcean.Seagull2.InternalAudit/InternalAudit/AuditInfoManagement/AuditInfoManagementViewFront.aspx"
                                                onclick="onDraftLinkClick()" target="_blank">资料清单维护</a>
                        </p>
                    </div>
                    <div class="tagContent" id="tagContent23">
                        <p>
                            <a style="width: 150px" href="../../SinoOcean.Seagull2.ConsumerSatisfaction/Survey/TargetCategory.aspx"
                                onclick="onDraftLinkClick()" target="_blank">发起客户满意度调查</a>
                        </p>
                    </div>
                    <div class="tagContent" id="tagContent25">
                        <p>
                            <a style="width: 150px" href="#" runat="server" id="A28" onclick="return openSmallForm('../../SinoOcean.Seagull2.EmployeeAssess/GeneralScoringCriteria/GeneralScoringCriteriaApply/GeneralScoringCriteriaApplyController.ashx?processDescKey=EmpAssess_GeneralScoringCriteria')">
                                维护通用评分标准</a> <a style="width: 140px" href="#" runat="server" id="A29" onclick="return openSmallForm('../../SinoOcean.Seagull2.EmployeeAssess/ForceDistributionRules/ForceDistributionRulesApply/ForceDistributionRulesApplyController.ashx?processDescKey=EmpAssess_ForceDistributionRules')">
                                    维护强制结果分布</a><a style="width: 140px" href="#" runat="server" id="A30" onclick="return openSmallForm('../../SinoOcean.Seagull2.EmployeeAssess/AssessmentModel/AssessmentModelApply/AssessmentModelApplyController.ashx?processDescKey=EmpAssess_AssessmentModel')">
                                        设计考核模型</a><a style="width: 120px" href="#" runat="server" id="A33" onclick="return openSmallForm('../../SinoOcean.Seagull2.EmployeeAssess/AssessmentSchemes/SchemesSelection.aspx')">
                                            维护考核方案</a><a style="width: 110px" href="#" runat="server" id="A34" onclick="return openSmallForm('../../SinoOcean.Seagull2.EmployeeAssess/MakeAssessmentTable/PreMakeAssessmentTable.aspx')">
                                                制定考核表</a><a style="width: 120px" href="#" runat="server" id="A40" onclick="return openSmallForm('../../SinoOcean.Seagull2.EmployeeAssess/MakeAssessmentTable/PreAdjustAssessmentTable.aspx')">
                                                    调整考核指标</a><a style="width: 110px" href="#" runat="server" id="A39" onclick="return openSmallForm('../../SinoOcean.Seagull2.EmployeeAssess/AssessmentProcess/PreAssessmentProcess.aspx')">
                                                        考核评价</a><a style="width: 100px" href="#" runat="server" id="A35" onclick="return openSmallForm('../../SinoOcean.Seagull2.EmployeeAssess/AssessmentAppeal/PreAssessmentAppeal.aspx')">
                                                            考核申诉</a><a style="width: 90px" href="#" runat="server" id="A38" onclick="return openSmallForm('../../SinoOcean.Seagull2.EmployeeAssess/AssessmentInterview/PreAssessmentInterview.aspx')">
                                                                考核面谈</a>
                        </p>
                        <div class="clear">
                        </div>
                        <p>
                            <a style="width: 110px" href="#" runat="server" id="A41" onclick="return openSmallForm('../../SinoOcean.Seagull2.EmployeeAssess/AssessmentResult/PreValidationAssessmentResult.aspx')">
                                确认考核结果</a> <a style="width: 110px" href="#" runat="server" id="A42" onclick="return openSmallForm('../../SinoOcean.Seagull2.EmployeeAssess/AssessmentRelationAdjust/PreAssessmentInterview.aspx')">
                                    调整考核对象</a>
                        </p>
                    </div>
                </div>
            </dd>
            <dt style="border-bottom: solid 1px #ccc; width: 100%; margin-left: 2px;"></dt>
            <dt>主线流程</dt>
            <dd class="ddItem" style="width: 100%">
                <ul id="tags1">
                    <li><a class="unUse">战略定位</a></li>
                    <li><a class="unUse">战略规划</a></li>
                    <li><a onclick="selectTag('tagContent12',this)" href="#">绩效规划</a></li>
                    <li><a class="unUse">产品线研究</a></li>
                    <li><a onclick="selectTag('tagContent27',this)" href="javascript:void(0);">融资管理</a></li>
                    <li><a onclick="selectTag('tagContent8',this)" href="#">土地拓展</a></li>
                    <li><a onclick="selectTag('tagContent7',this)" href="#">项目定义</a></li>
                    <li><a class="unUse">项目实施</a></li>
                    <li><a onclick="selectTag('tagContent5',this)" href="#">项目实施</a></li>
                    <li><a onclick="selectTag('tagContent24',this)" href="#">现场销售</a></li>
                    <li><a onclick="selectTag('tagContent28',this)" href="#">签约</a></li>
                    <li><a class="unUse">入伙管理</a></li>
                    <li><a class="unUse">产权办理</a></li>
                    <li><a class="unUse">集中整改</a></li>
                    <li><a onclick="selectTag('tagContent4',this)" href="#">公司客服</a></li>
                    <li><a class="unUse">物业服务</a></li>
                    <li><a href="#" onclick="selectTag('tagContent9',this)">测试</a></li>
                </ul>
                <div id="tagContentMainline">
                    <div class="tagContent selectTag1" id="tagContent4">
                        <p>
                            <a style="width: 200px" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/ListeningWindowsFrontController.aspx?appName=CustomerService&programName=ListeningWindows"
                                onclick="onDraftLinkClick();">倾听之窗论坛客户意见处理</a> <a style="width: 150px" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/ComplainFrontController.aspx?appName=CustomerService&programName=Complain"
                                    onclick="onDraftLinkClick();">项目客户投诉处理</a> <a style="width: 200px" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/ComplainFrontController.aspx?appName=CustomerService&programName=ComplainSurveillance "
                                        onclick="onDraftLinkClick();">服务监督系统客户投诉处理</a>
                        </p>
                        <div class="clear">
                        </div>
                    </div>
                    <div class="tagContent selectTag1" id="tagContent5">
                        <p>
                            <a style="width: 120px" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/WorkContactsFrontController.aspx?appName=ProjectImplement&programName=WorkContacts"
                                onclick="onDraftLinkClick();">工作联系单</a>
                        </p>
                        <div class="clear">
                        </div>
                    </div>
                    <div class="tagContent selectTag1" id="tagContent7">
                        <p>
                            <a style="width: 120px" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/PropertyEvaluationFrontController.aspx?appName=PropertyObtain&programName=propertyevaluation"
                                onclick="onDraftLinkClick();">图纸会审</a> <a style="width: 120px" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/DrawingHandoutsFrontController.aspx?appName=ProductsDefine&programName=DrawingHandouts"
                                    onclick="onDraftLinkClick();">图纸发放</a>
                        </p>
                        <div class="clear">
                        </div>
                    </div>
                    <div class="tagContent selectTag1" id="tagContent8">
                        <p>
                            <a style="width: 120px" href="../../SinoOcean.OA.BizProcesses/SinoOceanForms/PropertyEvaluationFrontController.aspx?appName=PropertyObtain&programName=PropertyEvaluation"
                                onclick="onDraftLinkClick();">土地拓展</a>
                        </p>
                        <div class="clear">
                        </div>
                    </div>
                    <div class="tagContent" id="tagContent9">
                        <p>
                            <a style="width: 160px" href="#" onclick="return openStandardForm('../../WebTestProject/WFDemo/TempProcessController.ashx?relativeParams=ProjectName%3d临时流程-2011');">
                                临时流程演示</a> <a style="width: 160px" href="#" onclick="return openStandardForm('../../WebTestProject/WorkItemDemo/NewForms/PlanDesign.ashx?processDescKey=PlanDesign&appName=演示流程&programName=制定计划&relativeParams=ProjectName%3d临时流程-2011');">
                                    计划流程</a> <a style="width: 180px" href="#" onclick="return openStandardForm('../../WebTestProject/WFDemo/DemoOrderController.ashx?processDescKey=WfDemoOrderProcess&appName=演示流程&programName=合同流程&relativeParams=ProjectName%3d常规流程-2011');">
                                        常规流程-合同流程演示</a> <a style="width: 180px" href="#" onclick="return openStandardForm('../../WebTestProject/WFDemo/DemoPaymentController.ashx?processDescKey=WfDemoPamentProcess&appName=演示流程&programName=支付流程&relativeParams=ProjectName%3d常规流程-2011');">
                                            常规流程-支付流程演示</a> <a style="width: 180px" href="#" onclick="return openStandardForm('../../WebTestProject/WorkItemDemo/NewForms/PlanDesign.ashx?processDescKey=MakePlanProcess&appName=演示流程&programName=制定计划&relativeParams=ProjectName%3d临时流程-2011');">
                                                计划流程-制定计划</a>
                        </p>
                    </div>
                    <div class="tagContent" id="tagContent24">
                        <p>
                            <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale/Sale/SaleTeam/SaleTeamEditAndApprovalController.ashx?processDescKey=sale_SaleTeamEdit"
                                onclick="onDraftLinkClick();" style="width: 150px">维护置业顾问团队</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale/Sale/PriceManagement/PrePriceManagement.aspx"
                                    onclick="onDraftLinkClick();" style="width: 100px">价格管理</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale/Sale/RentalSaleResource/RentalSaleResourceLock/RentalSaleResourcePrepose.aspx"
                                        onclick="onDraftLinkClick();" style="width: 150px">锁定或解锁房源</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale/Sale/CustomerAdmit/CustomerAdmitInfoPrepose.aspx"
                                            onclick="onDraftLinkClick();" style="width: 150px">录入客户信息</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale/Sale/InformationPort/RecognitionChips/RecognitionChips/RecognitionPrepose.aspx"
                                                onclick="onDraftLinkClick();" style="width: 100px">认筹</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale/Sale/InformationPort/RecognitionChips/ReturnRecognitionChips/ReturnRecognitionChipsPrepose.aspx"
                                                    onclick="onDraftLinkClick();" style="width: 100px">退认筹</a>
                            <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale/Sale/InformationPort/InformationPortPrepose.aspx"
                                onclick="onDraftLinkClick();" style="width: 100px">房屋销售</a>
                            <%--<a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale/Sale/CustomerAllocation/CustomerAllocationController.ashx?processDescKey=sale_CustomerAssignation" onclick="onDraftLinkClick();" style="width: 100px">分配客户</a>--%>
                            <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale/Sale/CustomerAllocation/FontPage.aspx"
                                onclick="onDraftLinkClick();">分配客户</a>
                        </p>
                        <p>
                            <br />
                            <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/MarketingProgram/FrontPage.aspx"
                                style="width: 150px" target="_blank">编制项目营销大纲</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/SalePromotionPlan/PromotionPlan.aspx"
                                    style="width: 120px" target="_blank">编制推广计划</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/OpeningPreparation/OpeningPreparation.aspx"
                                        style="width: 150px" target="_blank">编制开盘筹备报告</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/PriceDetermine/FrontPage.aspx"
                                            style="width: 150px" target="_blank">编制定价及实施报告</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/ContractTemplate/SalesContract/FrontPage.aspx "
                                                style="width: 120px" target="_blank">销售合同模板设置</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/ContractTemplate/DecorationContract/FrontPage.aspx "
                                                    style="width: 120px" target="_blank">精装修协议模板设置</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/ContractTemplate/SubscriptionAgreement/FrontPage.aspx "
                                                        style="width: 120px" target="_blank">认购书模板设置</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/ContractTemplate/RecognitionChipsContract/FrontPage.aspx "
                                                            style="width: 120px" target="_blank">认筹协议模板设置</a>
                            <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/SaleCertificate/SaleCertificateFrontPage.aspx"
                                style="width: 120px" target="_blank">预售证办理</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/MortgageBank/FrontPage.aspx"
                                    style="width: 120px" target="_blank">按揭银行</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/Packaging/FrontPage.aspx"
                                        style="width: 120px" target="_blank">示范区包装方案</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/SaleAnnouncement/FrontPage.aspx"
                                            style="width: 120px" target="_blank">销售案场公示</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/SaleAreaPredict/SaleAreaPredictFrontPage.aspx"
                                                style="width: 180px" target="_blank">销售面积预测及备案</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/PrepartorySalesProps/PrepartorySalesPropsDefault.aspx "
                                                    style="width: 180px" target="_blank">筹备销售道具工作指引</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/MarketingTraining/FrontPage.aspx "
                                                        style="width: 180px" target="_blank">组织销前集中培训工作指引</a>
                            <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/MarketingTraining/SingleFrontPage.aspx "
                                style="width: 180px" target="_blank">组织销前单独培训工作指引</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/OpenRiskCheck/FrontPage.aspx"
                                    style="width: 150px" target="_blank">开盘风险检查</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/BeforeShowRiskCheck/FrontPage.aspx"
                                        style="width: 150px" target="_blank">开放前风险检查</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/SendLetter/SignSummon/FrontPage.aspx"
                                            style="width: 120px" target="_blank">签约催告函</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/SendLetter/SubscribeCancel/FrontPage.aspx"
                                                style="width: 120px" target="_blank">认购书解除</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/SendLetter/OverduePay/FrontPage.aspx"
                                                    style="width: 120px" target="_blank">逾期付款通知书</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/SendLetter/TerminateContract/FrontPage.aspx"
                                                        style="width: 120px" target="_blank">解约通知书</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/PropertyContract/FrontPage.aspx"
                                                            style="width: 120px" target="_blank">前期物业合同</a>
                            <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/PropertyServiceContract/Default.aspx"
                                style="width: 200px" target="_blank">销售示范区物业服务</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/MarketingComplete/FrontPage.aspx"
                                    style="width: 150px" target="_blank">结案报告</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/CommissionCheck/FrontPage.aspx"
                                        style="width: 150px" target="_blank">月度佣金核算</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/Questionnaire/FrontPage.aspx"
                                            style="width: 150px" target="_blank">客户问卷调查</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale2/Sale2/CommissionCheck/FrontPageOutSide.aspx"
                                                style="width: 150px" target="_blank">佣金结算外网</a>
                        </p>
                    </div>
                    <div class="tagContent" id="tagContent28">
                        <p>
                            <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale/Sale/SurplusMoneyPayment/SurplusMoneyPaymentPrepose.aspx"
                                onclick="onDraftLinkClick();">缴纳余款</a> <a href="http://localhost/MCSWebApp/SinoOcean.Seagull2.FieldSale/Sale/ContractChange/ContractChangePrepose.aspx"
                                    onclick="onDraftLinkClick();">签约变更</a>
                        </p>
                    </div>
                    <div class="tagContent" id="tagContent27">
                        <p>
                            <a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Financing/Portal/CorporationPortal.aspx?title=合同签订前置页面&roleKey=Financing:JSJZ_FRGSRZZY&processDescKey=Financing_ContractSign&redirectUrl=/McsWebApp/SinoOcean.Seagull2.Financing/Financing/ContractSigning/ContractSigningViewController.ashx');"
                                href="javascript:void(0)">融资合同签订</a> <a style="width: 250px" onclick="return openStandardForm('/MCSWebApp/SinoOcean.Seagull2.Financing/Portal/ContractSearch.aspx?title=融资提款前置页面&roleKey=Financing:JSJZ_FRGSRZZY&processDescKey=Financing_Drawdown&redirectUrl=/McsWebApp/SinoOcean.Seagull2.Financing/Financing/Drawdown/DrawdownController.ashx');"
                                    href="javascript:void(0)">融资提款</a><a style="width: 250px" onclick="return openStandardForm('/MCSWebApp/SinoOcean.Seagull2.Financing/Portal/ContractSearch.aspx?title=融资提款(境外)前置页面&roleKey=Financing:JSJZ_FRGSRZZY&processDescKey=Financing_DrawdownOutside&redirectUrl=/McsWebApp/SinoOcean.Seagull2.Financing/Financing/DrawdownOutside/DrawdownOutsideController.ashx');"
                                        href="javascript:void(0)">融资提款(境外)</a> <a style="width: 250px" onclick="return openStandardForm('/MCSWebApp/SinoOcean.Seagull2.Financing/Portal/ContractSearch.aspx?title=融资还款及息费支出前置页面&roleKey=Financing:JSJZ_FRGSRZZY&processDescKey=Financing_Payment&redirectUrl=/McsWebApp/SinoOcean.Seagull2.Financing/Financing/Payment/AffirmPaymentController.ashx');"
                                            href="javascript:void(0)">融资还款及息费支出</a>
                        </p>
                        <p>
                            <a style="width: 250px" onclick="return openStandardForm('/MCSWebApp/SinoOcean.Seagull2.Financing/Portal/ContractSearch.aspx?title=融资还款及息费支出(境外)前置页面&roleKey=Financing:JSJZ_FRGSRZZY&processDescKey=Financing_PaymentOutside&redirectUrl=/McsWebApp/SinoOcean.Seagull2.Financing/Financing/PaymentOutside/PaymentOutsideController.ashx');"
                                href="javascript:void(0)">融资还款及息费支出(境外)</a> <a style="width: 250px" onclick="return openSmallForm('/MCSWebApp/SinoOcean.Seagull2.Financing/Portal/ContractFamilySelector.aspx?title=合同变更前置页面&roleKey=Financing:JSJZ_FRGSRZZY&processDescKey=Financing_ContractChanging&redirectUrl=/McsWebApp/SinoOcean.Seagull2.Financing/Financing/ContractChange/ContractChangeViewController.ashx');"
                                    href="javascript:void(0)">融资合同变更</a> <a style="width: 250px" onclick="return openStandardForm('/MCSWebApp/SinoOcean.Seagull2.Financing/Portal/ContractSearch.aspx?title=合同计划变更前置页面&roleKey=Financing:JSJZ_FRGSRZZY&processDescKey=Financing_ExecutePlanChanges_FillIn&redirectUrl=/McsWebApp/SinoOcean.Seagull2.Financing/Financing/ExecutePlanChanges/ExecutePlanChangesController.ashx');"
                                        href="javascript:void(0)">融资合同执行计划变更</a> <a style="width: 250px" onclick="return openStandardForm('/MCSWebApp/SinoOcean.Seagull2.Financing/Portal/ContractSearch.aspx');"
                                            href="javascript:void(0)">融资合同关闭</a>
                        </p>
                        <div class="clear">
                        </div>
                    </div>
                    <div class="tagContent" id="tagContent12">
                        <p>
                            <a style="width: 160px" href="#" id="A31" onclick="return openSmallForm('../../SinoOcean.Seagull2.PerformancePlan/PerformanceIndicatorMaintain/PerformanceIndicatorMaintainPreposePage.aspx')">
                                维护绩效指标库</a><a style="width: 150px" href="#" id="A32" onclick="return openSmallForm('../../SinoOcean.Seagull2.PerformancePlan/ScoringStandardMaintain/ScoringStandardMaintainController.ashx?processDescKey=ScoringStandardMaintain')">
                                    维护评分标准</a> <a style="width: 190px" href="#" id="A37" onclick="return openSmallForm('../../SinoOcean.Seagull2.PerformancePlan/BuildYearPerformanceIndicatorSystem/StartProcessPage.aspx')">
                                        构建绩效指标体系</a> <a style="width: 170px" href="#" id="A36" onclick="return openSmallForm('../../SinoOcean.Seagull2.PerformancePlan/PerformanceIndicatorAdjust/PerformanceIndicatorAdjustPreposePage.aspx')">
                                            绩效指标调整</a>
                            <%--<a style="width: 160px" href="#" onclick="return openStandardForm('../../WebTestProject/WorkItemDemo/NewForms/PlanDesign.ashx?processDescKey=MakePlanProcess&appName=演示流程&programName=制定计划&relativeParams=ProjectName%3d临时流程-2011');">
                                                制定计划</a>--%>
                        </p>
                        <div class="clear">
                        </div>
                    </div>
                </div>
                <p>
                </p>
                <p>
                </p>
                <p>
                </p>
                <p>
                </p>
            </dd>
        </div>
        <div class="clear">
        </div>
    </div>
</body>
</html>

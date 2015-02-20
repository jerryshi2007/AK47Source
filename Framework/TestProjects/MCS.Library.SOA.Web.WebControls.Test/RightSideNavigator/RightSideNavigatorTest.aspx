<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RightSideNavigatorTest.aspx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.RightSideNavigator.RightSideNavigatorTest" %>

<%@ Register assembly="MCS.Library.SOA.Web.WebControls" namespace="MCS.Web.WebControls" tagprefix="soa" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    	<soa:RightSideNavigator ID="RightSideNavigator1" runat="server">
    		<RelativeLinkCategories>
    			<soa:RelativeLinkCategory Title="标准规范">
    				<Links>
    					<soa:RelativeLinkItem Title="标准规范" CategoryName="StantardCate" />
    				</Links>
    			</soa:RelativeLinkCategory>
				<soa:RelativeLinkCategory Title="知识案例">
    				<Links>
    					<soa:RelativeLinkItem Title="内部知识" CategoryName="StantardCate" MoreCategoryName="MoreCate"/>
						<soa:RelativeLinkItem Title="外部知识" CategoryName="StantardCate" MoreCategoryName="MoreCate"/>
    				</Links>
    			</soa:RelativeLinkCategory>
				<soa:RelativeLinkCategory Title="法律法规">
    				<Links>
    					<soa:RelativeLinkItem Title="法律法规" CategoryName="StantardCate" MoreCategoryName="MoreCate"/>
    				</Links>
    			</soa:RelativeLinkCategory>
				<soa:RelativeLinkCategory Title="什么">
    				<Links>
    					<soa:RelativeLinkItem Title="法律法规" CategoryName="StantardCate" MoreCategoryName="MoreCate"/>
    				</Links>
    			</soa:RelativeLinkCategory>
    		</RelativeLinkCategories>
		</soa:RightSideNavigator>
    
    </div>
    </form>
</body>
</html>

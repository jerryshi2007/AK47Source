<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebUserControl1.ascx.cs" Inherits="MCS.Library.SOA.Web.WebControls.Test.DeluxeSearch.WebUserControl1" %>
<%@ Register assembly="MCS.Library.SOA.Web.WebControls" namespace="MCS.Web.WebControls" tagprefix="cc1" %>

    <cc1:DeluxeSearch ID="DeluxeSearch1" runat="server"  CustomSearchContainerControlID="searchContainer"
 
     CssClass="deluxe-search"   HasCategory="True" HasAdvanced="True" SearchField="SupplierName"  SearchMethod="SurffixLike">

         </cc1:DeluxeSearch>  
         
         
            <div id="searchContainer" runat="server" style="display:none;">
     aaaaaaaaaaaaaaaa</div>
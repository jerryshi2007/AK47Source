@echo off
set mstest="%vs120comntools%..\ide\mstest.exe"
set testDir="%MCS2013ConfigDir%\..\Framework\TestProjects" 

%mstest% /testcontainer:"%testDir%\MCS.Library.Test\bin\Debug\MCS.Library.Test.dll"
%mstest% /testcontainer:"%testDir%\MCS.Library.Data.Test\bin\Debug\MCS.Library.Data.Test.dll"
%mstest% /testcontainer:"%testDir%\MCS.Library.HtmlParser.Test\bin\Debug\MCS.Library.HtmlParser.Test.dll"
%mstest% /testcontainer:"%testDir%\MCS.Web.Library.Script.Json.Test\bin\Debug\MCS.Web.Library.Script.Json.Test.dll"

%mstest% /testcontainer:"%testDir%\MCS.Library.SOA.DataObjects.Tenant.Test\bin\Debug\MCS.Library.SOA.DataObjects.Tenant.Test.dll" /testsettings:"%testDir%\MCS.Library.SOA.DataObjects.Tenant.Test\DataObjects.Tenant.testsettings"

%mstest% /testcontainer:"%testDir%\MCS.Library.WF.Contracts.Converters.Test\bin\Debug\MCS.Library.WF.Contracts.Converters.Test.dll" /testsettings:"%testDir%\MCS.Library.WF.Contracts.Converters.Test\Converters.TestSettings.testsettings"

%mstest% /testcontainer:"%testDir%\WfOperationServices.Test\bin\Debug\WfOperationServices.Test.dll"

pause
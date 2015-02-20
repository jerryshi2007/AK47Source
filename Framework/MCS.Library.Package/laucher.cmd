
xcopy ..\..\bin\MCS.Library.WF.Contracts.??? lib\net451 /s /i /y
xcopy ..\..\bin\MCS.Library.WF.Contracts.Json.??? lib\net451 /s /i /y
xcopy ..\..\bin\MCS.Library.WF.Contracts.Proxies.??? lib\net451 /s /i /y
xcopy ..\..\bin\MCS.Library.??? lib\net451 /s /i /y
xcopy ..\..\bin\MCS.Library.Services.Contracts.??? lib\net451 /s /i /y
xcopy ..\..\bin\MCS.Library.WcfExtensions.??? lib\net451 /s /i /y
xcopy ..\..\bin\MCS.Web.Library.Script.Json.??? lib\net451 /s /i /y
xcopy ..\..\bin\MCS.Library.Office.OpenXml.*  lib\net451 /s /i /y
xcopy ..\..\bin\MCS.Library.OGUPermission.*  lib\net451 /s /i /y
xcopy ..\..\bin\MCS.Web.Library.??? lib\net451 /s /i /y      
xcopy ..\..\bin\MCS.Web.WebControls.??? lib\net451 /s /i /y
xcopy ..\..\bin\MCS.Library.SOA.Web.WebControls.??? lib\net451 /s /i /y
xcopy ..\..\bin\MCS.Library.ADHelper.??? lib\net451 /s /i /y
xcopy ..\..\bin\MCS.Library.Data.??? lib\net451 /s /i /y
xcopy ..\..\bin\MCS.Library.Passport.??? lib\net451 /s /i /y
xcopy ..\..\bin\MCS.Web.Responseive.Library.??? lib\net451 /s /i /y

set sver=%time%
set "sver=%sver::=%"
set "sver=%sver:.=%"

set bver=%date%
set "bver=%bver::=%"
set "bver=%bver:/=%"
set "bver=%bver:.=%"
set "bver=%bver:~0,8%"


set filesDir=%~dp0


set config=%1
set package=%2

set publishTarget=\\172.16.8.83\Packages\%config%

IF /I "%config%"=="NugetDebugPublish" set publishTarget=\\172.16.8.83\Packages\

set ver=0.0.0.0
IF /I "%config%"=="NugetDebugPublish" set publishTarget=\\172.16.8.83\Packages\
IF /I "%config%"=="NugetDebugPublish" set ver=0.1.%bver%.%sver%
IF /I "%config%"=="NugetReleasePublish" set ver=0.1.%bver%.%sver%

set "ver=%ver: =%"

echo %config%   ,  %ver%
copy  %package%.template.nuspec %package%.nuspec /y
echo   publishTarget=%publishTarget% 
BuildPublishPackage.cmd %package% %ver% %publishTarget%       "%filesDir%\..\..\..\CIIC.HSR.TSP.Files\NugetPackagesOutput\%config%"

                            



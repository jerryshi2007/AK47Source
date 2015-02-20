SET WorkDir=C:\Windows\Microsoft.NET\Framework\v4.0.30319
SET WorkDir64=C:\Windows\Microsoft.NET\Framework64\v4.0.30319
SET OutputDir=%2
SET SourceDir=%1
SET PublishTarget=%3
echo Publishing site %SourceDir% to %OutputDir%
del /S /Q %OutputDir%\*.*
rmdir /S /Q %OutputDir%\
%WorkDir%\msbuild /target:Build;_WPPCopyWebApplication /p:Configuration=%4;OutDir=%OutputDir% %SourceDir%\MCSWebApp\WorkflowDesigner\WorkflowDesigner.csproj
%WorkDir64%\msbuild /target:Build;_WPPCopyWebApplication /p:Configuration=%4;OutDir=%OutputDir% %SourceDir%\MCSWebApp\OACommonPages\OACommonPages.csproj
%WorkDir64%\msbuild /target:Build;_WPPCopyWebApplication /p:Configuration=%4;OutDir=%OutputDir% %SourceDir%\MCSWebApp\WfOperationServices\WfOperationServices.csproj
%WorkDir64%\msbuild /target:Build;_WPPCopyWebApplication /p:Configuration=%4;OutDir=%OutputDir% %SourceDir%\MCSWebApp\ResponsivePassportService\ResponsivePassportService.csproj
%WorkDir64%\msbuild /target:Build;_WPPCopyWebApplication /p:Configuration=%4;OutDir=%OutputDir% %SourceDir%\MCSWebApp\PassportService\PassportService.csproj
echo Site published successfully
echo Coping css js config etc.
xcopy %SourceDir%\Config_SSP\*.* %OutputDir%\_PublishedWebsites\Config_%PublishTarget% /y /s /i
xcopy %SourceDir%\Bin\*.* %OutputDir%\_PublishedWebsites\Bin /y /s /i
xcopy %SourceDir%\css\*.* %OutputDir%\_PublishedWebsites\css /y /s /i
xcopy %SourceDir%\frames\*.* %OutputDir%\_PublishedWebsites\frames /y /s /i
xcopy %SourceDir%\HBWebHelperControl\*.* %OutputDir%\_PublishedWebsites\HBWebHelperControl /y /s /i
xcopy %SourceDir%\hta\*.* %OutputDir%\_PublishedWebsites\hta /y /s /i
xcopy %SourceDir%\images\*.* %OutputDir%\_PublishedWebsites\images /y /s /i
xcopy %SourceDir%\img\*.* %OutputDir%\_PublishedWebsites\img /y /s /i
xcopy %SourceDir%\JavaScript\*.* %OutputDir%\_PublishedWebsites\JavaScript /y /s /i
xcopy %SourceDir%\xap\*.* %OutputDir%\_PublishedWebsites\xap /y /s /i
echo File copied successfully
echo %4
Pause
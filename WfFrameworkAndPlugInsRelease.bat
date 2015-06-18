"%windir%"\microsoft.net\framework\v4.0.30319\msbuild.exe %~dp0WfFrameworkAndPlugIns.csproj /p:Configuration=Release
xcopy .\Bin\CIIC.HSR.TSP.WF.ApproverPlugin.* .\MCSWebApp\WfOperationServices\bin\ /Y /D /R
xcopy .\Bin\*.xap .\MCSWebApp\Xap\ /Y /D /R
pause 
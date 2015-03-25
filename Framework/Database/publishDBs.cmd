@echo off
cd PermissionsCenterDB

"%windir%"\microsoft.net\framework\v4.0.30319\msbuild.exe /t:Publish /p:SqlPublishProfilePath=PermissionsCenterDB.publish.xml
"%windir%"\microsoft.net\framework\v4.0.30319\msbuild.exe /t:Publish /p:SqlPublishProfilePath=PermissionsCenterDB_Local.publish.xml

cd..

cd MCSWorkflowDB

"%windir%"\microsoft.net\framework\v4.0.30319\msbuild.exe /t:Publish /p:SqlPublishProfilePath=MCSWorkflowDB.local.test.publish.xml
"%windir%"\microsoft.net\framework\v4.0.30319\msbuild.exe /t:Publish /p:SqlPublishProfilePath=MCSWorkflowDB.archive.publish.xml
"%windir%"\microsoft.net\framework\v4.0.30319\msbuild.exe /t:Publish /p:SqlPublishProfilePath=MCSWorkflowDB.local.simulation.publish.xml
"%windir%"\microsoft.net\framework\v4.0.30319\msbuild.exe /t:Publish /p:SqlPublishProfilePath=MCSWorkflowDB.local.publish.xml

cd..

cd HB2008ParamsDB

"%windir%"\microsoft.net\framework\v4.0.30319\msbuild.exe /t:Publish /p:SqlPublishProfilePath=HB2008ParamsDB.local.test.publish.xml
"%windir%"\microsoft.net\framework\v4.0.30319\msbuild.exe /t:Publish /p:SqlPublishProfilePath=HB2008ParamsDB.local.publish.xml

cd..

pause


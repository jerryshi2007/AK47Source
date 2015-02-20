set ver=0.0.0.1
set config=%1
set package=PackageTest.Package

set publishTarget=\\172.16.8.83\Packages
IF /I "%1"=="Debug" set publishTarget=\\172.16.8.83\Packages\%1
echo %publishTarget%
BuildPublishPackage.cmd %package% %ver% %publishTarget% 

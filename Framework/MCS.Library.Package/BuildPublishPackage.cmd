@echo off
set exe=nuget.exe
:: Make sure the nupkg files are writeable and create backup
IF EXIST *.nupkg (
	echo.
	echo Creating backup...
	forfiles /m *.nupkg /c "cmd /c attrib -R @File"
	forfiles /m *.nupkg /c "cmd /c move /Y @File @File.bak"
)

echo.
echo Updating NuGet...
rem cmd /c %exe% update -Self

echo.
echo Creating package...
%exe% pack %1.nuspec  -Version %2

xcopy %1.%2.nupkg %3 /s /i /y
 copy    %1.%2.nupkg  %4   /y

:eof
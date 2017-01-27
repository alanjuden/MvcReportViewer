@echo off
echo Preparing to build the NuGet Package...
C:\NuGet\NuGet.exe pack %cd%\AlanJuden.MvcReportViewer.NetCore.nuspec
pause
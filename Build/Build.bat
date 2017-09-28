@echo off
echo.
set version=%1
set path=C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\;%path%

mkdir logs

set buildconfig=Release
set dnnversion=DNN7
Msbuild.exe ModuleSpecific.targets /p:VisualStudioVersion=14.0;DNNVersion=%dnnversion%;Version=%version%;Configuration=%buildconfig%;TargetFrameworkVersion=v4.0 /t:Install /l:FileLogger,Microsoft.Build.Engine;logfile=logs\Build_%buildconfig%_%dnnversion%.log;verbosity=diagnostic
if ERRORLEVEL 1 goto end

:end
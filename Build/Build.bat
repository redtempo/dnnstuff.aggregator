@echo off
echo.
set version=%1
set buildconfig=%2
set path=C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\;%path%

set dnnversion=DNN8
Msbuild.exe ModuleSpecific.targets /p:DNNVersion=%dnnversion%;Version=%version%;Configuration=%buildconfig%;TargetFrameworkVersion=v4.5;OutputPath="./Build/Output/%dnnversion%" /t:Install /l:FileLogger,Microsoft.Build.Engine;logfile=Logs\Build_%buildconfig%_%dnnversion%.log;verbosity=diagnostic
if ERRORLEVEL 1 goto end

set dnnversion=DNN7
Msbuild.exe ModuleSpecific.targets /p:DNNVersion=%dnnversion%;Version=%version%;Configuration=%buildconfig%;TargetFrameworkVersion=v4.0;OutputPath="./Build/Output/%dnnversion%" /t:Install /l:FileLogger,Microsoft.Build.Engine;logfile=Logs\Build_%buildconfig%_%dnnversion%.log;verbosity=diagnostic

:end
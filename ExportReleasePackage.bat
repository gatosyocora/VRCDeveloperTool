@echo off
set UNITY_EXE="C:\Program Files\Unity\Hub\Editor\2018.4.20f1\Editor\Unity.exe"
set EXPORT_PACKAGES=Assets\VRCDeveloperTool
set LOG_FILE=outputlog.txt
for /f %%i in ('cd') do set CURRENT_DICT=%%i
for /f %%i in ('date /t') do set PACKAGE_NAME=VRCDeveloperTool_%%i.unitypackage
set PACKAGE_NAME=%PACKAGE_NAME:/=%

%UNITY_EXE% -exportPackage %EXPORT_PACKAGES% %PACKAGE_NAME% -projectPath %CURRENT_DICT% -batchmode -nograhics -logfile %LOG_FILE% -quit

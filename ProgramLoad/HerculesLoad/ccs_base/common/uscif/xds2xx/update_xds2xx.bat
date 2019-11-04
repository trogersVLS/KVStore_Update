@echo off

if "%1" == "xds200" (
    goto XDS200
) else if "%1" == "xds220" (
    goto XDS220
) else  (
    echo ERROR: Unknown option "%1".
    echo Valid options are xds200 or xds220.
    echo Do not run this script on an XDS220 ISO.
    goto END
)

:XDS200
set OLDDIR=%CD%
cd %~dp0
if exist xds200_firmware_v1008.bin (
    echo .
    echo Updating Firmware ...
    xds2xx_conf update xds2xxu 0 xds200_firmware_v1008.bin
    echo .
    echo Rebooting, please wait ...
    xds2xx_conf boot xds2xxu 0
)

if exist xds2xx_cpld_v1008.xsvf (
    echo .
    echo Updating CPLD ...
    xds2xx_conf program xds2xxu 0 xds2xx_cpld_v1008.xsvf
)

if exist xds200_firmware_v1008.bin (
    echo .
    echo Reading Configuration ...
    echo .
    echo Check swRev is 1.0.0.8 or higher.
    echo .
    xds2xx_conf get xds2xxu 0
    echo .
)
chdir /d %OLDDIR%
goto END

:XDS220
set OLDDIR=%CD%
cd %~dp0
if exist xds220_firmware_v1008.bin (
    echo .
    echo Updating Firmware ...
    xds2xx_conf update xds2xxu 0 xds220_firmware_v1008.bin
    echo .
    echo Rebooting, please wait ...
    xds2xx_conf boot xds2xxu 0
)

if exist xds2xx_cpld_v1008.xsvf (
    echo .
    echo Updating CPLD ...
    xds2xx_conf program xds2xxu 0 xds2xx_cpld_v1008.xsvf
)

if exist xds220_firmware_v1008.bin (
    echo .
    echo Reading Configuration ...
    echo .
    echo Check swRev is 1.0.0.8 or higher.
    echo .
    xds2xx_conf get xds2xxu 0
    echo .
)
chdir /d %OLDDIR%
goto END

:END
pause

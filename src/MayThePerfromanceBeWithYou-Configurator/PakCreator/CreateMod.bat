@echo off
color 0e
@setlocal enableextensions

@pushd %~dp0

@echo -=Creating .Pak File=-
@echo *Path warnings can be ignored*
@echo "pakchunk99-Mods_MayThePerformanceBeWithYou_P\*.*" "..\..\..\*.*" >filelist.txt
.\UnrealPak.exe "pakchunk99-Mods_MayThePerformanceBeWithYou_P.pak" -create=filelist.txt -compress

@popd
@echo. 
@echo. 
@echo pakchunk99-Mods_CustomMod_P.pak has now been created
@echo You may now move it to your Fallen Order\SwGame\Content\Paks\ folder
@echo. 
:skip
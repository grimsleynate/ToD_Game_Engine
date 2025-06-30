@echo off
setlocal

pushd "%~dp0"

:: 1) Which glslangValidator?
echo === WHICH & VERSION ===
where glslangValidator
glslangValidator --version
echo.

:: 2) Show PATH & cwd
echo === ENV & CWD ===
echo PATH=%PATH%
echo Current Dir: %cd%
echo.

:: 3) Compute absolute paths
set ScriptDir=%~dp0
set HLSL=%ScriptDir%MyShader.hlsl
set VSOUT=%ScriptDir%MyShader.vert.spv
set FSOUT=%ScriptDir%MyShader.frag.spv

echo === PATHS ===
echo ScriptDir:  %ScriptDir%
echo HLSL File:  %HLSL%
echo VS Output:  %VSOUT%
echo FS Output:  %FSOUT%
echo.

if not exist "%HLSL%" (
  echo ERROR: HLSL not found at %HLSL%
  pause
  exit /B 1
)

:: 4) Compile with absolute paths
echo === COMPILING VS ===
glslangValidator -V -x hlsl -S vert -e VSMain -o "%VSOUT%" "%HLSL%"
if errorlevel 1 (
  echo FAILED VS compile  
  pause
  exit /B 1
)

echo === COMPILING PS ===
glslangValidator -V -x hlsl -S frag -e PSMain -o "%FSOUT%" "%HLSL%"
if errorlevel 1 (
  echo FAILED PS compile
  pause
  exit /B 1
)

echo Shaders compiled successfully.
pause

popd
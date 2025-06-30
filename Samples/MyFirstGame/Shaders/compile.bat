@echo off
REM 1) Jump into this script's folder
pushd "%~dp0"

REM 2) Show wher we are and what files exist
echo Current Dir: %cd%
dir /b

REM 3) Try compiling the vertex stage by relative name
echo.
echo --- Compiling VSMain from MyShader.hlsl ---
glslangValidator -V -x hlsl -S vert -e VSMain -o MyShader.vert.spv MyShader.hlsl -H
echo Exit code: %ERRORLEVEL%
if %ERRORLEVEL% NEQ 0 pause & exit /b %ERRORLEVEL%

echo.
echo === Compiling PSMain from MyShader.hlsl
glslangValidator -V -x hlsl -S frag -e PSMain -o MyShader.frag.spv MyShader.hlsl -H
echo %ERRORLEVEL% NEQ 0 pause & exit /b %ERRORLEVEL%

echo.
echo Both shaders compiled!
popd
pause
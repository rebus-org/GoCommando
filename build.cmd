@echo off

echo GoCommand build thing.
echo.

set buildVersion=%1

set outputPath="%~dp0\deploy"

if exist "%outputPath%" (
	rd "%outputPath%" /s/q
)

mkdir "%outputPath%"

if "%buildVersion%"=="" (
	echo.
	echo Please specify which version to build as an argument
	echo.
	goto exit
)

echo Building version %buildVersion%...

msbuild GoCommando\GoCommando.csproj -P:Configuration=Release

echo Tagging...

git tag %buildVersion%

echo Packing...

nuget\nuget pack Package.nuspec -out %outputPath% -version %buildVersion%

echo Pushing...

git push
git push --tags

:exit

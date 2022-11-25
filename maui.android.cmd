@echo off
echo Compile a new stand-alone release for Android
pause
pushd .
cd .\Clients\Quiz.Blazor.Maui.Standalone\
dotnet publish -c:Release -f:net7.0-android --interactive /p:AndroidSigningKeyPass=ksP1pp0zz0! /p:AndroidSigningStorePass=ksP1pp0zz0!
rem dotnet publish -c:Release -f:net6.0-android -o ...\..\..\publish --interactive /p:AndroidSigningKeyPass=ksP1pp0zz0! /p:AndroidSigningStorePass=ksP1pp0zz0!
rem dotnet publish -c:Release -f:net6.0-android -o ...\..\..\publish --interactive /p:AndroidSigningKeyPass /p:AndroidSigningStorePass
 
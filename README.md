# Yet Another Quiz Tool

A almost completed ASP.NET Core 6 (NET6.0) application for quizzes inspired by
[ArnabMSDN/Quiz-Application](http://squarespace.com/ "Title").


## Prerequisites

- Visual Studio 2022
- SQL Server 2019
- .NET Core SDK 6.0

## Configure and run the project

- Clone the complete code from Github
- Open solution Quiz-Application.sln in Visual Studio
- Make Changes in Connection Strings in the `appsettings.json` file to connect the database
- Create the database by runnong the `Update-Database` command in Package Manager Console 
- Run Database Script which is provided
- Build the solution which will restore all NuGet Packages
- Run Quiz-Application.Web Project

## Remarks

The solution is simple and implement a very basic security login system. Password are just base64 encoded.

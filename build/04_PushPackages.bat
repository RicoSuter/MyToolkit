set /p apiKey=NuGet API Key: 
set /p version=Package Version: 

nuget.exe push Packages/MyToolkit.%version%.nupkg %apiKey% -s https://nuget.org
nuget.exe push Packages/MyToolkit.Web.%version%.nupkg %apiKey% -s https://nuget.org
nuget.exe push Packages/MyToolkit.Http.%version%.nupkg %apiKey% -s https://nuget.org
nuget.exe push Packages/MyToolkit.Extended.%version%.nupkg %apiKey% -s https://nuget.org
nuget.exe push Packages/MyToolkit.AspNet.Mvc.%version%.nupkg %apiKey% -s https://nuget.org
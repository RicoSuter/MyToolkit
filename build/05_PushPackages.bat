set /p apiKey=NuGet API Key: 
set /p version=Package Version: 

nuget.exe push Packages/MyToolkit.%version%.nupkg %apiKey%
nuget.exe push Packages/MyToolkit.Web.%version%.nupkg %apiKey%
nuget.exe push Packages/MyToolkit.Http.%version%.nupkg %apiKey%
nuget.exe push Packages/MyToolkit.Extended.%version%.nupkg %apiKey%
nuget.exe push Packages/MyToolkit.AspNet.Mvc.%version%.nupkg %apiKey%
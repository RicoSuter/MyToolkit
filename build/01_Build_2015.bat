nuget restore ../src/MyToolkit.VS2015.sln
msbuild ../src/MyToolkit.VS2015.sln /p:Configuration=Release /t:rebuild

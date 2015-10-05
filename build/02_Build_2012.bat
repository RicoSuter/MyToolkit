nuget restore ../src/MyToolkit.VS2012.sln
msbuild ../src/MyToolkit.VS2012.sln /p:Configuration=Release /t:rebuild

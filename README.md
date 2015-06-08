# MyToolkit for .NET

MyToolkit is a set of .NET libraries containing lots of useful classes for various .NET platforms like WinRT (Universal apps), Windows Phone and WPF. The goal is to provide missing or replace existing classes to support the development of high-quality Windows and Windows Phone applications. For example, the library provides often used MVVM infrastructure classes, missing UI controls, IoC classes, additional LINQ extension methods and much more. The project is developed and maintained by [Rico Suter](http://rsuter.com).

[Available classes and components](https://github.com/MyToolkit/Core/wiki)

- MVVM classes (Messenger,  RelayCommand,  ViewModelBase,  ObservableObject) in portable class library 
- Networking classes:  HTTP with GZIP support,  WakeOnLan 
- XAML controls and converters, e.g.  MtPivot and  DataGrid for WinRT 
- Improved  paging classes for Windows Phone and Windows 8 apps 
- Additional collections like  ObservableDictionary,  MtObservableCollection or  ObservableCollectionView 
- And much more...

**Coming soon: Full support for the Universal Windows Platform (Windows 10)**

(This project has originally been hosted on [CodePlex](http://mytoolkit.codeplex.com))

If you found some bugs or have other comments, please create a Pull Request or GitHub issue. The library is free to use, but please put a link to this GitHub site in the source code or your application. 

Check my blog for other programming stuff on my website at <http://blog.rsuter.com>. 

## Visual Studio solutions

The solutions can be found in the `src` directory: 

**MyToolkit.VS2013.sln**

- Contains the projects for Windows 8.1, Windows Phone 8.1, Windows Phone Silverlight 8.0, WPF and Web
    
**MyToolkit.VS2012.sln**

- Contains the projects for Windows Phone 7 and Silverlight 5

## Installation

### Using NuGet (recommended)

The MyToolkit project is separated into the following packages:

- [MyToolkit](https://nuget.org/packages/MyToolkit) (PCL): MVVM classes, collections, everything which can be portable... 
- [MyToolkit.Extended](https://nuget.org/packages/MyToolkit.Extended) (WinRT/WP7/WP8/SL4/SL5/.Net4.5/(.NET4)): UI (e.g. controls) and framework dependent classes (e.g. Wake-On-Lan class). Don't use this assembly in Windows Phone background agents as some classes are not allowed in these projects. The  YouTube classes can be found in this package... 
- [MyToolkit.Http](https://nuget.org/packages/MyToolkit.Http) (PCL): Portable  HTTP classes with GZIP and file upload support. 
- [MyToolkit.Web](https://nuget.org/packages/MyToolkit.Web) (.NET4.5): .NET classes for Web/Server projects. 
- [MyToolkit.AspNet.Mvc](https://nuget.org/packages/MyToolkit.AspNet.Mvc) (.NET4.5): .NET classes for ASP.NET MVC projects. 

### Using template project

Not available yet. 

### Using ZIP download

Download the [assemblies as ZIP file here](https://github.com/MyToolkit/Core/releases). 

## Supported frameworks

![](https://rawgit.com/MyToolkit/Core/master/-%20Documents/Library%20Matrix.png)

A list with the compiled libraries (VS projects) and their dependencies: 

- <https://github.com/MyToolkit/Core/blob/master/Dependencies.txt>

## Sample applications
There is a sample application for Windows Phone and WinRT which demonstrates various MyToolkit classes (MVVM, Paging, YouTube, etc...) as well as some Windows 8 features. 

Clone or download the [source code](https://github.com/MyToolkit/Core/tree/master) and open the **MyToolkit.sln** solution. 

![](https://rawgit.com/MyToolkit/Core/master/-%20Documents/SampleWindowsStoreApp.png)

## Projects which use MyToolkit libraries

List with some sample projects which use the MyToolkit libraries: 

- [Project Dependency Browser](http://projectdependencybrowser.codeplex.com/)
- [Visual JSON Editor](https://visualjsoneditor.codeplex.com/)

## Donate

MyToolkit is a free project that is developed in spare time. You can show your appreciation for MyToolkit and support future development by donating.

[Donate with PayPal](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=2P7BJZSVJPQWQ)

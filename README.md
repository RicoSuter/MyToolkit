# MyToolkit for .NET

[![NuGet Version](http://img.shields.io/nuget/v/MyToolkit.svg?style=flat)](https://www.nuget.org/packages/MyToolkit/) 

MyToolkit is a set of .NET libraries containing lots of useful classes for various .NET platforms like UWP/WinRT (Universal Windows Apps), Windows Phone and WPF. The goal is to provide missing or replace existing classes to support the development of high-quality Windows and Windows Phone applications. For example, the library provides often used MVVM infrastructure classes, missing UI controls, IoC classes, additional LINQ extension methods and much more. The project is developed and maintained by [Rico Suter](http://rsuter.com).

[Available classes and components](https://github.com/MyToolkit/Core/wiki)

- [MVVM](https://github.com/MyToolkit/Core/wiki/MVVM-Overview) classes ([RelayCommand](https://github.com/MyToolkit/Core/wiki/RelayCommand), [ViewModelBase](https://github.com/MyToolkit/Core/wiki/ViewModelBase), [ObservableObject](https://github.com/MyToolkit/Core/wiki/ObservableObject), [Messenger](https://github.com/MyToolkit/Core/wiki/Messenger)) in portable class library 
- Networking classes: [HTTP](https://github.com/MyToolkit/Core/wiki/Http) with GZIP support,  WakeOnLan 
- XAML controls and converters, e.g. [MtPivot](https://github.com/MyToolkit/Core/wiki/MtPivot), [DataGrid](https://github.com/MyToolkit/Core/wiki/DataGrid) for WinRT/UWP and [Hamburger](https://github.com/MyToolkit/Core/wiki/Hamburger) for UWP
- Improved [paging](https://github.com/MyToolkit/Core/wiki/Paging-Overview) classes for Windows Phone and Windows 8 apps 
- Additional collections like [ObservableDictionary](https://github.com/MyToolkit/Core/wiki/ObservableDictionary),  [MtObservableCollection](https://github.com/MyToolkit/Core/wiki/MtObservableCollection) or [ObservableCollectionView](https://github.com/MyToolkit/Core/wiki/ObservableCollectionView) 
- And much more...

(This project has originally been hosted on [CodePlex](http://mytoolkit.codeplex.com))

If you found some bugs or have other comments, please create a Pull Request or GitHub issue. The library is free to use, but please put a link to this GitHub site in the source code or your application. 

Check my blog for other programming stuff on my website at <http://blog.rsuter.com>. 

## Installation

### Using NuGet (recommended)

The MyToolkit project is separated into the following packages:

- [MyToolkit](https://nuget.org/packages/MyToolkit) (PCL): MVVM classes, collections, everything which can be portable... 
- [MyToolkit.Extended](https://nuget.org/packages/MyToolkit.Extended) (UWP/WinRT/WP7/WP8/SL4/SL5/.Net4.5/(.NET4)): UI (e.g. controls) and framework dependent classes (e.g. Wake-On-Lan class). Don't use this assembly in Windows Phone background agents as some classes are not allowed in these projects. The  YouTube classes can be found in this package... 
- [MyToolkit.Http](https://nuget.org/packages/MyToolkit.Http) (PCL): Portable  HTTP classes with GZIP and file upload support. 
- [MyToolkit.Web](https://nuget.org/packages/MyToolkit.Web) (.NET4.5): .NET classes for Web/Server projects. 
- [MyToolkit.AspNet.Mvc](https://nuget.org/packages/MyToolkit.AspNet.Mvc) (.NET4.5): .NET classes for ASP.NET MVC projects. 

### Using template project

Not available yet. 

### Using ZIP download

Download the [assemblies as ZIP file here](https://github.com/MyToolkit/Core/releases). 

## Visual Studio solutions

The solutions can be found in the `src` directory: 

**MyToolkit.VS2015.sln**

- MyToolkit.Extended.Uwp (Universal Windows Platform: Windows 10, Windows 10 Mobile)

**MyToolkit.VS2013.sln**

- MyToolkit (PCL: UWP, W8, WP8.1, .NET 4.5, WP8SL)
- MyToolkit.AspNet.Mvc (ASP.NET MVC 5.x)
- MyToolkit.Extended.WinRT (PCL: W8.1, WP8.1)
- MyToolkit.Extended.Wp8  (WP8SL)
- MyToolkit.Extended.Wpf40 (.NET 4.0)
- MyToolkit.Extended.Wpf45 (.NET 4.5)
- MyToolkit.Extended.Http (PCL)
- MyToolkit.Extended.MachineLearning.WinRT (PCL: W8.1, WP8.1)
- MyToolkit.Web (.NET 4.5)

**MyToolkit.VS2012.sln**

- MyToolkit.Extended.Sl5 (Silverlight 5)
- MyToolkit.Extended.Wp7 (WP7SL)
- MyToolkit.Http.Legacy (PCL: SL4, WP7SL, .NET 4)
- MyToolkit.Legacy (PCL: SL4, WP7SL, .NET 4)

**Supported frameworks:**

![](https://rawgit.com/MyToolkit/Core/master/docs/LibraryMatrix.png)

Also check out the [list with the Visual Studio projects and their dependencies](https://github.com/MyToolkit/Core/blob/master/docs/Dependencies.md). 

## Sample applications
There is a sample application for Windows Phone and WinRT which demonstrates various MyToolkit classes (MVVM, Paging, YouTube, etc...) as well as some Windows 8 features. 

Clone or download the [source code](https://github.com/MyToolkit/Core/tree/master) and open the **MyToolkit.sln** solution. 

![](https://rawgit.com/MyToolkit/Core/master/docs/SampleWindowsStoreApp.png)

## How to build the library

- MyToolkit.Extended.Wp7
- MyToolkit.Extended.Wp8
    - You need the Windows Phone 8.0 SDK to build a Windows Phone 7 or 8 Silverlight project:
    - https://www.microsoft.com/en-us/download/confirmation.aspx?id=35471
- MyToolkit
    - Code Contracts Visual Studio extension: 
    - https://visualstudiogallery.msdn.microsoft.com/1ec7db13-3363-46c9-851f-1ce455f66970

## Projects which use MyToolkit libraries

List with some sample projects which use the MyToolkit libraries: 

- [Project Dependency Browser](http://projectdependencybrowser.org/)
- [Visual JSON Editor](http://visualjsoneditor.org/)

## Donate

MyToolkit is a free project that is developed in spare time. You can show your appreciation for MyToolkit and support future development by donating.

[Donate with PayPal](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=2P7BJZSVJPQWQ)

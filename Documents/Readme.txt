The MyToolkit library can be downloaded at http://mytoolkit.codeplex.com
Subversion URL: https://mytoolkit.svn.codeplex.com/svn



MVVM: ViewModel instantiation in XAML
=====================================

	XAML:
	============

	<phone:PhoneApplicationPage.Resources>
        <viewModels:MainViewModel x:Key="viewModel" />
	</phone:PhoneApplicationPage.Resources>

    <Grid x:Name="LayoutRoot" DataContext="{StaticResource viewModel}">
        ...
		<ListBox ...>
            ... // reference outer context
				<toolkit:ContextMenuService.ContextMenu>
					<toolkit:ContextMenu>
						<toolkit:MenuItem Header="{Binding Source={StaticResource viewModel}, Path=Strings.MenuDelete}" Tag="{Binding ID}" Click="DeleteNote"/>
					</toolkit:ContextMenu>
	

	Code-Behind: 
	============

	public partial class MainPage : PhoneApplicationPage
	{
		public MainViewModel Model { get { return (MainViewModel) Resources["viewModel"]; } }
using Windows.ApplicationModel.Resources; 
 
namespace SampleWindowsStoreApp.Localization 
{
	public static class LocalizedStrings 
	{
		private static readonly ResourceLoader resourceLoader; 
 
		static LocalizedStrings() 
		{
			resourceLoader = new ResourceLoader();
		}
 
		public static string LocalizedString 
		{
			get { return resourceLoader.GetString("LocalizedString"); }
		}
	}
}

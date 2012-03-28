using Microsoft.Phone.Marketplace;

namespace MyToolkit.Environment
{
	public static class Marketplace
	{
		public static bool IsTrial
		{
			get
			{
				#if DEBUG
				return true;
				#else
				var license = new LicenseInformation();
				return license.IsTrial();
				#endif
			}
		}
	}
}

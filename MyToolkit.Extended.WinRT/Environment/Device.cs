using System.Linq;
using Windows.Networking.Connectivity;

namespace MyToolkit.Environment
{
	public class Device
	{
        private static string _deviceId;

        /// <summary>
        /// Gets a unique ID which can be used to identify the current device. 
        /// </summary>
		public static string DeviceId
		{
			get
			{
                if (_deviceId == null)
				{
					_deviceId = NetworkInformation.GetConnectionProfiles().
						Where(p => p.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.ConstrainedInternetAccess).
						Select(p => p.NetworkAdapter.NetworkAdapterId).
						OrderBy(p => p).First().ToString();
				}

				return _deviceId;
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MyToolkit.Storage
{
	public static class StorageFolderExtensions
	{
		public static async Task<StorageFolder> GetOrCreateFolderAsync(this StorageFolder folder, string folderName)
		{
			try
			{
				return await folder.GetFolderAsync(folderName);
			}
			catch (FileNotFoundException) { }

			return await folder.CreateFolderAsync(folderName);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MyToolkit.Storage
{
	public static class StorageFolderExtensions
	{
		[DebuggerHidden]
		public static async Task<StorageFolder> GetOrCreateFolderAsync(this StorageFolder folder, string folderName)
		{
			try { return await folder.CreateFolderAsync(folderName); } catch { }
			return await folder.GetFolderAsync(folderName);
		}
	}
}
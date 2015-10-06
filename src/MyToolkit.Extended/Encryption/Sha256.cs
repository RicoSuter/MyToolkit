//-----------------------------------------------------------------------
// <copyright file="Sha256.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !WP8 && !WPF

using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;

namespace MyToolkit.Encryption
{
	public static class Sha256
	{
		public static string EncodeToBase64String(string input)
		{
			var algo = HashAlgorithmProvider.OpenAlgorithm("SHA256");   
			var buff = CryptographicBuffer.ConvertStringToBinary(input, BinaryStringEncoding.Utf8);
			var hashed = algo.HashData(buff);    
			return CryptographicBuffer.EncodeToBase64String(hashed);
		}

		public static string EncodeToHexString(string input)
		{
			var algo = HashAlgorithmProvider.OpenAlgorithm("SHA256");
			var buff = CryptographicBuffer.ConvertStringToBinary(input, BinaryStringEncoding.Utf8);
			var hashed = algo.HashData(buff);
			return CryptographicBuffer.EncodeToHexString(hashed);
		}
	}
}

#endif
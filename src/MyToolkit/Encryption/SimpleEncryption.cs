//-----------------------------------------------------------------------
// <copyright file="SimpleEncryption.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Text;

namespace MyToolkit.Encryption
{
    public static class SimpleEncryption
    {
        public static string Encrypt(string text, int key)
        {
            return Crypt(text, key);
        }

        public static string Decrypt(string text, int key)
        {
            return Crypt(text, key);
        }

        private static string Crypt(string text, int key)
        {
            var inSb = new StringBuilder(text);
            var outSb = new StringBuilder(text.Length);
            for (var i = 0; i < text.Length; i++)
            {
                var c = inSb[i];
                c = (char)(c ^ key);
                outSb.Append(c);
            }
            return outSb.ToString();
        }
    }
}

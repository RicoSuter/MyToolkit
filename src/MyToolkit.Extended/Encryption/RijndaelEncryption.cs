//-----------------------------------------------------------------------
// <copyright file="RijndaelEncryption.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !WINRT && !WP8

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace MyToolkit.Encryption
{
    /// <summary>Provides methods to encrypt and decrypt data using the Rijndael encryption algorithm.</summary>
    public static class RijndaelEncryption
    {
        private static Dictionary<string, Tuple<byte[], byte[]>> _passwords;

        private static Tuple<byte[], byte[]> DeriveBytes(string password)
        {
            if (_passwords == null)
                _passwords = new Dictionary<string, Tuple<byte[], byte[]>>();

            if (_passwords.ContainsKey(password))
                return _passwords[password];

            using (var pdb = new Rfc2898DeriveBytes(password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 
                    0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76}))
            {
                var tuple = new Tuple<byte[], byte[]>(pdb.GetBytes(32), pdb.GetBytes(16));
                _passwords[password] = tuple;
                return tuple;
            }
        }

        public static byte[] Encrypt(byte[] clearData, byte[] key, byte[] iv)
        {
            using (var ms = new MemoryStream())
            {
                using (var alg = Rijndael.Create())
                {
                    alg.Key = key;
                    alg.IV = iv;
                    using (var cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearData, 0, clearData.Length);
                        cs.Close();
                        return ms.ToArray();
                    }
                }
            }
        }

        public static string Encrypt(string clearText, string password)
        {
            var clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText);
            var bytes = DeriveBytes(password);
            var encryptedData = Encrypt(clearBytes, bytes.Item1, bytes.Item2);
            return Convert.ToBase64String(encryptedData);
        }

        public static byte[] Encrypt(byte[] clearData, string password)
        {
            var bytes = DeriveBytes(password);
            return Encrypt(clearData, bytes.Item1, bytes.Item2);
        }

        public static byte[] Decrypt(byte[] cipherData, byte[] key, byte[] iv)
        {
            using (var ms = new MemoryStream())
            {
                using (var alg = Rijndael.Create())
                {
                    alg.Key = key;
                    alg.IV = iv;
                    using (var cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherData, 0, cipherData.Length);
                        cs.Close();
                        return ms.ToArray();
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string password)
        {
            var cipherBytes = Convert.FromBase64String(cipherText);
            var bytes = DeriveBytes(password);
            var decryptedData = Decrypt(cipherBytes, bytes.Item1, bytes.Item2);
            return System.Text.Encoding.Unicode.GetString(decryptedData);
        }

        public static byte[] Decrypt(byte[] cipherData, string password)
        {
            var bytes = DeriveBytes(password);
            return Decrypt(cipherData, bytes.Item1, bytes.Item2);
        }
    }
}

#endif
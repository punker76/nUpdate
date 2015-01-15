﻿// Author: Dominic Beger (Trade/ProgTrade)

using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;

namespace nUpdate.Administration.Core
{
    public class AesManager
    {
        /// <summary>
        ///     Encrypts a string with the given key and initializing vector.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        /// <param name="keyPassword">The password which the key should be derived from.</param>
        /// <param name="ivPassword">The password which the initializing vector should be drived from.</param>
        /// <returns>Returns the encrypted string as an byte-array.</returns>
        public static byte[] Encrypt(string plainText, string keyPassword, string ivPassword)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (keyPassword == null || keyPassword.Length <= 0)
                throw new ArgumentNullException("keyPassword");
            if (ivPassword == null || ivPassword.Length <= 0)
                throw new ArgumentNullException("ivPassword");

            var keyPasswordDeriveBytes = new PasswordDeriveBytes(keyPassword,
                new byte[] {0x43, 0x87, 0x23, 0x72, 0x45, 0x56, 0x68, 0x14, 0x62, 0x84});
            var ivPasswordDeriveBytes = new PasswordDeriveBytes(ivPassword,
                new byte[] {0x43, 0x87, 0x23, 0x72, 0x45, 0x56, 0x68, 0x14, 0x62, 0x84});
            byte[] encrypted;
            // Create an AesManaged object
            // with the specified key and IV.
            using (var aesAlg = new AesManaged())
            {
                aesAlg.KeySize = 256;
                aesAlg.Key = keyPasswordDeriveBytes.GetBytes(aesAlg.KeySize/8);
                aesAlg.IV = ivPasswordDeriveBytes.GetBytes(aesAlg.BlockSize/8);

                // Create a decrytor to perform the stream transform.
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        /// <summary>
        ///     Decrypts a string with the given key and initializing vector.
        /// </summary>
        /// <param name="cipherText">The byte-array of the encrypted string.</param>
        /// <param name="keyPassword">The key to use.</param>
        /// <param name="ivPassword">The initializing vector to use.</param>
        /// <returns>Returns the plain string as SecureString.</returns>
        public static SecureString Decrypt(byte[] cipherText, string keyPassword, string ivPassword)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (keyPassword == null || keyPassword.Length <= 0)
                throw new ArgumentNullException("keyPassword");
            if (ivPassword == null || ivPassword.Length <= 0)
                throw new ArgumentNullException("ivPassword");

            var keyPasswordDeriveBytes = new PasswordDeriveBytes(keyPassword,
                new byte[] {0x43, 0x87, 0x23, 0x72, 0x45, 0x56, 0x68, 0x14, 0x62, 0x84});
            var ivPasswordDeriveBytes = new PasswordDeriveBytes(ivPassword,
                new byte[] {0x43, 0x87, 0x23, 0x72, 0x45, 0x56, 0x68, 0x14, 0x62, 0x84});

            // Declare the string used to hold
            // the decrypted text.
            string plaintext;

            // Create an AesManaged object
            // with the specified key and IV.
            using (var aesAlg = new AesManaged())
            {
                aesAlg.KeySize = 256;
                aesAlg.Key = keyPasswordDeriveBytes.GetBytes(aesAlg.KeySize/8);
                aesAlg.IV = ivPasswordDeriveBytes.GetBytes(aesAlg.BlockSize/8);

                // Create a decrytor to perform the stream transform.
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            var securedPlainText = new SecureString();
            foreach (var c in plaintext)
            {
                securedPlainText.AppendChar(c);
            }
            return securedPlainText;
        }
    }
}
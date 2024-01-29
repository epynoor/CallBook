﻿using System.Security.Cryptography;
using System.Text;

namespace CallBookSystem.Web.Helpers
{
    public static class Cipher
    {
        /// <summary>
        /// Encrypt a string.
        /// </summary>
        /// <param name="plainText">String to be encrypted</param>
        /// <param name="password">Password</param>
        public static string EncryptSHA256Str(string val)
        {
            byte[] passwordBytes = null;
            byte[] bytesEncrypted = null;
            try
            {
                string key = "IB";
                if (val == null)
                {
                    return null;
                }

                if (key == null)
                {
                    key = String.Empty;
                }

                // Get the bytes of the string
                var bytesToBeEncrypted = Encoding.UTF8.GetBytes(val);
                passwordBytes = Encoding.UTF8.GetBytes(key);

                // Hash the password with SHA256
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
                bytesEncrypted = Cipher.Encrypt(bytesToBeEncrypted, passwordBytes);
                return Convert.ToBase64String(bytesEncrypted);
            }
            catch (Exception ex)
            {
                passwordBytes = null;
                bytesEncrypted = null;
                throw ex;
            }
            finally
            {
                passwordBytes = null;
                bytesEncrypted = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        /// <summary>
        /// Decrypt a string.
        /// </summary>
        /// <param name="encryptedText">String to be decrypted</param>
        /// <param name="password">Password used during encryption</param>
        /// <exception cref="FormatException"></exception>
        public static string DecryptSHA256Str(string val)
        {
            byte[] passwordBytes = null;
            byte[] bytesDecrypted = null;
            try
            {
                string key = "IB";
                if (val == null)
                {
                    return null;
                }

                if (key == null)
                {
                    key = String.Empty;
                }

                // Get the bytes of the string
                var bytesToBeDecrypted = Convert.FromBase64String(val);
                passwordBytes = Encoding.UTF8.GetBytes(key);

                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);
                bytesDecrypted = Cipher.Decrypt(bytesToBeDecrypted, passwordBytes);
                return Encoding.UTF8.GetString(bytesDecrypted);
            }
            catch (Exception ex)
            {
                passwordBytes = null;
                bytesDecrypted = null;
                throw ex;
            }

            finally
            {
                passwordBytes = null;
                bytesDecrypted = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private static byte[] Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }

                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        private static byte[] Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            var saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                    AES.KeySize = 256;
                    AES.BlockSize = 128;
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);
                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }

                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }
    }
}

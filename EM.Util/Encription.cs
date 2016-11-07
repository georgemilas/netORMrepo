using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace EM.Util
{
    public class Encription
    {
        public readonly PasswordDeriveBytes PasswordDeriveBytes; // = "EM.Util.Encription";
        /// <summary>
        /// An Encryption utility based on creating IV and Secrets starting from given password
        /// </summary>
        /// <param name="password"></param>
        public Encription(string password = null)
        {
            this.PasswordDeriveBytes = GetPasswordDerivedBytes(password ?? "EM.Util.Encription");
        }
        // Encrypt a byte array into a byte array using a key and an IV 
        public static byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            MemoryStream ms = new MemoryStream();
            Rijndael alg = Rijndael.Create();       //uses AES, better then DES
            alg.Padding = PaddingMode.ISO10126;

            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write);

            // Write the data and make it do the encryption 
            cs.Write(clearData, 0, clearData.Length);
            cs.FlushFinalBlock();
            cs.Close();

            byte[] encryptedData = ms.ToArray();

            return encryptedData;
        }

        // Encrypt a string into a string 
        public string Encrypt(string clearText)
        {
            return Encrypt(clearText, this.PasswordDeriveBytes);
        }

        public static PasswordDeriveBytes GetPasswordDerivedBytes(string password)
        {
            return new PasswordDeriveBytes(password,
                new byte[] {0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76});
        }

        // Encrypt a string into a string using a password 
        //    Uses Encrypt(byte[], byte[], byte[]) 
        public static string Encrypt(string clearText, PasswordDeriveBytes pdb)
        {
            // First we need to turn the input string into a byte array. 
            byte[] clearBytes =
              System.Text.Encoding.ASCII.GetBytes(clearText);

            byte[] encryptedData = Encrypt(clearBytes, pdb.GetBytes(32), pdb.GetBytes(16));

            return Convert.ToBase64String(encryptedData);

        }

        // Decrypt a string into a string 
        public string Decrypt(string cipherText)
        {
            return Decrypt(cipherText, this.PasswordDeriveBytes);
        }

        // Decrypt a string into a string using a password  
        //    Uses Decrypt(byte[], byte[], byte[]) 
        public static string Decrypt(string cipherText, PasswordDeriveBytes pdb)
        {

            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] decryptedData = Decrypt(cipherBytes, pdb.GetBytes(32), pdb.GetBytes(16), cipherBytes.Length);
            return System.Text.Encoding.ASCII.GetString(decryptedData);
        }

        // Decrypt a byte array into a byte array using a key and an IV 
        public static byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV, int maxbuffersize)
        {
            byte[] realchiper = new byte[maxbuffersize];
            Array.Copy(cipherData, 0, realchiper, 0, maxbuffersize);

            MemoryStream ms = new MemoryStream(realchiper);
            Rijndael alg = Rijndael.Create();   //uses AES, better then DES
            alg.Padding = PaddingMode.ISO10126;
            alg.Key = Key;
            alg.IV = IV;
            CryptoStream cs = new CryptoStream(ms,
                alg.CreateDecryptor(), CryptoStreamMode.Read);

            BinaryReader b = new BinaryReader(cs);
            byte[] result = b.ReadBytes(maxbuffersize);
            b.BaseStream.Close();
            b.Close();

            return result;
        }

        public static string GetRfc2898Hash(string value, string salt, int iterations = 100, int hashLength = 190)
        {
            Encoding enc = Encoding.UTF8;

            using (Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(value, enc.GetBytes(salt), iterations))
            {
                return Convert.ToBase64String(rfc2898.GetBytes(hashLength));
            }
        }

    }
}

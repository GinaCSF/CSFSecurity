using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                switch (args[0].ToUpper())
                {
                    case "/ENCRYPT":
                        EncryptFile();
                        break;
                    case "/DECRYPT":
                        DecryptFile();
                        break;
                    case "/H":
                    case "/HELP":
                        Console.WriteLine("Help Syntax:");
                        Console.WriteLine("/Encrypt: Encrypt Prevention mechanism rule file.");
                        Console.WriteLine("/Decrypt: Decrypt Prevention mechanism rule file.");
                        break;
                    default:
                        Console.WriteLine("The parameter is wrong. Please enter the correct parameter.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("The parameter is wrong. Please enter the correct parameter.");
            }
            Console.WriteLine("\nPlease enter any key and leave.");
            //Console.ReadKey();
        }

        public static void EncryptFile()
        {
            string file = $"{Environment.CurrentDirectory}\\PAC.txt";
            string password = "abcd1234";

            if (File.Exists(file))
            {
                byte[] bytesToBeEncrypted = File.ReadAllBytes(file);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Hash the password with SHA256
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

                string fileEncrypted = $"{Environment.CurrentDirectory}\\CSF.BIN";
                File.WriteAllBytes(fileEncrypted, bytesEncrypted);

                Console.WriteLine("Encryption job succeeded.");
            }
            else
            {
                Console.WriteLine("Error:\n\n　　PAC.txt not exists.");
            }
        }

        public static void DecryptFile()
        {
            string fileEncrypted = $"{Environment.CurrentDirectory}\\CSF.BIN";
            string password = "abcd1234";

            if (File.Exists(fileEncrypted))
            {
                byte[] bytesToBeDecrypted = File.ReadAllBytes(fileEncrypted);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

                byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

                string file = $"{Environment.CurrentDirectory}\\PAC.txt";
                File.WriteAllBytes(file, bytesDecrypted);

                Console.WriteLine("Decryption job succeeded.");
            }
            else
            {
                Console.WriteLine("Error:\n\n　　CSF.BIN not exists.");
            }

        }

        public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
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

        public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
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
    }
}

using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Encryption
{
    class Program
    {
        static void Main(string[] args)
        {
            // Gets the PDF and converts it to a byte array
            // Also has the password as an input
            string pdfFilePath = ("C:/Users/jesar/Box/BYU/WS2021/IS414/Lab_1/Encryption and Hashing Lab Files/PO-encrypted.pdf");
            byte[] bytespdf = System.IO.File.ReadAllBytes(pdfFilePath);
            string password = "drpepperissuperior";
            
            // Call our decryption method which returns a byte array and receives the pdf as a byte array and the password
            byte[] decryptedMessage = Decrypt(bytespdf, password);

            //Makes the decrypted pdf in the same dirrectory as the program
            System.IO.File.WriteAllBytes("PO-decrypted.pdf", decryptedMessage);
        }

        public static byte[] Decrypt(byte[] cipherbytes, string password)
        {
            // Again, we need to convert (derive) our key from the string password.
            // We'll use a SHA256 hash again for this
            byte[] key = SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(password));

            // Let's read our bytes into a memory stream. If we were decrypting a file, we would probably use a Filestream
            using MemoryStream memStream = new MemoryStream(cipherbytes);
  
            // Create our instance of the AES algorithm and set its properties
            using Aes aes = Aes.Create();
            aes.Mode = CipherMode.CBC; // Match the encryption mode with the encrypt method
            aes.Padding = PaddingMode.PKCS7; // Again, match the padding type with the encrypt method

            // In this case, we need to get the IV that was prepended to our encrypted data
            byte[] iv = new byte[aes.IV.Length]; // create an array of the proper length (default IV length is what we want)
            memStream.Read(iv, 0, iv.Length); // read the IV from the beginning of our memory stream and populate our iv byte[]
 
            // We'll create a new cryptostream, this time with a Decryptor and in read mode
            using CryptoStream cryptStream = new CryptoStream(memStream, aes.CreateDecryptor(key, iv), CryptoStreamMode.Read);

            // We're going to read the decrypted information from the cryptostream using a binaryreader
            BinaryReader bReader = new BinaryReader(cryptStream);
            byte[] decryptedText = bReader.ReadBytes(2000000); // Reads the bytes and converts to a byte array and returns it

            return decryptedText;
        }
    }
}

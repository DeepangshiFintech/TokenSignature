using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TokenSignatureApp
{
    class Program
    {
        static void Main(string[] args)
        {
        again:
            Console.Write("Enter token string: ");
            string tokenString = Console.ReadLine();
            Console.WriteLine($"Token String: {tokenString}");

            // string pfxFilePath = @"File/NPI.pfx";
            string pfxFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "File", "NPI.pfx");
            Console.WriteLine($"PFX File Path: {pfxFilePath}");

            string pfxPassword = "123";

            try
            {
                X509Certificate2 certificate = new X509Certificate2(pfxFilePath, pfxPassword);
                RSA rsaPublicKey = certificate.GetRSAPublicKey();
                if (rsaPublicKey != null)
                {
                    byte[] tokenBytes = Encoding.UTF8.GetBytes(tokenString);
                    byte[] hashedToken;

                    using (SHA256 sha256 = SHA256.Create())
                    {
                        hashedToken = sha256.ComputeHash(tokenBytes);
                    }

                    byte[] signature = certificate.GetRSAPrivateKey().SignHash(hashedToken, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    string signatureBase64 = Convert.ToBase64String(signature);

                    Console.WriteLine($"Digital Signature (Base64): {signatureBase64}");
                    goto again;
                }
                else
                {
                    Console.WriteLine("Public key not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}

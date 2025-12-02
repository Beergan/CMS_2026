using System;
using System.Security.Cryptography;
using System.Text;

namespace CMS_2026.Utils
{
    public class CryptographyHelper
    {
        private const string Key = "sieulienket.vn";

        public static string Encrypt(string toEncrypt)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);
            
            using (var hashmd5 = MD5.Create())
            {
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(Key));
            }

            using (var tdes = System.Security.Cryptography.TripleDES.Create())
            {
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;
                
                using (ICryptoTransform cTransform = tdes.CreateEncryptor())
                {
                    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                    string strreturn = Convert.ToBase64String(resultArray, 0, resultArray.Length)
                        .Replace("/", "xet-")
                        .Replace("&", "va-")
                        .Replace("?", "hoi-")
                        .Replace("=", "ba-")
                        .Replace("+", "cong-");
                    return strreturn;
                }
            }
        }

        public static string Decrypt(string toDecrypt)
        {
            byte[] keyArray;
            byte[] toDecryptArray = Convert.FromBase64String(
                toDecrypt.Replace("xet-", "/")
                         .Replace("va-", "&")
                         .Replace("hoi-", "?")
                         .Replace("ba-", "=")
                         .Replace("cong-", "+"));

            using (var hashmd5 = MD5.Create())
            {
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes(Key));
            }

            using (var tdes = System.Security.Cryptography.TripleDES.Create())
            {
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;
                
                using (ICryptoTransform cTransform = tdes.CreateDecryptor())
                {
                    byte[] resultArray = cTransform.TransformFinalBlock(toDecryptArray, 0, toDecryptArray.Length);
                    return Encoding.UTF8.GetString(resultArray);
                }
            }
        }

        public static string HashSHA256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = sha256.ComputeHash(bytes);
                StringBuilder result = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    result.Append(hash[i].ToString("x2"));
                }
                return result.ToString();
            }
        }
    }
}


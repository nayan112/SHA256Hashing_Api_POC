using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.StaticFiles;

namespace SHA256Hashing_Api_POC.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HashingController : ControllerBase
    {
        private readonly ILogger<HashingController> _logger;
        private static string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public HashingController(ILogger<HashingController> logger)
        {
            _logger = logger;
        }

        [HttpPost("GetHash")]
        public async Task<ActionResult<string>> GetHash(string text)
        {
            var hash = await GetHashSha256(text);
            return hash;
        }

        [HttpPost("GetXMLHash")]
        public async Task<ActionResult<string>> GetXMLHash(IFormFile file)
        {
            //validate file name, type, ext
            //exception handling
            string hash = String.Empty;
            if (file.Length > 0)
            {
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        hash = await GetHashSha256(Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return hash;
        }

        [HttpPost("GetHTMLHash")]
        public async Task<ActionResult<string>> GetHTMLHash(IFormFile file)
        {
            //validate file name, type, ext
            //exception handling
            string hash = String.Empty;
            if (file.Length > 0)
            {
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        hash = await GetHashSha256(Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return hash;
        }

        [HttpPost("Encrypt")]
        public async Task<ActionResult<string>> Encrypt(IFormFile file)
        {
            string data = String.Empty;
            if (file.Length > 0)
            {
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        data = await Encrypt(Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            var fileName = $"Encrypted_{file.FileName}";
            string mimeType = "";
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out mimeType);
            byte[] fileBytes = Encoding.Unicode.GetBytes(data);

            return new FileContentResult(fileBytes, mimeType)
            {
                FileDownloadName = fileName
            };
        }
        [HttpPost("Decrypt")]
        public async Task<ActionResult<string>> Decrypt(IFormFile file)
        {
            string data = String.Empty;
            if (file.Length > 0)
            {
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        data = await Decrypt(Encoding.Unicode.GetString(stream.GetBuffer(), 0, (int)stream.Length));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            var fileName = $"Decrypted_{file.FileName}";
            string mimeType = "text/plain";
            //new FileExtensionContentTypeProvider().TryGetContentType(fileName, out mimeType);
            byte[] fileBytes = Encoding.UTF8.GetBytes(data);

            return new FileContentResult(fileBytes, mimeType)
            {
                FileDownloadName = fileName
            };
        }

        private static async Task<string> GetHashSha256(string text)
        {
            string hashString = string.Empty;
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            using (var myHash = new SHA256Managed())
            {
                byte[] hash = myHash.ComputeHash(bytes);
                foreach (byte x in hash)
                {
                    hashString += String.Format("{0:x2}", x);
                }
            }
            return hashString;
        }

        private static async Task<string> Encrypt(string encryptString)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }

        private static async Task<string> Decrypt(string cipherText)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, 
                    new byte[] 
                    {
                        0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
                    });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }
    }
}

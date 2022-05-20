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

namespace SHA256Hashing_Api_POC.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HashingController : ControllerBase
    {
        private readonly ILogger<HashingController> _logger;

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

        [HttpPost("GetFileHash")]
        public async Task<ActionResult<string>> GetFileHash(IFormFile file)
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
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SHA256Hashing_Api_POC.Controllers
{
    public class InputEncodingController : Controller
    {
        public InputEncodingController()
        {

        }

        [HttpPost("EncodeTextToBase64")]
        public async Task<ActionResult<string>> EncodeTextToBase64([FromBody]string data)
        {
            string hash = String.Empty;
            try
            {
                hash = Base64Encode(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return hash;
        }
        
        [HttpPost("DecodeBase64ToText")] //include "" around test
        public async Task<ActionResult<string>> DecodeBase64ToText([FromBody] string data)
        {
            string hash = String.Empty;
            try
            {
                hash = Base64Decode(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return hash;
        }

        [HttpPost("EncodeXMLFileToBase64")]
        public async Task<ActionResult<string>> EncodeXMLFileToBase64(IFormFile file)
        {
            string hash = String.Empty;
            if (file.Length > 0)
            {
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        hash = Base64Encode(Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return hash;
        }

        [HttpPost("DecoodeBase64FileToText")]
        public async Task<ActionResult<string>> DecodeBase64FileToText(IFormFile file)
        {
            string decodedtext = String.Empty;
            if (file.Length > 0)
            {
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        decodedtext = Base64Decode(Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return decodedtext;
        }

        private static string Base64Encode(string plainText)
        {
            plainText = RemoveStartingNadTrailingQuote(plainText);
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        private static string Base64Decode(string plainText)
        {
            plainText = RemoveStartingNadTrailingQuote(plainText);
            return Encoding.UTF8.GetString(Convert.FromBase64String(plainText));

        }
        private static string RemoveStartingAndTrailingQuote(string plainText)
        {
            if (plainText.StartsWith("\"") && plainText.EndsWith("\""))
            {
                plainText = plainText.Remove(0, 1);
                plainText = plainText.Remove(plainText.Length - 1, 1);
            }

            return plainText;
        }
    }
}

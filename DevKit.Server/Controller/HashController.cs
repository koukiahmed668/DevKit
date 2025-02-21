using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace DevKit.Server.Controller
{
    [Route("api/hash")]
    [ApiController]
    public class HashController : ControllerBase
    {
        [HttpPost("generate")]
        public IActionResult GenerateHash([FromBody] HashRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.InputText) || string.IsNullOrWhiteSpace(request.Algorithm))
            {
                return BadRequest("Input text and algorithm are required.");
            }

            string hashResult;
            try
            {
                hashResult = ComputeHash(request.InputText, request.Algorithm);
            }
            catch
            {
                return BadRequest("Unsupported hashing algorithm.");
            }

            return Ok(new { Hash = hashResult });
        }

        private string ComputeHash(string input, string algorithm)
        {
            using HashAlgorithm? hasher = algorithm.ToUpper() switch
            {
                "MD5" => MD5.Create(),
                "SHA-1" => SHA1.Create(),
                "SHA-256" => SHA256.Create(),
                "SHA-512" => SHA512.Create(),
                _ => throw new NotSupportedException()
            };

            byte[] hashBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }

    public class HashRequest
    {
        public string InputText { get; set; } = string.Empty;
        public string Algorithm { get; set; } = string.Empty;
    }
}

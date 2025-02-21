using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Text.Json;

namespace DevKit.Server.Controllers
{
    [Route("api/encode-decode")]
    [ApiController]
    public class EncoderDecoderController : ControllerBase
    {
        [HttpPost]
        public IActionResult EncodeDecode([FromBody] EncodeDecodeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.InputText))
                return BadRequest("Input text cannot be empty.");

            try
            {
                string result = request.Type.ToLower() switch
                {
                    "base64-encode" => Convert.ToBase64String(Encoding.UTF8.GetBytes(request.InputText)),
                    "base64-decode" => Encoding.UTF8.GetString(Convert.FromBase64String(request.InputText)),
                    "url-encode" => Uri.EscapeDataString(request.InputText),
                    "url-decode" => Uri.UnescapeDataString(request.InputText),
                    "jwt-decode" => DecodeJwt(request.InputText),
                    _ => throw new ArgumentException("Invalid type specified.")
                };

                return Ok(new { Result = result });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error processing request: {ex.Message}");
            }
        }

        private string DecodeJwt(string jwtToken)
        {
            var parts = jwtToken.Split('.');
            if (parts.Length < 2)
                throw new ArgumentException("Invalid JWT format.");

            string header = Encoding.UTF8.GetString(Convert.FromBase64String(PadBase64(parts[0])));
            string payload = Encoding.UTF8.GetString(Convert.FromBase64String(PadBase64(parts[1])));

            var formattedJson = JsonSerializer.Serialize(new { Header = JsonDocument.Parse(header), Payload = JsonDocument.Parse(payload) }, new JsonSerializerOptions { WriteIndented = true });

            return formattedJson;
        }

        private string PadBase64(string base64)
        {
            int mod4 = base64.Length % 4;
            if (mod4 > 0)
            {
                base64 += new string('=', 4 - mod4);
            }
            return base64.Replace('-', '+').Replace('_', '/'); // Fix Base64URL encoding
        }
    }

    public class EncodeDecodeRequest
    {
        public string InputText { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}

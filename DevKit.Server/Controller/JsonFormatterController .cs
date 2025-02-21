using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

[ApiController]
[Route("api/json")]
public class JsonFormatterController : ControllerBase
{
    [HttpPost("format")]
    public IActionResult FormatJson([FromBody] JsonRequest request)
    {
        try
        {
            // Parse and reformat JSON
            var parsed = JsonSerializer.Deserialize<object>(request.Input);
            var formattedJson = JsonSerializer.Serialize(parsed, new JsonSerializerOptions { WriteIndented = true });
            return Ok(new { formattedJson });
        }
        catch (JsonException)
        {
            return BadRequest("Invalid JSON input.");
        }
    }
}

public class JsonRequest
{
    public string Input { get; set; } = string.Empty;
}

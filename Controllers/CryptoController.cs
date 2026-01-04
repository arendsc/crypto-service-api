using CryptoService.Models;
using CryptoService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CryptoService.Controllers;

[Authorize(Roles = "crypto-user")]
[ApiController]
[Route("crypto")]
public class CryptoController : ControllerBase
{
    private readonly ICryptoService _crypto;
    private readonly ILogger<CryptoController> _logger;

    public CryptoController(ICryptoService crypto, ILogger<CryptoController> logger)
    {
        _crypto = crypto;
        _logger = logger;
    }

    [HttpPost("hash")]
    public IActionResult Hash([FromBody] HashRequest? request)
    {
        if (request == null || string.IsNullOrEmpty(request.Data))
        {
            return BadRequest(new { error = "Data is required" });
        }

        try
        {
            var hash = _crypto.Hash(request.Data);
            _logger.LogInformation("Hash operation successful!");
            return Ok(new { hash = Convert.ToBase64String(hash) });
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Hash operation failed");
            return BadRequest("Hash operation failed :(");
        }
    }

    [HttpPost("sign")]
    public IActionResult Sign([FromBody] SignRequest? request)
    {
        if (request == null || string.IsNullOrEmpty(request.Data))
        {
            return BadRequest(new { error = "Data is required" });
        }
        try {
            var signature = _crypto.Sign(request.Data);

            //log if signing was successful
            _logger.LogInformation("Sign operation successful. User={User}, DataLength={Length}", User.Identity?.Name ?? "anonymous", request.Data.Length);
            return Ok(new { signature = Convert.ToBase64String(signature) });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sign operation failed");
            return BadRequest("Sign operation failed :(");
        }
    }

    [HttpPost("verify")]
    public IActionResult Verify([FromBody] VerifyRequest? request)
    {
        if (request == null || string.IsNullOrEmpty(request.Data) || string.IsNullOrEmpty(request.Signature))
        {
            return BadRequest(new { error = "Data and signature are required" });
        }
        try
        {
            var signature = Convert.FromBase64String(request.Signature);
            //log if verifying was successful, only log DataLength and not the signature
            _logger.LogInformation("Verify operation successful. User={User}, DataLength={Length}", User.Identity?.Name ?? "anonymous", request.Data.Length);
            return Ok(new { verified = _crypto.Verify(request.Data, signature) });
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Verify operation failed");
            return BadRequest("Verify operation failed :(");
        }
    }
}


using System.ComponentModel.DataAnnotations;

namespace CryptoService.Models;

public record HashRequest(string Data);

public record SignRequest(
    [Required]
    [MaxLength(4096)]
    string Data);

public record VerifyRequest(
    [Required]
    [MaxLength(4096)]
    string Data, 
    
    [Required]
    [MaxLength(8192)]
    string Signature);


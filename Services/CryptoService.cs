using CryptoService.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace CryptoService.Services;

public interface ICryptoService
{
    byte[] Hash(string data);
    byte[] Sign(string data);
    bool Verify(string data, byte[] signature);
}

public class CryptoServiceImpl : ICryptoService
{
    private readonly ECDsa _signingKey;
    private readonly ECDsa _verificationKey;

    public CryptoServiceImpl(IConfiguration config)
    {
        var privateKeyBase64 = config["CryptoKeys:PrivateKey"];
        var publicKeyBase64 = config["CryptoKeys:PublicKey"];

        if (string.IsNullOrWhiteSpace(privateKeyBase64) || string.IsNullOrWhiteSpace(publicKeyBase64))
        {
            throw new InvalidOperationException("Crypto keys are not configured");
        }

        _signingKey = ECDsa.Create();
        _signingKey.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKeyBase64), out _);
        _verificationKey = ECDsa.Create();
        _verificationKey.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKeyBase64), out _);
    }

    public byte[] Hash(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return Array.Empty<byte>();
        }

        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(data);
        return sha256.ComputeHash(bytes);
    }

    public byte[] Sign(string data)
    {
        var bytes = Encoding.UTF8.GetBytes(data);
        return _signingKey.SignData(bytes, HashAlgorithmName.SHA256);
    }

    public bool Verify(string data, byte[] signature)
    {
        var bytes = Encoding.UTF8.GetBytes(data);
        return _verificationKey.VerifyData(bytes, signature, HashAlgorithmName.SHA256);
    }
}


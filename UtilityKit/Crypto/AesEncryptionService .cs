using System;
using System.IO;
using System.Security.Cryptography;

namespace UtilityKit.Crypto;


/// <summary>
/// Defines basic encryption service operations used to encrypt and decrypt
/// textual data using a symmetric algorithm.
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts the specified plain text and returns the cipher text together
    /// with the initialization vector (IV) used for encryption.
    /// </summary>
    /// <param name="plainText">The UTF-8 plain text to encrypt.</param>
    /// <returns>
    /// A tuple containing the encrypted bytes as <c>cipherText</c> and the
    /// initialization vector as <c>iv</c> required for decryption.
    /// </returns>
    (byte[] cipherText, byte[] iv) Encrypt(string plainText);

    /// <summary>
    /// Decrypts the specified cipher text using the provided initialization
    /// vector and returns the resulting UTF-8 plain text.
    /// </summary>
    /// <param name="cipherText">The encrypted data to decrypt.</param>
    /// <param name="iv">The initialization vector that was used to encrypt the data.</param>
    /// <returns>The decrypted plain text.</returns>
    string Decrypt(byte[] cipherText, byte[] iv);
}

/// <summary>
/// AES-based implementation of <see cref="IEncryptionService"/> that
/// provides simple synchronous methods to encrypt and decrypt strings.
/// </summary>
/// <remarks>
/// Example usage:
/// <code language="csharp">
/// // Generate a 256-bit key (32 bytes) and create the service
/// var key = new byte[32];
/// RandomNumberGenerator.Fill(key);
/// var service = new AesEncryptionService(key);
///
/// // Encrypt
/// var (cipher, iv) = service.Encrypt("Hello, world!");
/// var cipherBase64 = Convert.ToBase64String(cipher);
/// var ivBase64 = Convert.ToBase64String(iv);
///
/// // Later, to decrypt:
/// var cipherBytes = Convert.FromBase64String(cipherBase64);
/// var ivBytes = Convert.FromBase64String(ivBase64);
/// var plain = service.Decrypt(cipherBytes, ivBytes);
/// </code>
/// </remarks>
public class AesEncryptionService : IEncryptionService
{
    private readonly byte[] _key;

    /// <summary>
    /// Initializes a new instance of the <see cref="AesEncryptionService"/>
    /// with the specified symmetric key.
    /// </summary>
    /// <param name="key">The symmetric key to use for AES encryption/decryption.</param>
    public AesEncryptionService(byte[] key)
    {
        _key = key;
    }

    /// <summary>
    /// Encrypts the given plain text using AES and returns the cipher text
    /// and the initialization vector (IV) that must be supplied for
    /// decryption.
    /// </summary>
    /// <param name="plainText">The plain text to encrypt.</param>
    /// <returns>
    /// A tuple containing the encrypted bytes as <c>cipherText</c> and the
    /// <c>iv</c> used for the encryption operation.
    /// </returns>
    public (byte[] cipherText, byte[] iv) Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using var sw = new StreamWriter(cs);
        sw.Write(plainText);

        return (ms.ToArray(), aes.IV);
    }

    /// <summary>
    /// Decrypts the specified cipher text using the provided initialization
    /// vector and returns the resulting plain text.
    /// </summary>
    /// <param name="cipherText">The encrypted bytes to decrypt.</param>
    /// <param name="iv">The initialization vector that was used to encrypt the data.</param>
    /// <returns>The decrypted plain text.</returns>
    public string Decrypt(byte[] cipherText, byte[] iv)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(cipherText);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }
}

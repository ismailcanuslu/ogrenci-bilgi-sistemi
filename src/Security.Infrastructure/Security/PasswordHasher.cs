using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Security.Application.Abstractions.Security;

namespace Security.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;

    private const int MemorySize = 64 * 1024;
    private const int Iterations = 3;
    private const int DegreeOfParallelism = 2;

    private const int Argon2Version = 19;

    public string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);

        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        var hash = DeriveHash(
            password,
            salt,
            MemorySize,
            Iterations,
            DegreeOfParallelism);

        return EncodePhc(
            hash,
            salt,
            MemorySize,
            Iterations,
            DegreeOfParallelism);
    }

    public bool Verify(
        string password,
        string passwordHash)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);
        ArgumentException.ThrowIfNullOrEmpty(passwordHash);

        if (!TryParsePhc(
                passwordHash,
                out var salt,
                out var expectedHash,
                out var memorySize,
                out var iterations,
                out var degreeOfParallelism))
        {
            return false;
        }

        var actualHash = DeriveHash(
            password,
            salt,
            memorySize,
            iterations,
            degreeOfParallelism);

        return CryptographicOperations.FixedTimeEquals(
            actualHash,
            expectedHash);
    }

    private static byte[] DeriveHash(
        string password,
        byte[] salt,
        int memorySize,
        int iterations,
        int degreeOfParallelism)
    {
        var argon2 = new Argon2id(
            Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            MemorySize = memorySize,
            Iterations = iterations,
            DegreeOfParallelism = degreeOfParallelism
        };

        return argon2.GetBytes(HashSize);
    }

    private static string EncodePhc(
        byte[] hash,
        byte[] salt,
        int memorySize,
        int iterations,
        int degreeOfParallelism)
    {
        var saltBase64 = Convert.ToBase64String(salt)
            .TrimEnd('=');

        var hashBase64 = Convert.ToBase64String(hash)
            .TrimEnd('=');

        return
            $"$argon2id$v={Argon2Version}" +
            $"$m={memorySize},t={iterations},p={degreeOfParallelism}" +
            $"${saltBase64}" +
            $"${hashBase64}";
    }

    private static bool TryParsePhc(
        string encoded,
        out byte[] salt,
        out byte[] hash,
        out int memorySize,
        out int iterations,
        out int degreeOfParallelism)
    {
        salt = [];
        hash = [];
        memorySize = 0;
        iterations = 0;
        degreeOfParallelism = 0;

        var parts = encoded.Split('$');

        if (parts.Length != 6)
            return false;

        if (parts[1] != "argon2id")
            return false;

        if (!parts[2].StartsWith("v="))
            return false;

        if (!int.TryParse(
                parts[2]["v=".Length..],
                out var version) ||
            version != Argon2Version)
        {
            return false;
        }

        var parameters = parts[3].Split(',');

        if (parameters.Length != 3)
            return false;

        if (!TryParseParameter(
                parameters[0],
                "m",
                out memorySize))
        {
            return false;
        }

        if (!TryParseParameter(
                parameters[1],
                "t",
                out iterations))
        {
            return false;
        }

        if (!TryParseParameter(
                parameters[2],
                "p",
                out degreeOfParallelism))
        {
            return false;
        }

        if (memorySize <= 0 ||
            iterations <= 0 ||
            degreeOfParallelism <= 0)
        {
            return false;
        }

        try
        {
            salt = DecodeBase64(parts[4]);
            hash = DecodeBase64(parts[5]);
        }
        catch (FormatException)
        {
            return false;
        }

        return salt.Length == SaltSize &&
               hash.Length == HashSize;
    }

    private static bool TryParseParameter(
        string value,
        string name,
        out int result)
    {
        result = 0;

        if (!value.StartsWith($"{name}="))
            return false;

        return int.TryParse(
            value[$"{name}=".Length..],
            out result);
    }

    private static byte[] DecodeBase64(string value)
    {
        var padding = value.Length % 4;

        if (padding != 0)
        {
            value = value.PadRight(
                value.Length + (4 - padding),
                '=');
        }

        return Convert.FromBase64String(value);
    }
}
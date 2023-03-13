namespace API.Utils;

/// <summary>
/// A static class for generating random strings.<br/>
/// Can be used to generate random strings for passwords, tokens, etc.<br/>
/// Can be expanded to generate any combination of characters.<br/>
/// </summary>
public static class RandomStringGenerator
{
    private static readonly Random Random = new Random();
    private const string AlphaUpperNumeric = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const string AlphaLowerNumeric = "abcdefghijklmnopqrstuvwxyz0123456789";
    private const string AlphaNumeric = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const string Alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private const string AlphaUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string AlphaLower = "abcdefghijklmnopqrstuvwxyz";
    private const string Numeric = "0123456789";

    /// <summary>
    /// Generates a random string of the specified length, containing only uppercase letters and numbers.<br/>
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string GenerateAlphaNumeric(int length)
    {
        return new string(Enumerable.Repeat(AlphaNumeric, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
    
    /// <summary>
    /// Generates random string of the specified length, containing only numbers.<br/>
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string GenerateNumeric(int length)
    {
        return new string(Enumerable.Repeat(Numeric, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
}
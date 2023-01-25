namespace API.Utils;

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

    public static string GenerateAlphaNumeric(int length)
    {
        return new string(Enumerable.Repeat(AlphaNumeric, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
    
    public static string GenerateNumeric(int length)
    {
        return new string(Enumerable.Repeat(Numeric, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }
}
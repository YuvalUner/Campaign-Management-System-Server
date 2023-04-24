namespace DAL.Models;

public static class PropertyNames
{
    public const string Identifier = "Identifier";
    public const string LastName = "LastName";
    public const string FirstName = "FirstName";
    public const string CityName = "CityName";
    public const string BallotId = "BallotId";
    public const string StreetName = "StreetName";
    public const string HouseNumber = "HouseNumber";
    public const string Entrance = "Entrance";
    public const string Appartment = "Appartment";
    public const string HouseLetter = "HouseLetter";
    public const string ZipCode = "ZipCode";
    public const string Email1 = "Email1";
    public const string Email2 = "Email2";
    public const string Phone1 = "Phone1";
    public const string Phone2 = "Phone2";
    public const string SupportStatus = "SupportStatus";

    private static readonly string[] All = new[]
    {
        Identifier,
        LastName,
        FirstName,
        CityName,
        BallotId,
        StreetName,
        HouseNumber,
        Entrance,
        Appartment,
        HouseLetter,
        ZipCode,
        Email1,
        Email2,
        Phone1,
        Phone2,
        SupportStatus
    };
    
    public static bool IsValid(string propertyName)
    {
        return All.Contains(propertyName);
    }
}

public class ColumnMapping
{
    
    private readonly string? propertyName;
    public string? ColumnName { get; set; }

    public string? PropertyName
    {
        get => propertyName;
        init
        {
            if (!PropertyNames.IsValid(value))
            {
                throw new ArgumentException($"Invalid property name {value}");
            }

            propertyName = value;
        }
    }
}
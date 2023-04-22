namespace DAL.Models;

public static class PropertyNames
{
    public const string Identifier = "identifier";
    public const string LastName = "lastName";
    public const string FirstName = "firstName";
    public const string CityName = "cityName";
    public const string BallotId = "ballotId";
    public const string StreetName = "streetName";
    public const string HouseNumber = "houseNumber";
    public const string Entrance = "entrance";
    public const string Appartment = "appartment";
    public const string HouseLetter = "houseLetter";
    public const string ZipCode = "zipCode";
    public const string Email1 = "email1";
    public const string Email2 = "email2";
    public const string Phone1 = "phone1";
    public const string Phone2 = "phone2";
    public const string SupportStatus = "supportStatus";

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
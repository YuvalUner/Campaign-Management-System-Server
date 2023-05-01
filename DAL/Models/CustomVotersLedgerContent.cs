namespace DAL.Models;

public class CustomVotersLedgerContent
{
    public int? Identifier { get; set; }
    public string? LastName { get; set; } 
    public string? FirstName { get; set; }
    public string? CityName { get; set; } 
    public Decimal? BallotId { get; set; }
    public string? StreetName { get; set; } 
    public int? HouseNumber { get; set; } 
    public string? Entrance { get; set; } 
    public string? Appartment { get; set; } 
    public string? HouseLetter { get; set; } 
    public int? ZipCode { get; set; }
    public string? Email1 { get; set; }
    public string? Email2 { get; set; }
    public string? Phone1 { get; set; }
    public string? Phone2 { get; set; }
    public bool? SupportStatus { get; set; }
    
    public string? SupportStatusString { get; set; }

    
    public bool? ConvertToBool(string? val)
    {
        if (val == null)
        {
            return null;
        }
        val = val.ToLower();
        if (val == "supporting" || val == "true")
        {
            return true;
        }
        else if (val == "opposing" || val == "false")
        {
            return false;
        }

        return null;
    }

    private object? ConvertTo(string propertyName, string value)
    {
        try
        {
            return propertyName switch
            {
                "Identifier" => Int32.Parse(value),
                "BallotId" => Decimal.Parse(value),
                "HouseNumber" => Int32.Parse(value),
                "ZipCode" => Int32.Parse(value),
                "SupportStatus" => ConvertToBool(value),
                _ => value
            };
        }
        catch (Exception e)
        {
            return null;
        }
    }
    
    public void SetProperty(string propertyName, string value)
    {
        var property = this.GetType().GetProperty(propertyName);
        if (property != null)
        {
            property.SetValue(this, ConvertTo(propertyName, value));
        }
    }
    
}

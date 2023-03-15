namespace DAL.Models;

/// <summary>
/// The model for the user_work_preferences table.<br/>
/// A kinda pointless model seeing as its just a string with a name, but it's here for consistency.<br/>
/// </summary>
public class UserJobPreference
{
    public string? UserPreferencesText { get; set; }
}
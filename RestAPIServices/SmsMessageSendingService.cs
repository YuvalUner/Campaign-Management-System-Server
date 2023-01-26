using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telesign;

namespace RestAPIServices;

/// <summary>
/// An enum for country codes for phone numbers.
/// Format: CountryName = CountryCode
/// </summary>
public enum CountryCodes
{
    Israel = 972
}

/// <summary>
/// A class that transforms a phone number to a specific format.
/// Uses the builder pattern to allow for easy chaining of transformations.
/// </summary>
internal class PhoneNumberTransformer
{
    private delegate string PhoneNumberTransformerDelegate(string? phoneNumber);
    private readonly List<PhoneNumberTransformerDelegate> _transformers = new();
    
    public static PhoneNumberTransformer Create()
    {
        return new PhoneNumberTransformer();
    }
    
    private PhoneNumberTransformer AddTransformer(PhoneNumberTransformerDelegate transformer)
    {
        _transformers.Add(transformer);
        return this;
    }
    
    public string Transform(string? phoneNumber)
    {
        foreach (var transformer in _transformers)
        {
            phoneNumber = transformer(phoneNumber);
        }
        return phoneNumber;
    }
    
    public PhoneNumberTransformer CleanPhoneNumber()
    {
        string PhoneNumber(string? phoneNumber)
        {
            if (phoneNumber == null) return "";
            return phoneNumber.Replace(" ", "").Replace("-", "");
        }

        return AddTransformer(PhoneNumber);
    }

    public PhoneNumberTransformer AddCountryCode(CountryCodes code, bool keepZero)
    {
        string CountryCode(string? phoneNumber)
        {
            if (phoneNumber == null) return "";
            // Depending on the SMS service, some want to keep the initial 0 in numbers, and some do not.
            if (keepZero) return (int) code + phoneNumber;
            return (int) code + phoneNumber.Substring(1);
        }

        return AddTransformer(CountryCode);
    }

}

public class SmsMessageSendingService : ISmsMessageSendingService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmsMessageSendingService> _logger;
    
    public SmsMessageSendingService(IConfiguration configuration, ILogger<SmsMessageSendingService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendSmsMessageAsync(string phoneNumber, string message)
    {
        try
        {
            MessagingClient messagingClient = new MessagingClient(_configuration["SmsConfiguration:Telesign:CustomerId"],
                _configuration["SmsConfiguration:Telesign:ApiKey"]);
            RestClient.TelesignResponse telesignResponse = await messagingClient.MessageAsync(phoneNumber, message, "ARN");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while sending sms message");
        }
    }

    public async Task SendUserJoinedSmsAsync(string? userName, string? campaignName, string phoneNumber, CountryCodes countryCode)
    {
        string message = $"User {userName} joined campaign {campaignName}";
        phoneNumber = PhoneNumberTransformer.Create().CleanPhoneNumber().AddCountryCode(countryCode, true)
            .Transform(phoneNumber);
        await SendSmsMessageAsync(phoneNumber, message);
        
    }
    
    public async Task SendPhoneVerificationCodeAsync(string? phoneNumber, string code, CountryCodes countryCode)
    {
        string message = $"Your verification code is {code}";
        phoneNumber = PhoneNumberTransformer.Create().CleanPhoneNumber().AddCountryCode(countryCode, true)
            .Transform(phoneNumber);
        await SendSmsMessageAsync(phoneNumber, message);
    }
}
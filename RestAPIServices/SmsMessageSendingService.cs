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

public enum CallStatus
{
    Success = 1,
    Failed = 0
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

    private static List<int> TelesignNotFailedStatusCodes = new() { 290, 291, 292, 295, 251, 250, 200, 201, 203 };

    public SmsMessageSendingService(IConfiguration configuration, ILogger<SmsMessageSendingService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<int> SendSmsMessageAsync(string phoneNumber, string message)
    {
        try
        {
            MessagingClient messagingClient = new MessagingClient(
                _configuration["SmsConfiguration:Telesign:CustomerId"],
                _configuration["SmsConfiguration:Telesign:ApiKey"]);
            RestClient.TelesignResponse telesignResponse =
                await messagingClient.MessageAsync(phoneNumber, message, "ARN");
            return telesignResponse.StatusCode;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while sending sms message");
            return -1;
        }
    }

    public async Task SendUserJoinedSmsAsync(string? userName, string? campaignName, string phoneNumber,
        CountryCodes countryCode)
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

    public async Task SendRoleAssignedSmsAsync(string? roleName, string? campaignName, string phoneNumber,
        CountryCodes countryCode)
    {
        string message = $"You were assigned the role {roleName} in campaign {campaignName}";
        phoneNumber = PhoneNumberTransformer.Create().CleanPhoneNumber().AddCountryCode(countryCode, true)
            .Transform(phoneNumber);
        await SendSmsMessageAsync(phoneNumber, message);
    }

    public async Task SendJobAssignedSmsAsync(string? jobName, DateTime? jobStartTime, DateTime? jobEndTime,
        string? location, string phoneNumber, CountryCodes countryCode)
    {
        string message = $"You were assigned the job {jobName} from {jobStartTime} to {jobEndTime} at {location}";
        phoneNumber = PhoneNumberTransformer.Create().CleanPhoneNumber().AddCountryCode(countryCode, true)
            .Transform(phoneNumber);
        await SendSmsMessageAsync(phoneNumber, message);
    }

    public async Task SendJobUnAssignedSmsAsync(string? jobName, string? location, string phoneNumber,
        CountryCodes countryCode)
    {
        string message = $"You were unassigned from the job {jobName} at {location}";
        phoneNumber = PhoneNumberTransformer.Create().CleanPhoneNumber().AddCountryCode(countryCode, true)
            .Transform(phoneNumber);
        await SendSmsMessageAsync(phoneNumber, message);
    }

    public async Task<CallStatus> SendFreeTextSmsAsync(string? message, string phoneNumber, CountryCodes countryCode)
    {
        phoneNumber = PhoneNumberTransformer.Create().CleanPhoneNumber().AddCountryCode(countryCode, true)
            .Transform(phoneNumber);
        var res = await SendSmsMessageAsync(phoneNumber, message);
        return TelesignNotFailedStatusCodes.Contains(res) ? CallStatus.Success : CallStatus.Failed;
    }

    public async Task SendAddedEventParticipationSmsAsync(string? eventName,
        string? eventLocation, DateTime? startTime, DateTime? endTime, string? phoneNumber, CountryCodes countryCode)
    {
        phoneNumber = PhoneNumberTransformer.Create().CleanPhoneNumber().AddCountryCode(countryCode, true)
            .Transform(phoneNumber);
        string message;
        if (startTime != null && endTime != null)
        {
            message = $"You were added to the event {eventName} at {eventLocation} from {startTime} to {endTime}";
        }
        else if (startTime != null)
        {
            message = $"You were added to the event {eventName} at {eventLocation} from {startTime}";
        }
        else if (endTime != null)
        {
            message = $"You were added to the event {eventName} at {eventLocation} until {endTime}";
        }
        else
        {
            message = $"You were added to the event {eventName} at {eventLocation}";
        }
        await SendSmsMessageAsync(phoneNumber, message);
    }

    public async Task SendEventCreatedForUserSmsAsync(string? eventName,
        string? eventLocation, DateTime? startTime, DateTime? endTime, string creatorName,
        string? phoneNumber, CountryCodes countryCode)
    {
        phoneNumber = PhoneNumberTransformer.Create().CleanPhoneNumber().AddCountryCode(countryCode, true)
            .Transform(phoneNumber);
        string message;
        if (startTime != null && endTime != null)
        {
            message = $"The event {eventName} at {eventLocation} from {startTime} to {endTime} was created by" +
                      $" {creatorName} and added to your schedule";
        }
        else if (startTime != null)
        {
            message = $"The event {eventName} at {eventLocation} from {startTime} was created by {creatorName} and added" +
                      $" to your schedule";
        }
        else if (endTime != null)
        {
            message = $"The event {eventName} at {eventLocation} until {endTime} was created by {creatorName} and added" +
                      $" to your schedule";
        }
        else
        {
            message = $"The event {eventName} at {eventLocation} was created by {creatorName} and added to your schedule";
        }
        await SendSmsMessageAsync(phoneNumber, message);
    }

    public async Task SendEventDeletedMessageAsync(string? eventName, string? eventLocation,
        DateTime? startTime, DateTime? endTime, string? phoneNumber, string? senderName, CountryCodes countryCode)
    {
        phoneNumber = PhoneNumberTransformer.Create().CleanPhoneNumber().AddCountryCode(countryCode, true)
            .Transform(phoneNumber);
        
        string message;
        if (startTime != null && endTime != null)
        {
            message = $"{senderName}: \n The event {eventName} at {eventLocation} from {startTime} to {endTime} was deleted";
        }
        else if (startTime != null)
        {
            message = $"{senderName}: \n The event {eventName} at {eventLocation} from {startTime} was deleted";
        }
        else if (endTime != null)
        {
            message = $"{senderName}: \n The event {eventName} at {eventLocation} until {endTime} was deleted";
        }
        else
        {
            message = $"{senderName}: \n The event {eventName} at {eventLocation} was deleted";
        }
        await SendSmsMessageAsync(phoneNumber, message);
    }

    public async Task SendEventUpdatedMessageAsync(string? eventName, string? eventLocation,
        DateTime? startTime, DateTime? endTime, string? phoneNumber, string? senderName, CountryCodes countryCode)
    {
        phoneNumber = PhoneNumberTransformer.Create().CleanPhoneNumber().AddCountryCode(countryCode, true)
            .Transform(phoneNumber);
        
        string message = $"{senderName}'s event {eventName} updated \n";
        if (eventLocation != null)
        {
            message += $"Location moved to: {eventLocation} \n";
        }
        if (startTime != null)
        {
            message += $"Start time changed to: {startTime} \n";
        }
        if (endTime != null)
        {
            message += $"End time changed to: {endTime} \n";
        }
        await SendSmsMessageAsync(phoneNumber, message);
    }
}
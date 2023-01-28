﻿using DAL.DbAccess;

namespace API.Utils;

public static class ErrorMessages
{
    public const string JobNameRequired = "Job name is required";
    public const string UserNotFound = "User not found";
    public const string RoleNotFound = "Role not found";
    public const string RoleAlreadyExists = "Role already exists";
    public const string UserAlreadyExists = "User already exists";
    public const string JobTypeAlreadyExists = "Job type already exists";
    public const string JobTypeNotFound = "Job type not found";
    public const string TooManyRoles = "Too many roles in campaign";
    public const string UserAlreadyVerified = "User with this ID is already verified, this may require a report to the admin";
    public const string PermissionDoesNotExist = "Permission does not exist";
    public const string AlreadyHasPermission = "User already has this permission";
    public const string CityNotFound = "City not found";
    public const string RoleNameRequired = "Role name is required";
    public const string NameMustNotBeBuiltIn = "Name must not be a built-in name";
    public const string RequestedValueNotFound = "Requested value not found";
    public const string VerificationStatusError = "Verification status error - you must be verified to do this";
    public const string PhoneNumberNotFound = "Phone number not found";
    public const string VerificationCodeNotFound = "Verification code not found, please request a new one";
    public const string VerificationCodeExpired = "Verification code expired, please request a new one";
    public const string AuthorizationError = "Authorization error - you are not authorized to do this";
    public const string PermissionError = "Permission error - you do not have permission to do this";
    public const string PermissionOrAuthorizationError = "Permission or authorization error - you do not have permission to do this";
    public const string CityNameRequired = "City name is required when filtering for non-municipal campaigns";
    public const string CampaignNameRequired = "Campaign name is required";
    public const string CampaignNotFound = "Campaign not found";
    public const string JobNotFound = "Job not found";
    public const string CampaignNameOrCityNameRequired = "Campaign name or city name is required";
    public const string JobTypeRequired = "Job type is required";
    public const string TooManyJobTypes = "Too many job types in campaign";
    public const string DuplicateVerification = "You can not verify a user more than once";
    public const string VerificationFailed = "Verification failed, please check your info and try again";

    public static string FormatErrorMessage(string message, CustomStatusCode customStatusCode)
    {
        return $"Error Num {(int)customStatusCode} - {message}";
    }
}
using System.Collections;
using System.Data;
using System.Text;
using System.Text.Json.Serialization;
using API.Utils;
using DAL.DbAccess;
using DAL.Models;
using DAL.Services.Interfaces;
using ExcelDataReader;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static API.Utils.ErrorMessages;

namespace API.Controllers;

#region File Upload model

public class FileUploadParams
{
    public List<string>? ColumnNames { get; set; }
    public List<string>? PropertyNames { get; set; }
    public IFormFile? File { get; set; }
}

#endregion

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class CustomVotersLedgerController : Controller
{
    #region Private fields and constructor

    private readonly ICustomVotersLedgerService _customVotersLedgerService;
    private readonly ILogger<CustomVotersLedgerController> _logger;

    public CustomVotersLedgerController(ICustomVotersLedgerService customVotersLedgerService,
        ILogger<CustomVotersLedgerController> logger)
    {
        _customVotersLedgerService = customVotersLedgerService;
        _logger = logger;
    }
    
    #endregion

    #region Custom Voters Ledgers
    
    
    /// <summary>
    /// Creates a new custom voters ledger for the given campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign to add a ledger to.</param>
    /// <param name="customVotersLedger">An instance of <see cref="CustomVotersLedger"/> with the name of the new ledger.</param>
    /// <returns></returns>
    [HttpPost("create/{campaignGuid:guid}")]
    public async Task<IActionResult> CreateCustomLedger(Guid campaignGuid,
        [FromBody] CustomVotersLedger customVotersLedger)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CustomVotersLedger,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            customVotersLedger.CampaignGuid = campaignGuid;
            
            var (statusCode, guid) = await _customVotersLedgerService.CreateCustomVotersLedger(customVotersLedger);

            return statusCode switch
            {
                CustomStatusCode.CampaignNotFound => NotFound(FormatErrorMessage(CampaignNotFound, statusCode)),
                _ => Ok(new { LedgerGuid = guid })
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to create custom ledger");
            return StatusCode(500, "Error while trying to create custom ledger");
        }
    }
    
    /// <summary>
    /// Gets all of a campaign's custom voters ledgers.
    /// </summary>
    /// <param name="campaignGuid"></param>
    /// <returns></returns>
    [HttpGet("get-for-campaign/{campaignGuid:guid}")]
    public async Task<IActionResult> GetCustomLedgersForCampaign(Guid campaignGuid)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CustomVotersLedger,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var customVotersLedgers = await _customVotersLedgerService.GetCustomVotersLedgersByCampaignGuid(campaignGuid);

            return Ok(customVotersLedgers);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to get custom ledgers for campaign");
            return StatusCode(500, "Error while trying to get custom ledgers for campaign");
        }
    }
    
    
    /// <summary>
    /// Deletes an existing custom voters ledger for the given campaign.<br/>
    /// This will also delete all data associated with the ledger.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign to delete for.</param>
    /// <param name="ledgerGuid">Guid of the ledger to delete.</param>
    /// <returns></returns>
    [HttpDelete("delete/{campaignGuid:guid}/{ledgerGuid:guid}")]
    public async Task<IActionResult> DeleteCustomLedger(Guid campaignGuid, Guid ledgerGuid)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CustomVotersLedger,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var statusCode = await _customVotersLedgerService.DeleteCustomVotersLedger(ledgerGuid, campaignGuid);

            return statusCode switch
            {
                CustomStatusCode.LedgerNotFound => NotFound(FormatErrorMessage(LedgerNotFound, statusCode)),
                CustomStatusCode.CampaignNotFound => NotFound(FormatErrorMessage(CampaignNotFound, statusCode)),
                CustomStatusCode.BoundaryViolation => BadRequest(FormatErrorMessage(AuthorizationError, statusCode)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to delete custom ledger");
            return StatusCode(500, "Error while trying to delete custom ledger");
        }
    }
    
    /// <summary>
    /// Updates the name of an existing custom voters ledger for the given campaign.
    /// </summary>
    /// <param name="campaignGuid">Guid of the campaign the ledger belongs to.</param>
    /// <param name="customVotersLedger">An instance of <see cref="CustomVotersLedger"/> with the name and
    /// ledgerGuid properties not null.</param>
    /// <returns></returns>
    [HttpPut("update/{campaignGuid:guid}")]
    public async Task<IActionResult> UpdateCustomLedger(Guid campaignGuid,
        [FromBody] CustomVotersLedger customVotersLedger)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CustomVotersLedger,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }

            var statusCode = await _customVotersLedgerService.UpdateCustomVotersLedger(customVotersLedger, campaignGuid);

            return statusCode switch
            {
                CustomStatusCode.LedgerNotFound => NotFound(FormatErrorMessage(LedgerNotFound, statusCode)),
                CustomStatusCode.CampaignNotFound => NotFound(FormatErrorMessage(CampaignNotFound, statusCode)),
                CustomStatusCode.BoundaryViolation => BadRequest(FormatErrorMessage(AuthorizationError, statusCode)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to update custom ledger");
            return StatusCode(500, "Error while trying to update custom ledger");
        }
    }
    
    #endregion
    
    #region Custom Voters Ledger Rows

    [HttpPost("add-row/{campaignGuid:guid}/{ledgerGuid:guid}")]
    public async Task<IActionResult> AddRowToCustomLedger(Guid campaignGuid, Guid ledgerGuid,
        [FromBody] CustomVotersLedgerContent content)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.VotersLedger,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }
            
            if (content.Identifier == null)
            {
                return BadRequest(FormatErrorMessage(IdentifierMissing, CustomStatusCode.ValueCanNotBeNull));
            }
            
            var statusCode = await _customVotersLedgerService.AddCustomVotersLedgerRow(content, ledgerGuid);
            
            return statusCode switch
            {
                CustomStatusCode.LedgerNotFound => NotFound(FormatErrorMessage(LedgerNotFound, statusCode)),
                CustomStatusCode.DuplicateKey => BadRequest(FormatErrorMessage(RowAlreadyExists, statusCode)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to add row to custom ledger");
            return StatusCode(500, "Error while trying to add row to custom ledger");
        }
    }
    
    [HttpDelete("delete-row/{campaignGuid:guid}/{ledgerGuid:guid}")]
    public async Task<IActionResult> DeleteRowFromCustomLedger(Guid campaignGuid, Guid ledgerGuid, [FromQuery] int? rowId)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.VotersLedger,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }
            
            if (rowId == null)
            {
                return BadRequest(FormatErrorMessage(IdentifierMissing, CustomStatusCode.ValueCanNotBeNull));
            }

            var statusCode = await _customVotersLedgerService.DeleteCustomVotersLedgerRow(ledgerGuid, rowId.Value);
            
            return statusCode switch
            {
                CustomStatusCode.LedgerNotFound => NotFound(FormatErrorMessage(LedgerNotFound, statusCode)),
                CustomStatusCode.LedgerRowNotFound => NotFound(FormatErrorMessage(LedgerRowNotFound, statusCode)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to delete row from custom ledger");
            return StatusCode(500, "Error while trying to delete row from custom ledger");
        }
    }
    
    [HttpPut("update-row/{campaignGuid:guid}/{ledgerGuid:guid}")]
    public async Task<IActionResult> UpdateRowInCustomLedger(Guid campaignGuid, Guid ledgerGuid,
        [FromBody] CustomVotersLedgerContent content)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.VotersLedger,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }
            
            if (content.Identifier == null)
            {
                return BadRequest(FormatErrorMessage(IdentifierMissing, CustomStatusCode.ValueCanNotBeNull));
            }
            
            var statusCode = await _customVotersLedgerService.UpdateCustomVotersLedgerRow(content, ledgerGuid);
            
            return statusCode switch
            {
                CustomStatusCode.LedgerNotFound => NotFound(FormatErrorMessage(LedgerNotFound, statusCode)),
                CustomStatusCode.LedgerRowNotFound => NotFound(FormatErrorMessage(LedgerRowNotFound, statusCode)),
                _ => Ok()
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to update row in custom ledger");
            return StatusCode(500, "Error while trying to update row in custom ledger");
        }
    }

    [HttpGet("filter/{campaignGuid:guid}/{ledgerGuid:guid}")]
    public async Task<IActionResult> FilterCustomLedger(Guid campaignGuid, Guid ledgerGuid,
        [FromQuery] CustomLedgerFilterParams filter)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.VotersLedger,
                        PermissionType = PermissionTypes.View
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }
            
            var res = await _customVotersLedgerService.FilterCustomVotersLedger(ledgerGuid, filter);

            return Ok(res);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to filter custom ledger");
            return StatusCode(500, "Error while trying to filter custom ledger");
        }
    }
    
    #endregion

    #region Custom Voters Ledger Import
    #region Helper Functions

    /// <summary>
    /// This function handles checking the validity of the file uploaded by the user.
    /// </summary>
    /// <param name="columnMappings"></param>
    /// <param name="file"></param>
    /// <returns>True and a DataTable if the file is of a valid format, false and null otherwise.</returns>
    private (bool, DataTable?) CheckFormatValidity(IEnumerable<ColumnMapping> columnMappings, IFormFile file)
    {
        #region Creating a reader for the file
        
        IExcelDataReader excelReader;
        // Create a new ExcelReader based on the file type, or return false if the file type is not supported
        if (file.FileName.EndsWith(".xls"))
        {
            excelReader = ExcelReaderFactory.CreateBinaryReader(file.OpenReadStream(), new ExcelReaderConfiguration()
            {
                FallbackEncoding = Encoding.UTF8
            });
        }
        else if (file.FileName.EndsWith(".xlsx"))
        {
            excelReader = ExcelReaderFactory.CreateOpenXmlReader(file.OpenReadStream(), new ExcelReaderConfiguration()
            {
                FallbackEncoding = Encoding.UTF8
            });
        }
        else
        {
            return (false, null);
        }
        
        #endregion

        #region Creating a DataTable from the file
        
        // Read the first sheet of the file
        var ledger = excelReader.AsDataSet(new ExcelDataSetConfiguration()
        {
            ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
            {
                UseHeaderRow = true
            }
        });
        if (ledger == null)
        {
            return (false, null);
        }
        excelReader.Close();
        DataTable table = ledger.Tables[0];
        
        #endregion

        #region Verifying column validity
        
        // Check if the file contains the identifier column
        string? IdColumnName = columnMappings.FirstOrDefault(x => x.PropertyName == PropertyNames.Identifier)?.ColumnName;
        if (IdColumnName == null)
        {
            return (false, null);
        }
        if (!table.Columns.Contains(IdColumnName))
        {
            return (false, null);
        }
        
        // Check that the file contains at least one row
        if (table.Rows.Count == 0)
        {
            return (false, null);
        }

        // Check that the file contains all the columns specified in the column mappings
        foreach (var mapping in columnMappings)
        {
            if (string.IsNullOrWhiteSpace(mapping.ColumnName)
                || string.IsNullOrWhiteSpace(mapping.PropertyName)
                || !table.Columns.Contains(mapping.ColumnName))
            {
                return (false, null);
            }
        }
        
        #endregion
        
        return (true, table);
    }

    /// <summary>
    /// This function performs the mapping of each row of the excel file to the CustomVotersLedgerContent model
    /// </summary>
    /// <param name="columnMappings">A list of column mappings, telling the function which column in the table
    /// to map to each property.</param>
    /// <param name="table">The table to adapt to models.</param>
    /// <returns>true and a list of <see cref="CustomVotersLedgerContent"/> if the table itself is entirely valid,
    /// false and null otherwise.</returns>
    private (bool, List<CustomVotersLedgerContent>?) excelToModel(IEnumerable<ColumnMapping> columnMappings, DataTable table)
    {
        var res = new List<CustomVotersLedgerContent>();
        var sanitizer = new HtmlSanitizer();
        // Go over the rows of the table and map each row to a CustomVotersLedgerContent model
        foreach (DataRow row in table.Rows)
        {
            var content = new CustomVotersLedgerContent();
            foreach (var mapping in columnMappings)
            {
                // Sanitize the value before setting it in the model, to prevent XSS attacks
                var sanitizedValue = sanitizer.Sanitize(row[mapping.ColumnName].ToString());
                content.SetProperty(mapping.PropertyName, sanitizedValue);
            }
            if (content.Identifier == null)
            {
                return (false, null);
            }
            res.Add(content);
        }
        
        // After doing the mapping, check that no two rows have the same identifier
        var identifiers = res.Select(x => x.Identifier).ToList();
        if (identifiers.Count != identifiers.Distinct().Count())
        {
            return (false, null);
        }

        return (true, res);
    }
    
    #endregion

    [HttpPost("import/{campaignGuid:guid}/{ledgerGuid:guid}")]
    public async Task<IActionResult> ImportLedger(Guid campaignGuid, Guid ledgerGuid,
        [FromForm] FileUploadParams fileUploadParams)
    {
        try
        {
            if (!CombinedPermissionCampaignUtils.IsUserAuthorizedForCampaignAndHasPermission(HttpContext, campaignGuid,
                    new Permission()
                    {
                        PermissionTarget = PermissionTargets.CustomVotersLedger,
                        PermissionType = PermissionTypes.Edit
                    }))
            {
                return Unauthorized(FormatErrorMessage(PermissionOrAuthorizationError,
                    CustomStatusCode.PermissionOrAuthorizationError));
            }
            
            if (fileUploadParams.File == null 
                || fileUploadParams.PropertyNames == null 
                || !fileUploadParams.PropertyNames.Any()
                || fileUploadParams.ColumnNames == null
                || !fileUploadParams.ColumnNames.Any()
                || fileUploadParams.ColumnNames.Count != fileUploadParams.PropertyNames.Count
               )
            {
                return BadRequest(FormatErrorMessage(InvalidFile, CustomStatusCode.InvalidFile));
            }
            
            var ledgers = await _customVotersLedgerService.GetCustomVotersLedgersByCampaignGuid(campaignGuid);
            if (ledgers.All(x => x.LedgerGuid != ledgerGuid))
            {
                return NotFound(FormatErrorMessage(LedgerNotFound, CustomStatusCode.LedgerNotFound));
            }

            var file = fileUploadParams.File;
            var columnMappings = new List<ColumnMapping>();

            for (int i = 0; i < fileUploadParams.ColumnNames.Count; i++)
            {
                columnMappings.Add(new ColumnMapping()
                {
                    ColumnName = fileUploadParams.ColumnNames[i],
                    PropertyName = fileUploadParams.PropertyNames[i]
                });
            }

            var (formatValid, table) = CheckFormatValidity(columnMappings, file);
            if (!formatValid || table == null)
            {
                return BadRequest(FormatErrorMessage(InvalidFile, CustomStatusCode.InvalidFile));
            }
            var (tableValid, contentList) = excelToModel(columnMappings, table);
            if (!tableValid || contentList == null)
            {
                return BadRequest(FormatErrorMessage(InvalidFile, CustomStatusCode.InvalidFile));
            }
            
            var contentListArray = contentList.ToArray();
            
            // Split the content list into chunks of 1000 elements each, and import each chunk separately
            // This is done to avoid sending a request with a body that is too large, as these files can
            // be massive.
            int chunkSize = 1000;
            var chunks = contentList.Chunk(chunkSize);
            foreach (var chunk in chunks)
            {
                await _customVotersLedgerService.ImportLedger(ledgerGuid, JsonConvert.SerializeObject(chunk));
            }

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while trying to import custom ledger");
            return StatusCode(500, "Error while trying to import custom ledger");
        }
    }

    #endregion
    
}
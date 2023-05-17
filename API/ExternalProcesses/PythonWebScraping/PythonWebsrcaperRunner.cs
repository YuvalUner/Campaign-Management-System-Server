using System.Diagnostics;
using API.Models;
using Newtonsoft.Json;

namespace API.ExternalProcesses.PythonWebScraping;

public class PythonWebsrcaperRunner
{
    private readonly string? _pythonPath;
    private readonly string? _pythonScriptLocation;
    
    public PythonWebsrcaperRunner(IConfiguration configuration)
    {
        _pythonPath = configuration["PythonPath"];
        _pythonScriptLocation = configuration["PythonWebScrapingScriptLocation"];
    }
    
    public async Task<TweetsCollection?> RunPythonScript(string targetName, string? targetTwitterHandle, int? maxDays)
    {
        if (string.IsNullOrWhiteSpace(targetTwitterHandle))
        {
            targetTwitterHandle = "";
        }

        if (maxDays is null or < 0)
        {
            // Default to 30 days - the python script will default to it anyway.
            maxDays = 30;
        }
        
        var process = new Process
        {
            StartInfo =
            {
                FileName = _pythonPath,
                Arguments = $"{_pythonScriptLocation} {targetName} {targetTwitterHandle} {maxDays}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };

        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync();
        process.WaitForExit();

        try
        {
            var tweetsCollection = JsonConvert.DeserializeObject<TweetsCollection>(output);
            return tweetsCollection;
        }
        catch (Exception e)
        {
            return null;
        }
    }
}
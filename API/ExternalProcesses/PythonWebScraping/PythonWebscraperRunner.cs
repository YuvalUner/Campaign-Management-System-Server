using System.Diagnostics;
using API.Models;
using Newtonsoft.Json;

namespace API.ExternalProcesses.PythonWebScraping;

public class PythonWebscraperRunner : IPythonWebscraperRunner
{
    private readonly string? _pythonPath;
    private readonly string? _pythonScriptLocation;
    
    public PythonWebscraperRunner(IConfiguration configuration)
    {
        _pythonPath = configuration["PythonPath"];
        string? pythonScriptLocation = configuration["PythonWebScrapingScriptLocation"];
        string exeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        int apiDllBasePathIndex = exeFilePath.IndexOf("API", StringComparison.Ordinal);
        string apiDllBasePath = exeFilePath.Substring(0, apiDllBasePathIndex + 3);
        _pythonScriptLocation = apiDllBasePath + pythonScriptLocation;
    }
    
    public async Task<TweetsCollection?> RunPythonScript(string targetName, string? targetTwitterHandle, int? maxDays)
    {
        if (string.IsNullOrWhiteSpace(targetTwitterHandle))
        {
            targetTwitterHandle = "";
        }

        if (maxDays is null or < 1)
        {
            // Default to 30 days - the python script will default to it anyway.
            maxDays = 30;
        }
        
        var process = new Process
        {
            StartInfo =
            {
                FileName = _pythonPath,
                Arguments = $"{_pythonScriptLocation} \"{targetName}\" \"{targetTwitterHandle}\" {maxDays}",
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
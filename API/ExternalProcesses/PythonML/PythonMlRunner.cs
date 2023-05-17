using System.Diagnostics;
using API.Models;
using Newtonsoft.Json;

namespace API.ExternalProcesses.PythonML;


public class PythonMlRunner : IPythonMlRunner
{
    private readonly string? _pythonPath;
    private readonly string? _pythonScriptLocation;
    private readonly string? _jsonFileDirectory;
    static int _index = 0;
    static object _indexLock = new object();
    
    public PythonMlRunner(IConfiguration configuration)
    {
        _pythonPath = configuration["PythonPath"];
        string? pythonScriptLocation = configuration["PythonMlScriptLocation"];
        string? jsonFileDirectory = configuration["JsonFileDirectory"];
        string exeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        int apiDllBasePathIndex = exeFilePath.IndexOf("API", StringComparison.Ordinal);
        string apiDllBasePath = exeFilePath.Substring(0, apiDllBasePathIndex + 3);
        _pythonScriptLocation = apiDllBasePath + pythonScriptLocation;
        _jsonFileDirectory = apiDllBasePath + jsonFileDirectory;
    }
    
    public async Task<CombinedTextsList?> RunPythonScript(string[] articles, string[] targetTweets,
        string[] tweetsAboutTarget)
    {

        // Lock the index, to prevent multiple server threads from using the same index.
        string filePath;
        lock (_indexLock)
        {
            filePath = _jsonFileDirectory + "\\" + _index.ToString() + ".json";
            _index++;
        }

        var combinedTexts = new
        {
            articles,
            targetTweets,
            tweetsAboutTarget
        };
        var json = JsonConvert.SerializeObject(combinedTexts);
        await File.WriteAllTextAsync(filePath, json);
        
        var process = new Process
        {
            StartInfo =
            {
                FileName = _pythonPath,
                Arguments = $"{_pythonScriptLocation} {filePath}",
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
            File.Delete(filePath);
            var combinedTextsList = JsonConvert.DeserializeObject<CombinedTextsList>(output);
            return combinedTextsList;
        }
        catch (Exception e)
        {
            return null;
        }
    }

}
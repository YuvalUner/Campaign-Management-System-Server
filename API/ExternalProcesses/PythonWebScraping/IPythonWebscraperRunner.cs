using API.Models;

namespace API.ExternalProcesses.PythonWebScraping;

public interface IPythonWebscraperRunner
{
    Task<TweetsCollection?> RunPythonScript(string targetName, string? targetTwitterHandle, int? maxDays);
}
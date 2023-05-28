using API.Models;

namespace API.ExternalProcesses.PythonML;

public interface IPythonMlRunner
{
    Task<CombinedTextsList> RunPythonScript(List<string>? articles, List<string>? targetTweets,
        List<string>? tweetsAboutTarget);
}
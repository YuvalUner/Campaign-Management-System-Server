using API.Models;

namespace API.ExternalProcesses.PythonML;

public interface IPythonMlRunner
{
    Task<CombinedTextsList> RunPythonScript(string[] articles, string[] targetTweets,
        string[] tweetsAboutTarget);
}
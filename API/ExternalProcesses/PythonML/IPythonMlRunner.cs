using API.Models;

namespace API.ExternalProcesses;

public interface IPythonMlRunner
{
    Task<CombinedTextsList> RunPythonScript(string[] articles, string[] tweets);
}
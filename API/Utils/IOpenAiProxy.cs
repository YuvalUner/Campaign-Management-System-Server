using Standard.AI.OpenAI.Models.Services.Foundations.ChatCompletions;

namespace API.Utils;

public interface IOpenAiProxy
{
    /// <summary>
    /// Sends a message to OpenAI and returns the response.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task<ChatCompletionMessage[]> SendChatMessage(string message);
}
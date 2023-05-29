using Standard.AI.OpenAI.Clients.OpenAIs;
using Standard.AI.OpenAI.Models.Configurations;
using Standard.AI.OpenAI.Models.Services.Foundations.ChatCompletions;

namespace API.Utils;

public class OpenAiProxy : IOpenAiProxy
{
    readonly OpenAIClient openAIClient;

    public OpenAiProxy(IConfiguration configuration)
    {
        //initialize the configuration with api key and sub
        var openAIConfigurations = new OpenAIConfigurations
        {
            ApiKey = configuration["OpenAIApiKey"],
        };

        openAIClient = new OpenAIClient(openAIConfigurations);
    }

    static ChatCompletionMessage[] ToCompletionMessage(ChatCompletionChoice[] choices) => choices.Select(x => x.Message).ToArray();
    
    
    public Task<ChatCompletionMessage[]> SendChatMessage(string message)
    {
        var chatMsg = new ChatCompletionMessage() 
        { 
            Content = message, 
            Role = "assistant" 
        };
        return SendChatMessage(chatMsg);
    }
	
    /// <summary>
    /// Private method that handles the actual sending of the message to OpenAI.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    async Task<ChatCompletionMessage[]> SendChatMessage(ChatCompletionMessage message)
    {
        // Creating a list of messages to send to OpenAI, despite the fact that we are only sending one message,
        // because the API expects a list of messages.
        var sentMessages = new List<ChatCompletionMessage>();
        sentMessages.Add(message);

        var chatCompletion = new ChatCompletion
        {
            Request = new ChatCompletionRequest
            {
                Model = "gpt-3.5-turbo",
                Messages = sentMessages.ToArray(),
                Temperature = 0.2,
                CompletionsPerPrompt = 1,
                MaxTokens = 2000,
            }
        };

        var result = await openAIClient.ChatCompletions.SendChatCompletionAsync(chatCompletion);

        var choices = result.Response.Choices;

        var messages = ToCompletionMessage(choices);

        return messages;
    }
}
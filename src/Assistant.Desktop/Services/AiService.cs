using Assistant.Desktop.Entities;
using Assistant.Desktop.Mappers;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Assistant.Desktop.Services;

public class AiService(Kernel kernel) : IAiService
{
    public async Task<Message> GenerateAsync(List<Message> messages)
    {
        IChatCompletionService completionService = kernel.GetRequiredService<IChatCompletionService>();
        OpenAIPromptExecutionSettings openAiPromptExecutionSettings = new ()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        };
        
        ChatHistory history = messages.Where(x => x.Role != Message.ChatMessageRole.Tool).ToChatHistory();
        
        ChatMessageContent result = await completionService.GetChatMessageContentAsync(
            history,
            executionSettings: openAiPromptExecutionSettings,
            kernel: kernel);

        return new Message
        {
            Role = Message.ChatMessageRole.Assistant,
            Content = result.Content ?? "Error trying to get chat completion",
        };
    }

    public async Task<Message> GenerateChatTitleAsync(string message)
    {
        if (message.Length < 10)
        {
            return new Message() { Content = message, Role = Message.ChatMessageRole.Assistant, };
        }
        
        IChatCompletionService completionService = kernel.GetRequiredService<IChatCompletionService>();
        OpenAIPromptExecutionSettings openAiPromptExecutionSettings = new ()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        };

        ChatHistory history = new ChatHistory();
        history.AddSystemMessage("Based on the given user message, generate a 5-10 word title for the chat and say nothing else");
        history.AddUserMessage(message);
        
        ChatMessageContent result = await completionService.GetChatMessageContentAsync(
            history,
            executionSettings: openAiPromptExecutionSettings,
            kernel: kernel);
        
        string title = result.Content ?? message;
        title = title.TrimStart('"').TrimEnd('"');

        return new Message
        {
            Role = Message.ChatMessageRole.Assistant,
            Content = title
        };
    }
}

public interface IAiService
{
    Task<Message> GenerateAsync(List<Message> messages);
    Task<Message> GenerateChatTitleAsync(string message);
}

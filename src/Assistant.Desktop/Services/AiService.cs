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
}

public interface IAiService
{
    Task<Message> GenerateAsync(List<Message> messages);
}

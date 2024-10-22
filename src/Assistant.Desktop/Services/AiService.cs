using Assistant.Desktop.Mappers;
using Assistant.Desktop.ViewModels;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;

namespace Assistant.Desktop.Services;

public class AiService : IAiService
{
    private readonly Kernel _kernel;

    public AiService(Kernel kernel)
    {
        _kernel = kernel;
    }

    public async Task<ChatMessageViewModel> GenerateAsync(List<ChatMessageViewModel> chatMessages)
    {
        IChatCompletionService completionService = _kernel.GetRequiredService<IChatCompletionService>();
        OpenAIPromptExecutionSettings openAiPromptExecutionSettings = new ()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
        };

        ChatHistory history = chatMessages.ToChatHistory();

        ChatMessageContent result = await completionService.GetChatMessageContentAsync(
            history,
            executionSettings: openAiPromptExecutionSettings,
            kernel: _kernel);

        return new ChatMessageViewModel
        {
            Message = result?.Content ?? "Error trying to get chat completion", Role = "Assistant",
        };
    }
}

public interface IAiService
{
    Task<ChatMessageViewModel> GenerateAsync(List<ChatMessageViewModel> chatMessages);
}

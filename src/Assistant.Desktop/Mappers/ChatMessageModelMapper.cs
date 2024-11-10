using Assistant.Desktop.Entities;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI.Chat;

namespace Assistant.Desktop.Mappers;

public static class ChatMessageModelMapper
{
    private static ChatMessageRole ToChatMessageRole(this Message.ChatMessageRole role)
    {
        return role switch
        {
            Message.ChatMessageRole.User => ChatMessageRole.User,
            Message.ChatMessageRole.Assistant => ChatMessageRole.Assistant,
            Message.ChatMessageRole.Tool => ChatMessageRole.Tool,
            Message.ChatMessageRole.System => ChatMessageRole.System,
            _ => ChatMessageRole.User,
        };
    }

    public static ChatHistory ToChatHistory(this IEnumerable<Message> chatMessages)
    {
        ChatHistory history = new();

        foreach (Message chatMessage in chatMessages)
        {
            ChatMessageRole role = chatMessage.Role.ToChatMessageRole();
            switch (role)
            {
                case ChatMessageRole.System:
                    history.AddSystemMessage(chatMessage.Content);
                    break;
                case ChatMessageRole.User:
                    history.AddUserMessage(chatMessage.Content);
                    break;
                case ChatMessageRole.Assistant:
                    history.AddAssistantMessage(chatMessage.Content);
                    break;
            }
        }

        return history;
    }
}
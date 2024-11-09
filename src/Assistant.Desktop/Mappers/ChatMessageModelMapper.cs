using Assistant.Desktop.ViewModels;

using Microsoft.SemanticKernel.ChatCompletion;

using OpenAI.Chat;

namespace Assistant.Desktop.Mappers;

public static class ChatMessageModelMapper
{
    public static List<ChatMessage> ToChatMessageList(this List<ChatMessageViewModel> chatMessageViewModels)
    {
        return chatMessageViewModels.Select(x => x.ToChatMessage()).ToList();
    }
    
    public static ChatMessage ToChatMessage(this ChatMessageViewModel chatMessage)
    {
        ChatMessageRole role = chatMessage.Role.ToChatMessageRole();

        return role switch
        {
            ChatMessageRole.User => new UserChatMessage(chatMessage.Message),
            ChatMessageRole.Assistant => new AssistantChatMessage(chatMessage.Message),
            ChatMessageRole.Tool => new ToolChatMessage(chatMessage.Message),
            ChatMessageRole.System => new SystemChatMessage(chatMessage.Message),
            _ => new UserChatMessage(chatMessage.Message),
        };
    }
    
    public static ChatMessageRole ToChatMessageRole(this string role)
    {
        return role switch
        {
            "User" => ChatMessageRole.User,
            "Assistant" => ChatMessageRole.Assistant,
            "Tool" => ChatMessageRole.Tool,
            "System" => ChatMessageRole.System,
            _ => ChatMessageRole.User,
        };
    }

    public static ChatMessageRole ToChatMessageRole(this Entities.ChatMessage.ChatMessageRole role)
    {
        return role switch
        {
            Entities.ChatMessage.ChatMessageRole.User => ChatMessageRole.User,
            Entities.ChatMessage.ChatMessageRole.Assistant => ChatMessageRole.Assistant,
            Entities.ChatMessage.ChatMessageRole.Tool => ChatMessageRole.Tool,
            Entities.ChatMessage.ChatMessageRole.System => ChatMessageRole.System,
            _ => ChatMessageRole.User,
        };
    }

    public static ChatHistory ToChatHistory(this List<ChatMessageViewModel> chatMessageViewModels)
    {
        ChatHistory history = new ChatHistory();

        foreach (ChatMessageViewModel chatMessageViewModel in chatMessageViewModels)
        {
            ChatMessageRole role = chatMessageViewModel.Role.ToChatMessageRole();
            switch (role)
            {
                case ChatMessageRole.System:
                    history.AddSystemMessage(chatMessageViewModel.Message);
                    break;
                case ChatMessageRole.User:
                    history.AddUserMessage(chatMessageViewModel.Message);
                    break;
                case ChatMessageRole.Assistant:
                    history.AddAssistantMessage(chatMessageViewModel.Message);
                    break;
            }
        }
        
        return history;
    }

    public static ChatHistory ToChatHistory(this List<Entities.ChatMessage> chatMessages)
    {
        ChatHistory history = new ChatHistory();
        
        foreach (Entities.ChatMessage chatMessage in chatMessages)
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
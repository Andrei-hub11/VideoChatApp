using VideoChatApp.Contracts.DapperModels;
using VideoChatApp.Contracts.Response;

namespace VideoChatApp.Application.DTOMappers;

public static class MappingExtensionsMessage
{
    public static MessageResponseDTO ToDTO(this MessageMapping message)
    {
        return new MessageResponseDTO(
            message.MessageId,
            message.RoomId,
            message.MemberId,
            message.Content,
            message.SentAt
        );
    }

    public static IReadOnlyCollection<MessageResponseDTO> ToDTO(
        this IEnumerable<MessageMapping> messages
    )
    {
        return messages.Select(m => m.ToDTO()).ToList().AsReadOnly();
    }
}

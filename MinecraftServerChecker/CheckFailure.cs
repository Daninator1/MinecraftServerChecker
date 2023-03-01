using MinecraftClient.Scripting;

namespace MinecraftServerChecker;

public record CheckFailure
{
    public required ChatBot.DisconnectReason Reason { get; init; }
    public required string Message { get; init; }
}
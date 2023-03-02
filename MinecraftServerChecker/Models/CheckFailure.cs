using MinecraftClient.Scripting;

namespace MinecraftServerChecker.Models;

public record CheckFailure
{
    public required ChatBot.DisconnectReason Reason { get; init; }
    public required string Message { get; init; }
}
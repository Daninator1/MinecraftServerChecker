namespace MinecraftServerChecker.Models;

public record CheckSuccess
{
    public required bool IsOnlineMode { get; init; }
    public required IEnumerable<string> OnlinePlayers { get; init; }
}
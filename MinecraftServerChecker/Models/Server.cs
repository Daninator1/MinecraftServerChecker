namespace MinecraftServerChecker.Models;

public class Server
{
    public required string Ip { get; init; }
    public required ushort Port { get; init; }
}
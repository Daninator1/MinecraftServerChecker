using System.Reactive.Subjects;
using MinecraftClient;
using MinecraftClient.Protocol.Handlers.Forge;
using MinecraftClient.Protocol.ProfileKey;
using MinecraftClient.Protocol.Session;
using MinecraftClient.Scripting;
using OneOf;

namespace MinecraftServerChecker;

public class CustomMcClient : McClient
{
    public CustomMcClient(SessionToken session, PlayerKeyPair? playerKeyPair, string server_ip, ushort port, int protocolversion,
        ForgeInfo? forgeInfo) : base(session, playerKeyPair, server_ip, port, protocolversion, forgeInfo)
    {
        ConsoleInteractive.ConsoleReader.StopReadThread();
        Settings.InternalConfig.InteractiveMode = false;
    }

    public static readonly Subject<OneOf<CheckSuccess, CheckFailure>> JoinResultSubject = new();

    public override void OnGameJoined(bool isOnlineMode)
    {
        base.OnGameJoined(isOnlineMode);
        JoinResultSubject.OnNext(new CheckSuccess
        {
            IsOnlineMode = isOnlineMode,
            OnlinePlayers = Enumerable.Empty<string>(),
        });
    }

    public override void OnConnectionLost(ChatBot.DisconnectReason reason, string message)
    {
        base.OnConnectionLost(reason, message);
        JoinResultSubject.OnNext(new CheckFailure
        {
            Reason = reason,
            Message = message,
        });
    }
}
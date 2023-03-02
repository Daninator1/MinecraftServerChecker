using System.Reactive.Linq;
using MinecraftClient;
using MinecraftClient.Protocol;
using MinecraftClient.Protocol.Handlers.Forge;
using MinecraftClient.Protocol.ProfileKey;
using MinecraftClient.Protocol.Session;
using MinecraftServerChecker.Models;
using OneOf;

namespace MinecraftServerChecker;

public class ServerChecker
{
    private readonly UserLogin userLogin;
    private PlayerKeyPair? playerKeyPair;
    private SessionToken? session;

    public ServerChecker(UserLogin userLogin)
    {
        this.userLogin = userLogin;
    }

    public bool Login()
    {
        var loginResult = ProtocolHandler.GetLogin(this.userLogin.Email, this.userLogin.Password,
            Settings.MainConfigHealper.MainConfig.GeneralConfig.LoginType.microsoft,
            out this.session);

        if (loginResult != ProtocolHandler.LoginResult.Success) return false;

        this.playerKeyPair = KeyUtils.GetNewProfileKeys(this.session.ID);
        if (this.playerKeyPair is null) return false;

        return true;
    }

    public async Task<OneOf<CheckSuccess, CheckFailure>> CheckServerAsync(Server server)
    {
        if (this.session is null || this.playerKeyPair is null)
            throw new InvalidOperationException("Invalid session or player key pair (did you forget to call Login() first?)");

        var protocolVersion = 0;
        ForgeInfo? forgeInfo = null;

        ProtocolHandler.GetServerInfo(server.Ip, server.Port, ref protocolVersion, ref forgeInfo);

        var joinObservable = CustomMcClient.JoinResultSubject.FirstAsync().Timeout(TimeSpan.FromSeconds(10));
        var client = new CustomMcClient(this.session, this.playerKeyPair, server.Ip, server.Port, protocolVersion, forgeInfo);

        var joinResult = await joinObservable;

        return await joinResult.Match<Task<OneOf<CheckSuccess, CheckFailure>>>(
            async success =>
            {
                await Task.Delay(50);
                return success with { OnlinePlayers = client.GetOnlinePlayers() };
            },
            async failure => await Task.FromResult(failure));
    }
}
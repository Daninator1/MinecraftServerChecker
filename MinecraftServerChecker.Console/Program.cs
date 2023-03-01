using System.Reactive.Linq;
using Microsoft.Extensions.Configuration;
using MinecraftClient;
using MinecraftClient.Protocol;
using MinecraftClient.Protocol.Handlers.Forge;
using MinecraftClient.Protocol.ProfileKey;
using MinecraftServerChecker.Console;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var login = configuration.GetSection("UserLogin").Get<UserLogin>()!;

var result = ProtocolHandler.GetLogin(
    login.Email,
    login.Password,
    Settings.MainConfigHealper.MainConfig.GeneralConfig.LoginType.microsoft,
    out var session);

var playerKeyPair = KeyUtils.GetNewProfileKeys(session.ID);

var serverIp = args[0];
var serverPort = ushort.Parse(args[1]);
var protocolVersion = 0;
ForgeInfo? forgeInfo = null;

ProtocolHandler.GetServerInfo(serverIp, serverPort, ref protocolVersion, ref forgeInfo);

var joinObservable = CustomMcClient.JoinResultSubject.FirstAsync().Timeout(TimeSpan.FromSeconds(10));
var client = new CustomMcClient(session, playerKeyPair, serverIp, serverPort, protocolVersion, forgeInfo);
// CustomMcClient.JoinResultSubject.Subscribe(x => Console.WriteLine(x));
var joinResult = await joinObservable;
Console.WriteLine(joinResult);
Console.ReadKey();
Console.ReadKey();
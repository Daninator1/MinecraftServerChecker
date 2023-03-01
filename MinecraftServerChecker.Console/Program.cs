using Microsoft.Extensions.Configuration;
using MinecraftServerChecker;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var login = configuration.GetSection("UserLogin").Get<UserLogin>()!;

var checker = new ServerChecker(login);
var loginResult = checker.Login();

if (!loginResult)
{
    Console.WriteLine("Login failed");
    return;
}

var checkResult = await checker.CheckServerAsync(new Server
{
    Ip = args[0],
    Port = ushort.Parse(args[1]),
});

checkResult.Switch(
    success =>
    {
        Console.WriteLine("Success");
        Console.WriteLine("Online mode: " + success.IsOnlineMode);
        Console.WriteLine("Online players: " + string.Join(", ", success.OnlinePlayers));
    },
    failure =>
    {
        Console.WriteLine("Failure");
        Console.WriteLine("Failure reason: " + failure.Reason);
        Console.WriteLine("Failure message: " + failure.Message);
    });

Console.ReadKey();
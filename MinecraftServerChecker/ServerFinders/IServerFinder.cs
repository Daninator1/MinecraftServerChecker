using MinecraftServerChecker.Models;

namespace MinecraftServerChecker.ServerFinders;

public interface IServerFinder
{
    public IAsyncEnumerable<Server> FindServersAsync();
}
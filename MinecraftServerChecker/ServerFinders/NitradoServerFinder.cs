using HtmlAgilityPack;
using MinecraftServerChecker.Models;

namespace MinecraftServerChecker.ServerFinders;

public class NitradoServerFinder : IServerFinder
{
    private readonly HttpClient httpClient;
    private readonly string serverListPageTemplate;
    private readonly string serverPageTemplate;

    public NitradoServerFinder()
    {
        this.httpClient = new HttpClient();

        this.serverListPageTemplate =
            "https://server.nitrado.net/deu/toplist/index/sort:slots_used/direction:desc/page:{0}?server_game=MC";
        this.serverPageTemplate = "https://server.nitrado.net{0}";
    }

    public async IAsyncEnumerable<Server> FindServersAsync()
    {
        for (var i = 0; i < 100; i++)
        {
            var serverListPage = await this.httpClient.GetAsync(string.Format(this.serverListPageTemplate, i));
            var serverListPageDocument = new HtmlDocument();
            serverListPageDocument.LoadHtml(await serverListPage.Content.ReadAsStringAsync());
            var serverPages = GetServerPageUrls(serverListPageDocument);

            foreach (var serverPageUrl in serverPages)
            {
                var serverPage = await this.httpClient.GetAsync(string.Format(this.serverPageTemplate, serverPageUrl));
                var serverPageDocument = new HtmlDocument();
                serverPageDocument.LoadHtml(await serverPage.Content.ReadAsStringAsync());
                yield return GetServer(serverPageDocument);
            }
        }
    }

    private static IEnumerable<string> GetServerPageUrls(HtmlDocument htmlDocument)
        => htmlDocument
            .GetElementbyId("toplist-ajax-target")
            .ChildNodes
            .Where(child => child.Name == "div")
            .SelectMany(div => div.ChildNodes
                .Where(child => child.Name == "a")
                .Select(a => a.GetAttributeValue("href", "")));

    private static Server GetServer(HtmlDocument htmlDocument)
    {
        var ipText = htmlDocument.DocumentNode.Descendants().Single(d => d.HasClass("gr-copy-ip")).InnerText;
        var ipParts = ipText.Split(':');
        return new Server
        {
            Ip = ipParts[0],
            Port = ushort.Parse(ipParts[1]),
        };
    }
}

using Newtonsoft.Json;

public class NYTService : INYTService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<NYTService> _logger;

    public NYTService(IHttpClientFactory httpClientFactory, ILogger<NYTService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<NYTConnection> GetNYTConnection(DateOnly date)
    {
        NYTConnection connection = null!;
        using HttpClient client = _httpClientFactory.CreateClient("NYT");
        try
        {
            var result = await client.GetAsync($"{date:yyyy-MM-dd}.json");

            result.EnsureSuccessStatusCode();

            var rawConnectionJSON = await result.Content.ReadAsStringAsync();

            if(rawConnectionJSON is null)
            {
                return connection;
            }

            NYTConnectionDeserialized rawConnection = JsonConvert.DeserializeObject<NYTConnectionDeserialized>(rawConnectionJSON);

            if(rawConnection is null)
            {
                return connection;
            }

            connection = NYTConnection.FromDeSerialized(rawConnection);
        }
        catch(Exception ex)
        {
            _logger.LogError("Error retrieving NYT connections: {Error}", ex);
        }
        
        return connection;
    }
}
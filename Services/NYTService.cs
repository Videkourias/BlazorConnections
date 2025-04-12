
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

    public async Task<Connection> GetConnection(DateOnly date)
    {
        Connection connection = null!;

        // Check if the date is within the valid range (June 12, 2023 to tomorrow)
        if(date < new DateOnly(2023, 6, 12) || date > DateOnly.FromDateTime(DateTime.Now.AddDays(1)))
        {
            _logger.LogWarning("NYT connection date is out of range: {Date}", date);
            return null;
        }

        using HttpClient client = _httpClientFactory.CreateClient("NYT");
        try
        {
            var result = await client.GetAsync($"{date:yyyy-MM-dd}.json");

            result.EnsureSuccessStatusCode();

            var rawConnectionJSON = await result.Content.ReadAsStringAsync();

            if(rawConnectionJSON is null)
            {
                _logger.LogError("Failed to read raw Connection JSON from response.");
                return connection;
            }

            NYTConnection? rawConnection = JsonConvert.DeserializeObject<NYTConnection>(rawConnectionJSON);

            if(rawConnection is null)
            {
                _logger.LogError("Failed to deserialize raw Connection JSON.");
                return connection;
            }

            connection = Connection.FromNYTConnection(rawConnection);
        }
        catch(Exception ex)
        {
            _logger.LogError("Error retrieving NYT connections: {Error}", ex);
        }
        
        return connection;
    }
}
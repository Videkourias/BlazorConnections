
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
        // Check if the date is within the valid range (June 12, 2023 to tomorrow)
        if(date < new DateOnly(2023, 6, 12) || date > DateOnly.FromDateTime(DateTime.Now.AddDays(1)))
        {
            throw new ArgumentException("Selected date is outside valid range for NYT Connections API");
        }

        using HttpClient client = _httpClientFactory.CreateClient("NYT");
        try
        {
            var result = await client.GetAsync($"{date:yyyy-MM-dd}.json");

            result.EnsureSuccessStatusCode();
            
            string rawConnectionJSON = await result.Content.ReadAsStringAsync();

            if(string.IsNullOrWhiteSpace(rawConnectionJSON))
            {
                throw new InvalidOperationException("NYT Connections API response is unexpectedly null or empty");
            }

            // Translate from received api structure into preferred structure for game
            NYTConnection? rawConnection = JsonConvert.DeserializeObject<NYTConnection>(rawConnectionJSON) ?? throw new InvalidOperationException("NYT Connections API response JSON could not be translated");
            return Connection.FromNYTConnection(rawConnection);
        }
        catch(Exception e)
        {
            _logger.LogError($"NYT Connections API exception: {e}");
            throw; // Throw exception up to front end can display error to user. 
        }   
    }
}
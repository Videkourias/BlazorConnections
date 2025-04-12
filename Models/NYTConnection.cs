using Newtonsoft.Json;

public class NYTConnection
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("groups")]
    public Dictionary<string, Group> Groups { get; set; }

    [JsonProperty("startingGroups")]
    public List<List<string>> StartingGroups { get; set; }
}

public class Group
{
    [JsonProperty("level")]
    public int Level { get; set; }

    [JsonProperty("members")]
    public List<string> Members { get; set; }
}
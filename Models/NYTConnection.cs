using Newtonsoft.Json;

public class NYTConnection
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("categories")]
    public List<Category> Categories { get; set; }

}

public class Category
{
    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("cards")]
    public List<Card> Cards { get; set; }
}

public record Card
{
    [JsonProperty("content")]
    public string Content { get; set; }
    
}
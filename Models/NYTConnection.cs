
public class NYTConnection
{
    public int Id { get; set; }
    public IEnumerable<WordGroup> Groups { get; set; }
    public NYTConnection(NYTConnectionDeserialized deserialized)
    {
        Id = deserialized.Id;
        Groups = deserialized.Groups.Select(x => new WordGroup
        {
            GroupName = x.Key,
            Difficulty = (Difficulty)x.Value.Level,
            Words = x.Value.Members.Select(y => new Word
            {
                Value = y,
                IsSelected = false
            })
        });
    }
}

public class WordGroup
{
    public string? GroupName { get; set; }
    public Difficulty Difficulty { get; set; }
    public IEnumerable<Word>? Words { get; set; }
}

public class Word
{
    public string Value { get; set; }
    public bool IsSelected { get; set; }
}
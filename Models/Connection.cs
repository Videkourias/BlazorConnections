
public class Connection
{
    public int Id { get; set; }
    //Four groups of four words each
    public List<WordGroup> Groups { get; set; }
    //List of words the user has not yet grouped
    public List<Word> WordList { get; set; }
    public bool FourWordsSelected => WordList.Count(x => x.IsSelected) >= 4;
    public int Chances { get; set; } = 4;
    public bool HasWon => WordList.Count() == 0 && Chances > 0;

    public void ShuffledWordList()
    {
        WordList = Groups.Where(x => !x.Completed).SelectMany(x => x.Words).ToList();

        Random random = new Random();
        int c = WordList.Count();
        while (c > 1)
        {
            c--;
            int k = random.Next(c + 1);
            (WordList[c], WordList[k]) = (WordList[k], WordList[c]);
        }
    }

    public bool MarkGroupComplete(Difficulty diffculty)
    {
        var wordGroup = Groups.Where(x => x.Difficulty == diffculty).FirstOrDefault();

        if(wordGroup is null)
        {
            return false;
        }

        var wordsToRemove = wordGroup.Words.ToList();

        wordsToRemove.ForEach(x => 
        {
            WordList.Remove(x);
        });

        wordGroup.Completed = true;
        wordGroup.TimeCompleted = TimeOnly.FromDateTime(DateTime.UtcNow);
        return true;
    }

    //Generate Connection game object from NYTConnection object returned from API request
    public static Connection FromNYTConnection(NYTConnection deserialized)
    {
        Connection connection = new Connection
        {
            Id = deserialized.Id,
            Groups = deserialized.Groups.Select(x => new WordGroup
            {
                GroupName = x.Key,
                Difficulty = (Difficulty)x.Value.Level,
                Words = x.Value.Members.Select(y => new Word
                {
                    Value = y,
                    IsSelected = false,
                    Difficulty = (Difficulty)x.Value.Level
                }).ToList()
            }).ToList()
        };

        connection.ShuffledWordList();

        return connection;
    }

}

public class WordGroup
{
    public string? GroupName { get; set; }
    public bool Completed { get; set; } = false;
    public TimeOnly TimeCompleted { get; set; }
    public Difficulty Difficulty { get; set; }
    public List<Word> Words { get; set; }
}

public class Word
{
    public string Value { get; set; }
    public bool IsSelected { get; set; } = false;
    public Difficulty Difficulty { get; set; }
}
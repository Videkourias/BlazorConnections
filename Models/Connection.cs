
public class Connection
{
    public int Id { get; set; }
    //Four groups of four words each
    public List<WordGroup> Groups { get; set; }
    //List of words the user has not yet grouped
    public List<Word> WordList { get; set; } = new();
    public bool FourWordsSelected => WordList.Count(x => x.IsSelected) >= 4;
    public int Chances { get; set; } = 4;
    public bool HasWon => WordList.Count() == 0 && Chances > 0;
    public bool HasLost => WordList.Count() > 0 && Chances <= 0;
    public bool AnySelected => WordList.Any(x => x.IsSelected);

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

        if(wordGroup is null || wordGroup.Completed)
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
            Groups = deserialized.Categories.Select(x => new WordGroup
            {
                GroupName = x.Title,
                Difficulty = (Difficulty)deserialized.Categories.IndexOf(x),
                Words = x.Cards.Select(y => new Word
                {
                    Value = y.Content,
                    IsSelected = false,
                    Difficulty = (Difficulty)deserialized.Categories.IndexOf(x)
                }).ToList()
            }).ToList()
        };


        foreach(var category in deserialized.Categories)
        {
            foreach(var card in category.Cards)
            {
                connection.WordList.Add(new Word
                {
                    Value = card.Content,
                    Difficulty = (Difficulty)deserialized.Categories.IndexOf(category)
                });
            }
        }

        connection.ShuffledWordList();

        return connection;
    }

    public string GetCompletionMessage()
    {
        return Chances switch
        {
            4 => "Perfect",
            3 => "Great",
            2 => "Solid",
            1 => "Phew",
            _ => "Next Time"
        };
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
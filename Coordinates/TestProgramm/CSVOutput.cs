namespace TestProgramm;

public class CSVOutput
{
    private string filePath;
    private string csvString;
    private bool comments;

    public CSVOutput(string filePath, bool comments)
    {
        this.filePath = filePath;
        this.comments = comments;
        csvString = csvString = $"pilot number, result{(comments ? ", comment" : "")}\n";
    }

    public void AppendResult(int pilotNumber, string result, string comment = "")
    {
        csvString += $"{pilotNumber},{result}{(comments ? ", " + comment : "")}\n";
    }

    public void Save()
    {
        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }


        File.WriteAllText(filePath, csvString);
    }
}
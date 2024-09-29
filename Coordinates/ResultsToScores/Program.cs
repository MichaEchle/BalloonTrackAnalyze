// See https://aka.ms/new-console-template for more information
using Competition;
using LoggingConnector;
using Microsoft.Extensions.Logging;
using System.Globalization;

LogConnector.LoggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = LogConnector.LoggerFactory.CreateLogger<Program>();
logger.LogInformation("Results to scores calculation");
string filePath = string.Empty;
do
{
    logger.LogInformation("Please enter the file path and name of the .csv file containing the results:");
    filePath = Console.ReadLine().Replace("\"", "");
} while (!File.Exists(filePath));

string[] lines = File.ReadAllLines(filePath);
string header = lines[0];
int commaCount = header.Count(x => x == ',');
int semicolonCount = header.Count(x => x == ';');
char separator = commaCount > semicolonCount ? ',' : ';';
logger.LogInformation("Using separator: '{separator}'", separator);
string[] headers = header.Split(separator);
int pilotNumberColumn = -1;
int resultColumn = -1;
logger.LogInformation("Please select the column number containing the pilot numbers:");
for (int index = 0; index < headers.Length; index++)
{
    logger.LogInformation("Header {index}: {header}", index, headers[index]);
}
while (!int.TryParse(Console.ReadLine(), out pilotNumberColumn))
{
    logger.LogError("Invalid input. Please enter a valid number.");
}
logger.LogInformation("Please select the column number containing the results:");
while (!int.TryParse(Console.ReadLine(), out resultColumn))
{
    logger.LogError("Invalid input. Please enter a valid number.");
}
logger.LogInformation("Please select the type of result: ");
string[] resultWinningTypes = Enum.GetNames<ScoreCalculation.ResultWinningType>();
for (int index = 0; index < resultWinningTypes.Length; index++)
{
    logger.LogInformation("Type {index}: {type}", index, resultWinningTypes[index]);
}
int resultWinningType = -1;
while (!int.TryParse(Console.ReadLine(), out resultWinningType) || resultWinningType < 0 || resultWinningType >= resultWinningTypes.Length)
{
    logger.LogError("Invalid input. Please enter a valid number.");
}
ScoreCalculation.ResultWinningType winningType = Enum.Parse<ScoreCalculation.ResultWinningType>(resultWinningTypes[resultWinningType]);

List<(int pilotNumber, double result)> results = new();
int maxColumns = int.MinValue;
for (int index = 1; index < lines.Length; index++)
{
    try
    {
        string[] columns = lines[index].Split(separator);
        if (columns.Length > maxColumns)
        {
            maxColumns = columns.Length;
        }
        int pilotNumber = int.Parse(columns[pilotNumberColumn]);
        string resultText = columns[resultColumn].Replace(",", ".");
        double result = double.Parse(columns[resultColumn], System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture);
        results.Add((pilotNumber, result));
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to parse line '{line}'", lines[index]);
    }
}

logger.LogInformation("Pilot numbers and results parsed. Calculating source");


var scores = ScoreCalculation.CalculateScores(results, winningType);

foreach (var score in scores)
{
    logger.LogInformation("Pilot: {pilotNumber} Result: {result} Score: {score}", score.pilotNumber, score.result, score.score);
}

lines[0] += separator + "Score";
for (int index = 1; index < lines.Length; index++)
{
    List<string> columns = lines[index].Split(separator).ToList();
    columns.AddRange(Enumerable.Repeat(string.Empty, maxColumns - columns.Count));
    var score = scores.FirstOrDefault(x => x.pilotNumber.ToString() == columns[pilotNumberColumn]).score;
    columns.Add(score.ToString());
    lines[index] = string.Join(separator, columns);
}

File.WriteAllLines(filePath, lines);
logger.LogInformation("Scores written to file '{filePath}'", filePath);
Thread.Sleep(200);

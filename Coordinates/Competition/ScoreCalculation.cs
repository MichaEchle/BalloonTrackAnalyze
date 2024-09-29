using LoggingConnector;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Competition;
public static class ScoreCalculation
{
    public enum ResultWinningType
    {
        LowestWins,
        HighestWins
    }

    private static readonly ILogger<Flight> Logger = LogConnector.LoggerFactory.CreateLogger<Flight>();

    /// <summary>
    /// Calculates the scores of the pilots based on rules described in axmer 14.5 Points Formula
    /// </summary>
    /// <param name="results">the list of results mapped to the pilot number (use double.NaN when no result was achieved)</param>
    /// <returns>the list of scores and results, mapped to pilot number</returns>
    public static List<(int pilotNumber, double result, int score)> CalculateScores(List<(int pilotNumber, double result)> results, ResultWinningType resultWinning)
    {
        List<(int pilotNumber, double result, int score)> scores = new();
                List<(int pilotNumber, double result)> resultsWithValues= results.Where(x => !double.IsNaN(x.result)).ToList();
                List<(int pilotNumber, double result)> resultsWithoutValues = results.Where(x => double.IsNaN(x.result)).ToList();
        switch (resultWinning)
        {
            case ResultWinningType.LowestWins:
                Logger.LogInformation("Sorting results: lowest result wins; winning results first");
                resultsWithValues=resultsWithValues.OrderBy(x => x.result).ToList();
                results.Clear();
                results.AddRange(resultsWithValues);
                results.AddRange(resultsWithoutValues);
                break;
            case ResultWinningType.HighestWins:
                Logger.LogInformation("Sorting results: highest result wins; winning results first");
                resultsWithValues = resultsWithValues.OrderByDescending(x => x.result).ToList();
                results.Clear();
                results.AddRange(resultsWithValues);
                results.AddRange(resultsWithoutValues);
                break;
            default:
                Logger.LogError("Unknown ResultWinningType");
                return null;
        }
        int numberOfPilots = results.Count;// P = number of competitors ranked in the flight
        int medianRank = (int)Math.Ceiling((double)numberOfPilots / 2);// M = P/2 (rounded to the next higher number) (Median Rank)
        //R = competitor's results (meters, etc) if in the superior half => results[index].result
        double medianResult = results[medianRank - 1].result;// RM = Result of the competitor ranked at the median rank
        //L = competitor's ranking position if in the inferior portion => index (since the list is ordered by result at the beginning)
        double bestResult = results[0].result;// W = the winning result of the task
        int numberOfCompetitorsWithResult = results.Count(x => !double.IsNaN(x.result)); // A = number of competitors in group A
        // SM = rounded points score of the median ranking competitor, calculated under formula 2

        if (numberOfCompetitorsWithResult == 0)// 14.5.7 no competitors with achieved result
        {
            Logger.LogInformation("No competitors achieved result: Calculate scores after 14.5.7");
            foreach (var result in results)
            {
                scores.Add((result.pilotNumber, result.result, 500));
            }
            return scores;
        }
        if (numberOfCompetitorsWithResult < medianRank) // 14.5.6 less then half of the competitors have achieved a result
        {
            Logger.LogInformation("Less than half of the competitors achieved a result: Calculate scores after 14.5.6");
            double worstResultGroupA; // RM = lowest result in group A
            switch (resultWinning)
            {
                case ResultWinningType.LowestWins:
                    worstResultGroupA = results.Where(x => !double.IsNaN(x.result)).MaxBy(x => x.result).result;
                    break;
                case ResultWinningType.HighestWins:
                    worstResultGroupA = results.Where(x => !double.IsNaN(x.result)).MinBy(x => x.result).result;
                    break;
                default:
                    worstResultGroupA = double.NaN;
                    break;
            }

            //formula 2 :  1000 * (P + 1 - L) / P (in this case L = A)
            int worstScoreGroupA = (int)Math.Round(1000.0 * (numberOfPilots + 1 - numberOfCompetitorsWithResult) / numberOfPilots, 0, MidpointRounding.AwayFromZero); // SM = rounded score of lowest ranking competitor in group A, calculated under Formula Two

            Logger.LogInformation("P = {numberOfPilots}, M = {numberOfCompetitorsWithResult}, RM = {worstResultGroupA}, W = {bestResult}, A = {numberOfCompetitorsWithResult}, SM = {worstResultGroupA}",numberOfPilots ,numberOfCompetitorsWithResult, worstResultGroupA,bestResult,numberOfCompetitorsWithResult,worstScoreGroupA);

            //formula 3 : 1000 * (P + 1 - A) / P - 200
            foreach (var result in results.Where(x => double.IsNaN(x.result)))
            {
                int score = (int)Math.Round(1000.0 * (numberOfPilots + 1 - numberOfCompetitorsWithResult) / numberOfPilots - 200, 0, MidpointRounding.AwayFromZero);
                scores.Add((result.pilotNumber, result.result, score));
            }

            //formula 1 : 1000 - ((1000 - SM)/(RM - W)) * (R - W)
            for (int index = 0; index < numberOfCompetitorsWithResult - 1; index++)
            {
                int score = (int)Math.Round(1000 - ((1000 - worstScoreGroupA) / (worstResultGroupA - bestResult)) * (results[index].result - bestResult), 0, MidpointRounding.AwayFromZero);
                scores.Add((results[index].pilotNumber, results[index].result, score));
            }
            var worstPilotGroupA=results.First(x => x.result == worstResultGroupA);
            scores.Add((worstPilotGroupA.pilotNumber, worstPilotGroupA.result, worstScoreGroupA));
        }
        else
        {
            Logger.LogInformation("More than half of the competitors achieved a result: Calculate scores after 14.5.5");
            //formula 2 :  1000 * (P + 1 - L) / P
            for (int index = medianRank - 1; index <= numberOfCompetitorsWithResult - 1; index++)
            {
                int score = (int)Math.Round(1000.0 * (numberOfPilots + 1 - (index + 1)) / numberOfPilots, 0, MidpointRounding.AwayFromZero);
                scores.Add((results[index].pilotNumber, results[index].result, score));
            }
            int medianScore = scores.Where(x => x.result == medianResult).First().score;
            Logger.LogInformation("P = {numberOfPilots}, M = {medianRank}, RM = {medianResult}, W = {bestResult}, A = {numberOfCompetitorsWithResult}, SM = {medianScore}", numberOfPilots, medianRank, medianResult, bestResult, numberOfCompetitorsWithResult, medianScore);
            //formula 3 : 1000 * (P + 1 - A) / P - 200
            foreach (var result in results.Where(x => double.IsNaN(x.result)))
            {
                int score = (int)Math.Round(1000.0 * (numberOfPilots + 1 - numberOfCompetitorsWithResult) / numberOfPilots - 200, 0, MidpointRounding.AwayFromZero);
                scores.Add((result.pilotNumber, result.result, score));
            }

            //formula 1 : 1000 - ((1000 - SM)/(RM - W)) * (R - W)
            for (int index = 0; index < medianRank - 1; index++)
            {
                int score = (int)Math.Round(1000 - ((1000 - medianScore) / (medianResult - bestResult)) * (results[index].result - bestResult), 0, MidpointRounding.AwayFromZero);
                scores.Add((results[index].pilotNumber, results[index].result, score));
            }
        }
        scores = scores.OrderByDescending(x => x.score).ToList();
        return scores;
    }

    /// <summary>
    /// Calculates the scores of the pilots based on rules layed out in axmer 14.5 Points Formula
    /// </summary>
    /// <param name="results">the list of results mapped to the pilot number (use double.NaN when no result was achieved)</param>
    /// <returns>the list of scores and results, mapped to pilot number</returns>
    public static List<(int pilotNumber, double result, int score)> CalculateScores2(List<(int pilotNumber, double result)> results)
    {
        List<(int pilotNumber, double result, int score)> scores = new();
        results = results.OrderBy(x => x.result).ToList();
        int numberOfPilots = results.Count;// P = number of competitors ranked in the flight
        int medianRank = (int)Math.Ceiling((double)numberOfPilots / 2);// M = P/2 (rounded to the next higher number) (Median Rank)
                                                                       //R = competitor's results (meters, etc) if in the superior half => results[index].result
        double medianResult = results[medianRank - 1].result;// RM = Result of the competitor ranked at the median rank
                                                             //L = competitor's ranking position if in the inferior portion => index (since the list is ordered by result at the beginning)
        double bestResult = results[0].result;// W = the winning result of the task
        int numberOfCompetitorsWithResult = results.Count(x => !double.IsNaN(x.result)); // A = number of competitors in group A
                                                                                         // SM = rounded points score of the median ranking competitor, calculated under formula 2

        if (numberOfCompetitorsWithResult == 0)// 14.5.7 no competitors with achieved result
        {
            foreach (var result in results)
            {
                scores.Add((result.pilotNumber, result.result, 500));
            }
            return scores;
        }
        int medianScore = 0;
        bool lessThanHalfOfTheCompetitorsHaveAchievedAResult = false;
        if (numberOfCompetitorsWithResult < medianRank) // 14.5.6 less then half of the competitors have achieved a result
        {
            double worstResult =
            medianResult = results.Where(x => !double.IsNaN(x.result)).MaxBy(x => x.result).result;
            medianRank = results.IndexOf(results.First(x => x.result == medianResult)) + 1;
            medianScore = (int)Math.Round(1000.0 * (numberOfPilots + 1 - medianRank) / numberOfPilots, 0, MidpointRounding.AwayFromZero);
            lessThanHalfOfTheCompetitorsHaveAchievedAResult = true;
        }

        //formula 2 :  1000 * (P + 1 - L) / P
        for (int index = medianRank - 1; index <= numberOfCompetitorsWithResult - 1; index++)
        {
            int score = (int)Math.Round(1000.0 * (numberOfPilots + 1 - (index + 1)) / numberOfPilots, 0, MidpointRounding.AwayFromZero);
            scores.Add((results[index].pilotNumber, results[index].result, score));
        }
        if (!lessThanHalfOfTheCompetitorsHaveAchievedAResult)
        {
            medianScore = scores.Where(x => x.result == medianResult).First().score;
        }


        //formula 3 : 1000 * (P + 1 - A) / P - 200
        foreach (var result in results.Where(x => double.IsNaN(x.result)))
        {
            int score = (int)Math.Round(1000.0 * (numberOfPilots + 1 - numberOfCompetitorsWithResult) / numberOfPilots - 200, 0, MidpointRounding.AwayFromZero);
            scores.Add((result.pilotNumber, result.result, score));
        }

        //formula 1 : 1000 - ((1000 - SM)/(RM - W)) * (R - W)
        for (int index = 0; index < medianRank - 1; index++)
        {
            int score = (int)Math.Round(1000 - ((1000 - medianScore) / (medianResult - bestResult)) * (results[index].result - bestResult), 0, MidpointRounding.AwayFromZero);
            scores.Add((results[index].pilotNumber, results[index].result, score));
        }

        scores = scores.OrderByDescending(x => x.score).ToList();
        return scores;
    }

}

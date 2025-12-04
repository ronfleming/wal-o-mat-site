using WalOMat.Shared.Models;

namespace WalOMat.Client.Services;

/// <summary>
/// Calculates match scores between user answers and whale profiles.
/// Scoring: 2 points for exact match, 1 for adjacent, 0 for opposite.
/// Weighted questions count double.
/// </summary>
public class MatchingService
{
    public List<MatchResult> CalculateMatches(
        List<UserAnswer> answers,
        List<Question> questions,
        List<WhaleProfile> whales)
    {
        var questionLookup = questions.ToDictionary(q => q.Id);
        var results = new List<MatchResult>();

        foreach (var whale in whales)
        {
            var (earned, maximum) = CalculateScore(answers, questionLookup, whale.Id);
            var percentage = maximum > 0 ? (earned / (double)maximum) * 100 : 0;

            results.Add(new MatchResult
            {
                WhaleId = whale.Id,
                WhaleName = whale.Name,
                ImagePath = whale.ImagePath,
                MatchPercentage = Math.Round(percentage, 1),
                PointsEarned = earned,
                PointsMaximum = maximum
            });
        }

        return results.OrderByDescending(r => r.MatchPercentage).ToList();
    }

    private (int earned, int maximum) CalculateScore(
        List<UserAnswer> answers,
        Dictionary<string, Question> questions,
        string whaleId)
    {
        int earned = 0;
        int maximum = 0;

        foreach (var answer in answers)
        {
            // Skip unanswered questions
            if (answer.Position is null)
                continue;

            if (!questions.TryGetValue(answer.QuestionId, out var question))
                continue;

            if (!question.WhalePositions.TryGetValue(whaleId, out var whalePosition))
                continue;

            int multiplier = answer.IsWeighted ? 2 : 1;
            int points = ScorePosition(answer.Position.Value, whalePosition);

            earned += points * multiplier;
            maximum += 2 * multiplier; // Max is always 2 per question
        }

        return (earned, maximum);
    }

    /// <summary>
    /// Scores a single position comparison.
    /// </summary>
    private static int ScorePosition(int userPosition, int whalePosition)
    {
        // Exact match: 2 points
        if (userPosition == whalePosition)
            return 2;

        // Opposite positions (-1 vs 1): 0 points
        if (userPosition == -whalePosition && userPosition != 0)
            return 0;

        // Adjacent (one is neutral): 1 point
        return 1;
    }
}


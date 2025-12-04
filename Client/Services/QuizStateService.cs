using WalOMat.Shared.Models;

namespace WalOMat.Client.Services;

/// <summary>
/// Manages the state of an in-progress quiz session.
/// </summary>
public class QuizStateService
{
    private readonly List<UserAnswer> _answers = new();
    private readonly HashSet<string> _selectedWhaleIds = new();

    public QuizData? QuizData { get; private set; }
    public int CurrentQuestionIndex { get; private set; }
    public QuizPhase Phase { get; private set; } = QuizPhase.NotStarted;
    public IReadOnlyList<UserAnswer> Answers => _answers;
    public IReadOnlySet<string> SelectedWhaleIds => _selectedWhaleIds;
    public List<MatchResult>? Results { get; private set; }

    public Question? CurrentQuestion =>
        QuizData?.Questions.ElementAtOrDefault(CurrentQuestionIndex);

    public int TotalQuestions => QuizData?.Questions.Count ?? 0;
    public bool IsLastQuestion => CurrentQuestionIndex >= TotalQuestions - 1;

    public void Initialize(QuizData data)
    {
        QuizData = data;
        _answers.Clear();
        _selectedWhaleIds.Clear();
        CurrentQuestionIndex = 0;
        Phase = QuizPhase.NotStarted;
        Results = null;
    }

    public void StartQuiz()
    {
        Phase = QuizPhase.Answering;
        CurrentQuestionIndex = 0;
    }

    public void AnswerQuestion(int? position)
    {
        if (CurrentQuestion is null) return;

        // Update or add answer
        var existing = _answers.FindIndex(a => a.QuestionId == CurrentQuestion.Id);
        var answer = new UserAnswer
        {
            QuestionId = CurrentQuestion.Id,
            Position = position,
            IsWeighted = false
        };

        if (existing >= 0)
            _answers[existing] = answer;
        else
            _answers.Add(answer);
    }

    public void NextQuestion()
    {
        if (IsLastQuestion)
            Phase = QuizPhase.Weighting;
        else
            CurrentQuestionIndex++;
    }

    public void PreviousQuestion()
    {
        if (CurrentQuestionIndex > 0)
            CurrentQuestionIndex--;
    }

    public void GoToQuestion(int index)
    {
        if (index >= 0 && index < TotalQuestions)
            CurrentQuestionIndex = index;
    }

    public void SetWeight(string questionId, bool isWeighted)
    {
        var index = _answers.FindIndex(a => a.QuestionId == questionId);
        if (index < 0) return;

        var old = _answers[index];
        _answers[index] = new UserAnswer
        {
            QuestionId = old.QuestionId,
            Position = old.Position,
            IsWeighted = isWeighted
        };
    }

    public void FinishWeighting()
    {
        Phase = QuizPhase.WhaleSelection;
        // Default: select all whales
        if (QuizData is not null)
        {
            foreach (var whale in QuizData.Whales)
                _selectedWhaleIds.Add(whale.Id);
        }
    }

    public void ToggleWhaleSelection(string whaleId)
    {
        if (!_selectedWhaleIds.Remove(whaleId))
            _selectedWhaleIds.Add(whaleId);
    }

    public void SelectAllWhales()
    {
        if (QuizData is null) return;
        foreach (var whale in QuizData.Whales)
            _selectedWhaleIds.Add(whale.Id);
    }

    public void CalculateResults(MatchingService matchingService)
    {
        if (QuizData is null) return;

        var selectedWhales = QuizData.Whales
            .Where(w => _selectedWhaleIds.Contains(w.Id))
            .ToList();

        Results = matchingService.CalculateMatches(_answers, QuizData.Questions, selectedWhales);
        Phase = QuizPhase.Results;
    }

    public void Reset()
    {
        _answers.Clear();
        _selectedWhaleIds.Clear();
        CurrentQuestionIndex = 0;
        Phase = QuizPhase.NotStarted;
        Results = null;
    }

    public int? GetAnswerForQuestion(string questionId)
    {
        return _answers.FirstOrDefault(a => a.QuestionId == questionId)?.Position;
    }
}

public enum QuizPhase
{
    NotStarted,
    Answering,
    Weighting,
    WhaleSelection,
    Results
}


namespace WalOMat.Shared.Models;

/// <summary>
/// Request payload for saving quiz results.
/// Used by both the Client (to send) and the API (to receive).
/// </summary>
public class SaveResultRequest
{
    public string Language { get; set; } = "de";
    public List<AnswerDto> Answers { get; set; } = new();
    public List<string> SelectedWhales { get; set; } = new();
    public List<ResultDto> Results { get; set; } = new();
}

public class AnswerDto
{
    public string QuestionId { get; set; } = string.Empty;
    public int? Position { get; set; }
    public bool IsWeighted { get; set; }
}

public class ResultDto
{
    public string WhaleId { get; set; } = string.Empty;
    public double Percentage { get; set; }

    public ResultDto() { }
}


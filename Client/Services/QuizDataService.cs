using System.Net.Http.Json;
using WalOMat.Shared.Models;

namespace WalOMat.Client.Services;

/// <summary>
/// Loads quiz content from static JSON files.
/// </summary>
public class QuizDataService
{
    private readonly HttpClient _http;
    private QuizData? _cachedData;

    public QuizDataService(HttpClient http)
    {
        _http = http;
    }

    public async Task<QuizData> LoadQuizDataAsync()
    {
        if (_cachedData is not null)
            return _cachedData;

        var questions = await _http.GetFromJsonAsync<List<Question>>("data/questions.json")
            ?? throw new InvalidOperationException("Failed to load questions.");

        var whales = await _http.GetFromJsonAsync<List<WhaleProfile>>("data/whales.json")
            ?? throw new InvalidOperationException("Failed to load whale profiles.");

        _cachedData = new QuizData
        {
            Questions = questions,
            Whales = whales
        };

        return _cachedData;
    }
}


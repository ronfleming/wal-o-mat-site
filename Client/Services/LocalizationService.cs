namespace WalOMat.Client.Services;

/// <summary>
/// Simple localization service for DE/EN support.
/// Stores strings in dictionaries - easy to maintain and extend.
/// </summary>
public class LocalizationService
{
    private string _currentLanguage = "de";
    
    public string CurrentLanguage => _currentLanguage;
    public bool IsGerman => _currentLanguage == "de";
    public bool IsEnglish => _currentLanguage == "en";
    
    public event Action? OnLanguageChanged;

    public void SetLanguage(string language)
    {
        if (_currentLanguage != language && (language == "de" || language == "en"))
        {
            _currentLanguage = language;
            OnLanguageChanged?.Invoke();
        }
    }

    public void ToggleLanguage()
    {
        SetLanguage(_currentLanguage == "de" ? "en" : "de");
    }

    public string Get(string key)
    {
        var dict = _currentLanguage == "de" ? German : English;
        return dict.TryGetValue(key, out var value) ? value : key;
    }

    // German strings
    private static readonly Dictionary<string, string> German = new()
    {
        // Meta / SEO
        ["meta_title"] = "Wal-O-Mat – Welcher Wal bist du?",
        ["meta_description"] = "Finde mit diesem lustigen Persönlichkeitsquiz heraus, welche Walart am besten zu dir passt. Inspiriert vom deutschen Wahl-O-Mat.",
        
        // Navigation
        ["language_toggle"] = "English",
        ["about_link"] = "Über dieses Projekt",
        ["back_home"] = "← Zurück zur Startseite",
        
        // Splash page
        ["splash_title"] = "Wal-O-Mat",
        ["splash_tagline"] = "Welcher Wal bist du?",
        ["splash_about"] = "Wal-O-Mat ist eine humorvolle Hommage an den <a href=\"https://www.wahl-o-mat.de\" target=\"_blank\" rel=\"noopener\">Wahl-O-Mat</a> der Bundeszentrale für politische Bildung. Wir nutzen das gleiche Bewertungsprinzip – aber mit Walen! <em>Dies ist nicht der offizielle Wahl-O-Mat und steht in keiner Verbindung zur bpb.</em>",
        ["splash_intro"] = "Beantworte ein paar Fragen über dich selbst und finde heraus, welcher Wal am besten zu deiner Persönlichkeit passt!",
        ["splash_start"] = "Quiz starten",
        
        // About page
        ["about_title"] = "Über Wal-O-Mat",
        ["about_what_title"] = "Was ist das?",
        ["about_what_1"] = "Wal-O-Mat ist ein Persönlichkeitsquiz, das als liebevolle Parodie des deutschen <a href=\"https://www.wahl-o-mat.de\" target=\"_blank\" rel=\"noopener\">Wahl-O-Mat</a> entstanden ist—einem Tool der Bundeszentrale für politische Bildung, das Wählern hilft, ihre Positionen mit denen der Parteien zu vergleichen.",
        ["about_what_2"] = "Hier vergleichen wir stattdessen deine Persönlichkeit mit verschiedenen Walarten. Denn wer wollte nicht schon immer wissen, ob er eher ein geselliger Orca oder ein nachdenklicher Blauwal ist?",
        ["about_how_title"] = "Wie funktioniert es?",
        ["about_how_1"] = "Das Quiz präsentiert dir eine Reihe von Aussagen. Du kannst jeder Aussage zustimmen, neutral bleiben oder nicht zustimmen. Basierend auf deinen Antworten berechnen wir, welche Walart am besten zu dir passt.",
        ["about_how_2"] = "Du kannst auch bestimmte Fragen als besonders wichtig markieren. Diese zählen dann doppelt in der Auswertung.",
        ["about_scoring_title"] = "Punktevergabe",
        ["about_scoring_intro"] = "Die Punktevergabe folgt dem gleichen Prinzip wie beim echten Wahl-O-Mat:",
        ["about_scoring_2pts"] = "<strong>2 Punkte</strong> bei exakter Übereinstimmung (z.B. beide 'Stimme zu')",
        ["about_scoring_1pt"] = "<strong>1 Punkt</strong> bei Annäherung (z.B. du 'Stimme zu', Wal 'Neutral')",
        ["about_scoring_0pts"] = "<strong>0 Punkte</strong> bei Gegensätzen (z.B. du 'Stimme zu', Wal 'Stimme nicht zu')",
        ["about_scoring_note"] = "Gewichtete Fragen zählen doppelt. Übersprungene Fragen werden nicht gewertet.",
        ["about_tech_title"] = "Technischer Hintergrund",
        ["about_tech_text"] = "Dieses Projekt wurde mit <strong>Blazor WebAssembly</strong> entwickelt und auf <strong>Azure App Service</strong> gehostet. Der Quellcode ist auf <a href=\"https://github.com/ronfleming/wal-o-mat-site\" target=\"_blank\" rel=\"noopener\">GitHub</a> verfügbar.",
        ["about_privacy_title"] = "Datenschutz & Analytics",
        ["about_privacy_intro"] = "Wir verwenden Azure Application Insights, um die Nutzung unserer Website zu verstehen (z. B. Seitenaufrufe, geografische Region).",
        ["about_privacy_no_cookies"] = "Keine Tracking-Cookies",
        ["about_privacy_anonymous"] = "IP-Adressen werden anonymisiert",
        ["about_privacy_retention"] = "Daten werden nach 90 Tagen gelöscht",
        ["about_privacy_no_sharing"] = "Keine Weitergabe an Dritte",
        ["about_privacy_rights"] = "Bei Fragen zum Datenschutz oder zur Ausübung deiner Rechte (Auskunft, Löschung) kontaktiere uns über GitHub.",
        
        // Quiz
        ["quiz_loading"] = "Lade Fragen...",
        ["quiz_question"] = "Frage",
        ["quiz_of"] = "von",
        ["quiz_agree"] = "Stimme zu",
        ["quiz_neutral"] = "Neutral",
        ["quiz_disagree"] = "Stimme nicht zu",
        ["quiz_skip"] = "Überspringen",
        ["quiz_back"] = "← Zurück",
        ["quiz_next"] = "Weiter →",
        ["quiz_to_weighting"] = "Zur Gewichtung →",
        
        // Weighting
        ["weighting_title"] = "Gewichtung",
        ["weighting_intro"] = "Welche Fragen sind dir besonders wichtig? Diese zählen doppelt.",
        ["weighting_continue"] = "Weiter zur Wal-Auswahl →",
        
        // Whale selection
        ["selection_title"] = "Wal-Auswahl",
        ["selection_intro"] = "Mit welchen Walen möchtest du dich vergleichen?",
        ["selection_all"] = "Alle auswählen",
        ["selection_continue"] = "Ergebnis anzeigen →",
        
        // Results
        ["results_title"] = "Dein Ergebnis",
        ["results_intro"] = "So gut passt du zu den verschiedenen Walen:",
        ["results_restart"] = "Nochmal spielen",
        ["results_home"] = "Zur Startseite",
        
        // Sharing
        ["share_results"] = "Ergebnis teilen",
        ["share_saving"] = "Wird gespeichert...",
        ["share_success"] = "Link erstellt! Teile ihn mit deinen Freunden:",
        ["share_copy"] = "Link kopieren",
        ["share_copied"] = "Kopiert! ✓",
        ["shared_result_title"] = "Mein Wal",
        ["shared_result_top"] = "Die höchste Übereinstimmung war:",
        ["shared_result_other_matches"] = "Weitere Top-Übereinstimmungen:",
        ["shared_result_expired"] = "Dieser Link ist abgelaufen.",
        ["shared_result_take_quiz"] = "Quiz selbst machen →"
    };

    // English strings
    private static readonly Dictionary<string, string> English = new()
    {
        // Meta / SEO
        ["meta_title"] = "Wal-O-Mat – Which Whale Are You?",
        ["meta_description"] = "Find out which whale species matches your personality with this fun quiz. Inspired by Germany's Wahl-O-Mat.",
        
        // Navigation
        ["language_toggle"] = "Deutsch",
        ["about_link"] = "About this project",
        ["back_home"] = "← Back to home",
        
        // Splash page
        ["splash_title"] = "Wal-O-Mat",
        ["splash_tagline"] = "Which whale are you?",
        ["splash_about"] = "Wal-O-Mat is a playful homage to the <a href=\"https://www.wahl-o-mat.de\" target=\"_blank\" rel=\"noopener\">Wahl-O-Mat</a> by Germany's Federal Agency for Civic Education. We use the same scoring system – but with whales! <em>This is not the official Wahl-O-Mat and is not affiliated with the bpb.</em>",
        ["splash_intro"] = "Answer a few questions about yourself and discover which whale species best matches your personality!",
        ["splash_start"] = "Start Quiz",
        
        // About page
        ["about_title"] = "About Us – Wal-O-Mat",
        ["about_what_title"] = "What is this?",
        ["about_what_1"] = "Wal-O-Mat is a personality quiz inspired by Germany's <a href=\"https://www.wahl-o-mat.de\" target=\"_blank\" rel=\"noopener\">Wahl-O-Mat</a>—a tool by the Federal Agency for Civic Education that helps voters compare their positions with political parties.",
        ["about_what_2"] = "Here, we compare your personality with different whale species instead. Because who hasn't wondered whether they're more of a social orca or a thoughtful blue whale?",
        ["about_how_title"] = "How does it work?",
        ["about_how_1"] = "The quiz presents you with a series of statements. You can agree, stay neutral, or disagree with each one. Based on your answers, we calculate which whale species best matches your personality.",
        ["about_how_2"] = "You can also mark certain questions as especially important. These count double in the evaluation.",
        ["about_scoring_title"] = "Scoring",
        ["about_scoring_intro"] = "The scoring follows the same principle as the real Wahl-O-Mat:",
        ["about_scoring_2pts"] = "<strong>2 points</strong> for exact match (e.g., both \"Agree\")",
        ["about_scoring_1pt"] = "<strong>1 point</strong> for adjacent positions (e.g., you \"Agree\", whale \"Neutral\")",
        ["about_scoring_0pts"] = "<strong>0 points</strong> for opposites (e.g., you \"Agree\", whale \"Disagree\")",
        ["about_scoring_note"] = "Weighted questions count double. Skipped questions are not scored.",
        ["about_tech_title"] = "Technical Background",
        ["about_tech_text"] = "This project was built with <strong>Blazor WebAssembly</strong> and hosted on <strong>Azure App Service</strong>. Source code is available on <a href=\"https://github.com/ronfleming/wal-o-mat-site\" target=\"_blank\" rel=\"noopener\">GitHub</a>.",
        ["about_privacy_title"] = "Privacy & Analytics",
        ["about_privacy_intro"] = "We use Azure Application Insights to understand website usage (e.g., page views, geographic region).",
        ["about_privacy_no_cookies"] = "No tracking cookies",
        ["about_privacy_anonymous"] = "IP addresses are anonymized",
        ["about_privacy_retention"] = "Data is deleted after 90 days",
        ["about_privacy_no_sharing"] = "No data sharing with third parties",
        ["about_privacy_rights"] = "For privacy questions or to exercise your rights (access, deletion), contact us via GitHub.",
        
        // Quiz
        ["quiz_loading"] = "Loading questions...",
        ["quiz_question"] = "Question",
        ["quiz_of"] = "of",
        ["quiz_agree"] = "Agree",
        ["quiz_neutral"] = "Neutral",
        ["quiz_disagree"] = "Disagree",
        ["quiz_skip"] = "Skip",
        ["quiz_back"] = "← Back",
        ["quiz_next"] = "Next →",
        ["quiz_to_weighting"] = "Continue to weighting →",
        
        // Weighting
        ["weighting_title"] = "Weighting",
        ["weighting_intro"] = "Which questions are especially important to you? These count double.",
        ["weighting_continue"] = "Continue to whale selection →",
        
        // Whale selection
        ["selection_title"] = "Whale Selection",
        ["selection_intro"] = "Which whales would you like to compare yourself to?",
        ["selection_all"] = "Select all",
        ["selection_continue"] = "Show results →",
        
        // Results
        ["results_title"] = "Your Result",
        ["results_intro"] = "Here's how well you match with each whale:",
        ["results_restart"] = "Play again",
        ["results_home"] = "Back to home",
        
        // Sharing
        ["share_results"] = "Share result",
        ["share_saving"] = "Saving...",
        ["share_success"] = "Link created! Share it with your friends:",
        ["share_copy"] = "Copy link",
        ["share_copied"] = "Copied! ✓",
        ["shared_result_title"] = "My Whale",
        ["shared_result_top"] = "Your top match was:",
        ["shared_result_other_matches"] = "Other top matches:",
        ["shared_result_expired"] = "This link has expired.",
        ["shared_result_take_quiz"] = "Take the quiz yourself →"
    };
}


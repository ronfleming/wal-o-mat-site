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
        ["splash_intro"] = "14 Fragen. 10 Wale. 1 Wahrheit. Bist du bereit herauszufinden, welches Meeressäugetier in dir steckt?",
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
        ["about_tech_text"] = "Dieses Projekt wurde mit <strong>Blazor WebAssembly</strong> entwickelt und auf <strong>Azure Static Web Apps</strong> gehostet. Der Quellcode ist auf <a href=\"https://github.com/ronfleming/wal-o-mat-site\" target=\"_blank\" rel=\"noopener\">GitHub</a> verfügbar.",
        ["about_privacy_title"] = "Datenschutz & Analytics",
        ["about_privacy_intro"] = "Wir verwenden Azure Application Insights, um die Nutzung unserer Website zu verstehen (z. B. Seitenaufrufe, geografische Region).",
        ["about_privacy_no_cookies"] = "Keine Tracking-Cookies",
        ["about_privacy_anonymous"] = "IP-Adressen werden anonymisiert",
        ["about_privacy_retention"] = "Daten werden nach 90 Tagen gelöscht",
        ["about_privacy_no_sharing"] = "Keine Weitergabe an Dritte",
        ["about_privacy_rights"] = "Bei Fragen zum Datenschutz oder zur Ausübung deiner Rechte (Auskunft, Löschung) kontaktiere uns über GitHub.",
        
        // Quiz
        ["quiz_loading"] = "Lade Fragen...",
        ["quiz_loading_1"] = "Wale werden konsultiert...",
        ["quiz_loading_2"] = "Echolokalisierung läuft...",
        ["quiz_loading_3"] = "Die Wale streiten sich noch über dich...",
        ["quiz_loading_4"] = "Ein Narwal musste noch sein Horn polieren.",
        ["quiz_question"] = "Frage",
        ["quiz_of"] = "von",
        ["quiz_subtext_1"] = "Keine Sorge, wir beurteilen dich. Ein bisschen.",
        ["quiz_subtext_2"] = "Deine Therapeutin würde das hier lieben.",
        ["quiz_subtext_3"] = "Fast geschafft! (Das ist gelogen.)",
        ["quiz_subtext_4"] = "Schummeln ist erlaubt. Wir sehen es nur.",
        ["quiz_subtext_5"] = "Denk nicht zu lange nach. Außer du bist ein Pottwal.",
        ["quiz_subtext_6"] = "Die Wale beobachten dich.",
        ["quiz_subtext_7"] = "Tief durchatmen. Wie ein Blauwal.",
        ["quiz_subtext_8"] = "Dein innerer Wal wird langsam ungeduldig.",
        ["quiz_subtext_9"] = "Diese Antwort definiert dich. Kein Druck.",
        ["quiz_subtext_10"] = "Hör auf dein Bauchgefühl. Oder deine Flosse.",
        ["quiz_subtext_11"] = "Noch ein paar Fragen. Versprochen. Vielleicht.",
        ["quiz_subtext_12"] = "Selbsterkenntnis ist der erste Schritt. Wohin? Keine Ahnung.",
        ["quiz_subtext_13"] = "Die Spannung steigt. Wie bei einem Narwal-Horn.",
        ["quiz_subtext_14"] = "Letzte Runde! (Okay, fast letzte.)",
        ["quiz_subtext_15"] = "Irgendwo freut sich ein Wal auf dich.",
        ["quiz_subtext_16"] = "Du machst das super. Für einen Menschen.",
        ["quiz_agree"] = "Stimme zu",
        ["quiz_agree_tooltip"] = "Ja! Das bin so ich!",
        ["quiz_neutral"] = "Neutral",
        ["quiz_neutral_tooltip"] = "Ich commitme mich zu nichts.",
        ["quiz_disagree"] = "Stimme nicht zu",
        ["quiz_disagree_tooltip"] = "Absolut nicht.",
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
        ["results_intro"] = "Nach intensiver Analyse durch unser hochqualifiziertes Team von Meeresbiologen (lies: ein Algorithmus und viel Kaffee) steht fest...",
        ["results_restart"] = "Nochmal! (Diesmal ehrlicher)",
        ["results_home"] = "Zur Startseite",
        ["results_match_90"] = "Bist du sicher, dass du kein Wal bist?",
        ["results_match_70"] = "Seelenverwandt. Definitiv.",
        ["results_match_50"] = "Ihr würdet euch auf einer Party verstehen.",
        ["results_match_30"] = "Entfernte Bekannte. Nickt sich höflich zu.",
        ["results_match_low"] = "Ihr... habt Wasser gemeinsam.",
        
        // Dugong Easter Egg (all neutral answers)
        ["dugong_title"] = "Du bist ein... Dugong?",
        ["dugong_description"] = "Du bist... gar kein Wal. Du bist ein Dugong. Das passiert, wenn man sich nie entscheiden kann. Dugongs sind die 'Ich weiß nicht, such du aus'-Freunde des Ozeans.",
        ["dugong_traits"] = "Unentschlossen · Geht mit dem Strom · Vermeidet Konflikte · Entspannt",
        
        // Sharing
        ["share_results"] = "Ergebnis teilen",
        ["share_results_alt_1"] = "Freunde neidisch machen",
        ["share_results_alt_2"] = "Beweis verschicken",
        ["share_saving"] = "Wird gespeichert...",
        ["share_success"] = "Link erstellt! Teile ihn mit deinen Freunden:",
        ["share_copy"] = "Link kopieren",
        ["share_copied"] = "Kopiert! ✓",
        ["share_error"] = "Teilen fehlgeschlagen. Bitte versuche es erneut.",
        ["shared_result_title"] = "Mein Wal",
        ["shared_result_top"] = "Die höchste Übereinstimmung war:",
        ["shared_result_other_matches"] = "Weitere Top-Übereinstimmungen:",
        ["shared_result_expired"] = "Dieser Link ist abgelaufen.",
        ["shared_result_take_quiz"] = "Quiz selbst machen →",
        
        // Footer / Misc
        ["about_footer_joke"] = "Keine Wale wurden bei der Erstellung dieser Website verletzt. Ein Entwickler hat allerdings zu viel Kaffee getrunken.",
        
        // Gallery
        ["gallery_title"] = "Die Wale",
        ["gallery_intro"] = "Lerne alle 10 Walarten kennen, mit denen du dich vergleichen kannst.",
        ["gallery_description"] = "Entdecke alle Walarten im Wal-O-Mat Quiz: Orca, Blauwal, Buckelwal und mehr.",
        ["gallery_link"] = "Die Wale",
        ["gallery_take_quiz"] = "Quiz machen →",
        ["back_to_gallery"] = "← Zurück zur Galerie",
        ["whale_not_found"] = "Dieser Wal wurde nicht gefunden."
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
        ["splash_intro"] = "14 questions. 10 whales. 1 truth. Ready to discover which marine mammal lives inside you?",
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
        ["about_tech_text"] = "This project was built with <strong>Blazor WebAssembly</strong> and hosted on <strong>Azure Static Web Apps</strong>. Source code is available on <a href=\"https://github.com/ronfleming/wal-o-mat-site\" target=\"_blank\" rel=\"noopener\">GitHub</a>.",
        ["about_privacy_title"] = "Privacy & Analytics",
        ["about_privacy_intro"] = "We use Azure Application Insights to understand website usage (e.g., page views, geographic region).",
        ["about_privacy_no_cookies"] = "No tracking cookies",
        ["about_privacy_anonymous"] = "IP addresses are anonymized",
        ["about_privacy_retention"] = "Data is deleted after 90 days",
        ["about_privacy_no_sharing"] = "No data sharing with third parties",
        ["about_privacy_rights"] = "For privacy questions or to exercise your rights (access, deletion), contact us via GitHub.",
        
        // Quiz
        ["quiz_loading"] = "Loading questions...",
        ["quiz_loading_1"] = "Consulting the whales...",
        ["quiz_loading_2"] = "Echolocation in progress...",
        ["quiz_loading_3"] = "The whales are still arguing about you...",
        ["quiz_loading_4"] = "A narwhal had to polish their horn.",
        ["quiz_question"] = "Question",
        ["quiz_of"] = "of",
        ["quiz_subtext_1"] = "Don't worry, we're judging you. A little.",
        ["quiz_subtext_2"] = "Your therapist would love this.",
        ["quiz_subtext_3"] = "Almost done! (That's a lie.)",
        ["quiz_subtext_4"] = "Cheating is allowed. We just see it.",
        ["quiz_subtext_5"] = "Don't overthink it. Unless you're a sperm whale.",
        ["quiz_subtext_6"] = "The whales are watching.",
        ["quiz_subtext_7"] = "Take a deep breath. Like a blue whale.",
        ["quiz_subtext_8"] = "Your inner whale is getting impatient.",
        ["quiz_subtext_9"] = "This answer defines you. No pressure.",
        ["quiz_subtext_10"] = "Trust your gut. Or your flipper.",
        ["quiz_subtext_11"] = "Just a few more. Promise. Maybe.",
        ["quiz_subtext_12"] = "Self-awareness is the first step. To what? No idea.",
        ["quiz_subtext_13"] = "The tension rises. Like a narwhal's horn.",
        ["quiz_subtext_14"] = "Final stretch! (Okay, almost final.)",
        ["quiz_subtext_15"] = "Somewhere, a whale is excited to meet you.",
        ["quiz_subtext_16"] = "You're doing great. For a human.",
        ["quiz_agree"] = "Agree",
        ["quiz_agree_tooltip"] = "Yes! That's so me!",
        ["quiz_neutral"] = "Neutral",
        ["quiz_neutral_tooltip"] = "I'm not committing to anything.",
        ["quiz_disagree"] = "Disagree",
        ["quiz_disagree_tooltip"] = "Absolutely not.",
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
        ["results_intro"] = "After intensive analysis by our highly qualified team of marine biologists (read: an algorithm and lots of coffee), it's official...",
        ["results_restart"] = "Again! (Be honest this time)",
        ["results_home"] = "Back to home",
        ["results_match_90"] = "Are you sure you're not actually a whale?",
        ["results_match_70"] = "Soul relatives. Definitely.",
        ["results_match_50"] = "You'd get along at a party.",
        ["results_match_30"] = "Distant acquaintances. Polite nods.",
        ["results_match_low"] = "You both... like water?",
        
        // Dugong Easter Egg (all neutral answers)
        ["dugong_title"] = "You're a... Dugong?",
        ["dugong_description"] = "You're... not even a whale. You're a Dugong. This happens when you can't make decisions. Dugongs are the 'I don't know, you pick' friends of the ocean.",
        ["dugong_traits"] = "Indecisive · Goes with the Flow · Avoids Conflict · Chill",
        
        // Sharing
        ["share_results"] = "Share your results",
        ["share_results_alt_1"] = "Make your friends jealous",
        ["share_results_alt_2"] = "Send the proof",
        ["share_saving"] = "Saving...",
        ["share_success"] = "Link created! Share it with your friends:",
        ["share_copy"] = "Copy link",
        ["share_copied"] = "Copied! ✓",
        ["share_error"] = "Sharing failed. Please try again.",
        ["shared_result_title"] = "My Whale",
        ["shared_result_top"] = "Your top match was:",
        ["shared_result_other_matches"] = "Other top matches:",
        ["shared_result_expired"] = "This link has expired.",
        ["shared_result_take_quiz"] = "Take the quiz yourself →",
        
        // Footer / Misc
        ["about_footer_joke"] = "No whales were harmed in the making of this website. One developer did have too much coffee, though.",
        
        // Gallery
        ["gallery_title"] = "The Whales",
        ["gallery_intro"] = "Meet all 10 whale species you can match with.",
        ["gallery_description"] = "Discover all whale species in the Wal-O-Mat quiz: Orca, Blue Whale, Humpback, and more.",
        ["gallery_link"] = "The Whales",
        ["gallery_take_quiz"] = "Take the Quiz →",
        ["back_to_gallery"] = "← Back to Gallery",
        ["whale_not_found"] = "This whale was not found."
    };
}


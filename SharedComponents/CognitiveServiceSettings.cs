
using Microsoft.CognitiveServices.Speech;

namespace SharedComponents;

public class CognitiveServiceSettings
{
    public Uri Endpoint { get; set; }
    public string Key { get; set; }
    public string Region { get; set; }

}
public  abstract class LanguageServiceSettings: CognitiveServiceSettings
{
    public virtual string Language {  get; set; }
}

public class TextServiceSettings : LanguageServiceSettings { };
public class SpeechServiceSettings: LanguageServiceSettings
{
    public SpeechConfig SpeechConfig
    {
        get =>
              SpeechConfig.FromSubscription(Key, Region);
    }
}

public class SpeechTranslationSettings : LanguageServiceSettings
{
    public SpeechTranslationConfig SpeechTranslationConfig
    {
        get 
        {
                var config = SpeechTranslationConfig.FromSubscription(Key, Region);
                config.SpeechRecognitionLanguage = Language;
                foreach(var lang in ToLanguage)
                {
                    config.AddTargetLanguage(lang);
                }
                return config;
                
        }
            
    }
    public List<string> ToLanguage {get;set;}
}


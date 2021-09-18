
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


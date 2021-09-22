
using SharedComponents;

namespace Microsoft.Extensions.Configuration;
public static class ConfigurationCustomExtensions
{

    public static IConfigurationBuilder AddMySpeechConfig(this IConfigurationBuilder builder)
        => builder
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true);

    public static TextServiceSettings GetTextServiceSettings(this IConfiguration config) =>
        new TextServiceSettings()
        {
            Endpoint = new Uri(config.GetSection("TextAnalyticsService:Endpoint").Value),
            Key = config.GetSection("TextAnalyticsService:Key").Value,
            Region = config.GetSection("TextAnalyticsService:Region").Value,
            Language = config.GetSection("TextAnalyticsService:ToLanguage").Value,
        };
    public static SpeechServiceSettings GetSpeechServiceSettings(this IConfiguration config) =>
        new SpeechServiceSettings()
        {
            Endpoint = new Uri(config.GetSection("SpeechService:Endpoint").Value),
            Key = config.GetSection("SpeechService:Key").Value,
            Region = config.GetSection("SpeechService:Region").Value,
            Language = config.GetSection("SpeechService:ToLanguage").Value,
        };
}

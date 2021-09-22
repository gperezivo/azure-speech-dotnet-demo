using SharedComponents;
using Azure.AI.TextAnalytics;

namespace speaker_recognition;

public class MyTextAnalyticsService
{
    private readonly TextServiceSettings _settings;
    private readonly TextAnalyticsClient _client;
    public MyTextAnalyticsService(TextServiceSettings settings)
    {
        _settings = settings;
        _client = new TextAnalyticsClient(
            _settings.Endpoint, 
            new Azure.AzureKeyCredential(_settings.Key)
        );
    }

    public async Task<IEnumerable<string>> GetKeyPhrases(string document)
    {
        var response = await _client.ExtractKeyPhrasesAsync(document);
        return response.Value.Select(x=>x.ToString());
    }
    public async Task<string> GetFirstPersonEntity(string document)
    {

        var response = await _client.RecognizeEntitiesAsync(document).ConfigureAwait(false);
        var name = response?
            .Value?
            .Where(x => x.Category == EntityCategory.Person)?
            .OrderBy(x => x.Offset)?
            .FirstOrDefault();

        return name?.Text ?? "Unknown User";
        
        

    }

}

using SharedComponents;
using System.Text;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Speaker;

namespace speaker_recognition;
public class MySpeechService
{
    private readonly SpeechServiceSettings settings;
    

    public MySpeechService(SpeechServiceSettings settings)
    {
        this.settings = settings;
        
    }

    /// <summary>
    /// Transcribe a texto el fichero wav 
    /// </summary>
    /// <param name="path">Ruta del fichero wav</param>
    /// <returns>Transcripción de la voz (voces) del fichero wav</returns>
    public async Task<string> WavFileToText(string path)
    {
        var text = string.Empty;
        var audioConfig = AudioConfig.FromWavFileInput(path);
        using var speechRecognizer = new SpeechRecognizer(settings.SpeechConfig, settings.Language, audioConfig);
        
        var result = await speechRecognizer.RecognizeOnceAsync();
        
        return result.Text;

    }

    public async Task<VoiceProfile> RestCreateProfileAsync(VoiceProfileType type)
    {
        var endpoint = $"https://{settings.Region}.api.cognitive.microsoft.com/speaker/identification/v2.0/text-independent/profiles";
        using var httpClient = new HttpClient();

        using var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint);
        
        requestMessage.Content = new StringContent(
            Newtonsoft.Json.JsonConvert.SerializeObject(
                new
                {
                    locale = settings.Language.ToLower()
                }
            ),
            Encoding.UTF8,
            "application/json"
        );
        requestMessage.Headers.Add("Ocp-Apim-Subscription-Key", settings.Key);
        var result = await httpClient.SendAsync(requestMessage);
        if (result.IsSuccessStatusCode)
        {
            var content = await result.Content.ReadAsStringAsync();
            var profileId = Newtonsoft.Json.Linq.JObject.Parse(content)["profileId"].ToString();
            return new VoiceProfile(profileId,type);


        }
        else
            return null;
        
        
    }

}


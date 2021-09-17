using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", optional: true)
    .Build();

var speechServiceEndpoint = new Uri(config.GetSection("SpeechService:Endpoint").Value);
var speechServiceKey= config.GetSection("SpeechService:Key").Value;
var speechServiceRegion = config.GetSection("SpeechService:Region").Value;
var language = config.GetSection("SpeechService:ToLanguage").Value;

var speechConfig = SpeechConfig.FromSubscription(speechServiceKey, speechServiceRegion);
var audioConfig = AudioConfig.FromDefaultMicrophoneInput();

var stopRecognition = new TaskCompletionSource<int>();

using var speechRecognizer = new SpeechRecognizer(speechConfig,language,audioConfig);

speechRecognizer.SessionStopped += (sender, @event) =>
{
    stopRecognition.TrySetResult(0);
};
speechRecognizer.Canceled += (sender, @event) =>
{
    stopRecognition.TrySetResult(0);
};

speechRecognizer.Recognized += (sender, @event) =>
{
    var reason = @event?.Result?.Reason ?? ResultReason.NoMatch;
    if (reason == ResultReason.RecognizedSpeech)
    {
        Console.WriteLine(@event.Result?.Text);
    }
};

await speechRecognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

Task.WaitAll(new[]
{
    stopRecognition.Task
});

await speechRecognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);

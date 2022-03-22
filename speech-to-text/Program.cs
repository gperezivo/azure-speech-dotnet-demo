using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

var config = new ConfigurationBuilder()
    .AddMySpeechConfig()
    .Build();

var speechSettings = config.GetSpeechServiceSettings();
var translationSettings = config.GetSpeechTranslationConfig();
var speechConfig = speechSettings.SpeechConfig;
var translationConfig = translationSettings.SpeechTranslationConfig;
var audioConfig = AudioConfig.FromDefaultMicrophoneInput();

var stopRecognition = new TaskCompletionSource<int>();
var stopTranslate = new TaskCompletionSource<int>();
using var speechRecognizer = new SpeechRecognizer(speechConfig,speechSettings.Language,audioConfig);
using var speechTranslation = new TranslationRecognizer(translationSettings.SpeechTranslationConfig,audioConfig);

speechRecognizer.SessionStopped += (sender, @event) =>
{
    stopRecognition.TrySetResult(0);
};
speechRecognizer.Canceled += (sender, @event) =>
{
    stopRecognition.TrySetResult(0);
};
speechTranslation.SessionStopped += (sender, @event) =>
{
    stopTranslate.TrySetResult(0);
};
speechTranslation.Canceled += (sender, @event) =>
{
    stopTranslate.TrySetResult(0);
};

speechTranslation.Recognized += (sender, @event) =>
{
    Console.WriteLine($"Translated: {@event.Result.Text}");
    foreach(var result in @event.Result.Translations)
    {
        Console.ForegroundColor=ConsoleColor.Yellow;
        Console.WriteLine($"{result.Key}:{result.Value}");
        Console.ResetColor();
    }
};
speechRecognizer.Recognized += (sender, @event) =>
{
    var reason = @event?.Result?.Reason ?? ResultReason.NoMatch;
    if (reason == ResultReason.RecognizedSpeech)
    {
        Console.WriteLine($"{TimeSpan.FromTicks(@event.Result.OffsetInTicks)}: {@event.Result?.Text}");
    }
};
speechTranslation.StartContinuousRecognitionAsync();
speechRecognizer.StartContinuousRecognitionAsync();

Task.WaitAny(new[]
{
    stopRecognition.Task,
    stopTranslate.Task
});

await speechRecognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
await speechTranslation.StopContinuousRecognitionAsync().ConfigureAwait(false);

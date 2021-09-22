using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Speaker;
using Microsoft.Extensions.Configuration;
using NAudio.Wave;
using speaker_recognition;
using System.Collections.Concurrent;

Console.ForegroundColor = ConsoleColor.White; 
var config = new ConfigurationBuilder()
    .AddMySpeechConfig()
    .Build();

//Load configuration
var speechSettings = config.GetSpeechServiceSettings();
var textAnalyticsSettings = config.GetTextServiceSettings();
var speechConfig = speechSettings.SpeechConfig;

//Clases propias para pasar a texto la voz y obtener las entidades
//con estas clases obtenemos el nombre que dice la persona en el audio de presentación
var textAnalytics = new MyTextAnalyticsService(textAnalyticsSettings);
var speechService = new MySpeechService(speechSettings);


var recognizedNames = new Dictionary<string,(string name, VoiceProfile profile)>();
speechConfig.SpeechRecognitionLanguage = speechSettings.Language;

using var profileClient = new VoiceProfileClient(speechConfig);

var defaultColor = ConsoleColor.Gray;
foreach(var audio in Directory.GetFiles("./assets/train", "*.wav"))
 {

     Console.ForegroundColor = defaultColor;
     Console.WriteLine($"Analyzing text from {audio}");
     var document = await speechService.WavFileToText(audio);
     
     Console.WriteLine("Searching name...");
     var name = await textAnalytics.GetFirstPersonEntity(document);
     
    Console.WriteLine($"First name in {audio}: {name}");
     var profile = await profileClient.CreateProfileAsync(
         VoiceProfileType.TextIndependentIdentification,
         speechSettings.Language.ToLower()
         );

     var result = await profileClient.EnrollProfileAsync(profile, AudioConfig.FromWavFileInput(audio));
     var speakerRecognizer = new SpeakerRecognizer(speechConfig, AudioConfig.FromWavFileInput(audio));

     if (result.Reason == ResultReason.EnrolledVoiceProfile)
     {
         var verificationModel = SpeakerVerificationModel.FromProfile(profile);
         var verificationResult = await speakerRecognizer.RecognizeOnceAsync(verificationModel);
         if (verificationResult.Reason == ResultReason.RecognizedSpeakers)
         {
             Console.ForegroundColor = ConsoleColor.Green;
             Console.WriteLine($"Profile {profile.Id} assigned to {name}");
             recognizedNames.Add(profile.Id, new(name, profile));
         }
         else
         {
             Console.ForegroundColor = ConsoleColor.Red;
             if (verificationResult.Reason == ResultReason.Canceled)
             {
                 Console.WriteLine(SpeakerRecognitionCancellationDetails.FromResult(verificationResult).Reason);
                 Console.WriteLine(SpeakerRecognitionCancellationDetails.FromResult(verificationResult).ErrorDetails);
             }
             Console.WriteLine("Failed on profile verification");
             await profileClient.DeleteProfileAsync(profile);
         }
     }
     else
     {
         Console.ForegroundColor = ConsoleColor.Red;
         Console.WriteLine($"Invalid enroll request: {result.Reason}");
         await profileClient.DeleteProfileAsync(profile);
     }

 }


var profiles = recognizedNames.Values.Select(p => p.profile).ToList();
var model = SpeakerIdentificationModel.FromProfiles(profiles);
foreach(var voiceFile in Directory.GetFiles("./assets/recognize", "*.wav"))
{

    var text = await speechService.WavFileToText(voiceFile);
    Console.ForegroundColor = defaultColor;
    Console.WriteLine("Analyzing key phrases");
    var keyPhrases = await textAnalytics.GetKeyPhrases(text);
    
    
    Console.WriteLine($"Recognizing speakers in {voiceFile}");
    using var speakerRecognizer = new SpeakerRecognizer(
        speechConfig,
        AudioConfig.FromWavFileInput(voiceFile)
    );
    var result = await speakerRecognizer.RecognizeOnceAsync(model);
    Console.ForegroundColor = ConsoleColor.DarkYellow;
    Console.WriteLine(result.ToString());
    if(result.Reason == ResultReason.RecognizedSpeakers)
    {
        var recognizedName = recognizedNames.ContainsKey(result.ProfileId) ? recognizedNames[result.ProfileId].name : "Unknown Speaker";
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(
            $"{recognizedName} is talking about: {Environment.NewLine}\t"+
            $"{string.Join($"{Environment.NewLine}\t",keyPhrases??new string[] { })}"+
            $"{Environment.NewLine}in {voiceFile}"
        );
    }
    else
    {        
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Unable to recognize the speaker: {SpeakerRecognitionCancellationDetails.FromResult(result).ErrorDetails}");
    }
}
Console.ForegroundColor = defaultColor;

var allProfiles = await profileClient.GetAllProfilesAsync(VoiceProfileType.TextIndependentIdentification);
foreach(var profile in allProfiles)
{
    await profileClient.DeleteProfileAsync(profile);

}
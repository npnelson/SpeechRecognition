using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetToolBox.SpeechRecognition.Abstractions;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NetToolBox.SpeechRecognition.Azure
{
    internal sealed class AzureSpeechRecognizer : ISpeechRecognizer
    {
        private readonly IOptionsMonitor<AzureSpeechRecognizerSettings> _settingsOptionsMonitor;
        private readonly ILogger<AzureSpeechRecognizer> _logger;

        public AzureSpeechRecognizer(IOptionsMonitor<AzureSpeechRecognizerSettings> settingsOptionsMonitor, ILogger<AzureSpeechRecognizer> logger)
        {
            _settingsOptionsMonitor = settingsOptionsMonitor;
            _logger = logger;
        }
        public async Task<string> RecognizeSpeechFromPullWavStreamAsync(Stream audioStream)
        {
            //it looks like some of this could be made more async, but better stick with the microsoft sample for now and we won't be doing tons of recognition, so we should be safe. Also looks like the speech sdk itself could benefit from more async methods

            var sb = new StringBuilder();
            var settings = _settingsOptionsMonitor.CurrentValue;

            var config = SpeechConfig.FromSubscription(settings.SubscriptionKey, settings.Region);

            var stopRecognition = new TaskCompletionSource<int>();

            // Create an audio stream from a wav file.
            // Replace with your own audio file name.
            using (var audioInput = AzureSpeechHelpers.OpenWavFile(new BinaryReader(audioStream)))
            {
                // Creates a speech recognizer using audio stream input.
                using (var recognizer = new SpeechRecognizer(config, audioInput))
                {


                    recognizer.Recognized += (s, e) =>
                    {
                        if (e.Result.Reason == ResultReason.RecognizedSpeech)
                        {
                            sb.Append(e.Result.Text);
                        }
                        else if (e.Result.Reason == ResultReason.NoMatch)
                        {
                            _logger.LogInformation("Speech could not be recognized");
                        }
                    };

                    recognizer.Canceled += (s, e) =>
                    {
                        _logger.LogInformation($"CANCELED: Reason={e.Reason}");

                        if (e.Reason == CancellationReason.Error)
                        {
                            _logger.LogInformation($"CANCELED:  ErrorCode={e.ErrorCode} ErrorDetails={e.ErrorDetails}");

                        }

                        stopRecognition.TrySetResult(0);
                    };

                    recognizer.SessionStopped += (s, e) =>
                    {
                        _logger.LogInformation("Speech Recognition Session Stopped");
                        stopRecognition.TrySetResult(0);
                    };

                    // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
                    await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

                    // Waits for completion.
                    // Use Task.WaitAny to keep the task rooted.
                    Task.WaitAny(new[] { stopRecognition.Task });  //should this be await Task.WhenAny??

                    // Stops recognition.
                    await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                    return sb.ToString();
                }

            }
        }
    }
}

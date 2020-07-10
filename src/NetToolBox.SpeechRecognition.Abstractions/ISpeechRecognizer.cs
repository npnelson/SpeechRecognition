using System.IO;
using System.Threading.Tasks;

namespace NetToolBox.SpeechRecognition.Abstractions
{
    public interface ISpeechRecognizer
    {
        Task<string> RecognizeSpeechFromPullWavStreamAsync(Stream audioStream);
    }
}

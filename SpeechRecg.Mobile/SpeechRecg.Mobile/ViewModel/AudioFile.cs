using Plugin.AudioRecorder;

namespace SpeechRecg.Mobile.ViewModel
{
    public class AudioFile
    {
        public string PathFile { get; set; }
        public AudioRecorderService AudioRecorder { get; set; }
        public Plugin.SimpleAudioPlayer.ISimpleAudioPlayer AudioPlayer { get; set; }
    }
}
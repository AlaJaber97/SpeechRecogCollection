using System;
using System.Collections.Generic;
using System.Text;

namespace SpeechRecg.Mobile.AudioPlayer
{
    public interface IPlayService
    {
        int GetDuration(string fileName);
    }
}

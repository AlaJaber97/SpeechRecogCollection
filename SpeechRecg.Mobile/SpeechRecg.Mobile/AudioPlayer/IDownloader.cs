using System;
using System.Collections.Generic;
using System.Text;

namespace SpeechRecg.Mobile.DependenciesServices
{
    public interface IDownloader
    {
#pragma warning disable CA1054 // Uri parameters should not be strings
        void DownloadFile(string url, string pathfolder, string namefile);
#pragma warning restore CA1054 // Uri parameters should not be strings
        int ProgressPercentage { get; set; }
        event EventHandler<DownloadEventArgs> OnFileDownloaded;
    }
    public class DownloadEventArgs : EventArgs
    {
        public bool FileSaved { get; set; } = false;
        public DownloadEventArgs(bool fileSaved)
        {
            FileSaved = fileSaved;
        }
    }
}

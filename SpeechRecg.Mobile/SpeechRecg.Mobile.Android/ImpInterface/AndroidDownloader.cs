using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using SpeechRecg.Mobile.DependenciesServices;
using SpeechRecg.Mobile.Droid.DependencieServices;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidDownloader))]
namespace SpeechRecg.Mobile.Droid.DependencieServices
{
    public class AndroidDownloader : IDownloader
    {
        public event EventHandler<DownloadEventArgs> OnFileDownloaded;
        public int ProgressPercentage { get; set; }
        public void DownloadFile(string url, string pathfolder, string namefile)
        {
            string pathToNewFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), pathfolder);
            if(!Directory.Exists(pathToNewFolder))
                Directory.CreateDirectory(pathToNewFolder);

            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                string pathToNewFile = Path.Combine(pathToNewFolder, namefile);
                webClient.DownloadFileAsync(new Uri(url), pathToNewFile);
            }
            catch
            {
                if (OnFileDownloaded != null)
                    OnFileDownloaded.Invoke(this, new DownloadEventArgs(false));
            }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressPercentage = e.ProgressPercentage;
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                OnFileDownloaded?.Invoke(this, new DownloadEventArgs(false));
            }
            else
            {
                OnFileDownloaded?.Invoke(this, new DownloadEventArgs(true));
            }
        }
    }
}
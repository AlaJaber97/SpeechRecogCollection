using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SpeechRecg.Mobile.AudioPlayer;
using SpeechRecg.Mobile.Droid.ImpInterface;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidPlayService))]
namespace SpeechRecg.Mobile.Droid.ImpInterface
{
    public class AndroidPlayService : IPlayService
    {
        private MediaPlayer mediaPlayer;

        //string basepath = Xamarin.Essentials.FileSystem.CacheDirectory;
        public int GetDuration(string fileName)
        {
            mediaPlayer = new MediaPlayer();

            //string pathToNewFolder = Path.Combine(basepath, fileName);
            // Make sure this file is placed in the Android project's Assets folder with build action AndroidAsset.
            if (File.Exists(fileName))
            {
                //fileName = fileName.Split('/').Last();
                //AssetFileDescriptor descriptor = Android.App.Application.Context.Assets.OpenFd(fileName);
                //mediaPlayer.SetDataSource(descriptor.FileDescriptor, descriptor.StartOffset, descriptor.Length);
                mediaPlayer.SetDataSource(fileName);
                mediaPlayer.Prepare();
                var Duration = mediaPlayer.Duration;
                mediaPlayer.Release();
                mediaPlayer.Dispose();
                return Duration / 1000;
            }
            else
            {
                throw new Exception("File Not Exists or can not access to it");
            }
        }
    }
}
using System;
using System.IO;
using Android.App;
using Android.Media;
using SpeechRecg.Mobile.DependenciesServices;
using SpeechRecg.Mobile.Droid.DependencieServices;

[assembly: Xamarin.Forms.Dependency(typeof(WRSystem))]
namespace SpeechRecg.Mobile.Droid.DependencieServices
{
    public class WRSystem : IOSystem
    {
        string basepath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public double DurtionFile(string path)
        {
            MediaPlayer Player = new MediaPlayer();
            string audiopath = chackpath(path);
            Player.SetDataSource(audiopath);
            Player.Prepare();
            return Player.Duration;
        }
        public void DeleteFile(string pathfile)
        {
            string path = chackpath(pathfile);

            if (IsFileExists(path))
                File.Delete(path);
        }
        public void DeleteDirectory(string pathfolder)
        {
            string path = chackpath(pathfolder);

            if (Directory.Exists(path))
                Directory.Delete(path,true);
        }
        public bool IsFileExists(string pathfile)
        {
            string path = chackpath(pathfile);          

            if (Directory.Exists(GetPathDirectory(pathfile)))
                return File.Exists(path);
            else
                return false;
        }
        private string chackpath(string path)
        {
            string cpath;
            if (!path.Contains(basepath))
                cpath = Path.Combine(basepath, path);
            else
                cpath = path;
            return cpath;
        }
        private string GetPathDirectory(string PathFile)
        {
            var partOfPath = PathFile.Split('/');
            var pathDirec = string.Empty;
            foreach (var part in partOfPath)
                Path.Combine(pathDirec, part);
            return pathDirec;
        }
        public string GetFullPathFile(string pathFile)
        {                
            return Path.Combine(basepath, pathFile);
        }
    }
}
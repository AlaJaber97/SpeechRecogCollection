using System;
using System.Collections.Generic;
using System.Text;

namespace SpeechRecg.Mobile.DependenciesServices
{
    public interface IOSystem
    {
        bool IsFileExists(string path);
        double DurtionFile(string path);
        void DeleteFile(string pathfile);
        void DeleteDirectory(string pathfolder);
        string GetFullPathFile(string pathFile);
    }
}

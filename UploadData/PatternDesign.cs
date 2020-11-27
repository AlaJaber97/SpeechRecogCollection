using System;
using System.Collections.Generic;
using System.Text;
using static UploadData.EnumType;

namespace UploadData
{
    public static class PatternDesign
    {
        public static void PrintHead()
        {
            Console.Clear();
            Console.WriteLine($"==================== Welcome ==========================");
            Console.WriteLine($"=======================================================");
            Console.WriteLine($"================ Upload New File ======================");
        }
        public static void PrintGetSettingAudioFile(TypeUploadFile TypeUploadFile, string ForFolder)
        {
            Console.Clear();
            var type = TypeUploadFile == TypeUploadFile.Audio ? "Audio" : "Text" ;
            Console.WriteLine($"=========================================================");
            Console.WriteLine($"============== Entry Setting {type} File ================");
            if (!string.IsNullOrEmpty(ForFolder))
            {
                ForFolder = System.IO.Path.GetFileName(ForFolder);
                Console.WriteLine($"============== For Folder: {ForFolder}  ================");
            }
            Console.WriteLine($"=========================================================");
        }
        public static void PrintStartUploadFile()
        {
            Console.Clear();
            Console.WriteLine($"===========================================================");
            Console.WriteLine($"============== Upload Opration is Start Now ===============");
            Console.WriteLine($"===========================================================");
        }
        public static void PrintFinishUploadFile()
        {
            Console.Clear();
            Console.WriteLine($"===========================================================");
            Console.WriteLine($"============== Upload Opration is Finished ================");
            Console.WriteLine($"===========================================================");
        }
    }
}

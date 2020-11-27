using BLL.Sql.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static UploadData.EnumType;

namespace UploadData
{
    partial class Program
    {
        private static BLL.Sql.Models.AudioFile SettingAudioFile;
        private static BLL.Sql.Models.TransText SettingTextFile;

        private static string Token { get; set; }
        private static int CountUploaded = 0;
        static void Main(string[] args)
        {
            Task.WaitAll(LogIn());
            while (true)
            {
                TypeUploadFile TypeFileUpload;
                PatternDesign.PrintHead();
            EnterTypeFileUpload:
                try
                {
                    Console.Write("Select Type file upload { (1) audio, (2) text }: ");
                    if (int.TryParse(Console.ReadLine(), out int Input))
                    {
                        Input -= 1;
                        if (Input >= 0 && Input < 2)
                            TypeFileUpload = Enum.Parse<TypeUploadFile>($"{Input}");
                        else
                        {
                            throw new Exception("Worng Input, Try again");
                        }
                    }
                    else
                    {
                        throw new Exception("Worng Input, Try again");
                    }
                }
                catch(Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: {ex.ToString()}\n");
                    Console.ResetColor();
                    RemoveLastLine();
                    goto EnterTypeFileUpload;
                }
            EntryPath:
                try
                {
                    Console.Write("PathFiles/PathDirectory: ");
                    string PathDirectory = Console.ReadLine();
                    switch (TypeFileUpload) 
                    {
                        case TypeUploadFile.Audio:
                            var IsContentFiles = Directory.GetFiles(PathDirectory, "*.wav").Count() > 0 ||
                       Directory.GetDirectories(PathDirectory).Any(PathSubDirectory =>
                       Directory.GetFiles(PathSubDirectory, "*.wav").Count() > 0);
                            if (!IsContentFiles)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Wrong path, no wav files in the given path\n");
                                Console.ResetColor();
                                goto EntryPath;
                            }
                            GetAudioFrom(PathDirectory);
                            break;
                        case TypeUploadFile.Text:
                            if (!Path.HasExtension(PathDirectory))
                                PathDirectory = Directory.GetFiles(PathDirectory, "*.txt")?.Single();
                            if (PathDirectory == null)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Wrong path, no text file in the given path\n");
                                Console.ResetColor();
                                goto EntryPath;
                            }
                            GetTextFrom(PathDirectory);
                            break;
                        default:
                            throw new Exception("Worng Input, Try again");
                    }
                }
                catch(Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error: {ex}\n");
                    Console.ResetColor();
                    goto EntryPath;
                }
            }
        }

        private static async Task LogIn()
        {
        TryAgain:
            try
            {
                Console.Write("Enter your Email: ");
                string email = Console.ReadLine();
                if (email.ToLower() != "Info@YUlivetranslation.com".ToLower())
                    throw new Exception("Worng Email");
                var signIn = new BLL.Sql.Models.LoginAndRegister.Login { Email = email };
                var response = await new BLL.Services.HttpExtension<BLL.Sql.Models.LoginAndRegister.Login>().PostReturnStatusCodeAndString("Account/Login", signIn, null);
                if (response.statusCode == System.Net.HttpStatusCode.OK)
                {
                    Token = response.message;
                    Console.WriteLine("Login Successfully");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error : " + response.message);
                    Console.WriteLine("Login Unsuccessful");
                    Console.ResetColor();
                    goto TryAgain;
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex}\n");
                Console.ResetColor();
                goto TryAgain;
            }
        }
        private static void GetSettings(TypeUploadFile TypeUploadFile, string ForFolder="")
        {
            try
            {
                PatternDesign.PrintGetSettingAudioFile(TypeUploadFile, ForFolder);
                if (TypeUploadFile == TypeUploadFile.Audio)
                {
                EnterGender:
                    try
                    {
                        SettingAudioFile = new BLL.Sql.Models.AudioFile();
                        Console.Write("    Select Gender { (1) male, (2) female } : ");
                        SettingAudioFile.SpeakerGender = Enum.Parse<BLL.Enum.Gender>($"{int.Parse(Console.ReadLine()) - 1}");
                    }
                    catch
                    {
                        RemoveLastLine();
                        goto EnterGender;
                    }

                    Console.WriteLine();
                EnterAge:
                    try
                    {
                        Console.Write("    Select Age {number}: ");
                        SettingAudioFile.SpeakerAge = int.Parse(Console.ReadLine());
                    }
                    catch
                    {
                        RemoveLastLine();
                        goto EnterAge;
                    }
                    Console.WriteLine();

                EnterResevedFor:
                    try
                    {
                        Console.Write("    Select Reserved For {email, if empty press Enter}: ");
                        
                        var EmailUser = Console.ReadLine().ToLower();
                        if (!string.IsNullOrWhiteSpace(EmailUser) && !BLL.Constants.InfoAdmin.EmailAdmin.Any(email => string.Equals(email.ToLower(), EmailUser.ToLower())))
                            throw new Exception("Please, Insert Correct Admin Email Only");
                        SettingAudioFile.ReservedFor = EmailUser;
                    }
                    catch
                    {
                        RemoveLastLine();
                        goto EnterResevedFor;
                    }
                }
                else if (TypeUploadFile == TypeUploadFile.Text)
                {
                    SettingTextFile = new BLL.Sql.Models.TransText();
                    Console.Write("    Select Gender { (1) hybrid, (2) fashaa } : ");
                    SettingTextFile.Dialect = Enum.Parse<BLL.Enum.Dialect>($"{int.Parse(Console.ReadLine()) - 1}");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.ToString()}\n");
                Console.ResetColor();
            }

        }
        private static void GetTextFrom(string path)
        {
            try
            {
                GetSettings(EnumType.TypeUploadFile.Text);
                var Lines = File.ReadAllLines(path).ToList();
                var list = new List<BLL.Sql.Models.TransText>();
                foreach (var text in Lines)
                {
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        list.Add(new BLL.Sql.Models.TransText
                        {
                            ID = Guid.NewGuid(),
                            Text = Normalization(text),
                            Status = BLL.Enum.Status.Available,
                            Dialect = SettingTextFile.Dialect
                        });
                    }
                }
                Task.WaitAll(UploadText(list, Lines.Count));
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex}\n");
                Console.ResetColor();
            }
        }
        private static string Normalization(string text)
        {
            //Remove non-arabic char
            text = Regex.Replace(text, @"[^\u0621-\u063A\u0641-\u064A\u0020]+", " ");
            //Remove Dublicate Char
            //text = Regex.Replace(text, "(.)\\1+", "$1");

            //Normliz Hamza - Alef - Yeh - Heh
            //var normlizeHamza = Regex.Replace(s, @"[\u0624\u0626]", "\u0621");
            var normlizeAlef = Regex.Replace(text, @"[\u0622\u0623\u0625]", "\u0627");
            //var normlizeYeh = Regex.Replace(normlizeAlef, @"\u064A ", "\u0649 ");
            //var normlizeHeh = Regex.Replace(normlizeAlef, @"\u0647 ", "\u0629 ");
            //Remove any word less then 3 char
            //text = Regex.Replace(normlizeHeh, @"\b[\u0621-\u063A\u0641-\u064A\u0020]{2}\b", " ");
            //Remove Dublicate Space
            var pattern = new Regex("[\t\r]|[ ]{2,}");
            text = pattern.Replace(normlizeAlef, " ");
            text = text.Trim();
            return text;
        }
        private static void GetAudioFrom(string PathDirectory)
        {
            try
            {
                var PathSubDirectories = Directory.GetDirectories(PathDirectory);
                if (PathSubDirectories != null && PathSubDirectories.Length > 0)
                {
                    foreach (var PathSubDirectory in PathSubDirectories)
                    {
                        GetSettings(EnumType.TypeUploadFile.Audio, PathSubDirectory.Split('\\').Where(x => !string.IsNullOrWhiteSpace(x)).Last());
                        CallUploadFiles(Directory.GetFiles(PathSubDirectory, "*.wav").ToList());
                    }
                }
                else
                {
                    GetSettings(EnumType.TypeUploadFile.Audio, PathDirectory.Split('\\').Where(x=> !string.IsNullOrWhiteSpace(x)).Last());
                    CallUploadFiles(Directory.GetFiles(PathDirectory, "*.wav").ToList());
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.ToString()}\n");
                Console.ResetColor();
            }

        }
        private static void CallUploadFiles(List<string> PathFiles)
        {
            try
            {
                Console.WriteLine();
                PatternDesign.PrintStartUploadFile();
                ProgressBar ProgressBar = new ProgressBar("Upload Audio File", PathFiles.Count, 50);
                foreach (var PathFile in PathFiles)
                {
                    SettingAudioFile.Status = BLL.Enum.Status.Available;
                    SettingAudioFile.ID = Guid.NewGuid();
                    SettingAudioFile.Length = TimeSpan.FromSeconds(SoundOpration.GetSoundLength(PathFile));
                    var ModelInfoFile = SettingAudioFile;
                    Task.WaitAll(UploadFile(PathFile, ModelInfoFile.ID, ModelInfoFile, PathFiles.Count));
                    ProgressBar.Update(CountUploaded);
                }
                PatternDesign.PrintFinishUploadFile();
                //reset count upload file
                CountUploaded = 0;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex}\n");
                Console.ResetColor();
            }
        }
        private static async Task UploadFile(string path, Guid id, BLL.Sql.Models.AudioFile model,int CountFiles)
        {
            try
            {
                Stream stream = File.OpenRead(path);
                var result = await BLL.Services.UploadFile.UploadAudioAsync(id, Token, stream, "Console");
                if (result.statusCode == System.Net.HttpStatusCode.Created)
                {
                    var response = await new BLL.Services.HttpExtension<BLL.Sql.Models.AudioFile>().PostReturnStatusCodeAndString("AudioFiles", model, Token);
                    if (response.statusCode == System.Net.HttpStatusCode.OK)
                    {
                        CountUploaded++;
                        //Console.WriteLine($"Uploaded audio file {countuploaded} out of {CountFiles}");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: " + result.message);
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.ToString()}\n");
                Console.ResetColor();
            }
        }
        private static async Task UploadText(List<BLL.Sql.Models.TransText> TransTexts, int Countlines)
        {
            try
            {
                PatternDesign.PrintStartUploadFile();
                //ProgressBar ProgressBar = new ProgressBar("Upload Text Lines", Countlines, 50);
                var response = await new BLL.Services.HttpExtension<List<BLL.Sql.Models.TransText>>().PostReturnStatusCodeAndString("TransTexts", TransTexts, Token);
                if (response.statusCode == System.Net.HttpStatusCode.Created)
                {
                    //Console.WriteLine($"Uploaded audio file {Countlines} out of {Countlines}");
                }
                PatternDesign.PrintFinishUploadFile();
                CountUploaded = 0;
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.ToString()}\n");
                Console.ResetColor();
            }
        }
        private static void RemoveLastLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new String(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, Console.CursorTop);
        }
    }
}